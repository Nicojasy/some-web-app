using SomeWebApp.Core.Entities;
using System;
using System.Collections.Generic;

namespace SomeWebApp.Core.Repository
{
    public class RoleWithPermissionsModel
    {
        public RoleWithPermissionsModel() { }

        public UInt64 ID { get; set; }
        public string Name { get; set; }
        public string Discription { get; set; }
        public IEnumerable<PermissionModel> permissions { get; set; }
    }
}
