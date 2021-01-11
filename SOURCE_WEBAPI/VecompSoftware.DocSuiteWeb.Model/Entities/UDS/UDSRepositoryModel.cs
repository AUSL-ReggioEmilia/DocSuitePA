using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.UDS
{
    public class UDSRepositoryModel
    {
        #region [ Constructor ]
        public UDSRepositoryModel()
        {

        }

        public UDSRepositoryModel(Guid idRespository)
        {
            Id = idRespository;
        }
        #endregion

        #region [ Properties ]

        public DateTimeOffset? ActiveDate { get; set; }

        public DateTimeOffset? ExpiredDate { get; set; }

        public short SequenceCurrentYear { get; set; }

        public int SequenceCurrentNumber { get; set; }

        public Guid Id { get; set; }

        public string ModuleXML { get; set; }

        public string Name { get; set; }

        public UDSRepositoryStatus? Status { get; set; }

        public UDSSchemaRepositoryModel SchemaRepository { get; set; }

        public short? Version { get; set; }

        public int DSWEnvironment { get; set; }

        public string Alias { get; set; }

        #endregion

        #region [ Methods ]

        public bool HasId()
        {
            return !Id.Equals(Guid.Empty);
        }

        #endregion
    }
}
