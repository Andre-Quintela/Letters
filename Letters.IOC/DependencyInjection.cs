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
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEssayService, EssayService>();

            //Repositorys
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IEssayRepository, EssayRepository>();



            //DbContext
            services.AddDbContext<Letters.Infrastructure.Context.ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure
                    (
                        maxRetryCount: 5,           // Máximo de 5 tentativas
                        maxRetryDelay: TimeSpan.FromSeconds(30),    // Delay máximo de 30 segundos
                        errorNumbersToAdd: null     // Erros padrão do SQL Server
                    )
                    ));
            return services;
        }
    }
}
