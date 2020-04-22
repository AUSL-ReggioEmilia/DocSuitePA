using System;
using System.Collections.Generic;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Archives.Commons;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons;

namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.Fascicles
{
    public class FascicleFolderModel : DomainModel
    {
        #region [ Constructor ]
        public FascicleFolderModel(Guid id)
            : base(id)
        {

        }
        #endregion

        #region [ Properties ]
        public FascicleFolderStatus Status { get; set; }
        public FascicleFolderTypology Typology { get; set; }
        public string FascicleFolderPath { get; set; }
        public short FascicleFolderLevel { get; set; }
        public Guid? ParentInsertId { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public FascicleModel Fascicle { get; set; }
        public ICollection<GenericDocumentUnitModel> FascicleDocumentUnits { get; set; }
        public ICollection<DocumentModel> FascicleDocuments { get; set; }
        #endregion
    }
}
