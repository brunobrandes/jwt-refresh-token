using System;
using Jwt.Refresh.Token.Domain.Enums;

namespace Jwt.Refresh.Token.Domain.DataTransferObjects
{
    public class Token
    {
        public string Id { get; set; }
        public string UserId { get; }    
        public string AccessToken { get; }
        public TokenStatus Status { get; }
        public DateTimeOffset? Expires { get; }
        public int ExpiresMilliseconds
        {
            get
            {
                if (Expires.HasValue && Expires != DateTimeOffset.MinValue)
                    return (int)(Expires.Value - DateTimeOffset.UtcNow).TotalMilliseconds;

                return 0;
            }
        }

        public Token(string userId, TokenStatus tokenStatus)
        {
            UserId = userId;
            Status = tokenStatus;
        }

        public Token(string tokenId, string userId, string accessToken, TokenStatus tokenStatus, int expiresMilliseconds)
        {
            Id = tokenId;
            UserId = userId;
            AccessToken = accessToken;
            Status = tokenStatus;
            Expires = DateTimeOffset.UtcNow.AddMilliseconds(expiresMilliseconds);
        }
    }
}

