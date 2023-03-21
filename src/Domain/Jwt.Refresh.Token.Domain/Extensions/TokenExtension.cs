using System;
namespace Jwt.Refresh.Token.Domain.Extensions
{
	public static class TokenExtension
	{
		public static Entities.Token ToRevokeTokenEntity(this Entities.Token token, string ipAddress)
		{
            return new Entities.Token
            {
                Id = token.Id,
                UserId = token.UserId,
                AccessToken = token.AccessToken,
                Created = DateTimeOffset.UtcNow,
                CreatedIpAddress = token.CreatedIpAddress,
				Revoked = DateTimeOffset.UtcNow,
				RevokedIpAddress = ipAddress
            };
        }

        public static Entities.Token ToAuthorizedTokenEntity(this DataTransferObjects.Token token, string ipAddress, int expiresMilliseconds)
		{
			return new Entities.Token
			{
				Id = token.Id,
				UserId = token.UserId,
				AccessToken = token.AccessToken,
				Created = DateTimeOffset.UtcNow,
				CreatedIpAddress = ipAddress,
				Ttl = (int)TimeSpan.FromMilliseconds(expiresMilliseconds).TotalSeconds
            };  
		}
	}
}

