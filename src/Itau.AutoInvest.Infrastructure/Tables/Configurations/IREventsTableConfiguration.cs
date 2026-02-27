using Itau.AutoInvest.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itau.AutoInvest.Infrastructure.Tables.Configurations;

public class IREventsTableConfiguration : IEntityTypeConfiguration<IREventsTable>
{
    public void Configure(EntityTypeBuilder<IREventsTable> builder)
    {
        builder.HasOne(i => i.Client)
            .WithMany()
            .HasForeignKey(i => i.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.EventType)
            .HasConversion<string>(
                v => v.ToString().ToUpper(),
                v => Enum.Parse<IREventType>(v))
            .HasColumnType("ENUM('DEDO_DURO','IR_VENDA')");

        builder.Property(x => x.BaseValue).HasPrecision(18, 2);
        builder.Property(x => x.IRValue).HasPrecision(18, 2);
    }
}