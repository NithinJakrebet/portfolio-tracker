using Microsoft.EntityFrameworkCore;
using PortfolioTrackerApi.Models;

namespace PortfolioTrackerApi.Data
{
    public static class DbSeeder
    {
        private static readonly string[] Symbols =
        {
            "AAPL","MSFT","AMZN","NVDA","GOOGL","META","BRK.B","TSLA","LLY","JPM",
            "V","UNH","XOM","MA","PG","AVGO","JNJ","COST","HD","ADBE"
        };

        private static readonly string[] Exchanges = { "NASDAQ", "NYSE" };
        private static readonly string[] FirstNames = { "Alex", "Jordan", "Taylor", "Casey", "Morgan", "Riley", "Sam" };
        private static readonly string[] LastNames  = { "Lee", "Patel", "Garcia", "Nguyen", "Smith", "Brown", "Kim" };

        public static async Task SeedAsync(AppDbContext db)
        {
            // Only seed if empty
            if (await db.Users.AnyAsync()) return;

            var rand = new Random();

            // === Users ===
            var users = new List<User>();
            for (int i = 0; i < 5; i++)
            {
                var first = FirstNames[rand.Next(FirstNames.Length)];
                var last  = LastNames[rand.Next(LastNames.Length)];
                var full  = $"{first} {last}";
                var email = $"{first.ToLower()}.{last.ToLower()}{rand.Next(100, 999)}@example.com";

                users.Add(new User
                {
                    UserId = Guid.NewGuid(),
                    Email = email,
                    FullName = full,
                    PasswordHash = $"hash-{Guid.NewGuid()}",
                    IsActive = true
                });
            }

            await db.Users.AddRangeAsync(users);
            await db.SaveChangesAsync();

            // === Portfolios ===
            var portfolios = new List<Portfolio>();

            foreach (var user in users)
            {
                int portfolioCount = rand.Next(1, 4); // 1–3 portfolios per user

                for (int i = 0; i < portfolioCount; i++)
                {
                    var symbol = Symbols[rand.Next(Symbols.Length)];
                    var label  = $"{symbol} Portfolio {i + 1}";

                    portfolios.Add(new Portfolio
                    {
                        PortfolioId = Guid.NewGuid(),
                        UserId = user.UserId,
                        User = user,
                        OwnerName = user.FullName,
                        Label = label,
                        CashBalance = (decimal)(rand.NextDouble() * 50000.0 + 500.0)
                    });
                }
            }

            await db.Portfolios.AddRangeAsync(portfolios);
            await db.SaveChangesAsync();

            // === Holdings ===
            var holdings = new List<Holding>();

            foreach (var p in portfolios)
            {
                int holdingCount = rand.Next(3, 9); // 3–8 holdings
                var symbols = Symbols.OrderBy(_ => rand.Next()).Take(holdingCount).ToList();

                foreach (var sym in symbols)
                {
                    holdings.Add(new Holding
                    {
                        HoldingId = Guid.NewGuid(),
                        PortfolioId = p.PortfolioId,
                        Portfolio = p,
                        Symbol = sym,
                        Exchange = Exchanges[rand.Next(Exchanges.Length)],
                        Quantity = Math.Round((decimal)(rand.NextDouble() * 250.0 + 1.0), 6),
                        AvgCostBasis = Math.Round((decimal)(rand.NextDouble() * 480.0 + 20.0), 2)
                    });
                }
            }

            await db.Holdings.AddRangeAsync(holdings);
            await db.SaveChangesAsync();

            // === Transactions ===
            var transactions = new List<Transaction>();

            foreach (var p in portfolios)
            {
            int txCount = rand.Next(10, 31); // 10–30 tx per portfolio
            var userId = p.UserId;

            var portfolioSymbols = holdings
                  .Where(h => h.PortfolioId == p.PortfolioId)
                  .Select(h => h.Symbol)
                  .Distinct()
                  .ToList();

            for (int i = 0; i < txCount; i++)
            {
                  var kind = (TransactionType)rand.Next(0, 8); // 0..7
                  string currency = "USD";

                  string? symbol = null;
                  decimal? quantity = null;
                  decimal? price = null;
                  decimal grossAmount = 0m;
                  decimal fee = 0m;

                  // If we picked a position-affecting type but have no symbols,
                  // downgrade to a pure cash op (Deposit) so DB constraints are satisfied.
                  bool needsSymbol =
                        kind is TransactionType.Buy
                        or TransactionType.Sell
                        or TransactionType.Split
                        or TransactionType.Dividend
                        or TransactionType.Fee;

                  if (needsSymbol && portfolioSymbols.Count == 0)
                  {
                        kind = TransactionType.Deposit;
                  }

                  if (kind is TransactionType.Buy or TransactionType.Sell)
                  {
                        // We know portfolioSymbols.Count > 0 here
                        symbol = portfolioSymbols[rand.Next(portfolioSymbols.Count)];
                        quantity = Math.Round((decimal)(rand.NextDouble() * 50.0 + 1.0), 6);
                        price = Math.Round((decimal)(rand.NextDouble() * 480.0 + 20.0), 4);

                        var signed = quantity.Value * price.Value;
                        grossAmount = kind == TransactionType.Buy ? -signed : signed;
                        fee = Math.Round((decimal)(rand.NextDouble() * 10.0), 2);
                  }
                  else if (kind == TransactionType.Deposit)
                  {
                        grossAmount = Math.Round((decimal)(rand.NextDouble() * 4900.0 + 100.0), 2);
                  }
                  else if (kind == TransactionType.Withdrawal)
                  {
                        grossAmount = -Math.Round((decimal)(rand.NextDouble() * 4900.0 + 100.0), 2);
                  }
                  else if (kind == TransactionType.Dividend)
                  {
                        symbol = portfolioSymbols[rand.Next(portfolioSymbols.Count)];
                        grossAmount = Math.Round((decimal)(rand.NextDouble() * 200.0 + 1.0), 2);
                  }
                  else if (kind == TransactionType.Fee)
                  {
                        // DB constraint requires Symbol NOT NULL for Fee as well
                        if (portfolioSymbols.Count > 0)
                        {
                        symbol = portfolioSymbols[rand.Next(portfolioSymbols.Count)];
                        }
                        else
                        {
                        // Fallback symbol just to satisfy constraint; could represent
                        // a generic "cash account" if needed.
                        symbol = "CASH";
                        }

                        fee = Math.Round((decimal)(rand.NextDouble() * 10.0 + 1.0), 2);
                        grossAmount = -fee;
                  }
                  else if (kind == TransactionType.Interest)
                  {
                        grossAmount = Math.Round((decimal)(rand.NextDouble() * 50.0 + 1.0), 2);
                  }
                  else if (kind == TransactionType.Split)
                  {
                        // Constraint requires Quantity > 0 AND Price >= 0 AND Symbol NOT NULL
                        symbol = portfolioSymbols[rand.Next(portfolioSymbols.Count)];
                        quantity = Math.Round((decimal)(rand.NextDouble() * 10.0 + 1.0), 6);
                        price = 1.0m; // arbitrary non-negative price just to satisfy constraint
                        // For a split you might choose to keep GrossAmount = 0
                        grossAmount = 0m;
                  }

                  var executedAt = DateTime.UtcNow.AddDays(-rand.Next(0, 60));

                  transactions.Add(new Transaction
                  {
                        TransactionId = Guid.NewGuid(),
                        PortfolioId = p.PortfolioId,
                        Portfolio = p,
                        UserId = userId,
                        Type = kind,
                        Symbol = symbol,
                        Quantity = quantity,
                        Price = price,
                        GrossAmount = grossAmount,
                        Fee = fee,
                        Currency = currency,
                        ExternalRef = $"ORD-{rand.Next(10000000, 99999999)}",
                        Notes = null,
                        ExecutedAt = executedAt
                  });
            }
            }

            await db.Transactions.AddRangeAsync(transactions);
            await db.SaveChangesAsync();

        }
    }
}