using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CommunityResourceSharing.Models
{
    public class AppUser : IdentityUser
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = null!;
        [Required]
        public bool isAdmin { get; set; } = false; //role

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Resource>? Resources { get; set; }
        public ICollection<BorrowRequest>? BorrowRequests { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
