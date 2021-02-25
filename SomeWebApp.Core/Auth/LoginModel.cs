using System;

namespace SomeWebApp.Core.Auth
{
    public class LoginModel
    {
        public string Nickname { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
