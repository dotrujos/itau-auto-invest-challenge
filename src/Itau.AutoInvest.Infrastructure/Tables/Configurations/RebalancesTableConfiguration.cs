using Itau.AutoInvest.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itau.AutoInvest.Infrastructure.Tables.Configurations;

public class RebalancesTableConfiguration : IEntityTypeConfiguration<RebalancesTable>
{
    public void Configure(EntityTypeBuilder<RebalancesTable> builder)
    {
        builder.HasOne(r => r.Client)
            .WithMany()
            .HasForeignKey(r => r.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.RebalanceType)
            .HasConversion<string>(
                v => v.ToString().ToUpper(),
                v => Enum.Parse<RebalanceType>(v, true))
            .HasColumnType("ENUM('MUNDANCA_CESTA', 'DESVIO')");

        builder.Property(x => x.TickerPurchased).HasMaxLength(12);
        builder.Property(x => x.TickerSold).HasMaxLength(12);
        builder.Property(x => x.SalesValue).HasPrecision(18, 2);
    }
}