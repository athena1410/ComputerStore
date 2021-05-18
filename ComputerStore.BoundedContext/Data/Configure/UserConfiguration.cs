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
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(e => e.CreatedDate).HasColumnType("datetime");

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(e => e.FirstName).HasMaxLength(100);

            builder.Property(e => e.LastLogin).HasColumnType("datetime");

            builder.Property(e => e.LastName).HasMaxLength(100);

            builder.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.Phone).HasMaxLength(50);

            builder.Property(e => e.Status).HasDefaultValueSql("((0))");

            builder.Property(e => e.UpdatedDate).HasColumnType("datetime");

            builder.HasOne(d => d.Role)
                .WithMany(p => p.User)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");

            builder.HasOne(d => d.Website)
                .WithMany(p => p.User)
                .HasForeignKey(d => d.WebsiteId)
                .HasConstraintName("FK_User_Website");
        }
    }
}
