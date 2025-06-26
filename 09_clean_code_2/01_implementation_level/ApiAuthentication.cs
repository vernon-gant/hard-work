// BEFORE

public class ApiAuthenticationService : IApiAuthenticationService
{
    private readonly ICustomerService _customerService;
    private readonly IGroupService _groupService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiAuthenticationService(
        ICustomerService customerService,
        IGroupService groupService, IHttpContextAccessor httpContextAccessor)
    {
        _customerService = customerService;
        _groupService = groupService;
        _httpContextAccessor = httpContextAccessor;
    }

    public virtual async Task<Customer> GetAuthenticatedCustomer()
    {
        Customer customer = null;
        if (_httpContextAccessor.HttpContext == null) return null;

        string authHeader = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization];
        if (string.IsNullOrEmpty(authHeader))
            return null;

        if (IsApiFrontAuthenticated())
        {
            customer = await ApiCustomer();
            return customer;
        }

        var authenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
        if (!authenticateResult.Succeeded)
            return null;

        var emailClaim = authenticateResult.Principal.Claims.FirstOrDefault(claim => claim.Type == "Email");
        if (emailClaim != null)
            customer = await _customerService.GetCustomerByEmail(emailClaim.Value);

        if (customer is not { Active: true } || customer.Deleted || !await _groupService.IsRegistered(customer))
            return null;

        return customer;
    }

    private bool IsApiFrontAuthenticated()
    {
        var endpoint = _httpContextAccessor.HttpContext.GetEndpoint();
        if (endpoint == null) return false;

        var authorizeAttributes = endpoint.Metadata.GetOrderedMetadata<AuthorizeAttribute>();
        return authorizeAttributes.Any(attr => attr.AuthenticationSchemes?.Contains(FrontendAPIConfig.AuthenticationScheme) == true);
    }


    private async Task<Customer> ApiCustomer()
    {
        Customer customer = null;
        var authResult = await _httpContextAccessor.HttpContext!.AuthenticateAsync(FrontendAPIConfig.AuthenticationScheme);
        if (!authResult.Succeeded)
            return await _customerService.GetCustomerBySystemName(SystemCustomerNames.Anonymous);

        var email = authResult.Principal.Claims.FirstOrDefault(x => x.Type == "Email")?.Value;
        if (email is null)
        {
            //guest
            var id = authResult.Principal.Claims.FirstOrDefault(x => x.Type == "Guid")?.Value;
            if (id != null) customer = await _customerService.GetCustomerByGuid(Guid.Parse(id));
        }
        else
            customer = await _customerService.GetCustomerByEmail(email);

        return customer;
    }
}

// AFTER

public class ApiAuthenticationService : IApiAuthenticationService
{
    private readonly ICustomerService _customerService;
    private readonly IGroupService _groupService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiAuthenticationService(ICustomerService customerService, IGroupService groupService, IHttpContextAccessor httpContextAccessor)
    {
        _customerService = customerService;
        _groupService = groupService;
        _httpContextAccessor = httpContextAccessor;
    }

    public virtual async Task<Customer> GetAuthenticatedCustomer()
    {
        var authenticationContext = new ApiAuthenticationContext(_customerService, _groupService, _httpContextAccessor);
        var authenticationResult = await new ApiAuthenticationValidator().ValidateAsync(authenticationContext);
        return (Customer)authenticationResult.Errors.First().CustomState;
    }
}

/// In the context of API authentication, the system must ensure that:
/// 1. The HTTP context retrieved from http context accessor is not null and return null customer in case of failure.
/// 2. The authorization header is set when rule 1 holds true and return null customer in case of failure.
/// 3. The validator must return API customer when the API front is authenticated with the check performed using the customer service and rule 2 holds true.
/// 4. The authentication must be successful when rule 2 holds true and return null customer in case of failure.
/// 5. The customer must be active when rule 4 holds true and return null customer in case of failure.
/// 6. The customer must not be deleted when rule 5 holds true and return null customer in case of failure.
/// 7. The customer must be registered with the check performed using the group service when rule 6 holds true and return null customer in case of failure.
/// 8. The validator must return email customer when the email claim retrieved using the http context accessor is set and rule 7 holds true.
/// 9. The validator must return null customer when rule 8 does not hold true.
public record ApiAuthenticationContext(ICustomerService CustomerService, IGroupService GroupService, IHttpContextAccessor HttpContextAccessor);

public class ApiAuthenticationValidator : AbstractValidator<ApiAuthenticationContext>
{
    private Customer _apiCustomer;
    private Customer _emailCustomer;

    public ApiAuthenticationValidator()
    {
        RuleFor(WholeContext).Cascade(CascadeMode.Stop)
            .Must(HaveNonNullHttpContext).WithState(NullCustomer)
            .Must(HaveSetAuthorizationHeader).WithState(NullCustomer)
            .OverridePropertyName(WholeContextPropertyName)
            .DependentRules(() =>
            {
                RuleFor(WholeContext).Must(ReturnCustomer).WhenAsync(IsApiFrontAuthenticated).WithState(ApiCustomer).OverridePropertyName(WholeContextPropertyName);

                RuleFor(WholeContext).Cascade(CascadeMode.Stop)
                    .MustAsync(AuthenticateUsingHttpContextSuccessfully).WithState(NullCustomer)
                    .MustAsync(HaveActiveNotDeletedAndRegisteredCustomer).WithState(NullCustomer)
                    .Must(ReturnCustomer).WhenAsync(CustomerEmailClaimIsSet, ApplyConditionTo.CurrentValidator).WithState(EmailCustomer)
                    .Must(ReturnCustomer).WithState(NullCustomer)
                    .OverridePropertyName(WholeContextPropertyName);
            });
    }

    private const string WholeContextPropertyName = "WholeContext";

    private static Expression<Func<ApiAuthenticationContext, ApiAuthenticationContext>> WholeContext => context => context;

    private static bool HaveNonNullHttpContext(ApiAuthenticationContext context) => context.HttpContextAccessor.HttpContext != null;

    private static bool HaveSetAuthorizationHeader(ApiAuthenticationContext context) => context.HttpContextAccessor.HttpContext!.Request.Headers.ContainsKey(HeaderNames.Authorization);

    private async Task<Customer> GetApiCustomer(IHttpContextAccessor httpContextAccessor, ICustomerService customerService)
    {
        var authResult = await httpContextAccessor.HttpContext!.AuthenticateAsync(FrontendAPIConfig.AuthenticationScheme);

        if (!authResult.Succeeded)
            return await customerService.GetCustomerBySystemName(SystemCustomerNames.Anonymous);

        var email = authResult.Principal.Claims.FirstOrDefault(x => x.Type == "Email")?.Value;
        var id = authResult.Principal.Claims.FirstOrDefault(x => x.Type == "Guid")?.Value;

        if (email is not null) return await customerService.GetCustomerByEmail(email);

        return id is not null ? await customerService.GetCustomerByGuid(Guid.Parse(id)) : null;
    }

    private async Task<bool> IsApiFrontAuthenticated(ApiAuthenticationContext context, CancellationToken _)
    {
        var isApiFrontAuthenticated = context.HttpContextAccessor.HttpContext?
                                          .GetEndpoint()?
                                          .Metadata
                                          .GetOrderedMetadata<AuthorizeAttribute>()
                                          .Any(attr => attr.AuthenticationSchemes?.Contains(FrontendAPIConfig.AuthenticationScheme) == true)
                                      ?? false;

        if (isApiFrontAuthenticated) _apiCustomer = await GetApiCustomer(context.HttpContextAccessor, context.CustomerService);

        return isApiFrontAuthenticated;
    }

    private async Task<bool> AuthenticateUsingHttpContextSuccessfully(ApiAuthenticationContext context, CancellationToken _)
    {
        var authenticateResult = await context.HttpContextAccessor.HttpContext!.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
        return authenticateResult.Succeeded;
    }

    private async Task<bool> HaveActiveNotDeletedAndRegisteredCustomer(ApiAuthenticationContext context, CancellationToken _) => _apiCustomer.Active && !_apiCustomer.Deleted && await context.GroupService.IsRegistered(_apiCustomer);

    private async Task<bool> CustomerEmailClaimIsSet(ApiAuthenticationContext context, CancellationToken _)
    {
        var emailClaim = context.HttpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(claim => claim.Type == "Email");

        if (emailClaim != null)
            _emailCustomer = await context.CustomerService.GetCustomerByEmail(emailClaim.Value);

        return _emailCustomer != null;
    }

    private static bool ReturnCustomer(ApiAuthenticationContext context) => false;

    private static object NullCustomer(ApiAuthenticationContext context) => null;

    private object ApiCustomer(ApiAuthenticationContext context) => _apiCustomer;

    private object EmailCustomer(ApiAuthenticationContext context) => _emailCustomer;
}