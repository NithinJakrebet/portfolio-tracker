using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioTrackerApi.Models;
using PortfolioTrackerApi.Data;

namespace PortfolioTrackerApi.Controllers
{
      [ApiController]
      [Route("api/[controller]")]
      public class TransactionController(AppDbContext db) : ControllerBase
      {
            private readonly AppDbContext _db = db;

            [HttpGet]
            public async Task<ActionResult<IEnumerable<Transaction>>> GetAll()
            {
                  var t = await _db.Transactions.AsNoTracking().ToListAsync();
                  return Ok(t);
            }

            [HttpGet("{id:guid}")]
            public async Task<ActionResult<Transaction>> GetById(Guid id)
            {
                  var t = await _db.Transactions.FindAsync(id);
                  return t is null ? NotFound() : Ok(t);
            }


            [HttpGet("/api/users/{userId:guid}/transactions")]
            public async Task<ActionResult<IEnumerable<Transaction>>> GetByUser(Guid UserId)
            => Ok(await _db.Transactions
                              .Where(t => t.UserId == UserId)
                              .AsNoTracking()
                              .ToListAsync()
                  );

            [HttpGet("/api/portfolios/{portfolioId:guid}/transactions")]
            public async Task<ActionResult<IEnumerable<Transaction>>> GetByPortfolio(Guid PortfolioId)
            => Ok(
                        await _db.Transactions
                              .Where(t => t.PortfolioId == PortfolioId)
                              .AsNoTracking()
                              .ToListAsync()
                  );

            [HttpPost]
            public async Task<ActionResult<Transaction>> Create([FromBody] Transaction request)
            {
                  var portfolioExists = await _db.Portfolios.AnyAsync(p => p.PortfolioId == request.PortfolioId);
                  if (!portfolioExists) return BadRequest("PortfolioId is invalid.");
                  var userExists = await _db.Users.AnyAsync(u => u.UserId == request.UserId);
                  if (!userExists) return BadRequest("UserId is invalid.");

                  request.TransactionId = Guid.NewGuid();
                  request.CreatedAt = DateTime.UtcNow;
                  request.UpdatedAt = DateTime.UtcNow;

                  request.Portfolio = null;
                  request.User = null;
                  _db.Transactions.Add(request);
                  await _db.SaveChangesAsync();

                  return CreatedAtAction(nameof(GetById), new { id = request.TransactionId }, request);
            }

            [HttpPut("{id:guid}")]
            public async Task<IActionResult> Update(Guid id, [FromBody] Transaction request)
            {
                  var t = await _db.Transactions.FindAsync(id);
                  if (t == null) return NotFound();

                  t.Type = request.Type;
                  t.Symbol = request.Symbol;
                  t.Quantity = request.Quantity;
                  t.Price = request.Price;
                  t.GrossAmount = request.GrossAmount;
                  t.Fee = request.Fee;
                  t.Currency = request.Currency;
                  t.ExternalRef = request.ExternalRef;
                  t.Notes = request.Notes;

                  t.UpdatedAt = DateTime.UtcNow;

                  await _db.SaveChangesAsync();
                  return NoContent();
            }

            [HttpDelete("{id:guid}")]
            public async Task<IActionResult> Delete(Guid id)
            {
                  var transactionToDelete = await _db.Transactions.FindAsync(id);
                  if (transactionToDelete == null) return NotFound();

                  _db.Transactions.Remove(transactionToDelete);
                  await _db.SaveChangesAsync();
                  return NoContent();
            }
      }
}