using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Ycyj.Client.Model
{
    public class KnowledgeTreeManager : IKnowledgeTreeManager
    {
        private readonly INodeManager _nodeManager;

        public KnowledgeTreeManager(string xmlFilePath, INodeManager nodeManager)
        {
            XmlFilePath = xmlFilePath;
            _nodeManager = nodeManager;

            Root = new TreeNode(null);
            try
            {
                ReloadTree();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);

                // Simply reset the Root object if load failed.
                ResetTree();
            }
        }

        public string XmlFilePath { get; private set; }

        #region IKnowledgeTreeManager Members

        public TreeNode Root { get; private set; }

        public void UpdateTree()
        {
            XmlWriter writer = XmlWriter.Create(XmlFilePath,
                                                new XmlWriterSettings
                                                    {
                                                        OmitXmlDeclaration = false,
                                                        Indent = true
                                                    });
            writer.WriteStartDocument();
            writer.WriteStartElement("Root");
            foreach (TreeNode child in Root.Children)
                WriteTreeNode(child, writer);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }

        public void ReloadTree()
        {
            try
            {
                var document = new XmlDocument();
                document.Load(XmlFilePath);

                foreach (TreeNode child in Root.Children)
                    child.DetachFromParent();

                ExtractChildren(document.DocumentElement, Root);
            }
            catch (Exception e)
            {
                throw new KnowledgeTreeLoadFailException(
                    "Failed to load knowledge tree from " + Path.GetFullPath(XmlFilePath), e);
            }
        }

        public void ResetTree()
        {
            Root = new TreeNode(null);
        }

        #endregion

        private void ExtractChildren(XmlNode parentXml, TreeNode parent)
        {
            IEnumerable<Tuple<XmlNode, TreeNode>> children = from childXml in parentXml.ChildNodes.OfType<XmlNode>()
                                                             where
                                                                 childXml.Name == "Node" &&
                                                                 childXml.Attributes["Id"] != null
                                                             let childNode =
                                                                 _nodeManager.GetNodeById(
                                                                     childXml.Attributes["Id"].Value)
                                                             let childTreeNode = parent.AddChild(childNode)
                                                             select Tuple.Create(childXml, childTreeNode);
            Parallel.ForEach(children, child => ExtractChildren(child.Item1, child.Item2));
        }

        private static void WriteTreeNode(TreeNode node, XmlWriter writer)
        {
            writer.WriteStartElement("Node");
            writer.WriteAttributeString("Id", node.Id);
            foreach (TreeNode child in node.Children)
                WriteTreeNode(child, writer);
            writer.WriteEndElement();
        }
    }
}