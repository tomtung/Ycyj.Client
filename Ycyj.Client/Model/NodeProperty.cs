using System;

namespace Ycyj.Client.Model
{
    public class NodeProperty
    {
        private readonly NodePropertyMetadata _metadata;
        private object _value;

        public NodeProperty(NodePropertyMetadata metadata, object value)
        {
            if (metadata == null) throw new ArgumentNullException("metadata");
            if (!metadata.IsAssignableFromValue(value))
                throw new ArgumentException(@"The type " + metadata.Type + @" is not assignable from value " + value,
                                            "value");

            _metadata = metadata;
            Value = value;
        }

        public NodePropertyMetadata PropertyMetadata
        {
            get { return _metadata; }
        }

        public object Value
        {
            get { return _value; }
            set
            {
                if (!PropertyMetadata.IsAssignableFromValue(value))
                    throw new NotSupportedException();
                _value = value;
            }
        }

        public string PropertyName
        {
            get { return _metadata.Name; }
        }

        public Type PropertyType
        {
            get { return _metadata.Type; }
        }

        public bool TrySetValue(object value)
        {
            try
            {
                Value = value;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}