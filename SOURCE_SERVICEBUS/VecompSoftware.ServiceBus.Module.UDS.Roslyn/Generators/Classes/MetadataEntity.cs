using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.Helpers.UDS;
using VecompSoftware.ServiceBus.Module.UDS.Storage;

namespace VecompSoftware.ServiceBus.Module.UDS.Roslyn.Generators.Classes
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class MetadataEntity
    {
        #region [ Properties ]
        private static ILogger _log { get; set; }
        protected static IEnumerable<LogCategory> _logCategories = null;
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(CreateUDSClass));
                }
                return _logCategories;
            }
        }
        #endregion
        public MetadataEntity(ILogger log)
        {
            _log = log;
        }
        private Type GetMetadataPropertyType(FieldBaseType element)
        {
            if (element is TextField || element is LookupField || element is EnumField || element is StatusField)
            {
                return typeof(string);
            }
            if (element is BoolField)
            {
                return typeof(bool);
            }
            if (element is DateField)
            {
                return typeof(DateTimeOffset);
            }
            if (element is NumberField)
            {
                return typeof(double);
            }
            return null;
        }

        private Type GetMetadataBiblosType(FieldBaseType element)
        {
            if (element is TextField || element is LookupField || element is EnumField || element is StatusField)
            {
                return typeof(string);
            }
            if (element is BoolField)
            {
                return typeof(long);
            }
            if (element is DateField)
            {
                return typeof(DateTime);
            }
            if (element is NumberField)
            {
                return typeof(double);
            }
            return null;
        }
        /// <summary>
        /// Carica le informazioni sui metadati inseriti nell'entità principale
        /// </summary>
        /// <param name="storage"></param>
        /// <returns></returns>
        public UDSEntity LoadMetadata(UDSStorageFacade storage)
        {
            UDSEntity entity = new UDSEntity
            {
                TableName = storage.Builder.UDSTableName,
                Namespace = storage.Builder.UDSTableNameWithoutPrefix
            };
            Type metadataBiblosType;
            Type metadataPropertyType;

            foreach (Section sezione in storage.UDS.Model.Metadata.ToList())
            {
                foreach (FieldBaseType element in sezione.Items)
                {
                    metadataPropertyType = GetMetadataPropertyType(element);
                    metadataBiblosType = GetMetadataBiblosType(element);
                    _log.WriteInfo(new LogMessage(string.Concat("Nuovo metadato individuato in ", entity.TableName, " di tipo ", metadataPropertyType.Name, " e colonna ", element.ColumnName)), LogCategories);
                    if (metadataPropertyType != null && metadataBiblosType != null)
                    {
                        entity.MetaData.Add(new Metadata()
                        {
                            PropertyType = metadataPropertyType,
                            Nullable = metadataPropertyType != typeof(string),
                            BiblosPropertyType = metadataBiblosType,
                            PropertyName = element.ColumnName,
                            Required = element.Required,
                        });
                    }
                    else
                    {
                        _log.WriteWarning(new LogMessage(string.Concat("Metadato ", element.ColumnName, " scartato per la tabella ", entity.TableName, " in quanto non valido : ", element.GetType().Name)), LogCategories);
                    }

                }
            }
            return entity;
        }
    }
}
