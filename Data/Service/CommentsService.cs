using AuctionX.Models;

namespace AuctionX.Data.Service
{
    public class CommentsService : ICommentsService
    {
        private readonly ApplicationDbContext _context;
        public CommentsService(ApplicationDbContext context) 
        {
            _context = context;
        }
        async Task ICommentsService.Add(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }
    }
}
