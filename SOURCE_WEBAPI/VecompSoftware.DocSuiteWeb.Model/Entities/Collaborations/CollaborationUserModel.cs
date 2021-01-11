using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations
{
    public class CollaborationUserModel
    {
        public CollaborationUserModel() { }

        public string Account { get; set; }

        public short? IdRole { get; set; }

        public Guid? IdCollaborationUser { get; set; }

        public short? Incremental { get; set; }

        public bool? DestinationFirst { get; set; }

        public string DestinationName { get; set; }

        public string DestinationEmail { get; set; }

        public string DestinationType { get; set; }
    }
}
