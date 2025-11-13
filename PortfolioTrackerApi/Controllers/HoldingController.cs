using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioTrackerApi.Models;
using PortfolioTrackerApi.Data;

namespace PortfolioTrackerApi.Controllers
{
      [ApiController]
      [Route("api/[controller]")]
      public class HoldingController(AppDbContext db) : ControllerBase
      {
            private readonly AppDbContext _db = db;

            [HttpGet]
            public async Task<ActionResult<IEnumerable<Holding>>> GetAll()
            {
                  var h = await _db.Holdings.AsNoTracking().ToListAsync();
                  return Ok(h);
            }

            [HttpGet("{id:guid}")]
            public async Task<ActionResult<Holding>> GetById(Guid id)
            {
                  var h = await _db.Holdings.FindAsync(id);
                  return h is null ? NotFound() : Ok(h);
            }

            [HttpGet("/api/portfolios/{portfolioId:guid}/holdings")]
            public async Task<ActionResult<IEnumerable<Holding>>> GetByPortfolio(Guid portfolioId)
            => Ok(await _db.Holdings.Where(h => h.PortfolioId == portfolioId)
                                    .AsNoTracking()
                                    .ToListAsync());


            [HttpPost]
            public async Task<ActionResult<Holding>> Create([FromBody] Holding request)
            {
                  var portfolioExists = await _db.Portfolios.AnyAsync(p => p.PortfolioId == request.PortfolioId);
                  if (!portfolioExists) return BadRequest("PortfolioId is invalid.");

                  request.HoldingId = Guid.NewGuid();
                  request.CreatedAt = DateTime.UtcNow;
                  request.UpdatedAt = DateTime.UtcNow;

                  request.Portfolio = null;

                  _db.Holdings.Add(request);
                  await _db.SaveChangesAsync();

                  return CreatedAtAction(nameof(GetById), new { id = request.HoldingId }, request);
            }

            [HttpPut("{id:guid}")]
            public async Task<IActionResult> Update(Guid id, [FromBody] Holding request)
            {
                  var holdingToUpdate = await _db.Holdings.FindAsync(id);
                  if (holdingToUpdate == null) return NotFound();

                  holdingToUpdate.Symbol = request.Symbol;
                  holdingToUpdate.Exchange = request.Exchange;
                  holdingToUpdate.Quantity = request.Quantity;
                  holdingToUpdate.AvgCostBasis = request.AvgCostBasis;
                  holdingToUpdate.UpdatedAt = DateTime.UtcNow;

                  await _db.SaveChangesAsync();

                  return NoContent();
            }

            [HttpDelete("{id:guid}")]
            public async Task<IActionResult> Delete(Guid id)
            {
                  var holdingToDelete = await _db.Holdings.FindAsync(id);
                  if (holdingToDelete == null) return NotFound();

                  _db.Holdings.Remove(holdingToDelete);
                  await _db.SaveChangesAsync();
                  return NoContent();
            }
      }

}