using AuctionX.Models;

namespace AuctionX.Data.Service
{
    public interface ICommentsService
    {
        Task Add(Comment comment);
    }
}
