using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Threading.Tasks;
namespace PAUP.Models
{
    public class IgreDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<GameOnOffer> GamesOnOffer { get; set; }
        public DbSet<GameOnOrder> GamesOnOrder { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(@"server=localhost;port=3306;database=AlenPaup;user=root;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "Admin",
                    Surname = "Admin",
                    Adress = "",
                    Email = "admin@admin.admin",
                    AccountType = 1,
                    Password = Convert.ToBase64String(System.Security.Cryptography.SHA256.Create()
                                .ComputeHash(Encoding.UTF8.GetBytes("admin123")))
                
                }
            );
            modelBuilder.Entity<Game>().HasData(
                new Game
                {
                    Id = 1,
                    Name = "GTA San Andreas",
                    Amount = 3,
                    Price = (decimal)55.25,
                    Category = (int)Category.Avanturisticka,
                    ImgUrl = "/images/Games/default.png"
                },
                new Game
                {
                    Id = 2,
                    Name = "FIFA 2020",
                    Amount = 5,
                    Price = (decimal)125.99,
                    Category = (int)Category.Sportska,
                    ImgUrl = "/images/Games/default.png"
                }
            );
            modelBuilder.Entity<Order>()
            .Property(r => r.HasOffer)
            .HasConversion(new BoolToZeroOneConverter<Int16>());
            
            
        }
    }
}