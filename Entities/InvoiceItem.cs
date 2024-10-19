namespace FaturaApi.Entities
{
	public class InvoiceItem
	{
		public int InvoiceId { get; set; } // Foreign Key to Invoice
		public int ItemId { get; set; }     // Foreign Key to Item

		// Navigation Properties
		public Invoice Invoice { get; set; }
		public Item Item { get; set; }
	}
}
