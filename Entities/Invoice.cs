using System.ComponentModel.DataAnnotations;

namespace FaturaApi.Entities
{
	public class Invoice
	{
        public int Id { get; set; }
        public int InvoiceId { get; set; }
		public int UserId { get; set; } 
		public DateTime InvoiceDate { get; set; }
		public DateTime DueDate { get; set; }
		public int TotalAmount { get; set; }
		public string Status { get; set; } // Ödendi, Beklemede, İptal

		// Navigation Properties
		public User User { get; set; }
        public Client Client { get; set; }

        public int ClientId { get; set; }
        public List<Payment> Payments { get; set; } = new List<Payment>();
		public List<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>(); // Many-to-Many relation
	}
}
