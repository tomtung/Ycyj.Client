using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Ycyj.Client.Model
{
    /// <summary>
    /// 一个单节点，可能是题目、知识点或其它内容。
    /// 只包含节点本身内容，不包括彼此间关系。
    /// </summary>
    public class Node : DynamicObject
    {
        /// <summary>
        /// 节点全局唯一的标识。
        /// </summary>
        public readonly string Id;
        private readonly NodeMetadata _metadata;
        private readonly IEnumerable<NodeProperty> _properties;

        #region Constructors

        /// <summary>
        /// 以<paramref name="id" />为<see cref="Id"/>，
        /// <paramref name="metadata"/>为<see cref="Metadata"/>创建新节点。
        /// 节点各属性其类型的赋默认值（但string赋string.Empty）。
        /// </summary>
        public Node(string id, NodeMetadata metadata)
        {
            Id = id;
            _metadata = metadata;

            _properties = (from pm in Metadata.Properties
                           let value = DefaultValueFor(pm.Type)
                           select new NodeProperty(pm, value)).ToList();
        }

        /// <summary>
        /// 复制<paramref name="node"/>的内容，
        /// 并以<paramref name="id"/>为<see cref="Id"/>创建新节点。
        /// </summary>
        public Node(string id, Node node) : this(id, node.Metadata)
        {
            _properties = new List<NodeProperty>(_properties);
        }

        /// <summary>
        /// 以<paramref name="metadata"/>为<see cref="Metadata"/>创建新节点。
        /// 节点<see cref="Id"/>自动生成。
        /// 节点各属性其类型的赋默认值（但string赋string.Empty）。
        /// </summary>
        public Node(NodeMetadata metadata)
            : this(Guid.NewGuid().ToString(), metadata)
        {
        }

        /// <summary>
        /// 复制<paramref name="node"/>的内容。
        /// 节点<see cref="Id"/>自动生成。
        /// </summary>
        public Node(Node node)
            : this(Guid.NewGuid().ToString(), node)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// 节点的属性。
        /// </summary>
        public IEnumerable<NodeProperty> Properties
        {
            get { return _properties; }
        }

        /// <summary>
        /// 节点的元信息。
        /// </summary>
        public NodeMetadata Metadata
        {
            get { return _metadata; }
        }

        #endregion

        /// <summary>
        /// 通过属性名<paramref name="propertyName"/>访问属性值。
        /// </summary>
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

        /// <summary>
        /// 通过属性名<paramref name="propertyName"/>访问属性对象。
        /// </summary>
        public NodeProperty GetProperty(string propertyName)
        {
            return Properties.Where(p => p.PropertyName == propertyName).First();
        }

        /// <summary>
        /// 通过属性名<paramref name="propertyName"/>访问属性对象<paramref name="value"/>。
        /// </summary>
        /// <returns>当属性存在则返回true，否则返回false。</returns>
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