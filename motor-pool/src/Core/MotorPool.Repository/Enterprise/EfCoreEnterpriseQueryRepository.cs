using Microsoft.EntityFrameworkCore;
using MotorPool.Persistence;
using MotorPool.Repository.Manager;

namespace MotorPool.Repository.Enterprise;

using Enterprise = Domain.Enterprise;

public class EfCoreEnterpriseQueryRepository(AppDbContext dbContext) : EnterpriseQueryRepository
{
    public async ValueTask<List<Enterprise>> GetAllAsync(int managerId) => await BaseEnterpriseQuery().ForManager(managerId).ToListAsync();

    public async ValueTask<Enterprise?> GetByIdAsync(int enterpriseId) => await BaseEnterpriseQuery().FirstOrDefaultAsync(enterprise => enterprise.EnterpriseId == enterpriseId);

    private IQueryable<Enterprise> BaseEnterpriseQuery() => dbContext.Enterprises.AsNoTracking().Include(enterprise => enterprise.ManagerLinks);
}