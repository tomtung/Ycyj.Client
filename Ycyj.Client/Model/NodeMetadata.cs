using System;
using System.Collections.Generic;
using System.Linq;

namespace Ycyj.Client.Model
{
    public class NodeMetadata
    {
        public NodeMetadata(String name, IEnumerable<NodePropertyMetadata> properties)
        {
            Name = name;
            Properties = new List<NodePropertyMetadata>(properties);
        }

        public NodeMetadata(NodeMetadata metadata)
        {
            Name = metadata.Name;
            Properties = new List<NodePropertyMetadata>(metadata.Properties);
        }

        public String Name { get; private set; }
        public IEnumerable<NodePropertyMetadata> Properties { get; private set; }
    }

    internal static class NodeMetadataHelper
    {
        public static bool HasProperty(this NodeMetadata metadata, String name)
        {
            return metadata.Properties.Any(t => t.Name == name);
        }

        public static Type GetPropertyType(this NodeMetadata nodeMetadata, String name)
        {
            NodePropertyMetadata metadata = GetPropertyMetadata(nodeMetadata, name);
            return metadata != null ? metadata.Type : null;
        }

        public static NodePropertyMetadata GetPropertyMetadata(this NodeMetadata metadata, string name)
        {
            return metadata.HasProperty(name)
                       ? metadata.Properties.First(t => t.Name == name)
                       : null;
        }
    }
}