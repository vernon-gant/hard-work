using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MotorPool.UI.Pages.Error;

public class Status : PageModel
{

    public string ErrorMessage { get; set; } = string.Empty;

    public new int StatusCode { get; set; }

    public void OnGet(int statusCode)
    {
        StatusCode = statusCode;
        ErrorMessage = statusCode switch
        {
            404 => "Sorry, the resource you are looking for could not be found.",
            _ => "An unexpected error occurred."
        };
    }

}