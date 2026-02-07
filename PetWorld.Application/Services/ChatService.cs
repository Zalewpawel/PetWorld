using PetWorld.Domain.Entities;
using PetWorld.Domain.Repositories;

namespace PetWorld.Application.Services
{
    /// <summary>
    /// Interface for chat service operations.
    /// Defines contract for managing chat interactions with AI-powered responses.
    /// </summary>
    public interface IChatService
    {
        Task<ChatMessage> SendMessageAsync(string question);
        Task<List<ChatMessage>> GetAllMessagesAsync();
        Task<ChatMessage?> GetMessageByIdAsync(int id);
    }

    /// <summary>
    /// Service for managing chat interactions with AI-powered responses.
    /// Orchestrates the Writer-Critic agent system and persists chat messages.
    /// </summary>
    public class ChatService : IChatService
    {
        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly IProductRepository _productRepository;
        private readonly object _agentService;

        public ChatService(
            IChatMessageRepository chatMessageRepository,
            IProductRepository productRepository,
            object agentService)
        {
            _chatMessageRepository = chatMessageRepository;
            _productRepository = productRepository;
            _agentService = agentService;
        }

        /// <summary>
        /// Processes a user question through the AI agent system and persists the conversation.
        /// Uses reflection to invoke AgentService to maintain clean architecture boundaries.
        /// </summary>
        /// <param name="question">The user's question text.</param>
        /// <returns>A ChatMessage containing the question, AI-generated answer, and metadata.</returns>
        public async Task<ChatMessage> SendMessageAsync(string question)
        {
            var products = await _productRepository.GetAllAsync();

            var agentType = _agentService.GetType();
            var method = agentType.GetMethod("ProcessQuestionAsync");
            
            if (method != null)
            {
                var task = (Task<(string, int, string)>)method.Invoke(_agentService, new object[] { question, products });
                await task;
                var result = task.Result;
                var (answer, iterations, agentLog) = result;

                var message = new ChatMessage
                {
                    Question = question,
                    Answer = answer + (string.IsNullOrEmpty(agentLog) ? "" : $"\n\n---\n[Agent Log]\n{agentLog}"),
                    IterationCount = iterations,
                    CreatedAt = DateTime.Now
                };

                var savedMessage = await _chatMessageRepository.AddAsync(message);
                await _chatMessageRepository.SaveChangesAsync();
                
                return savedMessage;
            }

            var fallbackMessage = new ChatMessage
            {
                Question = question,
                Answer = "Błąd: Agent service niedostępny",
                IterationCount = 0,
                CreatedAt = DateTime.Now
            };

            return await _chatMessageRepository.AddAsync(fallbackMessage);
        }

        /// <summary>
        /// Retrieves all chat messages from the database.
        /// </summary>
        /// <returns>A list of all chat messages ordered by creation date.</returns>
        public async Task<List<ChatMessage>> GetAllMessagesAsync()
        {
            return await _chatMessageRepository.GetAllAsync();
        }

        /// <summary>
        /// Retrieves a specific chat message by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the chat message.</param>
        /// <returns>The chat message if found, otherwise null.</returns>
        public async Task<ChatMessage?> GetMessageByIdAsync(int id)
        {
            return await _chatMessageRepository.GetByIdAsync(id);
        }
    }
}

