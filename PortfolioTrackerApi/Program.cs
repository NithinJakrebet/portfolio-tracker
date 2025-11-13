using Microsoft.EntityFrameworkCore;      // for UseSqlite + AddDbContext
using PortfolioTrackerApi.Data;           // for AppDbContext

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    const int maxRetries = 5;
    for (var attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            Console.WriteLine($"[Startup] Ensuring database is created (attempt {attempt}/{maxRetries})...");
            db.Database.EnsureCreated();

            Console.WriteLine("[Startup] Running seed...");
            await DbSeeder.SeedAsync(db);
            Console.WriteLine("[Startup] Migrations/seed completed.");
            break;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Startup] Attempt {attempt} failed: {ex.Message}");

            if (attempt == maxRetries)
            {
                Console.WriteLine("[Startup] Max retries reached. Giving up.");
                throw;
            }

            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
}


app.MapControllers();
app.UseHttpsRedirection();
await app.RunAsync();
