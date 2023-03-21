using System;
namespace Jwt.Refresh.Token.Domain.Configurations
{
    public class JwtRefreshTokenExpiresOptions
    {
        public const string JwtRefreshTokenExpires = "JwtRefreshTokenExpires";

        public int CreateMilliseconds { get; set; }
        public int RefreshMilliseconds { get; set; }
    }
}

