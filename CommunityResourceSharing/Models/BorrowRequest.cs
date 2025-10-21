using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommunityResourceSharing.Models
{
    public class BorrowRequest
    {
        [Key]
        public int Id { get; set; }
        public int? ResourceId { get; set; }
        [ForeignKey("ResourceId")]
        public Resource? Resource { get; set; }

        public int? BorrowerId { get; set; }
        [ForeignKey("BorrowerId")]
        public AppUser? Borrower { get; set; }

        [Required]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Returned

        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public DateTime? ReturnDate { get; set; } // Nullable for not-yet-returned
    }
}
