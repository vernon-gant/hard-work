using OneOf;
using OneOf.Types;
using Telegram.Bot.Types;

namespace MotorPool.TelegramBot;

public interface MessageResolver
{
    OneOf<Command, ExecutionStep, None> Resolve(Message updateMessage, UserContext userContext);
}


public class DefaultMessageResolver(ActionFactory actionFactory) : MessageResolver
{
    public OneOf<Command, ExecutionStep, None> Resolve(Message updateMessage, UserContext userContext)
    {
        if (IsCommand(updateMessage.Text)) return actionFactory.CreateCommand(updateMessage.Text!);

        if (userContext.CurrentCommand.IsFinished()) return new None();

        return OneOf<Command, ExecutionStep, None>.FromT1(actionFactory.CreateExecutionStep(userContext.CurrentCommand.CurrentStep));
    }

    private static bool IsCommand(string? messageText) => messageText is not null && messageText.StartsWith('/');
}