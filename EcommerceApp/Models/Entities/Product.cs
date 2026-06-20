using System.ComponentModel.DataAnnotations;
namespace EcommerceApp.Models.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 999999)]
        public decimal Price { get; set; }

        public int Stock {  get; set; }

        public int CategoryId { get; set; }

        // Navigation Property
        public Category? Category { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
