using System;
using VecompSoftware.Helpers.UDS;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.UDSDesigner
{
    [Serializable]
    public class Element
    {
        public Element()
        {
            contactTypes = new List<string>();
            customActions = new List<string>();
        }
        public string ctrlType { get; set; }
        public uint rows { get; set; }
        public uint columns { get; set; }
        public string label { get; set; }
        public bool archiveReadOnly { get; set; }
        public bool titleReadOnly { get; set; }
        public string columnName { get; set; }
        public string subject { get; set; }
        public string archive { get; set; }
        public bool categoryReadOnly { get; set; }
        public bool categorySearchable { get; set; }
        public bool categoryDefaultEnabled { get; set; }
        public string collectionType { get; set; }
        public string defaultValue { get; set; }
        public string defaultSearchValue { get; set; }
        public string[] enumOptions { get; set; }
        public StatusType[] statusType { get; set; }
        public bool enabledWorkflow { get; set; }
        public bool enabledProtocol { get; set; }
        public bool enabledPEC { get; set; }
        public bool enabledPECButton { get; set; }
        public bool enabledMailButton { get; set; }
        public bool enabledMailRoleButton { get; set; }
        public bool enabledLinkButton { get; set; }
        public bool enabledCQRSSync { get; set; }
        public bool enabledConservation { get; set; } 
        public bool enabledCancelMotivation { get; set; }

        public bool hideRegistrationIdentifier { get; set; }
        public bool stampaConformeEnabled { get; set; }
        public bool showArchiveInProtocolSummaryEnabled { get; set; }
        public bool requiredRevisionUDSRepository { get; set; }
        public bool searchable { get; set; }
        public string contactTypeSelected { get; set; }

        public string format { get; set; }
        public ICollection<string> contactTypes { get; set; }

        public string idCategory { get; set; }
        public string categoryName { get; set; }
        public string idContainer { get; set; }
        public string containerName { get; set; }
        public bool containerReadOnly { get; set; }
        public bool containerSearchable { get; set; }
        public Guid idRepository { get; set; }
        public int DSWEnvironment { get; set; }
        public bool createBiblosArchive { get; set; }
        public bool multiLine { get; set; }
        public bool HTMLEnable { get; set; }
        public bool required { get; set; }
        public bool restrictedYear { get; set; }
        public bool enableDefaultDate { get; set; }
        public bool readOnly { get; set; }
        public bool enableMultifile { get; set; }
        public bool enableUpload { get; set; }
        public bool enableScanner { get; set; }
        public bool enableSign { get; set; }
        public bool signRequired { get; set; }
        public bool copyProtocol { get; set; }
        public bool copyResolution { get; set; }
        public bool copySeries { get; set; }
        public bool copyUDS { get; set; }
        public bool enableAD { get; set; }
        public bool enableAddressBook { get; set; }
        public bool enableADDistribution { get; set; }
        public bool enableManual { get; set; }
        public bool enableExcelImport { get; set; }
        public bool enableMultiContact { get; set; }
        public bool enableMultiAuth { get; set; }
        public string alias { get; set; }
        public bool documentDeletable { get; set; }
        public string clientId { get; set; }
        public bool modifyEnable { get; set; }
        public bool hiddenField { get; set; }
        public bool incrementalIdentityEnabled { get; set; }
        public bool createContainer { get; set; }
        public string lookupRepositoryName { get; set; }
        public string lookupFieldName { get; set; }
        public bool resultVisibility { get; set; }
        public bool subjectResultVisibility { get; set; }
        public bool categoryResultVisibility { get; set; }
        public short resultPosition { get; set; }
        public bool multipleValues { get; set; }
        public bool showLastChangedDate { get; set; }
        public bool showLastChangedUser { get; set; }
        public double? minValue { get; set; }
        public double? maxValue { get; set; }
        public bool myAuthorizedRolesEnabled { get; set; }
        public ICollection<string> customActions { get; set; }
        public string customActionSelected { get; set; }
        public string customActionKey { get; set; }
        public bool allowMultiUserAuthorization { get; set; }
        public bool userAuthorizationEnabled { get; set; }
        public bool documentTypeCoherencyInArchivingCollaborationDisabled { get; set; }
    }
}
