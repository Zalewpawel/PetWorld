using Microsoft.EntityFrameworkCore;
using PetWorld.Domain.Entities;

namespace PetWorld.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Royal Canin Adult Dog 15kg", Category = "Karma dla psów", Price = 289m, Description = "Premium karma dla dorosłych psów średnich ras" },
                new Product { Id = 2, Name = "Whiskas Adult Kurczak 7kg", Category = "Karma dla kotów", Price = 129m, Description = "Sucha karma dla dorosłych kotów z kurczakiem" },
                new Product { Id = 3, Name = "Tetra AquaSafe 500ml", Category = "Akwarystyka", Price = 45m, Description = "Uzdatniacz wody do akwarium, neutralizuje chlor" },
                new Product { Id = 4, Name = "Trixie Drapak XL 150cm", Category = "Akcesoria dla kotów", Price = 399m, Description = "Wysoki drapak z platformami i domkiem" },
                new Product { Id = 5, Name = "Kong Classic Large", Category = "Zabawki dla psów", Price = 69m, Description = "Wytrzymała zabawka do napełniania smakołykami" },
                new Product { Id = 6, Name = "Ferplast Klatka dla chomika", Category = "Gryzonie", Price = 189m, Description = "Klatka 60x40cm z wyposażeniem" },
                new Product { Id = 7, Name = "Flexi Smycz automatyczna 8m", Category = "Akcesoria dla psów", Price = 119m, Description = "Smycz zwijana dla psów do 50kg" },
                new Product { Id = 8, Name = "Brit Premium Kitten 8kg", Category = "Karma dla kotów", Price = 159m, Description = "Karma dla kociąt do 12 miesiąca życia" },
                new Product { Id = 9, Name = "JBL ProFlora CO2 Set", Category = "Akwarystyka", Price = 549m, Description = "Kompletny zestaw CO2 dla roślin akwariowych" },
                new Product { Id = 10, Name = "Vitapol Siano dla królików 1kg", Category = "Gryzonie", Price = 25m, Description = "Naturalne siano łąkowe, podstawa diety" }
            );
        }
    }
}
