using System;
using System.Configuration;
using Jwt.Refresh.Token.Domain.Configurations;
using Jwt.Refresh.Token.Domain.Entities.Repositories;
using Jwt.Refresh.Token.Domain.Services.Interfaces;
using Jwt.Refresh.Token.Infra.Cosmos.Configurations;
using Jwt.Refresh.Token.Infra.Cosmos.Entities.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jwt.Refresh.Token.Infra.Cosmos.DependencyInjections
{
	public static class InfraCosmosDiExtension
	{
        public static void AddJwtRefreshTokenCosmosServices(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtRefreshTokenCosmosOptions = new JwtRefreshTokenCosmosOptions();

            configuration.GetSection(JwtRefreshTokenCosmosOptions.JwtRefreshTokenCosmos)
                .Bind(jwtRefreshTokenCosmosOptions);

            services
                .AddScoped<ITokenRepository>(x => new TokenRepository(new CosmosClient(jwtRefreshTokenCosmosOptions.ConnectionString,
                new CosmosClientOptions
                {
                    ConnectionMode = ConnectionMode.Gateway,
                    AllowBulkExecution = true,
                    SerializerOptions = new CosmosSerializationOptions
                    {
                        Indented = true,
                        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                        IgnoreNullValues = true
                    }
                }),
                jwtRefreshTokenCosmosOptions.DatabaseId,
                jwtRefreshTokenCosmosOptions.TokenContainerId));
        }
    }
}