using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MotorPool.Domain;

namespace MotorPool.Persistence.Configurations;

public class EnterpriseManagerConfiguration : IEntityTypeConfiguration<EnterpriseManager>
{

    public void Configure(EntityTypeBuilder<EnterpriseManager> builder)
    {
        builder.HasKey(enterpriseManager => new { enterpriseManager.ManagerId, enterpriseManager.EnterpriseId });

        builder.HasOne(managerEnterprise => managerEnterprise.Manager)
               .WithMany(manager => manager.EnterpriseLinks)
               .HasForeignKey(managerEnterprise => managerEnterprise.ManagerId);

        builder.HasOne(managerEnterprise => managerEnterprise.Enterprise)
               .WithMany(enterprise => enterprise.ManagerLinks)
               .HasForeignKey(managerEnterprise => managerEnterprise.EnterpriseId);
    }

}