using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Letters.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Letters.Application.Interfaces;
using Letters.Application.Services;
using Letters.Domain.Interfaces;
using Letters.Infrastructure.Repositorys;

namespace Letters.IOC
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddinfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            // services.AddScoped<IYourService, YourServiceImplementation>();
            //Services
            services.AddScoped<IUserService, UserService>();

            //Repositorys
            services.AddScoped<IUserRepository, UserRepository>();



            //DbContext
            services.AddDbContext<Letters.Infrastructure.Context.ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")));
            return services;
        }

    }
}
