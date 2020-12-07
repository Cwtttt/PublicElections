using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PublicElections.Domain.Entities;
using PublicElections.Infrastructure.Options;

namespace PublicElections.Infrastructure.EntityFramework
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSqlDb(this IServiceCollection services, SqlSettings sqlSettings)
        {
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(sqlSettings.ConnectionString), ServiceLifetime.Transient);

            services.AddDefaultIdentity<ApplicationUser>()
                .AddEntityFrameworkStores<DataContext>();
        }
    }
}
