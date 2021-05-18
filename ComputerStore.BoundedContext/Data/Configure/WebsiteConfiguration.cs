//-----------------------------------------------------------------------
// <copyright file="CategoryController.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.BoundedContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComputerStore.BoundedContext.Data.Configure
{
    public class WebsiteConfiguration : IEntityTypeConfiguration<Website>
    {
        public void Configure(EntityTypeBuilder<Website> builder)
        {
            builder.Property(e => e.CreatedDate).HasColumnType("datetime");

            builder.Property(e => e.LogoUrl).HasMaxLength(255);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Note).HasMaxLength(255);

            builder.Property(e => e.Status).HasDefaultValueSql("((0))");

            builder.Property(e => e.UpdatedDate).HasColumnType("datetime");

            builder.Property(e => e.UrlPath)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.SecretKey)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(x => x.SecretKey).IsUnique();

            builder.HasOne(d => d.Company)
                .WithOne(p => p.Website)
                .HasForeignKey<Website>(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Website_Company");
            builder.HasIndex(x => x.Name).IsUnique();
            builder.HasIndex(x => x.UrlPath).IsUnique();
        }
    }
}
