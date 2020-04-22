using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; 
using System.Xml;
using System.Xml.Serialization;

using BiblosDS.Library.Storage.PaRERReferti.XSD;
using ParerStatusResponse; 

namespace BiblosDS.Library.Storage.PaRERReferti.Util
{
    public class EsitoStatusFactory
    {
        StatoConservazione _thisEsito; 

        public EsitoStatusFactory(string ParerResult)
        {
            XmlSerializer serializerEsito = new XmlSerializer(typeof(StatoConservazione));
            TextReader reader = new StringReader(ParerResult) ;

            _thisEsito = (StatoConservazione)serializerEsito.Deserialize(reader);

            reader.Dispose(); 
        }

        public bool HasError
        {
            get
            {
                if (_thisEsito.EsitoGenerale.CodiceEsito == ParerStatusResponse.ECEsitoExtType.POSITIVO)
                    return false ;
                else
                    return true; 
            }
        }

        public bool HasWarning
        {
            get
            {
                if (_thisEsito.EsitoGenerale.CodiceEsito == ParerStatusResponse.ECEsitoExtType.WARNING)
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
                    return "";
                }
                catch
                {
                    return ""; 
                }
            }
        }

        public Nullable<DateTime> StatusDate
        {
            get
            {
                try
                {
                    return _thisEsito.DataRichiestaStato ;
                }
                catch
                {
                    return null ; 
                }
            }
        }
    }
}
