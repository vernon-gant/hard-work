using Microsoft.Extensions.DependencyInjection;

namespace MotorPool.TelegramBot;

public interface ActionFactory
{
    Command CreateCommand(string commandName);

    ExecutionStep CreateExecutionStep(ExecutionStep currentStep);
}

public class DefaultActionFactory(IServiceScopeFactory scopeFactory) : ActionFactory
{
    private static readonly Dictionary<string, Type> CommandTypes = new()
    {
        { "/start", typeof(WelcomeCommand) },
        { "/welcome", typeof(WelcomeCommand) },
        { "/menu", typeof(MenuCommand) },
        { "/login", typeof(LoginCommand) },
        { "/logout", typeof(LogoutCommand) },
        { "/report", typeof(ReportCommand) },
    };

    public Command CreateCommand(string commandName)
    {
        using var scope = scopeFactory.CreateScope();
        Type commandType = CommandTypes.TryGetValue(commandName, out Type? type) ? type : typeof(UnknownCommand);
        return (Command)scope.ServiceProvider.GetRequiredService(commandType);
    }

    public ExecutionStep CreateExecutionStep(ExecutionStep currentStep)
    {
        using var scope = scopeFactory.CreateScope();
        return (ExecutionStep)scope.ServiceProvider.GetRequiredService(currentStep.GetType());
    }
}