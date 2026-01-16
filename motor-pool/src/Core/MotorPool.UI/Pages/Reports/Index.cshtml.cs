using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MotorPool.UI.Pages.Reports;

public class IndexModel : PageModel
{
    public readonly Dictionary<string, string> TYPE_PAGE_MAP = new()
    {
        { "Vehicle mileage", "./VehicleMileage"}
    };

    public void OnGet() { }
}