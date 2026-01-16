using MotorPool.Persistence.QueryObjects;
using MotorPool.Utils.Exceptions;

namespace MotorPool.Persistence;

public class PagedResult<T> where T : class
{

    public int TotalPages { get; private set; }

    public List<T> Elements { get; private set; } = new();

    public static PagedResult<T> FromOptionsAndElements(PageOptions pageOptions, List<T> pagedElements, int allElementsCount)
    {
        int lastPossiblePage = pageOptions.TotalPages(allElementsCount);

        if (pagedElements.Count == 0 && pageOptions.CurrentPage > 1) throw new ExceededPageLimitException(pageOptions.CurrentPage, lastPossiblePage);

        return new PagedResult<T>
        {
            TotalPages = lastPossiblePage,
            Elements = pagedElements
        };
    }

}