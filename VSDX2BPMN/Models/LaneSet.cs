using System.Xml.Serialization;

namespace Comindware.VSDX2BPMN.Models
{
    public class LaneSet
    {
        public LaneSet()
        {
            Lanes = new List<Lane>();
        }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "lane")]
        public List<Lane> Lanes { get; set; }
    }
}