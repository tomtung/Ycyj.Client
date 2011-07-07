using System;
using System.Runtime.Serialization;

namespace Ycyj.Client.Model
{
    /// <summary>
    /// ��������ͱ���֪ʶ��������ʵ�ִ˷�����
    /// </summary>
    public interface IKnowledgeTreeManager
    {
        /// <summary>
        /// ֪ʶ���ĸ���������Ӧ�κ�<see cref="Node"/>��
        /// </summary>
        TreeNode Root { get; }

        /// <summary>
        /// �������<see cref="Root"/>Ϊ����֪ʶ���������޸ġ�
        /// </summary>
        void UpdateTree();

        /// <summary>
        /// ���Ե�ǰ����<see cref="Root"/>Ϊ���������޸ģ��������롣
        /// <see cref="Root"/>��������ı䣬����<see cref="TreeNode.Children"/>���ݻᱻ���á�
        /// </summary>
        /// <exception cref="KnowledgeTreeLoadFailException">����������ʧ��ʱ�׳���</exception>
        void ReloadTree();

        /// <summary>
        /// ����������Ϊ�գ���<see cref="Root"/>����Ϊһ���¶���
        /// </summary>
        void ResetTree();
    }

    public static class KnowledgeTreeManagerHelper
    {
        /// <returns>����ɹ��򷵻�<code>true</code>�����򷵻�<code>false</code></returns>
        public static bool TryReloadTree(this IKnowledgeTreeManager treeManager)
        {
            try
            {
                treeManager.UpdateTree();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public class KnowledgeTreeLoadFailException : Exception
    {
        public KnowledgeTreeLoadFailException()
        {
        }

        public KnowledgeTreeLoadFailException(string message) : base(message)
        {
        }

        public KnowledgeTreeLoadFailException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected KnowledgeTreeLoadFailException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}