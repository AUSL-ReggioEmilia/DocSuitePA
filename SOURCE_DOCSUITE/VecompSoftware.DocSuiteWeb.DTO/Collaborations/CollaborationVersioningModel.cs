using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.DTO.Collaborations
{
    [Serializable]
    public class  CollaborationVersioningModel
    {
        public int IdDocument
        {
            get; set;
        }
        public DateTimeOffset RegistrationDate
        {
            get; set;
        }
    }
}
