using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MotorPool.Domain.Reports;
using MotorPool.Services.Reporting.Core;
using MotorPool.Services.Reporting.DTO;

namespace MotorPool.UI.Pages.Reports;

public class VehicleMileageModel(ReportService<VehicleMileageReport, VehicleMileageReportDTO> reportService) : PageModel
{
    [BindProperty]
    public VehicleMileageReportDTO ReportDto { get; set; } = null!;

    public VehicleMileageReport? VehicleMileageReport { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        VehicleMileageReport = await reportService.Generate(ReportDto);

        return Page();
    }
}