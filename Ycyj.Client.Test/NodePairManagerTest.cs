﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using FluentAssertions;
using Moq;
using Xunit;
using Ycyj.Client.Model;

namespace Ycyj.Client.Test
{
    public class NodePairManagerTest : IDisposable
    {
        private const string NonExistentFileName = "some_non-existent_file.xml";

        private const string CorruptedFileName = "corrupted_tree.xml";
        private const string CorrptedFileContent = "This is not a valid xml.";

        private const string CorrectFileName = "correct_node_pair.xml";
        private const string CorrectFileContent =
            @"<?xml version=""1.0"" encoding=""utf-8""?>
    <NodePairs>
	<NodePair>
		<Node Id=""aaa"" />
		<Node Id=""bbb"" />
	</NodePair>
	<NodePair>
		<Node Id=""aaa"" />
		<Node Id=""ccc"" />
	</NodePair>
	<NodePair>
		<Node Id=""aaa"" />
		<Node Id=""ddd"" />
	</NodePair>
	<NodePair>
		<Node Id=""ccc"" />
		<Node Id=""ddd"" />
	</NodePair>
</NodePairs>";

        private readonly Mock<INodeManager> _mockNodeManager = new Mock<INodeManager>();

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
        public NodePairManagerTest()
        {
            SetUpMockNodeManager();
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

            File.WriteAllText(CorrectFileName, CorrectFileContent);
        }

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

        [Fact]
        public void Given_file_not_exist_when_load_should_file_be_created()
        {
            File.Exists(NonExistentFileName).Should().BeFalse();
            var nodePairManager = new NodePairManager(NonExistentFileName, _mockNodeManager.Object);
            File.Exists(NonExistentFileName).Should().BeTrue();
        }

        [Fact]
        public void Given_file_is_valid_when_load_should_pair_nodes_be_correct()
        {
            var nodePairManager = new NodePairManager(CorrectFileName, _mockNodeManager.Object);
            File.WriteAllText(CorrectFileName, CorrectFileContent);
            Node[] pairNodes = nodePairManager.GetPairedNodesOf(nodea).ToArray();
            pairNodes.ElementAt(0).Id.Should().Be("bbb");
            pairNodes.ElementAt(1).Id.Should().Be("ccc");
            pairNodes.ElementAt(2).Id.Should().Be("ddd");
        }

        [Fact]
        public void Given_file_is_valid_when_load_shold_pair_and_unpair_method_be_correct()
        {
            var nodePairManager = new NodePairManager(CorrectFileName, _mockNodeManager.Object);
            File.WriteAllText(CorrectFileName, CorrectFileContent);
            bool flag = false;
            var document = new XmlDocument();
            document.Load(CorrectFileName);
            var nodePairElementList = document.GetElementsByTagName("NodePair");
            foreach (XmlElement nodePairElement in nodePairElementList)
            {
                if (nodePairElement.ChildNodes[0].Attributes["Id"].Value.Equals("aaa") && nodePairElement.ChildNodes[1].Attributes["Id"].Value.Equals("bbb"))
                    flag = true;
                if (nodePairElement.ChildNodes[1].Attributes["Id"].Value.Equals("aaa") && nodePairElement.ChildNodes[0].Attributes["Id"].Value.Equals("bbb"))
                    flag = true;
            }
            flag.Should().BeTrue();
            nodePairManager.UnpairNodes(nodea, nodeb);

            document.Load(CorrectFileName);
            nodePairElementList = document.GetElementsByTagName("NodePair");
            foreach (XmlElement nodePairElement in nodePairElementList)
            {
                if (nodePairElement.ChildNodes[0].Attributes["Id"].Value.Equals(nodea.Id))
                    nodePairElement.ChildNodes[1].Attributes["Id"].Value.Should().NotBe("bbb");
                if (nodePairElement.ChildNodes[1].Attributes["Id"].Value.Equals(nodea.Id))
                    nodePairElement.ChildNodes[0].Attributes["Id"].Value.Should().NotBe("bbb");
            }

            nodePairManager.PairNodes(nodea, nodeb);

            document.Load(CorrectFileName);
            nodePairElementList = document.GetElementsByTagName("NodePair");
            foreach (XmlElement nodePairElement in nodePairElementList)
            {
                if (nodePairElement.ChildNodes[0].Attributes["Id"].Value.Equals("aaa") && nodePairElement.ChildNodes[1].Attributes["Id"].Value.Equals("bbb"))
                    flag = true;
                if (nodePairElement.ChildNodes[1].Attributes["Id"].Value.Equals("aaa") && nodePairElement.ChildNodes[0].Attributes["Id"].Value.Equals("bbb"))
                    flag = true;
            }
            flag.Should().BeTrue();
        }


    }

}