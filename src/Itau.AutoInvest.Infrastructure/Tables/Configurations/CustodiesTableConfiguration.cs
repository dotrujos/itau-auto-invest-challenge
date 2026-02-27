using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itau.AutoInvest.Infrastructure.Tables.Configurations;

public class CustodiesTableConfiguration : IEntityTypeConfiguration<CustodiesTable>
{
    public void Configure(EntityTypeBuilder<CustodiesTable> builder)
    {
        builder.HasOne(c => c.GraphicalAccount)
            .WithMany()
            .HasForeignKey(c => c.GraphicalAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Ticker).HasMaxLength(10);
        builder.Property(x => x.AvaragePrice).HasPrecision(18, 2);
    }
}