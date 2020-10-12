using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BiblosDS.Library.Common.Objects.Response;

namespace BiblosDS.Library.Common.Enums
{
    public enum PreservationErrorCode : int
    {
        /// <summary>
        /// Nessun errore.
        /// </summary>
        E_NO_ERROR = ResponseError.E_NO_ERROR,
        /// <summary>
        /// Chiamata a metodo/proprietà non valida.
        /// </summary>
        E_INVALID_CALL = 1,
        /// <summary>
        /// Parametri in ingresso ad una funzione non coerenti.
        /// </summary>
        E_INVALID_PARAMS = 2,
        /// <summary>
        /// Eccezione generica (gestita ma non specificata).
        /// </summary>
        E_USER_DEFINED_EXCEPTION = 3,
        /// <summary>
        /// Eccezione di sistema.
        /// </summary>
        E_SYSTEM_EXCEPTION = 4,
        /// <summary>
        /// Eccezione lanciata quando un'operazione torna un risultato inaspettato o non corretto.
        /// </summary>
        E_UNEXPECTED_RESULT = 5,
        /// <summary>
        /// 
        /// </summary>
        E_PRESERVATION_EX = 6,
        /// <summary>
        /// Conservazione in verifica
        /// </summary>
        E_PRESERVATION_VERIFY_EX = 7,
        E_NO_DOCUMENT_EX = 8,
        E_EXIST_NO_CONSERVATED_DOCUMENT = 9
    }
}
