using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BiblosDS.Library.Common.Services;
using System.ComponentModel;
using BiblosDS.Library.Common.Objects;
using System.ServiceModel.Activation;
using System.IO;
using BiblosDS.Library.Common.Exceptions;
using VecompSoftware.ServiceContract.BiblosDS.Documents;
using BiblosDS.Library.Common.Utility;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Administration" in code, svc and config file together.
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class Administration : IAdministration
{
    static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Administration));
    public void DoWork()
    {
    }

    #region Get


    public bool CheckConnection()
    {
        return DocumentService.IsConnected();
    }


    public BindingList<Document> GetDocumentsInTransito()
    {
        return DocumentService.GetDocumentInTransito(0);
    }


    public BindingList<Document> GetDocumentsInStorage(Guid IdStorage)
    {
        return DocumentService.GetDocumentsInStorage(IdStorage);
    }


    public BindingList<DocumentStorage> GetAllStorages()
    {
        return StorageService.GetStorages();
    }

    public BindingList<DocumentStorage> GetAllStoragesWithServer()
    {
        return StorageService.GetStoragesWithServer();
    }

    public BindingList<DocumentStorage> GetStoragesFromArchive(Guid IdArchive)
    {
        return StorageService.GetStorageFromArchive(new DocumentArchive(IdArchive));
    }

    public DocumentStorage GetStorage(Guid IdStorage)
    {
        return StorageService.GetStorage(IdStorage);
    }

    public DocumentStorage GetStorageWithServer(Guid IdStorage)
    {
        return StorageService.GetStorageWithServer(IdStorage);
    }

    public BindingList<DocumentStorageArea> GetStorageAreas(Guid IdStorage)
    {
        return StorageService.GetAllStorageAreaFromStorage(IdStorage);
    }


    public BindingList<DocumentStorageArea> GetStorageAreasFromStorageArchive(Guid IdStorage, Guid IdArchive)
    {
        return StorageService.GetStorageAreasFromStorageArchive(IdStorage, IdArchive);
    }


    public DocumentStorageArea GetStorageArea(Guid IdStorageArea)
    {
        return StorageService.GetStorageArea(IdStorageArea);
    }


    public BindingList<Status> GetAllStorageAreaStatus()
    {
        return StorageService.GetAllStorageAreaStatus();
    }


    public BindingList<DocumentStorageRule> GetStorageRulesFromStorageAreaArchive(Guid IdStorageArea, Guid IdArchive)
    {
        return new BindingList<DocumentStorageRule>(StorageService.GetStorageRulesFromStorageAreaArchive(IdStorageArea, IdArchive).Select(x => new DocumentStorageRule { Attribute = x.Attribute, RuleFilter = x.RuleFilter, RuleFormat = x.RuleFormat, RuleOperator = x.RuleOperator, RuleOrder = x.RuleOrder }).ToList());
    }

    public BindingList<DocumentStorage> GetStoragesNotRelatedToArchive(Guid IdArchive)
    {
        return StorageService.GetStoragesNotRelatedToArchive(new DocumentArchive(IdArchive));
    }


    public DocumentArchive GetArchive(Guid IdArchive)
    {
        try
        {
            logger.DebugFormat("GetArchive {0}", IdArchive);
            return ArchiveService.GetArchive(IdArchive);
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            throw;
        }
    }

    public DocumentArchive GetArchiveWithServerConfigs(Guid IdArchive)
    {
        try
        {
            logger.DebugFormat("GetArchiveWithServerConfigs {0}", IdArchive);
            return ArchiveService.GetArchiveWithServerConfigs(IdArchive);
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            throw;
        }
    }

    public BindingList<DocumentArchive> GetArchives()
    {
        try
        {
            return ArchiveService.GetArchives();
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            throw;
        }
    }

    public BindingList<DocumentArchive> GetArchivesById(IEnumerable<Guid> idsArchive)
    {
        try
        {
            return ArchiveService.GetArchivesById(idsArchive);
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            throw;
        }
    }

    public BindingList<DocumentArchive> GetArchivesFromStorage(Guid IdStorage)
    {
        return ArchiveService.GetArchivesFromStorage(new DocumentStorage(IdStorage));
    }


    public BindingList<DocumentArchive> GetArchivesNotRelatedToStorage(Guid IdStorage)
    {
        return ArchiveService.GetArchivesNotRelatedToStorage(new DocumentStorage(IdStorage));
    }


    public BindingList<DocumentArchiveStorage> GetStorageArchiveRelationsFromStorage(Guid IdStorage)
    {
        return ArchiveStorageService.GetStorageArchiveRelationsFromStorage(new DocumentStorage(IdStorage));
    }


    public BindingList<DocumentArchiveStorage> GetStorageArchiveRelationsFromArchive(Guid IdArchive)
    {
        return ArchiveStorageService.GetStorageArchiveRelationsFromArchive(new DocumentArchive(IdArchive));
    }


    public BindingList<DocumentAttribute> GetAttributesFromArchive(Guid IdArchive)
    {
        return AttributeService.GetAttributesFromArchive(IdArchive);
    }


    public DocumentAttribute GetAttribute(Guid IdAttribute)
    {
        return AttributeService.GetAttribute(IdAttribute);
    }


    public BindingList<DocumentStorageRule> GetStorageRulesFromStorage(Guid IdStorage)
    {
        return StorageService.GetStorageRulesFromStorage(IdStorage);
    }


    public DocumentStorageRule GetStorageRule(Guid IdStorage, Guid IdAttribute)
    {
        return StorageService.GetStorageRule(IdStorage, IdAttribute);
    }


    public BindingList<DocumentStorageRule> GetStorageRules(Guid IdStorage, Guid IdArchive)
    {
        return StorageService.GetStorageRuleFromStorageArchive(IdStorage, IdArchive);
    }


    public BindingList<DocumentStorageType> GetStoragesType()
    {
        return StorageService.GetStoragesType();
    }


    public Document GetDocument(Guid IdDocument)
    {
        return DocumentService.GetDocument(IdDocument);
    }


    public BindingList<DocumentAttributeMode> GetAttributeModes()
    {
        return AttributeService.GetAttributeModes();
    }


    public BindingList<DocumentStorageAreaRule> GetStorageAreaRuleFromStorageArea(Guid IdStorageArea)
    {
        return StorageService.GetStorageAreaRulesFromStorageArea(IdStorageArea);
    }


    public DocumentStorageAreaRule GetStorageAreaRule(Guid IdStorageArea, Guid IdAttribute)
    {
        return StorageService.GetStorageAreaRule(IdStorageArea, IdAttribute);
    }


    public BindingList<DocumentRuleOperator> GetRuleOperators()
    {
        return StorageService.GetRuleOperators();
    }

    public BindingList<DocumentAttributeGroup> GetAttributeGroup(Guid IdArchive)
    {
        return AttributeService.GetAttributeGroup(IdArchive);
    }

    public BindingList<string> GetPreservationFiscalDocumentsTypes()
    {
        return DocumentService.GetPreservationFiscalDocumentsTypes();
    }

    public BindingList<Server> GetServers()
    {
        return ServerService.GetServers();
    }

    public Server GetServer(Guid serverId)
    {
        return ServerService.GetServer(serverId);
    }

    #endregion

    #region Add


    public void AddStorage(DocumentStorage Storage)
    {
        StorageService.AddStorage(Storage);
        using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
        {
            (clientChannel as IServiceDocumentStorage).InitializeStorage(Storage);
        }
    }


    public Guid AddArchive(DocumentArchive Archive)
    {
        return ArchiveService.AddArchive(Archive);
    }


    public void UpdateArchive(DocumentArchive Archive)
    {
        ArchiveService.UpdateArchive(Archive);
    }


    public void AddStorageToArchive(Guid IdStorage, Guid IdArchive, bool Enable)
    {
        StorageService.AddStorageToArchive(IdStorage, IdArchive, Enable);
    }


    public void EnableDisableStorageToArchive(Guid IdStorage, Guid IdArchive, bool Enable)
    {
        StorageService.EnableDisableStorageToArchive(IdStorage, IdArchive, Enable);
    }


    public void AddStorageRule(DocumentStorageRule StorageRule)
    {
        StorageService.AddStorageRule(StorageRule);
    }


    public void UpdateStorageRule(DocumentStorageRule StorageRule)
    {
        StorageService.UpdateStorageRule(StorageRule);
    }


    public void AddStorageArea(DocumentStorageArea StorageArea)
    {
        StorageService.AddStorageArea(StorageArea);
    }


    public void UpdateStorageArea(DocumentStorageArea StorageArea)
    {
        StorageService.UpdateStorageArea(StorageArea);
    }


    public void AddStorageAreaRule(DocumentStorageAreaRule StorageAreaRule)
    {
        StorageService.AddStorageAreaRule(StorageAreaRule);
    }


    public void UpdateStorageAreaRule(DocumentStorageAreaRule StorageAreaRule)
    {
        StorageService.UpdateStorageAreaRule(StorageAreaRule);
    }


    public void AddAttribute(DocumentAttribute DocumentAttribute)
    {
        AttributeService.AddAttribute(DocumentAttribute);
    }


    public void UpdateAttribute(DocumentAttribute DocumentAttribute)
    {
        AttributeService.UpdateAttribute(DocumentAttribute);
    }


    public void DeleteAttribute(Guid IdAttribute)
    {
        AttributeService.DeleteAttribute(IdAttribute);
    }


    public void UpdateStorage(DocumentStorage storage)
    {
        StorageService.UpdateStorage(storage);
    }


    public void UpdateArchiveStorage(DocumentArchiveStorage ArchiveStorage)
    {
        ArchiveStorageService.UpdateArchiveStorage(ArchiveStorage);
    }


    public void AddArchiveStorage(DocumentArchiveStorage ArchiveStorage)
    {
        ArchiveStorageService.AddArchiveStorage(ArchiveStorage);
    }


    public void DeleteArchiveStorage(DocumentArchiveStorage ArchiveStorage)
    {
        ArchiveStorageService.DeleteArchiveStorage(ArchiveStorage);
    }

    public void AddDocumentAttributeGroup(DocumentAttributeGroup AttributeGroup)
    {
        AttributeService.AddAttributeGroup(AttributeGroup);
    }

    public void UpdateDocumentAttributeGroup(DocumentAttributeGroup AttributeGroup)
    {
        throw new NotImplementedException();
    }

    public void AddAttributeGroup(DocumentAttributeGroup AttributeGroup)
    {
        AttributeService.AddAttributeGroup(AttributeGroup);
    }

    public void UpdateAttributeGroup(DocumentAttributeGroup AttributeGroup)
    {
        AttributeService.UpdateAttributeGroup(AttributeGroup);
    }

    public void DeleteAttributeGroup(Guid IdAttributeGroup)
    {
        AttributeService.DeleteAttributeGroup(IdAttributeGroup);
    }

    public Server UpdateServer(Server server)
    {
        return ServerService.UpdateServer(server, false);
    }

    public Server AddServer(Server server)
    {
        return ServerService.UpdateServer(server, true);
    }

    public bool DeleteServer(Server server)
    {
        return ServerService.DeleteServer(server);
    }

    public ArchiveServerConfig AddArchiveServerConfig(ArchiveServerConfig config)
    {
        return ServerService.AddArchiveServerConfig(config);
    }

    public void DeleteArchiveServerConfig(ArchiveServerConfig config)
    {
        ServerService.DeleteArchiveServerConfig(config);
    }

    #endregion

    public Guid CloneArchive(string templateName, string archiveName)
    {
        try
        {
            logger.InfoFormat("CloneArchive templateName:{0}, archiveName:{1}", templateName, archiveName);
            return ArchiveService.CloneArchive(templateName, archiveName);
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
        }
    }


    public Guid CloneCompanyArchive(string templateName, string archiveName, Company company)
    {
        logger.InfoFormat("CloneCompanyArchive {0} - {1}", templateName, archiveName);
        try
        {
            var idArchive = ArchiveService.CloneArchive(templateName, archiveName);
            //TODO 
            /*
             1.Verifica nella tabella company se l'idCompany esite (company.IdCompany)
             1.1 Se non esiste la company viene inserita la company con le informazioni nella tabella company
             2 Viene craata l'associazione ArchiveCompany
             * */

            Company comp = null;
            if (company != null)
                comp = ArchiveService.GetCompany(company.IdCompany);

            if (comp == null)
            {
                comp = ArchiveService.AddCompany(company);

                var toAdd = new ArchiveCompany
                {
                    IdArchive = idArchive,
                    IdCompany = comp.IdCompany,
                    WorkingDir = "", //Da dove la pesco?
                    XmlFileTemplatePath = "" //Da dove lo prendo?
                };

                toAdd = new BiblosDS.Library.Common.Preservation.Services.PreservationService().AddArchiveCompany(toAdd);
            }

            return idArchive;
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            throw;
        }
    }


    public void AddArchiveCertificate(Guid idArchive, string userName, string pin, string fileName, byte[] certificateBlob)
    {
        try
        {
            logger.InfoFormat("AddArchiveCertificate name:{0}, idArchive:{1}", fileName, idArchive);
            ArchiveService.AddArchiveCertificate(idArchive, userName, pin, fileName, certificateBlob);
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
        }
    }

    public DocumentArchiveCertificate GetArchiveCertificate(Guid idArchive)
    {
        try
        {
            logger.InfoFormat("GetArchiveCertificate idArchive:{0}", idArchive);
            return ArchiveService.GetArchiveCertificate(idArchive);
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
        }
    }
}
