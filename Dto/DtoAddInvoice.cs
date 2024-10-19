namespace FaturaApi.Dto
{
	public class DtoAddInvoice
	{
		public int InvoiceId { get; set; }
		public decimal TotalAmount { get; set; }
		public DateTime InvoiceDate { get; set; }
		public string Status { get; set; }
	}
}
