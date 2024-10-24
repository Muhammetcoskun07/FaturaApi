using FaturaApi.Data;
using FaturaApi.Dto;
using FaturaApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net.Mail;
using System.Net;

namespace FaturaApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InvoiceController(AppDbContext context)
        {
            _context = context;
        }
        
        [HttpGet("AllList")]
        public ActionResult<List<object>> GetAllInvoices()
        {
            var invoices = _context.Invoices
                .Include(i => i.Payments)
                .Include(i => i.InvoiceItems)
                .ThenInclude(ii => ii.Item)
                .Include(i => i.User)
                .Include(i => i.Client)
                .Select(i => new
                {
                    i.InvoiceId,
                    i.InvoiceDate,
                    i.DueDate,
                    i.TotalAmount,
                    i.Status,
                    User = new
                    {
                        i.User.UserId,
                        i.User.Name,
                        i.User.Email,
                    },
                    Client = new
                    {
                        i.Client.Id,
                        i.Client.Name,
                        i.Client.Email,
                        i.Client.Phone,
                        i.Client.City,
                        i.Client.PostCode,
                        i.Client.Country,
                        i.Client.StreetAddress
                    },
                    InvoiceItems = i.InvoiceItems.Select(ii => new
                    {
                        ii.Item.Id,
                        ii.Item.Name,
                        ii.Item.Price,
                        ii.Item.Quantity,
                        ii.Item.Total,
                        ii.Item.PaymentMethod
                    }).ToList(),
                    Payments = i.Payments.Select(p => new
                    {
                        p.PaymentId,
                        p.Amount,
                        p.PaymentDate
                    }).ToList()
                })
                .ToList();

            return Ok(invoices);
        }

		[HttpPost("CreateInvoice")]
		public IActionResult CreateInvoiceWithClient([FromBody] DtoAddInvoice invoiceDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var client = _context.Clients.FirstOrDefault(c => c.Id == invoiceDto.ClientId);
			if (client == null)
			{
				return NotFound("Belirtilen müşteri bulunamadı.");
			}

			var invoice = new Invoice
			{
				UserId = invoiceDto.UserId,
				ClientId = invoiceDto.ClientId,  // Client bilgisi ekleniyor
				InvoiceDate = invoiceDto.InvoiceDate,
				TotalAmount = invoiceDto.TotalAmount,
				Status = invoiceDto.Status
			};


			_context.Invoices.Add(invoice);
			_context.SaveChanges();

        //    var invoiceDetails = $@"
        //    <h1>Fatura Detayları</h1>
        //    <p><strong>Fatura Adı:</strong> {invoice.InvoiceName}</p>
        //    <p><strong>Fatura Tarihi:</strong> {invoice.CreatedTime.ToShortDateString()}</p>
        //    <p><strong>Ödeme Durumu:</strong> {invoice.PaymentStatus}</p>
        //    <p><strong>Son Ödeme Tarihi:</strong> {invoice.PaymentDue.ToShortDateString()}</p>
        //    <p><strong>Açıklama:</strong> {invoice.Description}</p>
        //    <h2>Ürünler</h2>
        //    <ul>
        //";


        //    foreach (var item in invoice.Items)
        //    {
        //        invoiceDetails += $@"
        //        <li>
        //            {item.Name} - {item.Quantity} x {item.Price:C} = {item.Total:C}
        //        </li>";
        //    }

        //    invoiceDetails += "</ul>";


        //    var smtpClient = new SmtpClient("smtp.eu.mailgun.org", 587)
        //    {
        //        Credentials = new NetworkCredential("postmaster@bildirim.bariscakdi.com.tr",
        //            "8eac27c024c6133c1ee30867d050a18f-a26b1841-8a6f61d9"),
        //        EnableSsl = true
        //    };

        //    var mailMessage = new MailMessage()
        //    {
        //        From = new MailAddress("postmaster@bildirim.bariscakdi.com.tr", "Invoice App"),
        //        Subject = "Yeni Fatura Bilgileri",
        //        Body = invoiceDetails,
        //        IsBodyHtml = true
        //    };

        //    mailMessage.To.Add(new MailAddress(client.Email, client.Name));

        //    smtpClient.Send(mailMessage);

            return Ok(new { message = "Fatura başarıyla kaydedildi" });
        }

		


		[HttpPut("{id}")]
        public IActionResult UpdateInvoice(int id, [FromBody] DtoAddInvoice invoiceDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var invoice = _context.Invoices.Find(id);
            if (invoice == null)
            {
                return NotFound($"!!! {id} not found.");
            }


            invoice.InvoiceDate = invoiceDto.InvoiceDate;
            invoice.TotalAmount = (int)invoiceDto.TotalAmount;
            invoice.Status = invoiceDto.Status;

            _context.Invoices.Update(invoice);
            _context.SaveChanges();


            return Ok(invoice);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteInvoice(int id)
        {
            var invoice = _context.Invoices.Find(id);
            if (invoice == null)
            {
                return NotFound($"Fatura ID {id} bulunamadı.");
            }

            _context.Invoices.Remove(invoice);
            _context.SaveChanges();

            return NoContent();
        }
		[HttpGet("GetInvoice/{id}")]
		public IActionResult GetInvoiceWithClient(int id)
		{
			var invoice = _context.Invoices
				.Include(i => i.User)
				.Include(i => i.Client)  // Client bilgilerini dahil ediyoruz
				.Select(i => new
				{
					i.InvoiceId,
					i.InvoiceDate,
					i.TotalAmount,
					i.Status,
					User = new
					{
						i.User.UserId,
						i.User.Name,
						i.User.Email
					},
					Client = new
					{
						i.Client.Id,
						i.Client.Name,
						i.Client.Email,
						i.Client.Phone,
						i.Client.City,
						i.Client.PostCode,
						i.Client.Country,
						i.Client.StreetAddress
					}
				})
				.FirstOrDefault(i => i.InvoiceId == id);

			if (invoice == null)
			{
				return NotFound($"Fatura ID {id} bulunamadı.");
			}

			return Ok(invoice);
		}


		[HttpGet("Search")]
        public ActionResult<List<object>> SearchInvoices(string searchTerm)
        {
            // Eğer arama terimi boşsa tüm faturaları döndür
            if (string.IsNullOrEmpty(searchTerm))
            {
                return Ok(_context.Invoices
                    .Include(i => i.User)
                    .Include(i => i.Client)
                    .Select(i => new
                    {
                        i.InvoiceId,
                        i.InvoiceDate,
                        i.TotalAmount,
                        i.Status,
                        User = new
                        {
                            i.User.UserId,
                            i.User.Name,
                            i.User.Email,
                        },
                        Client = new
                        {
                            i.Client.Id,
                            i.Client.Name,
                            i.Client.Email,
                            i.Client.Phone,
                            i.Client.City,
                            i.Client.PostCode,
                            i.Client.Country,
                            i.Client.StreetAddress
                        }
                    }).ToList());
            }

            var invoices = _context.Invoices
                .Include(i => i.User)
                .Include(i => i.Client)
                .Where(i => i.Client.Name.Contains(searchTerm) || i.User.Name.Contains(searchTerm))
                .Select(i => new
                {
                    i.InvoiceId,
                    i.InvoiceDate,
                    i.TotalAmount,
                    i.Status,
                    User = new
                    {
                        i.User.UserId,
                        i.User.Name,
                        i.User.Email,
                    },
                    Client = new
                    {
                        i.Client.Id,
                        i.Client.Name,
                        i.Client.Email,
                        i.Client.Phone,
                        i.Client.City,
                        i.Client.PostCode,
                        i.Client.Country,
                        i.Client.StreetAddress
                    }
                })
                .ToList();

            return Ok(invoices);
        }
        
        
    }
}
