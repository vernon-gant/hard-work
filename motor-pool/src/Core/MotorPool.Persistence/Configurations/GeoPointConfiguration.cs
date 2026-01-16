using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MotorPool.Domain;

namespace MotorPool.Persistence.Configurations;

public class GeoPointConfiguration : IEntityTypeConfiguration<GeoPoint>
{
    public void Configure(EntityTypeBuilder<GeoPoint> builder)
    {
        builder.Property(geoPoint => geoPoint.Latitude)
               .HasPrecision(9, 6);
        builder.Property(geoPoint => geoPoint.Longitude)
               .HasPrecision(9, 6);
        builder.HasIndex(geoPoint => geoPoint.VehicleId);

        builder.HasOne<Trip>(geoPoint => geoPoint.Trip)
               .WithMany(trip => trip.GeoPoints)
               .HasForeignKey(geoPoint => geoPoint.TripId)
               .IsRequired(false);
    }
}