using Microsoft.EntityFrameworkCore;
using urlshortener.Handlers;
using urlshortener.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApiDbContext>(options => options.UseNpgsql(connString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

UrlEndpoints.Map(app);

app.Run();

class ApiDbContext : DbContext
{
    public virtual DbSet<Urls> Urls { get; set; }

    public ApiDbContext(DbContextOptions<ApiDbContext> options): base(options)
    {
    }
}