using System.Xml.Serialization;

using Comindware.VSDX2BPMN.Сonstants;

namespace Comindware.VSDX2BPMN.Models
{
    public class Plane
    {
        public Plane()
        {
            Shapes = new List<Shape>();
        }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "bpmnElement")]
        public string BpmnElement { get; set; }

        [XmlElement(ElementName = "BPMNShape", Namespace = Namespaces.Bpmndi)]
        public List<Shape> Shapes { get; set; }
    }
}
