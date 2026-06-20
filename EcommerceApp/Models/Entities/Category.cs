using System.ComponentModel.DataAnnotations;
namespace EcommerceApp.Models.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        // Navigation Property
        public ICollection<Product> Products { get; set; } = new List<Product>();

    }
}
