﻿namespace FaturaApi.Dto
{
	public class DtoAddInvoice
	{
        public int Id { get; set; }
        public int UserId { get; set; }
		public int ClientId { get; set; }

		public string UserName { get; set; }
		public int TotalAmount { get; set; }
		public DateTime InvoiceDate { get; set; }

		public string Status { get; set; }
	}
}
