using CommunityResourceSharing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CommunityResourceSharing.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<BorrowRequest> BorrowRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Identity User Login configuration
            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.Property(l => l.LoginProvider)
                    .HasMaxLength(128)
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8mb4");  // changed to utf8mb4

                entity.Property(l => l.ProviderKey)
                    .HasMaxLength(128)
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8mb4");  // changed to utf8mb4

                entity.Property(l => l.UserId).HasMaxLength(128);
            });

            // Identity User Token configuration
            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.Property(t => t.LoginProvider)
                    .HasMaxLength(128)
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8mb4");  // changed

                entity.Property(t => t.Name)
                    .HasMaxLength(128)
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8mb4");  // changed

                entity.Property(t => t.UserId).HasMaxLength(128);
            });

            // Identity User Role configuration
            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.Property(ur => ur.UserId)
                    .HasMaxLength(128)
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8mb4");  // changed

                entity.Property(ur => ur.RoleId)
                    .HasMaxLength(128)
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8mb4");  // changed
            });

         

            modelBuilder.Entity<Resource>().HasOne(r => r.Owner).
                WithMany(y => y.Resources)
                .HasForeignKey(s => s.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);


            // Seed Resource (Owned by Admin User)
            modelBuilder.Entity<Resource>().HasData(
                new Resource
                {
                    Id = 1,
                    Title = "Book",
                    Description = "Encyclopedia",
                    Category = "Books",
                    Status = "Available",
                    OwnerId = 1,
                    CreatedAt = DateTime.UtcNow
                }
            );

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
