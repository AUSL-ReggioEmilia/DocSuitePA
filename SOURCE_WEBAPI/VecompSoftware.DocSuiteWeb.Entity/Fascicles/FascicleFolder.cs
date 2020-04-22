using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Entity.Fascicles
{
    public class FascicleFolder : DSWBaseEntity
    {
        #region [ Constructor ]
        public FascicleFolder() : this(Guid.NewGuid()) { }

        public FascicleFolder(Guid uniqueId)
            : base(uniqueId)
        {

            FascicleDocuments = new HashSet<FascicleDocument>();
            FascicleDocumentUnits = new HashSet<FascicleDocumentUnit>();
        }
        #endregion

        #region [ Properties ]

        public string Name { get; set; }

        public FascicleFolderStatus Status { get; set; }

        public FascicleFolderTypology Typology { get; set; }

        public string FascicleFolderPath { get; set; }

        public short FascicleFolderLevel { get; set; }

        /// <summary>
        /// Proprietà fake per passare il parent id in fase di inserimento
        /// </summary>
        public Guid? ParentInsertId { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual Category Category { get; set; }

        public virtual Fascicle Fascicle { get; set; }

        public virtual ICollection<FascicleDocumentUnit> FascicleDocumentUnits { get; set; }

        public virtual ICollection<FascicleDocument> FascicleDocuments { get; set; }
        #endregion
    }
}
