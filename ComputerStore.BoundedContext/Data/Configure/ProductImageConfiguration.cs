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
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.Property(e => e.CreatedDate).HasColumnType("datetime");

            builder.Property(e => e.ImageUrl)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.UpdatedDate).HasColumnType("datetime");

            builder.HasOne(d => d.Product)
                .WithMany(p => p.ProductImage)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ProductImage_Product");
        }
    }
}
