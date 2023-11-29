using VecompSoftware.DocSuiteWeb.Entity.Tenders;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Tenders
{
    public class TenderLotMapper : BaseEntityMapper<TenderLot, TenderLot>, ITenderLotMapper
    {
        public override TenderLot Map(TenderLot entity, TenderLot entityTransformed)
        {
            #region [ Base ]
            entityTransformed.CIG = entity.CIG;
            #endregion

            return entityTransformed;
        }

    }
}
