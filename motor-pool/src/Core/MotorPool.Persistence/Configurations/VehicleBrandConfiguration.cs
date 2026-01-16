using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MotorPool.Domain;

namespace MotorPool.Persistence.Configurations;

public class VehicleBrandConfiguration : IEntityTypeConfiguration<VehicleBrand>
{

    public void Configure(EntityTypeBuilder<VehicleBrand> builder)
    {
        builder.Property(vehicleBrand => vehicleBrand.Type).HasConversion<string>();
        builder.Property(vehicleBrand => vehicleBrand.FuelTankCapacityLiters).HasPrecision(10, 4);
        builder.Property(vehicleBrand => vehicleBrand.PayloadCapacityKg).HasPrecision(11, 5);

        builder.HasMany(vehicleBrand => vehicleBrand.Vehicles)
               .WithOne(vehicle => vehicle.VehicleBrand)
               .HasForeignKey(vehicle => vehicle.VehicleBrandId);
    }

}