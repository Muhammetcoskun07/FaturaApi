using FaturaApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace FaturaApi.Data
{
	public class AppDbContext:DbContext
	{
		public DbSet<Invoice> Invoices { get; set; }
		public DbSet<Item> Items { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Client> Clients { get; set; }
		public DbSet<Payment> Payments { get; set; }
		public DbSet<InvoiceItem> InvoiceItems { get; set; }

		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Invoice ile Item arasındaki Many-to-Many ilişkisini tanımlayın
			modelBuilder.Entity<InvoiceItem>()
				.HasKey(ii => new { ii.InvoiceId, ii.ItemId }); // Composite Primary Key

			modelBuilder.Entity<InvoiceItem>()
				.HasOne(ii => ii.Invoice)
				.WithMany(i => i.InvoiceItems)
				.HasForeignKey(ii => ii.InvoiceId);

			modelBuilder.Entity<InvoiceItem>()
				.HasOne(ii => ii.Item)
				.WithMany(i => i.InvoiceItems)
				.HasForeignKey(ii => ii.ItemId);
		}
	}
}
