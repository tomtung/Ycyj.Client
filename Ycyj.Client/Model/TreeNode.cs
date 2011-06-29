using System.Collections.Generic;

namespace Ycyj.Client.Model
{
    public class TreeNode
    {
        private readonly IList<TreeNode> _children = new List<TreeNode>();
        private readonly Node _node;

        public TreeNode(Node node, TreeNode parent = null)
        {
            _node = node;
            Parent = parent;
        }

        public string Id
        {
            get { return Node.Id; }
        }

        public Node Node
        {
            get { return _node; }
        }

        public TreeNode Parent { get; private set; }

        public IEnumerable<TreeNode> Children
        {
            get { return _children; }
        }

        public TreeNode AddChild(Node node)
        {
            var child = new TreeNode(node, this);
            _children.Add(child);
            return child;
        }

        public void DetachFromParent()
        {
            if (Parent == null) return;
            Parent._children.Remove(this);
            Parent = null;
        }
    }
}