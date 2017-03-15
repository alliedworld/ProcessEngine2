using System;
using System.Runtime.Serialization;

namespace Klaudwerk.PropertySet
{
    [Serializable]
    [DataContract]
    public class RolesSchema : StringSchema
    {
        public override string TypeName => "RolesList";
        public override void Validate(object value)
        {
            // empty validation
        }
    }
}