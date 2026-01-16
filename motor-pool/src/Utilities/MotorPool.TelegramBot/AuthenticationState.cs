using OneOf;
using OneOf.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MotorPool.TelegramBot;

public interface AuthenticationState
{
    ReplyKeyboardMarkup CommandsAvailable { get; }
    OneOf<True, Error> CommandPermitted(Command command);
}

public class LoggedIn : AuthenticationState
{
    private static readonly List<Type> PermittedCommands = new()
    {
        typeof(WelcomeCommand),
        typeof(ReportCommand),
        typeof(LogoutCommand)
    };

    public OneOf<True, Error> CommandPermitted(Command command)
    {
        return PermittedCommands.Contains(command.GetType())
            ? new True()
            : new Error();
    }

    public string StateChangeCommand { get; } = "/logout";

    public ReplyKeyboardMarkup CommandsAvailable { get; } = new(new[]
    {
        new KeyboardButton[] { "/welcome", "/report", "/logout" }
    })
    {
        ResizeKeyboard = true
    };
}

public class LoggedOut : AuthenticationState
{
    private static readonly List<Type> PermittedCommands = new()
    {
        typeof(WelcomeCommand),
        typeof(LoginCommand)
    };

    public string StateChangeCommand { get; } = "/login";

    public ReplyKeyboardMarkup CommandsAvailable { get; } = new(new[]
    {
        new KeyboardButton[] { "/welcome", "/login" }
    })
    {
        ResizeKeyboard = true
    };

    public OneOf<True, Error> CommandPermitted(Command command)
    {
        return PermittedCommands.Contains(command.GetType())
            ? new True()
            : new Error();
    }
}