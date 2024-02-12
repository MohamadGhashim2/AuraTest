using NuGet.Packaging.Signing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuraTest.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [ForeignKey("User")]  // Establishing the foreign key relationship
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }  // Navigation property

        [Required]
        public int TotalPrice { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public string Status { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    }
}