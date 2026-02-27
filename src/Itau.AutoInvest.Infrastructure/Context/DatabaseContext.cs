using Itau.AutoInvest.Infrastructure.Tables;
using Microsoft.EntityFrameworkCore;

namespace Itau.AutoInvest.Infrastructure.Context;

public class DatabaseContext : DbContext
{
    public DbSet<BasketItemsTable> BasketItems { get; set; }
    public DbSet<BasketRecommendationTable> BasketRecommendation { get; set; }
    public DbSet<BuyOrderTable> BuyOrder { get; set; }
    public DbSet<ClientsTable> Clients { get; set; }
    public DbSet<CurrenciesTable> Currencies { get; set; }
    public DbSet<CustodiesTable> Custodies { get; set; }
    public DbSet<DistributionsTable> Distributions { get; set; }
    public DbSet<IREventsTable> IREvents { get; set; }
    public DbSet<RebalancesTable> Rebalances { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}