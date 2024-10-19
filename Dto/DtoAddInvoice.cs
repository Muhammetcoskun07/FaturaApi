namespace FaturaApi.Dto
{
	public class DtoAddInvoice
	{
		public int InvoiceId { get; set; }
		public int UserId { get; set; }
		public int TotalAmount { get; set; }
		public DateTime InvoiceDate { get; set; }

		public string Status { get; set; }
	}
}
