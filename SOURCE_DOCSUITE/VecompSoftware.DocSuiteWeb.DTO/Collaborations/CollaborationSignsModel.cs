using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.DTO.Collaborations
{
    [Serializable]
    public class CollaborationSignModel
    {
        public CollaborationSignModel()
        {
            CollaborationVersioningModels = new List<CollaborationVersioningModel>();
        }
        public string SignUser
        {
            get; set;
        }
        public string SignName
        {
            get; set;
        }
        public string SignEmail
        {
            get; set;
        }
        public DateTimeOffset? SignDate
        {
            get; set;
        }
        public Boolean? IsRequired
        {
            get; set;
        }
        public Int16 Incremental
        {
            get; set;
        }
        public List<CollaborationVersioningModel> CollaborationVersioningModels
        {
            get; set;
        }
    }
}
