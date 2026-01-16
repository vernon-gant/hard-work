namespace MotorPool.Domain;

public class Manager
{
    public int ManagerId { get; set; }

    public List<EnterpriseManager> EnterpriseLinks { get; set; } = new();
}