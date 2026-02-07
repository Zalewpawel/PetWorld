namespace PetWorld.Domain.Repositories
{
    using PetWorld.Domain.Entities;

    public interface IChatMessageRepository
    {
        Task<ChatMessage> AddAsync(ChatMessage message);
        Task<List<ChatMessage>> GetAllAsync();
        Task<ChatMessage?> GetByIdAsync(int id);
        Task SaveChangesAsync();
    }
}
