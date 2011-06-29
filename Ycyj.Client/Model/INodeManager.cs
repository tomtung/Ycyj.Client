namespace Ycyj.Client.Model
{
    public interface INodeManager
    {
        Node GetNodeById(string id);
        void AddNode(Node node);
        void UpdateNode(Node node);
        void DeleteNode(Node node);
    }
}