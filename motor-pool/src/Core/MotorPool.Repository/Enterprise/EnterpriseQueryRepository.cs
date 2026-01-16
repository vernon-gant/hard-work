namespace MotorPool.Repository.Enterprise;

using Enterprise = Domain.Enterprise;

public interface EnterpriseQueryRepository
{
    ValueTask<List<Enterprise>> GetAllAsync(int managerId);

    ValueTask<Enterprise?> GetByIdAsync(int enterpriseId);
}