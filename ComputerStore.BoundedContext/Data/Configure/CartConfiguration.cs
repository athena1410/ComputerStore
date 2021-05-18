//-----------------------------------------------------------------------
// <copyright file="CartConfiguration.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.BoundedContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComputerStore.BoundedContext.Data.Configure
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.Property(e => e.CreatedDate).HasColumnType("datetime");

            builder.Property(e => e.Quantity)
                .IsRequired();

            builder.Property(e => e.UpdatedDate).HasColumnType("datetime");

            builder.HasOne(d => d.User)
                .WithMany(p => p.Cart)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Cart_User");

            builder.HasOne(d => d.Product)
                .WithMany(p => p.Cart)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Cart_Product");

            builder.HasOne(d => d.Website)
                .WithMany(p => p.Cart)
                .HasForeignKey(d => d.WebsiteId)
                .HasConstraintName("FK_Cart_Website");
        }
    }
}
