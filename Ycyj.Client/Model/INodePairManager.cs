using System.Collections.Generic;

namespace Ycyj.Client.Model
{
    public interface INodePairManager
    {
        IEnumerable<Node> GetPairedNodesOf(Node node);
        void PairNodes(Node node1, Node node2);
        void UnpairNodes(Node node1, Node node2);
    }
}