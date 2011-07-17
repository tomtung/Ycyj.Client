using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Ycyj.Client.Model
{
    public class NodeManager : INodeManager
    {
        private readonly string _rootPath;

        public NodeManager(string rootPath)
        {
            _rootPath = rootPath;
        }

        public Node GetNodeById(string id)
        {
            var nodePath = Path.Combine(_rootPath, id);
            if (!Directory.Exists(nodePath))
                return null;
            var metaDataXmlFile = Path.Combine(nodePath, "metadata.xml");

            var xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.IgnoreWhitespace = true;
            xmlReaderSettings.IgnoreComments = true;

            // read metadata.xml

            var xml2NodesMetadataConverter = new Xml2NodesMetadataConverter();
            var nodeMetadata = xml2NodesMetadataConverter.Extract(metaDataXmlFile);
            dynamic node = new Node(id, nodeMetadata);


            //read data.xml
            var dataXmlFile = Path.Combine(nodePath, "data.xml");
            using (XmlReader xmlReader = XmlReader.Create(dataXmlFile, xmlReaderSettings))
            {
                bool visitNode = false;
                while (xmlReader.Read())
                {
                    if (!xmlReader.Name.Equals("Node") && visitNode)
                    {
                        if (xmlReader.HasAttributes)
                        {
                            var msdocXml = xmlReader["File"];
                            var fs = new FileStream(Path.Combine(nodePath, msdocXml), FileMode.Open, FileAccess.Read);
                            var sr = new StreamReader(fs, Encoding.UTF8);
                            var msdoc = new MsDoc();
                            msdoc.Content = sr.ReadToEnd();
                            node[xmlReader.Name]= msdoc;
                            sr.Close();
                            fs.Close();
                        }
                        else
                        {
                            Type type = NodeMetadataHelper.GetPropertyMetadata(node.Metadata, xmlReader.Name).Type;
                            if (type.Equals(typeof(int)))
                            {
                                node[xmlReader.Name] = Convert.ToInt32(xmlReader.ReadString());
                            }
                            else if (type.Equals(typeof(decimal)))
                            {
                                node[xmlReader.Name]= Convert.ToDecimal(xmlReader.ReadString());
                            }
                            else if (type.Equals(typeof(string)))
                            {
                                node[xmlReader.Name]= xmlReader.ReadString();
                            }
                        }
                    }
                    else if (xmlReader.Name.Equals("Node")) visitNode = true;
                }
            }

            return node;
        }

        public void AddNode(Node node)
        {
            var nodePath = Path.Combine(_rootPath, node.Id);
            Directory.CreateDirectory(nodePath);
            addOrUpdateNode(node, nodePath);
        }

        public void UpdateNode(Node node)
        {
            var nodePath = Path.Combine(_rootPath, node.Id);
            addOrUpdateNode(node, nodePath);
        }

        public void DeleteNode(Node node)
        {
            var nodePath = Path.Combine(_rootPath, node.Id);
            string[] fileList = Directory.GetFiles(nodePath);
            //foreach (var file in fileList)
            //    File.Delete(file);
            Directory.Delete(nodePath, true);
        }

        private void addOrUpdateNode(Node node, string nodePath)
        {
            var xmlWriterSettings = new XmlWriterSettings {OmitXmlDeclaration = false, Indent = true};

            const string metaDataXml = "metadata.xml";
            const string dataXml = "data.xml";
            var metaDataXmlFile = Path.Combine(nodePath, metaDataXml);
            var dataXmlFile = Path.Combine(nodePath, dataXml);

            var nodePropertyMetadatas = node.Metadata.Properties;
            var nodeProperties = node.Properties;

            //write metadata.xml

            var xw = XmlWriter.Create(metaDataXmlFile, xmlWriterSettings);
            xw.WriteStartDocument();
            xw.WriteStartElement("NodeMetadata");
            xw.WriteAttributeString("Name", node.Metadata.Name);

            foreach (var npm in nodePropertyMetadatas)
            {
                xw.WriteStartElement("NodeProperty");
                xw.WriteAttributeString("Name", npm.Name);
                if (npm.Type.Equals(typeof(int)))
                {
                    xw.WriteAttributeString("Type", "int");
                }
                else if (npm.Type.Equals(typeof(decimal)))
                {
                    xw.WriteAttributeString("Type", "decimal");
                }
                else if (npm.Type.Equals(typeof(string)))
                {
                    xw.WriteAttributeString("Type", "string");
                }
                else if(npm.Type.Equals(typeof(MsDoc)))
                {
                    xw.WriteAttributeString("Type", "msdoc");
                }
                xw.WriteEndElement();
            }
            xw.WriteEndElement();
            xw.WriteEndDocument();
            xw.Close();


            //write data.xml

            xw = XmlWriter.Create(dataXmlFile, xmlWriterSettings);
            xw.WriteStartDocument();
            xw.WriteStartElement("Node");

            foreach (var nodeProperty in nodeProperties)
            {
                xw.WriteStartElement(nodeProperty.PropertyName);

                if (nodeProperty.PropertyType == typeof(MsDoc))
                {
                    xw.WriteAttributeString("File", nodeProperty.PropertyName + ".xml");
                }
                else
                {
                    //xw.WriteStartElement(nodeProperty.PropertyName);
                    xw.WriteString(nodeProperty.Value.ToString());
                    //xw.WriteEndElement();
                }
                xw.WriteEndElement();
            }

            xw.WriteEndElement();
            xw.WriteEndDocument();
            xw.Close();



            //write xml files of msdoc
            foreach (var npm in nodeProperties.Where(p => p.PropertyType == typeof(MsDoc)))
            {
                var msDocXmlFile = Path.Combine(nodePath, npm.PropertyName + ".xml");
                using (var fs = new FileStream(msDocXmlFile, FileMode.Create, FileAccess.ReadWrite))
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                    if (npm.Value != null) sw.Write(((MsDoc) (npm.Value)).Content);
            }
        }
    }
}
