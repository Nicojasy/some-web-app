using System;

namespace SomeWebApp.Core.Entities
{
    public class RoleModel
    {
        public RoleModel() { }
        public RoleModel(string name, string discription)
        {
            Name = name;
            Discription = discription;
        }

        public UInt64 ID { get; set; }
        public string Name { get; set; }
        public string Discription { get; set; }
    }
}
