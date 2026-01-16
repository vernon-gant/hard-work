using Microsoft.AspNetCore.Mvc.RazorPages;

using MotorPool.Domain;
using MotorPool.Repository.Enterprise;
using MotorPool.Services.Geo.Models;
using MotorPool.Services.Geo.Services;
using MotorPool.Services.Manager;

namespace MotorPool.UI.Pages.Trips;

public class IndexModel(EnterpriseQueryRepository enterpriseQueryRepository, TripQueryService tripQueryService) : PageModel
{
    public List<Enterprise> ManagerEnterprises { get; set; } = new();

    public List<int> SelectedEnterpriseIds { get; set; } = new();

    public DateTime StartDate { get; set; } = DateTime.UtcNow.AddMonths(-1);

    public DateTime EndDate { get; set; } = DateTime.UtcNow;

    public Dictionary<int, List<TripViewModel>> EnterpriseTrips { get; set; } = new();

    public async Task OnGet(List<int>? enterpriseIds, DateTime? startDate, DateTime? endDate)
    {
        var managerId = User.GetManagerId();

        // Load all enterprises for the manager
        ManagerEnterprises = await enterpriseQueryRepository.GetAllAsync(managerId);

        SelectedEnterpriseIds = enterpriseIds ?? new List<int>();
        StartDate = startDate ?? StartDate;
        EndDate = endDate ?? EndDate;

        // Filter selected enterprises
        var selectedEnterprises = ManagerEnterprises
                                 .Where(e => SelectedEnterpriseIds.Contains(e.EnterpriseId))
                                 .ToList();

        // Fetch trips for the selected enterprises
        EnterpriseTrips = await tripQueryService.GetEnterpriseTrips(selectedEnterprises, StartDate, EndDate);
    }
}