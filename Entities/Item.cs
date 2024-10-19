using System.ComponentModel.DataAnnotations;

namespace FaturaApi.Entities
{
	public class Item
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Amount { get; set; }
		public string PaymentMethod { get; set; } // Kredi Kartı, Banka Transferi vb.
		public int Quantity { get; set; }
		public int Price { get; set; } // Ürün fiyatı
		public int Total { get; set; } // Toplam Tutar

		// Navigation Property for the Many-to-Many relation
		public List<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
	}
}
