using Microsoft.EntityFrameworkCore;
namespace AuctionX
{
    public class PaginatedList<T>: List<T>
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize) 
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pagesize)
        { 
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pagesize).Take(pagesize).ToListAsync();

            return new PaginatedList<T>(items, count, pageIndex, pagesize);
        }
    }
}
