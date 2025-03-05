using System.Text.Json.Serialization;

namespace WikiGuesser.Server.DTOs
{
    public class RegisterDTO
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
