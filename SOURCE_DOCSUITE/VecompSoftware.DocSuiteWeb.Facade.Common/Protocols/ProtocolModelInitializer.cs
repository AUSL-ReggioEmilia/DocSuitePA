using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Templates;
using VecompSoftware.DocSuiteWeb.DTO.UDS;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.Helpers;
using VecompSoftware.Helpers.UDS;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.Services.StampaConforme.StampaConformeService;

namespace VecompSoftware.DocSuiteWeb.Facade.Common.Protocols
{
    public class ProtocolModelInitializer
    {
        private TempFileDocumentInfo SaveStream(string owner, byte[] content, string filename)
        {
            return SaveStream(owner, new DocumentModel()
            {
                FileName = filename,
                ContentStream = content,
            });
        }

        private TempFileDocumentInfo SaveStream(string owner, DocumentModel document)
        {
            string fileName = Path.GetFileName(FileHelper.ReplaceUnicode(document.FileName));
            string targetFileName = FileHelper.UniqueFileNameFormat(fileName, owner);
            string targetPath = Path.Combine(CommonUtil.GetInstance().AppTempPath, targetFileName);
            if (document.ContentStream == null && document.DocumentId.HasValue)
            {
                document.ContentStream = new BiblosDocumentInfo(document.DocumentId.Value).Stream;
            }
            FileInfo file = FileHelper.SaveStreamToDisk(targetPath, document.ContentStream);
            return new TempFileDocumentInfo(document.FileName, file);
        }

        private List<DocumentInfo> SaveStream(string owner, IEnumerable<DocumentModel> documents)
        {
            List<DocumentInfo> results = new List<DocumentInfo>();
            if (documents != null && documents.Any())
            {
                foreach (DocumentModel documentModel in documents)
                {
                    results.Add(SaveStream(owner, documentModel));
                }
            }
            return results;
        }

        public ProtocolInitializer GetProtocolInitializer(IReadOnlyCollection<TenantModel> tenantModels, ProtocolModel protocolModel,
            Collaboration collaboration, WorkflowProperty dsw_p_CollaborationSignSummaryTemplateId, WorkflowProperty dsw_a_Collaboration_GenerateSignSummary,
            WorkflowProperty dsw_p_ProposerRole, UDSDto udsDto)
        {
            ProtocolInitializer protInitializer = new ProtocolInitializer();
            // Oggetto
            protInitializer.Subject = protocolModel.Object;
            // Protocol Type
            protInitializer.ProtocolType = protocolModel.ProtocolType.EntityShortId;
            //Note
            protInitializer.Notes = protocolModel.Note;
            //Protocollo
            protInitializer.DocumentProtocol = protocolModel.DocumentProtocol;
            //Date
            protInitializer.DocumentDate = protocolModel.DocumentDate;
            // Classificazione
            if (protocolModel.Category != null && protocolModel.Category.IdCategory.HasValue)
            {
                protInitializer.Category = FacadeFactory.Instance.CategoryFacade.GetById(protocolModel.Category.IdCategory.Value);
            }
            if (protocolModel.Container != null && protocolModel.Container.IdContainer.HasValue)
            {
                protInitializer.Containers = new List<Data.Container> { FacadeFactory.Instance.ContainerFacade.GetById(Convert.ToInt32(protocolModel.Container.IdContainer)) };
            }
            if (protocolModel.DocumentTypeCode != null  )
            {      
                protInitializer.DocumentTypeLabel = FacadeFactory.Instance.TableDocTypeFacade.GetByCode(protocolModel.DocumentTypeCode).Description;
            }
      
            string owner = DocSuiteContext.Current.User.UserName;
            // Gestione documenti        
            if (protocolModel.MainDocument != null && !string.IsNullOrEmpty(protocolModel.MainDocument.FileName)
                && (protocolModel.MainDocument.ContentStream != null || protocolModel.MainDocument.DocumentId.HasValue))
            {
                protInitializer.MainDocument = SaveStream(owner, protocolModel.MainDocument);
            }

            // Allegati
            IEnumerable<DocumentModel> results = null;
            if (protocolModel.Attachments != null && (results = protocolModel.Attachments.Where(f => !string.IsNullOrEmpty(f.FileName) && (f.ContentStream != null || f.DocumentId.HasValue))).Any())
            {
                protInitializer.Attachments = SaveStream(owner, results);
            }

            if (collaboration != null && dsw_p_CollaborationSignSummaryTemplateId != null && dsw_a_Collaboration_GenerateSignSummary != null &&
                dsw_a_Collaboration_GenerateSignSummary.ValueBoolean.HasValue && dsw_a_Collaboration_GenerateSignSummary.ValueBoolean.Value &&
                dsw_p_CollaborationSignSummaryTemplateId.ValueGuid.HasValue && dsw_p_CollaborationSignSummaryTemplateId.ValueGuid.Value != Guid.Empty)
            {
                TemplateDocumentRepository templateDocumentRepository = WebAPIImpersonatorFacade.ImpersonateFinder(new TemplateDocumentRepositoryFinder(tenantModels),
                    (impersonationType, finder) =>
                    {
                        finder.UniqueId = dsw_p_CollaborationSignSummaryTemplateId.ValueGuid.Value;
                        finder.EnablePaging = false;
                        return finder.DoSearch().SingleOrDefault()?.Entity;
                    });

                if (templateDocumentRepository != null)
                {
                    BiblosChainInfo biblosChainInfo = new BiblosChainInfo(templateDocumentRepository.IdArchiveChain);
                    DocumentInfo biblosDocumentInfo = biblosChainInfo.Documents.Single(f => !f.IsRemoved);
                    List<BuildValueModel> buildValueModels = new List<BuildValueModel>();
                    buildValueModels.Add(new BuildValueModel()
                    {
                        IsHTML = false,
                        Name = "oggetto",
                        Value = protInitializer.Subject,
                    });
                    DateTime signDate;
                    string token;
                    foreach (CollaborationSign item in collaboration.CollaborationSigns)
                    {
                        signDate = item.SignDate ?? item.LastChangedDate.Value.DateTime;
                        token = signDate.DayOfWeek == DayOfWeek.Sunday ? "la" : "il";
                        buildValueModels.Add(new BuildValueModel()
                        {
                            IsHTML = false,
                            Name = $"signer_info_{item.Incremental}",
                            Value = $"{item.SignName} {token} {signDate.ToLongDateString()}",
                        });
                    }
                    buildValueModels = BuildValueProposerRole(dsw_p_ProposerRole, buildValueModels);
                    buildValueModels = BuildValueUDS(udsDto, buildValueModels);
                    byte[] pdf = Services.StampaConforme.Service.BuildPDF(biblosDocumentInfo.Stream, buildValueModels.ToArray(), string.Empty);
                    if (protInitializer.Attachments == null)
                    {
                        protInitializer.Attachments = new List<DocumentInfo>();
                    }
                    protInitializer.Attachments.Add(SaveStream(owner, pdf, "riepilogo_firmatari.pdf"));
                }
            }
            // Annessi
            results = null;
            if (protocolModel.Annexes != null && (results = protocolModel.Annexes.Where(f => !string.IsNullOrEmpty(f.FileName) && (f.ContentStream != null || f.DocumentId.HasValue))).Any())
            {
                protInitializer.Annexed = SaveStream(owner, results);
            }

            // Contatti
            protInitializer.Senders = new List<Data.ContactDTO>();
            protInitializer.Recipients = new List<Data.ContactDTO>();
            if (protocolModel.ContactManuals != null && protocolModel.ContactManuals.Any())
            {
                foreach (ProtocolContactManualModel protocolContactManualModel in protocolModel.ContactManuals)
                {
                    Data.Contact contact = new Data.Contact();
                    contact.ContactType = new Data.ContactType(Data.ContactType.Aoo);
                    contact.Description = protocolContactManualModel.Description;
                    contact.CertifiedMail = protocolContactManualModel.CertifiedEmail;
                    contact.EmailAddress = protocolContactManualModel.EMail;
                    if (!string.IsNullOrEmpty(protocolContactManualModel.Address))
                    {
                        contact.Address = new Data.Address();
                        contact.Address.Address = protocolContactManualModel.Address;
                    }

                    if (protocolContactManualModel.ComunicationType == ComunicationType.Sender)
                    {
                        protInitializer.Senders.Add(new Data.ContactDTO(contact, Data.ContactDTO.ContactType.Manual));
                    }
                    else
                    {
                        protInitializer.Recipients.Add(new Data.ContactDTO(contact, Data.ContactDTO.ContactType.Manual));
                    }
                }
            }
            if (protocolModel.Contacts != null && protocolModel.Contacts.Any())
            {
                foreach (ProtocolContactModel protocolContactModel in protocolModel.Contacts)
                {
                    Data.Contact contact = FacadeFactory.Instance.ContactFacade.GetById(protocolContactModel.IdContact);
                    if (protocolContactModel.ComunicationType == ComunicationType.Sender)
                    {
                        protInitializer.Senders.Add(new Data.ContactDTO(contact, Data.ContactDTO.ContactType.Address));
                    }
                    else
                    {
                        protInitializer.Recipients.Add(new Data.ContactDTO(contact, Data.ContactDTO.ContactType.Address));
                    }
                }
            }
            return protInitializer;
        }

        public static List<BuildValueModel> BuildValueUDS(UDSDto udsDto, List<BuildValueModel> buildValueModels)
        {
            if (udsDto != null)
            {
                buildValueModels.AddRange(udsDto.UDSModel.Model.Metadata
                    .SelectMany(f => f.Items.Select(x => new
                    {
                        Key = x.ColumnName,
                        Val = GetUDSValue(x)
                    }))
                    .Select(f => new BuildValueModel()
                    {
                        IsHTML = false,
                        Name = f.Key,
                        Value = f.Val
                    }));
            }
            return buildValueModels;
        }

        public static List<BuildValueModel> BuildValueProposerRole(WorkflowProperty dsw_p_ProposerRole, List<BuildValueModel> buildValueModels)
        {
            if (dsw_p_ProposerRole != null && !string.IsNullOrEmpty(dsw_p_ProposerRole.ValueString))
            {
                Model.Workflow.WorkflowRole workflowRole = JsonConvert.DeserializeObject<Model.Workflow.WorkflowRole>(dsw_p_ProposerRole.ValueString, DocSuiteContext.DefaultWebAPIJsonSerializerSettings);
                if (workflowRole != null && !string.IsNullOrEmpty(workflowRole.Name))
                {
                    buildValueModels.Add(new BuildValueModel()
                    {
                        IsHTML = false,
                        Name = "ProposerRoleName",
                        Value = workflowRole.Name,
                    });
                }
            }
            return buildValueModels;
        }

        public static string GetUDSValue(FieldBaseType field)
        {
            string ret = string.Empty;
            if (field is StatusField)
            {
                StatusField rField = field as StatusField;
                if (!string.IsNullOrEmpty(rField.Value))
                {
                    ret = rField.Value;
                }
                return ret;
            }
            if (field is LookupField)
            {
                LookupField rField = field as LookupField;
                if (!string.IsNullOrEmpty(rField.Value))
                {
                    ret = string.Join(", ", JsonConvert.DeserializeObject<string[]>(rField.Value));
                }
                return ret;
            }
            if (field is EnumField)
            {
                EnumField rField = field as EnumField;
                if (!string.IsNullOrEmpty(rField.Value))
                {
                    ret = string.Join(", ", JsonConvert.DeserializeObject<string[]>(rField.Value));
                }
                return ret;
            }
            if (field is NumberField)
            {
                NumberField rField = field as NumberField;
                if (rField.Value != double.MinValue)
                {
                    ret = rField.Value.ToString(rField.Format);
                }
                return ret;
            }
            if (field is BoolField)
            {
                BoolField rField = field as BoolField;
                ret = rField.Value ? "vero" : "falso";
                return ret;
            }
            if (field is DateField)
            {
                DateField rField = field as DateField;
                if (rField.Value != DateTime.MinValue)
                {
                    ret = rField.Value.ToLongDateString();
                    if (rField.RestrictedYear)
                    {
                        ret = rField.Value.Year.ToString();
                    }
                }
                return ret;
            }
            if (field is TextField)
            {
                TextField rField = field as TextField;
                ret = rField.Value;
            }
            return ret;
        }

    }
}
