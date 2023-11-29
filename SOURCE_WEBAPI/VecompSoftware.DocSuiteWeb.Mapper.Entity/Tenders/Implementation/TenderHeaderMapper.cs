using VecompSoftware.DocSuiteWeb.Entity.Tenders;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Tenders
{
    public class TenderHeaderMapper : BaseEntityMapper<TenderHeader, TenderHeader>, ITenderHeaderMapper
    {
        public override TenderHeader Map(TenderHeader entity, TenderHeader entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Abstract = entity.Abstract;
            entityTransformed.Title = entity.Title;
            entityTransformed.Year = entity.Year;
            #endregion

            return entityTransformed;
        }

    }
}
