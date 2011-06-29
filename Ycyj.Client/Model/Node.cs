using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Ycyj.Client.Model
{
    public class Node : DynamicObject
    {
        public readonly string Id;
        private readonly NodeMetadata _metadata;
        private readonly IEnumerable<NodeProperty> _properties;

        #region Constructors

        public Node(string id, NodeMetadata metadata)
        {
            Id = id;
            _metadata = metadata;

            _properties = (from pm in Metadata.Properties
                           let value = DefaultValueFor(pm.Type)
                           select new NodeProperty(pm, value)).ToList();
        }

        public Node(string id, Node node) : this(id, node.Metadata)
        {
            _properties = new List<NodeProperty>(_properties);
        }

        public Node(NodeMetadata metadata)
            : this(Guid.NewGuid().ToString(), metadata)
        {
        }

        public Node(Node node)
            : this(Guid.NewGuid().ToString(), node)
        {
        }

        #endregion Constructors

        #region Properties

        public IEnumerable<NodeProperty> Properties
        {
            get { return _properties; }
        }

        public NodeMetadata Metadata
        {
            get { return _metadata; }
        }

        #endregion

        public object this[string propertyName]
        {
            get { return GetProperty(propertyName).Value; }
            set { GetProperty(propertyName).Value = value; }
        }

        private static object DefaultValueFor(Type type)
        {
            if (type.IsValueType) return Activator.CreateInstance(type);
            if (type == typeof (string)) return string.Empty;
            return null;
        }

        public NodeProperty GetProperty(string propertyName)
        {
            return Properties.Where(p => p.PropertyName == propertyName).First();
        }

        public bool TryGetProperty(string propertyName, out NodeProperty value)
        {
            try
            {
                value = GetProperty(propertyName);
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            NodeProperty property;
            if (TryGetProperty(binder.Name, out property))
            {
                result = property.Value;
                return true;
            }
            result = null;
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            NodeProperty property;
            return TryGetProperty(binder.Name, out property) && property.TrySetValue(value);
        }
    }
}