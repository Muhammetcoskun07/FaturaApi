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
    public class ClientController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("{id}")]
        public IActionResult GetClientById(int id)
        {
            var client = _context.Clients
                .Include(p => p.Invoices)
                .Select(t => new { t.Id, t.Name, t.Email, t.Phone, t.City, t.PostCode, t.Country, t.StreetAddress, })
                .FirstOrDefault(p => p.Id == id);

            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }
        [HttpGet("AllList")]
        public ActionResult<List<DtoAddClient>> GetAllClient()
        {
            var clients = _context.Clients
                .Include(c => c.Invoices)
                .Select(c => new {
                    c.Id,
                    c.Name,
                    c.Email,
                    c.Phone,
                    c.City,
                    c.PostCode,
                    c.Country,
                    c.StreetAddress,
                    Invoices = c.Invoices.Select(i => new {
                        i.InvoiceId,
                        i.InvoiceDate,
                        i.DueDate,
                        i.TotalAmount,
                        i.Status
                    }).ToList()
                })
                .ToList();

            return Ok(clients);
        }
        [HttpPost]
        public IActionResult CreateClient([FromBody] DtoAddClient clientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var client = new Client
            {
                Name = clientDto.Name,
                Email = clientDto.Email,
                Phone = clientDto.Phone,
                City = clientDto.City,
                PostCode = clientDto.PostCode,
                Country = clientDto.Country,
                StreetAddress = clientDto.StreetAddress
            };

            _context.Clients.Add(client);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetClientById), new { id = client.Id }, client);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateInvoice(int id, [FromBody] DtoAddClient clientDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var client = _context.Clients.Find(id);
            if (client == null)
            {
                return NotFound($"!!! {id} not found.");
            }


            client.Name = clientDto.Name;
            client.Email = clientDto.Email;
            client.Phone = clientDto.Phone;
            client.City = clientDto.City;
            client.PostCode = clientDto.PostCode;
            client.Country = clientDto.Country;
            client.StreetAddress = clientDto.StreetAddress;

            _context.Clients.Update(client);
            _context.SaveChanges();


            return Ok(client);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteClient(int id)
        {
            var client = _context.Clients.Find(id);
            if (client == null)
            {
                return NotFound($"Fatura ID {id} bulunamadı.");
            }

            _context.Clients.Remove(client);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
