//-----------------------------------------------------------------------
// <copyright file="RefreshTokenConfiguration.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------
using ComputerStore.BoundedContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComputerStore.BoundedContext.Data.Configure
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.Property(e => e.Expires).HasColumnType("datetime");

            builder.Property(e => e.CreatedDate).HasColumnType("datetime");

            builder.Property(e => e.Revoked).HasColumnType("datetime");

            builder.HasOne(d => d.User)
                .WithMany(p => p.RefreshToken)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_RefreshToken_User");
        }
    }
}
