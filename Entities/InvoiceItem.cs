namespace FaturaApi.Entities
{
	public class InvoiceItem
	{
		public int InvoiceId { get; set; } 
		public int ItemId { get; set; }  

		
		public Invoice Invoice { get; set; }
		public Item Item { get; set; }
	}
}
