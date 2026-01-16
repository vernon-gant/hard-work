using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MotorPool.Domain;

namespace MotorPool.Persistence.Configurations;

public class EnterpriseConfiguration : IEntityTypeConfiguration<Enterprise>
{

    public void Configure(EntityTypeBuilder<Enterprise> builder)
    {
        builder.HasIndex(enterprise => enterprise.VAT).IsUnique();
        builder.HasIndex(enterprise => enterprise.Name).IsUnique();
    }

}