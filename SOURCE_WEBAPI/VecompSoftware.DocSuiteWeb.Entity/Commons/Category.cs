using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;

namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{
    public class Category : DSWBaseEntity
    {
        #region [ Constructor ]

        public Category() : this(Guid.NewGuid()) { }
        public Category(Guid uniqueId)
            : base(uniqueId)
        {
            Categories = new HashSet<Category>();
            Protocols = new HashSet<Protocol>();
            Resolutions = new HashSet<Resolution>();
            DocumentSeriesItems = new HashSet<DocumentSeriesItem>();
            Fascicles = new HashSet<Fascicle>();
            CategoryFascicles = new HashSet<CategoryFascicle>();
            DocumentUnits = new HashSet<DocumentUnit>();
            DocumentUnitFascicleHistoricizedCategories = new HashSet<DocumentUnitFascicleHistoricizedCategory>();
            DocumentUnitFascicleCategories = new HashSet<DocumentUnitFascicleCategory>();
            DossierFolders = new HashSet<DossierFolder>();
            FascicleFolders = new HashSet<FascicleFolder>();
            Processes = new HashSet<Process>();
            Categories = new HashSet<Category>();
        }
        #endregion

        #region[ Properties ]

        public string Name { get; set; }
        public bool IsActive { get; set; }
        public short? Code { get; set; }
        public string FullIncrementalPath { get; set; }
        public string FullCode { get; set; }
        public string FullSearchComputed { get; private set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual Category Parent { get; set; }
        public virtual CategorySchema CategorySchema { get; set; }
        public virtual MassimarioScarto MassimarioScarto { get; set; }
        public virtual MetadataRepository MetadataRepository { get; set; }
        public virtual TenantAOO TenantAOO { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Protocol> Protocols { get; set; }
        public virtual ICollection<Resolution> Resolutions { get; set; }
        public virtual ICollection<DocumentSeriesItem> DocumentSeriesItems { get; set; }
        public virtual ICollection<Fascicle> Fascicles { get; set; }
        public virtual ICollection<CategoryFascicle> CategoryFascicles { get; set; }
        public virtual ICollection<DocumentUnit> DocumentUnits { get; set; }
        public virtual ICollection<DocumentUnitFascicleHistoricizedCategory> DocumentUnitFascicleHistoricizedCategories { get; set; }
        public virtual ICollection<DocumentUnitFascicleCategory> DocumentUnitFascicleCategories { get; set; }
        public virtual ICollection<DossierFolder> DossierFolders { get; set; }
        public virtual ICollection<FascicleFolder> FascicleFolders { get; set; }
        public virtual ICollection<Process> Processes { get; set; }
        public virtual ICollection<Dossier> Dossiers { get; set; }

        #endregion
    }
}
