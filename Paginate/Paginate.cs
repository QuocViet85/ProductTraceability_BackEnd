namespace App;

public static class Paginate
{
    public const int DEFAULT_PAGENUMBER = 1;
    public const int DEFAULT_LIMIT = 10;
    public const int MAX_LIMIT = 100;

    public static void SetPaginate(ref int pageNumber, ref int limit)
    {
        if (pageNumber <= 0)
        {
            pageNumber = DEFAULT_PAGENUMBER;
        }

        if (limit <= 0)
        {
            limit = DEFAULT_LIMIT;
        }else if (limit > MAX_LIMIT)
        {
            limit = MAX_LIMIT;
        }
    }
}
