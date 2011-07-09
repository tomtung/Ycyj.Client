using System.Collections.Generic;

namespace Ycyj.Client.Model
{
    /// <summary>
    /// 管理节点见的多对多关系。
    /// 如知识点与题目间的关系。
    /// </summary>
    public interface INodePairManager
    {
        /// <summary>
        /// 获得与节点<paramref name="node"/>配对的节点。
        /// </summary>
        /// <returns>若存在与<paramref name="node"/>配对的节点则返回其对象，否则返回null。</returns>
        IEnumerable<Node> GetPairedNodesOf(Node node);

        /// <summary>
        /// 将节点<paramref name="node1"/>与<paramref name="node2"/>配对。若它们已配对则什么也不做。
        /// </summary>
        void PairNodes(Node node1, Node node2);

        /// <summary>
        /// 解除将节点<paramref name="node1"/>与<paramref name="node2"/>的配对。
        /// 若它们未配对则什么也不做。
        /// </summary>
        void UnpairNodes(Node node1, Node node2);
    }
}