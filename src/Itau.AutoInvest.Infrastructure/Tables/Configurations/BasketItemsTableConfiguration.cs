using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itau.AutoInvest.Infrastructure.Tables.Configurations;

public class BasketItemsTableConfiguration : IEntityTypeConfiguration<BasketItemsTable>
{
    public void Configure(EntityTypeBuilder<BasketItemsTable> builder)
    {
        builder.Property(x => x.Ticker).HasMaxLength(12);
        builder.Property(x => x.Percentage).HasPrecision(5, 2);

        builder.HasOne(bi => bi.ParentBasket)
            .WithMany(br => br.Items)
            .HasForeignKey(bi => bi.ParentBasketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}