using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.API
{
    public static class ResponseExtension
    {
        private static Dictionary<int, string> decodificaErrori;
        public static Dictionary<int, string> DecodificaErrori
        {
            get
            {
                if (ResponseExtension.decodificaErrori == null)
                    ResponseExtension.decodificaErrori = Helpers.GetErrori();

                return ResponseExtension.decodificaErrori;
            }
        }

        public static void CheckResponse(this ResponseBase response)
        {
            var codice = (int)response.CodiceEsito;

            if (response.CodiceEsito == CodiceErrore.NessunErrore)
            {
                if(DecodificaErrori.ContainsKey(codice))
                    response.MessaggioEsito = DecodificaErrori[codice];

                if (string.IsNullOrWhiteSpace(response.MessaggioEsito))
                    response.MessaggioEsito = response.CodiceEsito.ToString();
            }
            else if (DecodificaErrori.ContainsKey(codice))
            {                
                response.MessaggioErrore = DecodificaErrori[codice];

                if (string.IsNullOrWhiteSpace(response.MessaggioErrore))
                    response.MessaggioErrore = response.CodiceEsito.ToString();
            }
            else
            {
                response.MessaggioErrore = response.CodiceEsito.ToString();

                //codice = (int)CodiceErrore.ErroreGenerico;
                //if (DecodificaErrori.ContainsKey(codice))
                //{
                //    response.MessaggioErrore = DecodificaErrori[codice];
                //}
                //else
                //{
                //    response.MessaggioErrore = "Errore generico (codice errrore \"" + codice + "\" non gestito).";
                //}
            }
        }
    }
}
