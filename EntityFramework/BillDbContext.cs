using BankApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApi.EntityFramework
{
    public class BillDbContext : DbContext
    {
        public BillDbContext(DbContextOptions<BillDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Bills");
            modelBuilder.Entity<BillModel>().HasKey(x => x.number);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        public DbSet<BillModel> Bills { get; set; }

        public static void UpdateEntity(BillModel bill)
        {
            
        }
    }
}
