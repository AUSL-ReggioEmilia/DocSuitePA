using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.DTO.Resolutions;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Resolutions
{
    public class MapperResolutionKindType : BaseEntityMapper<ResolutionKind, ResolutionKindTypeResults>
    {
        #region Constructor
        public MapperResolutionKindType() : base() { }
        #endregion

        /// <summary>
        /// Mapping di una ResolutionKindDocumentSeries su ResolutionKindTypeResults.
        /// <see cref="ResolutionKindDocumentSeriesFacade.GetResolutionKindType"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DTO.Resolutions.ResolutionKindTypeResults TransformDTO(ResolutionKindDocumentSeries entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare ResolutionKindDocumentSeries se l'entità non è inizializzata");

            ResolutionKindTypeResults reslkindtype = new ResolutionKindTypeResults();
            reslkindtype.Name = entity.ResolutionKind.Name;
            reslkindtype.IsRequired = entity.DocumentRequired;
            reslkindtype.ResolutionKindIsActive = entity.ResolutionKind.IsActive;
            reslkindtype.ResolutionKindAmountEnabled = entity.ResolutionKind.AmountEnabled; 
            return reslkindtype;
        }

        protected override DTO.Resolutions.ResolutionKindTypeResults TransformDTO(ResolutionKind entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare ResolutionKind se l'entità non è inizializzata");

            ResolutionKindTypeResults reslkindtype = new ResolutionKindTypeResults();
            reslkindtype.Name = entity.Name;
            reslkindtype.IsRequired = entity.ResolutionKindDocumentSeries.FirstOrDefault().DocumentRequired;
            reslkindtype.ResolutionKindIsActive = entity.IsActive;
            reslkindtype.ResolutionKindAmountEnabled = entity.AmountEnabled;
            return reslkindtype;
        }

        protected override NHibernate.IQueryOver<ResolutionKind, ResolutionKind> MappingProjection(NHibernate.IQueryOver<ResolutionKind, ResolutionKind> queryOver)
        {
            ResolutionKindTypeResults reslKindType = null;
            ResolutionKindDocumentSeries resolutionKindDocumentSeries = null;
            Data.DocumentSeries documentSeries = null;

            queryOver.SelectList(list => list
                .Select(x => x.Id).WithAlias(() => reslKindType.IdResolutionKind)
                .Select(x => x.Name).WithAlias(() => reslKindType.Name)
                .Select(() => resolutionKindDocumentSeries.DocumentRequired).WithAlias(() => reslKindType.IsRequired)
                .Select(x => x.IsActive).WithAlias(() => reslKindType.ResolutionKindIsActive)
                .Select(x => x.AmountEnabled).WithAlias(() => reslKindType.ResolutionKindAmountEnabled)
                .Select(x => x.IsActive).WithAlias(() => reslKindType.ResolutionKindDocumentSeriesIsActive) 
                .Select(() => documentSeries.Id).WithAlias(() => reslKindType.DocumentSeriesId)
                .Select(() => resolutionKindDocumentSeries.Id).WithAlias(() => reslKindType.IdResolutionKindDocumentSeries)
                .Select(() => documentSeries.Name).WithAlias(() => reslKindType.DocumentSeriesName));
                
            return queryOver;           
        }
    }
}
