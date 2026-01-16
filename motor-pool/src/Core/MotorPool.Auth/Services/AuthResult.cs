namespace MotorPool.Auth.Services;

public class AuthResult
{
    public bool IsSuccess { get; set; }

    public string? Token { get; set; }

    public string? Error { get; set; }

    public static AuthResult Success(string token)
    {
        return new AuthResult { IsSuccess = true, Token = token };
    }

    public static AuthResult Failure()
    {
        return new AuthResult { IsSuccess = false };
    }

    public static AuthResult Failure(string error)
    {
        return new AuthResult { IsSuccess = false, Error = error };
    }
}