using System;
using System.Collections.Generic;
using System.Linq;

namespace Ycyj.Client.Model
{
    /// <summary>
    /// 节点元数据，描述了一个节点类型的信息。
    /// </summary>
    public class NodeMetadata
    {
        /// <summary>
        /// 构造一个<see cref="NodeMetadata"/>对象。
        /// </summary>
        /// <param name="name">节点元数据的名称。</param>
        /// <param name="properties">该元数据所描述节点类型应包含的属性。</param>
        public NodeMetadata(String name, IEnumerable<NodePropertyMetadata> properties)
        {
            Name = name;
            Properties = new List<NodePropertyMetadata>(properties);
        }

        /// <summary>
        /// 从<paramref name="metadata"/>复制得到一份新的<see cref="NodeMetadata"/>对象。
        /// </summary>
        public NodeMetadata(NodeMetadata metadata)
        {
            Name = metadata.Name;
            Properties = new List<NodePropertyMetadata>(metadata.Properties);
        }

        /// <summary>
        /// 节点元数据的名称。
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// 该元数据所描述节点类型应包含的属性。
        /// </summary>
        public IEnumerable<NodePropertyMetadata> Properties { get; private set; }
    }

    internal static class NodeMetadataHelper
    {
        /// <summary>
        /// 返回<paramref name="metadata"/>是否包含名为<paramref name="name"/>的属性。
        /// </summary>
        public static bool HasProperty(this NodeMetadata metadata, String name)
        {
            return metadata.Properties.Any(t => t.Name == name);
        }

        /// <summary>
        /// 返回<paramref name="metadata"/>中名为<paramref name="name"/>的属性元信息<see cref="NodePropertyMetadata"/>。
        /// </summary>
        public static NodePropertyMetadata GetPropertyMetadata(this NodeMetadata metadata, string name)
        {
            return metadata.HasProperty(name)
                       ? metadata.Properties.First(t => t.Name == name)
                       : null;
        }
    }
}