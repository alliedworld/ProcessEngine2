using System;
using Klaudwerk.PropertySet;
using Klaudwerk.PropertySet.Serialization;

namespace KlaudWerk.ProcessEngine.Persistence
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