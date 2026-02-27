using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itau.AutoInvest.Infrastructure.Tables.Configurations;

public class CurrenciesTableConfiguration : IEntityTypeConfiguration<CurrenciesTable>
{
    public void Configure(EntityTypeBuilder<CurrenciesTable> builder)
    {
        builder.Property(x => x.Ticker).HasMaxLength(10);
        builder.Property(x => x.ClosingPrice).HasPrecision(18, 4);
        builder.Property(x => x.MaximumPrice).HasPrecision(18, 4);
        builder.Property(x => x.MinimumPrice).HasPrecision(18, 4);
        builder.Property(x => x.OpeningPrice).HasPrecision(18, 4);
    }
}