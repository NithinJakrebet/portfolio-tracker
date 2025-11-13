using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioTrackerApi.Models;
using PortfolioTrackerApi.Data;

namespace PortfolioTrackerApi.Controllers
{
      [ApiController]
      [Route("api/[controller]")]
      public class PortfolioController(AppDbContext db) : ControllerBase
      {
            private readonly AppDbContext _db = db;

            [HttpGet]
            public async Task<ActionResult<IEnumerable<Portfolio>>> GetAll()
            {
                  var items = await _db.Portfolios.AsNoTracking().ToListAsync();
                  return Ok(items);
            }

            [HttpGet("{id:guid}")]
            public async Task<ActionResult<Portfolio>> GetById(Guid id)
            {
                  var p = await _db.Portfolios.FindAsync(id);
                  return p is null ? NotFound() : Ok(p);
            }

            [HttpGet("/api/users/{userId:guid}/portfolios")]
            public async Task<ActionResult<IEnumerable<Portfolio>>> GetByUser(Guid userId)
            => Ok(await _db.Portfolios.Where(p => p.UserId == userId)
                                    .AsNoTracking()
                                    .ToListAsync()
                  );


            // GET /api/portfolios/{id}/snapshot → holdings + cash + total value
            // work in progress
            // [HttpGet("{id:guid}/snapshot")]
            // public async Task<ActionResult<>> GetSnapshotByPortfolio(Guid id)
            // {
            //       var p = await _db.Portfolios.FindAsync(id);
            //       if (p == null) return BadRequest("PortfolioId is invalid.");

            //       var h = p.Holdings;
            //       var total = p.CashBalance +
            //       return
            //       {
            //       }
            // }
            
            // GET /api/portfolios/{id}/pnl → compute P&L from transactions
            // Still need to implement     

            [HttpPost]
            public async Task<ActionResult<Portfolio>> Create([FromBody] Portfolio request)
            {
                  var userExists = await _db.Users.AnyAsync(u => u.UserId == request.UserId);
                  if (!userExists) return BadRequest("UserId is invalid.");

                  request.PortfolioId = Guid.NewGuid();
                  request.CreatedAt = DateTime.UtcNow;
                  request.UpdatedAt = DateTime.UtcNow;

                  request.User = null!;

                  _db.Portfolios.Add(request);
                  await _db.SaveChangesAsync();

                  return CreatedAtAction(nameof(GetById), new { id = request.PortfolioId }, request);
            }


            [HttpPut("{id:guid}")]
            public async Task<IActionResult> Update(Guid id, [FromBody] Portfolio request)
            {
                  var portfolioToUpdate = await _db.Portfolios.FindAsync(id);                  
                  if (portfolioToUpdate == null) return NotFound(); 

                  portfolioToUpdate.OwnerName = request.OwnerName;
                  portfolioToUpdate.Label = request.Label;
                  portfolioToUpdate.CashBalance = request.CashBalance;
                  portfolioToUpdate.UpdatedAt = DateTime.UtcNow;

                  await _db.SaveChangesAsync();

                  return NoContent(); 
            }     

            [HttpDelete("{id:guid}")]
            public async Task<IActionResult> Delete(Guid id)
            {
                  var portfolioToDelete = await _db.Portfolios.FindAsync(id);
                  if (portfolioToDelete == null) return NotFound();

                  _db.Portfolios.Remove(portfolioToDelete);
                  await _db.SaveChangesAsync();
                  return NoContent(); 
            }
      }
}
