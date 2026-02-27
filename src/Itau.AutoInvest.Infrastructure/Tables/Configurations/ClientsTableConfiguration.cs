using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itau.AutoInvest.Infrastructure.Tables.Configurations;

public class ClientsTableConfiguration : IEntityTypeConfiguration<ClientsTable>
{
    public void Configure(EntityTypeBuilder<ClientsTable> builder)
    {
        builder.Property(x => x.Id).HasColumnType("BIGINT");
        
        builder.HasIndex(x => x.Cpf).IsUnique();
        
        builder.Property(x => x.MonthlyValue).HasPrecision(18, 4);
        
        builder.Property(x => x.Cpf).HasMaxLength(11);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Email).HasMaxLength(200);
        
        builder.Property(x => x.IsActive).HasDefaultValue(true);
    }
}