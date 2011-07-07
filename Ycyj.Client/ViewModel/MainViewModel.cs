using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ycyj.Client.Model;

namespace Ycyj.Client.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IKnowledgeTreeManager _knowledgeTreeManager;

        private readonly INodeMetadataManager _nodeMetadataManager;
        private readonly TreeNodeViewModel _treeRoot;

        private TreeNodeViewModel _selectedTreeNode;

        #region Commands

        private ICommand _addKnowledgePointCommand;

        private ICommand _deleteKnowledgePointCommand;

        public ICommand AddKnowledgePointCommand
        {
            get
            {
                return _addKnowledgePointCommand ??
                       (_addKnowledgePointCommand =
                        new RelayCommand(
                            () =>
                                {
                                    NodeMetadata nodeMetadata = _nodeMetadataManager["知识点"];
                                    dynamic node = new Node(nodeMetadata);
                                    node.标题 = "未命名";
                                    // TODO 把子节点通过INodeManager保存

                                    TreeNodeViewModel parentVm = SelectedTreeNode ?? _treeRoot;
                                    parentVm.AddChild(node);
                                    parentVm.IsExpanded = true;

                                    _knowledgeTreeManager.UpdateTree();
                                }));
            }
        }

        public ICommand DeleteKnowledgePointCommand
        {
            get
            {
                return _deleteKnowledgePointCommand ??
                       (_deleteKnowledgePointCommand =
                        new RelayCommand(
                            () =>
                                {
                                    SelectedTreeNode.DetachFromParent();
                                    // TODO 把子节点通过INodeManager都删掉

                                    _knowledgeTreeManager.UpdateTree();
                                },
                            () => SelectedTreeNode != null));
            }
        }

        #endregion

        public MainViewModel(IKnowledgeTreeManager knowledgeTreeManager, INodeMetadataManager nodeMetadataManager)
        {
            if (knowledgeTreeManager == null) throw new ArgumentNullException("knowledgeTreeManager");
            if (nodeMetadataManager == null) throw new ArgumentNullException("nodeMetadataManager");
            _knowledgeTreeManager = knowledgeTreeManager;
            _nodeMetadataManager = nodeMetadataManager;
            _treeRoot = new TreeNodeViewModel(_knowledgeTreeManager.Root);
        }

        /// <remarks>
        /// Note: This is not a bindable property!
        /// To keep sync, the following code should be registered to <see cref="System.Windows.Controls.TreeView.SelectedItemChanged"/>.
        /// <code>
        /// private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs&lt;object&gt; e)
        /// {
        /// 	try
        /// 	{
        /// 		dynamic treeView = sender;
        /// 		treeView.DataContext.SelectedTreeNode = treeView.SelectedItem;
        /// 	}catch{}
        /// }
        /// </code>
        /// </remarks>
        public TreeNodeViewModel SelectedTreeNode
        {
            get { return _selectedTreeNode; }
            set
            {
                if (_selectedTreeNode == value) return;
                _selectedTreeNode = value;
                if (_selectedTreeNode != null) _selectedTreeNode.IsSelected = true;
            }
        }

        public TreeNodeViewModel TreeRoot
        {
            get { return _treeRoot; }
        }
    }
}