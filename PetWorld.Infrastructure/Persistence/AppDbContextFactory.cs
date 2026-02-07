using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PetWorld.Infrastructure.Persistence
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            var connection = Environment.GetEnvironmentVariable("PETWORLD_CONNECTION")
                ?? "server=localhost;port=3306;database=PetWorldDb;user=root;password=PetPetPet!";

            var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));
            optionsBuilder.UseMySql(connection, serverVersion);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
