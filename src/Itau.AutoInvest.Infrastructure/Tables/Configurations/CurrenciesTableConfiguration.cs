using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itau.AutoInvest.Infrastructure.Tables.Configurations;

public class CurrenciesTableConfiguration : IEntityTypeConfiguration<StockQuoteTable>
{
    public void Configure(EntityTypeBuilder<StockQuoteTable> builder)
    {
        builder.Property(x => x.Ticker).HasMaxLength(12);
        builder.Property(x => x.ClosingPrice).HasPrecision(18, 4);
        builder.Property(x => x.MaximumPrice).HasPrecision(18, 4);
        builder.Property(x => x.MinimumPrice).HasPrecision(18, 4);
        builder.Property(x => x.OpeningPrice).HasPrecision(18, 4);
    }
}