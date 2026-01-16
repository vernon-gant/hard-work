using AutoMapper;
using MotorPool.Repository.Enterprise;
using MotorPool.Services.Enterprise.Models;

namespace MotorPool.Services.Enterprise;

public class EnterpriseProfile : Profile
{

    public EnterpriseProfile()
    {
        CreateMap<EnterpriseDTO, Domain.Enterprise>()
            .ForMember(enterprise => enterprise.Drivers, opt => opt.Ignore())
            .ForMember(enterprise => enterprise.Vehicles, opt => opt.Ignore())
            .ForMember(enterprise => enterprise.ManagerLinks, opt => opt.Ignore());

        CreateMap<Domain.Enterprise, SimpleEnterpriseViewModel>()
            .ForMember(simpleEnterpriseViewModel => simpleEnterpriseViewModel.ManagerIds,
                       opt => opt.MapFrom(enterprise => enterprise.ManagerLinks.Select(managerLink => managerLink.ManagerId)));

        CreateMap<Domain.Enterprise, FullEnterpriseViewModel>()
            .ForMember(enterpriseViewModel => enterpriseViewModel.DriverIds, opt => opt.MapFrom(enterprise => enterprise.Drivers.Select(driver => driver.DriverId)))
            .ForMember(enterpriseViewModel => enterpriseViewModel.VehicleIds, opt => opt.MapFrom(enterprise => enterprise.Vehicles.Select(vehicle => vehicle.VehicleId)))
            .ForMember(enterpriseViewModel => enterpriseViewModel.ManagerIds,
                       opt => opt.MapFrom(enterprise => enterprise.ManagerLinks.Select(managerLink => managerLink.ManagerId)));
    }

}