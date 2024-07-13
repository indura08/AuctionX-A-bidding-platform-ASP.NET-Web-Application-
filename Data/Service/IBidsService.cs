using AuctionX.Models;

namespace AuctionX.Data.Service
{
    public interface IBidsService
    {
        Task Add(Bid bid);
    }
}
