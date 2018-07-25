using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace FclEx
{
    public static class XmlExtensions
    {
        public static XElement RemoveComment(this XElement xml)
        {
            xml.DescendantNodes().OfType<XComment>().Remove();
            return xml;
        }

        public static string ToJson(this XNode xml, 
            Formatting formatting = Formatting.None, 
            bool omitRootObject = false)
        {
            return JsonConvert.SerializeXNode(xml);
        }

        public static string ToJson(this XmlNode xml,
            Formatting formatting = Formatting.None,
            bool omitRootObject = false)
        {
            return JsonConvert.SerializeXmlNode(xml);
        }


    }
}
