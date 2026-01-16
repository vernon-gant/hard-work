using AutoMapper;
using MotorPool.Services.VehicleBrand.Models;

namespace MotorPool.Services.VehicleBrand;

public class VehicleBrandProfile : Profile
{

    public VehicleBrandProfile()
    {
        CreateMap<VehicleBrandDTO, Domain.VehicleBrand>();
        CreateMap<Domain.VehicleBrand, VehicleBrandViewModel>()
            .ForMember(viewModel => viewModel.Type, opt => opt.MapFrom(vehicleBrand => vehicleBrand.Type));
    }

}