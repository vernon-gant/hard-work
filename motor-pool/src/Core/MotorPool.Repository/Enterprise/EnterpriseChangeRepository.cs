namespace MotorPool.Repository.Enterprise;

public interface EnterpriseChangeRepository
{
    ValueTask<Domain.Enterprise> CreateAsync(Domain.Enterprise enterprise);

    ValueTask UpdateAsync(Domain.Enterprise enterpriseToUpdate);

    ValueTask DeleteAsync(int enterpriseId);
}