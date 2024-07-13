using AuctionX.Models;

namespace AuctionX.Data.Service
{
    public interface IListings
    {
        IQueryable<Listing> Getall();

        Task Add(Listing listing);

        Task<Listing> GetById(int? id);

        Task SaveChnages();
    }
}
