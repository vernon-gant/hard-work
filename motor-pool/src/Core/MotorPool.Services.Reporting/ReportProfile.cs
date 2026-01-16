using AutoMapper;

using MotorPool.Domain.Reports;
using MotorPool.Services.Reporting.DTO;

namespace MotorPool.Services.Reporting;

public class ReportProfile : Profile
{
    public ReportProfile()
    {
        CreateMap<VehicleMileageReportDTO, VehicleMileageReport>();
    }
}