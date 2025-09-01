using CommunityResourceSharing.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityResourceSharing.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<BorrowRequest> BorrowRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Users>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Resource>().HasOne(r => r.Owner).
                WithMany(y => y.Resources)
                .HasForeignKey(s => s.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BorrowRequest>()
                .HasOne(br => br.Resource)
                .WithMany(r => r.BorrowRequests)
                .HasForeignKey(br => br.ResourceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BorrowRequest>()
                .HasOne(br => br.Borrower)
                .WithMany(u => u.BorrowRequests)
                .HasForeignKey(br => br.BorrowerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
