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

using System;
using Klaudwerk.PropertySet.Serialization;

namespace Klaudwerk.PropertySet.Persistence
{
    public class PersistentPropertyElement
    {
        public int Id { get; set; }

        /// <summary>
        /// Element name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the string value.
        /// </summary>
        /// <value>The string value.</value>
        public virtual string StringValue { get; set; }

        /// <summary>
        /// Gets or sets the int value.
        /// </summary>
        /// <value>The int value.</value>
        public virtual int? IntValue { get; set; }

        /// <summary>
        /// Gets or sets the long value.
        /// </summary>
        /// <value>The long value.</value>
        public virtual long? LongValue { get; set; }

        /// <summary>
        /// Gets or sets the double value.
        /// </summary>
        /// <value>The double value.</value>
        public virtual double? DoubleValue { get; set; }

        /// <summary>
        /// Gets or sets the date time value.
        /// </summary>
        /// <value>The date time value.</value>
        public virtual DateTime? DateTimeValue { get; set; }

        /// <summary>
        /// Gets or sets the raw value.
        /// </summary>
        /// <value>The raw value.</value>
        public virtual byte[] RawValue { get; set; }

        public void CopyFrom(PersistentPropertyElement sourceElement)
        {
            StringValue = sourceElement.StringValue;
            IntValue = sourceElement.IntValue;
            LongValue = sourceElement.LongValue;
            DoubleValue = sourceElement.DoubleValue;
            DateTimeValue = sourceElement.DateTimeValue;
            RawValue = sourceElement.RawValue;
        }
    }

    /// <summary>
    /// Persistent Property Element
    /// </summary>
    public class ValueSerializationTarget:PropertyElement,IValueSerializationTarget
    {
        private readonly PersistentPropertyElement _element;

        public ValueSerializationTarget(PersistentPropertyElement element)
        {
            _element = element;
        }

        public override object Value
        {
            get
            {
                switch (SerializationHint)
                {
                    case SerializationTypeHint.Bool:
                        return _element.IntValue.HasValue && _element.IntValue != 0;
                    case SerializationTypeHint.Int:
                        return _element.IntValue;
                    case SerializationTypeHint.Long:
                        return _element.LongValue;
                    case SerializationTypeHint.Double:
                        return _element.DoubleValue;
                    case SerializationTypeHint.DateTime:
                        return _element.DateTimeValue;
                    case SerializationTypeHint.String:
                        return _element.StringValue;
                    case SerializationTypeHint.ByteArray:
                        return _element.RawValue;
                    case SerializationTypeHint.Null:
                        return null;
                    default:
                        return null;
                }
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Set(int? value)
        {
            SerializationHint = SerializationTypeHint.Int;
            _element.IntValue = value;
        }

        public void Set(long? value)
        {
            SerializationHint = SerializationTypeHint.Long;
            _element.LongValue = value;
        }

        public void Set(double? value)
        {
            SerializationHint = SerializationTypeHint.Double;
            _element.DoubleValue = value;
        }

        public void Set(decimal? value)
        {
            SerializationHint = SerializationTypeHint.Double;
            _element.DoubleValue =(double?) value;
        }

        public void Set(bool? value)
        {
            SerializationHint = SerializationTypeHint.Bool;
            _element.IntValue = (value.HasValue&&value.Value)?1:0;

        }

        public void Set(string value)
        {
            SerializationHint = SerializationTypeHint.String;
            _element.StringValue = value;
        }

        public void Set(DateTime? value)
        {
            SerializationHint = SerializationTypeHint.DateTime;
            _element.DateTimeValue = value;
        }

        public void Set(byte[] value)
        {
            SerializationHint = SerializationTypeHint.ByteArray;
            _element.RawValue = value;
        }

        public void Set(byte[] value, Type type)
        {
            SerializationHint = SerializationTypeHint.BinaryObject;
            ValueType = type.FullName;
            _element.RawValue = value;
        }

        public void Set(string value, Type type)
        {
            SerializationHint = SerializationTypeHint.JsonString;
            ValueType = type.FullName;
            _element.StringValue = value;
        }
    }
}