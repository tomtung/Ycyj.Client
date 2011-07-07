using System;
using System.Diagnostics;

namespace Ycyj.Client.Model
{
    public interface IKnowledgeTreeManager
    {
        TreeNode Root { get; }
        void UpdateTree();
        void ReloadTree();
    }

    internal class MockKnowledgeTreeManager : IKnowledgeTreeManager
    {
        private readonly INodeMetadataManager _nodeMetadataManager;
        private TreeNode _root;

        public MockKnowledgeTreeManager(INodeMetadataManager nodeMetadataManager)
        {
            if (nodeMetadataManager == null) throw new ArgumentNullException("nodeMetadataManager");
            _nodeMetadataManager = nodeMetadataManager;
            ResetRoot();
        }

        #region IKnowledgeTreeManager Members

        public TreeNode Root
        {
            get
            {
                Debug.WriteLine("MockKnowledgeTreeManager.Root");
                return _root;
            }
        }

        public void UpdateTree()
        {
            Debug.WriteLine("MockKnowledgeTreeManager.UpdateTree()");

            Action<TreeNode, int> printTreeNode = null;
            printTreeNode =
                (node, depth) =>
                    {
                        for (int i = 0; i < depth; ++i)
                            Debug.Write('-');
                        Debug.WriteLine(node.Node["标题"]);

                        foreach (TreeNode child in node.Children)
                            printTreeNode(child, depth + 1);
                    };

            printTreeNode(_root, 0);
        }

        public void ReloadTree()
        {
            Debug.WriteLine("MockKnowledgeTreeManager.ReloadTree()");
            ResetRoot();
        }

        #endregion

        private void ResetRoot()
        {
            NodeMetadata nodeMetadata = _nodeMetadataManager["知识点"];

            dynamic n0 = new Node("0", nodeMetadata);
            n0.标题 = "n0";
            dynamic n1 = new Node("1", nodeMetadata);
            n1.标题 = "n1";
            dynamic n2 = new Node("2", nodeMetadata);
            n2.标题 = "n2";
            var n3 = new Node("3", nodeMetadata);
            n3["标题"] = "n3";

            _root = new TreeNode(n0);
            {
                _root.AddChild(n1);
                dynamic child2 = _root.AddChild(n2);
                child2.AddChild(n3);
            }
        }
    }
}