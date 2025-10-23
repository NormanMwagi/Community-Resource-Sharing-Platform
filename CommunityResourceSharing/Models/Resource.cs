using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommunityResourceSharing.Models
{
    public class Resource
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; } = null!;

        [MaxLength(500)]
        public string Description { get; set; } = null!;

        [Required, MaxLength(50)]
        public string Category { get; set; } = null!; // e.g., Books, Tools, Equipment

        [Required]
        public string Status { get; set; } = "Available"; // Available, Borrowed, Unavailable

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key
        public string? OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public AppUser? Owner { get; set; }        // Navigation
        public ICollection<BorrowRequest>? BorrowRequests { get; set; }
    }
}
