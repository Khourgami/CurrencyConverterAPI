using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CurrencyConverter.Domain.Entities;

namespace CurrencyConverter.Infrastructure.Persistence.EntityTypeConfigurations
{
    public class CurrencyConversionHistoryConfiguration : IEntityTypeConfiguration<CurrencyConversionHistory>
    {
        public void Configure(EntityTypeBuilder<CurrencyConversionHistory> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.SourceCurrency)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(c => c.TargetCurrency)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(c => c.Rate)
                .HasPrecision(18, 6);

            builder.Property(c => c.ResultAmount)
                .HasPrecision(18, 2);

            builder.Property(c => c.ConvertedAt)
                .IsRequired();
        }
    }
}
