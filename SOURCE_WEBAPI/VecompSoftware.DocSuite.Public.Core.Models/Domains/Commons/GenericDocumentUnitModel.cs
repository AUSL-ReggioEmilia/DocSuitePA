using System;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons;

namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.Archives.Commons
{
    [DocumentUnit(DocumentUnitType.Generic)]
    public class GenericDocumentUnitModel : DocumentUnitModel
    {
        #region [ Contructors ]
        public GenericDocumentUnitModel(Guid id, short year, int number, string subject,
            CategoryModel category, ContainerModel container, string location)
            : base(id, year, number, subject, category, container, location)
        {
        }
        public int Environment { get; set; }
        #endregion
    }
}
