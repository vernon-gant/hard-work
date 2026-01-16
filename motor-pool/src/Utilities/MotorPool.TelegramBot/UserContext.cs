using MotorPool.Domain.Reports;
using MotorPool.Services.Reporting.DTO;
using OneOf;

namespace MotorPool.TelegramBot;

public class UserContext
{
    public required long UserId { get; init; }

    public required string UserName { get; init; }

    public int? ManagerId { get; set; }

    public string EnteredEmail { get; set; } = string.Empty;

    public required AuthenticationState AuthenticationState { get; set; }

    public required Command CurrentCommand { get; set; }

    public ReportDTO ReportDTO { get; set; } = new();

    public void Update(OneOf<Command, ExecutionStep> resolvedMessage)
    {
        resolvedMessage.Switch(
            command => CurrentCommand = command,
            step => CurrentCommand.CurrentStep = step
        );
    }

    public void Reset()
    {
        ManagerId = null;
        EnteredEmail = string.Empty;
        AuthenticationState = new LoggedOut();
    }
}