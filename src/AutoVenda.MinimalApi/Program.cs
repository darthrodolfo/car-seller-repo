using AutoVenda.Infrastructure;
using AutoVenda.Infrastructure.Persistence;
using AutoVenda.MinimalApi.Features.Catalogue;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AutoVendaDbContext>();
    await db.Database.MigrateAsync();
}

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapListingsEndpoints();

app.Run();
