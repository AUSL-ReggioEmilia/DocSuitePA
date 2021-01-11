
namespace VecompSoftware.DocSuiteWeb.Model.Parameters
{
    public class CollaborationManagerModel
    {
        public string Account { get; set; }

        public string Type { get; set; }

        public short RoleId { get; set; }

        public string SignUser { get; set; }

        public string SignCertificate { get; set; }

        public bool IsAbsenceManaged { get; set; }
    }
}
