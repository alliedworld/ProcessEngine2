using System;
using System.Runtime.Serialization;

namespace Klaudwerk.PropertySet
{
    [Serializable]
    [DataContract]
    public class UsersSchema : StringSchema
    {
        public override string TypeName => "UsersList";
        public override void Validate(object value)
        {
            // empty validation
        }

    }
}