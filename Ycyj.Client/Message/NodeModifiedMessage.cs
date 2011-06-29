using GalaSoft.MvvmLight.Messaging;
using Ycyj.Client.Model;

namespace Ycyj.Client.Message
{
    public class NodeModifiedMessage : MessageBase
    {
        private readonly NodeModifyType _modifyType;
        private readonly Node _node;

        public NodeModifiedMessage(Node node, NodeModifyType modifyType)
        {
            _node = node;
            _modifyType = modifyType;
        }

        public NodeModifiedMessage(object sender, Node node, NodeModifyType modifyType) : base(sender)
        {
            _node = node;
            _modifyType = modifyType;
        }

        public NodeModifiedMessage(object sender, object target, Node node, NodeModifyType modifyType)
            : base(sender, target)
        {
            _node = node;
            _modifyType = modifyType;
        }

        public NodeModifyType ModifyType
        {
            get { return _modifyType; }
        }

        public Node Node
        {
            get { return _node; }
        }
    }

    public enum NodeModifyType
    {
        Add,
        Update,
        Delete
    }
}