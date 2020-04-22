using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Protocols
{
    public class ProtocolRoleModel
    {
        #region [ Constructor ]
        public ProtocolRoleModel()
        {

        }
        #endregion

        #region [ Properties ]
        public ProtocolRoleType? Type { get; set; }
        public ProtocolRoleDistributionType? DistributionType { get; set; }
        public ProtocolRoleNoteType? NoteType { get; set; }
        public ProtocolRoleStatus Status { get; set; }
        public string Note { get; set; }
        public RoleModel Role { get; set; }
        #endregion
    }
}
