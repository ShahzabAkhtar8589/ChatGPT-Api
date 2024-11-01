using ChatGPT_Api.DBContext;
using ChatGPT_Api.Interface;
using ChatGPT_Api.Middleware;
using ChatGPT_Api.Model;
using ChatGPT_Api.Services;
using Google.Api;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IChatGPT, ChatGPT_Api.Services.ChatGPT>();
builder.Services.AddDbContext<LoggingContext>();
builder.Services.AddHealthChecks()
               .AddSqlServer(
                   builder.Configuration.GetConnectionString("DefaultConnection"),
                   name: "sql",
                   failureStatus: HealthStatus.Unhealthy);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<AuthenticationApi>();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
