using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Resolutions;
using VecompSoftware.DocSuiteWeb.DTO.Resolutions;
using VecompSoftware.DocSuiteWeb.EntityMapper.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.Resolutions
{
    public class ResolutionKindDocumentSeriesFacade: BaseResolutionFacade<ResolutionKindDocumentSeries, Guid, ResolutionKindDocumentSeriesDao>
    {
         #region [ Fields ]
        private MapperResolutionKindType _mapperResolutionKindType;
        private readonly string _userName;
        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]

        public ResolutionKindDocumentSeriesFacade(string userName)
            : base()
        {
            _userName = userName;
            _mapperResolutionKindType = new MapperResolutionKindType();
        }

        #endregion [ Constructor ]

        #region [ Methods ]
        /// <summary>
        /// Verifica se è obbligatoria la gestione dei documenti, se la serie documentale non è prevista nella tipologia dell'atto corrente, il documento non è obbligatorio
        /// </summary>
        /// <param name="idResolutionKind">presente nell'atto corrente</param>
        /// <param name="idSeries">ricercata</param>
        /// <returns></returns>
        public bool IsDocumentRequired(Guid idResolutionKind, int idSeries)
        {
            ResolutionKindDocumentSeries item = _dao.GetByResolutionAndSeries(idResolutionKind, idSeries);
            if (item == null)
                return false;
            else
                return item.DocumentRequired;
        }


        /// <summary>
        /// Recupera tutte le resolution in base al DTO ResolutionKindType
        /// </summary>
        public ICollection<ResolutionKindTypeResults> GetResolutionKindType()
        {
            ICollection<ResolutionKindTypeResults> result = null;
            foreach (ResolutionKindDocumentSeries p in _dao.GetResolutionAndSeries())
            {
                result.Add(_mapperResolutionKindType.TransformDTO(p));
            }
            return result;
        }

        /// <summary>
        /// Recupera una  resolution in base al guid resolution kind  e id documentseries 
        /// </summary>
        public ResolutionKindDocumentSeries GetResolutionAndSeriesByReslAndSeries(Guid idResolutionKind, int idSeries)
        {
            return _dao.GetByResolutionAndSeries(idResolutionKind, idSeries);
        }

        //GetByIdResolutionKind
        public ICollection<ResolutionKindDocumentSeries> GetResolutionAndSeriesByIdResolutionKind(Guid idResolutionKind)
        {
            return _dao.GetByIdResolutionKind(idResolutionKind);
        }
        #endregion [ Methods ]
    }
}
