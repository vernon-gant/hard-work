namespace MotorPool.UI.Pages.Shared;

public class ActionsModel
{

    public required string IdRouteParameterName { get; set; } = string.Empty;

    public required int IdValue { get; set; }

    public bool HasChangeAccess { get; set; }

}