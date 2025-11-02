using System.ComponentModel.DataAnnotations;

namespace CommunityResourceSharing.DTOs
{
    public class ResourceDto
    {
        public int? Id { get; set; }   // nullable so not required on POST

        [Required, MaxLength(150)]
        public string Title { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; } = null!;

        [Required, MaxLength(50)]
        public string Category { get; set; } = null!;

        public string Status { get; set; } = "Available";

        public int? OwnerId { get; set; }
    }
    //post(don't need id)
    public class ResourceCreateDto
    {
        [Required, MaxLength(150)]
        public string Title { get; set; } = null!;
        [MaxLength(500)]
        public string? Description { get; set; } = null!;
        [Required, MaxLength(50)]
        public string Category { get; set; } = null!;
        public int? OwnerId { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; //
    }
}
