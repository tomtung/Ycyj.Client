using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ycyj.Client.Model
{
    interface IXml2NodesMetadataConverter
    {
        NodeMetadata Extract(string path);
    }
}
