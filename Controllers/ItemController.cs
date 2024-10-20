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
	public class ItemController : ControllerBase
	{
		private readonly AppDbContext _context;

		public ItemController(AppDbContext context)
		{
			_context = context;
		}
		[HttpGet("{id}")]
		public IActionResult GetItemById(int id)
		{
			var item = _context.Items
				.Include(p => p.InvoiceItems)
				.Select(t => new { t.Id, t.Name, t.Amount, t.PaymentMethod, t.Quantity, t.Price, t.Total })
				.FirstOrDefault(p => p.Id == id);

			if (item == null)
			{
				return NotFound();
			}

			return Ok(item);
		}
		[HttpGet("AllList")]
		public ActionResult<List<DtoAddItem>> GetAllItem()
		{
			var items = _context.Items
				.Include(i => i.InvoiceItems)
				.Select(i => new DtoAddItem
				{
					Id = i.Id,
					Name = i.Name,
					Amount = i.Amount,
					PaymentMethod = i.PaymentMethod,
					Quantity = i.Quantity,
					Price = i.Price,
					Total = i.Total
				})
				.ToList();

			return Ok(items);
		}
		[HttpPost("{id}")]
		public IActionResult CreateItem([FromBody] DtoAddItem itemDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var item = new Item
			{
				Name = itemDto.Name,
				Amount = itemDto.Amount,
				PaymentMethod = itemDto.PaymentMethod,
				Quantity = (int)itemDto.Quantity,
				Price = itemDto.Price,
				Total = itemDto.Total
			};

			_context.Items.Add(item);
			_context.SaveChanges();

			return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, item);
		}
		[HttpPut("{id}")]
		public IActionResult UpdateItem(int id, [FromBody] DtoAddItem itemDto)
		{

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}


			var item = _context.Items.Find(id);
			if (item == null)
			{
				return NotFound($"!!! {id} not found.");
			}


			item.Name = itemDto.Name;
			item.Amount = itemDto.Amount;
			item.PaymentMethod = itemDto.PaymentMethod;
			item.Quantity = itemDto.Quantity;
			item.Price = itemDto.Price;
			item.Total = itemDto.Total;

			_context.Items.Update(item);
			_context.SaveChanges();


			return Ok(item);
		}
		[HttpDelete("{id}")]
		public IActionResult DeleteItem(int id)
		{
			var item = _context.Items.Find(id);
			if (item == null)
			{
				return NotFound($"Fatura ID {id} bulunamadı.");
			}

			_context.Items.Remove(item);
			_context.SaveChanges();

			return NoContent();
		}
	}
}
