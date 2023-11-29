using NHibernate;
using System;
using APIFasc = VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using DSW = VecompSoftware.DocSuiteWeb.Data.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Commons
{
    public class MapperFasciclePeriod : BaseEntityMapper<DSW.FasciclePeriod, APIFasc.FasciclePeriod>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public MapperFasciclePeriod()
        {
        }
        #endregion

        #region [ Methods ]

        protected override IQueryOver<DSW.FasciclePeriod, DSW.FasciclePeriod> MappingProjection(IQueryOver<DSW.FasciclePeriod, DSW.FasciclePeriod> queryOver)
        {
            throw new NotImplementedException(); 
        }

        protected override APIFasc.FasciclePeriod TransformDTO(DSW.FasciclePeriod entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Impossibile trasformare FasciclePeriod se l'entità non è inizializzata");
            }

            APIFasc.FasciclePeriod fasciclePeriod = new APIFasc.FasciclePeriod();
            fasciclePeriod.IsActive = entity.IsActive;
            fasciclePeriod.LastChangedDate = entity.LastChangedDate;
            fasciclePeriod.LastChangedUser = entity.LastChangedUser;
            fasciclePeriod.PeriodDays = entity.PeriodDays;
            fasciclePeriod.PeriodName = entity.PeriodName;
            fasciclePeriod.RegistrationDate = entity.RegistrationDate;
            fasciclePeriod.RegistrationUser = entity.RegistrationUser;
            fasciclePeriod.UniqueId = entity.Id;

            return fasciclePeriod;
        }

        #endregion

    }
}
