using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using BiblosDS.Library.Common.Objects;
using System.Security.Cryptography;
using System.Configuration;
using BiblosDS.Library.Common.Utility;
using BiblosDS.Library.Common.Enums;
using System.Globalization;

namespace BiblosDS.Library.Common.Services
{
    public class AttributeService : ServiceBase
    {
        static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(AttributeService));
        /// <summary>
        /// Restituisce l'elenco delgi attributi associati a un archive
        /// </summary>
        /// <param name="IdArchive">Guid archive di cui ottenere gli attributi</param>
        /// <returns>BindingList<DocumentAttribute></returns>
        public static BindingList<DocumentAttribute> GetAttributesFromArchive(Guid IdArchive)
        {
            try
            {
                return DbProvider.GetAttributesFromArchive(IdArchive);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "AttributeService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static BindingList<DocumentAttribute> GetAttributesByGroup(Guid idArchive, AttributeGroupType groupType)
        {
            var attributes = GetAttributesFromArchive(idArchive);
            return new BindingList<DocumentAttribute>(attributes.Where(a => a.AttributeGroup.GroupType == groupType).ToList());
        }
        public static BindingList<DocumentAttribute> GetChildAttributes(Guid idArchive)
        {
            var attributes = GetAttributesFromArchive(idArchive);
            return new BindingList<DocumentAttribute>(attributes.Where(a => a.AttributeGroup.GroupType != AttributeGroupType.Chain).ToList());
        }

        /// <summary>
        /// Restituissce un DocumentAttribute dal Guid
        /// </summary>
        /// <param name="IdAttribute">Guid dell'attribute da ottenere</param>
        /// <returns>DocumentAttribute</returns>
        public static DocumentAttribute GetAttribute(Guid IdAttribute)
        {
            try
            {
                return DbProvider.GetAttribute(IdAttribute);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "AttributeService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }

        }

        public static BindingList<DocumentAttributeGroup> GetAttributeGroup(Guid IdArchive)
        {
            return DbProvider.GetAttributeGroup(IdArchive);
        }

        /// <summary>
        /// Restituisce un elenco di DocumentAttributes relativi alla chiave doppia IdArchive , IdStorage
        /// </summary>
        /// <param name="IdArchive">Guid dell'archivio</param>
        /// <param name="IdStorage">Guid dello storage</param>
        /// <returns>BindingList<DocumentAttribute></returns>
        public static BindingList<DocumentAttribute> GetAttribute(Guid IdArchive, Guid IdStorage)
        {
            try
            {
                return DbProvider.GetAttributesFromArchiveStorage(IdArchive, IdStorage);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "AttributeService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        /// <summary>
        /// Restituisce un elenco di DocumentAttributes associati a un Document
        /// </summary>
        /// <param name="IdDocument">Guid Document di cui ottenere gli attributes</param>
        /// <returns>BindingList<DocumentAttributeValue></returns>
        public static BindingList<DocumentAttributeValue> GetAttributeValues(Guid IdDocument)
        {
            try
            {
                return DbProvider.GetAttributesValuesFromDocument(IdDocument);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "AttributeService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        /// <summary>
        /// Verifica e conversione nell'invariant Type degli attribute values
        /// </summary>
        /// <param name="AttributeValues"></param>
        /// <returns>
        /// Primary Key result of the query
        /// </returns>
        public static string ParseAttributeValues(DocumentArchive Archive, BindingList<DocumentAttributeValue> AttributeValues, out DateTime? MainDate)
        {
            BindingList<DocumentAttribute> attributes = AttributeService.GetAttributesFromArchive(Archive.IdArchive);
            return ParseAttributeValues(AttributeValues, attributes, out MainDate);
        }

        /// <summary>
        /// Verifica e conversione nell'invariant Type degli attribute values
        /// </summary>
        /// <param name="AttributeValues"></param>
        /// <returns>
        /// Primary Key result of the query
        /// </returns>
        public static string ParseAttributeValues(BindingList<DocumentAttributeValue> AttributeValues, BindingList<DocumentAttribute> Attributes, out DateTime? MainDate)
        {
            MainDate = null;
            //Assign the attribute retrive from the DB to the AttributeValues
            foreach (var item in AttributeValues)
            {
                logger.DebugFormat("ParseAttributeValues -> check exists attribute '{0}' with ID {1}", item.Attribute.Name, item.Attribute.IdAttribute);
                //Find on the name to the compatibilitì with the previous version
                var attribute = from attr in Attributes
                                where attr.Name == item.Attribute.Name
                                select attr;
                item.Attribute = attribute.Single();
            }
            //
            StringBuilder result = new StringBuilder();
            var queryAttributeValues = from attr in AttributeValues
                                       orderby attr.Attribute.KeyOrder
                                       select attr;
            foreach (DocumentAttributeValue item in queryAttributeValues)
            {
                //object invariantValue = null;               
                //if (!string.IsNullOrEmpty(item.Attribute.Format) && !string.IsNullOrEmpty(item.Attribute.Validation))
                //{
                //    item.Value = UtilityService.Format(item.Value, item.Attribute.Validation, item.Attribute.Format, Type.GetType(item.Attribute.AttributeType), out invariantValue);
                //    //Se non viene passato dal client l'invariant value lo calcolo                    
                //    value = Convert.ChangeType(invariantValue, Type.GetType(item.Attribute.AttributeType), System.Globalization.CultureInfo.InvariantCulture);
                //    //item.InvariantValue = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}", value);                                            
                //}
                //else
                //{
                //    value = Convert.ChangeType(item.Value, Type.GetType(item.Attribute.AttributeType), System.Globalization.CultureInfo.InvariantCulture);                    
                //    //item.InvariantValue = value.ToString();
                //    //TODO Nel vecchi se non era presente la formattazione ed era MainDate veniva memorizzaro come yyyyMMdd
                //    if (item.Attribute.IsMainDate != null && (bool)item.Attribute.IsMainDate)
                //        item.Value = string.Format("{0:yyyyMMdd}", value);
                //}
                //TODO Trap Invalid Cast Exception                
                var val = item.Value.TryConvert(Type.GetType(item.Attribute.AttributeType));
                if (item.Attribute.KeyOrder > 0)
                {
                    if (!string.IsNullOrEmpty(item.Attribute.KeyFormat))
                    {
                        if (item.Attribute.KeyFormat.IndexOf('[') != -1)
                        {
                            var substring = item.Attribute.KeyFormat.Substring(item.Attribute.KeyFormat.IndexOf('[') + 1, item.Attribute.KeyFormat.IndexOf("]") - item.Attribute.KeyFormat.IndexOf('[') - 1);
                            var format = substring.Split(',');
                            int lenght = val.ToString().Length;
                            if (format.Length >= 2)
                            {
                                int start = 0;
                                int end = 0;
                                if (int.TryParse(format[0], out start) && int.TryParse(format[1], out end))
                                {
                                    if (start + end > lenght)
                                        throw new Exceptions.Attribute_Exception("Attribute value non conforme alla formattazione: " + item.Attribute.KeyFormat);
                                    val = val.ToString().Substring(start, end);
                                }
                            }
                            var stringFormat = item.Attribute.KeyFormat.Substring(item.Attribute.KeyFormat.IndexOf(']') + 1, item.Attribute.KeyFormat.Length - item.Attribute.KeyFormat.IndexOf("]") - 1);
                            result.AppendFormat(stringFormat, val);
                        }
                        else
                            result.AppendFormat(item.Attribute.KeyFormat, val);
                    }
                    else
                        result.Append(item.Value);
                }
                if (item.Attribute.IsMainDate != null && (bool)item.Attribute.IsMainDate)
                {
                    if (item.Value is string)
                    {
                        MainDate = DateTime.Parse(item.Value as string, new CultureInfo("it-IT"));
                    }
                    else
                    {
                        MainDate = item.Value.TryConvert<DateTime>();
                    }
                    
                    logger.DebugFormat("MAIN DATE CONVERTER '{0}' -> '{1}'", item.Value, MainDate);
                }
                    
            }
            return result.ToString();
        }

        public static string GetAttributesHash(BindingList<DocumentAttributeValue> AttributeValues)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            return UtilityService.GetStringFromBob(sha.ComputeHash(GetAttributeBlob(AttributeValues)));
        }

        public static bool ExistPrimaryKey(BindingList<DocumentAttribute> Attributes)
        {
            var query = (from m in Attributes
                         where m.KeyOrder > 0
                         select m).Count();
            return query > 0;
        }

        public static bool ConvertAttributeValueType(BindingList<DocumentAttributeValue> AttributeValues)
        {
            var configValue = ConfigurationManager.AppSettings["BiblosDS.DisableNullAttribute"];
            var disableNullAttribute = !string.IsNullOrEmpty(configValue) && configValue.Equals("true", StringComparison.InvariantCultureIgnoreCase);

            foreach (var item in AttributeValues)
            {
                if (!disableNullAttribute && item.Value == null)
                    continue; // Se il valore è nullo salto la conversione ed evito di assegnare un valore.

                switch (item.Attribute.AttributeType)
                {
                    case "System.String":
                        item.Value = item.Value.TryConvert<string>();
                        break;
                    case "System.Int64":
                        item.Value = item.Value.TryConvert<Int64>();
                        break;
                    case "System.Double":
                        item.Value = item.Value.TryConvert<double>();
                        break;
                    case "System.DateTime":
                        item.Value = item.Value.TryConvert<DateTime>();
                        break;
                    default:
                        throw new Exceptions.Attribute_Exception(string.Format("Attributo di tipo {0} non supportato.", item.Attribute.AttributeType));
                }
            }
            return true;
        }

        /// <summary>
        /// Verify the metadata
        /// </summary>
        /// <param name="Document"></param>
        /// <param name="Archive"></param>
        /// <param name="AttributeValues"></param>
        /// <param name="Modify"></param>
        /// <returns>
        /// The list of Exceptions on the AttributeValue
        /// </returns>
        public static BindingList<Exception> CheckMetaData(Document Document, DocumentArchive Archive, BindingList<DocumentAttributeValue> AttributeValues, Boolean Modify)
        {
            BindingList<DocumentAttribute> attributes = AttributeService.GetAttributesFromArchive(Archive.IdArchive);
            return CheckMetaData(Document, Archive, AttributeValues, attributes, Modify);
        }

        /// <summary>
        /// Verify the metadata
        /// </summary>
        /// <param name="Document"></param>
        /// <param name="Archive"></param>
        /// <param name="AttributeValues"></param>
        /// <param name="Modify"></param>
        /// <returns>
        /// The list of Exceptions on the AttributeValue
        /// </returns>
        public static BindingList<Exception> CheckMetaData(Document Document, DocumentArchive Archive, BindingList<DocumentAttributeValue> AttributeValues, BindingList<DocumentAttribute> Attributes, Boolean Modify)
        {
            if (Attributes == null || Attributes.Count == 0)
                return new BindingList<Exception>();

            BindingList<Exception> exceptions = new BindingList<Exception>();
            //1)
            //Filtro sugli attributi obbligatori
            //Check che esistano tutti gli attributi richiesti
            var query = from attr in Attributes
                        where !(from attrreq in AttributeValues
                                select attrreq.Attribute.Name).Contains(attr.Name)
                              && attr.IsRequired
                        select attr;
            var queryEmpty = from attr in Attributes
                             where (from attrreq in AttributeValues
                                    where attrreq.Value == null
                                    select attrreq.Attribute.Name).Contains(attr.Name)
                                   && attr.IsRequired
                             select attr;
            //Verifica sul tipo degli attributi
            if (ConfigurationManager.AppSettings["VerifyAttributeType"] != null && ConfigurationManager.AppSettings["VerifyAttributeType"].Equals("true"))
                foreach (var item in AttributeValues)
                {
                    var attr = Attributes.Where(x => x.Name == item.Attribute.Name).FirstOrDefault();
                    if (attr == null)
                        exceptions.Add(new Exceptions.Attribute_Exception("Errore, attributo non definito nell'archivio: " + item.Attribute.Name));
                    if (!item.Value.IsTypeOf(Type.GetType(attr.AttributeType)))
                        exceptions.Add(new Exceptions.Attribute_Exception("Errore nel tipo di dato: " + item.Attribute.Name + ". Previsto: " + attr.AttributeType));
                }
            foreach (var item in AttributeValues)
            {
                logger.DebugFormat("CheckMetaData {0} : {1}", item.Attribute.Name, item.Value);
                var attr = Attributes.Where(x => x.Name == item.Attribute.Name).FirstOrDefault();
                if (attr == null)
                    exceptions.Add(new Exceptions.Attribute_Exception("Errore, attributo non definito nell'archivio: " + item.Attribute.Name));
                else if (attr.MaxLenght.GetValueOrDefault() > 0 && item.Value != null && item.Value.ToString().Length > attr.MaxLenght.GetValueOrDefault())
                    exceptions.Add(new Exceptions.Attribute_Exception("Errore lunghezza del dato: " + item.Attribute.Name + ". Previsto: " + attr.MaxLenght.GetValueOrDefault() + " di " + item.Value.ToString().Length));
            }
            //Se esistono attributi obbligatori non presenti negli attributi del documento
            //check fallisce
            if (query.Count() > 0 || queryEmpty.Count() > 0)
            {
                StringBuilder requiredMessage = new StringBuilder();
                foreach (var item in query)
                {
                    requiredMessage.AppendFormat("Valore obbligatorio mancante {0}", item.Name);
                }
                foreach (var item in queryEmpty)
                {
                    requiredMessage.AppendFormat("Valore obbligatorio mancante {0}", item.Name);
                }
                exceptions.Add(new Exceptions.AttributeRequired_Exception(requiredMessage.ToString()));
            }
            //2)
            //Chech per modificabile
            if (Modify)
            {
                BindingList<DocumentAttributeValue> documentOldVersin = AttributeService.GetAttributeValues(Document.IdDocument);
                ConvertAttributeValueType(AttributeValues);
                //Case Mode = 0
                //Non modificabile dopo inserimento
                //Se gli attributi sono cambiati dalla versione precedente-->Errore
                //Query (Attributi che hanno mode a zero e che hanno il valore cambiato da quello salvato precedentemente
                var queryDisabled = from m in AttributeValues
                                    where (from attr in Attributes
                                           where attr.Mode.IdMode == 0
                                           select attr.IdAttribute).Contains(m.Attribute.IdAttribute)
                                           && !(from old in documentOldVersin
                                                where old.Attribute.IdAttribute == m.Attribute.IdAttribute
                                                select old.Value).Equals(m.Value)
                                    select m;
                foreach (var item in queryDisabled)
                {
                    var attr = documentOldVersin.Where(x => x.Attribute.IdAttribute == item.Attribute.IdAttribute).FirstOrDefault();
                    if (attr == null || !attr.Value.Equals(item.Value))
                        exceptions.Add(new Exceptions.Attribute_Exception("Errore manca diritto modifica sul campo: " + item.Attribute.Name));
                }
                //Case Mode = 1                
                //Modificabile se vuoto
                var queryWriteOnce = from m in AttributeValues
                                     where (from attr in Attributes
                                            where attr.Mode.IdMode == 1
                                            select attr.IdAttribute).Contains(m.Attribute.IdAttribute)
                                         && !(from old in documentOldVersin
                                              where old.Attribute.IdAttribute == m.Attribute.IdAttribute
                                              select old.Value).Equals(string.Empty)
                                     select m;
                foreach (var item in queryWriteOnce)
                {
                    //Scatena eccezione solo se effettivamente modificato.
                    if (documentOldVersin.Where(x => x.Value != item.Value && x.Attribute.IdAttribute == item.Attribute.IdAttribute).Count() > 0)
                        exceptions.Add(new Exceptions.Attribute_Exception("Errore manca diritto modifica sul campo valorizzato: " + item.Attribute.Name));
                }
                //Case Mode = 2
                //Modificabile se non archiviato
                if (Document.IsConservated.HasValue && (bool)Document.IsConservated)
                {
                    var ModifyUntilConservated = from m in AttributeValues
                                                 where (from attr in Attributes
                                                        where attr.Mode.IdMode == 2
                                                        select attr.IdAttribute).Contains(m.Attribute.IdAttribute)
                                                        && !(from old in documentOldVersin
                                                             where old.Attribute.IdAttribute == m.Attribute.IdAttribute
                                                             select old.Value).Equals(m.Value)
                                                 select m;
                    foreach (var item in queryWriteOnce)
                    {
                        exceptions.Add(new Exceptions.Attribute_Exception("Errore manca diritto modifica sul campo valorizzato: " + item.Attribute.Name));
                    }
                }
            }
            else
            {
                //foreach (var item in AttributeValues)
                //{
                //    if (item.Attribute.IsUnique.HasValue && item.Attribute.IsUnique.Value)
                //    {
                //        //TODO Check is is unique
                //    }
                //}
            }
            return exceptions;
        }

        #region Private

        /// <summary>
        /// Function to sign the document attribute
        /// </summary>
        /// <param name="AttributeValues"></param>
        /// <returns></returns>
        private static byte[] GetAttributeBlob(BindingList<DocumentAttributeValue> AttributeValues)
        {
            StringBuilder result = new StringBuilder();
            foreach (DocumentAttributeValue item in AttributeValues)
            {
                result.AppendFormat("{0}={1}", item.Attribute.Name, item.Value);
            }
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetBytes(result.ToString());
        }
        #endregion

        #region Admin

        /// <summary>
        /// Inserisce un nuovo DocumentAttribute
        /// </summary>
        /// <param name="DocumentAttribute">DocumentAttribute da inserire</param>
        public static void AddAttribute(DocumentAttribute DocumentAttribute)
        {
            if (string.IsNullOrEmpty(DocumentAttribute.Name)
                || string.IsNullOrEmpty(DocumentAttribute.AttributeType)
                || DocumentAttribute.Archive == null
                || DocumentAttribute.Mode == null)
                throw new Exception("Requisiti non sufficienti per l'operazione");
            DocumentAttribute.IdAttribute = Guid.NewGuid();
            DbAdminProvider.AddAttribute(DocumentAttribute);
        }
        /// <summary>
        /// Inserisce un nuovo gruppo di attributi.
        /// </summary>
        /// <param name="AttributeGroup"></param>
        public static void AddAttributeGroup(DocumentAttributeGroup AttributeGroup)
        {
            DbProvider.AddAttributeGroup(AttributeGroup);
        }
        /// <summary>
        /// Salva le modifiche a un DocumentAttribute
        /// </summary>
        /// <param name="DocumentAttribute">DocumentAttribute modificato</param>
        public static void UpdateAttribute(DocumentAttribute DocumentAttribute)
        {
            if (DocumentAttribute.IdAttribute == Guid.Empty
                || string.IsNullOrEmpty(DocumentAttribute.Name)
                || string.IsNullOrEmpty(DocumentAttribute.AttributeType)
                || DocumentAttribute.Archive == null
                || DocumentAttribute.Mode == null)
                throw new Exception("Requisiti non sufficienti per l'operazione");
            DbAdminProvider.UpdateAttribute(DocumentAttribute);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="AttributeGroup"></param>
        public static void UpdateAttributeGroup(DocumentAttributeGroup AttributeGroup)
        {
            DbProvider.UpdateAttributeGroup(AttributeGroup);
        }
        /// <summary>
        /// Elimina permanentemente un DocumentAttribute
        /// </summary>
        /// <param name="IdAttribute">Guid del DocumentAttribute da eliminare</param>
        public static void DeleteAttribute(Guid IdAttribute)
        {
            DbAdminProvider.DeleteAttribute(IdAttribute);
        }
        /// <summary>
        /// Cancellazione logica AttributeGroup.
        /// </summary>
        /// <param name="AttributeGroup"></param>
        public static void DeleteAttributeGroup(Guid IdAttributeGroup)
        {
            DbProvider.DeleteAttributeGroup(IdAttributeGroup);
        }
        /// <summary>
        /// Restituisce l'elenco di tutti i DocumentAttributeMode disponibili
        /// </summary>
        /// <returns>BindingList<DocumentAttributeMode> </returns>
        public static BindingList<DocumentAttributeMode> GetAttributeModes()
        {
            try
            {
                return DbAdminProvider.GetAttributeModes();
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "AttributeService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        #endregion


        public static void UpdateAttributesHash(Document doc)
        {
            var attrs = GetAttributeValues(doc.IdDocument);
            DbProvider.UpdateDocumentSignHeader(doc.IdDocument, GetAttributesHash(attrs));
        }

        /// <summary>
        /// Filter distinct values if the attribues in the archive
        /// </summary>
        /// <param name="idArchive"></param>
        /// <param name="idAttributes"></param>
        /// <returns></returns>
        public static Dictionary<Guid, BindingList<string>> GetDistinctAttributesValues(BindingList<Guid> idAttributes)
        {
            try
            {
                if (idAttributes == null)
                {
                    throw new Exceptions.Attribute_Exception("Nessun attributo passato. Specificare uno o più attributi e riprovare.");
                }
                logger.DebugFormat("GetDistinctAttributesValues: {0}", string.Join(";", idAttributes));
                Dictionary<Guid, BindingList<string>> result = new Dictionary<Guid, BindingList<string>>();
                foreach (var item in idAttributes)
                {
                    var values = DbProvider.GetDistinctAttributesValues(item);
                    result.Add(item, values);
                }
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public static BindingList<DocumentAttributeValue> GetFullDocumentAttributeValues(Guid idDocument)
        {
            try
            {
                return DbProvider.GetFullDocumentAttributeValues(idDocument);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General,
                                        "AttributeService.GetFullDocumentAttributeValues",
                                        e.ToString(),
                                        LoggingOperationType.BiblosDS_General,
                                        LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }
    }
}
