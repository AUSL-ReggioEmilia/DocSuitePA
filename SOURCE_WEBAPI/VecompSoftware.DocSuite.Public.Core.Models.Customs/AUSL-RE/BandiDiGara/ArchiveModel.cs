using System;
using System.Collections.Generic;
using VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters;

namespace VecompSoftware.DocSuite.Public.Core.Models.Customs.AUSL_RE.BandiDiGara
{
    public class ArchiveModel
    {
        public ArchiveModel()
        {
            UniqueId = Guid.NewGuid();
        }

        public Guid UniqueId { get; set; }
        public string ArchiveName { get; set; }
        public string Subject { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public ICollection<MetadataModel> Metadatas { get; set; }
        public ICollection<Domains.Commons.DocumentModel> Documents { get; set; }
    }
}
