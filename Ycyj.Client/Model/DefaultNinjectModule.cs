using Ninject.Modules;

namespace Ycyj.Client.Model
{
    public class DefaultNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IKnowledgeTreeManager>().To<MockKnowledgeTreeManager>().InSingletonScope();
            Bind<INodeMetadataManager>().To<MockNodeMetadataManager>().InSingletonScope();
        }
    }
}