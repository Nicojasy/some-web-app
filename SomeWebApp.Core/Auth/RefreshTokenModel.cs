using System;

namespace SomeWebApp.Core.Auth
{
    public class RefreshTokenModel
    {
        public string RefreshToken { get; set; }
        public DateTimeOffset CreationTimestamp { get; set; }
    }
}
