using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ComponentModel;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Exceptions;

namespace VecompSoftware.ServiceContract.BiblosDS.Documents
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAdministration" in both code and config file together.
    [ServiceContract(Namespace = "http://Vecomp.BiblosDs.Administration")]
    public interface IAdministration
    {
        [OperationContract]
        void DoWork();

        #region Clone

        /// <summary>
        /// Clone an archive from name
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="archiveName"></param>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        Guid CloneArchive(string templateName, string archiveName);

        
        /// <summary>
        /// Clone an Archive and set the association with the Company
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="archiveName"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        [OperationContract, FaultContract(typeof(BiblosDsException))]
        Guid CloneCompanyArchive(string templateName, string archiveName, Company company);

        #endregion

        #region Get

        [OperationContract]
        bool CheckConnection();

        [OperationContract]
        BindingList<Document> GetDocumentsInTransito();

        [OperationContract]
        BindingList<Document> GetDocumentsInStorage(Guid IdStorage);

        [OperationContract]
        BindingList<DocumentStorage> GetAllStorages();

        [OperationContract]
        BindingList<DocumentStorage> GetAllStoragesWithServer();

        [OperationContract]
        BindingList<DocumentStorage> GetStoragesFromArchive(Guid IdArchive);

        [OperationContract]
        DocumentStorage GetStorage(Guid IdStorage);

        [OperationContract]
        DocumentStorage GetStorageWithServer(Guid IdStorage);

        [OperationContract]
        BindingList<DocumentStorageArea> GetStorageAreas(Guid IdStorage);

        [OperationContract]
        BindingList<DocumentStorageArea> GetStorageAreasFromStorageArchive(Guid IdStorage, Guid IdArchive);

        [OperationContract]
        DocumentStorageArea GetStorageArea(Guid IdStorageArea);

        [OperationContract]
        BindingList<Status> GetAllStorageAreaStatus();

        [OperationContract]
        BindingList<DocumentStorageRule> GetStorageRulesFromStorageAreaArchive(Guid IdStorageArea, Guid IdArchive);

        [OperationContract]
        BindingList<DocumentStorage> GetStoragesNotRelatedToArchive(Guid IdArchive);

        [OperationContract]
        DocumentArchive GetArchive(Guid IdArchive);

        [OperationContract]
        DocumentArchive GetArchiveWithServerConfigs(Guid IdArchive);

        [OperationContract]
        BindingList<DocumentArchive> GetArchives();

        [OperationContract]
        BindingList<DocumentArchive> GetArchivesById(IEnumerable<Guid> idsArchive);

        [OperationContract]
        BindingList<DocumentArchive> GetArchivesFromStorage(Guid IdStorage);

        [OperationContract]
        BindingList<DocumentArchive> GetArchivesNotRelatedToStorage(Guid IdStorage);

        [OperationContract]
        BindingList<DocumentArchiveStorage> GetStorageArchiveRelationsFromStorage(Guid IdStorage);

        [OperationContract]
        BindingList<DocumentArchiveStorage> GetStorageArchiveRelationsFromArchive(Guid IdArchive);

        [OperationContract]
        BindingList<DocumentAttribute> GetAttributesFromArchive(Guid IdArchive);

        [OperationContract]
        DocumentAttribute GetAttribute(Guid IdAttribute);

        [OperationContract]
        BindingList<DocumentAttributeGroup> GetAttributeGroup(Guid IdArchive);

        [OperationContract]
        BindingList<DocumentStorageRule> GetStorageRulesFromStorage(Guid IdStorage);

        [OperationContract]
        DocumentStorageRule GetStorageRule(Guid IdStorage, Guid IdAttribute);

        [OperationContract]
        BindingList<DocumentStorageRule> GetStorageRules(Guid IdStorage, Guid IdArchive);

        [OperationContract]
        BindingList<DocumentStorageType> GetStoragesType();

        [OperationContract]
        Document GetDocument(Guid IdDocument);

        [OperationContract]
        BindingList<DocumentAttributeMode> GetAttributeModes();

        [OperationContract]
        BindingList<DocumentStorageAreaRule> GetStorageAreaRuleFromStorageArea(Guid IdStorageArea);

        [OperationContract]
        DocumentStorageAreaRule GetStorageAreaRule(Guid IdStorageArea, Guid IdAttribute);

        [OperationContract]
        BindingList<DocumentRuleOperator> GetRuleOperators();

        [OperationContract]
        BindingList<string> GetPreservationFiscalDocumentsTypes();

        [OperationContract]
        BindingList<Server> GetServers();

        [OperationContract]
        Server GetServer(Guid serverId);

        [OperationContract]
        Server UpdateServer(Server server);

        [OperationContract]
        Server AddServer(Server server);

        [OperationContract]
        bool DeleteServer(Server server);
        
        [OperationContract]
        ArchiveServerConfig AddArchiveServerConfig(ArchiveServerConfig config);

        [OperationContract]
        void DeleteArchiveServerConfig(ArchiveServerConfig config);
        
        #endregion

        #region Add

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void AddStorage(DocumentStorage Storage);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        Guid AddArchive(DocumentArchive Archive);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void UpdateArchive(DocumentArchive Archive);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void AddStorageToArchive(Guid IdStorage, Guid IdArchive, bool Enable);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void EnableDisableStorageToArchive(Guid IdStorage, Guid IdArchive, bool Enable);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void AddStorageRule(DocumentStorageRule StorageRule);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void UpdateStorageRule(DocumentStorageRule StorageRule);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void AddStorageArea(DocumentStorageArea StorageArea);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void UpdateStorageArea(DocumentStorageArea StorageArea);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void AddStorageAreaRule(DocumentStorageAreaRule StorageAreaRule);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void UpdateStorageAreaRule(DocumentStorageAreaRule StorageAreaRule);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void AddAttribute(DocumentAttribute DocumentAttribute);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void AddAttributeGroup(DocumentAttributeGroup AttributeGroup);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void UpdateAttributeGroup(DocumentAttributeGroup AttributeGroup);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void DeleteAttributeGroup(Guid IdAttributeGroup);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void UpdateAttribute(DocumentAttribute DocumentAttribute);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void DeleteAttribute(Guid IdAttribute);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void UpdateStorage(DocumentStorage storage);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void UpdateArchiveStorage(DocumentArchiveStorage ArchiveStorage);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void AddArchiveStorage(DocumentArchiveStorage ArchiveStorage);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void DeleteArchiveStorage(DocumentArchiveStorage ArchiveStorage);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void AddDocumentAttributeGroup(DocumentAttributeGroup AttributeGroup);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void UpdateDocumentAttributeGroup(DocumentAttributeGroup AttributeGroup);

        #endregion

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        void AddArchiveCertificate(Guid idArchive, string userName, string pin, string fileName, byte[] certificateBlob);

        [OperationContract, FaultContract(typeof(BiblosDsException))]
        DocumentArchiveCertificate GetArchiveCertificate(Guid idArchive);
    }
}
