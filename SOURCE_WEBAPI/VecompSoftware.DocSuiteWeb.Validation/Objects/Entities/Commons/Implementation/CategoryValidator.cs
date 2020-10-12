using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons
{
    public class CategoryValidator : ObjectValidator<Category, CategoryValidator>, ICategoryValidator
    {
        #region [ Constructor ]
        public CategoryValidator(ILogger logger, ICategoryValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        { }

        #endregion

        #region[ Properties ]

        public short EntityShortId { get; set; }
        public string Name { get; set; }
        public ActiveType? IsActive { get; set; }
        public short? Code { get; set; }
        public string FullIncrementalPath { get; set; }
        public string FullCode { get; set; }
        public string FullSearchComputed { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public Guid UniqueId { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public Category Parent { get; set; }
        public CategorySchema CategorySchema { get; set; }
        public MetadataRepository MetadataRepository { get; set; }
        public TenantAOO TenantAOO { get; set; }
        public ICollection<Category> Categories { get; set; }
        public ICollection<Protocol> Protocols { get; set; }
        public ICollection<DocumentSeriesItem> DocumentSeriesItems { get; set; }
        public ICollection<Fascicle> Fascicles { get; set; }
        public ICollection<CategoryFascicle> CategoryFascicles { get; set; }
        public ICollection<DocumentUnit> DocumentUnits { get; set; }
        public ICollection<DocumentUnitFascicleHistoricizedCategory> DocumentUnitFascicleHistoricizedCategories { get; set; }
        public ICollection<DocumentUnitFascicleCategory> DocumentUnitFascicleCategories { get; set; }
        public ICollection<DossierFolder> DossierFolders { get; set; }
        public ICollection<FascicleFolder> FascicleFolders { get; set; }
        public ICollection<Process> Processes { get; set; }
        #endregion
    }
}