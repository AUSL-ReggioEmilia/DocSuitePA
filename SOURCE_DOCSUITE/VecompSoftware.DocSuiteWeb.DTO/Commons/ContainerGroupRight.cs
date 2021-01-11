using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.DTO.Commons
{
    [Serializable]
    public class ContainerGroupRight
    {
        public ContainerGroupRight()
        {
            this.Users= new List<ContainerUserRight>();
        }

        public ContainerGroupRight(int IdContainer)
             : this()
        {
            this.IdContainer = IdContainer;
        }

        public int IdContainer { get; set; }

        public string GroupName { get; set; }

        public String Location { get; set; }
        public String Authorization { get; set; }

        public IList<ContainerUserRight> Users { get; set; }

       
    }
}
