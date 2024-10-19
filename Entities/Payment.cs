namespace FaturaApi.Entities
{
	public class Payment
	{
		public int PaymentId { get; set; }
		public int InvoiceId { get; set; } // Foreign Key to Invoice
		public DateTime PaymentDate { get; set; }
		public int Amount { get; set; }
		public string Status { get; set; } // Başarılı, Başarısız

		// Navigation Property
		public Invoice Invoice { get; set; }
	}
}
