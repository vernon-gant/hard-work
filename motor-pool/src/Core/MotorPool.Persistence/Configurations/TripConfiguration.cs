using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MotorPool.Domain;

namespace MotorPool.Persistence.Configurations;

public class TripConfiguration : IEntityTypeConfiguration<Trip>
{
    public void Configure(EntityTypeBuilder<Trip> builder)
    {
        builder.HasOne(trip => trip.Vehicle)
               .WithMany(vehicle => vehicle.Trips)
               .HasForeignKey(trip => trip.VehicleId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(trip => trip.StartGeoPoint)
               .WithOne()
               .HasForeignKey<Trip>(trip => trip.StartGeoPointId);

        builder.HasOne(trip => trip.EndGeoPoint)
               .WithOne()
               .HasForeignKey<Trip>(trip => trip.EndGeoPointId);
    }
}