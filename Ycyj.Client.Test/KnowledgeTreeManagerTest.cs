using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Moq;
using Xunit;
using Ycyj.Client.Model;

namespace Ycyj.Client.Test
{
    public class KnowledgeTreeManagerTest : IDisposable
    {
        private const string NonExistentFileName = "some_non-existent_file.xml";

        private const string CorruptedFileName = "corrupted_tree.xml";
        private const string CorrptedFileContent = "This is not a valid xml.";

        private const string CorrectFileName = "correct_tree.xml";

        private const string CorrectFileContent =
            @"<?xml version=""1.0"" encoding=""utf-8""?>

<Root>
  <Node Id=""1"">
    <Node Id=""1.1"" />
    <Node Id=""1.2"" />
  </Node>
  <Node Id=""2"">
    <Node Id=""2.1"">
      <Node Id=""2.1.1"" />
      <Node Id=""2.1.2"">
        <Node Id=""2.1.2.1"" />
      </Node>
    </Node>
  </Node>
</Root>";

        private readonly Mock<INodeManager> _mockNodeManager = new Mock<INodeManager>();

        private readonly NodeMetadata _nodeMetadata = new NodeMetadata("知识点", new[]
                                                                                  {
                                                                                      new NodePropertyMetadata("标题",
                                                                                                               typeof (
                                                                                                                   string
                                                                                                                   )),
                                                                                      new NodePropertyMetadata("内容",
                                                                                                               typeof (
                                                                                                                   MsDoc
                                                                                                                   ))
                                                                                  });

        public KnowledgeTreeManagerTest()
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
        public void Given_file_not_exist_when_should_tree_be_empty()
        {
            var treeManager = new KnowledgeTreeManager(NonExistentFileName, _mockNodeManager.Object);

            treeManager.Root.Children.Should().BeEmpty();
        }

        [Fact]
        public void Given_file_corrupted_when_should_tree_be_empty()
        {
            var treeManager = new KnowledgeTreeManager(CorruptedFileName, _mockNodeManager.Object);

            treeManager.Root.Children.Should().BeEmpty();
        }

        [Fact]
        public void Given_file_not_exist_when_reload_should_throw_exception()
        {
            if (File.Exists(NonExistentFileName)) File.Delete(NonExistentFileName);

            var treeManager = new KnowledgeTreeManager(NonExistentFileName, _mockNodeManager.Object);

            new Action(treeManager.ReloadTree).ShouldThrow<KnowledgeTreeLoadFailException>();
        }

        [Fact]
        public void Given_file_corrupted_when_reload_should_throw_exception()
        {
            var treeManager = new KnowledgeTreeManager(CorruptedFileName, _mockNodeManager.Object);

            new Action(treeManager.ReloadTree).ShouldThrow<KnowledgeTreeLoadFailException>();
        }

        [Fact]
        public void When_reset_should_root_be_reset_and_tree_be_empty()
        {
            var treeManager = new KnowledgeTreeManager(CorrectFileName, _mockNodeManager.Object);
            TreeNode oldRoot = treeManager.Root;

            treeManager.ResetTree();

            treeManager.Root.Should().NotBeSameAs(oldRoot);
            treeManager.Root.Children.Should().BeEmpty();
        }

        [Fact]
        public void Given_file_is_valid_when_load_should_tree_be_loaded()
        {
            var treeManager = new KnowledgeTreeManager(CorrectFileName, _mockNodeManager.Object);

            TreeNode[] treeNodes = treeManager.Root.Children.ToArray();
            treeNodes[0].Id.Should().Be("1");
            treeNodes[0].Children.ElementAt(0).Id.Should().Be("1.1");
            treeNodes[0].Children.ElementAt(1).Id.Should().Be("1.2");
            treeNodes[1].Id.Should().Be("2");
            treeNodes[1].Children.ElementAt(0).Id.Should().Be("2.1");
            treeNodes[1].Children.ElementAt(0).Children.ElementAt(0).Id.Should().Be("2.1.1");
            treeNodes[1].Children.ElementAt(0).Children.ElementAt(1).Id.Should().Be("2.1.2");
            treeNodes[1].Children.ElementAt(0).Children.ElementAt(1).Children.ElementAt(0).Id.Should().Be("2.1.2.1");
        }

        [Fact]
        public void When_update_should_the_tree_be_saved()
        {
            var treeManager = new KnowledgeTreeManager(CorrectFileName, _mockNodeManager.Object);

            treeManager.Root.Children.ElementAt(1).DetachFromParent();
            treeManager.Root.Children.ElementAt(0).Children.ElementAt(0).AddChild(new Node("1.1.1", _nodeMetadata));
            treeManager.UpdateTree();

            treeManager.ResetTree();
            treeManager.ReloadTree();
            treeManager.Root.Children.ElementAt(0).Id.Should().Be("1");
            treeManager.Root.Children.ElementAt(0).Children.ElementAt(0).Id.Should().Be("1.1");
            treeManager.Root.Children.ElementAt(0).Children.ElementAt(0).Children.ElementAt(0).Id.Should().Be("1.1.1");
            treeManager.Root.Children.ElementAt(0).Children.ElementAt(1).Id.Should().Be("1.2");
        }
    }
}