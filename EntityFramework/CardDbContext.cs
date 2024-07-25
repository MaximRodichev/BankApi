using BankApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApi.EntityFramework
{
    public class CardDbContext : DbContext
    {
        public CardDbContext(DbContextOptions<CardDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Cards");
            modelBuilder.Entity<CardModel>().HasKey(x => x.CardNumber);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        public DbSet<CardModel> Cards { get; set; }
    }
}
