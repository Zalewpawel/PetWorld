using Microsoft.EntityFrameworkCore;
using PetWorld.Domain.Entities;
using PetWorld.Domain.Repositories;
using PetWorld.Infrastructure.Persistence;

namespace PetWorld.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for managing product persistence.
    /// Handles CRUD operations for Product entities in the database.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new product to the database.
        /// </summary>
        /// <param name="product">The product entity to add.</param>
        /// <returns>The added product with generated ID.</returns>
        public async Task<Product> AddAsync(Product product)
        {
            var entry = await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        /// <summary>
        /// Retrieves all products ordered alphabetically by name.
        /// </summary>
        /// <returns>A list of all products.</returns>
        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific product by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the product.</param>
        /// <returns>The product if found, otherwise null.</returns>
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
