using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLPC.COVID19.Models
{
    public class DocumentUnitModel
    {
        public DocumentUnitModel(Guid uniqueId)
        {
            UniqueId = uniqueId;
            DocumentUnitChains = new List<DocumentModel>();
        }

        public Guid UniqueId { get; private set; }
        public string DocumentUnitName { get; set; }
        public short Year { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string RegistrationUser { get; set; }
        public string Subject { get; set; }
        public int Environment { get; set; }
        public string MainDocumentName { get; set; }
        public Guid? IdUDSRepository { get; set; }
        public ICollection<DocumentModel> DocumentUnitChains { get; set; }
    }
}
