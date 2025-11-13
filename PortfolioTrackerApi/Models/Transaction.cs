using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PortfolioTrackerApi.Models
{
    [Index(nameof(PortfolioId), nameof(ExecutedAt))]
    [Index(nameof(PortfolioId), nameof(Symbol), nameof(ExecutedAt))]
    public class Transaction
    {
        public Guid TransactionId { get; set; }
        public Guid PortfolioId { get; set; }
        public Portfolio? Portfolio { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public TransactionType Type { get; set; }
        [MaxLength(12)] public string? Symbol { get; set; }               
        [Precision(18, 6)] public decimal? Quantity { get; set; }            
        [Precision(18, 6)] public decimal? Price { get; set; }              
        [Precision(18, 2)] public decimal GrossAmount { get; set; }          
        [Precision(18, 2)] public decimal Fee { get; set; } = 0.00m;
        [MaxLength(3)]  public required string Currency { get; set; }     
        public string? ExternalRef { get; set; }          
        public string? Notes { get; set; }
        public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt  { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt  { get; set; } = DateTime.UtcNow;
    }

    public enum TransactionType
    {
        [Description("Buy")]        Buy,
        [Description("Sell")]       Sell,
        [Description("Deposit")]    Deposit,
        [Description("Withdrawal")] Withdrawal,
        [Description("Dividend")]   Dividend,
        [Description("Split")]      Split,
        [Description("Fee")]        Fee,
        [Description("Interest")]   Interest,
    }
}
