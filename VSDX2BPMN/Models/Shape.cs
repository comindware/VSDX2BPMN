using System.Xml.Serialization;

using Comindware.VSDX2BPMN.Сonstants;

namespace Comindware.VSDX2BPMN.Models
{
    public class Shape
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "bpmnElement")]
        public string BpmnElement { get; set; }

        [XmlElement(ElementName = "Bounds", Namespace = Namespaces.Dc)]
        public Bounds Bounds { get; set; }
    }
}
