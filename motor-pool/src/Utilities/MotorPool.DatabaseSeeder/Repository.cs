using MotorPool.Persistence;

namespace MotorPool.DatabaseSeeder;

public interface Repository
{
    List<int> GetVehicleBrandIds();

    bool AllEnterprisesExist(IEnumerable<int> enterpriseIds);
}

public class EfCoreRepository(AppDbContext dbContext) : Repository
{
    public List<int> GetVehicleBrandIds() => dbContext.VehicleBrands.Select(brand => brand.VehicleBrandId).ToList();

    public bool AllEnterprisesExist(IEnumerable<int> enterpriseIds)
    {
        HashSet<int> existingEnterprises = dbContext.Enterprises.Select(enterprise => enterprise.EnterpriseId).ToHashSet();
        HashSet<int> nonExistingEnterprises = enterpriseIds.Except(existingEnterprises).ToHashSet();

        if (nonExistingEnterprises.Count == 0) return true;

        Console.WriteLine("The following enterprise ids do not exist in the database:");
        foreach (int enterpriseId in nonExistingEnterprises) Console.WriteLine(enterpriseId);
        Console.WriteLine("Please provide valid enterprise ids.");

        return false;
    }
}