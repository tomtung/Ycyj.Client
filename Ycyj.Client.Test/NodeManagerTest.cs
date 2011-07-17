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

        public NodeManagerTest()
        {
            SetUpTestFiles();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (File.Exists(CorruptedFileName))
                File.Delete(CorruptedFileName);
            if (File.Exists(CorrectFileName))
                File.Delete(CorrectFileName);
            if(File.Exists(MetaDataXmlFileName))
                File.Delete(MetaDataXmlFileName);
            if (File.Exists(DataXmlFileName))
                File.Delete(DataXmlFileName);
            if(File.Exists(DaanXmlFileName))
                File.Delete(DaanXmlFileName);
            if(File.Exists(FenxiXmlFileName))
                File.Delete(FenxiXmlFileName);
            if(File.Exists(TimianXmlFileName))
                File.Delete(TimianXmlFileName);
        }

        #endregion
        private const string DataXmlFileName = "./root/1/data.xml";
        private const string DataXmlFileContent = 
            @"<?xml version=""1.0"" encoding=""utf-8""?>
<Node>
	<题面 File=""题面.xml""/>
	<分析 File=""分析.xml"" />
	<答案 File=""答案.xml"" />
	<注释>这是题目注释的内容。</注释>
	<难度>3</难度>
</Node>";
        private const string MetaDataXmlFileName = "./root/1/metadata.xml";
        private const string MetaDataXmlFileContent =
            @"<?xml version=""1.0"" encoding=""utf-8""?>
<NodeMetadata Name=""题目"">
    <NodeProperty Name=""题面"" Type=""msdoc"" />
    <NodeProperty Name=""分析"" Type=""msdoc"" />
    <NodeProperty Name=""答案"" Type=""msdoc"" />
    <NodeProperty Name=""注释"" Type=""string"" />
    <NodeProperty Name=""难度"" Type=""int"" />
</NodeMetadata>";

        private const string FenxiXmlFileName = "./root/1/分析.xml";
        private const string TimianXmlFileName = "./root/1/题面.xml";
        private const string DaanXmlFileName = "./root/1/答案.xml";
        private void SetUpTestFiles()
        {
            if (File.Exists(NonExistentFileName)) File.Delete(NonExistentFileName);

            File.WriteAllText(CorruptedFileName, CorrptedFileContent);

            Directory.CreateDirectory("./root/1");

            File.WriteAllText(DataXmlFileName, DataXmlFileContent);

            File.WriteAllText(MetaDataXmlFileName, MetaDataXmlFileContent);

            File.WriteAllText("./root/1/分析.xml", @"这是分析");
            File.WriteAllText("./root/1/题面.xml", @"这是题面");
            File.WriteAllText("./root/1/答案.xml", @"这是答案");
        }

        [Fact]
        public void Given_id_not_exist_should_getbyid_method_ok()
        {
            var nodeManager = new NodeManager("./root");
            var node = nodeManager.GetNodeById("1");
            node.Id.Should().Be("1");
            node.Metadata.Name.Should().Be("题目");
            node.Metadata.Properties.Count().Should().Be(5);
            node.Metadata.Properties.ToArray().ElementAt(0).Name.Should().Be("题面");
            node.Metadata.Properties.ToArray().ElementAt(4).Name.Should().Be("难度");
            dynamic x = node.Properties.ElementAt(0).Value;
            (x.Content as string).Should().Be("这是题面");
            node.Properties.ElementAt(4).Value.Should().Be(3);
            node.Properties.ElementAt(3).Value.Should().Be("这是题目注释的内容。");
        }

        [Fact]
        public void Given_id_exist_should_add_node_ok()
        {
            var nodeManager = new NodeManager("./root");
            dynamic node = new Node("mmm", _nodeMetadata);
            node.标题 = "标题";
            node.内容 = new MsDoc {Content = "内容"};
            nodeManager.AddNode(node);
            Directory.Exists("./root/mmm").Should().BeTrue();
            nodeManager.DeleteNode(node);
        }

        [Fact]
        public void Given_id_exist_should_update_node_ok()
        {
            var nodeManager = new NodeManager("./root");
            dynamic node = new Node("xxx", _nodeMetadata);
            node.标题 = "标题";
            node.内容 = new MsDoc { Content = "内容" };
            nodeManager.AddNode(node);
            Directory.Exists("./root/xxx").Should().BeTrue();
            node.标题 = "update标题";
            node.内容 = new MsDoc{Content = "update"}; 
            nodeManager.UpdateNode(node);
            Directory.Exists("./root/xxx").Should().BeTrue();
            var fs = new FileStream("./root/xxx/内容.xml", FileMode.Open, FileAccess.Read);
            var sr = new StreamReader(fs);
            var s = sr.ReadToEnd();
            s.Should().Be("update");
            fs.Close(); sr.Close();
            nodeManager.DeleteNode(node);
        }

        [Fact]
        public void Given_id_exist_should_delete_node_ok()
        {
            var nodeManager = new NodeManager("./root");
            dynamic node = new Node("mmm", _nodeMetadata);
            node.标题 = "标题";
            node.内容 = new MsDoc { Content = "内容" };
            nodeManager.AddNode(node);
            Directory.Exists("./root/mmm").Should().BeTrue();
            nodeManager.DeleteNode(node);
            Directory.Exists("./root/mmm").Should().BeFalse();
        }
    }
}