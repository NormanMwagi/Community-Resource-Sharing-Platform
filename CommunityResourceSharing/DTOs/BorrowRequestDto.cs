using CommunityResourceSharing.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommunityResourceSharing.DTOs
{
    public class BorrowRequestDto
    {
        public int? Id { get; set; }
        public int? ResourceId { get; set; }

        public int? BorrowerId { get; set; }

        [Required, MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Returned
        public DateTime? ReturnDate { get; set; } // Nullable for not-yet-returned
    }
}
