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
        private readonly INodePairManager _nodePairManager;
        private readonly TreeNodeViewModel _treeRoot;

        private TreeNodeViewModel _selectedTreeNode;

        #region INPC

        public const string NodeBeingEditedPropertyName = "NodeBeingEdited";

        public const string IsEditingProblemPropertyName = "IsEditingProblem";

        private bool _isEditingProblem;
        private Node _nodeBeingEdited;

        public Node NodeBeingEdited
        {
            get { return _nodeBeingEdited; }

            set
            {
                if (_nodeBeingEdited == value)
                    return;
                _nodeBeingEdited = value;
                RaisePropertyChanged(NodeBeingEditedPropertyName);
            }
        }

        public bool IsEditingProblem
        {
            get { return _isEditingProblem; }

            set
            {
                if (_isEditingProblem == value)
                {
                    return;
                }
                _isEditingProblem = value;

                RaisePropertyChanged(IsEditingProblemPropertyName);
            }
        }

        #endregion

        #region Commands

        private ICommand _addKnowledgePointCommand;
        private ICommand _addProblemCommand;

        private ICommand _deleteKnowledgePointCommand;
        private ICommand _endAddingProblemsCommand;
        private ICommand _reloadNodeCommand;
        private ICommand _saveAndEndAddingProblemsCommand;

        private ICommand _saveNodeCommand;
        private ICommand _selectedItemChangedCommand;
        private ICommand _startAddingProblemsCommand;

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
                                    _nodeManager.DeleteNode(NodeBeingEdited);
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
                                    if (UpdateNodeCommand.CanExecute(null))
                                        UpdateNodeCommand.Execute(null);
                                    SelectedTreeNode = selected;
                                    if (!IsEditingProblem)
                                        NodeBeingEdited = SelectedTreeNode != null ? SelectedTreeNode.Node : null;
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
                            () =>
                                {
                                    TreeNodeViewModel selectedTreeNode = SelectedTreeNode;
                                    Messenger.Default.Send(
                                        new NotificationMessageAction(this, Notifications.UpdateNode,
                                                                      () =>
                                                                          {
                                                                              _nodeManager.UpdateNode(
                                                                                  selectedTreeNode.Node);
                                                                              selectedTreeNode.RaiseTitlePropertyChanged
                                                                                  ();
                                                                          }));
                                },
                            () =>
                            !IsEditingProblem && NodeBeingEdited != null &&
                            _nodeManager.GetNodeById(NodeBeingEdited.Id) != null
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
                            () => NodeBeingEdited != null && _nodeManager.GetNodeById(NodeBeingEdited.Id) != null
                            ));
            }
        }

        public ICommand StartAddingProblemsCommand
        {
            get
            {
                return _startAddingProblemsCommand ??
                       (_startAddingProblemsCommand =
                        new RelayCommand(
                            () =>
                                {
                                    if (SelectedTreeNode != null)
                                        SelectedTreeNode.IsChecked = true;
                                    NodeBeingEdited = null;
                                    IsEditingProblem = true;
                                    AddProblemCommand.Execute(null);
                                }
                            ));
            }
        }

        public ICommand EndAddingProblemsCommand
        {
            get
            {
                return _endAddingProblemsCommand ??
                       (_endAddingProblemsCommand =
                        new RelayCommand(
                            () =>
                                {
                                    IsEditingProblem = false;
                                    TreeRoot.IsChecked = false;
                                    NodeBeingEdited = null;
                                    SelectedItemChangedCommand.Execute(SelectedTreeNode);
                                }
                            ));
            }
        }

        public ICommand AddProblemCommand
        {
            get
            {
                return _addProblemCommand ??
                       (_addProblemCommand =
                        new RelayCommand(
                            () =>
                                {
                                    if (NodeBeingEdited != null)
                                        SaveCurrentProblemNode();
                                    NodeMetadata nodeMetadata = _nodeMetadataManager["题目"];
                                    NodeBeingEdited = new Node(nodeMetadata);
                                }
                            ));
            }
        }

        public ICommand SaveAndEndAddingProblemsCommand
        {
            get
            {
                return _saveAndEndAddingProblemsCommand ??
                       (_saveAndEndAddingProblemsCommand =
                        new RelayCommand(
                            () =>
                                {
                                    SaveCurrentProblemNode();
                                    EndAddingProblemsCommand.Execute(null);
                                }
                            ));
            }
        }

        private void SaveCurrentProblemNode()
        {
            Messenger.Default.Send(
                new NotificationMessageAction(this, Notifications.UpdateNode,
                                              () =>
                                                  {
                                                      _nodeManager.AddNode(NodeBeingEdited);
                                                      TryPairWithEditedNode(TreeRoot);
                                                  }));
        }

        private void TryPairWithEditedNode(TreeNodeViewModel treeNode)
        {
            if (treeNode.IsChecked == false)
                return;
            if (treeNode.IsChecked == true)
                _nodePairManager.PairNodes(NodeBeingEdited, treeNode.Node);
            foreach (TreeViewItemViewModel child in treeNode.Children)
                TryPairWithEditedNode(child as TreeNodeViewModel);
        }

        #endregion

        public MainViewModel(INodeManager nodeManager, INodeMetadataManager nodeMetadataManager,
                             IKnowledgeTreeManager knowledgeTreeManager, INodePairManager nodePairManager)
        {
            if (nodeManager == null) throw new ArgumentNullException("nodeManager");
            if (nodeMetadataManager == null) throw new ArgumentNullException("nodeMetadataManager");
            if (knowledgeTreeManager == null) throw new ArgumentNullException("knowledgeTreeManager");
            if (nodePairManager == null) throw new ArgumentNullException("nodePairManager");
            _nodeManager = nodeManager;
            _nodeMetadataManager = nodeMetadataManager;
            _knowledgeTreeManager = knowledgeTreeManager;
            _nodePairManager = nodePairManager;
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