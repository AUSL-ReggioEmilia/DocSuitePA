using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.DTO.Commons
{
    [Serializable]
    public class CategoryUserRight
    {
        public CategoryUserRight()
        {

        }

        public string Name { get; set; }

        public bool InsertFascicle { get; set; }

        public bool ModifyFascicle { get; set; }

        public bool ManageFascicle { get; set; }

        public bool ViewFascicle { get; set; }

        public bool DeleteFascicle { get; set; }
    }
}
