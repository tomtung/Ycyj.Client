using System;

namespace Ycyj.Client.Model
{
    public class NodePropertyMetadata
    {
        private readonly string _name;
        private readonly Type _type;

        public NodePropertyMetadata(string name, Type type)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (type == null) throw new ArgumentNullException("type");
            _name = name;
            _type = type;
        }

        public string Name
        {
            get { return _name; }
        }

        public Type Type
        {
            get { return _type; }
        }

        public override string ToString()
        {
            return string.Format("{0} : {1}", _name, _type);
        }

        public bool Equals(NodePropertyMetadata other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._name, _name) && Equals(other._type, _type);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (NodePropertyMetadata)) return false;
            return Equals((NodePropertyMetadata) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_name != null ? _name.GetHashCode() : 0)*397) ^ (_type != null ? _type.GetHashCode() : 0);
            }
        }
    }

    public static class NodePropertyMetadataHelper
    {
        public static bool IsAssignableFromValue(this NodePropertyMetadata metadata, object value)
        {
            if (value == null) return !metadata.Type.IsValueType;
            return metadata.Type.IsAssignableFrom(value.GetType());
        }
    }
}