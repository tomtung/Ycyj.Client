using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Ycyj.Client.Model
{
    public class Node : DynamicObject
    {
        private readonly NodeMetadata _metadata;
        private readonly IDictionary<String, Object> _propertyValues = new Dictionary<string, object>();

        public Node(NodeMetadata metadata)
        {
            _metadata = metadata;
        }

        public Node(Node node)
        {
            _metadata = new NodeMetadata(node.Metadata);
            _propertyValues = new Dictionary<string, object>(_propertyValues);
        }

        public IEnumerable<NodeProperty> Properties
        {
            get
            {
                return from metadata in Metadata.Properties
                       let value = GetPropertyValue(metadata.Name)
                       select new NodeProperty(metadata, value);
            }
        }

        public NodeMetadata Metadata
        {
            get { return _metadata; }
        }

        public object GetPropertyValue(string propertyName)
        {
            if (!Metadata.HasProperty(propertyName))
                throw new ArgumentException(propertyName + @" is not a legal property.", "propertyName");

            // If the value is set, return it.
            if (_propertyValues.ContainsKey(propertyName))
                return _propertyValues[propertyName];

            Type type = Metadata.GetPropertyType(propertyName);
            // Otherwise, Return the default value of its type if it's of a value type
            if (type.IsValueType) return Activator.CreateInstance(type);
            // Otherwise, return an empty string if it's a string
            if (type == typeof (string)) return string.Empty;
            // Otherwise, return null
            return null;
        }

        public bool TryGetPropertyValue(string propertyName, out object value)
        {
            try
            {
                value = GetPropertyValue(propertyName);
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        public void SetPropertyValue(string propertyName, object value)
        {
            if (!Metadata.HasProperty(propertyName))
                throw new ArgumentException(propertyName + @" is not a legal property.", "propertyName");

            if (!Metadata.GetPropertyMetadata(propertyName).IsAssignableFromValue(value))
                throw new ArgumentException(propertyName + @" is not assignable from value " + value, "value");

            if (!_propertyValues.ContainsKey(propertyName))
                _propertyValues.Add(propertyName, value);
            else
                _propertyValues[propertyName] = value;
        }

        public bool TrySetPropertyValue(string propertyName, object value)
        {
            try
            {
                SetPropertyValue(propertyName, value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region DynamicObject

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return TryGetPropertyValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return TrySetPropertyValue(binder.Name, value);
        }

        #endregion
    }
}