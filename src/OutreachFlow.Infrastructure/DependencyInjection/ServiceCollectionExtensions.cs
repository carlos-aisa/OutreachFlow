using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OutreachFlow.Infrastructure.Persistence;

namespace OutreachFlow.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("OutreachFlow");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'OutreachFlow' is not configured.");
        }

        services.AddDbContext<OutreachFlowDbContext>(options =>
            options.UseSqlite(connectionString));

        return services;
    }
}
