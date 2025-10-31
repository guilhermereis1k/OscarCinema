using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OscarCinema.Domain.Entities.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Infrastructure.EntitiesConfiguration
{
    public class ExhibitionTypeConfiguration : IEntityTypeConfiguration<ExhibitionType>
    {
        public void Configure(EntityTypeBuilder<ExhibitionType> builder)
        {
            builder.ToTable("ExhibitionTypes");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.Description)
                .HasMaxLength(500);

            builder.Property(e => e.TechnicalSpecs)
                .HasMaxLength(200);

            builder.Property(e => e.Price)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(e => e.IsActive)
                .IsRequired();

        }
    }
}
