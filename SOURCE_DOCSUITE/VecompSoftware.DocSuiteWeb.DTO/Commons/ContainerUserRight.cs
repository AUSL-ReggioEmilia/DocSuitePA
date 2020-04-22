using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.DTO.Commons
{
    [Serializable]
    public class ContainerUserRight
    {
        public ContainerUserRight()
        {

        }

        public string Name { get; set; }

        public string GroupName { get; set; }
    }
}
