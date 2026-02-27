using AutoVenda.Domain.Catalogue;
using AutoVenda.Infrastructure.Catalogue;
using AutoVenda.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AutoVenda.Infrastructure;

/// <summary>
/// Registro de serviços da camada de infraestrutura (DbContext, repositórios).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=autovenda.db";

        services.AddDbContext<AutoVendaDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IListingRepository, ListingRepository>();

        return services;
    }
}
