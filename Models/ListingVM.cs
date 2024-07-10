using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuctionX.Models
{
    public class ListingVM
    {
        public int Id { get; set; }
        public string Titile { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public IFormFile Image { get; set; }    //methna i form fil kiynne file type eke input ekk apita gnna puluwan kiyna ekai
        public bool IsSold { get; set; } = false;
        [Required]
        public string? IdentityUserId { get; set; }
        [ForeignKey("IdentityUserId")]
        public IdentityUser? User { get; set; }
    }
}
