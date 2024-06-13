# Saving a file

Another project from old university days where we started learning about Dependency Injection.

```c#
public class FileSaver
{
    private readonly IEnumerable<IPdfConverter> _pdfConverters;
    private readonly IStorageFileService _fileService;
    private readonly IVirusScanner _virusScanner;
    private readonly IPublisherChannelFacade _publisherChannel;
    private readonly FileSavedEventIdentifier _savedIdentifier;
    private readonly IValidator<FileValidationContext> _fileValidator;
    private readonly ILogger<FileSaver> _logger;

    public FileSaver(
        IEnumerable<IPdfConverter> pdfConverters,
        IStorageFileService fileService,
        IVirusScanner virusScanner,
        IPublisherChannelFacade publisherChannel,
        FileSavedEventIdentifier savedIdentifier,
        IValidator<FileValidationContext> fileValidator,
        ILogger<FileSaver> logger)
    {
        _pdfConverters = pdfConverters;
        _fileService = fileService;
        _virusScanner = virusScanner;
        _publisherChannel = publisherChannel;
        _savedIdentifier = savedIdentifier;
        _fileValidator = fileValidator;
        _logger = logger;
    }

    public async Task<bool> SaveFileAsync(Guid documentId, string declaredMimeType, TempFile tempFile, CancellationToken cancellationToken)
    {
        // First step: Virus scanning
        var virusScanningResult = await _virusScanner.ScanAsync(tempFile, cancellationToken);
        if (!virusScanningResult.IsClean)
        {
            _logger.LogError("Virus scanning failed or detected threat for document {DocumentId}", documentId);
            return false;
        }

        // Second step: MIME type detection
        var mimeTypeResult = MimeType.FromTempFile(tempFile);
        if (mimeTypeResult.IsError)
        {
            _logger.LogError("Could not determine MIME type for document {DocumentId}", documentId);
            return false;
        }

        // Third step: File validation
        var validationContext = new FileValidationContext
        {
            SizeBytes = tempFile.SizeBytes,
            DeclaredMimeType = declaredMimeType,
            ActualMimeType = mimeTypeResult.Value,
            VirusScanningResult = virusScanningResult
        };

        var validationResult = await _fileValidator.ValidateAsync(validationContext, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Validation failed for document {DocumentId}", documentId);
            return false;
        }

        // Fourth step: Convert to PDF
        var converter = GetConverter(declaredMimeType);
        if (converter is null)
        {
            _logger.LogError("No suitable PDF converter found for MIME type {MimeType}", declaredMimeType);
            return false;
        }

        TempFile pdfFile;
        try
        {
            pdfFile = await converter.ToPdfAsync(tempFile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PDF conversion failed for document {DocumentId}", documentId);
            return false;
        }

        // Fifth step: Save PDF file
        bool saveSuccess;
        try
        {
            saveSuccess = await _fileService.SaveFileAsync(documentId, pdfFile, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Saving file failed for document {DocumentId}", documentId);
            return false;
        }
        finally
        {
            if (pdfFile != tempFile)
                pdfFile.Delete();
        }

        if (!saveSuccess)
        {
            _logger.LogError("File service reported unsuccessful save for document {DocumentId}", documentId);
            return false;
        }

        // Sixth step: Publish saved event
        bool published = await _publisherChannel.BasicPublishAsync(_savedIdentifier, new { DocumentId = documentId }, cancellationToken);
        if (!published)
        {
            _logger.LogError("Event publishing failed for document {DocumentId}", documentId);
            // Note: Does not return false here, causing inconsistency in handling errors
        }

        return true;
    }

    private IPdfConverter? GetConverter(string mimeType)
    {
        foreach (var converter in _pdfConverters)
        {
            if (converter.CanConvert(mimeType))
                return converter;
        }
        return null;
    }
}
```

Unmaintainable hell - by design when saving a file we want to perform a sequence of operations(even now this sounds like a pipeline - even when thinking about the sequence). But for some reason the decision was made to create onw giant method with all dependencies injected as the most important factor. Because by design we model a sequence of events before we save the actual file, the best abstraction from my side for would be a pipeline which can be again built and adjusted in any time. Each of our component will do exacly one things and thus will be more testable. This brings us to the root component

```c#
public interface IFileSaveHandler
{
    ValueTask<OneOf<Success, ValidationFailed, FileNotSaved, IOError>> HandleSaveAsync(FileSaveRequest request, CancellationToken cancellationToken);

    void SetNext(IFileSaveHandler next);
}
```

we typically have a chain of such handlers on the third level, which perform their tasks and can short circuit anytime. Here our "code" exacly follows the design. 1 to 1

```c#
public abstract class BaseSaveHandler : IFileSaveHandler
{
    private IFileSaveHandler? _next;
    public async ValueTask<OneOf<Success, ValidationFailed, FileNotSaved, IOError>> HandleSaveAsync(FileSaveRequest request, CancellationToken cancellationToken)
    {
        var result = await HandleSaveInternalAsync(request, cancellationToken);

        if (_next is null || !result.IsT0) return result;

        return await _next.HandleSaveAsync(request, cancellationToken);
    }

    protected abstract ValueTask<OneOf<Success, ValidationFailed, FileNotSaved, IOError>> HandleSaveInternalAsync(FileSaveRequest request, CancellationToken cancellationToken);

    public void SetNext(IFileSaveHandler next) => _next = next;
}
```

and each handler is responsible only for one thing

```c#
public class StorageSaveHandler(IEnumerable<IPdfConverter> pdfConverters, IStorageFileService fileService, ILogger<StorageSaveHandler> logger) : BaseSaveHandler
{
    protected override async ValueTask<OneOf<Success, ValidationFailed, FileNotSaved, IOError>> HandleSaveInternalAsync(FileSaveRequest request, CancellationToken cancellationToken)
    {
        var converter = pdfConverters.First(c => c.CanConvert(request.DeclaredMimeType));

        var pdfConversionResult = await converter.ToPdfAsync(request.TempFile);

        if (pdfConversionResult.IsT1) return pdfConversionResult.AsT1;

        var pdfTempFile = pdfConversionResult.AsT0;

        var saveResult = await fileService.SaveFileAsync(request.DocumentId, pdfTempFile, cancellationToken);

        if (pdfTempFile != request.TempFile) pdfTempFile.Delete();

        return saveResult.Match<OneOf<Success, ValidationFailed, FileNotSaved, IOError>>(success => success, error => error);
    }
}


public class ValidationHandler(IVirusScanner virusScanner, IValidator<FileValidationContext> fileValidator, ILogger<ValidationHandler> logger) : BaseSaveHandler
{
    protected override async ValueTask<OneOf<Success, ValidationFailed, FileNotSaved, IOError>> HandleSaveInternalAsync(FileSaveRequest request, CancellationToken cancellationToken)
    {
        var virusScanningResult = await virusScanner.ScanAsync(request.TempFile, cancellationToken);

        if (virusScanningResult.IsT1) return new IOError();

        var mimeTypeResult = MimeType.FromTempFile(request.TempFile);

        if (mimeTypeResult.IsT1)
        {
            logger.LogError("Failed to determine MIME type for file {@TempFile}", request.TempFile);
            return new IOError();
        }

        var validationContext = new FileValidationContext
                                {
                                    SizeBytes = request.TempFile.SizeBytes,
                                    DeclaredMimeType = request.DeclaredMimeType,
                                    ActualMimeType = mimeTypeResult.AsT0,
                                    VirusScanningResult = virusScanningResult.AsT0
                                };

        var validationResult = await fileValidator.ValidateAsync(validationContext, cancellationToken);

        return !validationResult.IsValid ? validationResult.ToValidationFailed() : new Success();
    }
}


public class PerformIndexHandler(IPublisherChannelFacade publisherChannel, FileSavedEventIdentifier savedIdentifier, ILogger<PerformIndexHandler> logger) : BaseSaveHandler
{
    protected override async ValueTask<OneOf<Success, ValidationFailed, FileNotSaved, IOError>> HandleSaveInternalAsync(FileSaveRequest request, CancellationToken cancellationToken)
    {
        var published = await publisherChannel.BasicPublishAsync(savedIdentifier, new { request.DocumentId }, cancellationToken);

        if (!published) logger.LogError("Failed to publish saved event for document {@DocumentId}", request.DocumentId);

        return new Success();
    }
}
```

Finally we can build our save pipeline and add it as a separate entity to our favourite DI container. 3rd level thinking first! This iteration took much less time, probably a couple of hours, but learned to treat sequence of some steps as pipeline approach - implementation can look differently, that's true, but the idea is the most import thing.