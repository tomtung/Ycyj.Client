using System;

namespace Ycyj.Client.Model
{
    // 用来管理整个应用的节点元数据。
    // 但该有什么行为？和节点元数据冲突要不要处理？还没有想好
    // 现在只放一个根据元数据名得对象的方法进去
    public interface INodeMetadataManager
    {
        NodeMetadata this[string metadataName] { get; }
    }

    internal class MockNodeMetadataManager : INodeMetadataManager
    {
        #region INodeMetadataManager Members

        public NodeMetadata this[string metadataName]
        {
            get
            {
                switch (metadataName)
                {
                    case "知识点":
                        var propertyMetadata = new[]
                                                   {
                                                       new NodePropertyMetadata("标题", typeof (string)),
                                                       new NodePropertyMetadata("内容", typeof (MsDoc))
                                                   };
                        return new NodeMetadata("知识点", propertyMetadata);
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        #endregion
    }
}