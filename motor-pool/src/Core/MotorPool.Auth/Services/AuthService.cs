using MotorPool.Auth.User;

namespace MotorPool.Auth.Services;

public interface AuthService
{
    ValueTask<AuthResult> LoginAsync(LoginDTO loginDTO);

    ValueTask<AuthResult> RegisterAsync(RegisterDTO registerDTO);

    ValueTask<UserViewModel> GetUserAsync(string userId);
}