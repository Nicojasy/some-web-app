using System;

namespace SomeWebApp.Core.Entities
{
    public class RefreshSessionModel
    {
        public RefreshSessionModel() { }
        public RefreshSessionModel(UInt64 id_user, string refreshToken)
        {
            ID_User = id_user;
            RefreshToken = refreshToken;
            CreationTimestamp = DateTimeOffset.UtcNow;
        }
        public RefreshSessionModel(UInt64 id_user, string refreshToken, DateTimeOffset creationTimestamp)
        {
            ID_User = id_user;
            RefreshToken = refreshToken;
            CreationTimestamp = creationTimestamp;
        }

        public UInt64 ID { get; set; }
        public UInt64 ID_User { get; set; }
        public string RefreshToken { get; set; }
        public DateTimeOffset CreationTimestamp { get; set; }
    }
}
