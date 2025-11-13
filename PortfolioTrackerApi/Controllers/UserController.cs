
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioTrackerApi.Data;
using PortfolioTrackerApi.Models;
using BCrypt.Net; 
using PortfolioTrackerApi.Dtos;


namespace PortfolioTrackerApi.Controllers
{
      [ApiController]
      [Route("api/[controller]")]
      public class UserController(AppDbContext db) : ControllerBase
      {
            private readonly AppDbContext _db = db;

            [HttpGet]
            public async Task<ActionResult<IEnumerable<User>>> GetAll()
            {
                  var users = await _db.Users.AsNoTracking().ToListAsync();
                  return Ok(users);
            }

            [HttpGet("{id:guid}")]
            public async Task<ActionResult<User>> GetById(Guid id)
            {
                  var u = await _db.Users.FindAsync(id);
                  return u is null ? NotFound() : Ok(u);
            }

            [HttpPost]
            public async Task<ActionResult<User>> Create([FromBody] UserCreateDto request)
            {
                  var exists = await _db.Users.AnyAsync(u => u.Email == request.Email);
                  if (exists) return Conflict("A user with that email already exists.");

                  var user = new User
                  {
                        UserId = Guid.NewGuid(),
                        Email = request.Email,
                        FullName = request.FullName,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                  };

                  _db.Users.Add(user);
                  await _db.SaveChangesAsync();

                  return CreatedAtAction(nameof(GetById), new { id = user.UserId }, user);
            }


            [HttpPut("{id:guid}")]
            public async Task<IActionResult> Update(Guid id, [FromBody] User request)
            {
                  var u = await _db.Users.FindAsync(id);
                  if (u == null) return NotFound();

                  if (request.Email != u.Email) 
                  {
                        bool emailExists = await _db.Users
                              .AnyAsync(x => x.Email == request.Email && x.UserId != id);

                        if (emailExists) return Conflict("Another user already uses this email.");
                  }

                  u.FullName = request.FullName;
                  u.UpdatedAt = DateTime.UtcNow;

                  await _db.SaveChangesAsync();
                  return NoContent();
            }

            [HttpDelete("{id:guid}")]
            public async Task<IActionResult> Delete(Guid id)
            {
                  var userToDelete = await _db.Users.FindAsync(id);
                  if (userToDelete == null) return NotFound();

                  _db.Users.Remove(userToDelete);
                  await _db.SaveChangesAsync();
                  return NoContent(); 
            }
      }
}