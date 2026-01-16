using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace MotorPool.TelegramBot;

public class BotHostedService(ITelegramBotClient telegramBotClient, IServiceScopeFactory serviceScopeFactory, ILogger<BotHostedService> logger) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Bot client started receiving messages");
        var receiverOptions = new ReceiverOptions();

        telegramBotClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );
        return Task.CompletedTask;
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received a new update {UpdateId}", update.Id);
        using var scope = serviceScopeFactory.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<UpdateHandler>();
        await handler.HandleUpdateAsync(update, cancellationToken);
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogInformation("Error occured {Message}", exception.Message);
        using var scope = serviceScopeFactory.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<UpdateHandler>();
        return handler.HandleErrorAsync(exception);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Bot client stopped receiving messages");
        return Task.CompletedTask;
    }
}