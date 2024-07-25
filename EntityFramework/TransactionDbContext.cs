using BankApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApi.EntityFramework
{
    public class TransactionDbContext : DbContext
    {
        public TransactionDbContext(DbContextOptions<TransactionDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Transactions");
            modelBuilder.Entity<TransactionModel>().HasKey(x => x.id);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        public DbSet<TransactionModel> Transactions { get; set; }
    }
}
