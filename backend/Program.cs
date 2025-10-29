using backend.Data;
using Microsoft.EntityFrameworkCore;
using backend.Services;

var builder = WebApplication.CreateBuilder(args);

// ✅ Enable Controllers
builder.Services.AddControllers();

// ✅ Enable Swagger (OpenAPI)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Connect SQLite database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ AuthService
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

// ✅ Enable Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Optional: allow frontend to call this API later
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

// ✅ HTTPS redirection is fine
app.UseHttpsRedirection();

// ✅ Enable Authorization (even if unused for now)
app.UseAuthorization();

// ✅ Map controller routes (super important)
app.MapControllers();

app.Run();
