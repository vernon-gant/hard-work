using AutoMapper;

using MotorPool.Domain;
using MotorPool.Services.Vehicles.Models;

namespace MotorPool.Services.Vehicles;

public class VehicleProfile : Profile
{
    public VehicleProfile()
    {
        CreateMap<Vehicle, VehicleViewModel>()
           .ForMember(vehicleViewModel => vehicleViewModel.CompanyName, opt => opt.MapFrom(vehicle => vehicle.VehicleBrand!.CompanyName))
           .ForMember(vehicleViewModel => vehicleViewModel.ModelName, opt => opt.MapFrom(vehicle => vehicle.VehicleBrand!.ModelName))
           .ForMember(vehicleViewModel => vehicleViewModel.DriverIds, opt => opt.MapFrom(vehicle => vehicle.DriverVehicles.Select(driverVehicle => driverVehicle.DriverId)))
           .ForMember(vehicleViewModel => vehicleViewModel.ManagerIds, opt => opt.MapFrom(vehicle => vehicle.Enterprise!.ManagerLinks.Select(managerLink => managerLink.ManagerId)))
           .ForMember(vehicleViewModel => vehicleViewModel.TripIds, opt => opt.MapFrom(vehicle => vehicle.Trips.Select(trip => trip.TripId)))
           .ForMember(vehicleViewModel => vehicleViewModel.AcquiredOn, opt => opt.MapFrom(vehicle => vehicle.AcquiredOnInEnterpriseTimeZone))
           .ForMember(vehicleViewModel => vehicleViewModel.TotalTripsCount, opt => opt.MapFrom(vehicle => vehicle.Trips.Count));

        CreateMap<VehicleDTO, Vehicle>();

        CreateMap<VehicleDTO, VehicleViewModel>();

        CreateMap<VehicleViewModel, VehicleDTO>();
    }
}