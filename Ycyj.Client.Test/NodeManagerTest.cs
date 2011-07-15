using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Moq;
using Xunit;
using Ycyj.Client.Model;

namespace Ycyj.Client.Test
{
    public class NodeManagerTest : IDisposable
    {
        private const string NonExistentFileName = "some_non-existent_file.xml";

        private const string CorruptedFileName = "corrupted_tree.xml";
        private const string CorrptedFileContent = "This is not a valid xml.";

        private const string CorrectFileName = "correct_node_pair.xml";


        //private readonly Mock<INodeManager> _mockNodeManager = new Mock<INodeManager>();

        private readonly NodeMetadata _nodeMetadata
            = new NodeMetadata("知识点", new[]
                                          {
                                              new NodePropertyMetadata("标题", typeof (string)),
                                              new NodePropertyMetadata("内容", typeof (MsDoc))
                                          });
        private readonly Node nodea = new Node("aaa", new NodeMetadata("知识点", new[]
                                          {
                                              new NodePropertyMetadata("标题", typeof (string)),
                                              new NodePropertyMetadata("内容", typeof (MsDoc))
                                          }));
        private readonly Node nodeb = new Node("bbb", new NodeMetadata("知识点", new[]
                                          {
                                              new NodePropertyMetadata("标题", typeof (string)),
                                              new NodePropertyMetadata("内容", typeof (MsDoc))
                                          }));
        private readonly Node nodec = new Node("ccc", new NodeMetadata("知识点", new[]
                                          {
                                              new NodePropertyMetadata("标题", typeof (string)),
                                              new NodePropertyMetadata("内容", typeof (MsDoc))
                                          }));
        private readonly Node noded = new Node("ddd", new NodeMetadata("知识点", new[]
                                          {
                                              new NodePropertyMetadata("标题", typeof (string)),
                                              new NodePropertyMetadata("内容", typeof (MsDoc))
                                          }));
        public NodeManagerTest()
        {
            //SetUpMockNodeManager();
            SetUpTestFiles();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (File.Exists(CorruptedFileName))
                File.Delete(CorruptedFileName);
            if (File.Exists(CorrectFileName))
                File.Delete(CorrectFileName);
        }

        #endregion

        private void SetUpTestFiles()
        {
            if (File.Exists(NonExistentFileName)) File.Delete(NonExistentFileName);

            File.WriteAllText(CorruptedFileName, CorrptedFileContent);

        }

        /*
        private void SetUpMockNodeManager()
        {
            _mockNodeManager.Setup(m => m.GetNodeById(It.IsAny<string>()))
                .Returns<string>(id =>
                {
                    dynamic node = new Node(id, _nodeMetadata);
                    node.标题 = "Title for " + id;
                    return node;
                });
        }
         * */


        [Fact]
        public void Given_id_not_exist_should_getbyid_method_ok()
        {
            var nodeManager = new NodeManager();
            var node = nodeManager.GetNodeById("1");
            node.Id.Should().Be("1");
            node.Metadata.Name.Should().Be("题目");
            node.Metadata.Properties.Count().Should().Be(5);
            node.Metadata.Properties.ToArray().ElementAt(0).Name.Should().Be("题面");
            node.Metadata.Properties.ToArray().ElementAt(4).Name.Should().Be("难度");
            dynamic x = node.Properties.ElementAt(0).Value;
            (x.Content as string).Should().Be("ttfdfsfdsf");
            node.Properties.ElementAt(4).Value.Should().Be(3);
            node.Properties.ElementAt(3).Value.Should().Be("这是题目注释的内容。");
        }

        [Fact]
        public void Given_id_exist_should_add_node_ok()
        {
            var nodeManager = new NodeManager();
            var node = new Node("mmm", _nodeMetadata);
            var msDoc = new MsDoc();
            msDoc.Content = "Dsafd";
            node.Properties.ElementAt(0).Value = "fds";
            node.Properties.ElementAt(1).Value = msDoc;
            nodeManager.AddNode(node);
            Directory.Exists("mmm").Should().BeTrue();
        }

        [Fact]
        public void Given_id_exist_should_update_node_ok()
        {
            var nodeManager = new NodeManager();
            var node = new Node("mmm", _nodeMetadata);
            var msDoc = new MsDoc();
            msDoc.Content = "Ddddddddsafd";
            node.Properties.ElementAt(0).Value = "ffffds";
            node.Properties.ElementAt(1).Value = msDoc;
            nodeManager.UpdateNode(node);
            Directory.Exists("mmm").Should().BeTrue();
        }

        [Fact]
        public void Given_id_exist_should_delete_node_ok()
        {
            var nodeManager = new NodeManager();
            var node = new Node("mmm", _nodeMetadata);
            var msDoc = new MsDoc();
            msDoc.Content = "Ddddddddsafd";
            node.Properties.ElementAt(0).Value = "ffffds";
            node.Properties.ElementAt(1).Value = msDoc;
            nodeManager.DeleteNode(node);
            Directory.Exists("mmm").Should().BeFalse();
        }


    }

}