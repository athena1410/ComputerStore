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
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(e => e.CreatedDate).HasColumnType("datetime");

            builder.Property(e => e.Discount).HasDefaultValueSql("((0))");

            builder.Property(e => e.Name)
                .IsRequired()                
                .HasMaxLength(100);

            builder.Property(e => e.Price).HasDefaultValueSql("((0))");

            builder.Property(e => e.ProductCode)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(e => e.Quantity).HasDefaultValueSql("((1))");

            builder.Property(e => e.Status).HasDefaultValueSql("((0))");

            builder.Property(e => e.UpdatedDate).HasColumnType("datetime");

            builder.Property(e => e.ViewCount).HasDefaultValueSql("((0))");

            builder.Property(e => e.Warranty).HasDefaultValueSql("((12))");

            builder.HasOne(d => d.Category)
                .WithMany(p => p.Product)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Category");

            builder.HasOne(d => d.Website)
                .WithMany(p => p.Product)
                .HasForeignKey(d => d.WebsiteId)
                .HasConstraintName("FK_Product_Website");
        }
    }
}
