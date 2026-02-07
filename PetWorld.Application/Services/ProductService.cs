using PetWorld.Domain.Entities;
using PetWorld.Domain.Repositories;

namespace PetWorld.Application.Services
{
    /// <summary>
    /// Interface for product service operations.
    /// Defines contract for managing product-related business logic.
    /// </summary>
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
    }

    /// <summary>
    /// Service for managing product operations.
    /// Provides business logic layer for product management.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        /// <summary>
        /// Retrieves all products from the catalog.
        /// </summary>
        /// <returns>A list of all products.</returns>
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        /// <summary>
        /// Retrieves a specific product by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the product.</param>
        /// <returns>The product if found, otherwise null.</returns>
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }
    }
}
