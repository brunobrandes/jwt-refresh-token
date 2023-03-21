using System;
using System.Text.Json.Serialization;
using Jwt.Refresh.Token.Domain.Enums;

namespace Jwt.Refresh.Token.Domain.Entities
{
	public class User
	{
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("status")]
        public UserStatus Status { get; set; }
    }
}

