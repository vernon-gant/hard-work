using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MotorPool.TelegramBot;

public abstract class Command
{
    public virtual ExecutionStep CurrentStep { get; set; } = new Finished();

    public abstract ValueTask ExecuteAsync(Message message, UserContext userContext, CancellationToken cancellationToken);

    public bool IsFinished() => CurrentStep is Finished;
}

public abstract class MultiStepCommand(IServiceScopeFactory scopeFactory, ITelegramBotClient botClient) : Command
{
    public override async ValueTask ExecuteAsync(Message message, UserContext userContext,
        CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();

        CurrentStep =  (ExecutionStep)scope.ServiceProvider.GetRequiredService(CurrentStep.GetType());

        var executionResult = await CurrentStep.ExecuteAsync(message, userContext, cancellationToken);

        await executionResult.Match(async success =>
        {
            if (success.Value is not null) await success.Value.Invoke(botClient, cancellationToken);
            CurrentStep = CurrentStep.NextStep;
        }, async error => { await error.Value.Invoke(botClient, cancellationToken); });
    }
}

public class WelcomeCommand(ITelegramBotClient botClient) : Command
{
    public override async ValueTask ExecuteAsync(Message message, UserContext userContext,
        CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(message.Chat.Id, "Welcome to the Motor Pool bot! This is a learning project, so please don't expect too much from it.", replyMarkup: userContext.AuthenticationState.CommandsAvailable, cancellationToken: cancellationToken);
    }
}

public class MenuCommand : Command
{
    public override ValueTask ExecuteAsync(Message message, UserContext userContext, CancellationToken cancellationToken) { return ValueTask.CompletedTask; }
}

public class UnknownCommand : Command
{
    public override ValueTask ExecuteAsync(Message message, UserContext userContext,
        CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
}

public class LoginCommand(ITelegramBotClient botClient, IServiceScopeFactory serviceScopeFactory) : MultiStepCommand(serviceScopeFactory, botClient)
{
    public override ExecutionStep CurrentStep { get; set; } = new PrintEnterEmail();
}

public class LogoutCommand(ITelegramBotClient botClient, ActionFactory actionFactory) : Command
{
    public override async ValueTask ExecuteAsync(Message message, UserContext userContext, CancellationToken cancellationToken)
    {
        userContext.Reset();

        userContext.CurrentCommand = actionFactory.CreateCommand("/menu");

        await botClient.SendTextMessageAsync(message.Chat.Id, "You have been logged out", replyMarkup: userContext.AuthenticationState.CommandsAvailable, cancellationToken: cancellationToken);
    }
}

public class ReportCommand(ITelegramBotClient botClient, IServiceScopeFactory serviceScopeFactory) : MultiStepCommand(serviceScopeFactory, botClient)
{
    public override ExecutionStep CurrentStep { get; set; } = new PrintReportPeriod();
}