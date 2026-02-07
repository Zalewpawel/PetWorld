namespace PetWorld.Domain.Repositories
{
    using PetWorld.Domain.Entities;

    public interface IProductRepository
    {
        Task<Product> AddAsync(Product product);
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
    }
}
