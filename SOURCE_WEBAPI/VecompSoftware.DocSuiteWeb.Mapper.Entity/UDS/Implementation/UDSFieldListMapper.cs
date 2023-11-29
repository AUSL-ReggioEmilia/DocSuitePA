using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.UDS;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.UDS
{
    public class UDSFieldListMapper : BaseEntityMapper<UDSFieldList, UDSFieldList>, IUDSFieldListMapper
    {
        public override UDSFieldList Map(UDSFieldList entity, UDSFieldList entityTransformed)
        {
            #region [ Base ]
            entityTransformed.FieldName = entity.FieldName;
            entityTransformed.Name = entity.Name;
            entityTransformed.Repository = entity.Repository;
            entityTransformed.Environment = entity.Environment;
            #endregion

            return entityTransformed;
        }

    }
}
