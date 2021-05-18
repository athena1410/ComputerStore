//-----------------------------------------------------------------------
// <copyright file="ComputerStoreContext.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.BoundedContext.Data.DataSeed;
using ComputerStore.BoundedContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace ComputerStore.BoundedContext.Data
{
    /// <summary>
    /// The ComputerStore DB (entity framework's) context
    /// </summary>
    public class ComputerStoreContext : DbContext, IDbContext
	{
		private readonly IDataSeeder dataSeeder;

		/// <summary>
		/// Initializes a new instance of the <see cref="ComputerStoreContext"/> class.
		/// </summary>
		/// <param name="options">The options.</param>
		/// <param name="dataSeeder"></param>
		public ComputerStoreContext(DbContextOptions<ComputerStoreContext> options, IDataSeeder dataSeeder) :
			base(options)
		{
			this.dataSeeder = dataSeeder;
		}

		public virtual DbSet<Category> Category { get; set; }
		public virtual DbSet<Company> Company { get; set; }
		public virtual DbSet<Order> Order { get; set; }
		public virtual DbSet<OrderDetail> OrderDetail { get; set; }
		public virtual DbSet<Product> Product { get; set; }
		public virtual DbSet<ProductImage> ProductImage { get; set; }
		public virtual DbSet<Role> Role { get; set; }
		public virtual DbSet<User> User { get; set; }
		public virtual DbSet<Website> Website { get; set; }
		public virtual DbSet<Cart> Cart { get; set; }
		public virtual DbSet<AnonymousCart> AnonymousCart { get; set; }

		/// <summary>
		/// Relation between tables.
		/// </summary>
		/// <param name="modelBuilder">Entity framework model builder before creating database</param>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
			dataSeeder.SeedData(modelBuilder);
		}

		public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseLoggerFactory(MyLoggerFactory);
			base.OnConfiguring(optionsBuilder);
		}
	}
}
