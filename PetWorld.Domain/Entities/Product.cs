using System.ComponentModel.DataAnnotations;

namespace PetWorld.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        
        [Required]
        public required string Name { get; set; }
        
        [Required]
        public required string Category { get; set; }
        
        public decimal Price { get; set; } = 0;
        
        public string? Description { get; set; }
    }
}
