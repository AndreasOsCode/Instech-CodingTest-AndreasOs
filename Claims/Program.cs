using System.Text.Json.Serialization;
using Claims.Auditing;
using Claims.Contexts;
using Claims.Contexts.Interfaces;
using Claims.Models;
using Claims.Services;
using Claims.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddDbContext<AuditContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var mongoClient = new MongoClient(builder.Configuration.GetConnectionString("MongoDb"));
builder.Services.AddDbContext<ClaimsDbContext>(
    options =>
    {
        var database = mongoClient.GetDatabase(builder.Configuration["MongoDb:DatabaseName"]);
        options.UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName);
    }
);
builder.Services.AddDbContext<CoversDbContext>(
    options =>
    {
        var database = mongoClient.GetDatabase(builder.Configuration["MongoDb:DatabaseName"]);
        options.UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName);
    }
);
builder.Services.AddScoped<IGenericDbContext<Claim>, ClaimsDbContext>();
builder.Services.AddScoped<IGenericDbContext<Cover>, CoversDbContext>();

builder.Services.AddScoped<IGenericDbService<Claim>, ClaimsDbService>();
builder.Services.AddScoped<IGenericDbService<Cover>, CoversDbService>();
builder.Services.AddScoped<IPremiumComputer<CoverType>, PremiumComputer>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
    context.Database.Migrate();
}

app.Run();

public partial class Program { }
