using System.Xml.Serialization;

namespace Comindware.VSDX2BPMN.Models
{
    [XmlRoot(ElementName = "process")]
    public class Process
    {
        public Process()
        {
            UserTasks = new List<Element>();
            InclusiveGateways = new List<Element>();
            TextAnnotations = new List<TextAnnotation>();
            LaneSet = new LaneSet();
        }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "laneSet")]
        public LaneSet LaneSet { get; set; }

        [XmlElement(ElementName = "startEvent")]
        public Element StartEvent { get; set; }

        [XmlElement("userTask")]
        public List<Element> UserTasks { get; set; }

        [XmlElement("inclusiveGateway")]
        public List<Element> InclusiveGateways { get; set; }

        [XmlElement("textAnnotation")]
        public List<TextAnnotation> TextAnnotations { get; set; }

        [XmlElement(ElementName = "endEvent")]
        public Element EndEvent { get; set; }
    }
}
