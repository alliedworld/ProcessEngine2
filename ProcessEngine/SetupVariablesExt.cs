using Klaudwerk.PropertySet;
using KlaudWerk.ProcessEngine.Builder;
using KlaudWerk.ProcessEngine.Definition;

namespace KlaudWerk.ProcessEngine
{
    /// <summary>
    /// Extension methods for Variables
    /// </summary>
    public static class SetupVariablesExt
    {
        /// <summary>
        /// Setup Properties with Associated Schemas in Property Set Collection
        /// </summary>
        /// <param name="collection"><see cref="IPropertySetCollection"/></param>
        /// <param name="vd"><see cref="VariableDefinition"/></param>
        public static void SetupVariable(this VariableDefinition vd,IPropertySetCollection collection)
        {
            switch (vd.VariableType)
            {
                case VariableTypeEnum.Char:
                    var charSchema = collection.Schemas.SchemaFactory.Create(typeof(char));
                    collection.Add(vd.Name, charSchema);
                    break;
                case VariableTypeEnum.Decimal:
                    var decimalSchema = collection.Schemas.SchemaFactory.Create(typeof(decimal));
                    collection.Add(vd.Name, decimalSchema);
                    break;
                case VariableTypeEnum.Int:
                    var intSchema = collection.Schemas.SchemaFactory.Create(typeof(int));
                    collection.Add(vd.Name, intSchema);
                    break;
                case VariableTypeEnum.String:
                    var stringSchema = collection.Schemas.SchemaFactory.Create(typeof(string));
                    collection.Add(vd.Name, stringSchema);
                    break;
                case VariableTypeEnum.Object:
                    var objectSchema = collection.Schemas.SchemaFactory.Create(typeof(object));
                    collection.Add(vd.Name, objectSchema);
                    break;
                case VariableTypeEnum.Json:
                    var jsonSchema = collection.Schemas.SchemaFactory.Create(typeof(string));
                    collection.Add(vd.Name, jsonSchema);
                    break;
                case VariableTypeEnum.Boolean:
                    var boolSchema = collection.Schemas.SchemaFactory.Create(typeof(bool));
                    collection.Add(vd.Name, boolSchema);
                    break;
                case VariableTypeEnum.None:
                    break;
            }
        }

    }
}