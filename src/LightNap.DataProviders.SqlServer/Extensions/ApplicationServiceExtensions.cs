using LightNap.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LightNap.DataProviders.SqlServer.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring application services.
    /// </summary>
    public static class ApplicationServiceExtensions
    {
        /// <summary>
        /// Adds the LightNap SQL Server services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection to add the services to.</param>
        /// <param name="configuration">The configuration to use for setting up the services.</param>
        /// <returns>The service collection with the added services.</returns>
        /// <exception cref="ArgumentException">Thrown when the required connection string 'DefaultConnection' is missing.</exception>
        public static IServiceCollection AddLightNapSqlServer(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentException("Required connection string 'DefaultConnection' is missing"),
                    sqlOptions => sqlOptions.MigrationsAssembly("LightNap.DataProviders.SqlServer")));
        }
    }
}