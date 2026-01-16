namespace MotorPool.Auth.User;

public class UserViewModel
{
    public required string UserName { get; set; }

    public required string Email { get; set; }

    public int? ManagerId { get; set; }
}