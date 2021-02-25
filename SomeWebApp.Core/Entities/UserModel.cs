using System;

namespace SomeWebApp.Core.Entities
{
    public class UserModel
    {
        public UserModel() { }
        public UserModel(string nickname, string email, string password)
        {
            Nickname = nickname;
            Password = password;
            Email = email;
            Bio = "";
            Photo = default;
            IsDeleted = false;
            RegistrationTimestamp = DateTimeOffset.UtcNow;
        }
        public UserModel(string nickname, string email, string password, string bio, string photo)
        {
            Nickname = nickname;
            Password = password;
            Email = email;
            Bio = "My Bio";
            Photo = photo;
            IsDeleted = false;
            RegistrationTimestamp = DateTimeOffset.UtcNow;
        }

        public UInt64 ID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Nickname { get; set; }
        public string Bio { get; set; }
        public string Photo { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset RegistrationTimestamp { get; set; }
    }
}