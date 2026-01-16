namespace MotorPool.UI.Pages.Shared;

public class PagingModel
{

    public required int TotalPages { get; set; }

    public required int CurrentPage { get; set; }

    public required int PagesDisplayRange { get; set; }

}