using DDB.HealthCycle.Data;
using DDB.HealthCycle.Data.TestData;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PlayerCharacterContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("PlayerCharacterContext")));

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using var scope = app.Services.CreateScope();
var pcContext = scope.ServiceProvider
    .GetRequiredService<PlayerCharacterContext>();
pcContext.Database.EnsureCreated();
if (app.Environment.IsDevelopment())
{
    // Seed test data
    await PlayerCharacterContextInitializer.Initialize(pcContext);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
