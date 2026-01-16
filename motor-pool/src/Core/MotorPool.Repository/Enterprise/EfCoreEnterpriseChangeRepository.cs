using Microsoft.EntityFrameworkCore;
using MotorPool.Domain;
using MotorPool.Persistence;

namespace MotorPool.Repository.Enterprise;

using Enterprise = Domain.Enterprise;

public class EfCoreEnterpriseChangeRepository(AppDbContext dbContext) : EnterpriseChangeRepository
{
    public async ValueTask<Enterprise> CreateAsync(Enterprise newEnterprise)
    {
        await dbContext.Enterprises.AddAsync(newEnterprise);

        await dbContext.SaveChangesAsync();

        return newEnterprise;
    }

    public async ValueTask UpdateAsync(Enterprise enterpriseToUpdate)
    {
        Enterprise? currentEnterprise = await dbContext.Enterprises.FindAsync(enterpriseToUpdate.EnterpriseId);

        if (currentEnterprise is null) return;

        dbContext.Enterprises.Update(enterpriseToUpdate);

        await dbContext.SaveChangesAsync();
    }

    public async ValueTask DeleteAsync(int enterpriseId)
    {
        Enterprise? enterprise = await dbContext.Enterprises.FindAsync(enterpriseId);

        if (enterprise is null) return;

        dbContext.Enterprises.Remove(enterprise);

        await dbContext.SaveChangesAsync();
    }
}