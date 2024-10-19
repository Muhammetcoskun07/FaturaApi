using FaturaApi.Data;
using FaturaApi.Dto;
using FaturaApi.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public IActionResult GetProgramById(int id)
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
        [HttpPost]
        public IActionResult CreateInvoice([FromBody] DtoAddInvoice invoiceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Veritabanı için Program nesnesi oluşturma
            var invoice = new Invoice
            {
                InvoiceId = invoiceDto.InvoiceId,
                InvoiceDate = invoiceDto.InvoiceDate,
                TotalAmount = (int)invoiceDto.TotalAmount,
                Status = invoiceDto.Status
            };

            _context.Invoices.Add(invoice);
            _context.SaveChanges();

            // Başarıyla oluşturulan programın ID’siyle dönüyoruz.
            return CreatedAtAction(nameof(GetProgramById), new { id = invoice.Id }, invoice);
        }

    }
}
