namespace BoticaOnlineAPI.Helpers
{
    public static class PaginacionHelper
    {
        public static IQueryable<T> Paginar<T>(IQueryable<T> query, int page, int pageSize)
        {
            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}