using AutoMapper;

using MotorPool.Domain;
using MotorPool.Services.Drivers.Models;

namespace MotorPool.Services.Drivers;

public class DriverProfile : Profile
{

    public DriverProfile()
    {
        CreateMap<Driver, DriverViewModel>()
            .ForMember(driverViewModel => driverViewModel.VehicleIds, opt => opt.MapFrom(driver => driver.DriverVehicles.Select(driverVehicle => driverVehicle.VehicleId)))
            .ForMember(driverViewModel => driverViewModel.ManagerIds, opt => opt.MapFrom(driver => driver.Enterprise!.ManagerLinks.Select(managerLink => managerLink.ManagerId)));
    }

}