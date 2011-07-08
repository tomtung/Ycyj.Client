using System.Collections.Generic;

namespace Ycyj.Client.Model
{
    /// <summary>
    /// ����ڵ���Ķ�Զ��ϵ��
    /// ��֪ʶ������Ŀ��Ĺ�ϵ��
    /// </summary>
    public interface INodePairManager
    {
        /// <summary>
        /// �����ڵ�<paramref name="node"/>��ԵĽڵ㡣
        /// </summary>
        /// <returns>��������<paramref name="node"/>��ԵĽڵ��򷵻�����󣬷��򷵻�null��</returns>
        IEnumerable<Node> GetPairedNodesOf(Node node);

        /// <summary>
        /// ���ڵ�<paramref name="node1"/>��<paramref name="node2"/>��ԡ��������������ʲôҲ������
        /// </summary>
        void PairNodes(Node node1, Node node2);

        /// <summary>
        /// ������ڵ�<paramref name="node1"/>��<paramref name="node2"/>����ԡ�
        /// ������δ�����ʲôҲ������
        /// </summary>
        void UnpairNodes(Node node1, Node node2);
    }
}
