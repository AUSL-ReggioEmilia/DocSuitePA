using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.Helpers.UDS;

namespace VecompSoftware.DocSuiteWeb.UDSDesigner
{
    public class UDSConverter
    {

        public static string collectionDocument = "Documenti";
        public static string collectionAnnexed = "Annessi";
        public static string collectionAttachment = "Allegati";

        private string title = string.Empty;
        private string subject = string.Empty;
        private string alias = string.Empty;
        private bool enabledWorkflow = false;
        private bool enabledProtocol = false;
        private bool enabledPEC = false;
        private bool enabledPECButton = false;
        private bool enabledMailButton = false;
        private bool enabledMailRoleButton = false;
        private bool enabledLinkButton = false;
        private bool enabledCQRSSync = true;
        private bool enabledCancelMotivation = false;
        private bool enabledConservation = false;
        private bool hideRegistrationIdentifier = false;
        private bool stampaConformeEnabled = true;
        private bool showArchiveInProtocolSummaryEnabled = true;
        private bool requiredRevisionUDSRepository = false;
        private bool incrementalIdentityEnabled = true;
        private bool createContainer = true;
        private bool resultVisibility = true;
        private bool modifyEnabled = true;
        private bool showLastChangedDate = false;
        private bool showLastChangedUser = false;
        private bool documentTypeCoherencyInArchivingCollaborationDisabled = false;


        //elenco di controlli
        private SectionManager sections = new SectionManager();
        private Dictionary<string, Document> documents = new Dictionary<string, Document>();
        private Authorizations authorizations = null;
        private List<Contacts> contacts = new List<Contacts>();
        private Category category = new Category();
        private Container container = new Container();
        private SubjectType subjectType = new SubjectType();

        private string ctlTitle = "Title";
        private string ctlHeader = "Header";
        private string ctlEnum = "Enum";
        private string ctlStatus = "Status";
        private string ctlText = "Text";
        private string ctlNumber = "Number";
        private string ctlDate = "Date";
        private string ctlCheckbox = "Checkbox";
        private string ctlLookup = "Lookup";
        private string ctlDocument = "Document";
        private string ctlContact = "Contact";
        private string ctlAuthorization = "Authorization";
        private string ctlTreeList = "TreeList";

        private readonly IDictionary<ContactType, string> _contactTypeDescriptions = new Dictionary<ContactType, string>()
        {
            { ContactType.None, "Non definito" },
            { ContactType.Sender, "Mittente" },
            { ContactType.Recipient, "Destinatario" }
        };
        private readonly IDictionary<CustomActionEnum, string> _customActionsDescriptions = new Dictionary<CustomActionEnum, string>()
        {
            { CustomActionEnum.None, "Non definito" },
            { CustomActionEnum.LeggivaloredachiavedellutentecorrentedaActiveDirectory, "Leggi “valore da chiave” dell’utente corrente da Active Directory" }
        };

        public List<string> GetContactTypeDescriptionLabels()
        {
            List<string> ListContactLabels = new List<string>();
            foreach (KeyValuePair<ContactType, string> item in _contactTypeDescriptions)
            {
                ListContactLabels.Add(item.Value);
            }
            return ListContactLabels;
        }

        public List<string> GetCustomActionDescriptionLabels()
        {
            List<string> ListCustomActionLabels = new List<string>();
            foreach (KeyValuePair<CustomActionEnum, string> item in _customActionsDescriptions)
            {
                ListCustomActionLabels.Add(item.Value);
            }
            return ListCustomActionLabels;
        }

        public UDSModel ConvertFromJson(JsModel jsonModel)
        {
            if (jsonModel.elements == null)
                return null;

            Dictionary<string, Func<Element, bool>> parseDict = GetElementParserDict();
            foreach (Element el in jsonModel.elements)
            {
                if (!parseDict.ContainsKey(el.ctrlType))
                    continue;

                parseDict[el.ctrlType](el);
            }

            UDSModel uds = new UDSModel();
            UnitaDocumentariaSpecifica model = uds.Model;

            category.ClientId = "CategoryId";
            category.Label = "Classificatore";

            container.ClientId = "ContainerId";
            container.Label = "Contenitore";
            container.CreateContainer = createContainer;

            subjectType.DefaultValue = subject;
            subjectType.ClientId = "SubjectId";
            subjectType.Label = "Oggetto";
            subjectType.ResultVisibility = resultVisibility;

            model.Title = title;
            model.Category = category;
            model.Container = container;
            model.Subject = subjectType;
            model.Subject.ModifyEnabled = modifyEnabled;
            model.WorkflowEnabled = enabledWorkflow;
            model.ProtocolEnabled = enabledProtocol;
            model.PECEnabled = enabledPEC;
            model.PECButtonEnabled = enabledPECButton;
            model.MailButtonEnabled = enabledMailButton;
            model.MailRoleButtonEnabled = enabledMailRoleButton;
            model.LinkButtonEnabled = enabledLinkButton;
            model.DocumentUnitSynchronizeEnabled = enabledCQRSSync;
            model.IncrementalIdentityEnabled = incrementalIdentityEnabled;
            model.Alias = alias;
            model.CancelMotivationRequired = enabledCancelMotivation;
            model.ConservationEnabled = enabledConservation;
            model.HideRegistrationIdentifier = hideRegistrationIdentifier;
            model.StampaConformeEnabled = stampaConformeEnabled;
            model.ShowArchiveInProtocolSummaryEnabled = showArchiveInProtocolSummaryEnabled;
            model.RequiredRevisionUDSRepository = requiredRevisionUDSRepository;
            model.ShowLastChangedDate = showLastChangedDate;
            model.ShowLastChangedUser = showLastChangedUser;
            model.DocumentTypeCoherencyInArchivingCollaborationDisabled = documentTypeCoherencyInArchivingCollaborationDisabled;
            model.DocumentTypeCoherencyInArchivingCollaborationDisabledSpecified = true;

            model.Contacts = contacts.ToArray();
            model.Documents = new Documents();

            if (documents.Count() > 0)
            {
                Document doc = null;

                if (documents.TryGetValue(collectionDocument, out doc))
                {
                    doc.ClientId = string.Concat("uds_doc_main_0");
                    model.Documents.Document = doc;
                }

                if (documents.TryGetValue(collectionAnnexed, out doc))
                {
                    doc.ClientId = string.Concat("uds_doc_annexed_1");
                    model.Documents.DocumentAnnexed = doc;
                }

                if (documents.TryGetValue(collectionAttachment, out doc))
                {
                    doc.ClientId = string.Concat("uds_doc_attachment_1");
                    model.Documents.DocumentAttachment = doc;
                }

            }

            model.Authorizations = authorizations;
            model.Metadata = sections.GetSections();

            return uds;
        }

        //A partire dal modello Uds crea un modello di default JSon
        public JsModel ConvertToJs(UDSModel uds)
        {
            UnitaDocumentariaSpecifica model = uds.Model;

            //title
            List<Element> elements = new List<Element>();
            Element titleElement = new Element
            {
                ctrlType = ctlTitle,
                label = model.Title,
                readOnly = false,
                searchable = true,
                modifyEnable = model.Subject.ModifyEnabled,
                subject = model.Subject.DefaultValue,
                alias = model.Alias,
                clientId = "SubjectId",
                enabledWorkflow = model.WorkflowEnabled,
                enabledProtocol = model.ProtocolEnabled,
                enabledPEC = model.PECEnabled,
                enabledPECButton = model.PECButtonEnabled,
                enabledMailButton = model.MailButtonEnabled,
                enabledMailRoleButton = model.MailRoleButtonEnabled,
                enabledLinkButton = model.LinkButtonEnabled,
                enabledCQRSSync = model.DocumentUnitSynchronizeEnabled,
                enabledConservation = model.ConservationEnabled,
                enabledCancelMotivation = model.CancelMotivationRequired,
                incrementalIdentityEnabled = model.IncrementalIdentityEnabled,
                subjectResultVisibility = model.Subject.ResultVisibility,
                hideRegistrationIdentifier = model.HideRegistrationIdentifier,
                stampaConformeEnabled = model.StampaConformeEnabled,
                showArchiveInProtocolSummaryEnabled = model.ShowArchiveInProtocolSummaryEnabled,
                requiredRevisionUDSRepository = model.RequiredRevisionUDSRepository,
                showLastChangedDate = model.ShowLastChangedDate,
                showLastChangedUser = model.ShowLastChangedUser,
                documentTypeCoherencyInArchivingCollaborationDisabled = model.DocumentTypeCoherencyInArchivingCollaborationDisabledSpecified && model.DocumentTypeCoherencyInArchivingCollaborationDisabled
            };

            if (model.Category != null)
            {
                titleElement.idCategory = model.Category.IdCategory;
                titleElement.categoryReadOnly = model.Category.ReadOnly;
                titleElement.categorySearchable = model.Category.Searchable;
                titleElement.categoryDefaultEnabled = model.Category.DefaultEnabled;
                titleElement.categoryResultVisibility = model.Category.ResultVisibility;
            }

            if (model.Container != null)
            {
                titleElement.idContainer = model.Container.IdContainer;
                titleElement.containerSearchable = model.Container.Searchable;
                titleElement.createContainer = model.Container.CreateContainer;
            }

            elements.Add(titleElement);

            //sections
            if (model.Metadata != null)
            {
                foreach (Section section in model.Metadata)
                {
                    if (section.SectionLabel != SectionManager.DefaultSectionName)
                    {
                        elements.Add(new Element
                        {
                            ctrlType = ctlHeader,
                            label = section.SectionLabel
                        });
                    }

                    if (section.Items != null)
                    {
                        foreach (FieldBaseType field in section.Items)
                        {
                            Element element = CreateFieldElement(field);
                            if (element != null)
                            {
                                if (element.clientId == "0" || string.IsNullOrEmpty(element.clientId))
                                {
                                    element.clientId = string.Concat("uds_", element.columnName, "_", elements.Count());
                                }
                                elements.Add(element);
                            }
                        }
                    }
                }
            }

            //documents
            if (model.Documents != null && model.Documents.Document != null)
            {
                elements.Add(CreateDocumentElement(model.Documents.Document, collectionDocument));
            }

            if (model.Documents != null && model.Documents.DocumentAnnexed != null)
            {
                elements.Add(CreateDocumentElement(model.Documents.DocumentAnnexed, collectionAnnexed));
            }

            if (model.Documents != null && model.Documents.DocumentAttachment != null)
            {
                elements.Add(CreateDocumentElement(model.Documents.DocumentAttachment, collectionAttachment));
            }

            //contacts
            if (model.Contacts != null)
            {
                foreach (Contacts contact in model.Contacts)
                {
                    elements.Add(CreateContactElement(contact));
                }
            }

            //authorizations
            if (model.Authorizations != null)
            {
                elements.Add(CreateAuthorizationsElement(model.Authorizations));
            }

            return new JsModel
            {
                elements = elements.ToArray()
            };
        }

        private Element CreateFieldElement(FieldBaseType obj)
        {
            if (obj.GetType() == typeof(EnumField))
            {
                EnumField field = obj as EnumField;
                Element enumField = new Element
                {
                    ctrlType = ctlEnum,
                    label = field.Label,
                    enumOptions = field.Options,
                    defaultValue = field.DefaultValue ?? "",
                    defaultSearchValue = field.DefaultSearchValue ?? "",
                    readOnly = field.ReadOnly,
                    required = field.Required,
                    columnName = field.ColumnName,
                    searchable = field.Searchable,
                    modifyEnable = field.ModifyEnabled,
                    hiddenField = field.HiddenField,
                    clientId = field.ClientId,
                    resultVisibility = field.ResultVisibility,
                    resultPosition = field.ResultPosition,
                    multipleValues = field.MultipleValues
                };
                if (field.Layout != null)
                {
                    enumField.rows = field.Layout.RowNumber;
                    enumField.columns = field.Layout.ColNumber;
                }

                return enumField;
            }

            if (obj.GetType() == typeof(StatusField))
            {
                StatusField field = obj as StatusField;
                Element statusField = new Element
                {
                    ctrlType = ctlStatus,
                    label = field.Label,
                    statusType = field.Options,
                    defaultValue = field.DefaultValue ?? "",
                    defaultSearchValue = field.DefaultSearchValue ?? "",
                    readOnly = field.ReadOnly,
                    required = field.Required,
                    columnName = field.ColumnName,
                    searchable = field.Searchable,
                    modifyEnable = field.ModifyEnabled,
                    hiddenField = field.HiddenField,
                    clientId = field.ClientId,
                    resultVisibility = field.ResultVisibility,
                    resultPosition = field.ResultPosition
                };
                if (field.Layout != null)
                {
                    statusField.rows = field.Layout.RowNumber;
                    statusField.columns = field.Layout.ColNumber;
                }

                return statusField;
            }

            if (obj.GetType() == typeof(TextField))
            {
                TextField field = obj as TextField;
                Element textField = new Element
                {
                    ctrlType = ctlText,
                    label = field.Label,
                    multiLine = field.Multiline,
                    HTMLEnable = field.HTMLEnable,
                    defaultValue = field.DefaultValue ?? "",
                    defaultSearchValue = field.DefaultSearchValue ?? "",
                    readOnly = field.ReadOnly,
                    required = field.Required,
                    columnName = field.ColumnName,
                    searchable = field.Searchable,
                    modifyEnable = field.ModifyEnabled,
                    hiddenField = field.HiddenField,
                    customActionKey = field.CustomActionKey,
                    clientId = field.ClientId,
                    resultVisibility = field.ResultVisibility,
                    resultPosition = field.ResultPosition
                };
                if (field.Layout != null)
                {
                    textField.rows = field.Layout.RowNumber;
                    textField.columns = field.Layout.ColNumber;
                }

                foreach (KeyValuePair<CustomActionEnum, string> item in _customActionsDescriptions)
                {
                    textField.customActions.Add(item.Value);
                }

                if (_customActionsDescriptions.ContainsKey(field.CustomAction))
                {
                    textField.customActionSelected = _customActionsDescriptions[field.CustomAction];
                }

                return textField;
            }

            if (obj.GetType() == typeof(DateField))
            {
                DateField field = obj as DateField;
                Element element = new Element
                {
                    ctrlType = ctlDate,
                    label = field.Label,
                    defaultSearchValue = field.DefaultSearchValue.ToString("dd/MM/yyyy") ?? "",
                    restrictedYear = field.RestrictedYear,
                    enableDefaultDate = field.DefaultTodayEnabled,
                    readOnly = field.ReadOnly,
                    required = field.Required,
                    columnName = field.ColumnName,
                    searchable = field.Searchable,
                    modifyEnable = field.ModifyEnabled,
                    hiddenField = field.HiddenField,
                    clientId = field.ClientId,
                    resultVisibility = field.ResultVisibility,
                    resultPosition = field.ResultPosition
                };

                element.defaultValue = "";
                if (field.DefaultValueSpecified && field.DefaultValue != DateTime.MinValue)
                    element.defaultValue = field.DefaultValue.ToString("dd/MM/yyyy");

                if (field.Layout != null)
                {
                    element.rows = field.Layout.RowNumber;
                    element.columns = field.Layout.ColNumber;
                }

                return element;
            }

            if (obj.GetType() == typeof(NumberField))
            {
                NumberField field = obj as NumberField;
                Element numberField = new Element
                {
                    ctrlType = ctlNumber,
                    label = field.Label,
                    defaultValue = field.DefaultValueSpecified == true ? field.DefaultValue.ToString() : "",
                    readOnly = field.ReadOnly,
                    required = field.Required,
                    columnName = field.ColumnName,
                    searchable = field.Searchable,
                    modifyEnable = field.ModifyEnabled,
                    hiddenField = field.HiddenField,
                    clientId = field.ClientId,
                    resultVisibility = field.ResultVisibility,
                    resultPosition = field.ResultPosition,
                    format = field.Format
                };
                if (field.MinValueSpecified)
                {
                    numberField.minValue = field.MinValue;
                }
                if (field.MaxValueSpecified)
                {
                    numberField.maxValue = field.MaxValue;
                }
                if (field.Layout != null)
                {
                    numberField.rows = field.Layout.RowNumber;
                    numberField.columns = field.Layout.ColNumber;
                }
                return numberField;
            }

            if (obj.GetType() == typeof(BoolField))
            {
                BoolField field = obj as BoolField;
                Element boolField = new Element
                {
                    ctrlType = ctlCheckbox,
                    label = field.Label,
                    defaultValue = field.DefaultValue == true && field.DefaultValueSpecified ? "True" : "",
                    readOnly = field.ReadOnly,
                    required = field.Required,
                    columnName = field.ColumnName,
                    searchable = field.Searchable,
                    modifyEnable = field.ModifyEnabled,
                    hiddenField = field.HiddenField,
                    clientId = field.ClientId,
                    resultVisibility = field.ResultVisibility,
                    resultPosition = field.ResultPosition
                };
                if (field.Layout != null)
                {
                    boolField.rows = field.Layout.RowNumber;
                    boolField.columns = field.Layout.ColNumber;
                }
                return boolField;
            }

            if (obj.GetType() == typeof(LookupField))
            {
                LookupField field = obj as LookupField;
                Element lookup = new Element
                {
                    ctrlType = ctlLookup,
                    label = field.Label,
                    required = field.Required,
                    columnName = field.ColumnName,
                    lookupRepositoryName = field.LookupArchiveName,
                    lookupFieldName = field.LookupArchiveColumnName == "_subject" ? "Oggetto" : field.LookupArchiveColumnName,
                    searchable = field.Searchable,
                    modifyEnable = field.ModifyEnabled,
                    hiddenField = field.HiddenField,
                    clientId = field.ClientId,
                    resultVisibility = field.ResultVisibility,
                    resultPosition = field.ResultPosition,
                    multipleValues = field.MultipleValues
                };
                if (field.Layout != null)
                {
                    lookup.rows = field.Layout.RowNumber;
                    lookup.columns = field.Layout.ColNumber;
                }
                return lookup;
            }

            if (obj.GetType() == typeof(TreeListField))
            {
                TreeListField field = obj as TreeListField;
                Element treeList = new Element
                {
                    ctrlType = ctlTreeList,
                    label = field.Label,
                    required = field.Required,
                    columnName = field.ColumnName,
                    searchable = field.Searchable,
                    modifyEnable = field.ModifyEnabled,
                    hiddenField = field.HiddenField,
                    clientId = field.ClientId,
                    resultVisibility = field.ResultVisibility,
                    resultPosition = field.ResultPosition,
                    defaultValue = field.DefaultValue
                };
                if (field.Layout != null)
                {
                    treeList.rows = field.Layout.RowNumber;
                    treeList.columns = field.Layout.ColNumber;
                }
                return treeList;
            }

            return null;
        }

        private Element CreateDocumentElement(Document doc, string collectionType)
        {
            Element document = new Element
            {
                ctrlType = ctlDocument,
                collectionType = collectionType,
                label = doc.Label,
                enableMultifile = doc.AllowMultiFile,
                archive = doc.BiblosArchive,
                enableUpload = doc.UploadEnabled,
                enableScanner = doc.ScannerEnabled,
                enableSign = doc.SignEnabled,
                signRequired = doc.SignRequired,
                copyProtocol = doc.CopyProtocol,
                copyResolution = doc.CopyResolution,
                copySeries = doc.CopySeries,
                copyUDS = doc.CopyUDS,
                createBiblosArchive = doc.CreateBiblosArchive,
                required = doc.Required,
                searchable = doc.Searchable,
                documentDeletable = doc.Deletable,
                modifyEnable = doc.ModifyEnabled
            };

            if (doc.Layout != null)
            {
                document.rows = doc.Layout.RowNumber;
                document.columns = doc.Layout.ColNumber;
            }
            return document;
        }

        private Element CreateContactElement(Contacts conts)
        {
            Element contact = new Element()
            {
                ctrlType = ctlContact,
                label = conts.Label,
                enableADDistribution = conts.ADDistributionListEnabled,
                enableAddressBook = conts.AddressBookEnabled,
                enableAD = conts.ADEnabled,
                enableMultiContact = conts.AllowMultiContact,
                enableExcelImport = conts.ExcelImportEnabled,
                enableManual = conts.ManualEnabled,
                required = conts.Required,
                searchable = conts.Searchable,
                modifyEnable = conts.ModifyEnabled,
                clientId = conts.ClientId,
                resultVisibility = conts.ResultVisibility,
                resultPosition = conts.ResultPosition

            };

            if (conts.Layout != null)
            {
                contact.rows = conts.Layout.RowNumber;
                contact.columns = conts.Layout.ColNumber;
            }


            foreach (KeyValuePair<ContactType, string> item in _contactTypeDescriptions)
            {
                contact.contactTypes.Add(item.Value);
            }

            if (_contactTypeDescriptions.ContainsKey(conts.ContactType))
            {
                contact.contactTypeSelected = _contactTypeDescriptions[conts.ContactType];
            }
            return contact;
        }

        private Element CreateAuthorizationsElement(Authorizations auth)
        {
            Element authorizations = new Element
            {
                ctrlType = ctlAuthorization,
                label = auth.Label,
                enableMultiAuth = auth.AllowMultiAuthorization,
                allowMultiUserAuthorization = auth.AllowMultiUserAuthorization,
                userAuthorizationEnabled = auth.UserAuthorizationEnabled,
                myAuthorizedRolesEnabled = auth.MyAuthorizedRolesEnabled,
                required = auth.Required,
                searchable = auth.Searchable,
                clientId = auth.ClientId,
                modifyEnable = auth.ModifyEnabled,
                resultVisibility = auth.ResultVisibility,
                resultPosition = auth.ResultPosition

            };
            if (auth.Layout != null)
            {
                authorizations.rows = auth.Layout.RowNumber;
                authorizations.columns = auth.Layout.ColNumber;
            }
            return authorizations;

        }

        public string JsToXml(JsModel jsModel, out List<String> errors)
        {
            errors = new List<String>();
            UDSModel uds = ConvertFromJson(jsModel);
            if (!uds.Model.Metadata.SelectMany(f => f.Items).Any())
            {
                errors.Add("E' necessario specificare almeno un metadato dell'archivio.");
            }
            return uds.SerializeToXml();
        }

        private Dictionary<string, Func<Element, bool>> GetElementParserDict()
        {
            Dictionary<string, Func<Element, bool>> parseDict = new Dictionary<string, Func<Element, bool>>();

            parseDict.Add(ctlTitle, ParseTitle);
            parseDict.Add(ctlHeader, ParseHeader);
            parseDict.Add(ctlText, ParseText);
            parseDict.Add(ctlEnum, ParseEnum);
            parseDict.Add(ctlStatus, ParseStatus);
            parseDict.Add(ctlNumber, ParseNumber);
            parseDict.Add(ctlDate, ParseDate);
            parseDict.Add(ctlCheckbox, ParseCheckbox);
            parseDict.Add(ctlLookup, ParseLookup);
            parseDict.Add(ctlDocument, ParseDocument);
            parseDict.Add(ctlContact, ParseContact);
            parseDict.Add(ctlAuthorization, ParseAuthorization);
            parseDict.Add(ctlTreeList, ParseTreeList);

            return parseDict;
        }

        //intestazione gruppo controlli
        private bool ParseHeader(Element el)
        {
            sections.AddNew(el.label);
            return true;
        }

        private bool ParseTitle(Element el)
        {
            title = el.label;
            subject = el.subject;
            enabledWorkflow = el.enabledWorkflow;
            enabledProtocol = el.enabledProtocol;
            enabledCancelMotivation = el.enabledCancelMotivation;
            enabledConservation = el.enabledConservation;

            hideRegistrationIdentifier = el.hideRegistrationIdentifier;
            stampaConformeEnabled = el.stampaConformeEnabled;
            showArchiveInProtocolSummaryEnabled = el.showArchiveInProtocolSummaryEnabled;
            requiredRevisionUDSRepository = el.requiredRevisionUDSRepository;

            enabledPEC = el.enabledPEC;
            enabledPECButton = el.enabledPECButton;
            enabledMailButton = el.enabledMailButton;
            enabledMailRoleButton = el.enabledMailRoleButton;
            enabledLinkButton = el.enabledLinkButton;
            enabledCQRSSync = el.enabledCQRSSync;
            incrementalIdentityEnabled = el.incrementalIdentityEnabled;
            alias = el.alias;
            createContainer = el.createContainer;
            resultVisibility = el.subjectResultVisibility;
            showLastChangedDate = el.showLastChangedDate;
            showLastChangedUser = el.showLastChangedUser;
            documentTypeCoherencyInArchivingCollaborationDisabled = el.documentTypeCoherencyInArchivingCollaborationDisabled;

            SectionExt section = sections.GetCurrent();

            category = new Category
            {
                IdCategory = el.idCategory,
                ReadOnly = el.categoryReadOnly,
                Searchable = el.categorySearchable,
                ModifyEnabled = el.modifyEnable,
                DefaultEnabled = el.categoryDefaultEnabled,
                ResultVisibility = el.categoryResultVisibility
            };

            container = new Container()
            {
                IdContainer = el.idContainer,
                Searchable = el.containerSearchable,
                CreateContainer = el.createContainer
            };

            return true;
        }

        private bool ParseText(Element el)
        {
            TextField field = new TextField
            {
                Label = el.label,
                Multiline = el.multiLine,
                HTMLEnable = el.HTMLEnable,
                DefaultValue = el.defaultValue.Trim(),
                ReadOnly = el.readOnly,
                Required = el.required,
                ModifyEnabled = el.modifyEnable,
                HiddenField = el.hiddenField,
                Searchable = el.searchable,
                CustomActionKey = el.customActionKey,
                ClientId = el.clientId,
                ColumnName = Utils.SafeSQLName(el.label),
                ResultVisibility = el.resultVisibility,
                ResultPosition = el.resultPosition,
                CustomAction = CustomActionEnum.None,
                Layout = new LayoutPosition
                {
                    PanelId = el.columns.ToString(),
                    ColNumber = el.columns,
                    RowNumber = el.rows
                }
            };

            field.CustomAction = CustomActionEnum.None;
            if (!string.IsNullOrEmpty(el.customActionSelected) && _customActionsDescriptions.Any(x => x.Value == el.customActionSelected))
            {
                field.CustomAction = _customActionsDescriptions.Single(x => x.Value == el.customActionSelected).Key;
            }

            sections.AddCtrl(field);
            return true;
        }

        private bool ParseEnum(Element el)
        {
            EnumField field = new EnumField
            {
                Label = el.label,
                Options = el.enumOptions,
                DefaultValue = el.defaultValue.Trim(),
                ReadOnly = el.readOnly,
                Required = el.required,
                Searchable = el.searchable,
                ModifyEnabled = el.modifyEnable,
                HiddenField = el.hiddenField,
                ClientId = el.clientId,
                ColumnName = Utils.SafeSQLName(el.label),
                ResultVisibility = el.resultVisibility,
                ResultPosition = el.resultPosition,
                MultipleValues = el.multipleValues,
                MultipleValuesSpecified = el.multipleValues,
                CustomAction = CustomActionEnum.None,
                Layout = new LayoutPosition
                {
                    PanelId = el.columns.ToString(),
                    ColNumber = el.columns,
                    RowNumber = el.rows
                }
            };

            sections.AddCtrl(field);
            return true;
        }

        private bool ParseStatus(Element el)
        {
            StatusField field = new StatusField
            {
                Label = el.label,

                Options = el.statusType.Select(val => new StatusType()
                {
                    Value = val.Value,
                    IconPath = val.IconPath,
                    MappingTag = val.MappingTag,
                    TagValue = val.TagValue
                }).ToArray(),
                DefaultValue = el.defaultValue.Trim(),
                ReadOnly = el.readOnly,
                Required = el.required,
                Searchable = el.searchable,
                ModifyEnabled = el.modifyEnable,
                HiddenField = el.hiddenField,
                ClientId = el.clientId,
                ColumnName = Utils.SafeSQLName(el.label),
                ResultVisibility = el.resultVisibility,
                ResultPosition = el.resultPosition,
                CustomAction = CustomActionEnum.None,
                Layout = new LayoutPosition
                {
                    PanelId = el.columns.ToString(),
                    ColNumber = el.columns,
                    RowNumber = el.rows
                }
            };

            sections.AddCtrl(field);
            return true;
        }

        private bool ParseDate(Element el)
        {
            DateTime dt = DateTime.MinValue;
            bool hasDefault = DateTime.TryParse(el.defaultValue.Trim(), out dt);

            DateField field = new DateField
            {
                Label = el.label,
                RestrictedYear = el.restrictedYear,
                DefaultTodayEnabled = el.enableDefaultDate,
                DefaultValue = dt,
                DefaultValueSpecified = hasDefault,
                ReadOnly = el.readOnly,
                Required = el.required,
                ModifyEnabled = el.modifyEnable,
                HiddenField = el.hiddenField,
                Searchable = el.searchable,
                ClientId = el.clientId,
                ColumnName = Utils.SafeSQLName(el.label),
                ResultVisibility = el.resultVisibility,
                ResultPosition = el.resultPosition,
                CustomAction = CustomActionEnum.None,
                Layout = new LayoutPosition
                {
                    PanelId = el.columns.ToString(),
                    ColNumber = el.columns,
                    RowNumber = el.rows
                }
            };

            sections.AddCtrl(field);
            return true;
        }

        private bool ParseNumber(Element el)
        {
            double val = 0;
            bool hasDefault = Double.TryParse(el.defaultValue.Trim(), out val);

            NumberField field = new NumberField
            {
                Label = el.label,
                DefaultValue = val,
                DefaultValueSpecified = hasDefault,
                ReadOnly = el.readOnly,
                Required = el.required,
                Searchable = el.searchable,
                ModifyEnabled = el.modifyEnable,
                HiddenField = el.hiddenField,
                ClientId = el.clientId,
                ColumnName = Utils.SafeSQLName(el.label),
                ResultVisibility = el.resultVisibility,
                ResultPosition = el.resultPosition,
                Format = el.format,
                MinValue = el.minValue ?? 0,
                MinValueSpecified = el.minValue.HasValue,
                MaxValue = el.maxValue ?? 0,
                MaxValueSpecified = el.maxValue.HasValue,
                CustomAction = CustomActionEnum.None,
                Layout = new LayoutPosition
                {
                    PanelId = el.columns.ToString(),
                    ColNumber = el.columns,
                    RowNumber = el.rows
                }
            };

            sections.AddCtrl(field);
            return true;
        }

        private bool ParseCheckbox(Element el)
        {
            bool val = false;
            bool hasDefault = Boolean.TryParse(el.defaultValue, out val);

            BoolField field = new BoolField
            {
                Label = el.label,
                DefaultValue = val,
                DefaultValueSpecified = true,
                ReadOnly = el.readOnly,
                Required = el.required,
                Searchable = el.searchable,
                ModifyEnabled = el.modifyEnable,
                HiddenField = el.hiddenField,
                ClientId = el.clientId,
                ColumnName = Utils.SafeSQLName(el.label),
                CustomAction = CustomActionEnum.None,
                Layout = new LayoutPosition
                {
                    PanelId = el.columns.ToString(),
                    ColNumber = el.columns,
                    RowNumber = el.rows
                }
            };

            sections.AddCtrl(field);
            return true;
        }

        private bool ParseLookup(Element el)
        {
            LookupField field = new LookupField
            {
                Label = el.label,
                LookupArchiveName = el.lookupRepositoryName,
                LookupArchiveColumnName = el.lookupFieldName == "Oggetto" ? "_subject" : el.lookupFieldName,
                Required = el.required,
                Searchable = el.searchable,
                ModifyEnabled = el.modifyEnable,
                HiddenField = el.hiddenField,
                ClientId = el.clientId,
                ColumnName = Utils.SafeSQLName(el.label),
                ResultVisibility = el.resultVisibility,
                ResultPosition = el.resultPosition,
                MultipleValues = el.multipleValues,
                MultipleValuesSpecified = el.multipleValues,
                CustomAction = CustomActionEnum.None,
                Layout = new LayoutPosition
                {
                    PanelId = el.columns.ToString(),
                    ColNumber = el.columns,
                    RowNumber = el.rows
                }
            };

            sections.AddCtrl(field);
            return true;
        }

        private bool ParseDocument(Element el)
        {
            if (el.collectionType == "*")
                return true;

            SectionExt section = sections.GetCurrent();

            Document doc = new Document
            {
                Label = el.label,
                AllowMultiFile = el.enableMultifile,
                BiblosArchive = el.archive,
                UploadEnabled = el.enableUpload,
                ScannerEnabled = el.enableScanner,
                SignEnabled = el.enableSign,
                SignRequired = el.signRequired,
                CopyProtocol = el.copyProtocol,
                CopyResolution = el.copyResolution,
                CopyUDS = el.copyUDS,
                CopySeries = el.copySeries,
                CreateBiblosArchive = el.createBiblosArchive,
                Required = el.required,
                Searchable = el.searchable,
                Deletable = el.documentDeletable,
                ModifyEnabled = el.modifyEnable,
                ClientId = el.clientId,
                Layout = new LayoutPosition
                {
                    PanelId = el.columns.ToString(),
                    ColNumber = el.columns,
                    RowNumber = el.rows
                }
            };
            doc.ClientId = string.Concat("uds_doc_", el.collectionType, "_", documents.Count);
            //impedisce duplicati
            try
            {
                documents.Add(el.collectionType, doc);
            }
            catch
            {
                throw new ApplicationException(String.Format("Collezione '{0}' duplicata.", el.collectionType));
            }

            //verifica esista collezione documenti
            if (!documents.ContainsKey(UDSConverter.collectionDocument))
            {
                throw new ApplicationException(String.Format("Collezione '{0}' mancante.", UDSConverter.collectionDocument));
            }

            return true;
        }

        private bool ParseContact(Element el)
        {
            SectionExt section = sections.GetCurrent();

            Contacts contact = new Contacts
            {
                Label = el.label,
                ADDistributionListEnabled = el.enableADDistribution,
                AddressBookEnabled = el.enableAddressBook,
                ADEnabled = el.enableAD,
                AllowMultiContact = el.enableMultiContact,
                ExcelImportEnabled = el.enableExcelImport,
                ManualEnabled = el.enableManual,
                Required = el.required,
                Searchable = el.searchable,
                ClientId = el.clientId,
                ModifyEnabled = el.modifyEnable,
                ResultPosition = el.resultPosition,
                ResultVisibility = el.resultVisibility,
                Layout = new LayoutPosition
                {
                    PanelId = el.columns.ToString(),
                    ColNumber = el.columns,
                    RowNumber = el.rows
                }
            };

            contact.ContactTypeSpecified = true;
            contact.ContactType = ContactType.None;
            if (!string.IsNullOrEmpty(el.contactTypeSelected) && _contactTypeDescriptions.Any(x => x.Value == el.contactTypeSelected))
            {
                contact.ContactType = _contactTypeDescriptions.Single(x => x.Value == el.contactTypeSelected).Key;
            }

            contact.ClientId = string.Concat("uds_contact_", el.collectionType, "_", contacts.Count);
            contacts.Add(contact);

            return true;
        }

        private bool ParseAuthorization(Element el)
        {
            SectionExt section = sections.GetCurrent();

            authorizations = new Authorizations
            {
                Label = el.label,
                AllowMultiUserAuthorization = el.allowMultiUserAuthorization,
                UserAuthorizationEnabled = el.userAuthorizationEnabled,
                AllowMultiAuthorization = el.enableMultiAuth,
                MyAuthorizedRolesEnabled = el.myAuthorizedRolesEnabled,
                Required = el.required,
                Searchable = el.searchable,
                ClientId = el.clientId,
                ModifyEnabled = el.modifyEnable,
                ResultVisibility = el.resultVisibility,
                ResultPosition = el.resultPosition,
                Layout = new LayoutPosition
                {
                    PanelId = el.columns.ToString(),
                    ColNumber = el.columns,
                    RowNumber = el.rows
                }
            };
            authorizations.ClientId = string.Concat("uds_auth_", el.collectionType, "_", section.Ctrls.Count);
            return true;
        }

        private bool ParseTreeList(Element el)
        {
            TreeListField field = new TreeListField
            {
                Label = el.label,
                Required = el.required,
                Searchable = el.searchable,
                ModifyEnabled = el.modifyEnable,
                HiddenField = el.hiddenField,
                ClientId = el.clientId,
                ColumnName = Utils.SafeSQLName(el.label),
                ResultVisibility = el.resultVisibility,
                ResultPosition = el.resultPosition,
                DefaultValue = el.defaultValue,
                CustomAction = CustomActionEnum.None,
                Layout = new LayoutPosition
                {
                    PanelId = el.columns.ToString(),
                    ColNumber = el.columns,
                    RowNumber = el.rows
                }
            };

            sections.AddCtrl(field);
            return true;
        }
    }
}