namespace MotorPool.Domain;

public class EnterpriseManager
{

    public int ManagerId { get; set; }

    public Manager Manager { get; set; } = null!;

    public int EnterpriseId { get; set; }

    public Enterprise Enterprise { get; set; } = null!;

}