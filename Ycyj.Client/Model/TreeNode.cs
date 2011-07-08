using System.Collections.Generic;

namespace Ycyj.Client.Model
{
    /// <summary>
    /// 表示当节点以树状形式组织时，树中的一个节点。
    /// </summary>
    public class TreeNode
    {
        private readonly IList<TreeNode> _children = new List<TreeNode>();
        private readonly Node _node;

        /// <summary>
        /// 构造一个<see cref="TreeNode"/>，
        /// 其对应节点为<paramref name="node"/>，父节点为<paramref name="parent"/>。
        /// </summary>
        public TreeNode(Node node, TreeNode parent = null)
        {
            _node = node;
            Parent = parent;
        }

        /// <summary>
        /// 树节点对应节点的Id。
        /// </summary>
        public string Id
        {
            get { return Node.Id; }
        }

        /// <summary>
        /// 树节点对应的节点。
        /// </summary>
        public Node Node
        {
            get { return _node; }
        }

        /// <summary>
        /// 树节点的父节点。
        /// </summary>
        public TreeNode Parent { get; private set; }

        /// <summary>
        /// 树节点的子节点。
        /// </summary>
        public IEnumerable<TreeNode> Children
        {
            get { return _children; }
        }

        /// <summary>
        /// 给树节点增加子节点<paramref name="node"/>。
        /// </summary>
        public TreeNode AddChild(Node node)
        {
            var child = new TreeNode(node, this);
            _children.Add(child);
            return child;
        }

        /// <summary>
        /// 解除此节点与其夫节点的关系。
        /// </summary>
        public void DetachFromParent()
        {
            if (Parent == null) return;
            Parent._children.Remove(this);
            Parent = null;
        }
    }
}