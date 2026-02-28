using Itau.AutoInvest.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itau.AutoInvest.Infrastructure.Tables.Configurations;

public class BuyOrderTableConfiguration : IEntityTypeConfiguration<BuyOrderTable>
{
    public void Configure(EntityTypeBuilder<BuyOrderTable> builder)
    {
        builder.Property(x => x.Ticker).HasMaxLength(12);
        builder.Property(x => x.UnitPrice).HasPrecision(18, 4);
        builder.Property(x => x.MarketType)
            .HasConversion<string>(
                v => v.ToString().ToUpper(),
                v => Enum.Parse<MarketType>(v))
            .HasColumnType("ENUM('LOTE','FRACIONARIO')");
    }
}