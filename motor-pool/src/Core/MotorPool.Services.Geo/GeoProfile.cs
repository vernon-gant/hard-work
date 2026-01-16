using AutoMapper;

using MotorPool.Domain;
using MotorPool.Services.Geo.Models;

namespace MotorPool.Services.Geo;

public class GeoProfile : Profile
{

    public GeoProfile()
    {
        DateTimeToEnterpriseZoneConverter enterpriseZoneConverter = new ();

        CreateMap<GeoPoint, GeoPointViewModel>()
            .ForMember(viewModel => viewModel.Point, options => options.MapFrom(src => new PointViewModel { LatitudeDouble = src.Latitude, LongitudeDouble = src.Longitude }))
            .ForMember(viewModel => viewModel.RecordedAt, options => options.ConvertUsing(enterpriseZoneConverter, src => src.RecordedAt));

        CreateMap<Trip, TripViewModel>()
            .ForMember(viewModel => viewModel.StartTime, options => options.ConvertUsing(enterpriseZoneConverter, src => src.StartTime))
            .ForMember(viewModel => viewModel.EndTime, options => options.ConvertUsing(enterpriseZoneConverter, src => src.EndTime));
    }

}