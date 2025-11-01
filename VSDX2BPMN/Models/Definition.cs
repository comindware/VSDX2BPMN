using System.Xml.Serialization;

using Comindware.VSDX2BPMN.Сonstants;

namespace Comindware.VSDX2BPMN.Models
{
    [XmlRoot(ElementName = "definitions", Namespace = Namespaces.Model)]
    public class Definition
    {
        public Definition()
        {
            Process = new Process();
        }

        [XmlElement(ElementName = "collaboration")]
        public Collaboration Collaboration { get; set; }

        [XmlElement(ElementName = "process")]
        public Process Process { get; set; }

        [XmlElement(ElementName = "BPMNDiagram", Namespace = Namespaces.Bpmndi)]
        public Diagram Diagram { get; set; }
    }
}
