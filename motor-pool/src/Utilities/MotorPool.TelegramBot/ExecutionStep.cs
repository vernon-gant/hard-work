using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using MotorPool.Auth.User;
using MotorPool.Domain;
using MotorPool.Domain.Reports;
using MotorPool.Repository.Vehicle;
using MotorPool.Services.Reporting.Core;
using MotorPool.Services.Reporting.DTO;
using OneOf;
using OneOf.Types;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MotorPool.TelegramBot;

using Callback = Func<ITelegramBotClient, CancellationToken, ValueTask<Message>>;

public interface ExecutionStep
{
    ValueTask<OneOf<Success<Callback?>, Error<Callback>>> ExecuteAsync(Message message, UserContext userContext, CancellationToken cancellationToken);

    ExecutionStep NextStep { get; set; }
}

public class PrintEnterEmail : ExecutionStep
{
    public ValueTask<OneOf<Success<Callback?>, Error<Callback>>> ExecuteAsync(Message message, UserContext userContext, CancellationToken cancellationToken)
    {
        long chatId = message.Chat.Id;

        return ValueTask.FromResult<OneOf<Success<Callback?>, Error<Callback>>>(new Success<Callback?>(async (botClient, token) => await botClient.SendTextMessageAsync(chatId, "Please enter your email address", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: token)));
    }

    public ExecutionStep NextStep { get; set; } = new ReadManagerEmail(null!);
}

public class ReadManagerEmail(UserManager<ApplicationUser> userManager) : ExecutionStep
{
    public async ValueTask<OneOf<Success<Callback?>, Error<Callback>>> ExecuteAsync(Message message, UserContext userContext, CancellationToken cancellationToken)
    {
        long chatId = message.Chat.Id;

        if (message.Text is not { } text)
        {
            NextStep = new ReadManagerEmail(null!);
            return new Error<Callback>(async (botClient, token) => await botClient.SendTextMessageAsync(chatId, "Empty email address, please enter a valid email address", cancellationToken: token));
        }

        ApplicationUser? user = await userManager.FindByEmailAsync(text);

        bool isManagerExists;

        if (user == null) isManagerExists = false;
        else
        {
            IList<Claim> claims = await userManager.GetClaimsAsync(user);

            isManagerExists = claims.Any(claim => claim.Type == "ManagerId");
        }

        if (!isManagerExists)
        {
            NextStep = new ReadManagerEmail(null!);
            return new Error<Callback>(async (botClient, token) => await botClient.SendTextMessageAsync(chatId, "Manager not found, please enter a valid email address", cancellationToken: token));
        }

        userContext.EnteredEmail = text;
        return new Success<Callback?>(async (botClient, token) => await botClient.SendTextMessageAsync(chatId, "Please enter your manager's password", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: token));
    }

    public ExecutionStep NextStep { get; set; } = new ReadManagerPassword(null!);
}

public class ReadManagerPassword(UserManager<ApplicationUser> userManager) : ExecutionStep
{
    public async ValueTask<OneOf<Success<Callback?>, Error<Callback>>> ExecuteAsync(Message message, UserContext userContext, CancellationToken cancellationToken)
    {
        long chatId = message.Chat.Id;

        if (message.Text is not { } text)
        {
            NextStep = new ReadManagerPassword(null!);
            return new Error<Callback>(async (botClient, token) => await botClient.SendTextMessageAsync(chatId, "Empty password, please enter a valid password", cancellationToken: token));
        }

        string email = userContext.EnteredEmail;

        ApplicationUser? user = await userManager.FindByEmailAsync(email);

        bool authResult = await userManager.CheckPasswordAsync(user!, text);

        if (!authResult)
        {
            NextStep = new ReadManagerPassword(null!);
            return new Error<Callback>(async (botClient, token) => await botClient.SendTextMessageAsync(chatId, "Invalid password, please enter a valid password", cancellationToken: token));
        }

        Claim? claim = (await userManager.GetClaimsAsync(user!)).FirstOrDefault(claim => claim.Type == "ManagerId");

        userContext.ManagerId = claim == null ? -1 : int.Parse(claim.Value);

        userContext.AuthenticationState = new LoggedIn();

        return new Success<Callback?>(async (botClient, token) => await botClient.SendTextMessageAsync(chatId, "You are logged in", replyMarkup: userContext.AuthenticationState.CommandsAvailable, cancellationToken: token));
    }

    public ExecutionStep NextStep { get; set; } = new Finished();
}

public class Finished : ExecutionStep
{
    public ValueTask<OneOf<Success<Callback?>, Error<Callback>>> ExecuteAsync(Message message, UserContext userContext, CancellationToken cancellationToken) => ValueTask.FromResult<OneOf<Success<Callback?>, Error<Callback>>>(new Success<Callback?>(null));

    public ExecutionStep NextStep { get; set; } = null!;
}

public class PrintReportPeriod : ExecutionStep
{
    public ValueTask<OneOf<Success<Callback?>, Error<Callback>>> ExecuteAsync(Message message, UserContext userContext, CancellationToken cancellationToken)
    {
        long chatId = message.Chat.Id;

        ReplyKeyboardMarkup reportPeriods = new(new[]
        {
            new KeyboardButton[] { "Day", "Month" }
        })
        {
            ResizeKeyboard = true
        };

        return ValueTask.FromResult<OneOf<Success<Callback?>, Error<Callback>>>(new Success<Callback?>(async (botClient, token) => await botClient.SendTextMessageAsync(chatId, "Choose a report period", replyMarkup: reportPeriods, cancellationToken: token)));
    }

    public ExecutionStep NextStep { get; set; } = new ReadReportPeriod();
}

public class ReadReportPeriod : ExecutionStep
{
    public ValueTask<OneOf<Success<Callback?>, Error<Callback>>> ExecuteAsync(Message message, UserContext userContext, CancellationToken cancellationToken)
    {
        long chatId = message.Chat.Id;

        if (message.Text is not { } reportType)
        {
            NextStep = new ReadReportPeriod();
            return ValueTask.FromResult<OneOf<Success<Callback?>, Error<Callback>>>(new Error<Callback>(async (botClient, token) => await botClient.SendTextMessageAsync(chatId, "Empty report period, please enter a valid one", cancellationToken: token)));
        }

        switch (reportType)
        {
            case "Day":
                userContext.ReportDTO!.Period = Period.Day;
                break;
            case "Month":
                userContext.ReportDTO!.Period = Period.Month;
                break;
            default:
                NextStep = new ReadReportPeriod();
                return ValueTask.FromResult<OneOf<Success<Callback?>, Error<Callback>>>(new Error<Callback>(async (botClient, token) => await botClient.SendTextMessageAsync(chatId, "Invalid report period!", cancellationToken: token)));
        }

        NextStep = new ReadStartEndDates();
        return ValueTask.FromResult<OneOf<Success<Callback?>, Error<Callback>>>(new Success<Callback?>(async (botClient, token) => await botClient.SendTextMessageAsync(chatId, "Please enter the start and end dates", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: token)));
    }

    public ExecutionStep NextStep { get; set; } = null!;
}


public class ReadStartEndDates : ExecutionStep
{
    public ValueTask<OneOf<Success<Callback?>, Error<Callback>>> ExecuteAsync(Message message, UserContext userContext, CancellationToken cancellationToken)
    {
        long chatId = message.Chat.Id;

        if (message.Text is not { } text)
        {
            NextStep = new ReadStartEndDates();
            return ValueTask.FromResult<OneOf<Success<Callback?>, Error<Callback>>>(new Error<Callback>(async (botClient, token) => await botClient.SendTextMessageAsync(chatId, "Empty start end dates", cancellationToken: token)));
        }

        string[] dates = text.Split(' ');

        if (dates.Length != 2)
        {
            NextStep = new ReadStartEndDates();
            return ValueTask.FromResult<OneOf<Success<Callback?>, Error<Callback>>>(new Error<Callback>(async (botClient, token) => await botClient.SendTextMessageAsync(chatId, "Invalid dates, please enter a valid start and end dates", cancellationToken: token)));
        }

        if (!DateOnly.TryParse(dates[0], out DateOnly startDate) || !DateOnly.TryParse(dates[1], out DateOnly endDate))
        {
            NextStep = new ReadStartEndDates();
            return ValueTask.FromResult<OneOf<Success<Callback?>, Error<Callback>>>(new Error<Callback>(async (botClient, token) => await botClient.SendTextMessageAsync(chatId, "Invalid dates, please enter a valid start and end dates", cancellationToken: token)));
        }

        userContext.ReportDTO.StartTime = startDate;
        userContext.ReportDTO.EndTime = endDate;

        ReplyKeyboardMarkup reportTypes = new(new[]
        {
            new KeyboardButton[] { "Vehicle mileage" }
        })
        {
            ResizeKeyboard = true
        };

        return ValueTask.FromResult<OneOf<Success<Callback?>, Error<Callback>>>(new Success<Callback?>(async (botClient, token) => await botClient.SendTextMessageAsync(chatId, "Please enter a report type", replyMarkup: reportTypes, cancellationToken: token)));
    }

    public ExecutionStep NextStep { get; set; } = new ReadReportType();
}

public class ReadReportType : ExecutionStep
{
    public ValueTask<OneOf<Success<Callback?>, Error<Callback>>> ExecuteAsync(Message message, UserContext userContext, CancellationToken cancellationToken)
    {
        long chatId = message.Chat.Id;

        if (message.Text is not { } reportType)
        {
            NextStep = new ReadReportType();
            return ValueTask.FromResult<OneOf<Success<Callback?>, Error<Callback>>>(new Error<Callback>(async (botClient, token) => await botClient.SendTextMessageAsync(chatId, "Empty report type, please enter a valid one", cancellationToken: token)));
        }

        switch (reportType)
        {
            case "Vehicle mileage":
                userContext.ReportDTO = VehicleMileageReportDTO.FromReportDTO(userContext.ReportDTO!);
                NextStep = new ReadVehicleId(null!, null!);
                break;
            default:
                NextStep = new ReadReportType();
                return ValueTask.FromResult<OneOf<Success<Callback?>, Error<Callback>>>(new Error<Callback>(async (botClient, token) => await botClient.SendTextMessageAsync(chatId, "Invalid report type!", cancellationToken: token)));
        }

        return ValueTask.FromResult<OneOf<Success<Callback?>, Error<Callback>>>(new Success<Callback?>(async (botClient, token) => await botClient.SendTextMessageAsync(chatId, "Please enter a vehicle id", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: token)));

    }

    public ExecutionStep NextStep { get; set; } = null!;
}

public class ReadVehicleId(VehicleQueryRepository vehicleQueryRepository, ReportService<VehicleMileageReport, VehicleMileageReportDTO> reportService) : ExecutionStep
{
    public async ValueTask<OneOf<Success<Callback?>, Error<Callback>>> ExecuteAsync(Message message, UserContext userContext, CancellationToken cancellationToken)
    {
        long chatId = message.Chat.Id;

        if (message.Text is not { } text)
        {
            NextStep = new ReadVehicleId(null!, null!);
            return new Error<Callback>(async (botClient, token) => await botClient.SendTextMessageAsync(chatId, "Please enter a vehicle id", cancellationToken: token));
        }

        if (!int.TryParse(text, out int vehicleId))
        {
            NextStep = new ReadVehicleId(null!, null!);
            return new Error<Callback>(async (botClient, token) => await botClient.SendTextMessageAsync(chatId, "Not a valid vehicle id, please enter a valid vehicle id", cancellationToken: token));
        }

        Vehicle? vehicle = await vehicleQueryRepository.GetByIdAsync(vehicleId);

        if (vehicle == null)
        {
            NextStep = new ReadVehicleId(null!, null!);
            return new Error<Callback>(async (botClient, token) => await botClient.SendTextMessageAsync(chatId, "Vehicle not found, please enter a valid vehicle id", cancellationToken: token));
        }

        VehicleMileageReportDTO reportDTO = (userContext.ReportDTO as VehicleMileageReportDTO)!;
        reportDTO.VehicleId = vehicleId;

        var generatedReport = await reportService.Generate(reportDTO);

        string reportResultAsString = generatedReport.Result.Count > 0? generatedReport.Result.Select(pair => $"{pair.Key}: {pair.Value}").Aggregate((current, next) => $"{current}\n{next}") : "No data found";

        return new Success<Callback?>(async (botClient, token) => await botClient.SendTextMessageAsync(chatId, reportResultAsString, replyMarkup: userContext.AuthenticationState.CommandsAvailable, cancellationToken: token));
    }

    public ExecutionStep NextStep { get; set; } = new Finished();
}