using System;
using System.Runtime.Serialization;

namespace Ycyj.Client.Model
{
    /// <summary>
    /// 用于载入和保存知识点树的类实现此方法。
    /// </summary>
    public interface IKnowledgeTreeManager
    {
        /// <summary>
        /// 知识树的根。它不对应任何<see cref="Node"/>。
        /// </summary>
        TreeNode Root { get; }

        /// <summary>
        /// 保存对以<see cref="Root"/>为根的知识树所做的修改。
        /// </summary>
        void UpdateTree();

        /// <summary>
        /// 忽略当前对以<see cref="Root"/>为根的树的修改，重新载入。
        /// <see cref="Root"/>对象本身不会改变，但其<see cref="TreeNode.Children"/>内容会被重置。
        /// </summary>
        /// <exception cref="KnowledgeTreeLoadFailException">当重新载入失败时抛出。</exception>
        void ReloadTree();

        /// <summary>
        /// 重置整棵树为空，且<see cref="Root"/>被置为一个新对象。
        /// </summary>
        void ResetTree();
    }

    public static class KnowledgeTreeManagerHelper
    {
        /// <returns>载入成功则返回<code>true</code>，否则返回<code>false</code></returns>
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