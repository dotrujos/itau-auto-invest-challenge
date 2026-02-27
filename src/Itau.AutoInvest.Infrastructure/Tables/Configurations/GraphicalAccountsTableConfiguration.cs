using Itau.AutoInvest.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static System.Enum;

namespace Itau.AutoInvest.Infrastructure.Tables.Configurations;

public class GraphicalAccountsTableConfiguration : IEntityTypeConfiguration<GraphicalAccountsTable>
{
    public void Configure(EntityTypeBuilder<GraphicalAccountsTable> builder)
    {
        builder.HasIndex(x => x.AccountNumber).IsUnique();
        
        builder.Property(x => x.AccountNumber).HasMaxLength(20);
        builder.Property(x => x.AccountType)
            .HasConversion<string>(
                v => v.ToString().ToUpper(),
                v => Enum.Parse<AccountType>(v))
            .HasColumnType("ENUM('MASTER', 'FILHOTE')");

        builder.HasOne(a => a.Client)
            .WithOne(c => c.GraphicalAccountTable)
            .HasForeignKey<GraphicalAccountsTable>(a => a.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}