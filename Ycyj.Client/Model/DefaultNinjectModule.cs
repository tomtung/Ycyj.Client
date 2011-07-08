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

            var mockNodeManager = new Mock<INodeManager>();
            var nodeMetadata =
                new NodeMetadata("֪ʶ��", new[]
                                            {
                                                new NodePropertyMetadata("����", typeof (string)),
                                                new NodePropertyMetadata("����", typeof (MsDoc))
                                            });
            mockNodeManager.Setup(m => m.GetNodeById(It.IsAny<string>()))
                .Returns<string>(id =>
                {
                    dynamic node = new Node(id, nodeMetadata);
                    node.���� = "Title for " + id;
                    return node;
                });
            Bind<INodeManager>().ToConstant(mockNodeManager.Object);
        }
    }
}