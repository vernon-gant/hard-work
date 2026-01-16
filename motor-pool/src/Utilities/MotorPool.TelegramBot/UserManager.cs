using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace MotorPool.TelegramBot;

public interface UserManager
{
    UserContext? GetUser(long userId);

    UserContext AddUser(long userId, string userName);
}

public class DefaultUserManager(IServiceScopeFactory serviceScopeFactory) : UserManager
{
    private readonly ConcurrentDictionary<long, UserContext> _users = new();

    public UserContext? GetUser(long userId) => _users.TryGetValue(userId, out UserContext? userContext) ? userContext : null;

    public UserContext AddUser(long userId, string userName)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var actionFactory = scope.ServiceProvider.GetRequiredService<ActionFactory>();
        UserContext userContext = new()
        {
            UserId = userId,
            UserName = userName,
            AuthenticationState = new LoggedOut(),
            CurrentCommand = actionFactory.CreateCommand("/start")
        };

        _users.TryAdd(userId, userContext);

        return userContext;
    }
}