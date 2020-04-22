using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;

namespace VecompSoftware.JeepService.CollegioSindacaleTorino
{
    internal class Tools
    {
        #region [ Fields ]

        private static readonly Lazy<FacadeFactory> LazyFactory = new Lazy<FacadeFactory>();

        #endregion

        #region [ Properties ]

        public static FacadeFactory Factory
        {
            get { return LazyFactory.Value; }
        }

        #endregion

        #region [ Methods ]

        /// <summary> Torna la descrizione al plurale della tipologia. </summary>
        public static string ResolutionTypeCaptionPlural(short type)
        {
            switch (type)
            {
                case ResolutionType.IdentifierDelibera:
                    return "Delibere";
                case ResolutionType.IdentifierDetermina:
                    return "Determine";
                default:
                    throw new CollegioSindacaleTorinoException("Caso non previsto.");
            }
        }

        /// <summary> Torna la descrizione al plurale della tipologia. </summary>
        public static string ResolutionTypeCaption(short type)
        {
            switch (type)
            {
                case ResolutionType.IdentifierDelibera:
                    return "delibera";
                case ResolutionType.IdentifierDetermina:
                    return "determina";
                default:
                    throw new CollegioSindacaleTorinoException("Caso non previsto.");
            }
        }

        #endregion
    }
}
