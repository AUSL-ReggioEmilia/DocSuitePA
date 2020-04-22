using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; 
using System.Xml;
using System.Xml.Serialization;

using BiblosDS.Library.Storage.PaRERReferti.XSD;

namespace BiblosDS.Library.Storage.PaRERReferti.Util
{
    public class EsitoFactory
    {
        EsitoVersamento _thisEsito; 

        public EsitoFactory(string ParerResult)
        {
            XmlSerializer serializerEsito = new XmlSerializer(typeof(EsitoVersamento));
            TextReader reader = new StringReader(ParerResult) ;

            _thisEsito = (EsitoVersamento)serializerEsito.Deserialize(reader);

            reader.Dispose(); 
        }

        public bool HasError
        {
            get
            {
                if (_thisEsito.EsitoGenerale.CodiceEsito == ECEsitoExtType.NEGATIVO)
                    return true ;
                else
                    return false ; 
            }
        }

        public bool HasWarning
        {
            get
            {
                if (_thisEsito.EsitoGenerale.CodiceEsito == ECEsitoExtType.WARNING)
                    return true;
                else
                    return false;
            }
        }

        public string Errors
        {
            get
            {
                try
                {
                    return _thisEsito.EsitoGenerale.MessaggioErrore;
                }
                catch
                {
                    return ""; 
                }
            }
        }

        public string Uri
        {
            get
            {
                try
                {
                    return _thisEsito.UnitaDocumentaria.DocumentoPrincipale.Componenti[0].URN;
                }
                catch
                {
                    return ""; 
                }
            }
        }

        public Nullable<DateTime> ArchiviedDate
        {
            get
            {
                try
                {
                    return _thisEsito.DataVersamento;
                }
                catch
                {
                    return null ; 
                }
            }
        }
    }
}
