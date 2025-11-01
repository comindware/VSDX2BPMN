using System.Xml.Serialization;

namespace Comindware.VSDX2BPMN.Models
{
    public class Lane
    {
        public Lane()
        {
            FlowNodeRefs = new List<string>();
        }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "flowNodeRef")]
        public List<string> FlowNodeRefs { get; set; }
    }
}
