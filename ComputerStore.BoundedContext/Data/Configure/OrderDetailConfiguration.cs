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
    public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.HasKey(e => new { e.OrderId, e.ProductId });

            builder.Property(e => e.Discount).HasDefaultValueSql("((0))");

            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.Property(e => e.Quantity).HasDefaultValueSql("((1))");

            builder.HasOne(d => d.Order)
                .WithMany(p => p.OrderDetail)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetail_Order");

            builder.HasOne(d => d.Product)
                .WithMany(p => p.OrderDetail)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetail_Product");
        }
    }
}
