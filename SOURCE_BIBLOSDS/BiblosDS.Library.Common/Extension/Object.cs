using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data.Objects.DataClasses;
using System.Data;
using System.Collections;
using BiblosDS.Library.Common.Extension;
using System.Data.Objects;
using System.Linq.Expressions;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data.Metadata.Edm;
using BiblosDS.Library.Common.Objects;
using System.ComponentModel;

using BibDSModel = BiblosDS.Library.Common.Model;
using BiblosDS.Library.Common.Enums;

namespace System
{
    public static class IQueryableEx
    {

        internal static Document Convert(this BibDSModel.Document doc, int level = 0, int deepLevel = 5)
        {
            return doc.Convert(level, deepLevel, typeof(Preservation), typeof(DocumentCache));
        }

        internal static Document Convert(this BibDSModel.Document doc, int level, int deepLevel, params Type[] ignoredTypes)
        {
            if (doc == null || level > deepLevel)
                return null;

            var result = new Document
            {
                IdBiblos = doc.IdBiblos,
                IdDocument = doc.IdDocument,
                Archive = Convert(doc.Archive),
                AttributeValues = Convert(doc.AttributesValue, level + 1, deepLevel),
                Certificate = Convert(doc.CertificateStore, level + 1, deepLevel),
                ChainOrder = doc.ChainOrder,
                DateCreated = doc.DateCreated,
                DateExpire = doc.DateExpire,
                DateMain = doc.DateMain,
                DocumentHash = doc.DocumentHash,
                //DocumentLink = Convert(doc.DocumentLink, level + 1, deepLevel),
                //DocumentParent = Convert(doc.DocumentParent, level + 1, deepLevel),
                //DocumentParentVersion = Convert(doc.DocumentParentVersion, level + 1, deepLevel),
                FullSign = doc.FullSign,
                IdUserCheckOut = doc.IdUserCheckOut,
                IsCheckOut = Convert(doc.IsCheckOut),
                IsConfirmed = ConvertNull(doc.IsConfirmed),
                IsConservated = Convert(doc.IsConservated),
                IsLinked = Convert(doc.IsLinked),
                IsVisible = Convert(doc.IsVisible),
                Name = doc.Name,
                NodeType = Convert(doc.DocumentNodeType, level + 1, deepLevel),
                Permissions = Convert(doc.Permission),
                PrimaryKeyValue = doc.PrimaryKeyValue,
                SignHeader = doc.SignHeader,
                Size = doc.Size,
                Status = Convert(doc.DocumentStatus, level + 1, deepLevel),
                Storage = Convert(doc.Storage, level + 1, deepLevel),
                StorageArea = Convert(doc.StorageArea, level + 1, deepLevel),
                StorageVersion = doc.StorageVersion,
                IsDetached = doc.IsDetached,
                Version = doc.Version,
                IdPreservation = doc.PreservationDocuments?.SingleOrDefault()?.IdPreservation,
                PreservationIndex = doc.PreservationDocuments?.SingleOrDefault()?.PreservationIndex,
                IdThumbnail = doc.IdThumbnail,
                IdPdf = doc.IdPdf,
                IsLatestVersion = doc.IsLatestVersion,
                IdArchiveCertificate = doc.IdArchiveCertificate,
                IdAwardBatch = doc.IdAwardBatch,
                IsRemoved = (doc.IsDetached.HasValue && doc.IsDetached.Value) || doc.IdDocumentStatus == (int)DocumentStatus.RemovedFromStorage
        };

            //
            //TODO: Attributes ed AttributesValue andranno gestiti con la lista dei tipi da ignorare (convertendo il Document associato ad essi si perderanno le informazioni legate alla Preservation)
            //

            if (ignoredTypes != null && !ignoredTypes.Contains(typeof(Preservation)))
                result.Preservation = doc.PreservationDocuments?.SingleOrDefault()?.Preservation.Convert(level + 1, deepLevel);
            if (ignoredTypes != null && !ignoredTypes.Contains(typeof(DocumentCache)))
                result.Cache = Convert(doc.DocumentCache);
            result.DocumentLink = Convert(doc.DocumentLink, level + 1, deepLevel, ignoredTypes);
            if (doc.IdDocumentLink.HasValue && result.DocumentLink == null)
                result.DocumentLink = new Document(doc.IdDocumentLink.Value);
            result.DocumentParent = Convert(doc.DocumentParent, level + 1, deepLevel, ignoredTypes);
            if (doc.IdParentBiblos.HasValue && result.DocumentParent == null)
                result.DocumentParent = new Document(doc.IdParentBiblos.Value);
            result.DocumentParentVersion = Convert(doc.DocumentParentVersion, level + 1, deepLevel, ignoredTypes);
            if (doc.IdParentVersion.HasValue && result.DocumentParentVersion == null)
                result.DocumentParentVersion = new Document(doc.IdParentVersion.Value);
            return result;
        }

        internal static BindingList<DocumentCache> Convert(this IEnumerable<BibDSModel.DocumentCache> cache)
        {
            BindingList<DocumentCache> items = new BindingList<DocumentCache>();
            foreach (var item in cache)
            {
                items.Add(new DocumentCache { ServerName = item.ServerName, FileName = item.FileName, Signature = item.Signature, IdDocument = item.IdDocument });
            }
            return items;
        }

        internal static BindingList<DocumentPermission> Convert(this IEnumerable<BibDSModel.Permission> permissions)
        {
            return null;
        }

        internal static DocumentStorageArea Convert(this BibDSModel.StorageArea area, int level = 0, int deepLevel = 5)
        {
            if (area == null || level > deepLevel)
                return null;
            return new DocumentStorageArea
            {
                IdStorageArea = area.IdStorageArea,
                CurrentFileNumber = area.CurrentFileNumber,
                CurrentSize = area.CurrentSize.HasValue ? area.CurrentSize.Value : 0,
                Enable = Convert(area.Enable),
                MaxFileNumber = area.MaxFileNumber,
                MaxSize = area.MaxSize,
                Name = area.Name,
                Path = area.Path,
                Priority = area.Priority.HasValue ? area.Priority.Value : 0,
                Status = Convert(area.StorageStatus, level + 1, deepLevel),
                Storage = Convert(area.Storage, level + 1, deepLevel),
                Archive = area.Archive != null ? Convert(area.Archive, level + 1, deepLevel) : area.IdArchive.HasValue ? new DocumentArchive(area.IdArchive.GetValueOrDefault()) : null
            };
        }

        internal static Status Convert(this BibDSModel.DocumentStatus status, int level = 0, int deepLevel = 5)
        {
            if (status == null || level > deepLevel)
                return null;
            return new Status { IdStatus = status.IdDocumentStatus, Description = status.Description };
        }

        internal static Status Convert(this BibDSModel.StorageStatus status, int level = 0, int deepLevel = 5)
        {
            if (status == null || level > deepLevel)
                return null;
            return new Status { IdStatus = status.IdStorageStatus, Description = status.Status };
        }

        internal static DocumentNodeType Convert(this BibDSModel.DocumentNodeType nodeType, int level = 0, int deepLevel = 5)
        {
            if (nodeType == null || level > deepLevel)
                return null;
            return new DocumentNodeType { IdNodeType = nodeType.Id, Description = nodeType.Description };
        }

        internal static DocumentCertificate Convert(this BibDSModel.CertificateStore cert, int level = 0, int deepLevel = 5)
        {
            if (cert == null || level > deepLevel)
                return null;
            return new DocumentCertificate { IdCertificate = cert.IdCertificate, Name = cert.Name, Password = cert.Password, IsOnDisk = Convert(cert.IsOnDisk), Path = cert.Path };
        }

        internal static DocumentAttribute Convert(this BibDSModel.Attributes attr, int level = 0, int deepLevel = 5)
        {
            if (attr == null || level > deepLevel)
                return null;
            return new DocumentAttribute
            {
                Archive = Convert(attr.Archive, level + 1, deepLevel),
                AttributeGroup = Convert(attr.AttributesGroup, level + 1, deepLevel),
                AttributeType = attr.AttributeType,
                ConservationPosition = attr.ConservationPosition,
                DefaultValue = attr.DefaultValue,
                Format = attr.Format,
                IdAttribute = attr.IdAttribute,
                IsAutoInc = Convert(attr.IsAutoInc),
                IsChainAttribute = Convert(attr.IsChainAttribute),
                IsEnumerator = Convert(attr.IsEnumerator),
                IsMainDate = Convert(attr.IsMainDate),
                IsRequired = Convert(attr.IsRequired),
                IsUnique = Convert(attr.IsUnique),
                IsVisible = Convert(attr.IsVisible),
                IsSectional = attr.IsSectional.GetValueOrDefault(false),
                KeyFilter = attr.KeyFilter,
                KeyFormat = attr.KeyFormat,
                KeyOrder = attr.KeyOrder,
                MaxLenght = attr.MaxLenght,
                Mode = Convert(attr.AttributesMode, level + 1, deepLevel),
                Name = attr.Name,
                Description = attr.Description,
                Validation = attr.Validation,
                IsRequiredForPreservation = Convert(attr.IsRequiredForPreservation),
                IsVisibleForUser = Convert(attr.IsVisibleForUser)
            };
        }

        internal static DocumentAttributeGroup Convert(this BibDSModel.AttributesGroup group, int level = 0, int deepLevel = 5)
        {
            if (group == null || level > deepLevel)
                return null;
            return new DocumentAttributeGroup { IdAttributeGroup = group.IdAttributeGroup, Description = group.Description, IdAttributeType = group.IdAttributeGroupType, IdArchive = group.IdArchive, IsVisible = Convert(group.IsVisible) };
        }

        internal static DocumentAttributeMode Convert(this BibDSModel.AttributesMode mode, int level = 0, int deepLevel = 5)
        {
            if (mode == null || level > deepLevel)
                return null;
            return new DocumentAttributeMode { IdMode = mode.IdMode, Description = mode.Description };
        }


        internal static DocumentArchiveCertificate Convert(this BibDSModel.ArchiveCertificate arc, int level = 0, int deepLevel = 5)
        {
            if (arc == null || level > deepLevel)
                return null;

            return new DocumentArchiveCertificate
            {
                IdArchiveCertificate = arc.IdArchiveCertificate,
                UserName = arc.CertificateUserName,
                Pin = arc.CertificatePin,
                CertificateBlob = System.Convert.FromBase64String(arc.CertificateBase64),
                FileName = arc.CertificateFileName
            };
        }

        internal static DocumentArchive ConvertArchive(this BibDSModel.PreservationTableValuedResult valuedResult)
        {
            if (valuedResult == null || valuedResult.Archive_IdArchive == null)
            {
                return null;
            }

            return new DocumentArchive()
            {
                AuthorizationAssembly = valuedResult.Archive_AuthorizationAssembly,
                AuthorizationClassName = valuedResult.Archive_AuthorizationClassName,
                AutoVersion = Convert(valuedResult.Archive_AutoVersion),
                EnableSecurity = Convert(valuedResult.Archive_EnableSecurity),
                IdArchive = valuedResult.Archive_IdArchive.Value,
                IsLegal = Convert(valuedResult.Archive_IsLegal),
                LastIdBiblos = valuedResult.Archive_LastIdBiblos.GetValueOrDefault(0),
                LowerCache = valuedResult.Archive_LowerCache,
                MaxCache = valuedResult.Archive_MaxCache,
                Name = valuedResult.Archive_Name,
                PathCache = valuedResult.Archive_PathCache,
                PathTransito = valuedResult.Archive_PathTransito,
                PathPreservation = valuedResult.Archive_PathPreservation,
                UpperCache = valuedResult.Archive_UpperCache,
                ThumbnailEmabled = valuedResult.Archive_ThumbnailEnabled.GetValueOrDefault(),
                PdfConversionEmabled = valuedResult.Archive_PdfConversionEnabled.GetValueOrDefault(),
                FullSignEnabled = valuedResult.Archive_FullSignEnabled.GetValueOrDefault(true),
                TransitoEnabled = valuedResult.Archive_TransitoEnabled.GetValueOrDefault(true),
                FiscalDocumentType = valuedResult.Archive_FiscalDocumentType,
                ODBCConnection = valuedResult.Archive_ODBCConnection,
                PreservationConfiguration = valuedResult.Archive_PreservationConfiguration
            };
        }

        internal static DocumentArchive Convert(this BibDSModel.Archive arc, int level = 0, int deepLevel = 5)
        {
            if (arc == null || level > deepLevel)
                return null;
            var serverConf = new BindingList<ArchiveServerConfig>();

            if (arc.ArchiveServerConfig != null)
            {
                foreach (var cfg in arc.ArchiveServerConfig)
                {
                    serverConf.Add(cfg.Convert(level + 1, deepLevel));
                }
            }

            return new DocumentArchive
            {
                AuthorizationAssembly = arc.AuthorizationAssembly,
                AuthorizationClassName = arc.AuthorizationClassName,
                AutoVersion = Convert(arc.AutoVersion),
                EnableSecurity = Convert(arc.EnableSecurity),
                IdArchive = arc.IdArchive,
                IsLegal = Convert(arc.IsLegal),
                LastIdBiblos = arc.LastIdBiblos,
                LowerCache = arc.LowerCache,
                MaxCache = arc.MaxCache,
                Name = arc.Name,
                PathCache = arc.PathCache,
                PathTransito = arc.PathTransito,
                PathPreservation = arc.PathPreservation,
                UpperCache = arc.UpperCache,
                ThumbnailEmabled = arc.ThumbnailEnabled.GetValueOrDefault(),
                PdfConversionEmabled = arc.PdfConversionEnabled.GetValueOrDefault(),
                FullSignEnabled = arc.FullSignEnabled.GetValueOrDefault(true),
                TransitoEnabled = arc.TransitoEnabled.GetValueOrDefault(true),
                FiscalDocumentType = arc.FiscalDocumentType,
                ServerConfigs = serverConf,
                ODBCConnection = arc.ODBCConnection,
                PreservationConfiguration = arc.PreservationConfiguration
            };
        }

        internal static DocumentArchiveStorage Convert(this BibDSModel.ArchiveStorage arc, int level = 0, int deepLevel = 5)
        {
            if (arc == null || level > deepLevel)
                return null;
            return new DocumentArchiveStorage
            {
                Archive = Convert(arc.Archive),
                Active = Convert(arc.Active),
                Storage = Convert(arc.Storage)
            };
        }

        internal static DocumentStorageRule Convert(this BibDSModel.StorageRule rule, int level = 0, int deepLevel = 5)
        {
            if (rule == null || level > deepLevel)
                return null;
            return new DocumentStorageRule
            {
                Attribute = rule.Attributes.Convert(level + 1, deepLevel),
                RuleFilter = rule.RuleFilter,
                RuleFormat = rule.RuleFormat,
                RuleOrder = rule.RuleOrder,
                RuleOperator = rule.RuleOperator.Convert(level + 1, deepLevel),
                Storage = rule.Storage.Convert(level + 1, deepLevel)
            };
        }

        internal static DocumentStorageAreaRule Convert(this BibDSModel.StorageAreaRule rule, int level = 0, int deepLevel = 5)
        {
            if (rule == null || level > deepLevel)
                return null;
            return new DocumentStorageAreaRule
            {
                Attribute = rule.Attributes.Convert(level + 1, deepLevel),
                RuleFilter = rule.RuleFilter,
                RuleFormat = rule.RuleFormat,
                RuleOrder = rule.RuleOrder,
                RuleOperator = rule.RuleOperator.Convert(level + 1, deepLevel),
                StorageArea = rule.StorageArea.Convert(level + 1, deepLevel),
                IsCalculated = rule.IsCalculated.GetValueOrDefault()
            };
        }

        internal static DocumentRuleOperator Convert(this BibDSModel.RuleOperator rule, int level = 0, int deepLevel = 5)
        {
            if (rule == null || level > deepLevel)
                return null;
            return new DocumentRuleOperator
            {
                Descrizione = rule.Descrizione,
                IdRuleOperator = rule.IdRuleOperator
            };
        }

        internal static DocumentStorageType Convert(this BibDSModel.StorageType storType, int level = 0, int deepLevel = 5)
        {
            if (storType == null || level > deepLevel)
                return null;
            return new DocumentStorageType { IdStorageType = storType.IdStorageType, StorageAssembly = storType.StorageAssembly, StorageClassName = storType.StorageClassName, Type = storType.Type };
        }

        internal static DocumentTransito Convert(this BibDSModel.Transit transit, int level = 0, int deepLevel = 5)
        {
            if (transit == null || level > deepLevel)
                return null;
            return new DocumentTransito
            {
                IdTransit = transit.IdTransit,
                IdDocument = transit.IdDocument,
                IdServer = transit.IdServer,
                LocalPath = transit.LocalPath,
                DateRetry = transit.DateRetry.GetValueOrDefault(),
                Status = transit.Status.GetValueOrDefault(),
                Retry = transit.Retry,
                ServerName = transit.ServerName,
                TarnsitoStatus = (BiblosDS.Library.Common.Enums.DocumentTarnsitoStatus)transit.Retry
            };
        }

        internal static DocumentStorage Convert(this BibDSModel.Storage stor, int level = 0, int deepLevel = 5)
        {
            if (stor == null || level > deepLevel)
                return null;
            return new DocumentStorage
            {
                AuthenticationKey = stor.AuthenticationKey,
                AuthenticationPassword = stor.AuthenticationPassword,
                EnableFulText = Convert(stor.EnableFulText),
                IdStorage = stor.IdStorage,
                IsVisible = ConvertNull(stor.IsVisible),
                MainPath = stor.MainPath,
                Name = stor.Name,
                Priority = stor.Priority,
                StorageRuleAssembly = stor.StorageRuleAssembly,
                StorageRuleClassName = stor.StorageRuleClassName,
                StorageType = Convert(stor.StorageType, level + 1, deepLevel),
                Server = stor.Server.Convert(level + 1, deepLevel)
            };
        }

        internal static DocumentAttachTransito Convert(this BibDSModel.DocumentAttachTransit tran, int level = 0, int deepLevel = 5)
        {
            if (tran == null || level > deepLevel)
                return null;
            return new DocumentAttachTransito
            {
                DateRetry = tran.DateRetry.HasValue ? tran.DateRetry.Value : DateTime.MinValue,
                Attach = tran.DocumentAttach.Convert(level + 1, deepLevel),
                IdDocumentAttach = tran.IdDocumentAttach,
                LocalPath = tran.LocalPath,
                Retry = tran.Retry.GetValueOrDefault(),
                Status = tran.Status.GetValueOrDefault(),
            };
        }

        internal static DocumentAttach Convert(this BibDSModel.DocumentAttach att, int level = 0, int deepLevel = 5)
        {
            if (att == null || level > deepLevel)
                return null;

            return new DocumentAttach
            {
                IdDocumentAttach = att.IdDocumentAttach,
                IdDocument = att.IdDocument,
                AttachOrder = att.AttachOrder.GetValueOrDefault(),
                DateCreated = att.DateCreated,
                Document = att.Document.Convert(level + 1, deepLevel),
                Status = att.DocumentStatus.Convert(level + 1, deepLevel),
                Name = att.Name,
                IsVisible = att.IsVisible.GetValueOrDefault() == 1,
                IsConfirmed = att.IsConfirmed.GetValueOrDefault() == 1,
                Size = att.Size,
            };
        }

        internal static Server Convert(this BibDSModel.Server srv, int level = 0, int deepLevel = 5)
        {
            if (srv == null || level > deepLevel)
                return null;

            var role = ServerRole.Undefined;
            //TryParse.
            try
            {
                if (!string.IsNullOrEmpty(srv.ServerRole))
                    role = (ServerRole)Enum.Parse(typeof(ServerRole), srv.ServerRole);
            }
            catch
            {
                throw new Exception("Server Role:" + srv.ServerRole + " not defined.");
            }
            //
            return new Server
            {
                IdServer = srv.IdServer,
                ServerName = srv.ServerName,
                ServerRole = role,
                DocumentServiceUrl = srv.DocumentServiceUrl,
                DocumentServiceBinding = srv.DocumentServiceBinding,
                DocumentServiceBindingConfiguration = srv.DocumentServiceBindingConfiguration,
                StorageServiceUrl = srv.StorageServiceUrl,
                StorageServiceBinding = srv.StorageServiceBinding,
                StorageServiceBindingConfiguration = srv.StorageServiceBindingConfiguration
            };
        }

        internal static DocumentServer Convert(this BibDSModel.DocumentServer cfg, int level = 0, int deepLevel = 5, params Type[] ignoredTypes)
        {
            if (cfg == null || level > deepLevel)
                return null;

            return new DocumentServer
            {
                Document = (ignoredTypes != null && ignoredTypes.Contains(typeof(Document))) ? null : cfg.Document.Convert(level, deepLevel),
                DateCreated = cfg.DateCreated,
                Status = cfg.DocumentStatus.Convert(level, deepLevel),
                Server = cfg.Server.Convert(level, deepLevel),
                Storage = cfg.Storage.Convert(level, deepLevel),
                StorageArea = cfg.StorageArea.Convert(level, deepLevel)
            };
        }

        internal static ArchiveServerConfig Convert(this BibDSModel.ArchiveServerConfig cfg, int level = 0, int deepLevel = 5)
        {
            if (cfg == null || level > deepLevel)
                return null;

            return new ArchiveServerConfig
            {
                IdArchiveServerConfig = cfg.IdArchiveServerConfig,
                Archive = cfg.Archive.Convert(level + 1, deepLevel),
                Server = cfg.Server.Convert(level + 1, deepLevel),
                TransitEnabled = cfg.TransitoEnabled.HasValue && cfg.TransitoEnabled.Value,
                TransitPath = cfg.PathTransito,
            };
        }

        internal static FileTableModel Convert(this BibDSModel.FileTableModel model, int level = 0, int deepLevel = 5)
        {
            if (model == null || level > deepLevel)
                return null;

            return new FileTableModel
            {
                StreamId = model.stream_id,
                FileStream = model.file_stream,
                Name = model.name,
                Path = model.path
            };
        }

        internal static bool? ConvertNull(short? bit)
        {
            if (!bit.HasValue)
                return null;
            return bit.Value == (short)1;
        }

        internal static bool Convert(short? bit)
        {
            if (!bit.HasValue)
                return false;
            return bit.Value == (short)1;
        }

        internal static DocumentAttributeValue Convert(this BibDSModel.AttributesValue attr, int level = 0, int deepLevel = 5)
        {
            if (attr == null || level > deepLevel)
                return null;
            var attributeValueItem = new DocumentAttributeValue { IdAttribute = attr.IdAttribute, Attribute = Convert(attr.Attributes, level + 1, deepLevel) };
            switch (attr.Attributes.AttributeType)
            {
                case "System.String":
                    attributeValueItem.Value = attr.ValueString;
                    break;
                case "System.Int64":
                    attributeValueItem.Value = attr.ValueInt;
                    break;
                case "System.Double":
                    attributeValueItem.Value = attr.ValueFloat;
                    break;
                case "System.DateTime":
                    attributeValueItem.Value = attr.ValueDateTime;
                    break;
            }
            return attributeValueItem;
        }

        internal static BindingList<DocumentAttributeValue> Convert(this IEnumerable<BibDSModel.AttributesValue> attrs, int level = 0, int deepLevel = 5)
        {
            BindingList<DocumentAttributeValue> items = new BindingList<DocumentAttributeValue>();
            if (attrs != null)
            {
                foreach (var item in attrs)
                {

                    items.Add(item.Convert(level, deepLevel));
                }
            }
            return items;
        }

        internal static DocumentAttributeValue Convert(this BibDSModel.AttributeValueTableValuedResult valuedResult)
        {
            if (valuedResult == null)
            {
                return null;
            }
            var attributeValueItem = new DocumentAttributeValue { IdAttribute = valuedResult.AttributeValues_IdAttribute };            
            attributeValueItem.Attribute = new DocumentAttribute()
            {
                AttributeType = valuedResult.Attributes_AttributeType,
                ConservationPosition = valuedResult.Attributes_ConservationPosition,
                DefaultValue = valuedResult.Attributes_DefaultValue,
                Format = valuedResult.Attributes_Format,
                IdAttribute = valuedResult.Attributes_IdAttribute,
                IsAutoInc = Convert(valuedResult.Attributes_IsAutoInc),
                IsChainAttribute = Convert(valuedResult.Attributes_IsChainAttribute),
                IsEnumerator = Convert(valuedResult.Attributes_IsEnumerator),
                IsMainDate = Convert(valuedResult.Attributes_IsMainDate),
                IsRequired = Convert(valuedResult.Attributes_IsRequired),
                IsUnique = Convert(valuedResult.Attributes_IsUnique),
                IsVisible = Convert(valuedResult.Attributes_IsVisible),
                IsSectional = valuedResult.Attributes_IsSectional.GetValueOrDefault(false),
                KeyFilter = valuedResult.Attributes_KeyFilter,
                KeyFormat = valuedResult.Attributes_KeyFormat,
                KeyOrder = valuedResult.Attributes_KeyOrder,
                MaxLenght = valuedResult.Attributes_MaxLenght,
                Name = valuedResult.Attributes_Name,
                Description = valuedResult.Attributes_Description,
                Validation = valuedResult.Attributes_Validation,
                IsRequiredForPreservation = Convert(valuedResult.Attributes_IsRequiredForPreservation),
                IsVisibleForUser = Convert(valuedResult.Attributes_IsVisibleForUser)
            };

            switch (valuedResult.Attributes_AttributeType)
            {
                case "System.String":
                    attributeValueItem.Value = valuedResult.AttributeValues_ValueString;
                    break;
                case "System.Int64":
                    attributeValueItem.Value = valuedResult.AttributeValues_ValueInt;
                    break;
                case "System.Double":
                    attributeValueItem.Value = valuedResult.AttributeValues_ValueFloat;
                    break;
                case "System.DateTime":
                    attributeValueItem.Value = valuedResult.AttributeValues_ValueDateTime;
                    break;
            }
            return attributeValueItem;
        }

        internal static Company Convert(this BibDSModel.Company com, int level = 0, int deepLevel = 5)
        {
            if (com == null || level > deepLevel)
                return null;

            return new Company
            {
                Address = com.Address,
                CompanyName = com.CompanyName,
                FiscalCode = com.FiscalCode,
                //IdArchive = ?
                IdCompany = com.IdCompany,
                PECEmail = com.PECEmail,
                TemplateADEFile = com.TemplateADEFile,
                TemplateCloseFile = com.TemplateCloseFile,
                TemplateIndexFile = com.TemplateIndexFile,
            };
        }


        internal static AwardBatch Convert(this BibDSModel.AwardBatch batch, int level = 0, int deepLevel = 5)
        {
            if (batch == null || level > deepLevel)
                return null;

            return new AwardBatch
            {
                DateFrom = batch.DateFrom,
                DateTo = batch.DateTo,
                IdArchive = batch.IdArchive,
                IdAwardBatch = batch.IdAwardBatch,
                IdParentBatch = batch.IdParentBatch,
                IsAuto = batch.IsAuto == 1,
                IsOpen = batch.IsOpen == 1,
                Name = batch.Name,
                IdPDVDocument = batch.IdPDVDocument,
                IdRDVDocument = batch.IdRDVDocument,
                IsRDVSigned = batch.IsRDVSigned
            };
        }


        internal static DocumentUnit Convert(this BibDSModel.DocumentUnit docUnit, int level = 0, int deepLevel = 5)
        {
            if (docUnit == null || level > deepLevel)
                return null;

            return new DocumentUnit
            {
                Classification = docUnit.Classification,
                CloseDate = docUnit.CloseDate,
                IdDocumentUnit = docUnit.IdDocumentUnit,
                Identifier = docUnit.Identifier,
                InsertDate = docUnit.InsertDate,
                Subject = docUnit.Subject,
                UriFascicle = docUnit.UriFascicle,
                XmlDoc = docUnit.XmlDoc
            };
        }


        internal static DocumentUnitChain Convert(this BibDSModel.DocumentUnitChain docChain, int level = 0, int deepLevel = 5)
        {
            if (docChain == null || level > deepLevel)
                return null;

            return new DocumentUnitChain
            {
                IdDocumentUnit = docChain.IdDocumentUnit,
                IdParentBiblos = docChain.IdParentBiblos,
                Name = docChain.Name,
                Document = docChain.Document.Convert(),
                DocumentUnit = docChain.DocumentUnit.Convert()
            };
        }


        internal static DocumentUnitAggregate Convert(this BibDSModel.DocumentUnitAggregate docAggregate, int level = 0, int deepLevel = 5)
        {
            if (docAggregate == null || level > deepLevel)
                return null;

            return new DocumentUnitAggregate
            {
                AggregationType = docAggregate.AggregationType,
                CloseDate = docAggregate.CloseDate,
                IdAggregate = docAggregate.IdAggregate,
                PreservationDate = docAggregate.PreservationDate,
                XmlFascicle = docAggregate.XmlFascicle
            };
        }


        internal static void TryToAttach(this EntityReference objSource, EntityObject entity, BiblosDS.Library.Common.Model.BiblosDS2010Entities db)
        {
            objSource.TryToAttach(entity, null, db);

        }

        internal static void TryToAttach(this EntityReference objSource, EntityObject entity, EntityReference referenceOriginal, BiblosDS.Library.Common.Model.BiblosDS2010Entities db)
        {
            entity.EntityKey = db.CreateEntityKey(entity.GetType().Name, entity);
            objSource.EntityKey = entity.EntityKey;
            if (referenceOriginal != null)
                referenceOriginal.EntityKey = entity.EntityKey;
        }

        public static T TryToConvertTo<T>(this object objSource, ObjectContext db, bool dbSource)
        {
            return objSource.TryToConvertTo<T>(db, dbSource, true);
        }

        public static T TryToConvertTo<T>(this object objSource, ObjectContext db, bool dbSource, bool loadSubObject)
        {
            T retValue = (T)Activator.CreateInstance(typeof(T));
            List<string> boolValues = new List<string>();
            if (DbToBoolMapper.ContainsKey(retValue.GetType().Name))
                boolValues = DbToBoolMapper[retValue.GetType().Name];
            PropertyInfo[] info = objSource.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var item in info)
            {
                string name = item.Name;
                object o = objSource.PublicGetProperty(item.Name);
                if (o == null)
                    continue;
                if (dbSource)
                {
                    if (DbToObjectMapper.ContainsKey(name))
                        name = DbToObjectMapper[name];
                    if (boolValues.Contains(name))
                    {
                        if (o != null)
                            o = (short)o == (short)1;
                    }
                    if (o is EntityObject)
                    {
                        //if (!loadSubObject)
                        //    continue;
                        object entity = null;
                        switch (o.GetType().Name)
                        {
                            case "Document":
                                if (loadSubObject)
                                    entity = o.TryToConvertTo<BiblosDS.Library.Common.Objects.Document>(db, true, false);
                                break;
                            case "Archive":
                                entity = o.TryToConvertTo<BiblosDS.Library.Common.Objects.DocumentArchive>(db, true, false);
                                break;
                            case "ArchiveStorage":
                                entity = o.TryToConvertTo<BiblosDS.Library.Common.Objects.DocumentArchiveStorage>(db, true, false);
                                break;
                            case "Attributes":
                                entity = o.TryToConvertTo<BiblosDS.Library.Common.Objects.DocumentAttribute>(db, true, false);
                                break;
                            case "AttributesMode":
                                entity = o.TryToConvertTo<BiblosDS.Library.Common.Objects.DocumentAttributeMode>(db, true, false);
                                break;
                            case "AttributeValue":
                                entity = o.TryToConvertTo<BiblosDS.Library.Common.Objects.DocumentAttributeValue>(db, true, false);
                                break;
                            case "Certificate":
                                entity = o.TryToConvertTo<BiblosDS.Library.Common.Objects.DocumentCertificate>(db, true, false);
                                break;
                            case "Content":
                                entity = o.TryToConvertTo<BiblosDS.Library.Common.Objects.DocumentContent>(db, true, false);
                                break;
                            case "Permission":
                                if (loadSubObject)
                                    entity = o.TryToConvertTo<BiblosDS.Library.Common.Objects.DocumentPermission>(db, true, false);
                                break;
                            case "DocumentStatus":
                                entity = o.TryToConvertTo<BiblosDS.Library.Common.Objects.Status>(db, true, false);
                                break;
                            case "Storage":
                                entity = o.TryToConvertTo<BiblosDS.Library.Common.Objects.DocumentStorage>(db, true, false);
                                break;
                            case "StorageArea":
                                entity = o.TryToConvertTo<BiblosDS.Library.Common.Objects.DocumentStorageArea>(db, true, false);
                                break;
                            case "StorageType":
                                entity = o.TryToConvertTo<BiblosDS.Library.Common.Objects.DocumentStorageType>(db, true, false);
                                break;
                            case "StorageRule":
                                entity = o.TryToConvertTo<BiblosDS.Library.Common.Objects.DocumentStorageRule>(db, true, false);
                                break;
                            case "StorageAreaRule":
                                entity = o.TryToConvertTo<BiblosDS.Library.Common.Objects.DocumentStorageAreaRule>(db, true, false);
                                break;
                            case "RuleOperator":
                                entity = o.TryToConvertTo<BiblosDS.Library.Common.Objects.DocumentRuleOperator>(db, true, false);
                                break;
                            case "CertificateStore":
                                entity = o.TryToConvertTo<BiblosDS.Library.Common.Objects.DocumentCertificate>(db, true, false);
                                break;
                            case "AttributesGroup":
                                entity = o.TryToConvertTo<BiblosDS.Library.Common.Objects.DocumentAttributeGroup>(db, true, false);
                                break;
                            default:
                                entity = o;
                                break;
                        }
                        retValue.PublicSetProperty(name, entity);
                    }
                    else if (typeof(System.Data.Objects.DataClasses.EntityCollection<>).Name == o.GetType().Name)
                    {
                        if (loadSubObject && o is EntityCollection<BibDSModel.AttributesValue>)
                        {
                            BindingList<DocumentAttributeValue> items = new BindingList<DocumentAttributeValue>();
                            foreach (var c in (EntityCollection<BibDSModel.AttributesValue>)o)
                            {
                                var attributeValueItem = new DocumentAttributeValue();
                                if (c.Attributes == null)
                                    continue;
                                attributeValueItem.Attribute = c.Attributes.TryToConvertTo<DocumentAttribute>(db, true, false);

                                switch (c.Attributes.AttributeType)
                                {
                                    case "System.String":
                                        attributeValueItem.Value = c.ValueString;
                                        break;
                                    case "System.Int64":
                                        attributeValueItem.Value = c.ValueInt;
                                        break;
                                    case "System.Double":
                                        attributeValueItem.Value = c.ValueFloat;
                                        break;
                                    case "System.DateTime":
                                        attributeValueItem.Value = c.ValueDateTime;
                                        break;
                                }
                                items.Add(attributeValueItem);
                            }
                            retValue.PublicSetProperty(name, items);
                        }
                    }
                    else
                        retValue.PublicSetProperty(name, o);
                }
                else
                {
                    if (DbToObjectMapper.ContainsValue(name))
                        name = DbToObjectMapper.Where(x => x.Value == name).FirstOrDefault().Key;
                    if (boolValues.Contains(name))
                    {
                        if (o != null)
                            o = (bool)o ? (short)1 : (short)0;
                    }

                    if (o is DocumentContent)
                        continue;
                    else if (typeof(T).Name == "Attributes")
                    {
                        if (name == "Editable" || name == "Disabled")
                            continue;
                    }
                    object entity = o;

                    if (entity is EntityObject)
                    {
                        //EntityKey key = db.CreateEntityKey(entity.GetType().Name, entity);
                        //object objInstance = null;
                        //if (db.TryGetObjectByKey(key, out objInstance))
                        //{
                        //    if (objInstance is EntityObject && (objInstance as EntityObject).EntityState != EntityState.Added)
                        //    {
                        //        retValue.PublicSetProperty(name, objInstance);
                        //    }
                        //}
                        //else
                        //    retValue.PublicSetProperty(name, entity);
                    }
                    if (entity is BiblosDSObject)
                    {
                        continue;
                    }
                    else if (entity is IBindingList)
                    {

                    }
                    else
                        retValue.PublicSetProperty(name, entity);
                }
            }
            return retValue;
        }


        public static void AttachUpdated(this ObjectContext context, EntityObject objectDetached)
        {

            if (objectDetached.EntityState == EntityState.Detached)
            {

                object currentEntityInDb = null;
                if (objectDetached.EntityKey == null)
                    objectDetached.EntityKey = context.CreateEntityKey(objectDetached.GetType().Name, objectDetached);
                if (context.TryGetObjectByKey(objectDetached.EntityKey, out currentEntityInDb))
                {
                    context.ApplyCurrentValues(objectDetached.EntityKey.EntitySetName, objectDetached);
                    //(CDLTLL)Apply property changes to all referenced entities in context 
                    context.ApplyReferencePropertyChanges((IEntityWithRelationships)objectDetached, (IEntityWithRelationships)currentEntityInDb); //Custom extensor method 

                }

                else
                {

                    throw new ObjectNotFoundException();

                }

            }

        }

        public static T TryToConvertTo<T>(this object objSource, bool dbSource)
        {
            return TryToConvertTo<T>(objSource, null, dbSource);
        }

        public static T TryToConvertTo<T>(this object objSource, ObjectContext db)
        {
            return TryToConvertTo<T>(objSource, db, false);
        }

        public static void ApplyReferencePropertyChanges(this ObjectContext context, IEntityWithRelationships newEntity, IEntityWithRelationships oldEntity)
        {
            foreach (var relatedEnd in oldEntity.RelationshipManager.GetAllRelatedEnds())
            {

                var oldRef = relatedEnd as EntityReference;

                if (oldRef != null)
                {
                    // this related end is a reference not a collection 
                    var newRef = newEntity.RelationshipManager.GetRelatedEnd(oldRef.RelationshipName, oldRef.TargetRoleName) as EntityReference;
                    oldRef.EntityKey = newRef.EntityKey;
                }

            }

        }


        private static Dictionary<string, List<string>> DbToBoolMapper
        {
            get
            {
                Dictionary<string, List<string>> mapper = new Dictionary<string, List<string>>();
                mapper.Add("DocumentTransito", new List<string> { "Mode" });
                mapper.Add("Transito", new List<string> { "Mode" });
                mapper.Add("Archive", new List<string> { "IsLegal", "EnableSecurity", "AutoVersion" });
                mapper.Add("Permission", new List<string> { "IsGroup" });
                mapper.Add("DocumentPermission", new List<string> { "IsGroup" });
                mapper.Add("DocumentArchive", new List<string> { "IsLegal", "EnableSecurity", "AutoVersion" });
                mapper.Add("Attributes", new List<string> { "IsVisible", "IsChainAttribute", "IsUnique", "IsRequired", "IsRequiredForPreservation", "IsMainDate", "IsEnumerator", "IsAutoInc", "IsRequiredForPreservation", "IsVisibleForUser" });
                mapper.Add("DocumentAttribute", new List<string> { "IsVisible", "IsChainAttribute", "IsUnique", "IsRequired", "IsRequiredForPreservation", "IsMainDate", "IsEnumerator", "IsAutoInc", "IsRequiredForPreservation", "IsVisibleForUser" });
                mapper.Add("DocumentAttributeGroup", new List<string> { "IsVisible", "IsChainAttribute", "IsUnique", "IsRequired", "IsMainDate", "IsEnumerator", "IsAutoInc", "ConservationPosition" });
                mapper.Add("Document", new List<string> { "IsVisible", "IsCheckOut", "IsConfirmed", "IsConservated", "IsLinked", "IsVisible" });
                mapper.Add("DocumentStorageArea", new List<string> { "Enable" });
                mapper.Add("StorageArea", new List<string> { "Enable" });
                mapper.Add("DocumentStorage", new List<string> { "EnableFulText", "IsVisible" });
                mapper.Add("Storage", new List<string> { "EnableFulText", "IsVisible" });
                mapper.Add("DocumentArchiveStorage", new List<string> { "Active" });
                mapper.Add("ArchiveStorage", new List<string> { "Active" });
                mapper.Add("AttributesGroup", new List<string> { "IsVisible" });
                mapper.Add("AttributeGroup", new List<string> { "IsVisible" });
                return mapper;
            }
        }

        private static Dictionary<string, string> DbToObjectMapper
        {
            get
            {
                Dictionary<string, string> mapper = new Dictionary<string, string>();
                mapper.Add("Attributes", "Attribute");
                mapper.Add("AttributesMode", "Mode");
                mapper.Add("AttributesGroup", "AttributeGroup");
                mapper.Add("PermissionMode", "Mode");
                mapper.Add("DocumentStatus", "Status");
                mapper.Add("NodeType", "DocumentNodeType");
                mapper.Add("StorageAreaRule", "DocumentStorageRule");
                mapper.Add("Permission", "Permissions");
                mapper.Add("AttributesValue", "AttributeValues");
                mapper.Add("CertificateStore", "Certificate");
                mapper.Add("IdDocumentStatus", "IdStatus");
                return mapper;
            }
        }
    }


    public static class ObjectEx
    {
        static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ObjectEx));
        /// <summary>
        /// Prova a convertire l'oggetto nel tipo specificato dal generic parameter
        /// </summary>
        /// <typeparam name="T">Tipo a cui convertire l'oggetto</typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T TryConvert<T>(this Object obj)
        {
            logger.DebugFormat("TryConvert obj: type '{0}' ", obj != null ? obj.GetType() : null);
            logger.DebugFormat("TryConvert obj: {0}", obj);
            T retValue = default(T);

            if (obj != null)
                retValue = obj.TryConvert<T>(retValue);

            logger.DebugFormat("TryConvert ret obj: {0}", retValue);
            return retValue;
        }

        /// <summary>
        /// Prova a convertire l'oggetto nel tipo specificato dal generic parameter
        /// </summary>
        /// <typeparam name="T">Tipo a cui convertire l'oggetto</typeparam>
        /// <param name="obj"></param>
        /// <param name="defValue">Valore da assegnare nel caso in cui la conversione non riesca</param>
        /// <returns></returns>
        public static T TryConvert<T>(this Object obj, T defValue)
        {
            T retValue = defValue;

            if (obj != null)
            {
                try
                {
                    retValue = (T)obj.TryConvert(typeof(T));
                }
                catch
                {
                }
            }

            return retValue;
        }

        //public static Object TryConvert<T>(this Object obj)
        //{
        //    if (obj == null)
        //        return default(T);

        //    return obj.TryConvert(typeof(T));
        //}

        /// <summary>
        /// Prova a convertire l'oggetto nel tipo specificato dal generic parameter
        /// Nota: se il tipo a cui desideri convertire l'oggetto è noto in fase di compilazione invoca il metodo generico!
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="newType">Tipo a cui convertire l'oggetto</param>
        /// <returns></returns>
        public static Object TryConvert(this Object obj, Type newType)
        {
            Object objConverted = null;

            if (obj == null)
                return null;

            if (obj.IsTypeOf(newType))
                objConverted = obj;
            else
            {
                try
                {
                    if (newType.IsTypeOf(typeof(Guid)))
                        objConverted = new Guid(obj.ToString());
                    else if (newType == typeof(String))
                        objConverted = obj.ToString();
                    else
                    {
                        try
                        {
                            objConverted = Convert.ChangeType(obj, newType, null /*CultureInfo.CurrentCulture*/);
                        }
                        catch
                        {
                            if (newType.IsNullable())
                                objConverted = Convert.ChangeType(obj, newType.NullableUnderlyingType(), null /*CultureInfo.CurrentCulture*/);
                        }
                    }
                }
                catch
                {
                }
            }

            return objConverted;
        }

        public static Object TryConvert(this Object obj, Type newType, CultureInfo culture)
        {
            Object objConverted = null;

            if (obj == null)
                return null;

            if (obj.IsTypeOf(newType))
                objConverted = obj;
            else
            {
                try
                {
                    if (newType.IsTypeOf(typeof(Guid)))
                        objConverted = new Guid(obj.ToString());
                    else if (newType == typeof(String))
                        objConverted = obj.ToString();
                    else
                    {
                        try
                        {
                            objConverted = Convert.ChangeType(obj, newType, culture);
                        }
                        catch
                        {
                            if (newType.IsNullable())
                                objConverted = Convert.ChangeType(obj, newType.NullableUnderlyingType(), culture);
                        }
                    }
                }
                catch
                {
                }
            }

            return objConverted;
        }
    }

    public static class TypeEx
    {
        public const BindingFlags cstBindingFlagsDefault = BindingFlags.Default;
        public const BindingFlags cstBindingFlagsExtended = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.IgnoreCase;

        public static bool IsTypeOf(this Object obj, Type type)
        {
            if (obj == null ||
                type == null)
                return false;

            return obj.GetType().IsTypeOf(type);
        }

        public static bool IsTypeOf(this Type objType, Type type)
        {
            if (objType == type)
                return true;

            if (objType == null ||
                type == null)
                return false;

            if (type.IsInterface)
                return objType.GetInterface(type.FullName, true) != null;
            else if (type.IsClass)
                return (/*objType == type || */objType.IsSubclassOf(type));
            /*
            else if (type.IsPrimitive || type.IsValueType)
                return objType == type;
            */

            return false;
        }

        public static Type NullableType(this Type T)
        {
            if (T.IsValueType)
            {
                switch (T.FullName)
                {
                    case "System.Boolean":
                        {
                            return typeof(Nullable<Boolean>);
                        }
                    case "System.Byte":
                        {
                            return typeof(Nullable<Byte>);
                        }
                    case "System.Char":
                        {
                            return typeof(Nullable<Char>);
                        }
                    case "System.DateTime":
                        {
                            return typeof(Nullable<DateTime>);
                        }
                    case "System.Decimal":
                        {
                            return typeof(Nullable<Decimal>);
                        }
                    case "System.Double":
                        {
                            return typeof(Nullable<Double>);
                        }
                    case "System.Int16":
                        {
                            return typeof(Nullable<Int16>);
                        }
                    case "System.Int32":
                        {
                            return typeof(Nullable<Int32>);
                        }
                    case "System.Int64":
                        {
                            return typeof(Nullable<Int64>);
                        }
                    case "System.SByte":
                        {
                            return typeof(Nullable<SByte>);
                        }
                    case "System.Single":
                        {
                            return typeof(Nullable<Single>);
                        }
                    case "System.UInt16":
                        {
                            return typeof(Nullable<UInt16>);
                        }
                    case "System.UInt32":
                        {
                            return typeof(Nullable<UInt32>);
                        }
                    case "System.UInt64":
                        {
                            return typeof(Nullable<UInt64>);
                        }
                }
            }

            return T;
        }

        public static Type NullableUnderlyingType(this Type T)
        {
            Type type = Nullable.GetUnderlyingType(T);

            if (type != null)
                return type;

            return T;
        }

        public static Boolean IsNullable(this Type T)
        {
            return T == NullableType(T);
        }

        public static Type ResolveType(this String typeName)
        {
            Type type = null;
            Boolean bNullable = false;

            if (String.IsNullOrEmpty(typeName))
                return null;

            typeName = typeName.Trim();

            if (typeName.EndsWith("?"))
            {
                bNullable = true;
                typeName = typeName.Replace("?", "");
            }

            type = Type.GetType(typeName);

            if (type == null)
                type = Type.GetType(String.Format("System.{0}", typeName));

            if (type != null &&
                bNullable)
                type = type.NullableType();

            return type;
        }

        public static List<String> GetPropertyPaths(this Type type, String propertyName, Boolean bBreakAtFirst, Int32 maxLevelDepth, BindingFlags bindingFlags)
        {
            List<String> pathList = new List<String>();

            List<Type> typeList = new List<Type>();
            Int32 curLevel = 0;
            List<String> propertyPath = new List<String>();

            type.GetPropertyPaths(pathList, typeList, propertyName, bBreakAtFirst, maxLevelDepth, ref curLevel, ref propertyPath, bindingFlags);

            return pathList;
        }

        private static void GetPropertyPaths(this Type type, List<String> pathList, List<Type> typeList, String propertyName, Boolean bBreakAtFirst, Int32 maxLevelDepth, ref Int32 curLevel, ref List<String> propertyPath, BindingFlags bindingFlags)
        {
            ParameterInfo[] parameters;
            PropertyInfo[] properties;

            PropertyInfo propertyFound;
            StringBuilder strBuilder;

            if (type == null)
                return;

            typeList.Add(type);

            properties = type.GetProperties(bindingFlags).Where(p => p.CanRead).ToArray();
            propertyFound = properties.Where(p => String.Compare(p.Name, propertyName, StringComparison.InvariantCultureIgnoreCase) == 0).FirstOrDefault();

            if (propertyFound != null)
            {
                strBuilder = new StringBuilder();

                foreach (var partialPath in propertyPath)
                {
                    strBuilder.Append(partialPath);
                    strBuilder.Append(".");
                }

                strBuilder.Append(propertyName);
                pathList.Add(strBuilder.ToString());

                if (bBreakAtFirst)
                    return;
            }

            foreach (PropertyInfo property in properties)
            {
                try
                {
                    if ((parameters = property.GetIndexParameters()) == null || parameters.Length == 0)
                    {
                        if (!property.PropertyType.IsValueType &&
                            property.PropertyType != typeof(String) &&
                            !typeList.Contains(property.PropertyType))
                        {
                            if (maxLevelDepth <= 0 ||
                                curLevel < maxLevelDepth)
                            {
                                try
                                {
                                    curLevel++;

                                    propertyPath.Add(property.Name);
                                    property.PropertyType.GetPropertyPaths(pathList, typeList, propertyName, bBreakAtFirst, maxLevelDepth, ref curLevel, ref propertyPath, bindingFlags);

                                    if (pathList.Count > 0 &&
                                        bBreakAtFirst)
                                        break;
                                    else
                                        propertyPath.RemoveAt(propertyPath.Count - 1);
                                }
                                finally
                                {
                                    curLevel--;
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }
    }
}
