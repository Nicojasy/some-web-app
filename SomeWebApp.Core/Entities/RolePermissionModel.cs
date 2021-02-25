using System;

namespace SomeWebApp.Core.Entities
{
    public class RolePermissionModel
    {
        public RolePermissionModel() { }
        public RolePermissionModel(UInt64 id_role, UInt64 id_permission)
        {
            ID_Role = id_role;
            ID_Permission = id_permission;
        }

        public UInt64 ID { get; set; }
        public UInt64 ID_Role { get; set; }
        public UInt64 ID_Permission { get; set; }
    }
}
