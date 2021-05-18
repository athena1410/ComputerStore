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
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(e => e.CreatedDate).HasColumnType("datetime");

            builder.Property(e => e.OrderState).HasDefaultValueSql("((0))");

            builder.Property(e => e.PaymentState).HasDefaultValueSql("((0))");

            builder.Property(e => e.Phone).HasMaxLength(20);

            builder.Property(e => e.ShipAddress).HasMaxLength(255);

            builder.Property(e => e.Status).HasDefaultValueSql("((0))");

            builder.Property(e => e.UpdatedDate).HasColumnType("datetime");

            builder.HasOne(d => d.User)
                .WithMany(p => p.Order)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_User");

            builder.HasOne(d => d.Website)
                .WithMany(p => p.Order)
                .HasForeignKey(d => d.WebsiteId)
                .HasConstraintName("FK_Order_Website");
        }
    }
}
