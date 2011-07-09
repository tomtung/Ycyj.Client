using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Ycyj.Client.Message;
using Ycyj.Client.Model;

namespace Ycyj.Client.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IKnowledgeTreeManager _knowledgeTreeManager;
        private readonly INodeManager _nodeManager;
        private readonly INodeMetadataManager _nodeMetadataManager;
        private readonly TreeNodeViewModel _treeRoot;

        private TreeNodeViewModel _selectedTreeNode;

        #region INPC

        public const string SelectedNodePropertyName = "SelectedNode";
        private Node _selectedNode;

        public Node SelectedNode
        {
            get { return _selectedNode; }

            set
            {
                if (_selectedNode == value)
                    return;
                _selectedNode = value;
                RaisePropertyChanged(SelectedNodePropertyName);
            }
        }

        #endregion

        #region Commands

        private ICommand _addKnowledgePointCommand;

        private ICommand _deleteKnowledgePointCommand;
        private ICommand _reloadNodeCommand;

        private ICommand _saveNodeCommand;
        private ICommand _selectedItemChangedCommand;

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
                                    _nodeManager.AddNode(node);
                                    TreeNodeViewModel parentVm = SelectedTreeNode ?? TreeRoot;
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
                                    _nodeManager.DeleteNode(SelectedNode);
                                    SelectedTreeNode.DetachFromParent();
                                    _knowledgeTreeManager.UpdateTree();
                                },
                            () => SelectedTreeNode != null));
            }
        }

        public ICommand SelectedItemChangedCommand
        {
            get
            {
                return _selectedItemChangedCommand ??
                       (_selectedItemChangedCommand =
                        new RelayCommand<TreeNodeViewModel>(
                            selected =>
                                {
                                    UpdateNodeCommand.Execute(null);
                                    SelectedTreeNode = selected;
                                    SelectedNode = SelectedTreeNode.Node;
                                }));
            }
        }

        public ICommand UpdateNodeCommand
        {
            get
            {
                return _saveNodeCommand ??
                       (_saveNodeCommand =
                        new RelayCommand(
                            () => Messenger.Default.Send(
                                new NotificationMessageAction(this, Notifications.UpdateNode,
                                                              () => _nodeManager.UpdateNode(SelectedNode))),
                            () => SelectedNode != null && _nodeManager.GetNodeById(SelectedNode.Id) != null
                            ));
            }
        }

        public ICommand ReloadNodeCommand
        {
            get
            {
                return _reloadNodeCommand ??
                       (_reloadNodeCommand =
                        new RelayCommand(
                            () => Messenger.Default.Send(new NotificationMessage(this, Notifications.ReloadNode)),
                            () => SelectedNode != null && _nodeManager.GetNodeById(SelectedNode.Id) != null
                            ));
            }
        }

        #endregion

        public MainViewModel(INodeManager nodeManager, INodeMetadataManager nodeMetadataManager,
                             IKnowledgeTreeManager knowledgeTreeManager)
        {
            if (nodeManager == null) throw new ArgumentNullException("nodeManager");
            if (nodeMetadataManager == null) throw new ArgumentNullException("nodeMetadataManager");
            if (knowledgeTreeManager == null) throw new ArgumentNullException("knowledgeTreeManager");
            _nodeManager = nodeManager;
            _nodeMetadataManager = nodeMetadataManager;
            _knowledgeTreeManager = knowledgeTreeManager;
            _treeRoot = new TreeNodeViewModel(_knowledgeTreeManager.Root);
        }

        /// <remarks>
        /// Note: This is not a bindable property!
        /// To keep sync, the view should fire SelectedItemChangedCommand (via EventToCommand)
        /// and pass the new selected item as its parameter.
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