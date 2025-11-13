using Microsoft.EntityFrameworkCore;
using PortfolioTrackerApi.Models;

namespace PortfolioTrackerApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
        public DbSet<Portfolio> Portfolios => Set<Portfolio>();
        public DbSet<Holding> Holdings => Set<Holding>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Transaction> Transactions => Set<Transaction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaction>()
               .HasOne(t => t.User)
               .WithMany(u => u.Transactions)
               .HasForeignKey(t => t.UserId)
               .OnDelete(DeleteBehavior.Cascade);
                
             modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Portfolio)
                .WithMany(p => p.Transactions)
                .HasForeignKey(t => t.PortfolioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Portfolio>()
                .HasOne(p => p.User)
                .WithMany(u => u.Portfolios)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Holding>()
                .HasOne(h => h.Portfolio)
                .WithMany(p => p.Holdings)
                .HasForeignKey(h => h.PortfolioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(t => t.Type).HasConversion<string>();

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint(
                        "CK_Tx_GrossAmount_Sign",
                        "CASE " +
                        " WHEN Type IN ('Deposit','Dividend','Interest') THEN GrossAmount >= 0 " +
                        " WHEN Type IN ('Withdrawal','Fee') THEN GrossAmount <= 0 " +
                        " ELSE 1 END");

                    t.HasCheckConstraint(
                        "CK_Tx_SymbolRequired_For_Positions",
                        "CASE " +
                        " WHEN Type IN ('Buy','Sell','Split','Fee','Dividend') THEN Symbol IS NOT NULL " +
                        " ELSE 1 END");

                    t.HasCheckConstraint(
                        "CK_Tx_Positions_Quantity_Price",
                        "CASE " +
                        " WHEN Type IN ('Buy','Sell','Split') THEN Quantity IS NOT NULL AND Quantity > 0 AND Price IS NOT NULL AND Price >= 0 " +
                        " ELSE 1 END");
                });
            });
        }
    }
}
