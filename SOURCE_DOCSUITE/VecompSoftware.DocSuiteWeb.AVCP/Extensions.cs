using System;
using VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.DocSuiteWeb.AVCP
{
    public static class Extensions
    {
        public static string CodiceServizio(this Resolution resolution)
        {
            try
            {
                string[] attoTokens = resolution.InclusiveNumber.Split('/');
                if (attoTokens.Length < 1)
                    throw new DocSuiteException("Errore resolution", "Impossibile estrarre codice servizio.");

                return attoTokens.Length > 2 ? attoTokens[1] : String.Empty;
            }
            catch (Exception exc)
            {
                throw new DocSuiteException("Errore in estrazione CodiceServizio per Resolution " + resolution.Id, exc);
            }
        }

        public static Int32 NumeroAtto(this Resolution resolution)
        {
            try
            {
                string[] attoTokens = resolution.InclusiveNumber.Split('/');
                if (attoTokens.Length < 1)
                    throw new DocSuiteException("Errore resolution", "Impossibile estrarre numero atto.");

                return Int32.Parse(attoTokens[attoTokens.Length - 1]);
            }
            catch (Exception exc)
            {
                throw new DocSuiteException("Errore in estrazione NumeroAtto per Resolution " + resolution.Id, exc);
            }
        }

        public static Int32 MyCategoryId(this Resolution resolution)
        {
            var c = resolution.SubCategory == null ? resolution.Category.Id : resolution.SubCategory.Id;
            return c;
        }

    }
}
