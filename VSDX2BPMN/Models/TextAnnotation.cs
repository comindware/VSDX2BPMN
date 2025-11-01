using System.Xml.Serialization;

namespace Comindware.VSDX2BPMN.Models
{
    public class TextAnnotation
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "text")]
        public string Text { get; set; }
    }
}
