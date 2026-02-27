using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itau.AutoInvest.Infrastructure.Tables.Configurations;

public class BasketRecommendationTableConfiguration : IEntityTypeConfiguration<BasketRecommendationTable>
{
    public void Configure(EntityTypeBuilder<BasketRecommendationTable> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(100);
    }
}