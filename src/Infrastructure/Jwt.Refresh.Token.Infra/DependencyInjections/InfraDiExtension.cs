using System;
using Jwt.Refresh.Token.Application;
using Jwt.Refresh.Token.Application.Interfaces;
using Jwt.Refresh.Token.Domain.Configurations;
using Jwt.Refresh.Token.Domain.Services;
using Jwt.Refresh.Token.Domain.Services.Interfaces;
using Jwt.Refresh.Token.Infra.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jwt.Refresh.Token.Infra.DependencyInjections
{
    public static class InfraDiExtension
    {
        public static void BindJwtRefreshTokenExpiresOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtRefreshTokenExpiresOptions>(configuration
                .GetSection(JwtRefreshTokenExpiresOptions.JwtRefreshTokenExpires));
        }

        public static void AddJwtRefreshTokenServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddScoped<IClaimsIdentityService, ClaimsIdentityService>();

            services.Configure<JwtRefreshTokenDescriptorOptions>(configuration
                .GetSection(JwtRefreshTokenDescriptorOptions.JwtRefreshTokenDescriptor));

            services
                .AddScoped<IJwtTokenService, JwtTokenService>();

            services
                .AddScoped<ITokenAppService, TokenAppService>();
        }
    }
}

