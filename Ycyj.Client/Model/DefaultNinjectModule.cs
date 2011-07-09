using System.Diagnostics;
using Moq;
using Ninject.Modules;

namespace Ycyj.Client.Model
{
    public class DefaultNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IKnowledgeTreeManager>().To<KnowledgeTreeManager>().InSingletonScope();
            Bind<string>().ToConstant("./tree.xml").WhenInjectedInto<KnowledgeTreeManager>();
            Bind<INodeMetadataManager>().To<MockNodeMetadataManager>().InSingletonScope();
            Bind<INodeManager>().ToConstant(MockNodeManager());
        }

        private static INodeManager MockNodeManager()
        {
            var mockNodeManager = new Mock<INodeManager>();
            var nodeMetadata =
                new NodeMetadata("知识点", new[]
                                            {
                                                new NodePropertyMetadata("标题", typeof (string)),
                                                new NodePropertyMetadata("内容", typeof (MsDoc))
                                            });
            mockNodeManager.Setup(m => m.GetNodeById(It.IsAny<string>()))
                .Returns<string>(id =>
                                     {
                                         dynamic node = new Node(id, nodeMetadata);
                                         node.标题 = "Title for " + id;
                                         return node;
                                     });
            mockNodeManager.Setup(m => m.AddNode(It.IsAny<Node>()))
                .Callback((Node node) => { Debug.WriteLine("Add Node"); });
            mockNodeManager.Setup(m => m.UpdateNode(It.IsAny<Node>()))
                .Callback((Node node) => { Debug.WriteLine("Update Node"); });
            mockNodeManager.Setup(m => m.DeleteNode(It.IsAny<Node>()))
                .Callback((Node node) => { Debug.WriteLine("Delete Node"); });
            return mockNodeManager.Object;
        }
    }
}