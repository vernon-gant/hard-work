namespace MotorPool.Repository.Manager;

public static class ManagerLINQExtension
{

    public static IQueryable<Domain.Vehicle> ForManager(this IQueryable<Domain.Vehicle> vehicles, int managerId) =>
        vehicles.Where(vehicle => vehicle.Enterprise != null && vehicle.Enterprise.ManagerLinks.Any(managerLink => managerLink.ManagerId == managerId));

    public static IQueryable<Domain.Driver> ForManager(this IQueryable<Domain.Driver> drivers, int managerId) =>
        drivers.Where(driver => driver.Enterprise != null && driver.Enterprise.ManagerLinks.Any(managerLink => managerLink.ManagerId == managerId));

    public static IQueryable<Domain.Enterprise> ForManager(this IQueryable<Domain.Enterprise> enterprises, int managerId) =>
        enterprises.Where(enterprise => enterprise.ManagerLinks.Any(managerLink => managerLink.ManagerId == managerId));

}