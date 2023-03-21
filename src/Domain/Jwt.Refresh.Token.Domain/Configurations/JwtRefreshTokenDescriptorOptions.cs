using System;
namespace Jwt.Refresh.Token.Domain.Configurations
{
	public class JwtRefreshTokenDescriptorOptions
    {
        public const string JwtRefreshTokenDescriptor = "JwtRefreshTokenDescriptor";

        public string AlgorithmKey { get; set; }        
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}

