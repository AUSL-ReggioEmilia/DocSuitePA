using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations
{
    public class CollaborationSignModel
    {
        public CollaborationSignModel() { }

        public Guid? IdCollaborationSign { get; set; }

        public short Incremental { get; set; }

        public bool IsActive { get; set; }

        public bool? IsRequired { get; set; }

        public DateTime? SignDate { get; set; }

        public string SignUser { get; set; }

        public string SignName { get; set; }

        public string SignEmail { get; set; }

        public bool? IsAbsent { get; set; }
    }
}
