using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MotorPool.Domain;

namespace MotorPool.Persistence.Configurations;

public class DriverConfiguration : IEntityTypeConfiguration<Driver>
{

    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.Property(driver => driver.Salary).HasPrecision(9, 3);
    }

}