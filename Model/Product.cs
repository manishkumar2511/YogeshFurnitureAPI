using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace YogeshFurnitureAPI.Model
{
    public class Product
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string ? ImageUrl { get; set; }
        public int CategoryId { get; set; }

        [NotMapped]
        public IFormFile? ProductImage { get; set; }

        [JsonIgnore]
        public Category? Category { get; set; } = new Category();
    }
}
