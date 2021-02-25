using System;

namespace SomeWebApp.Core.Entities
{
    public class PermissionModel
    {
        public PermissionModel() { }
        public PermissionModel(string name, string discription)
        {
            Name = name;
            Discription = discription;
        }

        public UInt64 ID { get; set; }
        public string Name { get; set; }
        public string Discription { get; set; }
    }
}
