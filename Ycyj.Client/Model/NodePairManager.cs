using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Ycyj.Client.Model
{
    public class NodePairManager : INodePairManager
    {
        public string XmlFilePath { get; private set; }
        public NodePairManager(string xmlFilePath)
        {
            XmlFilePath = xmlFilePath;
        }

        public IEnumerable<Node> GetPairedNodesOf(Node node)
        {
            var pairedNodeList = new List<Node>();
            var document = new XmlDocument();
            if (!File.Exists(XmlFilePath)) return null;
            document.Load(XmlFilePath);
            var xmlNode = document.DocumentElement;
            try
            {
                var xmlNodes = xmlNode.ChildNodes;
                var nodeManager = new NodeManager();
                for (var i = 0; i < xmlNodes.Count; i++)
                {
                    var currentXmlNode = xmlNodes[i];
                    var firstXmlNode = currentXmlNode.ChildNodes[0];
                    var secondXmlNode = currentXmlNode.ChildNodes[1];
                    if (firstXmlNode.Attributes["Id"].Value.Equals(node.Id))
                    {
                        pairedNodeList.Add(nodeManager.GetNodeById(secondXmlNode.Attributes["Id"].Value));
                    }
                    else if (secondXmlNode.Attributes["Id"].Value.Equals(node.Id))
                    {
                        pairedNodeList.Add(nodeManager.GetNodeById(firstXmlNode.Attributes["Id"].Value));
                    }
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
            return pairedNodeList;
        }

        public void PairNodes(Node node1, Node node2)
        {
            try
            {
                XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
                xmlReaderSettings.IgnoreWhitespace = true;
                xmlReaderSettings.IgnoreComments = true;

                if (!File.Exists(XmlFilePath)) return;
                var document = new XmlDocument();
                document.Load(XmlFilePath);

                var xmlElementList = document.GetElementsByTagName("NodePairs");
                var newXmlElement = document.CreateElement("NodePair");
                var firstNode = document.CreateElement("Node");
                var secondNode = document.CreateElement("Node");
                firstNode.SetAttribute("Id", node1.Id);
                secondNode.SetAttribute("Id", node2.Id);
                newXmlElement.AppendChild(firstNode);
                newXmlElement.AppendChild(secondNode);
                xmlElementList[0].AppendChild(newXmlElement);
                document.Save(XmlFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void UnpairNodes(Node node1, Node node2)
        {
            try
            {
                var xmlReaderSettings = new XmlReaderSettings();
                xmlReaderSettings.IgnoreWhitespace = true;
                xmlReaderSettings.IgnoreComments = true;
                if (!File.Exists(XmlFilePath)) return;
                var document = new XmlDocument();
                document.Load(XmlFilePath);

                var xmlElement = document.GetElementsByTagName("NodePairs");
                var nodePairElementList = document.GetElementsByTagName("NodePair");
                foreach (XmlElement nodePairElement in nodePairElementList)
                {
                    if (nodePairElement.ChildNodes[0].Attributes["Id"].Value.Equals(node1.Id))
                    {
                        if (nodePairElement.ChildNodes[1].Attributes["Id"].Value.Equals(node2.Id))
                        {
                            xmlElement[0].RemoveChild(nodePairElement);
                            break;
                        }
                    }
                    if (nodePairElement.ChildNodes[0].Attributes["Id"].Value.Equals(node2.Id))
                    {
                        if (nodePairElement.ChildNodes[1].Attributes["Id"].Value.Equals(node1.Id))
                        {
                            xmlElement[0].RemoveChild(nodePairElement);
                            break;
                        }
                    }
                }
                document.Save(XmlFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
