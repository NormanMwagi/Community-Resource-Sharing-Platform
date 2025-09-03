using System.ComponentModel.DataAnnotations;

namespace CommunityResourceSharing.Models
{
    public class Users
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required, EmailAddress, MaxLength(191)]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        public bool isAdmin { get; set; } = false; //role

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Resource>? Resources { get; set; }
        public ICollection<BorrowRequest>? BorrowRequests { get; set; }
    }
}
