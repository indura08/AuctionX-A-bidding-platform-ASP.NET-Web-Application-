using AuctionX.Models;

namespace AuctionX.Data.Service
{
    public interface IListings
    {
        IQueryable<Listing> Getall();
    }
}
