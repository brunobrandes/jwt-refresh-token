using System;
using System.Text.Json.Serialization;

namespace Jwt.Refresh.Token.Domain.Entities
{
    public class Token
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        [JsonPropertyName("created")]
        public DateTimeOffset Created { get; set; }

        [JsonPropertyName("createdIpAddress")]
        public string CreatedIpAddress { get; set; }

        [JsonPropertyName("revoked")]
        public DateTimeOffset? Revoked { get; set; }

        [JsonPropertyName("revokedIpAddress")]
        public string RevokedIpAddress { get; set; }

        [JsonPropertyName("ttl")]
        public int Ttl { get; set; }
    }
}

