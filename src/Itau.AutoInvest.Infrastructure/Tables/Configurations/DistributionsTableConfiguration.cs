using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itau.AutoInvest.Infrastructure.Tables.Configurations;

public class DistributionsTableConfiguration : IEntityTypeConfiguration<DistributionsTable>
{
    public void Configure(EntityTypeBuilder<DistributionsTable> builder)
    {
        builder.Property(x => x.Ticker).HasMaxLength(12);
        builder.Property(x => x.UnitPrice).HasPrecision(18, 4);
    }
}