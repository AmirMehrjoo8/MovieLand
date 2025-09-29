using Microsoft.EntityFrameworkCore;
using MovieLand.Models;

namespace MovieLand.Models.Context
{
    public class MovieLandDbContext : DbContext
    {
        public MovieLandDbContext(DbContextOptions<MovieLandDbContext> options) : base(options)
        {
        }

        // DbSet properties
        public DbSet<User> Users { get; set; }
        public DbSet<SubCard> SubCards { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<DiscountCode> DiscountCodes { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User → SubCard (many-to-one)
            modelBuilder.Entity<User>()
                .HasOne(u => u.SubCard)
                .WithMany(s => s.Users)
                .HasForeignKey(u => u.SubCardId)
                .OnDelete(DeleteBehavior.Restrict);

            // Transaction → User (many-to-one)
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Transaction → SubCard (many-to-one)
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.SubCard)
                .WithMany(s => s.Transactions)
                .HasForeignKey(t => t.SubCardId)
                .OnDelete(DeleteBehavior.Restrict);

            // Comment → User (many-to-one)
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SubCard>().HasData(
                new SubCard
                {
                    SubCardId = 1,
                    Credit = 0,
                    Price = 0
                },
                new SubCard
                {
                    SubCardId = 2,
                    Credit = 1,
                    Price = 119000
                },
                new SubCard
                {
                    SubCardId = 3,
                    Credit = 6,
                    Price = 498000
                },
                new SubCard
                {
                    SubCardId = 4,
                    Credit = 12,
                    Price = 697000
                }
            );


            modelBuilder.Entity<User>().HasData(new User
            {
                UserId = 1,
                SubCardId = 1,
                Username = "admin",
                Name = "ادمین اصلی",
                Password = "12345678",
                Email = "amirmehrjoo08@gmail.com",
                Phone = "09304310044",
                RegisterDate = new DateTime(2025, 1, 1),
                Type = 1,
                SubStartDate = new DateTime(2025, 1, 1),
                SubExpireDate = new DateTime(2025, 12, 12)
            });
        }
    }
}
