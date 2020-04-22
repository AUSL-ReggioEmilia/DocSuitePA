using System.Collections.Generic;

namespace VecompSoftware.JeepService.PosteWeb.Service
{
    internal static class ValorizzaStatoLavorazione
    {
        #region [ Fields ]

        public const char Prezzato = 'R';
        public const char Postalizzato = 'S';

        public const char Annullato = 'A';
        public const char NonConvertito = 'N';
        public const char NonValido = 'Y';
        public const char PagineInEccesso = 'J';
        public const char NonPrezzato = 'G';
        public const char TimeoutConferma = 'U';
        public const char TimeoutPostel = 'V';
        public const char ErroreNegliEsiti = 'W';

        #endregion

        #region [ Properties ]

        /// <summary> Insieme degli stati positivi. </summary>
        public static List<char> OkStatus
        {
            get
            {
                return new List<char>
                {
                    Prezzato,
                    Postalizzato
                };
            }
        }

        /// <summary> Insieme degli stati d'errore. </summary>
        public static List<char> ErrorStatus
        {
            get
            {
                return new List<char>
                {
                    NonConvertito,
                    NonValido,
                    PagineInEccesso,
                    NonPrezzato,
                    Annullato,
                    TimeoutConferma,
                    TimeoutPostel,
                    ErroreNegliEsiti
                };
            }
        }

        #endregion

    }
}