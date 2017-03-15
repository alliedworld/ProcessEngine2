using System;
using System.Runtime.Serialization;

namespace Klaudwerk.PropertySet
{
    [Serializable]
    [DataContract]
    public class GroupsSchema : StringSchema
    {
        public override string TypeName => "GroupsList";
        public override void Validate(object value)
        {
            // empty validation
        }

    }
}