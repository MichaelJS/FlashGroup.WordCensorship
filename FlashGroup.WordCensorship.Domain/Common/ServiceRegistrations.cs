using FlashGroup.WordCensorship.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlashGroup.WordCensorship.Domain.Common
{
    public static class ServiceRegistrations
    {
        /// <summary>
        /// Adds Word Censorship related services and repositories to the IServiceCollection.
        /// </summary>
        /// <param name="services">Extension to the IServiceCollection.</param>
        /// <param name="configuration">Configuration required to load depenant configurations.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddWordCensorshipServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("CensorshipDBConn");
            if (String.IsNullOrWhiteSpace(connectionString))                            
                throw new ArgumentNullException("CensorshipDBConn connection string is not configured.");

            // Register caching
            services.AddMemoryCache();

            //Services
            services.AddScoped<ISanitizeService, SanitizeService>();
            services.AddScoped<ISensitiveWordService, SensitiveWordService>();

            //Repositories
            services.AddScoped<IDBClient>(s => new DBClient(connectionString));
            services.AddScoped<ISensitiveWordRepo, SensitiveWordRepo>();
        }
    }
}
