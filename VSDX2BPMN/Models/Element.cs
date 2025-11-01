using System.Xml.Serialization;

namespace Comindware.VSDX2BPMN.Models
{
    public class Element
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}
