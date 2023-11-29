Imports System.Collections.Generic
Imports System.DirectoryServices
Imports System.Globalization
Imports System.Linq
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.UDS
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.Web.HtmlStructure
Imports VecompSoftware.Services.Biblos.Models

Public Class uscUDSDynamics
    Inherits DocSuite2008BaseControl

#Region "Fields"
    Public Const DYNAMIC_LABEL_NAME_FORMAT As String = "lbl_{0}"
    Public Const DYNAMIC_FIELD_NAME_FORMAT As String = "field_{0}"
    Public Const DYNAMIC_VALIDATOR_NAME_FORMAT As String = "validator_{0}"
    Public Const RANGE_VALIDATOR_NAME_FORMAT As String = "range_validator_{0}"
    Public Const DYNAMIC_MANUAL_SEARCH_CONTACT_NAME_FORMAT As String = "manual_search_field_{0}"
    Public Const CTL_TITLE As String = "TitleField"
    Public Const CTL_HEADER As String = "HeaderField"
    Public Const CTL_ENUM As String = "EnumField"
    Public Const CTL_TEXT As String = "TextField"
    Public Const CTL_NUMBER As String = "NumberField"
    Public Const CTL_DATE As String = "DateField"
    Public Const CTL_CHECKBOX As String = "BoolField"
    Public Const CTL_LOOKUP As String = "LookupField"
    Public Const CTL_STATUS As String = "StatusField"
    Public Const CTL_TREE_LIST_FIELD As String = "TreeListField"
    Public Const COLLECTION_DOCUMENT As String = "Documenti"
    Public Const COLLECTION_ANNEXED As String = "Annessi"
    Public Const COLLECTION_ATTACHMENT As String = "Allegati"
    Private Const CUSTOM_VALIDATION_FUNCTION As String = "customValidateControl"
    Public Const ACTION_TYPE_EDIT As String = "Edit"
    Public Const ACTION_TYPE_VIEW As String = "View"
    Public Const ACTION_TYPE_SEARCH As String = "Search"
    Public Const ACTION_TYPE_INSERT As String = "Insert"
    Public Const CSS_DISPLAY_NONE As String = "dsw-display-none"
    Public Const CSS_DISPLAY_INLINE As String = "dsw-display-inline"
    Public Const CSS_DISPLAY_BLOCK As String = "dsw-display-block"
    Private Const CURRENCY_FORMAT As String = "C"
    Private Const INTEGER_FORMAT As String = "0"
    Private Const FOURDIGITS_DECIMAL_FORMAT As String = "0.0000"

    Private _workflowSignedDocRequired As IDictionary(Of String, Boolean)
#End Region

#Region "Messages"
    Private Const DOCUMENT_TO_SIGN_REQUIRED As String = "<i>(Il documento deve essere firmato)</i>"
#End Region

#Region "Properties"
    Public Event OnNeedDynamicsSource(ByVal sender As Object, ByVal e As EventArgs)

    Private Class UDSTableControlModel
        Public Property Target As TableRowCollection
        Public Property RowIndex As Integer?
        Public Property RowCss As String
        Public Property Colspans As IList(Of Integer)
        Public Property Widths As IList(Of Integer)
        Public Property Controls As IList(Of Control)
        Public Property CellsCss As IList(Of String)
        Public Property LayoutPosition As LayoutPosition
    End Class

    Private Property CurrentControls As ICollection(Of UDSDynamicControlDto)
        Get
            If Session(String.Format("{0}_CurrentControls", ID)) Is Nothing Then
                Session(String.Format("{0}_CurrentControls", ID)) = New List(Of UDSDynamicControlDto)
            End If
            Return DirectCast(Session(String.Format("{0}_CurrentControls", ID)), ICollection(Of UDSDynamicControlDto))
        End Get
        Set(ByVal value As ICollection(Of UDSDynamicControlDto))
            Session(String.Format("{0}_CurrentControls", ID)) = value
        End Set
    End Property

    Private Property CurrentModelControls As UnitaDocumentariaSpecifica
        Get
            If ViewState(String.Format("{0}_CurrentModelControls", ID)) IsNot Nothing Then
                Return DirectCast(ViewState(String.Format("{0}_CurrentModelControls", ID)), UnitaDocumentariaSpecifica)
            End If
            Return Nothing
        End Get
        Set(ByVal value As UnitaDocumentariaSpecifica)
            ViewState(String.Format("{0}_CurrentModelControls", ID)) = value
        End Set
    End Property

    Public Property IsReadOnly As Boolean
        Get
            If ViewState(String.Format("{0}_IsReadOnly", ID)) IsNot Nothing Then
                Return DirectCast(ViewState(String.Format("{0}_IsReadOnly", ID)), Boolean)
            End If
            Return False
        End Get
        Set(ByVal value As Boolean)
            ViewState(String.Format("{0}_IsReadOnly", ID)) = value
        End Set
    End Property

    Public Property ViewMetadata As Boolean
        Get
            If ViewState(String.Format("{0}_ViewMetadata", ID)) IsNot Nothing Then
                Return DirectCast(ViewState(String.Format("{0}_ViewMetadata", ID)), Boolean)
            End If
            Return True
        End Get
        Set(ByVal value As Boolean)
            ViewState(String.Format("{0}_ViewMetadata", ID)) = value
        End Set
    End Property

    Public Property ViewContacts As Boolean
        Get
            If ViewState(String.Format("{0}_ViewContacts", ID)) IsNot Nothing Then
                Return DirectCast(ViewState(String.Format("{0}_ViewContacts", ID)), Boolean)
            End If
            Return True
        End Get
        Set(ByVal value As Boolean)
            ViewState(String.Format("{0}_ViewContacts", ID)) = value
        End Set
    End Property

    Public Property ViewDocuments As Boolean
        Get
            If ViewState(String.Format("{0}_ViewDocuments", ID)) IsNot Nothing Then
                Return DirectCast(ViewState(String.Format("{0}_ViewDocuments", ID)), Boolean)
            End If
            Return True
        End Get
        Set(ByVal value As Boolean)
            ViewState(String.Format("{0}_ViewDocuments", ID)) = value
        End Set
    End Property

    Public Property DocumentsReadonly As Boolean
        Get
            If ViewState(String.Format("{0}_DocumentsReadonly", ID)) IsNot Nothing Then
                Return DirectCast(ViewState(String.Format("{0}_DocumentsReadonly", ID)), Boolean)
            End If
            Return True
        End Get
        Set(ByVal value As Boolean)
            ViewState(String.Format("{0}_DocumentsReadonly", ID)) = value
        End Set
    End Property

    Public Property ViewAuthorizations As Boolean
        Get
            If ViewState(String.Format("{0}_ViewAuthorizations", ID)) IsNot Nothing Then
                Return DirectCast(ViewState(String.Format("{0}_ViewAuthorizations", ID)), Boolean)
            End If
            Return True
        End Get
        Set(ByVal value As Boolean)
            ViewState(String.Format("{0}_ViewAuthorizations", ID)) = value
        End Set
    End Property

    Public Property EnableDefaultData As Boolean
        Get
            If ViewState(String.Format("{0}_EnableDefaultData", ID)) IsNot Nothing Then
                Return DirectCast(ViewState(String.Format("{0}_EnableDefaultData", ID)), Boolean)
            End If
            Return False
        End Get
        Set(ByVal value As Boolean)
            ViewState(String.Format("{0}_EnableDefaultData", ID)) = value
        End Set
    End Property

    Public Property WorkflowSignedDocRequired As IDictionary(Of String, Boolean)
        Get
            If ViewState(String.Format("{0}_WorkflowSignedDocRequired", ID)) IsNot Nothing Then
                Return DirectCast(ViewState(String.Format("{0}_WorkflowSignedDocRequired", ID)), IDictionary(Of String, Boolean))
            End If
            Return New Dictionary(Of String, Boolean)
        End Get
        Set(ByVal value As IDictionary(Of String, Boolean))
            ViewState(String.Format("{0}_WorkflowSignedDocRequired", ID)) = value
        End Set
    End Property

    Public Property ActionType As String
        Get
            If ViewState(String.Format("{0}_ActionType", ID)) Is Nothing Then
                Return Nothing
            End If
            Return DirectCast(ViewState(String.Format("{0}_ActionType", ID)), String)
        End Get
        Set(value As String)
            ViewState(String.Format("{0}_ActionType", ID)) = value
        End Set
    End Property

    Public Property SessionIsEmpty As Boolean

    Private Property SearchableControls As List(Of UDSTableControlModel)

    Public Property UDSFieldListChildren As List(Of KeyValuePair(Of String, Guid))
        Get
            If ViewState(String.Format("{0}_UDSFieldListChildren", ID)) IsNot Nothing Then
                Return DirectCast(ViewState(String.Format("{0}_UDSFieldListChildren", ID)), List(Of KeyValuePair(Of String, Guid)))
            End If
            Return Nothing
        End Get
        Set(ByVal value As List(Of KeyValuePair(Of String, Guid)))
            ViewState(String.Format("{0}_UDSFieldListChildren", ID)) = value
        End Set
    End Property

    Public Property IdUDSRepository As Guid?
        Get
            If ViewState(String.Format("{0}_IdUDSRepository", ID)) IsNot Nothing Then
                Return DirectCast(ViewState(String.Format("{0}_IdUDSRepository", ID)), Guid)
            End If
            Return Nothing
        End Get
        Set(ByVal value As Guid?)
            ViewState(String.Format("{0}_IdUDSRepository", ID)) = value
        End Set
    End Property

    Public Property MyAuthorizedRolesEnabled As Boolean
#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ' SET NO CACHE
        Page.Response.Cache.SetAllowResponseInBrowserHistory(False)
        Page.Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Page.Response.Cache.SetNoStore()
        Page.Response.Cache.SetValidUntilExpires(True)

        InitializeAjax()
        If Not IsPostBack Then
            Me.CurrentControls = Nothing
        End If

        If CurrentModelControls IsNot Nothing Then
            LoadDynamicControls(Me.CurrentModelControls, True)
        End If

        If Not IsPostBack Then
            If EnableDefaultData Then
                LoadDefaultData()
            End If
            RaiseEvent OnNeedDynamicsSource(Me, New EventArgs())
        End If
    End Sub

#Region "Save/Load UserControls state"
    'todo: spostare tutta la logica del SaveControlState per ogni singolo user control
    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsReadOnly Then
            Page.RegisterRequiresControlState(Me)
        End If
    End Sub

    Protected Overrides Function SaveControlState() As Object
        Dim source As ICollection(Of UDSDynamicControlDto) = New List(Of UDSDynamicControlDto)
        For Each control As UDSDynamicControlDto In Me.CurrentControls
            Select Case control.DynamicControlName
                Case GetType(uscDocumentUpload).Name
                    Dim dynamicControl As uscDocumentUpload = DirectCast(dynamicControls.FindControl(control.IdControl), uscDocumentUpload)
                    Dim customProperties As IDictionary(Of String, Object) = New Dictionary(Of String, Object)
                    customProperties.Add(NameOf(dynamicControl.DocumentsToDelete), dynamicControl.DocumentsToDelete)
                    source.Add(New UDSDynamicControlDto() With {.IdControl = control.IdControl, .Value = dynamicControl.TreeViewControl.GetXml(), .CustomProperties = customProperties})
                Case GetType(uscSettori).Name
                    Dim dynamicControl As uscSettori = DirectCast(dynamicControls.FindControl(control.IdControl), uscSettori)
                    Dim roles As IList(Of Data.Role) = dynamicControl.GetRoles()
                    Dim users As Dictionary(Of String, String) = dynamicControl.GetUsers().ToDictionary(Function(x) x.Key, Function(x) x.Value)
                    source.Add(New UDSDynamicControlDto() With {.IdControl = control.IdControl, .DynamicControlName = control.DynamicControlName, .Value = roles})
                    source.Add(New UDSDynamicControlDto() With {.IdControl = control.IdControl, .DynamicControlName = control.DynamicControlName, .Value = users})
                Case GetType(uscContattiSel).Name
                    Dim dynamicControl As uscContattiSel = DirectCast(dynamicControls.FindControl(control.IdControl), uscContattiSel)
                    Dim contacts As IList(Of Data.ContactDTO) = dynamicControl.GetContacts(False)
                    source.Add(New UDSDynamicControlDto() With {.IdControl = control.IdControl, .DynamicControlName = control.DynamicControlName, .Value = contacts})
                Case GetType(RadEditor).Name
                    Dim dynamicControl As RadEditor = DirectCast(dynamicControls.FindControl(control.IdControl), RadEditor)
                    Dim content As String = dynamicControl.Content
                    source.Add(New UDSDynamicControlDto() With {.IdControl = control.IdControl, .DynamicControlName = control.DynamicControlName, .Value = content})
            End Select
        Next
        Me.CurrentControls = Nothing
        Return source
    End Function

    Protected Overrides Sub LoadControlState(savedState As Object)
        Dim source As ICollection(Of UDSDynamicControlDto) = New List(Of UDSDynamicControlDto)
        source = DirectCast(savedState, ICollection(Of UDSDynamicControlDto))
        Dim contacts As IList(Of Data.ContactDTO)
        Dim roles As IList(Of Data.Role)
        Dim tmpRoles As IList(Of Data.Role)
        Dim users As Dictionary(Of String, String)
        Dim tmpUsers As Dictionary(Of String, String)

        For Each val As UDSDynamicControlDto In source

            If (val.DynamicControlName.Eq(GetType(uscContattiSel).Name)) AndAlso val.Value IsNot Nothing Then
                contacts = DirectCast(val.Value, IList(Of Data.ContactDTO))
                For Each contact As Data.ContactDTO In contacts.Where(Function(x) x.Type.Equals(Data.ContactDTO.ContactType.Address))
                    contact.Contact = Facade.ContactFacade.GetById(contact.Contact.Id)
                Next
                val.Value = contacts
            End If

            If (val.DynamicControlName.Eq(GetType(uscSettori).Name)) AndAlso val.Value IsNot Nothing Then
                If TypeOf val.Value Is IList(Of Data.Role) Then
                    roles = DirectCast(val.Value, IList(Of Data.Role))
                    tmpRoles = New List(Of Data.Role)(roles.Count)
                    For Each role As Data.Role In roles
                        tmpRoles.Add(Facade.RoleFacade.GetById(role.Id))
                    Next
                    val.Value = tmpRoles
                ElseIf TypeOf val.Value Is Dictionary(Of String, String) Then
                    users = DirectCast(val.Value, Dictionary(Of String, String))
                    tmpUsers = New Dictionary(Of String, String)(users.Count)
                    For Each user As KeyValuePair(Of String, String) In users
                        tmpUsers.Add(user.Key, user.Value)
                    Next
                    val.Value = tmpUsers
                End If
            End If

            Me.CurrentControls.Add(New UDSDynamicControlDto() With {.IdControl = val.IdControl, .Value = val.Value, .CustomProperties = val.CustomProperties})
        Next
    End Sub
#End Region

#End Region

#Region "Methods"

    Public Sub ResetState()
        Me.CurrentControls = Nothing
    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, dynamicControls)
    End Sub

    ''' <summary>
    ''' Dato un modello inizializzo lo user control caricando i dati dinamici impostati nel modello stesso
    ''' </summary>
    ''' <param name="udsModel">modello della pagina di tipo UnitaDocumentariaSpecifica</param>
    Public Sub LoadDynamicControls(udsModel As UnitaDocumentariaSpecifica, showRole As Boolean)
        If udsModel Is Nothing OrElse udsModel.Metadata Is Nothing OrElse udsModel.Metadata(0).Items Is Nothing Then
            Exit Sub
        End If

        ' Metodo per cancellazione dei dati in ViewState dei controlli figli di DynamicControl
        dynamicControls.Controls.Clear()

        Dim maxColumn As UInteger
        Dim authorizationsColumn As UInteger
        Dim contactsColumn As UInteger

        If udsModel.Metadata(0).Items(0).Layout IsNot Nothing Then
            Dim noOfColumns As UInteger = udsModel.Metadata(0).Items.Max(Function(x) x.Layout.ColNumber)

            Dim documentsColumn As UInteger = udsModel.Documents.Document.Layout.ColNumber

            authorizationsColumn = CUInt(If(udsModel.Authorizations IsNot Nothing AndAlso udsModel.Authorizations.Layout IsNot Nothing, udsModel.Authorizations.Layout.ColNumber, 0))

            contactsColumn = CUInt(If(udsModel.Contacts IsNot Nothing AndAlso udsModel.Contacts.All(Function(x) x.Layout IsNot Nothing), udsModel.Contacts.Max(Function(x) x.Layout.ColNumber), 0))

            maxColumn = {noOfColumns, documentsColumn, authorizationsColumn, contactsColumn}.Max(Function(x) x)
        Else
            maxColumn = 0
        End If

        Dim table1 As Table = Nothing
        Dim table2 As Table = Nothing
        Dim table3 As Table = Nothing

        If maxColumn = 0 Then
            table1 = CreateDynamicTable(Nothing, Nothing, maxColumn)
        End If
        If maxColumn = 1 Then
            table1 = CreateDynamicTable(Nothing, Nothing, maxColumn)
            table2 = CreateDynamicTable(Nothing, Nothing, maxColumn)
        End If
        If maxColumn = 2 Then
            table1 = CreateDynamicTable(Nothing, Nothing, maxColumn)
            table2 = CreateDynamicTable(Nothing, Nothing, maxColumn)
            table3 = CreateDynamicTable(Nothing, Nothing, maxColumn)
        End If

        SearchableControls = New List(Of UDSTableControlModel)()

        If ViewMetadata Then
            'Aggiungo i field dinamici
            Dim DocumentRowNumber As UInteger = 1
            Dim AuthorizationsRowNumber As UInteger = 2
            For Each element As Section In udsModel.Metadata
                If element.Items IsNot Nothing Then

                    If element.Items(0).Layout Is Nothing Then
                        For index As Integer = 0 To element.Items.Length - 1
                            element.Items(index).Layout = New LayoutPosition()
                            element.Items(index).Layout.RowNumber = CUInt(index)
                            element.Items(index).Layout.ColNumber = 0
                        Next
                        If udsModel.Contacts IsNot Nothing Then
                            For index As Integer = 0 To udsModel.Contacts.Length - 1
                                udsModel.Contacts(index).Layout = New LayoutPosition()
                                udsModel.Contacts(index).Layout.RowNumber = CUInt(element.Items.Length + index)
                                udsModel.Contacts(index).Layout.ColNumber = 0
                            Next
                        End If
                        If udsModel.Documents IsNot Nothing Then
                            udsModel.Documents.Document.Layout = New LayoutPosition()
                            If udsModel.Contacts IsNot Nothing Then
                                udsModel.Documents.Document.Layout.RowNumber = CUInt(element.Items.Length + udsModel.Contacts.Length)
                            Else
                                udsModel.Documents.Document.Layout.RowNumber = DocumentRowNumber
                            End If
                            udsModel.Documents.Document.Layout.ColNumber = 0
                        End If
                        If udsModel.Authorizations IsNot Nothing Then
                            udsModel.Authorizations.Layout = New LayoutPosition()
                            If udsModel.Contacts IsNot Nothing Then
                                udsModel.Authorizations.Layout.RowNumber = CUInt(element.Items.Length + udsModel.Contacts.Length + 1)
                            Else
                                udsModel.Authorizations.Layout.RowNumber = AuthorizationsRowNumber
                            End If
                            udsModel.Authorizations.Layout.ColNumber = 0
                        End If
                    End If

                    For Each item As Object In element.Items.ToList().OrderBy(Function(x) x.Layout.RowNumber).ThenBy(Function(y) y.Layout.ColNumber)
                        Dim itemReadonly As Object = ReadOnlyAfterValueDefined(item)
                        Dim elementName As String = itemReadonly.GetType().Name
                        Dim correctTable As Table = If(item.Layout.ColNumber = 0, table1, If(item.Layout.ColNumber = 1, table2, table3))
                        Select Case elementName
                            Case CTL_ENUM
                                AddEnumControl(correctTable, DirectCast(itemReadonly, EnumField))
                            Case CTL_STATUS
                                AddStatusControl(correctTable, DirectCast(itemReadonly, StatusField))
                            Case CTL_TEXT
                                AddTextControl(correctTable, DirectCast(itemReadonly, TextField))
                            Case CTL_NUMBER
                                AddNumberControl(correctTable, DirectCast(itemReadonly, NumberField))
                            Case CTL_DATE
                                AddDateControl(correctTable, DirectCast(itemReadonly, DateField))
                            Case CTL_CHECKBOX
                                AddCheckBoxControl(correctTable, DirectCast(itemReadonly, BoolField))
                            Case CTL_LOOKUP
                                AddLookupControl(correctTable, DirectCast(itemReadonly, LookupField))
                            Case CTL_TREE_LIST_FIELD
                                AddTreeListControl(correctTable, DirectCast(itemReadonly, TreeListField))
                        End Select
                    Next
                End If
                DocumentRowNumber = CUInt(DocumentRowNumber + 2)
                AuthorizationsRowNumber = CUInt(AuthorizationsRowNumber + 2)
            Next
        End If

        If ViewContacts Then
            'Aggiungo i contatti
            If udsModel.Contacts IsNot Nothing Then
                For Each contact As Contacts In udsModel.Contacts
                    Dim correctTable As Table = If(contact.Layout.ColNumber = 0, table1, If(contact.Layout.ColNumber = 1, table2, table3))
                    If ActionType.Eq(ACTION_TYPE_VIEW) Then
                        correctTable = CreateDynamicTable(True, contact.Label)
                    End If
                    AddContactControl(correctTable, contact)
                Next
            End If
        End If

        If ViewDocuments Then
            'Aggiungo i documenti
            If udsModel.Documents IsNot Nothing Then
                Dim correctTable As Table = If(udsModel.Documents.Document.Layout.ColNumber = 0, table1, If(udsModel.Documents.Document.Layout.ColNumber = 1, table2, table3))
                'Documento principale
                If udsModel.Documents.Document IsNot Nothing Then
                    AddDocumentControl(correctTable, udsModel.Documents.Document, Not udsModel.StampaConformeEnabled)
                End If

                'Allegati
                If udsModel.Documents.DocumentAttachment IsNot Nothing Then
                    AddDocumentControl(correctTable, udsModel.Documents.DocumentAttachment, Not udsModel.StampaConformeEnabled)
                End If

                'Annessi
                If udsModel.Documents.DocumentAnnexed IsNot Nothing Then
                    AddDocumentControl(correctTable, udsModel.Documents.DocumentAnnexed, Not udsModel.StampaConformeEnabled)
                End If
            End If
        End If

        If ViewAuthorizations AndAlso showRole Then
            If udsModel.Authorizations IsNot Nothing Then
                Dim correctTable As Table = If(udsModel.Authorizations.Layout.ColNumber = 0, table1, If(udsModel.Authorizations.Layout.ColNumber = 1, table2, table3))
                If ActionType.Eq(ACTION_TYPE_VIEW) Then
                    correctTable = CreateDynamicTable(True, udsModel.Authorizations.Label)
                End If
                AddRoleControl(correctTable, udsModel.Authorizations)
            End If
        End If

        If ActionType.Eq(ACTION_TYPE_SEARCH) Then
            RenderUDSSearchControls(table1, table2, table3)
        End If

        'Salvo il modello corrente nel viewState perchè possa essere interpretato ad ogni postback
        Me.CurrentModelControls = udsModel
    End Sub

    Private Function ReadOnlyAfterValueDefined(item As Object) As Object
        If ActionType.Eq(ACTION_TYPE_INSERT) Then
            Dim elementName As String = item.GetType().Name
            Select Case elementName
                Case CTL_ENUM
                    Dim enumReadonly As EnumField = DirectCast(item, EnumField)
                    If enumReadonly.DefaultValue = String.Empty AndAlso enumReadonly.ReadOnly = True Then
                        enumReadonly.ReadOnly = False
                        enumReadonly.ModifyEnabled = True
                        Return enumReadonly
                    End If
                Case CTL_STATUS
                    Dim statusReadonly As StatusField = DirectCast(item, StatusField)
                    If statusReadonly.DefaultValue = String.Empty AndAlso statusReadonly.ReadOnly = True Then
                        statusReadonly.ReadOnly = False
                        statusReadonly.ModifyEnabled = True
                        Return statusReadonly
                    End If
                Case CTL_TEXT
                    Dim textReadonly As TextField = DirectCast(item, TextField)
                    If textReadonly.DefaultValue = String.Empty AndAlso textReadonly.ReadOnly = True Then
                        textReadonly.ReadOnly = False
                        textReadonly.ModifyEnabled = True
                        Return textReadonly
                    End If
                Case CTL_NUMBER
                    Dim numberReadonly As NumberField = DirectCast(item, NumberField)
                    If numberReadonly.DefaultValueSpecified = False AndAlso numberReadonly.ReadOnly = True Then
                        numberReadonly.ReadOnly = False
                        numberReadonly.ModifyEnabled = True
                        Return numberReadonly
                    End If
                Case CTL_DATE
                    Dim dateReadonly As DateField = DirectCast(item, DateField)
                    Dim dt As Date? = New DateTime
                    If dt.Equals(dateReadonly.DefaultValue) AndAlso dateReadonly.ReadOnly = True Then
                        dateReadonly.ReadOnly = False
                        dateReadonly.ModifyEnabled = True
                        Return dateReadonly
                    End If
                Case CTL_CHECKBOX
                    Dim boolReadonly As BoolField = DirectCast(item, BoolField)
                    If boolReadonly.DefaultValueSpecified = False AndAlso boolReadonly.ReadOnly = True Then
                        boolReadonly.ReadOnly = False
                        boolReadonly.ModifyEnabled = True
                        Return boolReadonly
                    End If
                Case CTL_LOOKUP
                    Dim lookupReadonly As LookupField = DirectCast(item, LookupField)
                    If lookupReadonly.Value = String.Empty AndAlso lookupReadonly.ReadOnly = True Then
                        lookupReadonly.ReadOnly = False
                        lookupReadonly.ModifyEnabled = True
                        Return lookupReadonly
                    End If
            End Select
        End If
        Return item 'If the item is not for UDSInsert.aspx, leave it untouched
    End Function

    ''' <summary>
    ''' Carica i valori di default
    ''' </summary>
    Public Sub LoadDefaultData()
        SetUDSValues(Me.CurrentModelControls)
    End Sub

    ''' <summary>
    ''' Imposta i valori nella pagina dell'UDS specificata
    ''' </summary>
    Public Sub SetUDSValues(model As UnitaDocumentariaSpecifica)
        'Setto i valori per le sezioni dinamiche
        Dim modelField As UDSModelField = Nothing
        Dim ctrl As Control = Nothing
        Dim enableControl As Boolean = True
        Dim statusValue As String
        Dim statusLblControl As Control
        For Each element As Section In model.Metadata
            If element.Items IsNot Nothing Then
                For Each item As FieldBaseType In element.Items
                    modelField = New UDSModelField(item)
                    ctrl = dynamicControls.FindControl(WebHelper.SafeControlIdName(String.Format(DYNAMIC_FIELD_NAME_FORMAT, modelField.ColumnName)))
                    If ctrl IsNot Nothing Then
                        Dim realValue As Object = modelField.Value

                        Dim allowFormatValue As Boolean = False
                        Dim realValueIsNull As Boolean = realValue Is Nothing OrElse (TypeOf realValue Is String AndAlso String.IsNullOrEmpty(CType(realValue, String)))
                        realValue = If(realValueIsNull AndAlso ActionType.Eq(ACTION_TYPE_INSERT), modelField.DefaultValue, realValue)

                        enableControl = (Not IsReadOnly) AndAlso (Not item.HiddenField)
                        If ActionType.Eq(ACTION_TYPE_EDIT) AndAlso (Not item.ModifyEnabled) AndAlso Not realValueIsNull Then
                            enableControl = False
                        End If
                        Dim controlName As String = ctrl.GetType().Name
                        If Not IsReadOnly AndAlso TypeOf item Is LookupField Then
                            controlName = "uscUDSLookup"
                        End If
                        If Not IsReadOnly AndAlso TypeOf item Is TreeListField Then
                            controlName = "uscUDSFieldListTree"
                        End If
                        If IsReadOnly AndAlso TypeOf item Is DateField Then
                            allowFormatValue = True
                        End If

                        If IsReadOnly AndAlso TypeOf item Is StatusField Then
                            statusValue = CType(realValue, String)
                            realValue = CType(item, StatusField).Options.FirstOrDefault(Function(f) f.Value = statusValue)
                            statusLblControl = dynamicControls.FindControl(WebHelper.SafeControlIdName(String.Format(DYNAMIC_LABEL_NAME_FORMAT, String.Concat(modelField.ColumnName, "_status"))))
                            Try
                                ''add try checker to workaround used from collaboration summmary when exapnd archive has status metadata defined.
                                Dim parent As Control = statusLblControl.Parent.Parent
                                parent.Controls.Add(statusLblControl)
                            Catch
                            End Try
                            SetControlValue(statusLblControl, GetType(Label).Name, statusValue, modifiable:=enableControl)
                        End If
                        If ActionType.Eq(ACTION_TYPE_INSERT) AndAlso TypeOf item Is DateField Then
                            If DirectCast(item, DateField).DefaultTodayEnabled Then
                                realValue = Date.Now
                            End If
                        End If
                        SetControlValue(ctrl, controlName, realValue, modifiable:=enableControl, allowFormatValue:=allowFormatValue)
                    End If
                Next
            End If
        Next

        'Aggiungo i contatti
        If model.Contacts IsNot Nothing Then
            For Each contact As Contacts In model.Contacts
                ctrl = dynamicControls.FindControl(WebHelper.SafeControlIdName(String.Format(DYNAMIC_FIELD_NAME_FORMAT, contact.Label)))
                If ctrl IsNot Nothing Then
                    Dim realValueIsNull As Boolean = (contact.ContactInstances Is Nothing) AndAlso (contact.ContactManualInstances Is Nothing)
                    enableControl = Not IsReadOnly
                    If ActionType.Eq(ACTION_TYPE_EDIT) AndAlso (Not contact.ModifyEnabled) AndAlso Not realValueIsNull Then
                        enableControl = False
                    End If
                    SetControlValue(ctrl, "uscContattiSel", contact, modifiable:=enableControl)
                End If
            Next
        End If

        'Aggiungo i documenti
        If model.Documents IsNot Nothing Then
            'Documento principale
            If model.Documents.Document IsNot Nothing Then
                ctrl = dynamicControls.FindControl(WebHelper.SafeControlIdName(String.Format(DYNAMIC_FIELD_NAME_FORMAT, model.Documents.Document.Label)))
                If ctrl IsNot Nothing Then
                    enableControl = Not IsReadOnly
                    If ActionType.Eq(ACTION_TYPE_EDIT) AndAlso (Not model.Documents.Document.ModifyEnabled) AndAlso model.Documents.Document.Instances IsNot Nothing Then
                        enableControl = False
                    End If
                    SetControlValue(ctrl, "uscDocumentUpload", model.Documents.Document, modifiable:=enableControl)
                End If
            End If

            'Allegati
            If model.Documents.DocumentAttachment IsNot Nothing Then
                ctrl = dynamicControls.FindControl(WebHelper.SafeControlIdName(String.Format(DYNAMIC_FIELD_NAME_FORMAT, model.Documents.DocumentAttachment.Label)))
                If ctrl IsNot Nothing Then
                    enableControl = Not IsReadOnly
                    If ActionType.Eq(ACTION_TYPE_EDIT) AndAlso (Not model.Documents.DocumentAttachment.ModifyEnabled) AndAlso model.Documents.DocumentAttachment.Instances IsNot Nothing Then
                        enableControl = False
                    End If
                    SetControlValue(ctrl, "uscDocumentUpload", model.Documents.DocumentAttachment, modifiable:=enableControl)
                End If
            End If

            'Annessi
            If model.Documents.DocumentAnnexed IsNot Nothing Then
                ctrl = dynamicControls.FindControl(WebHelper.SafeControlIdName(String.Format(DYNAMIC_FIELD_NAME_FORMAT, model.Documents.DocumentAnnexed.Label)))
                If ctrl IsNot Nothing Then
                    enableControl = Not IsReadOnly
                    If ActionType.Eq(ACTION_TYPE_EDIT) AndAlso (Not model.Documents.DocumentAnnexed.ModifyEnabled) AndAlso model.Documents.DocumentAnnexed.Instances IsNot Nothing Then
                        enableControl = False
                    End If
                    SetControlValue(ctrl, "uscDocumentUpload", model.Documents.DocumentAnnexed, modifiable:=enableControl)
                End If
            End If
        End If

        'Setto le autorizzazioni
        If model.Authorizations IsNot Nothing Then
            ctrl = dynamicControls.FindControl(WebHelper.SafeControlIdName(String.Format(DYNAMIC_FIELD_NAME_FORMAT, model.Authorizations.Label)))
            If ctrl IsNot Nothing Then
                enableControl = Not IsReadOnly
                If ActionType.Eq(ACTION_TYPE_EDIT) AndAlso (Not model.Authorizations.ModifyEnabled) AndAlso model.Authorizations.Instances IsNot Nothing Then
                    enableControl = False
                End If
                SetControlValue(ctrl, "uscSettori", model.Authorizations.Instances, modifiable:=enableControl)
            End If
        End If
    End Sub

    Public Function GetUDSValues() As UnitaDocumentariaSpecifica
        If Me.CurrentModelControls Is Nothing Then
            Return Nothing
        End If

        'Recupero i valori per le sezioni dinamiche
        For Each element As Section In Me.CurrentModelControls.Metadata
            If element.Items IsNot Nothing Then
                For Each item As Object In element.Items
                    Dim modelField As UDSModelField = New UDSModelField(item)
                    Dim ctrl As Control = dynamicControls.FindControl(WebHelper.SafeControlIdName(String.Format(DYNAMIC_FIELD_NAME_FORMAT, modelField.ColumnName)))
                    If ctrl IsNot Nothing Then
                        Dim controlName As String = ctrl.GetType().Name
                        If item.GetType() = GetType(LookupField) AndAlso Not IsReadOnly Then
                            controlName = "uscUDSLookup"
                        End If
                        If item.GetType() = GetType(TreeListField) AndAlso Not IsReadOnly Then
                            controlName = "uscUDSFieldListTree"
                        End If
                        If item.GetType() = GetType(BoolField) AndAlso (ActionType.Eq(ACTION_TYPE_SEARCH) OrElse ActionType.Eq(ACTION_TYPE_INSERT) OrElse ActionType.Eq(ACTION_TYPE_EDIT)) Then
                            controlName = "checkTodrop"
                        End If
                        FillPageControl(ctrl, controlName, item)
                    End If

                Next
            End If
        Next

        'Recupero i contatti
        If Me.CurrentModelControls IsNot Nothing AndAlso Me.CurrentModelControls.Contacts IsNot Nothing Then
            For Each contact As Contacts In Me.CurrentModelControls.Contacts
                Dim ctrl As Control = dynamicControls.FindControl(WebHelper.SafeControlIdName(String.Format(DYNAMIC_FIELD_NAME_FORMAT, contact.Label)))
                If ctrl IsNot Nothing Then
                    FillPageControl(ctrl, "uscContattiSel", contact)
                End If
            Next
        End If

        'Recupero i documenti
        If Me.CurrentModelControls.Documents IsNot Nothing Then
            'Documento principale
            If Me.CurrentModelControls.Documents.Document IsNot Nothing Then
                Dim ctrl As Control = dynamicControls.FindControl(WebHelper.SafeControlIdName(String.Format(DYNAMIC_FIELD_NAME_FORMAT, Me.CurrentModelControls.Documents.Document.Label)))
                If ctrl IsNot Nothing Then
                    FillPageControl(ctrl, "uscDocumentUpload", CurrentModelControls.Documents.Document)
                End If
            End If

            'Allegati
            If Me.CurrentModelControls.Documents.DocumentAttachment IsNot Nothing Then
                Dim ctrl As Control = dynamicControls.FindControl(WebHelper.SafeControlIdName(String.Format(DYNAMIC_FIELD_NAME_FORMAT, Me.CurrentModelControls.Documents.DocumentAttachment.Label)))
                If ctrl IsNot Nothing Then
                    FillPageControl(ctrl, "uscDocumentUpload", CurrentModelControls.Documents.DocumentAttachment)
                End If
            End If

            'Annessi
            If Me.CurrentModelControls.Documents.DocumentAnnexed IsNot Nothing Then
                Dim ctrl As Control = dynamicControls.FindControl(WebHelper.SafeControlIdName(String.Format(DYNAMIC_FIELD_NAME_FORMAT, Me.CurrentModelControls.Documents.DocumentAnnexed.Label)))
                If ctrl IsNot Nothing Then
                    FillPageControl(ctrl, "uscDocumentUpload", CurrentModelControls.Documents.DocumentAnnexed)
                End If
            End If
        End If

        'Recupero le autorizzazioni
        If Me.CurrentModelControls.Authorizations IsNot Nothing Then
            Dim ctrl As Control = dynamicControls.FindControl(WebHelper.SafeControlIdName(String.Format(DYNAMIC_FIELD_NAME_FORMAT, Me.CurrentModelControls.Authorizations.Label)))
            If ctrl IsNot Nothing Then
                FillPageControl(ctrl, "uscSettori", CurrentModelControls.Authorizations)
            End If
        End If

        Return Me.CurrentModelControls
    End Function

    ''' <summary>
    ''' Creo dinamicamente la tabella in cui inserire tutti i dati dinamici della pagina
    ''' </summary>
    Private Function CreateDynamicTable(Optional withHeader As Boolean = False, Optional headerLabel As String = "", Optional column As UInteger = 0) As Table
        Dim table As Table = New Table() With {.CssClass = String.Format(" datatable udsDataTable border-bottom-collapse layout{0}", column), .EnableViewState = False}
        If ActionType.Eq(ACTION_TYPE_VIEW) Then
            table.CssClass = String.Format(" datatable udsDataTable border-bottom-collapse layoutView{0}", column)
            If withHeader Then
                Dim header As TableHeaderRow = New TableHeaderRow()
                Dim cellHeader As TableHeaderCell = New TableHeaderCell()
                cellHeader.ColumnSpan = 2
                cellHeader.Text = headerLabel
                header.Cells.Add(cellHeader)
                Dim Label As Label = New Label()
                Label.Text = headerLabel
                table.Rows.Add(header)
            End If
        End If

        dynamicControls.Controls.Add(table)
        Return table
    End Function

    Private Function GetLabelName(name As String) As String
        Return String.Format("{0}:", name)
    End Function

    ''' <summary>
    ''' Aggiunge un nuovo controllo Validator
    ''' </summary>
    Public Sub AddValidatorControl(table As Table, parentId As String, parentName As String)
        Dim control As Control = dynamicControls.FindControl(WebHelper.SafeControlIdName(parentId))
        Dim tdControl As Control = New Control()
        Dim validator As Control = New ValidatorStructure().GetStructure(String.Format(DYNAMIC_VALIDATOR_NAME_FORMAT, parentId), String.Format("Campo {0} Obbligatorio", parentName), parentId, ValidatorDisplay.Dynamic, String.Empty)
        table.Rows.AddRaw("Chiaro", Nothing, Nothing, {tdControl, validator}, {"col-dsw-2", "col-dsw-8"})
    End Sub

    ''' <summary>
    ''' Aggiunge un nuovo controllo custom Validator
    ''' </summary>
    Public Sub AddCustomValidatorControl(table As Table, parentId As String, validationCallback As String, parentName As String)
        Dim control As Control = dynamicControls.FindControl(WebHelper.SafeControlIdName(parentId))
        Dim tdControl As Control = New Control()
        Dim validator As Control = New ValidatorStructure().GetCustomStructure(String.Format(DYNAMIC_VALIDATOR_NAME_FORMAT, parentId), String.Format("Campo {0} Obbligatorio", parentName), parentId, validationCallback, ValidatorDisplay.Dynamic, String.Empty)
        table.Rows.AddRaw("Chiaro", Nothing, Nothing, {tdControl, validator}, {"col-dsw-2", "col-dsw-8"})
    End Sub

    Public Sub AddNumericRangeValidatorControl(table As Table, parentId As String, parentName As String, minValue As Double, maxValue As Double)
        Dim control As Control = dynamicControls.FindControl(WebHelper.SafeControlIdName(parentId))
        Dim tdControl As Control = New Control()
        Dim validator As Control = New ValidatorStructure().GetRangeStructure(String.Format(RANGE_VALIDATOR_NAME_FORMAT, parentId), $"Inserire un valore compreso tra {minValue} e {maxValue}", parentId, minValue, maxValue, ValidatorDisplay.Dynamic, ValidationDataType.Double, String.Empty)
        table.Rows.AddRaw("Chiaro", Nothing, Nothing, {tdControl, validator}, {"col-dsw-2", "col-dsw-8"})
    End Sub

    ''' <summary>
    ''' Aggiunge un nuovo controllo Enumeratore
    ''' </summary>
    Public Sub AddEnumControl(table As Table, element As EnumField)
        If ActionType.Eq(ACTION_TYPE_SEARCH) AndAlso Not element.Searchable Then
            Return
        End If

        Dim comboValues As IDictionary(Of String, String) = New Dictionary(Of String, String)
        If element.Options IsNot Nothing Then
            For Each value As String In element.Options
                comboValues.Add(value, value)
            Next
        End If

        comboValues = comboValues.OrderBy(Function(x) x.Key).ToDictionary(Function(p) p.Key, Function(p) p.Value)

        Dim label As Control = New LabelStructure().GetStructure(String.Format(DYNAMIC_LABEL_NAME_FORMAT, element.ColumnName), GetLabelName(element.Label), String.Empty)
        Dim comboControlId As String = String.Format(DYNAMIC_FIELD_NAME_FORMAT, element.ColumnName).ToLower()
        Dim comboControl As Control
        If IsReadOnly Then
            comboControl = New LabelStructure().GetStructure(comboControlId, CType(ViewState(comboControlId), String), String.Empty)
            Dim enumLabel As Label = DirectCast(comboControl, Label)
            enumLabel.AddAttribute("IsJSONValue", "True")
        Else
            Dim controlIsRequierd As Boolean = element.Required AndAlso Not IsReadOnly AndAlso Not ActionType.Eq(ACTION_TYPE_VIEW)
            Dim errorMessage As String = $"Campo {element.Label} Obbligatorio"

            If element.MultipleValues Then
                comboControl = New ComboStructure().GetRadComboBoxStructure(comboControlId, New KeyValuePair(Of Integer, UnitType)(200, UnitType.Pixel), comboValues,
                                                                    String.Empty, ActionType.Eq(ACTION_TYPE_SEARCH) OrElse Not element.ReadOnly,
                                                                    controlIsRequierd, errorMessage, CSS_DISPLAY_BLOCK)
                Dim radComboBox As RadComboBox = DirectCast(comboControl.Controls(0), RadComboBox)
                radComboBox.CheckBoxes = True
            Else
                comboControl = New ComboStructure().GetRadStructure(comboControlId, New KeyValuePair(Of Integer, UnitType)(200, UnitType.Pixel), comboValues,
                                                                    String.Empty, ActionType.Eq(ACTION_TYPE_SEARCH) OrElse Not element.ReadOnly,
                                                                    controlIsRequierd, errorMessage, CSS_DISPLAY_BLOCK)
            End If
        End If

        Dim cssRow As String = "Chiaro"
        If element.HiddenField Then
            cssRow = CSS_DISPLAY_NONE
        End If

        If ActionType.Eq(ACTION_TYPE_SEARCH) Then
            SearchableControls.Add(New UDSTableControlModel With {
                .RowCss = cssRow,
                .Controls = {label, comboControl},
                .CellsCss = {"col-dsw-2 label", "col-dsw-8"},
                .LayoutPosition = element.Layout
            })
        Else
            table.Rows.AddRaw(cssRow, Nothing, Nothing, {label, comboControl}, {"col-dsw-2 label", "col-dsw-8"})
        End If
    End Sub

    ''' <summary>
    ''' Aggiunge un nuovo controllo Stato
    ''' </summary>
    Public Sub AddStatusControl(table As Table, element As StatusField)
        If ActionType.Eq(ACTION_TYPE_SEARCH) AndAlso Not element.Searchable Then
            Return
        End If
        Dim comboValues As IDictionary(Of String, String) = New Dictionary(Of String, String)
        Dim controls As IList(Of Control) = New List(Of Control)()
        If element.Options IsNot Nothing Then
            For Each value As StatusType In element.Options
                comboValues.Add(value.Value, value.Value)
            Next
        End If
        controls.Add(New LabelStructure().GetStructure(String.Format(DYNAMIC_LABEL_NAME_FORMAT, element.ColumnName), GetLabelName(element.Label), String.Empty))
        Dim statusControlId As String = String.Format(DYNAMIC_FIELD_NAME_FORMAT, element.ColumnName).ToLower()
        Dim statusControl As Control
        If IsReadOnly Then
            statusControl = New ImageStructure().GetStructure(statusControlId, String.Empty, String.Empty)
            statusControl.Controls.Add(New LabelStructure().GetStructure(String.Format(DYNAMIC_LABEL_NAME_FORMAT, String.Concat(element.ColumnName, "_status")),
                                                                         GetLabelName(element.Label), String.Empty))
        Else
            Dim controlIsRequierd As Boolean = element.Required AndAlso Not IsReadOnly AndAlso Not ActionType.Eq(ACTION_TYPE_VIEW)
            Dim errorMessage As String = $"Campo {element.Label} Obbligatorio"

            statusControl = New ComboStructure().GetRadStructure(statusControlId, New KeyValuePair(Of Integer, UnitType)(200, UnitType.Pixel), comboValues,
                                                                 String.Empty, ActionType.Eq(ACTION_TYPE_SEARCH) OrElse Not element.ReadOnly,
                                                                 controlIsRequierd, errorMessage, CSS_DISPLAY_BLOCK)
        End If
        controls.Add(statusControl)
        Dim cssRow As String = "Chiaro"
        If element.HiddenField Then
            cssRow = CSS_DISPLAY_NONE
        End If

        If ActionType.Eq(ACTION_TYPE_SEARCH) Then
            SearchableControls.Add(New UDSTableControlModel With {
                .RowCss = cssRow,
                .Controls = controls,
                .CellsCss = {"col-dsw-2 label", "col-dsw-8"},
                .LayoutPosition = element.Layout
            })
        Else
            table.Rows.AddRaw(cssRow, Nothing, Nothing, controls, {"col-dsw-2 label", "col-dsw-8"})
        End If
    End Sub

    ''' <summary>
    ''' Aggiunge un nuovo controllo TextBox
    ''' </summary>
    Public Sub AddTextControl(table As Table, element As TextField)
        If ActionType.Eq(ACTION_TYPE_SEARCH) AndAlso Not element.Searchable Then
            Return
        End If

        Dim label As Control = New LabelStructure().GetStructure(String.Format(DYNAMIC_LABEL_NAME_FORMAT, element.ColumnName), GetLabelName(element.Label), String.Empty)
        Dim textBoxControlId As String = String.Format(DYNAMIC_FIELD_NAME_FORMAT, element.ColumnName).ToLower()
        Dim textBoxControl As Control
        If IsReadOnly Then
            If element.HTMLEnable Then
                Return
            End If
            textBoxControl = New LabelStructure().GetStructure(textBoxControlId, CType(ViewState(textBoxControlId), String), String.Empty)
        Else
            Dim controlIsRequierd As Boolean = element.Required AndAlso Not IsReadOnly AndAlso Not ActionType.Eq(ACTION_TYPE_VIEW)
            Dim errorMessage As String = $"Campo {element.Label} Obbligatorio"

            Dim txtMode As InputMode = If(element.Multiline, InputMode.MultiLine, InputMode.SingleLine)
            If element.HTMLEnable AndAlso (ActionType.Eq(ACTION_TYPE_EDIT) OrElse ActionType.Eq(ACTION_TYPE_INSERT)) Then
                textBoxControl = New TextStructure().GetRadEditorStructure(textBoxControlId, New KeyValuePair(Of Integer, UnitType)(100, UnitType.Percentage), Nothing,
                                                                     CType(ViewState(textBoxControlId), String), txtMode, String.Empty,
                                                                    ActionType.Eq(ACTION_TYPE_SEARCH) OrElse Not element.ReadOnly,
                                                                    controlIsRequierd, errorMessage, CSS_DISPLAY_BLOCK)
                Dim existControl As UDSDynamicControlDto = CurrentControls.FirstOrDefault(Function(x) x.IdControl.Eq(textBoxControlId))
                If existControl IsNot Nothing AndAlso existControl.Value IsNot Nothing Then
                    Dim selectedValue As String = DirectCast(existControl.Value, String)

                    If Not String.IsNullOrEmpty(selectedValue) Then
                        DirectCast(textBoxControl.Controls(0), RadEditor).Content = selectedValue
                    End If
                End If
                CurrentControls.Add(New UDSDynamicControlDto() With {.IdControl = textBoxControlId, .DynamicControlName = "RadEditor"})
            Else
                textBoxControl = New TextStructure().GetRadStructure(textBoxControlId, New KeyValuePair(Of Integer, UnitType)(100, UnitType.Percentage), Nothing,
                                                                     CType(ViewState(textBoxControlId), String), txtMode, String.Empty,
                                                                     ActionType.Eq(ACTION_TYPE_SEARCH) OrElse Not element.ReadOnly,
                                                                     controlIsRequierd, errorMessage, CSS_DISPLAY_BLOCK)
            End If
        End If

        Dim cssRow As String = "Chiaro"
        If element.HiddenField Then
            cssRow = CSS_DISPLAY_NONE
        End If

        If ActionType.Eq(ACTION_TYPE_SEARCH) Then
            SearchableControls.Add(New UDSTableControlModel With {
                .RowCss = cssRow,
                .Controls = {label, textBoxControl},
                .CellsCss = {"col-dsw-2 label", "col-dsw-8"},
                .LayoutPosition = element.Layout
            })
        Else
            table.Rows.AddRaw(cssRow, Nothing, Nothing, {label, textBoxControl}, {"col-dsw-2 label", "col-dsw-8"})
        End If
    End Sub

    ''' <summary>
    ''' Aggiunge un nuovo controllo Numerico
    ''' </summary>
    Public Sub AddNumberControl(table As Table, element As NumberField)
        If ActionType.Eq(ACTION_TYPE_SEARCH) AndAlso Not element.Searchable Then
            Return
        End If

        Dim label As Control = New LabelStructure().GetStructure(String.Format(DYNAMIC_LABEL_NAME_FORMAT, element.ColumnName), GetLabelName(element.Label), String.Empty)
        Dim numberControlId As String = String.Format(DYNAMIC_FIELD_NAME_FORMAT, element.ColumnName).ToLower()

        Dim numberFromControl As Control = Nothing
        Dim labelFromNumberControl As Control = Nothing
        Dim numberToControl As Control = Nothing
        Dim labelToNumberControl As Control = Nothing
        Dim numberControl As Control = Nothing

        Dim fromNumberControlId As String = $"{numberControlId}FromNumber"
        Dim toNumberControlId As String = $"{numberControlId}ToNumber"
        Dim fromNumberLabelControlId As String = $"{numberControlId}From"
        Dim toNumberLabelControlId As String = $"{numberControlId}To"
        Dim fromElementColumnName As String = $"{element.ColumnName}From"
        Dim toElementColumnName As String = $"{element.ColumnName}To"

        Dim minValue As Double = If(element.MinValueSpecified, element.MinValue, Integer.MinValue)
        Dim maxValue As Double = If(element.MaxValueSpecified, element.MaxValue, Integer.MaxValue)

        Dim controlIsRequierd As Boolean = element.Required AndAlso Not IsReadOnly AndAlso Not ActionType.Eq(ACTION_TYPE_VIEW)
        Dim errorMessage As String = $"Campo {element.Label} Obbligatorio"
        Dim controlIsRangeRequired As Boolean = (element.MinValueSpecified OrElse element.MaxValueSpecified) AndAlso Not IsReadOnly AndAlso
            Not ActionType.Eq(ACTION_TYPE_VIEW) AndAlso Not ActionType.Eq(ACTION_TYPE_SEARCH)
        Dim rangeErrorMessage As String = $"Inserire un valore compreso tra {minValue} e {maxValue}"

        If ActionType.Eq(ACTION_TYPE_SEARCH) Then
            If IsReadOnly Then
                numberFromControl = New LabelStructure().GetStructure(fromNumberControlId, CType(ViewState(fromNumberControlId), String), String.Empty)
                numberToControl = New LabelStructure().GetStructure(toNumberControlId, CType(ViewState(toNumberControlId), String), String.Empty)
                labelFromNumberControl = New LabelStructure().GetStructure(fromNumberLabelControlId, CType(ViewState(fromNumberLabelControlId), String), String.Empty)
                labelToNumberControl = New LabelStructure().GetStructure(toNumberLabelControlId, CType(ViewState(toNumberLabelControlId), String), String.Empty)
            Else
                labelFromNumberControl = New LabelStructure().GetStructure(String.Format(DYNAMIC_LABEL_NAME_FORMAT, fromElementColumnName), "Da ", String.Empty)
                numberFromControl = New NumericStructure().GetRadStructure(fromNumberControlId, New KeyValuePair(Of Integer, UnitType)(100, UnitType.Pixel),
                                                                      Nothing, Nothing, String.Empty, 0, ActionType.Eq(ACTION_TYPE_SEARCH) OrElse Not element.ReadOnly,
                                                                      controlIsRequierd, errorMessage, controlIsRangeRequired, rangeErrorMessage, minValue, maxValue, CSS_DISPLAY_INLINE)

                labelToNumberControl = New LabelStructure().GetStructure(String.Format(DYNAMIC_LABEL_NAME_FORMAT, toElementColumnName), "A ", String.Empty)
                numberToControl = New NumericStructure().GetRadStructure(toNumberControlId, New KeyValuePair(Of Integer, UnitType)(100, UnitType.Pixel),
                                                                      Nothing, Nothing, String.Empty, 0, ActionType.Eq(ACTION_TYPE_SEARCH) OrElse Not element.ReadOnly,
                                                                      controlIsRequierd, errorMessage, controlIsRangeRequired, rangeErrorMessage, minValue, maxValue, CSS_DISPLAY_INLINE)
            End If
        Else
            If IsReadOnly Then
                numberControl = New LabelStructure().GetStructure(numberControlId, CType(ViewState(numberControlId), String), String.Empty)
                DirectCast(numberControl, Label).AddAttribute("IsNumeric", True.ToString())
                DirectCast(numberControl, Label).AddAttribute("NumericFormat", element.Format)
            Else
                numberControl = New NumericStructure().GetRadStructure(numberControlId, New KeyValuePair(Of Integer, UnitType)(100, UnitType.Pixel), Nothing,
                                                                       Nothing, String.Empty, 0, ActionType.Eq(ACTION_TYPE_SEARCH) OrElse Not element.ReadOnly,
                                                                       controlIsRequierd, errorMessage, controlIsRangeRequired, rangeErrorMessage, minValue, maxValue, CSS_DISPLAY_BLOCK)
                Dim numericTextBoxControl As RadNumericTextBox = DirectCast(numberControl.Controls(0), RadNumericTextBox)
                numericTextBoxControl.NumberFormat.GroupSeparator = String.Empty

                If element.Format = CURRENCY_FORMAT Then
                    Dim currencySymbol As String = numericTextBoxControl.Culture.NumberFormat.CurrencySymbol
                    label = New LabelStructure().GetStructure(String.Format(DYNAMIC_LABEL_NAME_FORMAT, element.ColumnName), GetLabelName($"{element.Label} ({currencySymbol})"), String.Empty)
                End If

                FormatNumberControl(element, numericTextBoxControl)
            End If
        End If

        Dim lastNumberControlForm As Control = New Control()
        If labelFromNumberControl IsNot Nothing AndAlso numberFromControl IsNot Nothing AndAlso labelToNumberControl IsNot Nothing AndAlso numberToControl IsNot Nothing Then
            lastNumberControlForm.Controls.Add(labelFromNumberControl)
            lastNumberControlForm.Controls.Add(numberFromControl)
            lastNumberControlForm.Controls.Add(labelToNumberControl)
            lastNumberControlForm.Controls.Add(numberToControl)
        End If

        Dim cssRow As String = If(element.HiddenField, CSS_DISPLAY_NONE, "Chiaro dsw-vertical-middle")
        Dim rowNumberControl As Control = If(ActionType.Eq(ACTION_TYPE_SEARCH), lastNumberControlForm, numberControl)
        Dim numberControlCssClass As String = If(ActionType.Eq(ACTION_TYPE_SEARCH), "boldLabel", "col-dsw-8")

        If ActionType.Eq(ACTION_TYPE_SEARCH) Then
            SearchableControls.Add(New UDSTableControlModel With {
                .RowCss = cssRow,
                .Controls = {label, rowNumberControl},
                .CellsCss = {"col-dsw-2 label", numberControlCssClass},
                .LayoutPosition = element.Layout
            })
        Else
            table.Rows.AddRaw(cssRow, Nothing, Nothing, {label, rowNumberControl}, {"col-dsw-2 label", numberControlCssClass})
        End If
    End Sub

    ''' <summary>
    ''' Aggiunge un nuovo controllo DatePicker
    ''' </summary>
    Public Sub AddDateControl(table As Table, element As DateField)
        If ActionType.Eq(ACTION_TYPE_SEARCH) AndAlso Not element.Searchable Then
            Return
        End If

        Dim label As Control = New LabelStructure().GetStructure(String.Format(DYNAMIC_LABEL_NAME_FORMAT, element.ColumnName), GetLabelName(element.Label), String.Empty)
        Dim dateControlId As String = String.Format(DYNAMIC_FIELD_NAME_FORMAT, element.ColumnName).ToLower()

        Dim defaultValue As DateTime? = Nothing
        If Not element.DefaultValue.Equals(DateTime.MinValue) Then
            defaultValue = element.DefaultValue
        End If
        Dim dateFromControl As Control = Nothing
        Dim labelFromDataControl As Control = Nothing
        Dim dateToControl As Control = Nothing
        Dim labelToDataControl As Control = Nothing
        Dim dateControl As Control = Nothing

        Dim controlIsRequierd As Boolean = element.Required AndAlso Not IsReadOnly
        Dim errorMessage As String = $"Campo {element.Label} Obbligatorio"

        If ActionType.Eq(ACTION_TYPE_SEARCH) Then
            If IsReadOnly Then
                dateFromControl = New LabelStructure().GetStructure(String.Concat(dateControlId, "FromDate"), CType(ViewState(String.Concat(dateControlId, "FromDate")), String), String.Empty)
                dateToControl = New LabelStructure().GetStructure(String.Concat(dateControlId, "ToDate"), CType(ViewState(String.Concat(dateControlId, "ToDate")), String), String.Empty)
                labelFromDataControl = New LabelStructure().GetStructure(String.Concat(dateControlId, "From"), CType(ViewState(String.Concat(dateControlId, "From")), String), String.Empty)
                labelToDataControl = New LabelStructure().GetStructure(String.Concat(dateControlId, "To"), CType(ViewState(String.Concat(dateControlId, "To")), String), String.Empty)
            Else
                labelFromDataControl = New LabelStructure().GetStructure(String.Format(DYNAMIC_LABEL_NAME_FORMAT, String.Concat(element.ColumnName, "From")), "Da ", String.Empty)
                dateFromControl = New DateTimeStructure().GetRadStructure(String.Concat(dateControlId, "FromDate"), New KeyValuePair(Of Integer, UnitType)(150, UnitType.Pixel),
                                                                      CType(ViewState(String.Concat(dateControlId, "FromDate")), Date), String.Empty,
                                                                      ActionType.Eq(ACTION_TYPE_SEARCH) OrElse Not element.ReadOnly,
                                                                      controlIsRequierd, errorMessage, CSS_DISPLAY_INLINE)
                labelToDataControl = New LabelStructure().GetStructure(String.Format(DYNAMIC_LABEL_NAME_FORMAT, String.Concat(element.ColumnName, "To")), "A ", String.Empty)
                dateToControl = New DateTimeStructure().GetRadStructure(String.Concat(dateControlId, "ToDate"), New KeyValuePair(Of Integer, UnitType)(150, UnitType.Pixel),
                                                                      CType(ViewState(String.Concat(dateControlId, "ToDate")), Date), String.Empty,
                                                                      ActionType.Eq(ACTION_TYPE_SEARCH) OrElse Not element.ReadOnly,
                                                                      controlIsRequierd, errorMessage, CSS_DISPLAY_INLINE)
            End If
        Else
            If IsReadOnly Then
                dateControl = New LabelStructure().GetStructure(dateControlId, CType(ViewState(dateControlId), String), String.Empty)
            Else
                dateControl = New DateTimeStructure().GetRadStructure(dateControlId, New KeyValuePair(Of Integer, UnitType)(100, UnitType.Pixel),
                                                                      CType(ViewState(dateControlId), Date), String.Empty,
                                                                      ActionType.Eq(ACTION_TYPE_SEARCH) OrElse Not element.ReadOnly,
                                                                      controlIsRequierd, errorMessage, CSS_DISPLAY_BLOCK)
            End If
        End If

        Dim lastDataControlForm As Control = New Control()

        If labelFromDataControl IsNot Nothing AndAlso dateFromControl IsNot Nothing AndAlso labelToDataControl IsNot Nothing AndAlso dateToControl IsNot Nothing Then
            lastDataControlForm.Controls.Add(labelFromDataControl)
            lastDataControlForm.Controls.Add(dateFromControl)
            lastDataControlForm.Controls.Add(labelToDataControl)
            lastDataControlForm.Controls.Add(dateToControl)
        End If

        Dim cssRow As String = "Chiaro dsw-vertical-middle"
        If element.HiddenField Then
            cssRow = CSS_DISPLAY_NONE
        End If


        If ActionType.Eq(ACTION_TYPE_SEARCH) Then
            SearchableControls.Add(New UDSTableControlModel With {
                .RowCss = cssRow,
                .Controls = {label, lastDataControlForm},
                .CellsCss = {"col-dsw-2 label", "boldLabel"},
                .LayoutPosition = element.Layout
            })
        Else
            table.Rows.AddRaw(cssRow, Nothing, Nothing, {label, dateControl}, {"col-dsw-2 label", "col-dsw-8"})
        End If

        If ActionType.Eq(ACTION_TYPE_VIEW) Then
            Return
        End If
    End Sub

    Public Function GetControlsBetween(fieldToGet As String) As Control
        Return dynamicControls.FindControl(WebHelper.SafeControlIdName(fieldToGet).ToLower())
    End Function


    ''' <summary>
    ''' Aggiunge un nuovo controllo CheckBox
    ''' </summary>
    Public Sub AddCheckBoxControl(table As Table, element As BoolField)
        If ActionType.Eq(ACTION_TYPE_SEARCH) AndAlso Not element.Searchable Then
            Return
        End If

        Dim label As Control = New LabelStructure().GetStructure(String.Format(DYNAMIC_LABEL_NAME_FORMAT, element.ColumnName), GetLabelName(element.Label), String.Empty)
        Dim cssRow As String = "Chiaro"
        If element.HiddenField Then
            cssRow = CSS_DISPLAY_NONE
        End If
        If ActionType.Eq(ACTION_TYPE_SEARCH) OrElse ActionType.Eq(ACTION_TYPE_INSERT) OrElse ActionType.Eq(ACTION_TYPE_EDIT) Then
            Dim controlIsRequierd As Boolean = element.Required AndAlso Not IsReadOnly
            Dim errorMessage As String = $"Campo {element.Label} Obbligatorio"

            Dim comboValues As IDictionary(Of String, String) = New Dictionary(Of String, String)
            comboValues.Add("Vero", "True")
            comboValues.Add("Falso", "False")
            Dim DropDownControlId As String = String.Format(DYNAMIC_FIELD_NAME_FORMAT, element.ColumnName).ToLower()
            Dim DropDownControl As Control = New ComboStructure().GetRadStructure(DropDownControlId, New KeyValuePair(Of Integer, UnitType)(200, UnitType.Pixel), comboValues,
                                                                                  String.Empty, True, controlIsRequierd, errorMessage, CSS_DISPLAY_BLOCK)

            If ActionType.Eq(ACTION_TYPE_SEARCH) Then
                SearchableControls.Add(New UDSTableControlModel With {
                    .RowCss = cssRow,
                    .Controls = {label, DropDownControl},
                    .CellsCss = {"col-dsw-2 label", "col-dsw-8"},
                    .LayoutPosition = element.Layout
                })
            Else
                table.Rows.AddRaw(cssRow, Nothing, Nothing, {label, DropDownControl}, {"col-dsw-2 label", "col-dsw-8"})
            End If
        Else
            Dim checkBoxControlId As String = String.Format(DYNAMIC_FIELD_NAME_FORMAT, element.ColumnName).ToLower()
            Dim checkBoxControl As Control = New CheckBoxStructure().GetStructure(checkBoxControlId, Nothing, CType(ViewState(checkBoxControlId), Boolean), String.Empty, (Not element.ReadOnly AndAlso Not IsReadOnly))
            table.Rows.AddRaw(cssRow, Nothing, Nothing, {label, checkBoxControl}, {"col-dsw-2 label", "col-dsw-8"})
            'If element.Required AndAlso Not IsReadOnly Then
            'todo: non funziona la validazione dei checkbox, verificare come implementarla
            'AddCustomValidatorControl(table, checkBoxControlId, CUSTOM_VALIDATION_FUNCTION, element.Label)
            'End If
        End If

    End Sub

    ''' <summary>
    ''' Aggiunge un nuovo controllo di tipo uscUDSLookup
    ''' </summary>
    Public Sub AddLookupControl(table As Table, element As LookupField)
        If ActionType.Eq(ACTION_TYPE_SEARCH) AndAlso (Not element.Searchable OrElse element.MultipleValues) Then
            Return
        End If

        Dim label As Control = New LabelStructure().GetStructure(String.Format(DYNAMIC_LABEL_NAME_FORMAT, element.ColumnName), GetLabelName(element.Label), String.Empty)
        Dim uscUDSLookupId As String = String.Format(DYNAMIC_FIELD_NAME_FORMAT, element.ColumnName).ToLower()
        Dim cssRow As String = "Chiaro"

        If IsReadOnly Then
            Dim uscLookupControl As Control = New LabelStructure().GetStructure(uscUDSLookupId, CType(ViewState(uscUDSLookupId), String), String.Empty)
            Dim lookupLabel As Label = DirectCast(uscLookupControl, Label)
            lookupLabel.AddAttribute("IsJSONValue", "True")

            If ActionType.Eq(ACTION_TYPE_SEARCH) Then
                SearchableControls.Add(New UDSTableControlModel With {
                    .RowCss = cssRow,
                    .Controls = {label, lookupLabel},
                    .CellsCss = {"col-dsw-2 label", "col-dsw-8"},
                    .LayoutPosition = element.Layout
                })
            Else
                table.Rows.AddRaw(cssRow, Nothing, Nothing, {label, lookupLabel}, {"col-dsw-2 label", "col-dsw-8"})
            End If
        Else
            Dim uscUDSLookupControl As uscUDSLookup = DirectCast(LoadControl("~/UDS/UserControl/uscUDSLookup.ascx"), uscUDSLookup)
            'Carico i valori del precedente postback
            Dim existControl As UDSDynamicControlDto = CurrentControls.FirstOrDefault(Function(x) x.IdControl.Eq(uscUDSLookupId))
            If existControl IsNot Nothing AndAlso existControl.Value IsNot Nothing Then
                Dim selectedValue As String = DirectCast(existControl.Value, String)
                If Not String.IsNullOrEmpty(selectedValue) Then
                    uscUDSLookupControl.LookupValue = selectedValue
                End If
            End If
            uscUDSLookupControl.ID = uscUDSLookupId
            uscUDSLookupControl.UDSName = element.LookupArchiveName
            uscUDSLookupControl.PropertyName = element.LookupArchiveColumnName
            uscUDSLookupControl.IsRequired = element.Required AndAlso Not IsReadOnly AndAlso Not ActionType.Eq(ACTION_TYPE_SEARCH)
            uscUDSLookupControl.CheckBoxesEnabled = Not IsReadOnly AndAlso element.MultipleValues
            If uscUDSLookupControl.CheckBoxesEnabled Then
                uscUDSLookupControl.HiddenFieldId = hiddenLookup.ClientID
                uscUDSLookupControl.LookupLabel = element.Label
            End If
            uscUDSLookupControl.ErrorMessage = String.Concat("Campo ", element.Label, " obbligatorio")

            If ActionType.Eq(ACTION_TYPE_SEARCH) Then
                SearchableControls.Add(New UDSTableControlModel With {
                    .RowCss = cssRow,
                    .Controls = {label, uscUDSLookupControl},
                    .CellsCss = {"col-dsw-2 label", "col-dsw-8"},
                    .LayoutPosition = element.Layout
                })
            Else
                table.Rows.AddRaw(cssRow, Nothing, Nothing, {label, uscUDSLookupControl}, {"col-dsw-2 label", "col-dsw-8"})
            End If

            CurrentControls.Add(New UDSDynamicControlDto() With {.IdControl = uscUDSLookupId, .DynamicControlName = "uscUDSLookup"})

        End If
    End Sub

    Public Sub AddTreeListControl(table As Table, element As TreeListField)
        If ActionType.Eq(ACTION_TYPE_SEARCH) AndAlso Not element.Searchable Then
            Return
        End If

        Dim label As Control = New LabelStructure().GetStructure(String.Format(DYNAMIC_LABEL_NAME_FORMAT, element.ColumnName), GetLabelName(element.Label), String.Empty)
        Dim uscUDSFieldListTreeId As String = String.Format(DYNAMIC_FIELD_NAME_FORMAT, element.ColumnName).ToLower()
        Dim cssRow As String = "Chiaro"

        Dim uscUDSFieldListTreeControl As uscUDSFieldListTree = DirectCast(LoadControl("~/UDS/UserControl/uscUDSFieldListTree.ascx"), uscUDSFieldListTree)
        uscUDSFieldListTreeControl.ID = uscUDSFieldListTreeId
        uscUDSFieldListTreeControl.IdUDSRepository = IdUDSRepository
        uscUDSFieldListTreeControl.IsReadOnly = ActionType.Eq(ACTION_TYPE_VIEW)
        uscUDSFieldListTreeControl.IsRequired = element.Required AndAlso Not ActionType.Eq(ACTION_TYPE_VIEW) AndAlso Not ActionType.Eq(ACTION_TYPE_SEARCH)
        uscUDSFieldListTreeControl.ErrorMessage = $"Campo {element.Label} obbligatorio"
        uscUDSFieldListTreeControl.SetFieldNameAttribute(element.ColumnName)
        uscUDSFieldListTreeControl.HiddenFieldId = hiddenFieldList.ClientID

        If (ActionType.Eq(ACTION_TYPE_VIEW) OrElse ActionType.Eq(ACTION_TYPE_EDIT)) AndAlso UDSFieldListChildren IsNot Nothing AndAlso String.IsNullOrEmpty(hiddenFieldList.Value) Then
            uscUDSFieldListTreeControl.LoadUDSFielsListParents(UDSFieldListChildren)
        End If

        If (ActionType.Eq(ACTION_TYPE_INSERT) OrElse ActionType.Eq(ACTION_TYPE_EDIT)) AndAlso Not String.IsNullOrEmpty(hiddenFieldList.Value) Then
            Dim udsFieldListChildren As List(Of KeyValuePair(Of String, Guid)) = GetUDSFieldListChildren(element.ColumnName, $"{Me.ClientID}_{uscUDSFieldListTreeId}")
            If udsFieldListChildren IsNot Nothing Then
                uscUDSFieldListTreeControl.LoadUDSFielsListParents(udsFieldListChildren)
            End If
        End If

        table.Rows.AddRaw(cssRow, Nothing, Nothing, {label, uscUDSFieldListTreeControl}, {"col-dsw-2 label", "col-dsw-8"})
        CurrentControls.Add(New UDSDynamicControlDto() With {.IdControl = uscUDSFieldListTreeId, .DynamicControlName = "uscUDSFieldListTree"})
    End Sub

    ''' <summary>
    ''' Aggiunge un nuovo controllo di tipo uscContattiSel
    ''' </summary>
    Public Sub AddContactControl(table As Table, element As Contacts)
        If ActionType.Eq(ACTION_TYPE_SEARCH) AndAlso Not element.Searchable Then
            Return
        End If

        Dim label As Control = New LabelStructure().GetStructure(String.Format(DYNAMIC_LABEL_NAME_FORMAT, element.Label), GetLabelName(element.Label), String.Empty)
        Dim uscContactId As String = WebHelper.SafeControlIdName(String.Format(DYNAMIC_FIELD_NAME_FORMAT, element.Label))

        Dim uscContact As uscContattiSel = DirectCast(LoadControl("~/UserControl/uscContattiSel.ascx"), uscContattiSel)
        uscContact.ID = uscContactId
        uscContact.ProtType = True
        uscContact.MultiSelect = element.AllowMultiContact AndAlso Not ActionType.Eq(ACTION_TYPE_SEARCH)
        uscContact.Multiple = element.AllowMultiContact AndAlso Not ActionType.Eq(ACTION_TYPE_SEARCH)
        uscContact.IsRequired = element.Required AndAlso Not IsReadOnly AndAlso ActionType IsNot Nothing
        uscContact.Initialize()

        'Carico i valori del precedente postback
        Dim existControl As UDSDynamicControlDto = Me.CurrentControls.FirstOrDefault(Function(x) x.IdControl.Eq(uscContactId))
        If existControl IsNot Nothing AndAlso existControl.Value IsNot Nothing Then
            Dim contacts As IList(Of Data.ContactDTO) = DirectCast(existControl.Value, IList(Of Data.ContactDTO))
            If contacts IsNot Nothing Then
                uscContact.DataSource = contacts
                uscContact.DataBind()
            End If
        End If

        uscContact.HeaderVisible = False
        uscContact.ButtonDeleteVisible = Not IsReadOnly
        uscContact.ButtonManualMultiVisible = Not IsReadOnly AndAlso element.AllowMultiContact AndAlso Not ActionType.Eq(ACTION_TYPE_SEARCH)
        uscContact.ButtonManualVisible = Not IsReadOnly AndAlso element.ManualEnabled AndAlso Not ActionType.Eq(ACTION_TYPE_SEARCH)
        uscContact.ButtonImportManualVisible = Not IsReadOnly AndAlso element.ExcelImportEnabled AndAlso Not ActionType.Eq(ACTION_TYPE_SEARCH)
        uscContact.ButtonSelectVisible = (Not IsReadOnly AndAlso element.AddressBookEnabled) OrElse ActionType.Eq(ACTION_TYPE_SEARCH)
        uscContact.ButtonSelectDomainVisible = ((Not IsReadOnly AndAlso element.ADEnabled) OrElse ActionType.Eq(ACTION_TYPE_SEARCH)) AndAlso ProtocolEnv.AbilitazioneRubricaDomain
        uscContact.ButtonImportVisible = Not IsReadOnly AndAlso Not ActionType.Eq(ACTION_TYPE_SEARCH)
        uscContact.ReadOnlyProperties = IsReadOnly

        If ActionType.Eq(ACTION_TYPE_SEARCH) Then
            Dim contactTable As Table = New Table() With {
                .ID = String.Format(DYNAMIC_FIELD_NAME_FORMAT, Guid.NewGuid()),
                .CssClass = "datatable",
                .EnableViewState = False
            }

            Dim tmpPanel As Panel = New Panel()
            Dim manualSearchContactId As String = String.Format(DYNAMIC_MANUAL_SEARCH_CONTACT_NAME_FORMAT, element.Label).ToLower()
            Dim manualSearchContact As Control = New TextStructure().GetRadStructure(manualSearchContactId, New KeyValuePair(Of Integer, UnitType)(100, UnitType.Percentage),
                                                                                        Nothing, String.Empty, InputMode.SingleLine, String.Empty)

            Dim manualSearchContactlabel As Control = New LabelStructure().GetStructure(String.Format(DYNAMIC_LABEL_NAME_FORMAT, Guid.NewGuid()), "Descrizione", "label")
            DirectCast(manualSearchContactlabel, Label).Style.Add(HtmlTextWriterStyle.MarginRight, "10px")
            tmpPanel.Controls.Add(manualSearchContactlabel)
            tmpPanel.Controls.Add(manualSearchContact)

            contactTable.Rows.AddRaw(String.Empty, Nothing, Nothing, {uscContact}, {"col-dsw-10"})
            contactTable.Rows.AddRaw(String.Empty, Nothing, Nothing, {tmpPanel}, {"col-dsw-8"})

            SearchableControls.Add(New UDSTableControlModel With {
                .RowCss = "Chiaro",
                .Controls = {label, contactTable},
                .CellsCss = {"col-dsw-2 label", "col-dsw-8"},
                .LayoutPosition = element.Layout
            })

        Else

            If Not ActionType.Eq(ACTION_TYPE_VIEW) Then
                table.Rows.AddRaw("Chiaro", Nothing, Nothing, {label, uscContact}, {"col-dsw-2 label", "col-dsw-8"})
                If element.Layout IsNot Nothing Then
                    Try
                        table.Rows.AddRaw(CType((element.Layout.RowNumber - 1), Integer?), "Chiaro", Nothing, Nothing, {label, uscContact}, {"col-dsw-2 label", "col-dsw-8"})
                    Catch ex As Exception
                        table.Rows.AddRaw("Chiaro", Nothing, Nothing, {label, uscContact}, {"col-dsw-2 label", "col-dsw-8"})
                    End Try
                End If
            Else
                table.Rows.AddRaw("Chiaro", Nothing, Nothing, {uscContact}, {"col-dsw-8"})
                If element.Layout IsNot Nothing Then
                    Try
                        table.Rows.AddRaw(CType((element.Layout.RowNumber - 1), Integer?), "Chiaro", Nothing, Nothing, {uscContact}, {"col-dsw-8"})
                    Catch ex As Exception
                        table.Rows.AddRaw("Chiaro", Nothing, Nothing, {uscContact}, {"col-dsw-8"})
                    End Try
                End If
            End If

        End If

        CurrentControls.Add(New UDSDynamicControlDto() With {.IdControl = uscContactId, .DynamicControlName = "uscContattiSel"})
    End Sub

    Private Function IsSignRequired(controlName As String) As Boolean
        If Me.CurrentModelControls.Documents Is Nothing Then
            Return False
        End If


        If Me.WorkflowSignedDocRequired.Any(Function(x) WebHelper.SafeControlIdName(String.Format(DYNAMIC_FIELD_NAME_FORMAT, x.Key)).Eq(controlName)) Then
            Return Me.WorkflowSignedDocRequired.First(Function(x) WebHelper.SafeControlIdName(String.Format(DYNAMIC_FIELD_NAME_FORMAT, x.Key)).Eq(controlName)).Value
        End If

        Dim docControlId As String = WebHelper.SafeControlIdName(String.Format(DYNAMIC_FIELD_NAME_FORMAT, Me.CurrentModelControls.Documents.Document?.Label))
        Dim attControlId As String = WebHelper.SafeControlIdName(String.Format(DYNAMIC_FIELD_NAME_FORMAT, Me.CurrentModelControls.Documents.DocumentAttachment?.Label))
        Dim annControlId As String = WebHelper.SafeControlIdName(String.Format(DYNAMIC_FIELD_NAME_FORMAT, Me.CurrentModelControls.Documents.DocumentAnnexed?.Label))

        Select Case controlName
            Case docControlId
                Return Me.CurrentModelControls.Documents.Document.SignRequired
                Exit Select
            Case attControlId
                Return Me.CurrentModelControls.Documents.DocumentAttachment.SignRequired
                Exit Select
            Case annControlId
                Return Me.CurrentModelControls.Documents.DocumentAnnexed.SignRequired
                Exit Select
        End Select
        Return False
    End Function

    Private Sub DocumentUploadedCallback(sender As Object, doc As DocumentEventArgs)
        Dim uscDoc As uscDocumentUpload = DirectCast(sender, uscDocumentUpload)
        If Not doc.Document.IsSigned AndAlso IsSignRequired(uscDoc.ID) Then
            uscDoc.SetDocumentInfoName(doc.Document, DOCUMENT_TO_SIGN_REQUIRED)
        End If

    End Sub

    Private Sub DocumentSignedCallback(sender As Object, doc As DocumentSignedEventArgs)
        Dim uscDoc As uscDocumentUpload = DirectCast(sender, uscDocumentUpload)
        If Not doc.DestinationDocument.IsSigned AndAlso IsSignRequired(uscDoc.ID) Then
            uscDoc.SetDocumentInfoName(doc.DestinationDocument, DOCUMENT_TO_SIGN_REQUIRED)
        End If
    End Sub

    ''' <summary>
    ''' Aggiunge un nuovo controllo di tipo uscDocumentUpload
    ''' </summary>
    Public Sub AddDocumentControl(table As Table, element As Document, allowUnlimitFileSize As Boolean)
        If IsReadOnly Then
            Exit Sub
        End If

        Dim label As Control = New LabelStructure().GetStructure(String.Format(DYNAMIC_LABEL_NAME_FORMAT, element.Label), GetLabelName(element.Label), String.Empty)
        Dim documentId As String = WebHelper.SafeControlIdName(String.Format(DYNAMIC_FIELD_NAME_FORMAT, element.Label))

        Dim uscDocument As uscDocumentUpload = DirectCast(LoadControl("~/UserControl/uscDocumentUpload.ascx"), uscDocumentUpload)
        uscDocument.ID = documentId
        uscDocument.ReadOnly = element.ReadOnly OrElse DocumentsReadonly
        uscDocument.ButtonPreviewEnabled = Not element.ReadOnly AndAlso Not DocumentsReadonly
        uscDocument.IsDocumentRequired = element.Required
        uscDocument.DocumentDeletable = element.Deletable AndAlso Not DocumentsReadonly
        uscDocument.ButtonRemoveEnabled = uscDocument.DocumentDeletable
        uscDocument.AllowUnlimitFileSize = allowUnlimitFileSize
        uscDocument.Initialize()

        'Carico i valori del precedente postback
        Dim existControl As UDSDynamicControlDto = Me.CurrentControls.FirstOrDefault(Function(x) x.IdControl.Eq(documentId))
        If existControl IsNot Nothing AndAlso existControl.Value IsNot Nothing Then
            Dim xml As String = existControl.Value.ToString()
            If Not String.IsNullOrEmpty(xml) Then
                uscDocument.TreeViewControl.LoadXml(xml)
                uscDocument.TreeViewControl.ExpandAllNodes()
                If uscDocument.TreeViewControl.Nodes.Count > 0 Then
                    uscDocument.TreeViewControl.Nodes(0).Font.Bold = True
                End If

                If existControl.CustomProperties.ContainsKey(NameOf(uscDocument.DocumentsToDelete)) _
                    AndAlso existControl.CustomProperties(NameOf(uscDocument.DocumentsToDelete)) IsNot Nothing _
                    AndAlso TypeOf existControl.CustomProperties(NameOf(uscDocument.DocumentsToDelete)) Is IList(Of Guid) Then

                    uscDocument.DocumentsToDelete = DirectCast(existControl.CustomProperties(NameOf(uscDocument.DocumentsToDelete)), IList(Of Guid))
                End If
                uscDocument.TreeViewControl.DataBind()
            End If
        End If

        uscDocument.HeaderVisible = False
        uscDocument.MultipleDocuments = element.AllowMultiFile
        uscDocument.HideScannerMultipleDocumentButton = element.AllowMultiFile
        uscDocument.ButtonFileEnabled = element.UploadEnabled AndAlso Not uscDocument.ReadOnly
        uscDocument.ButtonScannerEnabled = element.ScannerEnabled AndAlso Not uscDocument.ReadOnly
        uscDocument.SignButtonEnabled = element.SignEnabled AndAlso Not uscDocument.ReadOnly
        uscDocument.ButtonCopyProtocol.Visible = element.CopyProtocol AndAlso Not uscDocument.ReadOnly
        uscDocument.ButtonCopyResl.Visible = element.CopyResolution AndAlso Not uscDocument.ReadOnly
        uscDocument.ButtonCopySeries.Visible = element.CopySeries AndAlso Not uscDocument.ReadOnly
        uscDocument.ButtonCopyUDS.Visible = element.CopyUDS AndAlso Not uscDocument.ReadOnly

        AddHandler uscDocument.DocumentUploaded, AddressOf DocumentUploadedCallback
        AddHandler uscDocument.DocumentSigned, AddressOf DocumentSignedCallback

        table.Rows.AddRaw("Chiaro", Nothing, Nothing, {label, uscDocument}, {"col-dsw-2 label", "col-dsw-8"})
        If element.Layout IsNot Nothing Then
            table.Rows.AddRaw(element.Layout.RowNumber - 1, "Chiaro", Nothing, Nothing, {label, uscDocument}, {"col-dsw-2 label", "col-dsw-8"})
        End If
        Me.CurrentControls.Add(New UDSDynamicControlDto() With {.IdControl = documentId, .DynamicControlName = "uscDocumentUpload"})
    End Sub

    ''' <summary>
    ''' Aggiunge un nuovo controllo di tipo uscSettori
    ''' </summary>
    Public Sub AddRoleControl(table As Table, element As Authorizations)
        If ActionType.Eq(ACTION_TYPE_SEARCH) AndAlso Not element.Searchable Then
            Return
        End If

        Dim label As Control = New LabelStructure().GetStructure(String.Format(DYNAMIC_LABEL_NAME_FORMAT, element.Label), GetLabelName(element.Label), String.Empty)
        Dim authId As String = WebHelper.SafeControlIdName(String.Format(DYNAMIC_FIELD_NAME_FORMAT, element.Label))

        Dim uscAuth As uscSettori = DirectCast(Page.LoadControl("~/UserControl/uscSettori.ascx"), uscSettori)
        uscAuth.ID = authId
        uscAuth.ReadOnly = IsReadOnly
        uscAuth.Required = element.Required AndAlso Not IsReadOnly
        uscAuth.Initialize()

        'Carico i valori del precedente postback
        Dim rolesControl As UDSDynamicControlDto = Me.CurrentControls.FirstOrDefault(Function(x) x.IdControl.Eq(authId))
        If rolesControl IsNot Nothing AndAlso rolesControl.Value IsNot Nothing Then
            Dim xml As String = rolesControl.Value.ToString()
            If Not String.IsNullOrEmpty(xml) Then
                Dim roles As IList(Of Data.Role) = DirectCast(rolesControl.Value, IList(Of Data.Role))
                If roles IsNot Nothing Then
                    uscAuth.SourceRoles = roles.ToList()
                End If
            End If
        End If
        Dim usersControl As UDSDynamicControlDto = Me.CurrentControls.LastOrDefault(Function(x) x.IdControl = authId)
        If usersControl IsNot Nothing AndAlso usersControl.Value IsNot Nothing Then
            Dim xml As String = usersControl.Value.ToString()
            If Not String.IsNullOrEmpty(xml) Then
                Dim users As Dictionary(Of String, String) = DirectCast(usersControl.Value, Dictionary(Of String, String))
                If users IsNot Nothing Then
                    uscAuth.SourceUsers = users
                End If
            End If
        End If

        uscAuth.DataBind()
        uscAuth.HeaderVisible = False
        uscAuth.MultipleRoles = element.AllowMultiAuthorization
        uscAuth.MultiSelect = element.AllowMultiAuthorization
        uscAuth.MyAuthorizedRolesEnabled = element.MyAuthorizedRolesEnabled AndAlso ActionType.Eq(ACTION_TYPE_INSERT)
        uscAuth.RoleEnvironment = Data.DSWEnvironment.DocumentSeries
        uscAuth.CurrentRoleUserViewMode = Nothing
        uscAuth.UserMultiSelectionEnabled = element.AllowMultiUserAuthorization

        If ActionType IsNot Nothing AndAlso (ActionType.Eq(ACTION_TYPE_INSERT) OrElse ActionType.Equals(ACTION_TYPE_EDIT)) Then
            uscAuth.UserAuthorizationEnabled = element.UserAuthorizationEnabled
            uscAuth.InitializeUserAuthorization()
        End If
        If ActionType.Eq(ACTION_TYPE_INSERT) Then
            AddHandler uscAuth.RoleUserAdded, AddressOf OnRoleUserAdded
            If MyAuthorizedRolesEnabled AndAlso uscAuth.MyAuthorizedRolesEnabled Then
                uscAuth.AddMyAuthorizedRoles(Data.DSWEnvironment.DocumentSeries)
                MyAuthorizedRolesEnabled = False
            End If
        End If

        If ActionType.Eq(ACTION_TYPE_VIEW) Then
            table.Rows.AddRaw("Chiaro", Nothing, Nothing, {uscAuth}, {"col-dsw-10"})
            If element.Layout IsNot Nothing Then
                Try
                    table.Rows.AddRaw(CType((element.Layout.RowNumber - 1), Integer?), "Chiaro", Nothing, Nothing, {uscAuth}, {"col-dsw-10"})
                Catch ex As Exception
                    table.Rows.AddRaw("Chiaro", Nothing, Nothing, {uscAuth}, {"col-dsw-10"})
                End Try

            End If
        Else
            If ActionType.Eq(ACTION_TYPE_SEARCH) Then
                SearchableControls.Add(New UDSTableControlModel With {
                    .RowCss = "Chiaro",
                    .Controls = {label, uscAuth},
                    .CellsCss = {"col-dsw-2 label", "col-dsw-8"},
                    .LayoutPosition = element.Layout
                })
            Else
                table.Rows.AddRaw("Chiaro", Nothing, Nothing, {label, uscAuth}, {"col-dsw-2 label", "col-dsw-8"})
                If element.Layout IsNot Nothing Then
                    table.Rows.AddRaw(CType((element.Layout.RowNumber - 1), Integer?), "Chiaro", Nothing, Nothing, {label, uscAuth}, {"col-dsw-2 label", "col-dsw-8"})
                End If
            End If
        End If
        AjaxManager.AjaxSettings.AddAjaxSetting(uscAuth, uscAuth)
        Me.CurrentControls.Add(New UDSDynamicControlDto() With {.IdControl = authId, .DynamicControlName = "uscSettori"})
    End Sub

    Protected Sub OnRoleUserAdded(ByVal sender As Object, ByVal e As RoleUserEventArgs)
        If Not ActionType.Eq(ACTION_TYPE_INSERT) Then
            Exit Sub
        End If

        Dim userProperties As ResultPropertyCollection = CommonAD.GetUserADValueByKey(e.RoleUser.Key)
        If userProperties IsNot Nothing Then
            For Each element As Section In CurrentModelControls.Metadata
                If element.Items IsNot Nothing Then
                    For Each item As FieldBaseType In element.Items.Where(Function(x) TypeOf x Is TextField)
                        Dim textItem As TextField = DirectCast(item, TextField)
                        If textItem.CustomAction = CustomActionEnum.LeggivaloredachiavedellutentecorrentedaActiveDirectory AndAlso Not CurrentModelControls.Authorizations.AllowMultiUserAuthorization Then
                            Dim realValue As String = String.Empty
                            If userProperties.Contains(textItem.CustomActionKey) Then
                                realValue = userProperties(textItem.CustomActionKey)(0).ToString()
                            End If
                            Dim modelField As UDSModelField = New UDSModelField(item)
                            Dim ctrl As Control = dynamicControls.FindControl(WebHelper.SafeControlIdName(String.Format(DYNAMIC_FIELD_NAME_FORMAT, modelField.ColumnName)))
                            Dim controlName As String = ctrl.GetType().Name
                            Dim enableControl As Boolean = (Not IsReadOnly) AndAlso (Not item.HiddenField)
                            Dim realValueIsNull As Boolean = realValue Is Nothing OrElse String.IsNullOrEmpty(realValue)
                            If ActionType.Eq(ACTION_TYPE_EDIT) AndAlso (Not item.ModifyEnabled) AndAlso Not realValueIsNull Then
                                enableControl = False
                            End If
                            SetControlValue(ctrl, controlName, realValue, modifiable:=enableControl)
                        End If
                    Next
                End If
            Next
        End If
    End Sub
    ''' <summary>
    ''' Imposto i valori per i field dinamici
    ''' </summary>
    Private Sub SetControlValue(ctrl As Control, controlName As String, value As Object, Optional modifiable As Boolean = False, Optional allowFormatValue As Boolean = False)
        If value Is Nothing Then
            Exit Sub
        End If

        Select Case controlName

            Case GetType(Label).Name
                SetLabelField(DirectCast(ctrl, Label), value.ToString(), enabled:=modifiable, allowFormatValue:=allowFormatValue)

            Case GetType(CheckBox).Name
                SetBooleanField(DirectCast(ctrl, CheckBox), Boolean.Parse(value.ToString()), enabled:=modifiable)

            Case GetType(RadDropDownList).Name
                SetEnumField(DirectCast(ctrl, RadDropDownList), value.ToString(), enabled:=modifiable)

            Case GetType(RadComboBox).Name
                SetEnumField(DirectCast(ctrl, RadComboBox), value.ToString(), enabled:=modifiable)

            Case GetType(Image).Name
                SetStatusField(DirectCast(ctrl, Image), CType(value, StatusType), enabled:=modifiable)

            Case GetType(RadTextBox).Name
                SetTextField(DirectCast(ctrl, RadTextBox), value.ToString(), enabled:=modifiable)

            Case GetType(RadEditor).Name
                SetTextField(DirectCast(ctrl, RadEditor), value.ToString(), enabled:=modifiable)

            Case GetType(RadDatePicker).Name
                SetDateField(DirectCast(ctrl, RadDatePicker), DateTime.Parse(value.ToString()), enabled:=modifiable)

            Case GetType(RadNumericTextBox).Name
                SetNumericField(DirectCast(ctrl, RadNumericTextBox), Double.Parse(value.ToString()), enabled:=modifiable)

            Case GetType(uscUDSLookup).Name
                SetLookupField(DirectCast(ctrl, uscUDSLookup), value.ToString(), enabled:=modifiable)

            Case GetType(uscUDSFieldListTree).Name
                SetTreeListField(DirectCast(ctrl, uscUDSFieldListTree))

            Case GetType(uscContattiSel).Name
                If value.GetType() = GetType(Contacts) Then
                    Dim contacts As Object() = Enumerable.Empty(Of Object).ToArray()
                    Dim elem As Contacts = DirectCast(value, Contacts)
                    If elem.ContactInstances IsNot Nothing Then
                        contacts = contacts.Concat(elem.ContactInstances).ToArray()
                    End If

                    If elem.ContactManualInstances IsNot Nothing Then
                        contacts = contacts.Concat(elem.ContactManualInstances).ToArray()
                    End If
                    SetContactsControl(DirectCast(ctrl, uscContattiSel), contacts, enabled:=modifiable)
                End If

            Case GetType(uscSettori).Name
                Dim authInstances As AuthorizationInstance() = DirectCast(value, AuthorizationInstance())

                Dim roleIds As ICollection(Of Integer) = authInstances.Select(Function(s) s.IdAuthorization).ToList()
                Dim roles As ICollection(Of Data.Role) = Facade.RoleFacade.GetByIds(roleIds)

                Dim users As IDictionary(Of String, String) = New Dictionary(Of String, String)()
                For Each userInstance As AuthorizationInstance In authInstances.Where(Function(x) x.AuthorizationInstanceType = AuthorizationInstanceType.User)
                    Dim user As AccountModel = CommonAD.GetAccount(userInstance.Username.Split("\"c)(1))
                    users.Add(user.GetFullUserName(), $"{user.DisplayName} ({user.GetFullUserName()})")
                Next

                SetAuthorizationControl(DirectCast(ctrl, uscSettori), roles, users, enabled:=modifiable)

            Case GetType(uscDocumentUpload).Name
                Dim doc As Document = DirectCast(value, Document)
                SetDocumentControl(DirectCast(ctrl, uscDocumentUpload), doc, enabled:=modifiable)

        End Select
    End Sub

    ''' <summary>
    ''' Imposto i valori per i field dinamici
    ''' </summary>
    Private Sub FillPageControl(ctrl As Control, controlName As String, ByRef element As Object)
        Select Case controlName

            Case "checkTodrop"
                Dim field As BoolField = DirectCast(element, BoolField)
                Dim selValue As String = DirectCast(ctrl, RadDropDownList).SelectedValue
                If Not String.IsNullOrEmpty(selValue) Then
                    field.ValueSpecified = True
                    field.Value = CBool(DirectCast(ctrl, RadDropDownList).SelectedValue)
                End If

            Case GetType(CheckBox).Name
                Dim field As BoolField = DirectCast(element, BoolField)
                field.ValueSpecified = True
                field.Value = DirectCast(ctrl, CheckBox).Checked

            Case GetType(RadDropDownList).Name

                Dim selValue As String = DirectCast(ctrl, RadDropDownList).SelectedText
                If TypeOf element Is EnumField Then
                    Dim field As EnumField = DirectCast(element, EnumField)
                    If Not String.IsNullOrEmpty(selValue) Then
                        Dim values As ICollection(Of String) = New List(Of String)()
                        values.Add(selValue)
                        field.Value = JsonConvert.SerializeObject(values)
                    End If
                End If


                If TypeOf element Is StatusField Then
                    Dim field As StatusField = DirectCast(element, StatusField)
                    If Not String.IsNullOrEmpty(selValue) Then
                        field.Value = DirectCast(ctrl, RadDropDownList).SelectedText
                    End If
                End If

            Case GetType(RadComboBox).Name
                Dim selValue As String = String.Empty
                If TypeOf element Is EnumField Then
                    Dim field As EnumField = DirectCast(element, EnumField)
                    selValue = DirectCast(ctrl, RadComboBox).SelectedValue
                    If field.MultipleValues Then
                        Dim checkedItems As ICollection(Of RadComboBoxItem) = DirectCast(ctrl, RadComboBox).CheckedItems
                        If checkedItems IsNot Nothing AndAlso checkedItems.Count > 0 Then
                            selValue = JsonConvert.SerializeObject(checkedItems.Select(Function(x) x.Value))
                        End If
                    Else
                        Dim values As ICollection(Of String) = New List(Of String)()
                        values.Add(selValue)
                        selValue = JsonConvert.SerializeObject(values)
                    End If

                    If Not String.IsNullOrEmpty(selValue) Then
                        field.Value = selValue
                    End If

                End If
                If TypeOf element Is StatusField Then
                    selValue = DirectCast(ctrl, RadComboBox).SelectedValue
                    Dim field As StatusField = DirectCast(element, StatusField)
                    If Not String.IsNullOrEmpty(selValue) Then
                        field.Value = DirectCast(ctrl, RadComboBox).SelectedValue
                    End If
                End If

            Case GetType(RadTextBox).Name
                Dim field As TextField = DirectCast(element, TextField)
                field.Value = DirectCast(ctrl, RadTextBox).Text

            Case GetType(RadEditor).Name
                Dim field As TextField = DirectCast(element, TextField)
                field.Value = DirectCast(ctrl, RadEditor).Content

            Case GetType(RadDatePicker).Name
                Dim field As DateField = DirectCast(element, DateField)
                If DirectCast(ctrl, RadDatePicker).SelectedDate.HasValue AndAlso DirectCast(ctrl, RadDatePicker).SelectedDate.Value > DateTime.MinValue Then
                    field.ValueSpecified = True
                    field.Value = DirectCast(ctrl, RadDatePicker).SelectedDate.Value
                End If

            Case GetType(RadNumericTextBox).Name
                Dim field As NumberField = DirectCast(element, NumberField)
                If DirectCast(ctrl, RadNumericTextBox).Value.HasValue Then
                    field.ValueSpecified = True
                    field.Value = DirectCast(ctrl, RadNumericTextBox).Value.Value
                End If

            Case GetType(uscUDSLookup).Name
                Dim field As LookupField = DirectCast(element, LookupField)
                Dim uscLookup As uscUDSLookup = DirectCast(ctrl, uscUDSLookup)

                Dim lookupValue As String = uscLookup.SelectedValue
                If String.IsNullOrEmpty(lookupValue) AndAlso Not String.IsNullOrEmpty(hiddenLookup.Value) Then
                    Dim hiddenFieldValues As IDictionary(Of String, String()) = JsonConvert.DeserializeObject(Of IDictionary(Of String, String()))(hiddenLookup.Value)
                    If hiddenFieldValues.Keys.Contains(field.Label) Then
                        lookupValue = JsonConvert.SerializeObject(hiddenFieldValues.Item(field.Label))
                    End If
                ElseIf Not String.IsNullOrEmpty(lookupValue) Then
                    Dim lookupValues As ICollection(Of String) = New List(Of String)()
                    lookupValues.Add(lookupValue)
                    lookupValue = JsonConvert.SerializeObject(lookupValues)
                End If

                field.Value = lookupValue

            Case GetType(uscUDSFieldListTree).Name
                Dim field As TreeListField = DirectCast(element, TreeListField)
                Dim uscFieldListTree As uscUDSFieldListTree = DirectCast(ctrl, uscUDSFieldListTree)
                If Not String.IsNullOrEmpty(uscFieldListTree.SelectedNodeValue) Then
                    field.Value = uscFieldListTree.SelectedNodeValue
                ElseIf Not String.IsNullOrEmpty(hiddenFieldList.Value) Then
                    Dim hiddenFieldValue As String = hiddenFieldList.Value
                    If String.IsNullOrEmpty(hiddenFieldValue) Then
                        field.Value = hiddenFieldValue
                    End If
                End If

            Case GetType(uscContattiSel).Name
                Dim field As Contacts = DirectCast(element, Contacts)
                Dim uscContact As uscContattiSel = DirectCast(ctrl, uscContattiSel)
                field.ContactInstances = GetContactField(uscContact)
                field.ContactManualInstances = GetManualContactField(uscContact)

            Case GetType(uscSettori).Name
                Dim auth As Authorizations = DirectCast(element, Authorizations)
                Dim uscSettori As uscSettori = DirectCast(ctrl, uscSettori)
                auth.Instances = GetAuthorizationField(uscSettori)

            Case GetType(uscDocumentUpload).Name
                Dim document As Document = DirectCast(element, Document)
                Dim uscDocumentUpload As uscDocumentUpload = DirectCast(ctrl, uscDocumentUpload)
                document.Instances = GetDocumentField(uscDocumentUpload)

        End Select
    End Sub

    Private Function GetContactField(control As uscContattiSel) As ContactInstance()
        Dim contacts As IList(Of ContactInstance) = New List(Of ContactInstance)
        Dim addressContacts As IList(Of Data.ContactDTO) = control.GetAddressContacts(False)
        For Each addressContact As Data.ContactDTO In addressContacts
            Dim instance As ContactInstance = New ContactInstance()
            instance.IdContact = addressContact.Contact.Id
            contacts.Add(instance)
        Next

        Return contacts.ToArray()
    End Function

    Private Function GetManualContactField(control As uscContattiSel) As ContactManualInstance()
        Dim contacts As IList(Of ContactManualInstance) = New List(Of ContactManualInstance)

        Dim manualContacts As IList(Of Data.ContactDTO) = control.GetManualContacts()
        For Each manualContact As Data.ContactDTO In manualContacts
            Dim instance As ContactManualInstance = New ContactManualInstance()
            manualContact.Contact.UniqueId = Guid.Empty
            instance.ContactDescription = JsonConvert.SerializeObject(manualContact, New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Serialize})
            contacts.Add(instance)
        Next

        Dim manualSearchCtrl As Control = dynamicControls.FindControl(WebHelper.SafeControlIdName(String.Format(DYNAMIC_MANUAL_SEARCH_CONTACT_NAME_FORMAT, control.ID.Replace("field_", String.Empty))))
        If ActionType.Eq(ACTION_TYPE_SEARCH) AndAlso manualSearchCtrl IsNot Nothing Then
            Dim searchDescription As String = DirectCast(manualSearchCtrl, RadTextBox).Text
            If Not String.IsNullOrEmpty(searchDescription) Then
                Dim dto As Data.ContactDTO = New Data.ContactDTO(New Data.Contact() With {.Description = searchDescription, .ContactType = New Data.ContactType(Data.ContactType.Mistery)}, Data.ContactDTO.ContactType.Manual)
                Dim searchInstance As ContactManualInstance = New ContactManualInstance()
                searchInstance.ContactDescription = JsonConvert.SerializeObject(dto, New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Serialize})
                contacts.Add(searchInstance)
            End If
        End If

        Return contacts.ToArray()
    End Function

    Private Function GetAuthorizationField(control As uscSettori) As AuthorizationInstance()
        Dim authorizations As IList(Of AuthorizationInstance) = New List(Of AuthorizationInstance)
        Dim selectedRoles As IList(Of Data.Role) = control.GetRoles()
        If selectedRoles.Any() Then
            authorizations = selectedRoles.Select(Function(s) New AuthorizationInstance() With {.IdAuthorization = s.Id, .UniqueId = s.UniqueId.ToString()}).ToList()
        End If
        Dim selectedUsers As IDictionary(Of String, String) = control.GetUsers()
        If selectedUsers.Any() Then
            For Each selectedUser As KeyValuePair(Of String, String) In selectedUsers
                authorizations.Add(New AuthorizationInstance With
                {
                    .AuthorizationInstanceType = AuthorizationInstanceType.User,
                    .Username = selectedUser.Key
                })
            Next
        End If
        Return authorizations.ToArray()
    End Function

    Private Function GetDocumentField(control As uscDocumentUpload) As DocumentInstance()
        Dim documents As IList(Of DocumentInstance) = New List(Of DocumentInstance)
        Dim selectedDocuments As IList(Of DocumentInfo) = control.DocumentInfos
        Dim documentStored As BiblosDocumentInfo = Nothing
        Dim documentInstance As DocumentInstance = Nothing

        If selectedDocuments.Any() Then
            For Each document As DocumentInfo In selectedDocuments
                documentInstance = New DocumentInstance() With {.DocumentName = document.Name}
                If TypeOf document Is BiblosDocumentInfo Then
                    If Not ActionType.Eq(ACTION_TYPE_EDIT) Then
                        documentStored = document.ArchiveInBiblos(CommonShared.CurrentWorkflowLocation.ProtBiblosDSDB, Guid.Empty)
                        documentInstance.IdDocumentToStore = documentStored.DocumentId.ToString()
                    End If
                    documentInstance.StoredChainId = DirectCast(document, BiblosDocumentInfo).ChainId.ToString()
                Else
                    documentStored = document.ArchiveInBiblos(CommonShared.CurrentWorkflowLocation.ProtBiblosDSDB, Guid.Empty)
                    documentInstance.IdDocumentToStore = documentStored.DocumentId.ToString()
                End If
                documents.Add(documentInstance)
            Next
        End If
        Return documents.ToArray()
    End Function

    Private Sub SetBooleanField(control As CheckBox, value As Boolean, Optional enabled As Boolean = False)
        control.Checked = value
        control.Enabled = enabled
        ViewState(control.ID) = value
    End Sub

    Private Sub SetEnumField(control As RadComboBox, value As String, Optional enabled As Boolean = False)
        If Not String.IsNullOrEmpty(value) Then
            Dim selectedItems As ICollection(Of String) = New List(Of String)()
            Try
                selectedItems = JsonConvert.DeserializeObject(Of ICollection(Of String))(value)
            Catch
                selectedItems = New List(Of String) From {value}
            End Try

            If control.CheckBoxes Then
                Dim toCheckItems As ICollection(Of RadComboBoxItem) = control.Items.Where(Function(f) selectedItems.Any(Function(x) x = f.Text)).ToList()
                If toCheckItems.Count > 0 Then
                    For Each toCheckItem As RadComboBoxItem In toCheckItems
                        toCheckItem.Checked = True
                    Next
                End If
            Else
                If selectedItems.Count = 1 Then
                    Dim selectectItem As RadComboBoxItem = control.Items.SingleOrDefault(Function(f) f.Text.Eq(selectedItems.First()))
                    If selectectItem IsNot Nothing Then
                        selectectItem.Selected = True
                        control.Text = selectectItem.Text
                        control.SelectedValue = selectectItem.Value
                    End If
                End If
            End If
            control.Enabled = enabled
            control.DataBind()
        End If
    End Sub

    Private Sub SetEnumField(control As RadDropDownList, value As String, Optional enabled As Boolean = False)
        If Not String.IsNullOrEmpty(value) Then
            Dim selectedValue As ICollection(Of String) = New List(Of String)()
            Try
                selectedValue = JsonConvert.DeserializeObject(Of ICollection(Of String))(value)
            Catch
                selectedValue = New List(Of String) From {value}
            End Try

            If selectedValue IsNot Nothing AndAlso selectedValue.Count = 1 Then
                Dim selectedItem As DropDownListItem = control.Items.SingleOrDefault(Function(f) f.Value.Eq(selectedValue.First))
                If selectedItem IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(selectedValue.First) Then
                    selectedItem.Selected = True
                    control.SelectedText = selectedItem.Text
                    control.SelectedValue = selectedItem.Value
                End If
            End If
            control.Enabled = enabled
            control.DataBind()
        End If
    End Sub

    Private Sub SetStatusField(control As Image, statusType As StatusType, Optional enabled As Boolean = False)
        If statusType IsNot Nothing Then
            control.ImageUrl = statusType.IconPath
            control.AlternateText = statusType.Value
            control.ToolTip = statusType.Value
            control.Enabled = enabled
            control.DataBind()
        End If
    End Sub
    Private Sub SetLabelField(control As Label, value As String, Optional enabled As Boolean = False, Optional allowFormatValue As Boolean = False)
        'Verifico se il valore è una data
        Dim valueDate As DateTime
        If allowFormatValue AndAlso Date.TryParse(value, valueDate) Then
            value = String.Empty
            If Not valueDate.Year.Equals(DateTime.MinValue.Year) Then
                value = valueDate.ToString("dd/MM/yyyy")
            End If
        End If

        If Not String.IsNullOrEmpty(value) AndAlso control.Attributes("IsJSONValue") IsNot Nothing AndAlso control.Attributes("IsJSONValue") = "True" Then
            Dim enumValues As String() = JsonConvert.DeserializeObject(Of String())(value)
            value = String.Join(", ", enumValues)
        End If

        If Not String.IsNullOrEmpty(value) AndAlso control.Attributes("IsNumeric") IsNot Nothing AndAlso control.Attributes("IsNumeric") = True.ToString() Then
            Dim format As String = String.Empty
            If control.Attributes("NumericFormat") IsNot Nothing Then
                format = control.Attributes("NumericFormat")
            End If
            value = Double.Parse(value).ToString(format)
        End If

        control.Text = value
        control.Enabled = enabled
        ViewState(control.ID) = value
    End Sub

    Private Sub SetTextField(control As RadTextBox, value As String, Optional enabled As Boolean = False)
        control.Text = value
        control.Enabled = enabled
        ViewState(control.ID) = value
    End Sub
    Private Sub SetTextField(control As RadEditor, value As String, Optional enabled As Boolean = False)
        control.Content = value
        control.Enabled = enabled
        ViewState(control.ID) = value
    End Sub

    Private Sub SetDateField(control As RadDatePicker, value As DateTime, Optional enabled As Boolean = False)
        If Not value.Year.Equals(DateTime.MinValue.Year) Then
            control.SelectedDate = value
            ViewState(control.ID) = value
        End If
        control.Enabled = enabled
    End Sub

    Private Sub SetNumericField(control As RadNumericTextBox, value As Double, Optional enabled As Boolean = False)
        control.Value = value
        control.Enabled = enabled
        ViewState(control.ID) = value
    End Sub

    Private Sub SetTreeListField(control As uscUDSFieldListTree)
        control.LoadUDSFieldListTree(IdUDSRepository)
        If (ActionType.Eq(ACTION_TYPE_EDIT)) AndAlso UDSFieldListChildren IsNot Nothing Then
            control.LoadUDSFielsListParents(UDSFieldListChildren)
        End If
    End Sub

    Private Sub SetLookupField(control As uscUDSLookup, value As String, Optional enabled As Boolean = False)
        If Not String.IsNullOrEmpty(value) Then
            '***non fare la deserializzazione lo fa lato client (this._udsService.getLookupValues)
            control.LookupValue = value
            control.Enabled = enabled
            ViewState(control.ID) = value
        End If
    End Sub

    Private Sub SetDocumentControl(control As uscDocumentUpload, document As Document, Optional enabled As Boolean = False)
        If document Is Nothing OrElse document.Instances Is Nothing Then
            Exit Sub
        End If

        Dim bibDocInfos As IList(Of BiblosDocumentInfo) = New List(Of BiblosDocumentInfo)
        Dim docInfos As IList(Of DocumentInfo) = New List(Of DocumentInfo)
        For Each instance As DocumentInstance In document.Instances
            If String.IsNullOrEmpty(instance.StoredChainId) Then
                docInfos.Add(New BiblosDocumentInfo(Guid.Parse(instance.IdDocumentToStore)))
            Else
                Dim bibDocs As IList(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocumentsLatestVersion(Guid.Parse(instance.StoredChainId))
                'Verifico se è stato passato l'ID document invece dell'ID chain
                If bibDocs.Count = 0 Then
                    bibDocs = BiblosDocumentInfo.GetDocumentInfo(Guid.Parse(instance.StoredChainId), Nothing, True)
                End If
                For Each doc As BiblosDocumentInfo In bibDocs
                    Try
                        Dim chainId As Guid = doc.ChainId
                        docInfos.Add(doc)
                    Catch ex As Exception
                    End Try
                Next
            End If
        Next
        control.ReadOnly = Not enabled
        control.LoadDocumentInfo(bibDocInfos.Cast(Of DocumentInfo).ToList())
        control.LoadDocumentInfo(docInfos)
        If IsSignRequired(control.ID) Then
            Dim alert As Boolean = False
            For Each docChoosen As DocumentInfo In docInfos
                If Not docChoosen.IsSigned Then
                    alert = True
                End If
                If alert Then
                    control.SetDocumentInfoName(docChoosen, DOCUMENT_TO_SIGN_REQUIRED)
                End If
            Next
        End If
    End Sub

    Private Sub SetContactsControl(control As uscContattiSel, values As Object(), Optional enabled As Boolean = False)
        If values Is Nothing Then
            Exit Sub
        End If

        Dim dtos As IList(Of Data.ContactDTO) = New List(Of Data.ContactDTO)()
        For Each instance As Object In values
            Dim dto As Data.ContactDTO = New Data.ContactDTO()
            If instance.[GetType]() = GetType(ContactInstance) Then
                dto.Id = DirectCast(instance, ContactInstance).IdContact
                dto.Contact = Facade.ContactFacade.GetById(dto.Id)
                dto.Type = Data.ContactDTO.ContactType.Address
            Else
                Dim manualInstance As ContactManualInstance = DirectCast(instance, ContactManualInstance)
                dto = JsonConvert.DeserializeObject(Of Data.ContactDTO)(manualInstance.ContactDescription)
            End If
            dtos.Add(dto)
        Next

        control.ReadOnly = Not enabled

        control.DataSource.Clear()
        control.DataSource = dtos
        control.DataBind()
    End Sub

    Private Sub SetAuthorizationControl(control As uscSettori, roles As ICollection(Of Data.Role), users As IDictionary(Of String, String), Optional enabled As Boolean = False)
        If roles Is Nothing Then
            Exit Sub
        End If
        control.ReadOnly = Not enabled
        control.SourceRoles.Clear()
        control.SourceRoles.AddRange(roles)
        control.SourceUsers.Clear()
        control.SourceUsers = users
        control.DataBind()
    End Sub

    Public Function GetDeletedDocuments(document As Document) As IList(Of Guid)
        If Not document.Deletable Then
            Return New List(Of Guid)
        End If

        Dim ctrl As Control = dynamicControls.FindControl(WebHelper.SafeControlIdName(String.Format(DYNAMIC_FIELD_NAME_FORMAT, document.Label)))
        Return DirectCast(ctrl, uscDocumentUpload).DocumentsToDelete
    End Function

    Public Sub FormatNumberControl(element As NumberField, numberControl As RadNumericTextBox)
        Select Case element.Format
            Case INTEGER_FORMAT
                numberControl.NumberFormat.DecimalDigits = 0
            Case FOURDIGITS_DECIMAL_FORMAT
                numberControl.NumberFormat.DecimalDigits = 4
            Case CURRENCY_FORMAT
                numberControl.NumberFormat.DecimalDigits = 2
                numberControl.Type = NumericType.Number
            Case Else
                numberControl.NumberFormat.DecimalDigits = 2
        End Select
    End Sub

    Public Sub SaveDynamicFiltersToSession()

        For Each element As Section In CurrentModelControls.Metadata
            If element.Items IsNot Nothing Then
                For Each item As FieldBaseType In element.Items
                    Dim modelField As UDSModelField = New UDSModelField(item)
                    Dim ctrl As Control = dynamicControls.FindControl(WebHelper.SafeControlIdName(String.Format(DYNAMIC_FIELD_NAME_FORMAT, modelField.ColumnName)))
                    If ctrl IsNot Nothing Then
                        Dim controlName As String = ctrl.GetType().Name
                        If GetType(RadDropDownList).Name = controlName Then
                            Dim ddlCtrl As RadDropDownList = DirectCast(ctrl, RadDropDownList)
                            modelField.Value = GetModelFieldValue(item, ddlCtrl.SelectedValue)
                        ElseIf GetType(RadComboBox).Name = controlName Then
                            Dim ddlCtrl As RadComboBox = DirectCast(ctrl, RadComboBox)
                            modelField.Value = GetModelFieldValue(item, ddlCtrl.SelectedValue)
                        End If
                    End If
                Next
            End If
        Next

        Session("TempUDSRepositoryDynamicSearchFilters") = CurrentModelControls
    End Sub

    Private Function GetModelFieldValue(item As FieldBaseType, ddlSelectedValue As String) As Object
        If item.GetType().Name = "BoolField" Then
            If Not String.IsNullOrEmpty(ddlSelectedValue) Then
                Return Convert.ToBoolean(ddlSelectedValue)
            Else
                Return Nothing
            End If
        ElseIf (item.GetType().Name = "StatusField") Then
            Return ddlSelectedValue
        Else
            Return $"[""{ddlSelectedValue}""]"
        End If
    End Function

    Public Sub InitializeDynamicFilters()
        SessionIsEmpty = True
        If Session("TempUDSRepositoryDynamicSearchFilters") IsNot Nothing Then
            SessionIsEmpty = False
            CurrentModelControls = CType(Session("TempUDSRepositoryDynamicSearchFilters"), UnitaDocumentariaSpecifica)
            Session("TempUDSRepositoryDynamicSearchFilters") = Nothing

            Dim controlList As List(Of Object) = New List(Of Object)()
            Dim modelField As UDSModelField
            Dim ctrl As Control
            For Each element As Section In Me.CurrentModelControls.Metadata
                If element.Items IsNot Nothing Then
                    For Each item As FieldBaseType In element.Items
                        modelField = New UDSModelField(item)
                        ctrl = dynamicControls.FindControl(WebHelper.SafeControlIdName(String.Format(DYNAMIC_FIELD_NAME_FORMAT, modelField.ColumnName)))
                        If ctrl IsNot Nothing Then
                            controlList.Add(New With {.ControlName = ctrl.GetType().Name, .ClientID = ctrl.ClientID})
                        End If
                    Next
                End If
            Next
            AjaxManager.ResponseScripts.Add(String.Format("loadControlsValues({0},{1});", JsonConvert.SerializeObject(CurrentModelControls.Metadata), JsonConvert.SerializeObject(controlList)))
        End If
    End Sub

    Private Sub RenderUDSSearchControls(table1 As Table, table2 As Table, table3 As Table)
        Dim columnGroups As IEnumerable(Of IGrouping(Of UInteger, UDSTableControlModel)) = SearchableControls.GroupBy(Function(x) x.LayoutPosition.ColNumber)
        For Each columnGroup As IGrouping(Of UInteger, UDSTableControlModel) In columnGroups
            Dim columnControls As List(Of UDSTableControlModel) = columnGroup.OrderBy(Function(x) x.LayoutPosition.RowNumber).ToList()
            For Each columnControl As UDSTableControlModel In columnControls
                Dim correctTable As Table = If(columnControl.LayoutPosition.ColNumber = 0, table1, If(columnControl.LayoutPosition.ColNumber = 1, table2, table3))
                correctTable.Rows.AddRaw(columnControl.RowCss, columnControl.Colspans, columnControl.Widths, columnControl.Controls, columnControl.CellsCss)
            Next
        Next
    End Sub

    Private Function GetUDSFieldListChildren(columnName As String, uscUDSFieldListClientId As String) As List(Of KeyValuePair(Of String, Guid))
        Dim hiddenFieldListValue As Dictionary(Of String, Guid) = JsonConvert.DeserializeObject(Of Dictionary(Of String, Guid))(hiddenFieldList.Value)
        Dim kvpSelectedNode As KeyValuePair(Of String, Guid) = hiddenFieldListValue.FirstOrDefault(Function(x) x.Key = uscUDSFieldListClientId)
        If kvpSelectedNode.Key IsNot Nothing Then
            Dim selectedUDSFieldListId As Guid = kvpSelectedNode.Value
            Return New List(Of KeyValuePair(Of String, Guid)) From {New KeyValuePair(Of String, Guid)(columnName, selectedUDSFieldListId)}
        End If
        Return Nothing
    End Function
#End Region
End Class