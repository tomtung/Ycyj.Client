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
            Bind<INodeManager>().ToConstant(mockNodeManager.Object);
        }
    }
}