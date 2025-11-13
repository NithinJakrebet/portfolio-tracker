using Microsoft.EntityFrameworkCore;

namespace PortfolioTrackerApi.Models
{
    public class Holding
    {
        public Guid HoldingId { get; set; }
        public Guid PortfolioId { get; set; }
        public Portfolio? Portfolio { get; set; }
        public required string Symbol { get; set; }
        public string? Exchange { get; set; }
        [Precision(18,6)] public decimal Quantity { get; set; }
        [Precision(18,2)] public decimal AvgCostBasis { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}