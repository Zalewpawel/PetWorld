using System.ComponentModel.DataAnnotations;

namespace PetWorld.Domain.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }
        
        [Required]
        public required string Question { get; set; }
        
        public string? Answer { get; set; }
        
        public int IterationCount { get; set; } = 0;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
