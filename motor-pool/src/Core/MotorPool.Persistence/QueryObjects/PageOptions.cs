using MotorPool.Utils.ValidationAttributes;

namespace MotorPool.Persistence.QueryObjects;

public class Options
{
    [NonNegative(CanBeZero = false)]
    public int? CurrentPage { get; set; }

    [NonNegative(CanBeZero = false)]
    public int? ElementsPerPage { get; set; }

    public bool WithTrips { get; set; } = false;

    public PageOptions ToPageOptions(int? defaultElementsPerPage = null) => new()
                                                                            {
                                                                                CurrentPage = CurrentPage ?? PageOptions.DEFAULT_PAGE_NUMBER,
                                                                                ElementsPerPage = ElementsPerPage ?? defaultElementsPerPage ?? PageOptions.DEFAULT_ELEMENTS_PER_PAGE_AMOUNT
                                                                            };
}

public class PageOptions
{
    public static readonly int DEFAULT_PAGE_NUMBER = 1;

    public static readonly int DEFAULT_ELEMENTS_PER_PAGE_AMOUNT = 30;

    public required int CurrentPage { get; set; }

    public required int ElementsPerPage { get; set; }

    public int TotalPages(int entitiesCount) => entitiesCount / ElementsPerPage + (entitiesCount % ElementsPerPage == 0 ? 0 : 1);
}