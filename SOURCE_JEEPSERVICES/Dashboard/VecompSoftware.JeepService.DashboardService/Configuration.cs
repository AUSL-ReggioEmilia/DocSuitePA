using System.Collections.Generic;
using System.Xml.Serialization;

namespace JeepService.JeepService.DashboardService
{
    [XmlRoot(ElementName = "CONFIGURATION")]
    public class Configuration
    {
        [XmlAttribute("JeepConfig")]
        public bool JeepConfig { get; set; }

        [XmlAttribute("Log4Net")]
        public bool Log4Net { get; set; }

        [XmlAttribute("ApplicationConfig")]
        public bool ApplicationConfig { get; set; }

        [XmlArray("MODULES"), XmlArrayItem("MODULE")]
        public List<Module> Modules { get; set; }
    }
    [XmlRoot(ElementName = "MODULE", Namespace = "")]
    public class Module
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("Source")]
        public string Source { get; set; }
    }
}
