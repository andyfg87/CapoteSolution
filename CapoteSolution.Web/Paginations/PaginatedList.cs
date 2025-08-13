using CapoteSolution.Web.Interface;
using Microsoft.EntityFrameworkCore;

namespace CapoteSolution.Web.Paginations
{
    public class PaginatedList<T> : List<T>, IPaginatedList
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public RouteValueDictionary RouteValues { get; set; }

        public PaginatedList(IEnumerable<T> items, int count, int pageIndex, int pageSize, RouteValueDictionary routeValues = null)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            RouteValues = routeValues ?? new RouteValueDictionary();

            RouteValues["pageSize"] = pageSize.ToString();

            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize, RouteValueDictionary routeValues = null)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize,routeValues);
        }

        public static PaginatedList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize, RouteValueDictionary routeValues = null)
        {
            var count = source.Count();
            var items = source
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
