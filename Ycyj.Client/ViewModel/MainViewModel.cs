using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ninject;
using Ycyj.Client.Model;

namespace Ycyj.Client.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly TreeNodeViewModel _treeRoot
            = new TreeNodeViewModel(App.Kernel.Get<IKnowledgeTreeManager>().Root);

        private TreeNodeViewModel _selectedTreeNode;

        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
            }
        }

        /// <remarks>
        /// This is not a bindable property!
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
                                    NodeMetadata nodeMetadata = App.Kernel.Get<INodeMetadataManager>()["知识点"];
                                    dynamic node = new Node(nodeMetadata);
                                    node.标题 = "未命名";
                                    // TODO 把子节点通过INodeManager保存

                                    TreeNodeViewModel parentVm = SelectedTreeNode ?? _treeRoot;
                                    parentVm.AddChild(node);
                                    parentVm.IsExpanded = true;

                                    App.Kernel.Get<IKnowledgeTreeManager>().UpdateTree();
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

                                    App.Kernel.Get<IKnowledgeTreeManager>().UpdateTree();
                                },
                            () => SelectedTreeNode != null));
            }
        }

        #endregion
    }
}