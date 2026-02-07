using Microsoft.EntityFrameworkCore;
using PetWorld.Domain.Entities;
using PetWorld.Domain.Repositories;
using PetWorld.Infrastructure.Persistence;

namespace PetWorld.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for managing chat message persistence.
    /// Handles CRUD operations for ChatMessage entities in the database.
    /// </summary>
    public class ChatMessageRepository : IChatMessageRepository
    {
        private readonly AppDbContext _context;

        public ChatMessageRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new chat message to the database.
        /// </summary>
        /// <param name="message">The chat message entity to add.</param>
        /// <returns>The added chat message with generated ID.</returns>
        public async Task<ChatMessage> AddAsync(ChatMessage message)
        {
            var entry = await _context.ChatMessages.AddAsync(message);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        /// <summary>
        /// Retrieves all chat messages ordered by creation date (newest first).
        /// </summary>
        /// <returns>A list of all chat messages.</returns>
        public async Task<List<ChatMessage>> GetAllAsync()
        {
            return await _context.ChatMessages
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific chat message by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the chat message.</param>
        /// <returns>The chat message if found, otherwise null.</returns>
        public async Task<ChatMessage?> GetByIdAsync(int id)
        {
            return await _context.ChatMessages.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// Persists all pending changes to the database.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
