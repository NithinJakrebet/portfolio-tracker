using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PortfolioTrackerApi.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        public Guid UserId { get; set; }
        [Required, EmailAddress, MaxLength(255)] public required string Email { get; set; }
        public ICollection<Portfolio> Portfolios { get; set; } = new List<Portfolio>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public required string PasswordHash { get; set; }
        public required string FullName { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
