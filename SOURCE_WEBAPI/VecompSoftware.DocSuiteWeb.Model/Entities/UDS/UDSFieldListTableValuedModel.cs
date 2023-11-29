using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.UDS
{
    public class UDSFieldListTableValuedModel
    {
        public Guid UniqueId { get; set; }
        public Guid IdUDSRepository { get; set; }
        public string FieldName { get; set; }
        public string Name { get; set; }
        public UDSFieldListStatus Status { get; set; }
        public int Environment { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public string UDSFieldListPath { get; set; }
        public short UDSFieldListLevel { get; set; }
        public int CountChildren { get; set; }
    }
}
