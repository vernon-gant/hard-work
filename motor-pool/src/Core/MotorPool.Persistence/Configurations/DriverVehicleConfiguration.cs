using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MotorPool.Domain;

namespace MotorPool.Persistence.Configurations;

public class DriverVehicleConfiguration : IEntityTypeConfiguration<DriverVehicle>
{

    public void Configure(EntityTypeBuilder<DriverVehicle> modelBuilder)
    {
        modelBuilder.HasKey(dv => new { dv.DriverId, dv.VehicleId });

        modelBuilder.HasOne(driverVehicle => driverVehicle.Driver)
                    .WithMany(driver => driver.DriverVehicles)
                    .HasForeignKey(driverVehicle => driverVehicle.DriverId);

        modelBuilder.HasOne(driverVehicle => driverVehicle.Vehicle)
                    .WithMany(vehicle => vehicle.DriverVehicles)
                    .HasForeignKey(driverVehicle => driverVehicle.VehicleId);
    }

}