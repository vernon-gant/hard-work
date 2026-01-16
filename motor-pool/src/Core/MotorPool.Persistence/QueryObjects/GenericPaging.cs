namespace MotorPool.Persistence.QueryObjects;

public static class GenericPaging
{

    public static IQueryable<T> Page<T>(this IQueryable<T> query, PageOptions pageOptions)
    {
        if (pageOptions.ElementsPerPage <= 0) throw new ArgumentOutOfRangeException(nameof(pageOptions.ElementsPerPage), "amount of elements per page must be bigger then 0");

        if (pageOptions.CurrentPage < 1) throw new ArgumentOutOfRangeException(nameof(pageOptions.CurrentPage), "page number must be at least 0");

        int zeroBasedPageNumber = pageOptions.CurrentPage - 1;

        if (zeroBasedPageNumber != 0) query = query.Skip(zeroBasedPageNumber * pageOptions.ElementsPerPage);

        return query.Take(pageOptions.ElementsPerPage).OrderBy(x => x);
    }

}