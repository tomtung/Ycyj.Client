namespace Ycyj.Client.Model
{
    /// <summary>
    /// 实现对节点<see cref="Node"/>的管理，即增删改查。
    /// </summary>
    public interface INodeManager
    {
        /// <summary>
        /// 获得以<paramref name="id"/>为Id的<see cref="Node"/>对象。
        /// </summary>
        /// <returns>若对应节点存在则返回对应对象，否则返回null。</returns>
        Node GetNodeById(string id);
        
        /// <summary>
        /// 增加新节点<paramref name="node"/>。
        /// </summary>
        void AddNode(Node node);

        /// <summary>
        /// 提交节点<paramref name="node"/>的更改。
        /// </summary>
        void UpdateNode(Node node);

        /// <summary>
        /// 删除节点<paramref name="node"/>。
        /// </summary>
        void DeleteNode(Node node);
    }
}