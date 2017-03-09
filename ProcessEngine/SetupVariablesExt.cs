/**
The MIT License (MIT)

Copyright (c) 2016 Igor Polouektov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
  */
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
                    var doubleSchema = collection.Schemas.SchemaFactory.Create(typeof(double));
                    collection.Add(vd.Name, doubleSchema);
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