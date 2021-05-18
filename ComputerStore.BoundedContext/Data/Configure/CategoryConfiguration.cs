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
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        //Configure property and relation 
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(e => e.CreatedDate).HasColumnType("datetime");

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Status).HasDefaultValueSql("((0))");

            builder.Property(e => e.UpdatedDate).HasColumnType("datetime");
        }
    }
}
