using System;

namespace SomeWebApp.Core.Auth
{
    public class TokensModel
    {
        public TokensModel() { }
        public TokensModel(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            CreationTimestamp = DateTimeOffset.UtcNow;
        }

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTimeOffset CreationTimestamp { get; set; }
    }
}
