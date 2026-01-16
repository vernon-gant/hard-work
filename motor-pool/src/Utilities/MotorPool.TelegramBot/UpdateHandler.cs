using Microsoft.Extensions.Logging;
using OneOf;
using OneOf.Types;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace MotorPool.TelegramBot;

public interface UpdateHandler
{
    Task HandleUpdateAsync(Update update, CancellationToken cancellationToken);

    Task HandleErrorAsync(Exception exception);
}

public class DefaultUpdateHandler(
    ILogger<DefaultUpdateHandler> logger,
    ITelegramBotClient botClient,
    UserManager userManager,
    MessageResolver messageResolver) : UpdateHandler
{
    public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message) return;

        UserContext userContext = userManager.GetUser(message.Chat.Id) ??
                                  userManager.AddUser(message.Chat.Id, message.Chat.Username!);

        OneOf<Command, ExecutionStep, None> resolvedMessage = messageResolver.Resolve(message, userContext);

        if (resolvedMessage.IsT0)
        {
            Command command = resolvedMessage.AsT0;
            OneOf<True, Error> commandPermitted = userContext.AuthenticationState.CommandPermitted(command);
            if (commandPermitted.IsT0)
            {
                userContext.Update(command);
            }
            else
            {
                logger.LogWarning("Unauthorized command {Command} from {UserId}", command.GetType().Name, userContext.UserId);
                await botClient.SendTextMessageAsync(message.Chat.Id, "Command not permitted", replyMarkup: userContext.AuthenticationState.CommandsAvailable, cancellationToken: cancellationToken);
                return;
            }
        }

        if (resolvedMessage.IsT1)
        {
            ExecutionStep executionStep = resolvedMessage.AsT1;
            userContext.Update(OneOf<Command, ExecutionStep>.FromT1(executionStep));
        }

        if (resolvedMessage.IsT2)
        {
            return;
        }

        Command currentCommand = userContext.CurrentCommand;
        await currentCommand.ExecuteAsync(message, userContext, cancellationToken);
    }

    public Task HandleErrorAsync(Exception exception)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException =>
                $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        logger.LogError("{ErrorMessage}", errorMessage);
        return Task.CompletedTask;
    }
}