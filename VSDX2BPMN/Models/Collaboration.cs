using System.Xml.Serialization;

namespace Comindware.VSDX2BPMN.Models
{
    public class Collaboration
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "participant")]
        public Participant Participant { get; set; }
    }
}
