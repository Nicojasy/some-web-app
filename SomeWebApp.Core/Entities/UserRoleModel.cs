using System;

namespace SomeWebApp.Core.Entities
{
    public class UserRoleModel
    {
        public UserRoleModel() { }
        public UserRoleModel(UInt64 id_user, UInt64 id_role)
        {
            ID_User = id_user;
            ID_Role = id_role;
        }

        public UInt64 ID { get; set; }
        public UInt64 ID_User { get; set; }
        public UInt64 ID_Role { get; set; }
    }
}
