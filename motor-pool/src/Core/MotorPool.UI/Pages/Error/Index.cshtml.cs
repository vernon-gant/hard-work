using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MotorPool.UI.Pages.Error;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel(ILogger<ErrorModel> logger) : PageModel
{

    private readonly ILogger<ErrorModel> _logger = logger;

    public string? RequestId { get; set; }

    public string ErrorMessage { get; set; } = "An unexpected error occurred.";

    public string ErrorType { get; set; } = "General Error";

    public string? ReturnLink { get; set; } = "/";

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public void OnGet(string? errorMessage, string? errorType)
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

        if (TempData["ErrorMessage"] is string tempDataErrorMessage) ErrorMessage = tempDataErrorMessage;

        if (TempData["ErrorType"] is string tempDataErrorType) ErrorType = tempDataErrorType;

        if (TempData["ReturnLink"] is string tempDataReturnLink) ReturnLink = tempDataReturnLink;
    }

}