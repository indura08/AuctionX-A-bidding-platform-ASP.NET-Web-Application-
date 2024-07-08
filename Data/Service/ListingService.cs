using AuctionX.Models;
using Microsoft.EntityFrameworkCore;

namespace AuctionX.Data.Service
{
    public class ListingService : IListings
    {
        private readonly ApplicationDbContext _context;
        public ListingService(ApplicationDbContext context)
        {
            _context = context;
        }
        public IQueryable<Listing> Getall()
        {
            var applicationDbContext = _context.Listings.Include(l => l.User);
            return applicationDbContext;
        }
    }
}
