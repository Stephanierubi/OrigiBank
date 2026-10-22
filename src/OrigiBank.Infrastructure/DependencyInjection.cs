using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrigiBank.Application.Abstractions;
using OrigiBank.Infrastructure.Persistence;
using OrigiBank.Infrastructure.Repositories;
namespace OrigiBank.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default") ?? "Data Source=origibank.db";
        services.AddDbContext<OrigiBankDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}