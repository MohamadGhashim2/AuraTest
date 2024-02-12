using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuraTest.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product Name is required.")]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "Product Amount is required.")]
        public int ProductAmount { get; set; }

        [Required(ErrorMessage = "Product Category is required.")]
        public int CategoryId { get; set; }  

        [ForeignKey("CategoryId")]  
        public virtual Category Category { get; set; }  

        [Required(ErrorMessage = "Product Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal ProductPrice { get; set; }

        [MaxLength(500, ErrorMessage = "Product Description cannot exceed 500 characters.")]
        public string ProductDescription { get; set; }

        [Required(ErrorMessage = "Product Size is required.")]
        public string ProductSize { get; set; }

        [Required(ErrorMessage = "Product Color is required.")]
        public string ProductColor { get; set; }

        [Url(ErrorMessage = "Invalid Image URL.")]
        public string ProductImageUrl { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }


        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}