using System.ComponentModel.DataAnnotations;

namespace CommunityResourceSharing.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsAdmin { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // For Create/Update requests
    public class CreateUserDto
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required, EmailAddress, MaxLength(191)]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!; 

        public bool IsAdmin { get; set; } = false;
    }
}
