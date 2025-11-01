using System.Xml.Serialization;

using Comindware.VSDX2BPMN.Сonstants;

namespace Comindware.VSDX2BPMN.Models
{
    public class Diagram
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "BPMNPlane", Namespace = Namespaces.Bpmndi)]
        public Plane Plane { get; set; }
    }
}
