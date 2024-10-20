using FaturaApi.Data;
using FaturaApi.Dto;
using FaturaApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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
        [HttpGet("{id}")]
        public IActionResult GetInvoiceById(int id)
        {
            var invoice = _context.Invoices
                .Include(p => p.User)
                .Select(t => new { t.InvoiceId, t.InvoiceDate, t.TotalAmount, t.Status, })
                .FirstOrDefault(p => p.InvoiceId == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return Ok(invoice);
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

        [HttpPost]
        public IActionResult CreateInvoice([FromBody] DtoAddInvoice invoiceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var invoice = new Invoice
            {
                InvoiceId = invoiceDto.InvoiceId,
                UserId = invoiceDto.UserId,
                InvoiceDate = invoiceDto.InvoiceDate,
                TotalAmount = (int)invoiceDto.TotalAmount,
                Status = invoiceDto.Status
            };

            _context.Invoices.Add(invoice);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetInvoiceById), new { id = invoice.Id }, invoice);
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


            invoice.InvoiceId = invoiceDto.InvoiceId;
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
        

    }
}
