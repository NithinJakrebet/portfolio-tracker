namespace PortfolioTrackerApi.Models
{
      public class Portfolio
      {
            public Guid PortfolioId { get; set; }
            public ICollection<Holding> Holdings { get; set; } = new List<Holding>();
            public Guid UserId { get; set; }
            public required User User { get; set; }
            public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

            public required string OwnerName { get; set; }
            public required string Label { get; set; }
            public decimal CashBalance { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

      }
      
}
