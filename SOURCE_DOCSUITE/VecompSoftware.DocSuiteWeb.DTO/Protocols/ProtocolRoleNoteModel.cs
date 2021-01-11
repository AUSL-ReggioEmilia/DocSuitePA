using System;

namespace VecompSoftware.DocSuiteWeb.DTO.Protocols
{
    [Serializable]
    public class ProtocolRoleNoteModel
    {
        public int IdRole { get; set; }

        public string Note { get; set; }

        public short? Status { get; set; }

        public short? NoteType { get; set; }
    }
}
