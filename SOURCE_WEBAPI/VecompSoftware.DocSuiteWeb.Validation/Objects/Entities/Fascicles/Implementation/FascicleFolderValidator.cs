using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles
{
    public class FascicleFolderValidator : ObjectValidator<FascicleFolder, FascicleFolderValidator>, IFascicleFolderValidator
    {
        #region [ Constructor ]

        public FascicleFolderValidator(ILogger logger, IFascicleFolderValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        {
            ICollection<FascicleDocumentUnit> FascicleDocumentUnits = new Collection<FascicleDocumentUnit>();
            ICollection<FascicleDocument> FascicleDocuments = new Collection<FascicleDocument>();
        }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public string Name { get; set; }

        public FascicleFolderStatus Status { get; set; }

        public FascicleFolderTypology Typology { get; set; }

        public byte[] Timestamp { get; set; }

        public string FascicleFolderPath { get; set; }

        public short FascicleFolderLevel { get; set; }

        public Guid? ParentInsertId { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public Fascicle Fascicle { get; set; }
        public Category Category { get; set; }
        public ICollection<FascicleDocumentUnit> FascicleDocumentUnits { get; set; }

        public ICollection<FascicleDocument> FascicleDocuments { get; set; }
        #endregion
    }
}

