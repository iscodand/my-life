using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MyLifeApp.Infrastructure.Identity
{
    public static class ServicesExtensions
    {
        // TO-DO: Add services extensions for Identity.Infra
        public static void AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddTransient<IUserService, UserService>();
            //services.AddTransient<ITokenRepository, TokenService>();

            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"])),
                    ValidateIssuerSigningKey = true
                };
            });
        }
    }
}
