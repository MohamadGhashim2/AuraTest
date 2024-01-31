using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace AuraTest.Models
{
    public class Categories
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category Name is required.")]
        [MaxLength(50, ErrorMessage = "Category Name cannot exceed 50 characters.")]
        public string CategoryName { get; set; }

        [MaxLength(500, ErrorMessage = "Category Description cannot exceed 500 characters.")]
        public string CategoryDescription { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
