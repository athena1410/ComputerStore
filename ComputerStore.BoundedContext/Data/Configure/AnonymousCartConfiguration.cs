//-----------------------------------------------------------------------
// <copyright file="AnonymousCartConfiguration.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.BoundedContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComputerStore.BoundedContext.Data.Configure
{
    public class AnonymousCartConfiguration : IEntityTypeConfiguration<AnonymousCart>
    {
        public void Configure(EntityTypeBuilder<AnonymousCart> builder)
        {
            builder.Property(e => e.CreatedDate).HasColumnType("datetime");

            builder.Property(e => e.Quantity)
                .IsRequired();

            builder.Property(e => e.UpdatedDate).HasColumnType("datetime");

            builder.HasOne(d => d.Website)
                .WithMany(p => p.AnonymousCart)
                .HasForeignKey(d => d.WebsiteId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_AnonymousCart_User");

            builder.HasOne(d => d.Product)
                .WithMany(p => p.AnonymousCart)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_AnonymousCartt_Product");
        }
    }
}
