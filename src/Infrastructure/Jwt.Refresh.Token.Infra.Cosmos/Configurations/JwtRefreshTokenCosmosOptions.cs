using System;
using Jwt.Refresh.Token.Domain.Configurations;

namespace Jwt.Refresh.Token.Infra.Cosmos.Configurations
{
	public class JwtRefreshTokenCosmosOptions
    {
        public const string JwtRefreshTokenCosmos = "JwtRefreshTokenCosmos";

        public string ConnectionString { get; set; }
        public string DatabaseId { get; set; }
        public string TokenContainerId { get; set; }
    }
}

