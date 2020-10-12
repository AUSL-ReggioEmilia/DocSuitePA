using System;
using System.Collections.Generic;
using VecompSoftware.BPM.Integrations.Modules.SM.ACATA.Models;
using VecompSoftware.BPM.Integrations.Modules.SM.ACATA.ObjectService;
using VecompSoftware.BPM.Integrations.Modules.SM.ACATA.Services;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.BPM.Integrations.Modules.SM.ACATA.Helpers
{
    public class QueryService
    {
        #region [ Fields ]
        private readonly AcarisService _acarisService;
        private readonly RepositoryService.ObjectIdType _repositoryId;
        private readonly BackOfficeService.PrincipalIdType _principalId;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ILogger _logger;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(Execution));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public QueryService(AcarisService acarisService, RepositoryService.ObjectIdType repositoryId, BackOfficeService.PrincipalIdType principalId, ILogger logger)
        {
            _acarisService = acarisService;
            _repositoryId = repositoryId;
            _principalId = principalId;
            _logger = logger;
        }
        #endregion

        #region [ Methods ]
        public string QuerySubjectRegistryServiceFor(string targetType, string propertyName, string propertyValue, string resultPropertyName)
        {
            SubjectRegistryService.QueryableObjectType target = new SubjectRegistryService.QueryableObjectType() { @object = targetType };
            SubjectRegistryService.PropertyFilterType filter = new SubjectRegistryService.PropertyFilterType() { filterType = SubjectRegistryService.enumPropertyFilter.all };
            List<SubjectRegistryService.QueryConditionType> criteria = new List<SubjectRegistryService.QueryConditionType>()
            {
                new SubjectRegistryService.QueryConditionType()
                {
                    propertyName = propertyName,
                    @operator = SubjectRegistryService.enumQueryOperator.equals,
                    value = propertyValue
                }
            };
            string resultValue;
            try
            {
                BackOfficeService.PagingResponseType result = _acarisService.QuerySubjectRegistry(_repositoryId, _principalId, target, filter, criteria.ToArray());
                if (result == null)
                {
                    return string.Empty;
                }
                resultValue = SearchForProperty(result, targetType, resultPropertyName);
                if (resultValue == null)
                {
                    _logger.WriteError(new LogMessage($"[SM.ACATA] --> No result was found for the target {targetType} with the property {propertyName} equals to {propertyName}"), LogCategories);
                    throw new Exception($"Nessun risultato per {targetType} con {propertyName} = {propertyValue}");
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"[SM.ACATA] --> Error when query subject service "), ex, LogCategories);
                throw;
            }
            return resultValue;
        }
        public string QueryBackofficeServiceFor(string targetType, string propertyName, string propertyValue, string resultPropertyName)
        {
            BackOfficeService.QueryableObjectType target = new BackOfficeService.QueryableObjectType() { @object = targetType };
            BackOfficeService.PropertyFilterType filter = new BackOfficeService.PropertyFilterType() { filterType = BackOfficeService.enumPropertyFilter.all };
            List<BackOfficeService.QueryConditionType> criteria = new List<BackOfficeService.QueryConditionType>()
            {
                new BackOfficeService.QueryConditionType()
                {
                    propertyName = propertyName,
                    @operator = BackOfficeService.enumQueryOperator.equals,
                    value = propertyValue
                }
            };
            string resultValue;
            try
            {
                BackOfficeService.PagingResponseType result = _acarisService.Query(_repositoryId, _principalId, target, filter, criteria.ToArray(), null);

                resultValue = SearchForProperty(result, targetType, resultPropertyName);
                if (resultValue == null)
                {
                    _logger.WriteError(new LogMessage($"[SM.ACATA] --> No result was found for the target {targetType} with the property {propertyName} equals to {propertyName}"), LogCategories);
                    throw new Exception($"Nessun risultato per {targetType } con {propertyName } = { propertyValue}");
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"[SM.ACATA] --> errore in recupero {resultPropertyName } per {targetType}"), ex, LogCategories);
                throw;
            }
            return resultValue;
        }
        public OfficialBookService.ObjectIdType QueryNodeByCode(string code)
        {
            BackOfficeService.QueryableObjectType target = new BackOfficeService.QueryableObjectType() { @object = DocumentService.enumObjectType.NodoPropertiesType.ToString() };
            BackOfficeService.PropertyFilterType filter = new BackOfficeService.PropertyFilterType() { filterType = BackOfficeService.enumPropertyFilter.none };
            List<BackOfficeService.QueryConditionType> criteria = new List<BackOfficeService.QueryConditionType>()
            {
                new BackOfficeService.QueryConditionType()
                {
                    propertyName = "codice",
                    @operator = BackOfficeService.enumQueryOperator.equals,
                    value = code
                },
                new BackOfficeService.QueryConditionType()
                {
                    propertyName = "flagValido",
                    @operator = BackOfficeService.enumQueryOperator.equals,
                    value = "S"
                }
            };
            OfficialBookService.ObjectIdType objectId = new OfficialBookService.ObjectIdType();
            try
            {
                BackOfficeService.PagingResponseType result = _acarisService.Query(_repositoryId, _principalId, target, filter, criteria.ToArray(), null, 5);
                objectId.value = SearchForProperty(result, DocumentService.enumObjectType.NodoPropertiesType.ToString(), "objectId");

                if (objectId.value == null)
                {
                    _logger.WriteError(new LogMessage($"[SM.ACATA] --> Node {code} was not found, or is not valid"), LogCategories);
                    throw new Exception($"Nodo {code} non trovato o non valido");
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"[SM.ACATA] --> Errore in recupero objectId per nodo "), ex, LogCategories);
                throw;
            }
            return objectId;
        }


        public long QuerySubjectMembershipId(RepositoryService.ObjectIdType _repositoryId, BackOfficeService.PrincipalIdType _principalId, string institutionCode)
        {
            SubjectRegistryService.QueryableObjectType target = new SubjectRegistryService.QueryableObjectType() { @object = "TipologiaSoggettoView" };
            SubjectRegistryService.PropertyFilterType filter = new SubjectRegistryService.PropertyFilterType() { filterType = SubjectRegistryService.enumPropertyFilter.all };
            List<SubjectRegistryService.QueryConditionType> criteria = new List<SubjectRegistryService.QueryConditionType>()
            {
                new SubjectRegistryService.QueryConditionType()
                {
                    propertyName = "codiceFonte",
                    @operator = SubjectRegistryService.enumQueryOperator.equals,
                    value = "SA"
                },
                new SubjectRegistryService.QueryConditionType()
                {
                    propertyName = "codiceEnte",
                    @operator = SubjectRegistryService.enumQueryOperator.equals,
                    value = institutionCode
                }
            };

            SubjectRegistryService.ObjectIdType objectId = new SubjectRegistryService.ObjectIdType();
            try
            {
                BackOfficeService.PagingResponseType result = _acarisService.QuerySubjectRegistry(_repositoryId, _principalId, target, filter, criteria.ToArray(), null);
                objectId.value = SearchForProperty(result, "TipologiaSoggettoView", "idTipoSoggetto");
                if (objectId.value == null)
                {
                    _logger.WriteError(new LogMessage($"[SM.ACATA] --> Membership Id was not found"), LogCategories);
                    throw new Exception($"[SM.ACATA] --> Membership id not found");
                }
            }
            catch (Exception)
            {
                _logger.WriteError(new LogMessage($"[SM.ACATA] --> Errore in recupero membership Id"), LogCategories);
                throw;
            }
            return Convert.ToInt64(objectId.value);
        }


        public string QueryObjectServiceFor(string targetType, string propertyName, string propertyValue, string resultPropertyName)
        {
            ObjectService.QueryableObjectType target = new ObjectService.QueryableObjectType() { @object = targetType };
            ObjectService.PropertyFilterType filter = new ObjectService.PropertyFilterType()
            {
                filterType = ObjectService.enumPropertyFilter.list
            };

            ObjectService.QueryNameType[] values = new ObjectService.QueryNameType[1];
            values[0] = new ObjectService.QueryNameType()
            {
                className = targetType,
                propertyName = resultPropertyName
            };

            filter.propertyList = values;

            ObjectService.QueryConditionType[] criteria = new ObjectService.QueryConditionType[1];
            ObjectService.QueryConditionType qct = new ObjectService.QueryConditionType()
            {
                propertyName = propertyName,
                @operator = ObjectService.enumQueryOperator.equals,
                value = propertyValue
            };
            criteria[0] = qct;
            string resultValue = null;

            try
            {
                BackOfficeService.PagingResponseType result = _acarisService.Query(_repositoryId, _principalId, target, filter, criteria, null);
                resultValue = SearchForProperty(result, targetType, resultPropertyName);
                if (resultValue == null)
                {
                    _logger.WriteError(new LogMessage($"[SM.ACATA] -> No result for {targetType} with {propertyName} = {propertyValue}"), LogCategories);
                    throw new Exception($"Nessun risultato per {targetType} con {propertyName} = {propertyValue}");
                }

            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"[SM.ACATA] -> Error getting {resultPropertyName} for {targetType} - {ex.Message}"), LogCategories);

                throw;
            }

            return resultValue;
        }
        public List<DocumentClassification> SearchAttachmentsRegisteredClassification(OfficialBookService.ObjectIdType classificazioneId)
        {
            ObjectService.QueryableObjectType target = new ObjectService.QueryableObjectType() { @object = "ElencoAllegatiAClassificazionePrincipaleView" };
            ObjectService.PropertyFilterType filter = new ObjectService.PropertyFilterType() { filterType = ObjectService.enumPropertyFilter.all };

            List<ObjectService.QueryConditionType> criteria = new List<ObjectService.QueryConditionType>()
            {
                new ObjectService.QueryConditionType()
                {
                      propertyName = "dbKeyClassificazionePrincipale",
                      @operator = ObjectService.enumQueryOperator.equals,
                      value = QueryObjectServiceFor(enumFolderObjectType.ClassificazionePropertiesType.ToString(), "objectId", classificazioneId.value, "dbKey")
                }
            };

            List<DocumentClassification> attachments = new List<DocumentClassification>();
            try
            {
                BackOfficeService.PagingResponseType result = _acarisService.QueryObjectService(_repositoryId, _principalId, target, filter, criteria.ToArray(), null, 5);

                if (result != null && result.objects.Length > 0)
                {
                    for (int i = 0; i < result.objects.Length; i++)
                    {
                        DocumentClassification attachment = new DocumentClassification()
                        {
                            ObjectIdClassificazione = SearchForProperty(result.objects[i], enumObjectType.ClassificazionePropertiesType.ToString(), "objectIdClassificazione"),
                            ObjectIdDocumento = SearchForProperty(result.objects[i], enumObjectType.DocumentoSemplicePropertiesType.ToString(), "objectIdDocumento"),
                        };

                        attachment.OggettoDocumento = QueryObjectServiceFor(enumObjectType.DocumentoSemplicePropertiesType.ToString(), "objectId", attachment.ObjectIdDocumento, "oggetto");
                        attachments.Add(attachment);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore in recupero objectId allegati a classificazione  {classificazioneId} - {ex.Message}");
            }
            return attachments;
        }

        #endregion

        #region [ Helpers ]
        private string SearchForProperty(BackOfficeService.PagingResponseType pagingResponseType, string className, string propertyName)
        {
            return SearchForProperty(pagingResponseType.objects, className, propertyName);
        }
        private string SearchForProperty(BackOfficeService.ObjectResponseType[] objectResponses, string className, string propertyName)
        {
            string result = null;
            foreach (BackOfficeService.ObjectResponseType ort in objectResponses)
            {
                result = SearchForProperty(ort, className, propertyName);
                if (result != null)
                {
                    break;
                }
            }
            return result;
        }
        private string SearchForProperty(BackOfficeService.ObjectResponseType ort, string className, string propertyName)
        {
            string result = null;
            for (int i = 0; i < ort.properties.Length; i++)
            {
                BackOfficeService.PropertyType pt = ort.properties[i];
                if (pt.queryName.className.Equals(className) && pt.queryName.propertyName.Equals(propertyName))
                {
                    result = pt.value[0];
                    break;
                }
            }
            return result;
        }
        #endregion
    }
}
