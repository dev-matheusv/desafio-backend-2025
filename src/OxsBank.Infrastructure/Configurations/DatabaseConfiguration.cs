using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OxsBank.Infrastructure.Persistence;

namespace OxsBank.Infrastructure.Configurations
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, 
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<OxsBankDbContext>(options =>
                options.UseNpgsql(connectionString));

            return services;
        }
    }
}