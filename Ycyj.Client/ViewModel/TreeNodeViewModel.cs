using Ycyj.Client.Model;

namespace Ycyj.Client.ViewModel
{
    public class TreeNodeViewModel : TreeViewItemViewModel
    {
        private readonly TreeNode _treeNode;

        public TreeNodeViewModel(TreeNode treeNode, TreeViewItemViewModel parent) : base(parent)
        {
            _treeNode = treeNode;
            LoadChildren();
        }

        public TreeNodeViewModel(TreeNode treeNode) : this(treeNode, null)
        {
        }

        private TreeNode TreeNode
        {
            get { return _treeNode; }
        }

        public string Title
        {
            get { return TreeNode.Node["标题"].ToString(); }
        }

        private void LoadChildren()
        {
            foreach (TreeNode child in _treeNode.Children)
                AddChild(child);
        }

        public void AddChild(Node node)
        {
            AddChild(_treeNode.AddChild(node));
        }

        public void AddChild(TreeNode treeNode)
        {
            ChildrenSource.Add(new TreeNodeViewModel(treeNode, this));
        }

        protected override void BeforeDetachFromParent()
        {
            TreeNode.DetachFromParent();
        }
    }
}