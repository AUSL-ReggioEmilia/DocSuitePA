using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

using BiblosDS.Library.Storage.PaRERReferti.XSD;
using BiblosDS.Library.Storage.PaRERReferti.Entities;
using BiblosDS.Library.Storage.PaRERReferti.Util;  

namespace BiblosDS.Library.Storage.PaRERReferti
{
    /// <summary>
    /// Factory di request per il Parer
    /// </summary>
    public class RequestFactory 
    {
        public static string GetXmlRequestForParer(Document thisDocument, ParerContext thisContext, string TipoDocumento, string Ambiente)
        {
            UnitaDocumentaria thisParerDoc = GetRequestForParer(thisDocument, thisContext, TipoDocumento, Ambiente);
            return GetXmlForUnitaDocumentaria(thisParerDoc); 
        }

        /// <summary>
        /// crea una richiesta per l'inserimento in Parer di un documento
        /// </summary>
        /// <returns></returns>
        private static UnitaDocumentaria GetRequestForParer(Document thisDocument, ParerContext thisContext, string TipoDocumento, string Ambiente)
        {
            UnitaDocumentaria thisParerDoc = new UnitaDocumentaria();
            
            ParerConfig thisConfig = new ParerConfig();

            thisParerDoc = UnitaDocumetariaParerHelper.GetIntestazione(thisParerDoc, thisDocument, thisContext, Ambiente);
            thisParerDoc = UnitaDocumetariaParerHelper.GetConfigurazione(thisParerDoc, thisDocument, thisContext, thisConfig);
            thisParerDoc = UnitaDocumetariaParerHelper.GetDocumentoPrincipale(thisParerDoc, thisDocument);
            // thisParerDoc = UnitaDocumetariaParerHelper.GetFascicolo(thisParerDoc, thisDocument);
            thisParerDoc = UnitaDocumetariaParerHelper.GetProfilo(thisParerDoc, thisDocument);
            
            // non c'è gestione allegati 
            // thisParerDoc = UnitaDocumetariaParerHelper.GetProfiloAllegati(thisParerDoc, thisDocument);
            thisParerDoc = UnitaDocumetariaParerHelper.GetProfiloCollegate(thisParerDoc, thisDocument); 

            // Carica i dati specifici dei referti 
            switch (TipoDocumento)
            {
                case "REFERTO":
                    thisParerDoc = UnitaDocumetariaParerHelper.GetDatiSpecificiReferto(thisParerDoc, thisDocument);
                    break;
                case "MODELLO770" :
                    thisParerDoc = UnitaDocumetariaParerHelper.GetDatiSpecificiModello770(thisParerDoc, thisDocument); 
                    break;
                case "CUD" :
                    thisParerDoc = UnitaDocumetariaParerHelper.GetDatiSpecificiCUD(thisParerDoc, thisDocument);
                    break; 
                case "CEDOLINO STIPENDIALE" :
                    thisParerDoc = UnitaDocumetariaParerHelper.GetDatiSpecificiCedolino(thisParerDoc, thisDocument);
                    break; 
                case "CEDOLINO STIPENDIALE CUMULATIVO" :
                    thisParerDoc = UnitaDocumetariaParerHelper.GetDatiSpecificiCedolinoComulativo(thisParerDoc, thisDocument); 
                    break; 
                case "MODELLOF24" :
                    // F24 non ha dati specifici
                    // thisParerDoc = UnitaDocumetariaParerHelper.GetDatiSpecificiModelloF24(thisParerDoc, thisDocument);
                    break; 
            }
           
            return thisParerDoc; 
        }

        /// <summary>
        /// Serializzazione con encoding 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string SerializeObjectToXmlString<T>(T obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());

            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                return writer.ToString();
            }
        }

        /// <summary>
        /// ritorna l'xml per la richiesta di esibizione al Parer
        /// </summary>
        /// <param name="thisRecupero"></param>
        /// <returns></returns>
        public static string GetXmlForStatusRequest(ParerStatus.Recupero thisRecupero)
        {
            string xmlRecupero = SerializeObjectToXmlString<ParerStatus.Recupero>(thisRecupero);

            xmlRecupero = ReplaceFirst(xmlRecupero, "encoding=\"utf-16\"", "");

            do
            {
                xmlRecupero = ReplaceFirst(xmlRecupero, "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");
            } while (xmlRecupero.IndexOf("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"") != -1);
            do
            {
                xmlRecupero = ReplaceFirst(xmlRecupero, "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
            } while (xmlRecupero.IndexOf("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"") != -1);

            return xmlRecupero;  
        }
        
        /// <summary>
        /// Ritorna l'xml per il conferimento di una unità documentaria
        /// </summary>
        /// <param name="thisParerDoc"></param>
        /// <returns></returns>
        private static string GetXmlForUnitaDocumentaria(UnitaDocumentaria thisParerDoc)
        {
            string xmlUnitaDocumentaria = SerializeObjectToXmlString<UnitaDocumentaria>(thisParerDoc); 
           
            // tolgo la dichiarazione del utf16 
            xmlUnitaDocumentaria = ReplaceFirst(xmlUnitaDocumentaria, "encoding=\"utf-16\"", ""); 

            // dato che ci sono due 
            //   <DatiSpecifici>
            //   <DatiSpecifici xmlns:xsi=....>
            // dobbiamo togliere i primi 
             
            xmlUnitaDocumentaria = ReplaceFirst(xmlUnitaDocumentaria, "<DatiSpecifici>","") ;                          
            xmlUnitaDocumentaria = ReplaceFirst(xmlUnitaDocumentaria,"</DatiSpecifici>","") ;

            // nella gestione allegati e annessi si possono trovare diversi <DatiSpecifici xsi:nil="true" /> 
            // che vanno tolti
            do
            {
                xmlUnitaDocumentaria = ReplaceFirst(xmlUnitaDocumentaria, "<DatiSpecifici xsi:nil=\"true\" />", ""); 
            } while (xmlUnitaDocumentaria.IndexOf("<DatiSpecifici xsi:nil=\"true\" />") != -1);

            do 
            {
                xmlUnitaDocumentaria = ReplaceFirst(xmlUnitaDocumentaria, "<DatiSpecificiMigrazione xsi:nil=\"true\" />", "") ;
            } while (xmlUnitaDocumentaria.IndexOf("<DatiSpecificiMigrazione xsi:nil=\"true\" />") != -1) ; 


            // tomassetti 20120119 : xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd=http://www.w3.org/2001/XMLSchema
            // **REMOVE** 20121205 :  xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
            do
            {
                xmlUnitaDocumentaria = ReplaceFirst(xmlUnitaDocumentaria, "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");
            } while (xmlUnitaDocumentaria.IndexOf("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"") != -1); 
            do 
            {
                xmlUnitaDocumentaria = ReplaceFirst(xmlUnitaDocumentaria, "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
            } while (xmlUnitaDocumentaria.IndexOf("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"") != -1); 

            return xmlUnitaDocumentaria;  
        }

        private static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }
}
