using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ycyj.Client.Model;
using Xunit;
using FluentAssertions;

namespace Ycyj.Client.Test
{
    public class Xml2NodesMetadataConverterTest
    {
        private const string NonExistentFileName = "some_non-existent_file.xml";

        private const string CorruptedFileName = "corrupted_tree.xml";
        private const string CorrptedFileContent = "This is not a valid xml.";

        private const string CorrectFileName = "metadata.xml";
        private const string CorrectFileContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<NodeMetadata Name=""题目"">
    <NodeProperty Name=""题面"" Type=""msdoc"" />
    <NodeProperty Name=""分析"" Type=""msdoc"" />
    <NodeProperty Name=""答案"" Type=""msdoc"" />
    <NodeProperty Name=""注释"" Type=""string"" />
    <NodeProperty Name=""难度"" Type=""int"" />
</NodeMetadata>";

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
        public Xml2NodesMetadataConverterTest()
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
            File.WriteAllText(CorrectFileName, CorrectFileContent);
        }

        [Fact]
        public void Test1()
        {
            var xml2Converter = new Xml2NodesMetadataConverter();
            var nodeMetadata = xml2Converter.Extract(CorrectFileName);
            nodeMetadata.Name.Should().Be("题目");
        }
    }
}
