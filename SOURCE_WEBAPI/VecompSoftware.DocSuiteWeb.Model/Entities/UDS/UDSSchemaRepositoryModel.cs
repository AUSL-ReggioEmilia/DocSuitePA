using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.UDS
{
    public class UDSSchemaRepositoryModel
    {
        #region [ Constructor ]

        #endregion

        #region [ Properties ]

        public DateTimeOffset? ActiveDate { get; set; }

        public DateTimeOffset? ExpiredDate { get; set; }

        public Guid Id { get; set; }

        public string SchemaXML { get; set; }

        public short? Version { get; set; }

        #endregion

        #region [ Methods ]

        public bool HasId()
        {
            return !Id.Equals(Guid.Empty);
        }

        #endregion
    }
}
