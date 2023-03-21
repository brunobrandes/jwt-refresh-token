using System;
using Jwt.Refresh.Token.Domain.Constants;
using Jwt.Refresh.Token.Domain.Enums;

namespace Jwt.Refresh.Token.Domain.DataTransferObjects
{
    public class Token
    {
        public string Id { get; set; }
        public string UserId { get; }    
        public string AccessToken { get; }
        public TokenStatus Status { get; }
        public DateTime? Expires { get; }
        public int ExpiresMilliseconds
        {
            get
            {
                if (Expires.HasValue && Expires != DateTimeOffset.MinValue)
                    return (int)(Expires.Value - DateTimeOffset.UtcNow).TotalMilliseconds;

                return 0;
            }
        }

        public TokenError Error { get; set; }

        public Token(string userId, TokenStatus tokenStatus, TokenError error = default)
        {
            UserId = userId;
            Status = tokenStatus;
            Error = error;
        }

        public Token(string tokenId, string userId, string accessToken, TokenStatus tokenStatus, int expiresMilliseconds)
        {
            Id = tokenId;
            UserId = userId;
            AccessToken = accessToken;
            Status = tokenStatus;
            Expires = DateTime.UtcNow.AddMilliseconds(expiresMilliseconds);
        }
    }

    public class TokenError
    {
        public string Message { get; }
        public string InnerException { get; }

        public TokenError(string errorMessage, Exception exception = default)
        {
            Message = errorMessage;
            InnerException = exception?.ToString();
        }
    }
}

