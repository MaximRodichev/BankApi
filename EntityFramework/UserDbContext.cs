using BankApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApi.EntityFramework
{
    public class UserDbContext : DbContext
    {
         public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Users");
            modelBuilder.Entity<UserModel>().HasKey(x=> x.UserId);
            modelBuilder.Entity<UserModel>().Property(x=> x.UserId).IsRequired().ValueGeneratedOnAdd();
        }
        public DbSet<UserModel> Users { get; set; }
    }
}
