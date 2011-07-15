using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Ycyj.Client.Model
{
    public class Xml2NodesMetadataConverter : IXml2NodesMetadataConverter
    {
        #region IXml2NodesMetadataConverter Members

        public NodeMetadata Extract(string path)
        {
            XDocument xDoc = XDocument.Load(path);
            return xDoc.Root != null
            ? GetNodeMetadata(xDoc.Root)
            : null;
        }

        #endregion

        private static NodeMetadata GetNodeMetadata(XElement metadataElem)
        {
            return new NodeMetadata(GetTypeName(metadataElem), GetProperties(metadataElem));
        }

        private static IEnumerable<NodePropertyMetadata> GetProperties(XElement metadataElem)
        {
            return from propertyElem in metadataElem.Elements()
                   where propertyElem.Name == "NodeProperty"
                   let propNameAttribute = propertyElem.Attribute(XName.Get("Name"))
                   let propTypeAttribute = propertyElem.Attribute(XName.Get("Type"))
                   where propNameAttribute != null && propTypeAttribute != null
                   let propName = propNameAttribute.Value
                   let propType = GetPropertyType(propTypeAttribute)
                   select new NodePropertyMetadata(propName, propType);
        }

        private static Type GetPropertyType(XAttribute propTypeAttribute)
        {
            // TODO: Move this config to an xml file or sth
            Type propType;
            switch (propTypeAttribute.Value)
            {
                case "msdoc":
                    propType = typeof(MsDoc);
                    break;
                case "int":
                    propType = typeof(int);
                    break;
                case "string":
                    propType = typeof(string);
                    break;
                case "decimal":
                    propType = typeof(decimal);
                    break;
                default:
                    propType = typeof(Nullable);
                    break;
            }
            return propType;
        }

        private static string GetTypeName(XElement metadataElem)
        {
            XAttribute nameAttribute = metadataElem.Attribute(XName.Get("Name"));
            return nameAttribute != null
            ? nameAttribute.Value
            : string.Empty;
        }

        /*
        public IEnumerable<NodeMetadata> Extract()
        {
            throw new NotImplementedException();
        }
         * */
    }
}
