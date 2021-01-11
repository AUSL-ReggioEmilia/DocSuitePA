Imports System.Collections.Generic
Imports System.Xml
Imports System.Text
Imports System.IO
Imports System.Linq
Imports System.Drawing
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports Newtonsoft.Json
Imports VecompSoftware.Services.Logging
Imports System.Text.RegularExpressions
Imports System.Web
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.DocSuiteWeb.DTO.Commons

Partial Public Class uscContattiSel
    Inherits DocSuite2008BaseControl
    Implements IBindingUserControl(Of ContactDTO)

    ''' <summary> Segnala l'aggiunta di nuovi Contatti da rubrica </summary>
    Public Event ItemsAdded(ByVal sender As Object, ByVal e As ContactsEventArgs)

    Public Event OChartItemContactsAdded(sender As Object, e As OChartItemContactsEventArgs)

    Public Event ShowContactList(ByVal sender As Object, ByVal e As EventArgs)
    Public Event ContactAdded(ByVal sender As Object, ByVal e As EventArgs)
    Public Event ContactRemoved(ByVal sender As Object, ByVal e As EventArgs)
    Public Event ManualContactAdded(ByVal sender As Object, ByVal e As EventArgs)
    Public Event ImportExcelContactAdded(ByVal sender As Object, ByVal e As EventArgs)
    Public Event RoleUserContactAdded(ByVal sender As Object, ByVal e As EventArgs)
    Public Event IPAContactAdded(ByVal sender As Object, ByVal e As EventArgs)
    Private ReadOnly _mailDescritionExp As Regex = New Regex("[,.]")

#Region " Fields "

    Public Const ManualContactAttribute As String = "ManualContact"
    Public Const FromTemplateAttribute As String = "FromTemplate"
    Private Const CONTACT_ADDRESSBOOK As String = "ADDRESSBOOK"
    Private Const CONTACT_MANUAL As String = "MANUAL"
    Private Const CONTACT_IPA As String = "IPA"
    Private Const CONTACT_ROLEUSER As String = "ROLEUSER"
    Private Const ROOT_DESC As String = "Root"

    Private Const DefaultOpenWindowScript As String = "return {0}_OpenWindowOLD('{1}', '{2}', {0}{3});"
    Private Const OpenWindowSmartScript As String = "return {0}_OpenWindowSmart('{1}', {0}{2});"
    Private Const FullSizeOpenWindowScript As String = "return {0}_OpenWindowOLDFullScreen('{1}', '{2}', {0}{3});"

    Private _action As String = String.Empty
    Private _multiple As Boolean
    Private _multiSelect As Boolean
    Private _singleCheck As Boolean
    Private _protType As Boolean
    Private _contactList As IList(Of ContactDTO)
    Private _enableCompression As Boolean
    Private _lastRemovedNode As RadTreeNode
    Private _forceAddressBook As Boolean
    Private _currentProtocol As Protocol
    Private _fiscalCodeRequired As Boolean = False
    Private _templateModels As List(Of TemplateAuthorizationModel)
#End Region

#Region " Properties "

    Public Property AVCPBusinessContactEnabled As Boolean = False

    Public Property RestrictionZoneId As String
        Get
            Return CType(ViewState("RestrictionZoneId"), String)
        End Get
        Set(value As String)
            ViewState("RestrictionZoneId") = value
        End Set
    End Property

    Public Property SimpleMode As Boolean
        Get
            If ViewState("SimpleMode") Is Nothing Then
                ViewState("SimpleMode") = False
            End If
            Return CType(ViewState("SimpleMode"), Boolean)
        End Get
        Set(value As Boolean)
            ViewState("SimpleMode") = DocSuiteContext.Current.ProtocolEnv.PECSimpleMode AndAlso value
        End Set
    End Property


    Public ReadOnly Property TreeViewContact() As RadTreeView
        Get
            Return RadTreeContact
        End Get
    End Property


    ''' <summary> Forza lo usercontrol ad utilizzare la rubrica anche se attivo ADAM o AD. </summary>
    Public Property ForceAddressBook() As Boolean
        Get
            Return _forceAddressBook
        End Get
        Set(ByVal value As Boolean)
            _forceAddressBook = value
        End Set
    End Property

    Public Property Action() As String
        Get
            Return _action
        End Get
        Set(ByVal value As String)
            _action = value
        End Set
    End Property

    Public ReadOnly Property SessionMultipleContacts As Boolean
        Get
            If Not Session.Item("MultipleContacts") Is Nothing Then
                Return CBool(Session.Item("MultipleContacts"))
            Else
                Return False
            End If
        End Get
    End Property

    Public Property Multiple() As Boolean
        Get
            If (SessionMultipleContacts) Then
                Return SessionMultipleContacts
            End If
            Return _multiple
        End Get
        Set(ByVal value As Boolean)
            _multiple = value
        End Set
    End Property

    Public Property MultiSelect() As Boolean
        Get
            Return _multiSelect
        End Get
        Set(ByVal value As Boolean)
            _multiSelect = value
        End Set
    End Property

    Public Property SingleCheck() As Boolean
        Get
            Return _singleCheck
        End Get
        Set(ByVal value As Boolean)
            _singleCheck = value
        End Set
    End Property

    Public Property IsRequired() As Boolean
        Get
            Return TreeValidator.Enabled
        End Get
        Set(ByVal value As Boolean)
            TreeValidator.Enabled = value
        End Set
    End Property

    Public Property RequiredErrorMessage() As String
        Get
            Return TreeValidator.ErrorMessage
        End Get
        Set(ByVal value As String)
            TreeValidator.ErrorMessage = value
        End Set
    End Property

    Public Property [ReadOnly]() As Boolean
        Get
            Return (Not panelButtons.Visible)
        End Get
        Set(ByVal value As Boolean)
            panelButtons.Visible = (Not value)
            If (value = True) Then
                TreeValidator.Enabled = False
                lblCount.Visible = False
                chkCopia.Visible = False
            End If
        End Set
    End Property

    Public Property ProtType() As Boolean
        Get
            Return _protType
        End Get
        Set(ByVal value As Boolean)
            _protType = value
        End Set
    End Property

    Public Property Caption() As String
        Get
            Return lblCaption.Text
        End Get
        Set(ByVal value As String)
            lblCaption.Text = value
        End Set
    End Property

    ''' <summary> ? </summary>
    ''' <remarks>Abilita OnlyManualDetail sui contatti manuali</remarks>
    Public Property ReadOnlyProperties() As Boolean
        Get
            If ViewState("_readOnlyProperties") Is Nothing Then
                Return False
            Else
                Return ViewState("_readOnlyProperties")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("_readOnlyProperties") = value
        End Set
    End Property

    ''' <summary> Contatto radice da mostrare nella ricerca </summary>
    Public Property ContactRoot() As Integer?
        Get
            Return CType(ViewState("_contactRoot"), Integer?)
        End Get
        Set(ByVal value As Integer?)
            ViewState("_contactRoot") = value
        End Set
    End Property

    ''' <summary> Contatti da escludere nella ricerca </summary>
    Public Property ExcludeContacts() As List(Of Integer)
        Get
            Dim exclContact As List(Of Integer) = CType(ViewState("ExcludeContact"), List(Of Integer))
            If exclContact.IsNullOrEmpty() Then
                ViewState("ExcludeContact") = New List(Of Integer)
            End If
            Return CType(ViewState("ExcludeContact"), List(Of Integer))
        End Get
        Set(ByVal value As List(Of Integer))
            If value IsNot Nothing AndAlso value.Count() > 0 Then
                ViewState("ExcludeContact") = value
            Else
                ViewState("ExcludeContact") = New List(Of Integer)
            End If
        End Set
    End Property
    ' <summary> Nodi di settore da escludere nella ricerca </summary>
    Public Property ExcludeRoleRoot() As Boolean?
        Get
            Return CType(ViewState("ExcludeRoleRoot"), Boolean?)
        End Get
        Set(ByVal value As Boolean?)
            ViewState("ExcludeRoleRoot") = value
        End Set
    End Property

    Public Property IsFiscalCodeRequired() As Boolean
        Get
            If ViewState("FCRequired") IsNot Nothing Then
                _fiscalCodeRequired = ViewState("FCRequired")
            End If
            Return _fiscalCodeRequired
        End Get
        Set(ByVal value As Boolean)
            ViewState("FCRequired") = value
            _fiscalCodeRequired = value
        End Set
    End Property

    Public Property UseAD() As Boolean
        Get
            If ViewState("_useAD") Is Nothing Then
                Return False
            Else
                Return ViewState("_useAD")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("_useAD") = value
        End Set
    End Property

    Private ReadOnly Property EnableCount As Boolean
        Get
            Return pnlIntestazione.Visible AndAlso ProtocolEnv.ShowContactCount
        End Get
    End Property

    Public ReadOnly Property Count() As Integer
        Get
            Return GetContacts(ProtocolEnv.ContactCountGetAllSelected).Count
        End Get
    End Property

    ''' <summary> Abilita la compressione dei nodi contatti </summary>
    ''' <value> True: visualizza un nodo che identifica il numero di contatti presenti, False: visualizza tutti i contatti </value>
    ''' <remarks> La compressione viene eseguita solo se si supera il numero di contatti impostato su ContactMaxItems </remarks>
    Public Property EnableCompression() As Boolean
        Get
            Return _enableCompression
        End Get
        Set(ByVal value As Boolean)
            _enableCompression = value
        End Set
    End Property

    Public Property CaptionCC() As String
        Get
            Return chkCopia.Text
        End Get
        Set(ByVal value As String)
            chkCopia.Text = value
        End Set
    End Property

    Public Property EnableCC() As Boolean
        Get
            Return chkCopia.Visible
        End Get
        Set(ByVal value As Boolean)
            chkCopia.Visible = value
        End Set
    End Property

    Public ReadOnly Property CCChecked() As Boolean
        Get
            If chkCopia.Visible = False Then
                Return False
            Else
                Return chkCopia.Checked
            End If
        End Get
    End Property

    ''' <summary> Imposta l'etichetta del nodo root della treeview del controllo. </summary>
    Public Property TreeViewCaption() As String
        Get
            Return RadTreeContact.Nodes(0).Text
        End Get
        Set(ByVal value As String)
            RadTreeContact.Nodes(0).Text = value
        End Set
    End Property

    ''' <summary> Restituisce il controllo TreeView utilizzato per visualizzare i contatti. </summary>
    Public ReadOnly Property TreeViewControl() As RadTreeView
        Get
            Return RadTreeContact
        End Get
    End Property

    Public Property EnableCheck() As Boolean
        Get
            Dim checkable As Boolean? = CType(ViewState("EnableCheck"), Boolean?)
            If checkable.HasValue Then Return checkable.Value
            Return False
        End Get
        Set(ByVal value As Boolean)
            TreeViewControl.CheckBoxes = value
            ViewState("EnableCheck") = value
        End Set
    End Property

    Public ReadOnly Property LastRemovedNode() As RadTreeNode
        Get
            Return _lastRemovedNode
        End Get
    End Property

    Public Property JsonContactAdded() As String
        Get
            Return CType(ViewState("_jsonContact"), String)
        End Get
        Set(ByVal value As String)
            ViewState("_jsonContact") = value
        End Set
    End Property

    Public Property DataSource() As IList(Of ContactDTO) Implements IBindingUserControl(Of ContactDTO).DataSource
        Get
            If _contactList Is Nothing Then
                _contactList = New List(Of ContactDTO)
            End If
            Return _contactList
        End Get
        Set(ByVal value As IList(Of ContactDTO))
            _contactList = value
        End Set
    End Property

    Public Property CurrentProtocol() As Protocol
        Get
            Return _currentProtocol
        End Get
        Set(ByVal value As Protocol)
            _currentProtocol = value
        End Set
    End Property

    Public Property HeaderVisible() As Boolean
        Get
            Return tblHeader.Visible
        End Get
        Set(ByVal value As Boolean)
            tblHeader.Visible = value
        End Set
    End Property

    ''' <summary> Restituisce il controllo che identifica l'Header del controllo. </summary>
    Public ReadOnly Property Header() As Panel
        Get
            Return pnlIntestazione
        End Get
    End Property

    Public ReadOnly Property InnerPanelButtons As Panel
        Get
            Return panelButtons
        End Get
    End Property

    Public ReadOnly Property InnerPanelOnlyContact As Panel
        Get
            Return panelOnlyContact
        End Get
    End Property

    ''' <summary> Visualizza/Nasconde il pulsante di selezione contatto da rubrica. </summary>
    Public Property ButtonSelectVisible() As Boolean
        Get
            Return btnSelContact.Visible
        End Get
        Set(ByVal value As Boolean)
            btnSelContact.Visible = value
        End Set
    End Property
    ''' <summary>
    ''' Visualizza/nasconde il pulsante di selezione da AD (controllare il parametro AbilitazioneRubricaDomain)
    ''' </summary>
    Public Property ButtonSelectDomainVisible As Boolean
        Get
            If ViewState("_buttonSelectDomainVisible") Is Nothing Then
                ViewState("_buttonSelectDomainVisible") = ProtocolEnv.AbilitazioneRubricaDomain
            End If
            Return DirectCast(ViewState("_buttonSelectDomainVisible"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("_buttonSelectDomainVisible") = value
        End Set
    End Property

    Public Property ButtonSelectOChartVisible As Boolean
        Get
            Return ButtonSelContactOChart.Visible
        End Get
        Set(value As Boolean)
            ButtonSelContactOChart.Visible = value AndAlso DocSuiteContext.Current.ProtocolEnv.AbilitazioneRubricaOChart
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde il pulsante di eliminazione contatto. </summary>
    Public Property ButtonDeleteVisible() As Boolean
        Get
            Return btnDelContact.Visible
        End Get
        Set(ByVal value As Boolean)
            btnDelContact.Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde il pulsante di selezione contatto manuale. </summary>
    Public Property ButtonManualVisible() As Boolean
        Get
            Return btnAddManual.Visible
        End Get
        Set(ByVal value As Boolean)
            btnAddManual.Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde il pulsante di selezione contatto manuale. </summary>
    Public Property ButtonAddMyselfVisible() As Boolean
        Get
            Return btnAddMyself.Visible
        End Get
        Set(ByVal value As Boolean)
            btnAddMyself.Visible = value
        End Set
    End Property

    Public Property ButtonManualMultiVisible() As Boolean
        Get
            Return btnAddManualMulti.Visible
        End Get
        Set(ByVal value As Boolean)
            If value And Not MultiSelect Then
                Throw New DocSuiteException("Non è possibile abilitare l'inserimento multiplo se non è attivo il MultiSelect")
            End If
            btnAddManualMulti.Visible = value
        End Set
    End Property

    ''' <summary> Visibilità pulsante dettagli contatto </summary>
    Public Property ButtonPropertiesVisible() As Boolean
        Get
            Return cmdDetails.Visible
        End Get
        Set(ByVal value As Boolean)
            cmdDetails.Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde il pulsante di importazione contatti da Xml. </summary>
    Public Property ButtonImportVisible() As Boolean
        Get
            Return btnImportContact.Visible
        End Get
        Set(ByVal value As Boolean)
            btnImportContact.Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde il pulsante di importazione contatti da Excel. </summary>
    Public Property ButtonImportManualVisible() As Boolean
        Get
            Return btnImportContactManual.Visible
        End Get
        Set(ByVal value As Boolean)
            Dim bSet As Boolean = value And ProtocolEnv.IsImportContactManualEnabled
            btnImportContactManual.Visible = bSet
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde il pulsante di selezione contatto IPA o RubricaAUS. </summary>
    Public Property ButtonIPAVisible() As Boolean
        Get
            Return btnIPAContact.Visible
        End Get
        Set(ByVal value As Boolean)
            btnIPAContact.Visible = value
        End Set
    End Property

    ''' <summary> Visualizza/Nasconde il pulsante di selezione contatto Role. </summary>
    Public Property ButtonRoleVisible() As Boolean
        Get
            Return btnRoleUser.Visible
        End Get
        Set(ByVal value As Boolean)
            btnRoleUser.Visible = value
        End Set
    End Property

    Public Property ButtonContactSmartVisible() As Boolean
        Get
            Return btnContactSmart.Visible
        End Get
        Set(ByVal value As Boolean)
            btnContactSmart.Visible = value
        End Set
    End Property

    Public ReadOnly Property AddRoleButton() As ImageButton
        Get
            Return btnRoleUser
        End Get
    End Property

    Public Property ValidationGroup As String
        Get
            Return TreeValidator.ValidationGroup
        End Get
        Set(value As String)
            TreeValidator.ValidationGroup = value
        End Set
    End Property

    ''' <summary>
    ''' Visualizza/Nasconde il pulsante di selezione contatto del sistema di interscambio
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ButtonSdiContactVisible() As Boolean
        Get
            Return btnAddSdiContact.Visible
        End Get
        Set(ByVal value As Boolean)
            btnAddSdiContact.Visible = value
        End Set
    End Property

    ''' <summary> Descrizione con la quale si apre la finestra di ricerca. </summary>
    Private Property DefaultDescription() As String
        Get
            Return ViewState("txtContactDescr")
        End Get
        Set(ByVal value As String)
            ViewState("txtContactDescr") = value
        End Set
    End Property

    Public Property IsEnable() As Boolean
        Get
            If ViewState("_isEnable") Is Nothing Then
                ViewState("_isEnable") = True
            End If

            Return DirectCast(ViewState("_isEnable"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            ViewState("_isEnable") = value
        End Set
    End Property

    Public Property AjaxEnable() As Boolean
        Get
            If ViewState("_ajaxEnable") Is Nothing Then
                Return True
            Else
                Return ViewState("_ajaxEnable")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("_ajaxEnable") = value
        End Set
    End Property

    Public Property SearchAll As Boolean

    Public Property APIDefaultProvider As Boolean?
        Get
            Return CType(ViewState("_apiDefaultProvider"), Boolean?)
        End Get
        Set(ByVal value As Boolean?)
            ViewState("_apiDefaultProvider") = value
        End Set
    End Property

    Public Property EnvironmentType As String
        Get
            Return DirectCast(ViewState("collaborationType"), String)
        End Get
        Set(value As String)
            ViewState("collaborationType") = value
        End Set
    End Property

    Public Property CollaborationType As String

    Public Property TemplateName As String

    Public Property HistoricizeDate As DateTime?

    Public ReadOnly Property TemplateModels As List(Of TemplateAuthorizationModel)
        Get
            If _templateModels Is Nothing Then
                _templateModels = JsonConvert.DeserializeObject(Of List(Of TemplateAuthorizationModel))(DocSuiteContext.Current.ProtocolEnv.TemplatesAuthorizations)
            End If
            Return _templateModels
        End Get
    End Property

    Public Property FascicleContactEnabled As Boolean

    Public Property EnableFlagGroupChild As Boolean = False

    Public Property SearchInCategoryContacts As Integer?

    Public Property CategoryContactsProcedureType As String

    Public Property SearchInRoleContacts As Integer?

    Public ReadOnly Property TableContent As HtmlTable
        Get
            Return tableId
        End Get
    End Property

    Public Property RoleContactsProcedureType As String

    Public ReadOnly Property CurrentUserContact As String
        Get
            Dim contact As Contact = New Contact With {
                .Description = CommonUtil.GetInstance().UserDescription(),
                .EmailAddress = CommonUtil.GetInstance().UserMail(),
                .ContactType = New ContactType(ContactType.Person),
                .Code = DocSuiteContext.Current.User.FullUserName,
                .SearchCode = DocSuiteContext.Current.User.UserName
            }
            Return JsonConvert.SerializeObject(contact)
        End Get
    End Property
#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        btnSelContactDomain.Visible = ButtonSelectDomainVisible
        If Not DocSuiteContext.Current.ProtocolEnv.AbilitazioneRubricaOChart Then
            ButtonSelectOChartVisible = False
        End If

        If Not IsPostBack Then
            Initialize()
        End If

        InitializeAjax()
        InitializeControls()
        RadTreeContact.CheckBoxes = EnableCheck OrElse EnableCC
        RadTreeContact.Nodes(0).Checkable = False
        SetTooltips()
    End Sub

    Protected Sub uscContattiSel_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arg As String = e.Argument.Replace("~", "'")
        Dim arguments As String() = Split(arg, "|", 3)
        If Not arguments(0).Eq(ClientID) Then
            Exit Sub
        End If

        Select Case arguments(1)
            Case "OChartItemContact"
                Dim serialized As String = arguments(2)
                If String.IsNullOrWhiteSpace(serialized) Then
                    Return
                End If

                Dim contacts As List(Of Contact) = JsonConvert.DeserializeObject(Of List(Of Contact))(serialized)
                Dim identifiers As Integer() = contacts.Select(Function(c) c.Id).ToArray()
                Dim renewed As IList(Of Contact) = FacadeFactory.Instance.ContactFacade.GetContactWithId(identifiers)
                AddItems(renewed)

                Dim args As New OChartItemContactsEventArgs()
                args.Contacts = contacts
                args.ItemFullCode = contacts.First().Note
                RaiseEvent OChartItemContactsAdded(Me, args)

            Case "OChartItemContactManual"
                Dim serialized As String = arguments(2)
                If String.IsNullOrWhiteSpace(serialized) Then
                    Return
                End If

                Dim contacts As List(Of Contact) = JsonConvert.DeserializeObject(Of List(Of Contact))(serialized)
                For Each cc As Contact In contacts
                    cc.IsActive = 1S
                    cc.Parent = Nothing
                    cc.ContactType = New ContactType() With {.Id = "P"c}
                    cc.EmailAddress = RegexHelper.MatchEmail(cc.EmailAddress)
                    cc.CertifiedMail = RegexHelper.MatchEmail(cc.CertifiedMail)

                    MultipleContactsTest()
                    AddManualContact(cc, JsonConvert.SerializeObject(cc), ContactTypeEnum.Manual, String.Empty)
                Next

                Dim args As New OChartItemContactsEventArgs()
                args.Contacts = contacts
                RaiseEvent OChartItemContactsAdded(Me, args)

            Case "ContactAD"
                Dim localArg As String = HttpUtility.HtmlDecode(arguments(2))
                JsonContactAdded = localArg
                Dim contacts As List(Of Contact) = JsonConvert.DeserializeObject(Of List(Of Contact))(JsonContactAdded)

                For Each item As Contact In contacts
                    Dim temp As String = JsonConvert.SerializeObject(item)
                    item.CertifiedMail = RegexHelper.MatchEmail(item.CertifiedMail)
                    item.EmailAddress = RegexHelper.MatchEmail(item.EmailAddress)
                    AddManualContact(item, temp, ContactTypeEnum.Manual, "")
                    Dim args As New ContactsEventArgs()
                    args.Contacts = contacts
                    RaiseEvent ImportExcelContactAdded(Me, args)
                Next
                UpdateCount()
            Case "ManualMulti"
                For Each item As String In arguments(2).Split(";"c)
                    Dim name As String = RegexHelper.MatchName(item)
                    Dim email As String = RegexHelper.MatchEmail(item)

                    If String.IsNullOrEmpty(email) Then
                        Continue For
                    End If

                    Dim c As New Contact()
                    c.Description = If(String.IsNullOrEmpty(name), String.Empty, _mailDescritionExp.Replace(name, String.Empty))
                    c.CertifiedMail = email
                    c.IsActive = Convert.ToInt16(1)
                    c.Parent = Nothing

                    c.ContactType = New ContactType()
                    c.ContactType.Id = ContactType.Person

                    Dim temp As String = JsonConvert.SerializeObject(c)
                    AddManualContact(c, temp, ContactTypeEnum.Manual, "")

                Next
                UpdateCount()
            Case "Update"
                If Not String.IsNullOrEmpty(arguments(2)) Then
                    Dim ids As String() = arguments(2).Split(","c)
                    Dim idContactList(ids.Length) As Integer
                    For i As Integer = 0 To ids.Length - 1
                        idContactList(i) = Integer.Parse(ids(i))
                    Next
                    Dim contacts As IList(Of Contact) = Facade.ContactFacade.GetContactWithId(idContactList)
                    If IsFiscalCodeRequired Then
                        If contacts.Any(Function(x) String.IsNullOrEmpty(x.FiscalCode)) Then
                            BasePage.AjaxAlert("Alcuni contatti non hanno codice fiscale e sono stati scartati.")
                            contacts = contacts.Where(Function(x) Not String.IsNullOrEmpty(x.FiscalCode)).ToList()
                        End If
                    End If
                    AddItems(contacts)
                    ' Lancio l'evento che indica che un nuovo Contatto da Rubrica è stato aggiunto
                    Dim args As New ContactsEventArgs()
                    args.Contacts = contacts
                    RaiseEvent ItemsAdded(Me, args)

                End If
                UpdateCount()

            Case "DeleteAll"
                RadTreeContact.Nodes(0).Nodes.Clear()
                RaiseEvent ContactRemoved(Me, New EventArgs())

            Case "Import"
                Dim reader As New JsonTextReader(New StringReader(arguments(2)))
                While (reader.Read())
                    If (reader.TokenType = JsonToken.EndArray) Then
                        Exit While
                    End If
                    If (reader.TokenType <> JsonToken.StartArray) Then
                        reader.Read()
                        Dim info As New FileInfo(CType(reader.Value, String).Replace("\'", "'"))
                        Dim xmlDoc As New XmlDocument
                        If FileHelper.MatchExtension(info.Extension, FileHelper.XML) Then
                            Try
                                xmlDoc.Load(CommonUtil.GetInstance().AppTempPath & info.Name)
                                Dim listDto As IList(Of ContactDTO) = Facade.ContactFacade.ImportFromXml(xmlDoc)
                                DataSource = listDto
                                ExecuteDataBind(True, False, 0)
                            Catch ex As Exception
                                FileLogger.Warn(LoggerName, ex.Message, ex)
                                BasePage.AjaxAlert(ex.Message)
                            End Try
                        Else
                            BasePage.AjaxAlert("Formato del File non corretto{0}L'estensione deve essere .XLS{0}Importazione Interrotta", Environment.NewLine)
                        End If
                    End If
                End While
            Case "ImportExcel"
                ' Parsing dell'argomento per estrarre nome file excel
                Dim deserialized As Dictionary(Of String, String) = JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(arguments(2))
                Dim keys As New List(Of String)(deserialized.Keys)
                Dim filename As String = CommonInstance.AppTempPath & keys(0).TrimStart("""")
                ImportFromExcel(filename, False)

            Case Else
                Dim localArg As String = HttpUtility.HtmlDecode(arguments(2))
                JsonContactAdded = localArg
                Dim contact As Contact = JsonConvert.DeserializeObject(Of Contact)(JsonContactAdded)
                Select Case (arguments(1))
                    Case "Mod", "Vis"
                        UpdateManualContact(contact, localArg)
                    Case "Ins"
                        MultipleContactsTest()
                        SetManualContact(contact, localArg, contact.ContactType.Id, "", False)
                    Case "InsUsers"
                        MultipleContactsTest()
                        If CheckMultipleContacts(contact) Then
                            Exit Sub
                        End If
                        SetManualContact(contact, localArg, contact.ContactType.Id, "", False)
                End Select
        End Select

        UpdateCount()
        TreeValidator.Validate()
    End Sub

    Protected Sub btnDelContact_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs) Handles btnDelContact.Click
        Dim nodesToRemove As IList(Of RadTreeNode) = If(Me.EnableCheck, RadTreeContact.CheckedNodes, RadTreeContact.SelectedNodes)
        If nodesToRemove.IsNullOrEmpty() Then
            BasePage.AjaxAlert("Attenzione selezionare almeno un contatto.")
            Exit Sub
        End If
        If (nodesToRemove.Count = 1 AndAlso nodesToRemove.All(Function(f) f.Text = TreeViewCaption)) Then
            Exit Sub
        End If

        Dim templatemodel As TemplateAuthorizationModel = Nothing
        If TemplateModels IsNot Nothing Then
            templatemodel = TemplateModels.Where(Function(x) x.TemplateName = TemplateName).FirstOrDefault()
        End If

        Dim contactsList As List(Of Contact) = New List(Of Contact)

        For Each node As RadTreeNode In nodesToRemove
            Dim contactNode As String = node.Attributes(ManualContactAttribute)

            'Add contacts to list to be removed from session
            If contactNode IsNot Nothing Then
                Dim contact As Contact = JsonConvert.DeserializeObject(Of Contact)(node.Attributes(ManualContactAttribute))
                contactsList.Add(contact)
            End If

            If Not node.Enabled Then
                Continue For
            End If

            If templatemodel IsNot Nothing AndAlso Not templatemodel.LockedSigners.IsNullOrEmpty AndAlso templatemodel.LockedSigners.Contains(node.Text) Then
                Continue For
            End If

            Dim parentNode As RadTreeNode = node.ParentNode
            node.Remove()
            _lastRemovedNode = node

            If parentNode IsNot Nothing Then CheckParentNodes(parentNode)
            RaiseEvent ContactRemoved(Me, New EventArgs())
        Next

        'Register script to remove contact from session
        If Not contactsList.Count = 0 Then
            Dim jsonContactsToRemove As String = JsonConvert.SerializeObject(contactsList)
            Dim script As String = DeleteContactsSessionStorage(jsonContactsToRemove, ClientID)
            AjaxManager.ResponseScripts.Add(script)
        End If

        UpdateCount()
    End Sub

    Private Sub btnContactMaxItems_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs) Handles btnContactMaxItems.Click
        RaiseEvent ShowContactList(Me, New EventArgs)
        ExecuteDataBind(False, False, 0)
        btnContactMaxItems.Visible = False
        cmdDetails.Visible = True
        CheckAllNodes()
    End Sub

    Private Sub BtnImportContactManualClick(ByVal sender As Object, ByVal e As ImageClickEventArgs) Handles btnImportContactManual.Click
        Dim allowedExtensions As String = Server.UrlDecode(".xls,.xlsx")
        Dim url As String = String.Format("../UserControl/CommonUploadDocument.aspx?{0}=true&{1}={2}", CommonUploadDocument.ExcelImportQueryItem, CommonUploadDocument.AllowedExtensionsQueryItem, allowedExtensions)
        Dim closeImportManualFunction As String = String.Format(DefaultOpenWindowScript, ID, url, "windowImportContact", "_CloseImportManualFunction")
        AjaxManager.ResponseScripts.Add(closeImportManualFunction)
    End Sub

    Protected Sub cmdDetails_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs) Handles cmdDetails.Click
        Dim callBackFunc As String = "_CloseFunction"

        If [ReadOnly] Then
            Dim readOnlyUrl As New StringBuilder("../UserControl/CommonSelContactManual.aspx?Action=Vis")
            readOnlyUrl.AppendFormat("&JsonContact={0}", EncodeContact(RadTreeContact.SelectedNode.Attributes(ManualContactAttribute)))
            If SimpleMode Then
                readOnlyUrl.Append("&SimpleMode=true")
            End If
            AjaxManager.ResponseScripts.Add(String.Format(DefaultOpenWindowScript, ID, readOnlyUrl.ToString(), "windowSelContactManual", callBackFunc))
            Exit Sub
        End If

        Dim node As RadTreeNode = RadTreeContact.SelectedNode
        If (node Is Nothing) OrElse (node.Value.Eq(ROOT_DESC)) Then
            BasePage.AjaxAlert("Nodo selezionato non valido")
            Exit Sub
        End If

        Dim url As New StringBuilder()
        Select Case node.Attributes("ContactPart")
            Case CONTACT_MANUAL
                url.Append("../UserControl/CommonSelContactManual.aspx?Action=Vis")
                If ReadOnlyProperties Then
                    url.Append("&OnlyManualDetail=true")
                End If
                url.AppendFormat("&JsonContact={0}", EncodeContact(node.Attributes(ManualContactAttribute)))
                callBackFunc = "_CloseManualFunction"

            Case CONTACT_ROLEUSER
                url.Append("../UserControl/CommonSelContactManual.aspx?Action=Vis")
                url.AppendFormat("&JsonContact={0}", EncodeContact(node.Attributes(ManualContactAttribute)))

            Case CONTACT_IPA
                url.Append("../UserControl/CommonSelContactManual.aspx?Action=Vis&ReadOnly=true")
                url.AppendFormat("&JsonContact={0}", EncodeContact(node.Attributes(ManualContactAttribute)))

            Case CONTACT_ADDRESSBOOK
                url.Append("../UserControl/CommonSelContactRubrica.aspx?Action=Vis&OnlyDetail=true")
                url.AppendFormat("&idContact={0}", node.Value)
                url.AppendFormat("&AVCPBusinessContactEnabled={0}", AVCPBusinessContactEnabled)
                url.AppendFormat("&FascicleContactEnabled={0}", FascicleContactEnabled)

        End Select

        If ProtType Then
            url.Append("&ProtType=true")
        End If

        If SimpleMode Then
            url.Append("&SimpleMode=true")
        End If

        AjaxManager.ResponseScripts.Add(String.Format(DefaultOpenWindowScript, ID, url.ToString(), "windowSelContactManual", callBackFunc))

    End Sub

    Private Sub btnAddMyself_Click(sender As Object, e As ImageClickEventArgs) Handles btnAddMyself.Click
        Try
            Dim c As New Contact()
            c.Description = CommonUtil.GetInstance().UserDescription
            c.CertifiedMail = CommonUtil.GetInstance().UserMail
            c.IsActive = 1S
            c.Parent = Nothing

            c.ContactType = New ContactType(ContactType.Person)

            AddManualContact(c, JsonConvert.SerializeObject(c), ContactTypeEnum.Manual, "")

            UpdateCount()

        Catch ex As Exception
            FileLogger.Error(LoggerName, "Errore inserimento indirizzo Operatore", ex)
            BasePage.AjaxAlert("Errore in fase di inserimento indirizzo Operatore")
        End Try
    End Sub

    Private Sub btnAddSdiContact_Click(sender As Object, e As ImageClickEventArgs) Handles btnAddSdiContact.Click
        Dim dto As ContactDTO = ProtocolContactFacade.GetSdiContactDto(CurrentProtocol)
        If dto Is Nothing Then
            BasePage.AjaxAlert("Non è presente un indirizzo valido per il Sistema di Interscambio. Verificare il parametro InvoicePAContactSDI")
            Exit Sub
        End If

        AddManualContact(dto.Contact, JsonConvert.SerializeObject(dto.Contact), ContactTypeEnum.Manual, "")
        UpdateCount()
    End Sub

    Protected Sub TreeValidator_ServerValidate(ByVal source As Object, ByVal args As ServerValidateEventArgs)
        args.IsValid = RadTreeContact.Nodes(0).Nodes.Count <> 0
    End Sub

#End Region

#Region " Methods "

    Public Sub Initialize()
        ' Imposto le immgini dei pulsanti
        btnImportContact.ImageUrl = ImagePath.SmallXml
        ButtonSelContactOChart.ImageUrl = ImagePath.SmallNetworkShare

        ' Inizializzo i codici JS di apertura finestra lato Client
        btnAddManual.OnClientClick = AddManualCode()
        btnSelContact.OnClientClick = AddSelContactCode()
        btnSelContact2.OnClientClick = AddSelContactCode()
        btnIPAContact.OnClientClick = AddIpaCode()
        If ProtocolEnv.AUSIntegrationEnabled Then
            btnIPAContact.OnClientClick = String.Format(DefaultOpenWindowScript, ID, $"../UserControl/CommonSelAUSContact.aspx?Type={BasePage.Type}&ParentID={ID}", "windowSelContact", "_CloseManualFunction")
        End If
        Dim contactDomainUrl As String
        contactDomainUrl = $"../UserControl/CommonSelContactDomain.aspx?Type={BasePage.Type}&ParentID={ID}"
        If Not String.IsNullOrEmpty(ProtocolEnv.OmniBusApplicationKey) Then
            contactDomainUrl = $"../UserControl/CommonSelContactOmniBus.aspx?Type={BasePage.Type}&ParentID={ID}"
        End If
        btnSelContactDomain.OnClientClick = String.Format(DefaultOpenWindowScript, ID, contactDomainUrl, "windowSelContact", "_CloseDomain")

        'Selezione contatti per collaborazione
        Dim params As StringBuilder = New StringBuilder()
        params.Append("Titolo=Autorizzazioni&Action=SelResp")
        If Not String.IsNullOrEmpty(CollaborationType) Then
            params.AppendFormat("&CollaborationType={0}", CollaborationType)
        End If
        If Not String.IsNullOrEmpty(EnvironmentType) Then
            params.AppendFormat("&Type={0}", EnvironmentType)
        End If
        btnRoleUser.OnClientClick = String.Format(DefaultOpenWindowScript, ID, "../User/UserSelRoleUser.aspx?" & CommonShared.AppendSecurityCheck(params.ToString()), "windowSelRoleUser", "_CloseRole")

        btnImportContact.OnClientClick = String.Format(DefaultOpenWindowScript, ID, String.Format("../UserControl/CommonUploadDocument.aspx?allowedextensions={0}", Server.UrlDecode(FileHelper.XML)), "windowImportContact", "_CloseManualFunction")
        btnAddManualMulti.OnClientClick = String.Format("{0}_ManualMulti(); return false;", ID)
        If ProtocolEnv.EnableContactAndDistributionGroup Then
            btnAddManualMulti.OnClientClick = String.Format(DefaultOpenWindowScript, ID, "../UserControl/CommonSelContactEvolution.aspx?Type=" & BasePage.Type & "&ParentID= " & ID, "windowSelContact", "_CloseManualMulti")
        End If
        InitializeButtonSelContactOChart()
        InitializeContactRest()

    End Sub

    Private Sub InitializeControls()
        WebUtils.ExpandOnClientNodeAttachEvent(RadTreeContact)
        If [ReadOnly] Then
            EnableCC = False
            Multiple = True
            IsRequired = False
        End If

        If _singleCheck Then
            RadTreeContact.OnClientNodeChecking = ID & "_ClientNodeChecking"
        End If

        lblCount.Visible = EnableCount

        If SimpleMode AndAlso Not String.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.PECSimpleModeHideButtons) Then
            ' Rubrica|Adam|Elimina|Manuale|Info|Import|ImportManual|Ipa|Role
            Dim parameters As New List(Of String)(DocSuiteContext.Current.ProtocolEnv.PECSimpleModeHideButtons.ToLowerInvariant().Split("|"c))

            If parameters.Contains("rubrica") Then
                ButtonSelectVisible = False
            End If
            If parameters.Contains("domain") Then
                ButtonSelectDomainVisible = False
            End If
            If parameters.Contains("ochart") Then
                ButtonSelectOChartVisible = False
            End If
            If parameters.Contains("elimina") Then
                ButtonDeleteVisible = False
            End If
            If parameters.Contains("manuale") Then
                ButtonManualVisible = False
            End If
            If parameters.Contains("info") Then
                ButtonPropertiesVisible = False
            End If
            If parameters.Contains("import") Then
                ButtonImportVisible = False
            End If
            If parameters.Contains("importmanual") Then
                ButtonImportManualVisible = False
            End If
            If parameters.Contains("ipa") Then
                ButtonIPAVisible = False
            End If
            If parameters.Contains("role") Then
                ButtonRoleVisible = False
            End If
            If parameters.Contains("sdicontact") Then
                ButtonSdiContactVisible = False
            End If
        End If
    End Sub

    Private Sub InitializeAjax()
        If [ReadOnly] Then
            Exit Sub
        End If

        AddHandler AjaxManager.AjaxRequest, AddressOf uscContattiSel_AjaxRequest
        If Not AjaxEnable Then
            Exit Sub
        End If

        AjaxManager.AjaxSettings.AddAjaxSetting(btnDelContact, RadTreeContact)
        AjaxManager.AjaxSettings.AddAjaxSetting(RadTreeContact, RadTreeContact)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, RadTreeContact)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnContactMaxItems, RadTreeContact)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnContactMaxItems, panelButtons)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnContactMaxItems, pnlIntestazione)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnImportContactManual, RadTreeContact)

        If HeaderVisible Then
            AjaxManager.AjaxSettings.AddAjaxSetting(btnDelContact, pnlIntestazione)
        End If

        If EnableCount Then
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlIntestazione)
        End If

        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, panelButtons)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, panelOnlyContact)

        If btnAddMyself.Visible Then
            AjaxManager.AjaxSettings.AddAjaxSetting(btnAddMyself, RadTreeContact)
        End If

    End Sub

    Public Function GetContacts(allSelected As Boolean) As IList(Of ContactDTO)
        Dim retval As New List(Of ContactDTO)

        ' Contatti Rubrica
        retval.AddRange(GetAddressContacts(allSelected))

        ' Contatti Manuali
        retval.AddRange(GetManualContacts())

        Return retval
    End Function

    Public Function GetManualContacts() As IList(Of ContactDTO)
        Dim manualContactList As New List(Of ContactDTO)
        'Se c'è il bottone di espansione, significa che il nodo è fittizio perchè raggruppato
        'Devo quindi estrarre i veri contatti
        If btnContactMaxItems.Visible Then
            'esegue il caricamento dei contatti manuali
            Dim lm As IList(Of ProtocolContactManual) = Facade.ProtocolContactManualFacade.GetByProtocol(CurrentProtocol)
            For Each protContact As ProtocolContactManual In lm
                Dim vContactDto As ContactDTO = New ContactDTO()
                vContactDto.Contact = protContact.Contact
                'Ricarico l'informazione originale di "Copia Conoscenza"
                vContactDto.IsCopiaConoscenza = (protContact.Type = "CC")
                vContactDto.Type = ContactDTO.ContactType.Manual
                vContactDto.IdManualContact = protContact.Id
                If protContact.ComunicationType.Eq(ProtocolContactCommunicationType.Recipient) Then
                    manualContactList.Add(vContactDto)
                End If
            Next
            Return manualContactList
        End If

        For Each node As RadTreeNode In RadTreeContact.Nodes(0).Nodes
            Select Case node.Attributes("ContactPart")
                Case CONTACT_MANUAL, CONTACT_IPA, CONTACT_ROLEUSER
                    Dim vContact As New ContactDTO
                    vContact.Contact = JsonConvert.DeserializeObject(Of Contact)(node.Attributes(ManualContactAttribute))
                    vContact.IsCopiaConoscenza = node.Checked
                    If Not String.IsNullOrEmpty(node.Attributes("IdManualContact")) Then
                        vContact.IdManualContact = JsonConvert.DeserializeObject(Of Guid)(node.Attributes("IdManualContact"))
                        vContact.Contact = Facade.ProtocolContactManualFacade.GetById(vContact.IdManualContact.Value).Contact
                    End If
                    vContact.Contact = vContact.Contact.EscapingJSON(vContact.Contact, Function(f) HttpUtility.HtmlDecode(f))
                    manualContactList.Add(vContact)
            End Select
        Next
        Return manualContactList
    End Function

    Public Function GetSelectedContacts() As IList(Of ContactDTO)
        Dim manualContactList As New List(Of ContactDTO)

        For Each node As RadTreeNode In RadTreeContact.CheckedNodes
            Select Case node.Attributes("ContactPart")
                Case CONTACT_MANUAL, CONTACT_IPA, CONTACT_ROLEUSER
                    Dim vContact As New ContactDTO
                    vContact.Contact = JsonConvert.DeserializeObject(Of Contact)(node.Attributes(ManualContactAttribute))

                    If Not String.IsNullOrEmpty(node.Attributes("IdManualContact")) Then
                        vContact.IdManualContact = JsonConvert.DeserializeObject(Of Guid)(node.Attributes("IdManualContact"))
                        vContact.Contact = Facade.ProtocolContactManualFacade.GetById(vContact.IdManualContact.Value).Contact
                    End If
                    manualContactList.Add(vContact)
                Case CONTACT_ADDRESSBOOK
                    Dim vContactDto As New ContactDTO()
                    vContactDto.Contact = Facade.ContactFacade.GetById(Integer.Parse(node.Value))
                    vContactDto.Type = ContactDTO.ContactType.Address
                    manualContactList.Add(vContactDto)
            End Select
        Next
        Return manualContactList
    End Function

    Public Function GetAddressContacts(allSelected As Boolean) As IList(Of ContactDTO)
        Dim contactList As IList(Of ContactDTO) = New List(Of ContactDTO)
        'contatti da rubrica
        If allSelected Then
            GetAllSelectedChildNodes(RadTreeContact.Nodes(0), contactList)
        Else
            GetChildNodesRecursively(RadTreeContact.Nodes(0), contactList)
        End If

        Return contactList
    End Function

    ''' <summary> Carica eventuali contatti OChart (se presenti) </summary>
    Public Function GetOChartContacts() As IList(Of ContactDTO)
        Return CType(GetContacts(False), List(Of ContactDTO)).FindAll(Function(x) Not String.IsNullOrEmpty(x.Contact.Note))
    End Function

    ''' <summary> Importazione da xls in annessi. </summary>
    Public Sub ImportFromExcel(ByVal contact As DocumentInfo, throwException As Boolean)
        If contact IsNot Nothing Then
            ImportFromExcel(contact.SaveToDisk(New DirectoryInfo(CommonInstance.AppTempPath)).FullName, throwException)
        End If
    End Sub

    Private Sub ImportFromExcel(ByVal filename As String, throwExceptions As Boolean)
        Try
            If Not FileHelper.MatchExtension(filename, FileHelper.XLS) Then
                Throw New DocSuiteException("Importazione Excel", String.Format("Formato del File non corretto, necessaria estensione [{0}].", FileHelper.XLS))
            End If

            'Richiamo la pagina di importazione
            Dim nameWithoutPath As String = Path.GetFileName(filename)
            Dim script As String = String.Format(DefaultOpenWindowScript, ID, "../UserControl/CommonExcelImportContact.aspx?Type=" & BasePage.Type & "&ParentID= " & ID & "&FileName=" & nameWithoutPath, "windowImportContact", "_CloseManualMulti")
            AjaxManager.ResponseScripts.Add(script)

        Catch ex As DocSuiteException
            If throwExceptions Then
                Throw
            End If
            ' Errore utente: loggo per ottimizzazioni future
            FileLogger.Info(LoggerName, "Errore utente In importazione excel.", ex)
            BasePage.AjaxAlert("Attenzione:{0}{1}", Environment.NewLine, ex.Descrizione)

        Catch ex As Exception
            If throwExceptions Then
                Throw
            End If
            ' Errore non previsto
            FileLogger.Warn(LoggerName, "Errore non gestito in importazione excel.", ex)
            BasePage.AjaxAlert("Importazione Interrotta, contattare un tecnico:{0}{1}", Environment.NewLine, ex.Message)
        End Try
    End Sub

    Private Sub SetManualContact(ByVal contact As Contact, ByVal jsonContact As String, ByVal contactType As Char, ByVal jsonIdManualContact As String, ByVal isCc As Boolean)
        SetManualContact(contact, jsonContact, contactType, jsonIdManualContact, isCc, False)
    End Sub

    Private Sub SetManualContact(ByVal contact As Contact, ByVal jsonContact As String, ByVal contactType As Char, ByVal jsonIdManualContact As String, ByVal isCc As Boolean, ByVal isLocked As Boolean)
        Select Case Char.ToUpperInvariant(contactType)
            Case "I"c
                AddManualContact(contact, jsonContact, ContactTypeEnum.IPA, "", isLocked, isCc)
                RaiseEvent IPAContactAdded(Me, New EventArgs())
            Case "R"c
                AddManualContact(contact, jsonContact, ContactTypeEnum.RoleUser, "", isLocked, isCc)
                RaiseEvent RoleUserContactAdded(Me, New EventArgs())
            Case Else
                AddManualContact(contact, jsonContact, ContactTypeEnum.Manual, jsonIdManualContact, isLocked, isCc)
                Dim args As New ContactsEventArgs()
                args.Contacts = contact.AsList()

                If UseSessionStorage Then
                    Dim script As String = SetContactSessionStorage(jsonContact, ClientID)
                    AjaxManager.ResponseScripts.Add(script)
                End If

                RaiseEvent ManualContactAdded(Me, args)
        End Select
    End Sub

    Public Shared Function SetContactSessionStorage(jsonContact As String, clientId As String) As String
        Dim script As String = String.Format("{0}_uscContattiSel.setContacts({1});", clientId, jsonContact)
        Return script
    End Function
    Public Shared Function DeleteContactsSessionStorage(jsonContact As String, clientId As String) As String
        Dim script As String = String.Format("{0}_uscContattiSel.deleteContacts({1});", clientId, jsonContact)
        Return script
    End Function

    Private Sub AddManualContact(ByVal contact As Contact, ByVal manualContact As String, ByVal contactType As ContactTypeEnum, ByVal jsonIdManualContact As String, Optional ByVal isCc As Boolean = False)
        AddManualContact(contact, manualContact, contactType, jsonIdManualContact, False, isCc)
    End Sub

    Private Sub AddManualContact(ByVal contact As Contact, ByVal manualContact As String, ByVal contactType As ContactTypeEnum, ByVal jsonIdManualContact As String, ByVal isLocked As Boolean, Optional ByVal isCc As Boolean = False)
        Dim node As New RadTreeNode()
        node.Text = contact.FullDescription(SimpleMode)
        node.Value = String.Empty
        node.Attributes.Add("ContactType", CType(contact.ContactType.Id, String).ToUpperInvariant())
        node.Attributes.Add(ManualContactAttribute, manualContact)
        node.Expanded = True
        node.Font.Bold = True
        If isLocked Then
            node.Enabled = False
        End If
        node.Attributes.Add("Selected", "1")
        If ReadOnlyProperties Then
            node.Attributes.Add("FIP", contact.FullIncrementalPath)
        End If

        Select Case contactType
            Case ContactTypeEnum.Manual
                node.ImageUrl = Page.ResolveClientUrl("../Comm/Images/Interop/Manuale.gif")
                node.Attributes.Add("ContactPart", CONTACT_MANUAL)
                node.Attributes.Add("IdManualContact", jsonIdManualContact)
            Case ContactTypeEnum.IPA
                node.ImageUrl = Page.ResolveClientUrl("../Comm/Images/Interop/Building.gif")
                node.Attributes.Add("ContactPart", CONTACT_IPA)
            Case ContactTypeEnum.RoleUser
                node.ImageUrl = Page.ResolveClientUrl("../Comm/Images/Interop/AdAm.gif")
                node.Attributes.Add("ContactPart", CONTACT_ROLEUSER)
        End Select
        'copia conoscenza
        SetForCC(node, isCc)
        'colore nodo
        TreeViewUtils.ChangeNodesForeColor(node, If(IsEnable, Color.Empty, Color.Gray))
        'aggiunge alla lista il nodo

        If TemplateName IsNot Nothing AndAlso TemplateModels.Where(Function(x) x.TemplateName = TemplateName AndAlso x.AddSignersOnTop = True).Any() Then
            If (contact.FullIncrementalPath Is Nothing) OrElse Not contact.FullIncrementalPath.Contains("|") Then
                Dim position As Integer = 0
                Dim templateNode As RadTreeNode = RadTreeContact.Nodes(0).GetAllNodes().FirstOrDefault(Function(f) Not String.IsNullOrEmpty(f.Attributes().Item(FromTemplateAttribute)))
                If templateNode IsNot Nothing Then
                    position = templateNode.Index
                End If
                RadTreeContact.Nodes(0).Nodes.Insert(position, node)
                Exit Sub
            End If
        Else
            If (contact.FullIncrementalPath Is Nothing) OrElse Not contact.FullIncrementalPath.Contains("|") Then
                RadTreeContact.Nodes(0).Nodes.Add(node)
                Exit Sub
            End If
        End If

        Dim index As Integer = contact.FullIncrementalPath.LastIndexOf("|", StringComparison.OrdinalIgnoreCase)
        If index < 0 Then
            index = 0
        End If
        Dim parentNode As RadTreeNode = RadTreeContact.FindNodeByAttribute("FIP", contact.FullIncrementalPath.Substring(0, index))

        If TemplateName IsNot Nothing AndAlso Not TemplateModels.Select(Function(x) x.TemplateName = TemplateName AndAlso x.AddSignersOnTop = True).Any() Then
            If parentNode IsNot Nothing Then
                parentNode.Nodes.Add(node)
            Else
                RadTreeContact.Nodes(0).Nodes.Add(node)
            End If
        Else
            If parentNode IsNot Nothing Then
                parentNode.Nodes.Insert(0, node)
            Else
                RadTreeContact.Nodes(0).Nodes.Insert(0, node)
            End If
        End If
    End Sub

    Private Sub UpdateManualContact(ByVal contact As Contact, ByVal manualContact As String)
        Dim node As RadTreeNode = RadTreeContact.SelectedNode
        If node Is Nothing Then
            Return
        End If

        If SimpleMode AndAlso Not String.IsNullOrEmpty(contact.CertifiedMail) Then
            node.Text = String.Format("{0} ({1})", Replace(contact.Description, "|", " "), contact.CertifiedMail)
        Else
            node.Text = Replace(contact.Description, "|", " ")
        End If
        node.Value = String.Empty
        node.ImageUrl = Page.ResolveClientUrl("~/Comm/Images/Interop/Manuale.gif")
        node.Attributes.Add("ContactType", CType(contact.ContactType.Id, String).ToUpperInvariant())
        node.Attributes.Add("ContactPart", CONTACT_MANUAL)
        node.Attributes.Add(ManualContactAttribute, manualContact)
        node.Expanded = True
    End Sub

    Private Sub AddItems(ByVal contacts As IList(Of Contact))
        AddItems(contacts, Nothing)
    End Sub

    Private Sub AddItems(ByVal contacts As IList(Of Contact), isCc As Boolean?)
        AddItems(contacts, isCc, False)
    End Sub

    Private Sub AddItems(ByVal contacts As IList(Of Contact), isCc As Boolean?, isLocked As Boolean)
        If contacts.IsNullOrEmpty Then
            Return
        End If

        MultipleContactsTest()

        Dim allWithFiscalCode As Boolean = True
        For Each contact As Contact In contacts
            Dim node As RadTreeNode = AddNode(Nothing, contact)
            If isCc.HasValue Then
                SetForCC(node, isCc.Value)
            End If
            node.Checkable = True
            node.Font.Bold = True
            If isLocked Then
                node.Enabled = False
            End If
            node.Attributes.Add("Selected", "1")
        Next



        RaiseEvent ContactAdded(Me, New EventArgs())

    End Sub

    Private Sub AddItem(ByVal contact As Contact, cc As Boolean?)
        AddItem(contact, cc, False)
    End Sub

    Private Sub AddItem(ByVal contact As Contact, cc As Boolean?, isLocked As Boolean)
        AddItems(New List(Of Contact) From {contact}, cc, isLocked)
    End Sub

    Private Sub SetForCC(ByRef node As RadTreeNode, ByVal isCc As Boolean)
        If Not EnableCC Then
            node.Text &= If(isCc, " (CC)", "")
        Else
            node.Checked = isCc
        End If
    End Sub

    Private Function AddNode(ByRef node As RadTreeNode, ByVal contact As Contact) As RadTreeNode
        Dim nodeToAdd As New RadTreeNode()
        If RadTreeContact.FindNodeByValue(contact.Id.ToString()) IsNot Nothing Then
            Return RadTreeContact.FindNodeByValue(contact.Id.ToString())
        End If

        If HistoricizeDate.HasValue AndAlso contact.ContactNames IsNot Nothing AndAlso contact.ContactNames.Any() Then
            contact.Description = contact.ContactNames.SingleOrDefault(Function(f) f.FromDate <= HistoricizeDate.Value AndAlso (Not f.ToDate.HasValue OrElse (f.ToDate.HasValue AndAlso HistoricizeDate.Value <= f.ToDate.Value))).Name
        End If

        nodeToAdd.Text = ContactFacade.FormatContact(contact)
        nodeToAdd.Value = contact.Id.ToString()
        nodeToAdd.ImageUrl = ImagePath.ContactTypeIcon(contact.ContactType.Id, contact.isLocked.HasValue AndAlso contact.isLocked.Value = 1)
        nodeToAdd.Attributes.Add("ContactType", contact.ContactType.Id)
        nodeToAdd.Attributes.Add("ContactPart", CONTACT_ADDRESSBOOK)
        nodeToAdd.Expanded = True
        TreeViewUtils.ChangeNodesForeColor(nodeToAdd, If(IsEnable, Color.Empty, Color.Gray))

        If contact IsNot Nothing Then
            If contact.Parent Is Nothing Then 'Primo Livello
                RadTreeContact.Nodes(0).Nodes.Add(nodeToAdd)
            Else
                Dim newNode As RadTreeNode = RadTreeContact.FindNodeByValue(contact.Parent.Id.ToString())
                If (newNode Is Nothing) Then
                    AddNode(nodeToAdd, contact.Parent)
                Else
                    newNode.Nodes.Add(nodeToAdd)
                End If
            End If
        End If
        If node IsNot Nothing Then
            nodeToAdd.Nodes.Add(node)
        End If
        nodeToAdd.Checkable = False

        Return nodeToAdd
    End Function

    Public Sub CheckAll()
        CheckAllRecursive(RadTreeContact.Nodes(0).Nodes)
    End Sub
    Public Sub CheckAllRecursive(nodes As RadTreeNodeCollection)
        For Each childNode As RadTreeNode In nodes
            If childNode.Checkable Then
                childNode.Checked = True
            End If
            If childNode.Nodes IsNot Nothing AndAlso childNode.Nodes.Count > 0 Then
                CheckAllRecursive(childNode.Nodes)
            End If

        Next
    End Sub

    Private Sub GetChildNodesRecursively(ByVal node As RadTreeNode, ByRef contacts As IList(Of ContactDTO))
        If (node.Nodes.Count = 0) Then
            If (Not String.IsNullOrEmpty(node.Value)) AndAlso (Not node.Value.Eq(ROOT_DESC)) AndAlso (node.Value.IndexOf("§") < 0) Then
                Dim vContactDto As New ContactDTO()
                vContactDto.Contact = Facade.ContactFacade.GetById(Integer.Parse(node.Value))
                vContactDto.IsCopiaConoscenza = node.Checked
                If node.Attributes("ContactPart").Eq(CONTACT_ADDRESSBOOK) AndAlso node.Attributes("Selected").Eq("1") Then
                    vContactDto.Type = ContactDTO.ContactType.Address
                    contacts.Add(vContactDto)
                End If
            End If
        End If
        For Each childNode As RadTreeNode In node.Nodes
            GetChildNodesRecursively(childNode, contacts)
        Next
    End Sub

    Private Sub GetAllSelectedChildNodes(ByVal node As RadTreeNode, ByRef contacts As IList(Of ContactDTO))
        If node Is Nothing Then
            Exit Sub
        End If

        If (Not String.IsNullOrEmpty(node.Value)) AndAlso (Not node.Value.Eq(ROOT_DESC)) AndAlso (node.Value.IndexOf("§") < 0) Then
            Dim vContact As Contact = Facade.ContactFacade.GetById(Integer.Parse(node.Value))
            Dim vContactDto As New ContactDTO()
            vContactDto.Contact = vContact
            If node.Attributes("Selected").Eq("1") Then
                vContactDto.Type = ContactDTO.ContactType.Address
                vContactDto.IsCopiaConoscenza = node.Checked
                contacts.Add(vContactDto)
            End If
        End If
        For Each childNode As RadTreeNode In node.Nodes
            GetAllSelectedChildNodes(childNode, contacts)
        Next
    End Sub

    ''' <summary>  </summary>
    ''' <param name="node">Nodo padre del nodo eliminato</param>
    ''' <remarks>
    ''' Nella casistica di gestione nodi 7.2.9 style, sono considerati nodi validi solo le foglie.
    ''' Di conseguenza nel momento in cui elimino una foglia, se non avevo checkato il padre 
    ''' devo eliminare anche tutti i padri non checkati: altrimenti in fase di lettura risulterà un contatto vuoto.
    ''' </remarks>
    Private Shared Sub CheckParentNodes(ByRef node As RadTreeNode)
        ''Verifico se esiste il nodo corrente
        If node Is Nothing OrElse node.Value.Eq(ROOT_DESC) Then
            Exit Sub
        End If
        '' Verifico se il nodo NON possiede l'attributo di selezione
        If node.Attributes("Selected").Eq("1") Then
            Exit Sub
        End If
        '' Verifico se dopo l'eliminazione del nodo precedente ha ancora figli
        If node.Nodes.Count > 0 Then
            Exit Sub
        End If
        'Se non ha più figli allora lo posso eliminare
        Dim parentNode As RadTreeNode = node.ParentNode
        node.Remove()
        '' Verifico il padre
        CheckParentNodes(parentNode)
    End Sub

    ''' <summary> Imposta i ToolTip dei Pulsanti. </summary>
    Private Sub SetTooltips()
        Dim sSingolare As String
        Select Case lblCaption.Text.ToLower
            Case "mittenti"
                sSingolare = "Mittente"
            Case "destinatari"
                sSingolare = "Destinatario"
            Case Else
                sSingolare = lblCaption.Text
        End Select

        btnSelContact.ToolTip = String.Format("Inserisci {0} da Rubrica", lblCaption.Text)
        btnAddSdiContact.ToolTip = String.Format("Inserisci {0} SDI.", lblCaption.Text)
        btnSelContactDomain.ToolTip = String.Format("Inserisci {0} da Rubrica Aziendale", lblCaption.Text)
        ButtonSelContactOChart.ToolTip = String.Format("Inserisci {0} da Rubrica Organigramma", lblCaption.Text)
        btnAddManual.ToolTip = String.Format("Inserisci {0} Manuale", sSingolare)
        btnAddManualMulti.ToolTip = String.Format("Inserisci {0} Multiplo", sSingolare)
        If ProtocolEnv.EnableContactAndDistributionGroup Then
            btnAddManualMulti.ToolTip = "Inserimento Contatti e Liste di Distribuzione"
        End If

        btnDelContact.ToolTip = "Elimina " & sSingolare
        cmdDetails.ToolTip = "Proprietà del " & sSingolare
        btnImportContact.ToolTip = "Importazione Contatti nei " & lblCaption.Text
        btnImportContactManual.ToolTip = "Importazione Contatti Manuali nei " & lblCaption.Text
        btnRoleUser.ToolTip = "Inserisci Dirigente o Vice"
        btnIPAContact.ToolTip = String.Format("Inserisci {0} da IPA", lblCaption.Text)
        If ProtocolEnv.AUSIntegrationEnabled Then
            btnIPAContact.ToolTip = "Rubrica AUS"
        End If

        btnAddMyself.ToolTip = "Invia a me"
    End Sub

    ''' <summary> Testa se è attivo il flag per inserimento contatti multipli. </summary>
    ''' <remarks>  Se non è attivo pulisce la treeview. </remarks>
    Private Sub MultipleContactsTest()
        If Not Multiple Then
            RadTreeContact.Nodes(0).Nodes.Clear()
        End If
    End Sub

    ''' <summary> Controlla che non sia presente lo stesso utente. </summary>
    ''' <remarks>  Se lo trova restituisce True.  </remarks>
    Private Function CheckMultipleContacts(ByVal contact As Contact) As Boolean
        For Each n As RadTreeNode In RadTreeContact.Nodes
            For Each child As RadTreeNode In n.Nodes
                Dim foundNode As RadTreeNode = n.Nodes.FindNodeByText(contact.FullDescription)
                If foundNode IsNot Nothing Then
                    Return True
                End If
            Next
            Return False
        Next
    End Function

    Public Overrides Sub DataBind() Implements IBindingUserControl(Of ContactDTO).DataBind
        DataBind(0)
    End Sub

    Public Overloads Sub DataBind(ByVal contactsCount As Integer)
        ExecuteDataBind(False, EnableCompression, contactsCount)
    End Sub

    ''' <summary> Popola la TreeView con l'elenco contatti </summary>
    ''' <param name="append"> True: appende i contatti, False: ripulisce la treeview e poi aggiunge i contatti </param>
    ''' <param name="showCompressed"> Indica se visualizzare i contatti compressi </param>
    ''' <param name="countToShow"> Numero di contatti da mostrare nel controllo compresso </param>
    Private Sub ExecuteDataBind(ByVal append As Boolean, ByVal showCompressed As Boolean, ByVal countToShow As Integer)
        If Not showCompressed Then
            BindData(append)
            UpdateCount(Count)
            Exit Sub
        End If

        ' Utilizza i contatore passato come parametro, se non valorizzato cerca di recuperare il valore dalla lista contatti
        countToShow = If(countToShow > 0, countToShow, DataSource.Count)
        ' Aggiunge alla treeview un numero contatti limitato
        If countToShow > ProtocolEnv.ContactMaxItems Then
            RadTreeContact.Nodes(0).Nodes.Clear()
            RadTreeContact.Nodes(0).Nodes.Add(New RadTreeNode(String.Format("{0} ({1})", TreeViewCaption, countToShow), ROOT_DESC) With {.ToolTip = "Espandibile"})

            btnContactMaxItems.ToolTip = String.Format("Espandi {0}", TreeViewCaption)
            btnContactMaxItems.ImageUrl = ImagePath.SmallExpand
            btnContactMaxItems.Visible = True
            cmdDetails.Visible = False
            UpdateCount(countToShow)
            Exit Sub
        End If

        ' Contatti insufficenti ad abilitare la compressione
        BindData(False)
        UpdateCount(Count)

        btnContactMaxItems.Visible = False
        cmdDetails.Visible = True

    End Sub

    ''' <summary> Popola la treeview con l'elenco contatti specificato nel DataSource. </summary>
    ''' <param name="append">True: appende i contatti, False: aggiunge i contatti da una situazione pulita</param>
    Private Sub BindData(ByVal append As Boolean)
        If Not append Then
            RadTreeContact.Nodes(0).Nodes.Clear()
        End If

        If DataSource Is Nothing Then
            Exit Sub
        End If

        For Each contact As ContactDTO In DataSource
            Select Case contact.Type
                Case ContactDTO.ContactType.Address
                    AddItem(contact.Contact, contact.IsCopiaConoscenza, contact.IsLocked)
                Case ContactDTO.ContactType.Manual
                    Dim jsonManualContact As String = Nothing
                    If contact.IdManualContact IsNot Nothing Then
                        jsonManualContact = JsonConvert.SerializeObject(contact.IdManualContact)
                    End If
                    contact.Contact = Data.Contact.EscapingJSON(contact.Contact, Function(f) HttpUtility.HtmlDecode(f))
                    SetManualContact(
                        contact.Contact,
                        JsonConvert.SerializeObject(contact.Contact.Duplicate()),
                        contact.Contact.ContactType.Id,
                        jsonManualContact,
                        contact.IsCopiaConoscenza, contact.IsLocked)
            End Select
            ' Seleziono il primo contatto
            If RadTreeContact.Nodes(0).Nodes.Count > 0 Then
                RadTreeContact.Nodes(0).Nodes(0).Selected = True
            End If
        Next

    End Sub

    ''' <summary> Aggiorna il numero di contatti inseriti. </summary>
    Private Sub UpdateCount()
        UpdateCount(Count)
    End Sub

    ''' <summary> Aggiorna l'etichetta del numero di contatti. </summary>
    ''' <param name="contactCount">Nuovo numero di contatti</param>
    Private Sub UpdateCount(ByVal contactCount As Integer)
        If Not EnableCount Then
            Exit Sub
        End If

        If contactCount > 0 Then
            lblCount.Text = String.Format("({0})", contactCount)
            lblCount.ToolTip = String.Format("{0} {1}", contactCount, If(contactCount = 1, "contatto", "contatti"))
        Else
            lblCount.Text = ""
            lblCount.ToolTip = ""
        End If
    End Sub

    ''' <summary> Selezione di tutti i nodi. </summary>
    ''' <remarks>
    ''' TODO: capire come mai non viene usato <see cref="RadTreeView.CheckAllNodes"/>
    ''' </remarks>
    Public Overloads Sub CheckAllNodes()
        RadTreeContact.CheckAllNodes()
        CheckAllNodes(RadTreeContact.Nodes)
    End Sub

    Public Overloads Sub CheckAllNodes(ByVal nodesToCheck As RadTreeNodeCollection)
        For Each tn As RadTreeNode In nodesToCheck
            tn.Checked = True
            If tn.Nodes.Count > 0 Then
                CheckAllNodes(tn.Nodes)
            End If
        Next
    End Sub

    ''' <summary> Nasconde i bottoni per l'aggiunta manuale dei contatti. </summary>
    ''' <remarks> Se il pannello che contiene i bottoni è nascosto, viene prima reso visibile. </remarks>
    Public Sub HideManualButtons()
        If Not panelButtons.Visible Then
            panelButtons.Visible = True
        End If
        btnAddManual.Visible = False
        btnAddMyself.Visible = False
        btnAddManualMulti.Visible = False
        cmdDetails.Visible = False
        btnImportContact.Visible = False
        btnImportContactManual.Visible = False
    End Sub

    Public Property OnlyContact() As Boolean
        Get
            Return panelOnlyContact.Visible
        End Get
        Set(value As Boolean)
            panelOnlyContact.Visible = value
            panelButtons.Visible = Not value
        End Set
    End Property

    Public Function AddCollaborationContact(ByVal contactType As Char, ByVal description As String, ByVal emailAddress As String, ByVal account As String, ByVal note As String, ByVal roleUserIdRole As String, ByVal checkable As Boolean, Optional ByVal checked As Boolean = False, Optional bold As Boolean = False, Optional order As Boolean = False, Optional fromTemplate As Boolean = False) As RadTreeNode
        'creo il contatto
        Dim contact As New Contact()
        With contact
            .Description = description
            .EmailAddress = emailAddress
            .Code = account
            .Note = note
            .RoleUserIdRole = roleUserIdRole
            .ContactType = New ContactType(contactType)
        End With
        'aggiungo il nodo
        Dim tn As New RadTreeNode
        tn.Text = description
        tn.ImageUrl = ImagePath.ContactTypeIcon(contactType)
        tn.Checkable = checkable
        tn.Checked = checked
        tn.Font.Bold = bold
        ' Inserisco il contatto serializzato
        tn.Attributes.Add(ManualContactAttribute, JsonConvert.SerializeObject(contact))
        tn.Attributes.Add("ContactPart", CONTACT_ROLEUSER)
        If fromTemplate Then
            tn.Attributes.Add(FromTemplateAttribute, "true")
        End If
        RadTreeContact.Nodes(0).Nodes.Add(tn)
        Return tn
    End Function

    Public Sub Enable()
        pnlIntestazione.Enabled = True
        IsEnable = True
        TreeViewUtils.ChangeNodesForeColor(RadTreeContact.Nodes(0), Color.Empty)
    End Sub

    Public Sub Disable()
        pnlIntestazione.Enabled = False
        IsEnable = False
        TreeViewUtils.ChangeNodesForeColor(RadTreeContact.Nodes(0), Color.Gray)
    End Sub

    Public Sub OpenDoubleWindows(ByVal contactIDs As String)
        Dim url As String = "../UserControl/CommonSelContactDouble.aspx?ContactId=" & contactIDs
        AjaxManager.ResponseScripts.Add(String.Format(DefaultOpenWindowScript, ID, url, "windowSelContactDouble", "_CloseFunction"))
    End Sub

    Private Function AddManualCode() As String

        Dim url As New StringBuilder("../UserControl/CommonSelContactManual.aspx?")
        url.AppendFormat("Action=Ins&ParentID={0}", ID)
        If ProtType Then
            url.Append("&ProtType=true")
        End If
        If IsFiscalCodeRequired Then
            url.Append("&FCRequired=true")
        End If
        If SimpleMode Then
            url.Append("&SimpleMode=true")
        End If

        Return String.Format(DefaultOpenWindowScript, ID, url.ToString(), If(SimpleMode, "windowSelContactManualSimpleMode", "windowSelContactManual"), "_CloseManualFunction")
    End Function
    Private Sub InitializeContactRest()
        If ButtonContactSmartVisible Then
            Dim contactSmartQs As StringBuilder = New StringBuilder()
            contactSmartQs.Append($"ParentID={ID}")
            contactSmartQs.Append($"&AVCPBusinessContactEnabled={AVCPBusinessContactEnabled}")
            If ContactRoot.HasValue Then
                contactSmartQs.Append($"&FilterByParentId={ContactRoot.Value.ToString()}")
            End If

            btnContactSmart.OnClientClick = String.Format(OpenWindowSmartScript, ID, $"../UserControl/CommonSelContactRest.aspx?{contactSmartQs.ToString()}", "_CloseSmart")
            ButtonManualVisible = False
            ButtonManualMultiVisible = False
            ButtonSelectVisible = False
        End If
    End Sub

    Private Function AddSelContactCode() As String
        Dim url As StringBuilder
        Dim callBack As String
        If Not ForceAddressBook AndAlso UseAD Then
            url = New StringBuilder("../UserControl/CommonSelContactDomain.aspx?")
            url.AppendFormat("Type={0}", BasePage.Type)
            url.Append("&")
            url.AppendFormat("ParentID={0}", ID)

            callBack = "_CloseDomain"
        Else
            url = New StringBuilder("../UserControl/CommonSelContactRubrica.aspx?")
            url.AppendFormat("ParentID={0}", ID)
            url.AppendFormat("&AVCPBusinessContactEnabled={0}", AVCPBusinessContactEnabled)
            url.AppendFormat("&FascicleContactEnabled={0}", FascicleContactEnabled)
            If Not String.IsNullOrEmpty(BasePage.Type) Then
                url.AppendFormat("&Type={0}", BasePage.Type)
            End If
            If Not String.IsNullOrEmpty(Action) Then
                url.AppendFormat("&Action={0}", Action)
            End If
            If MultiSelect Then
                url.Append("&MultiSelect=true")
            End If
            If ProtType Then
                url.Append("&ProtType=true")
            End If
            If Not String.IsNullOrEmpty(DefaultDescription) Then
                url.AppendFormat("&DefaultDescr={0}", DefaultDescription)
            End If
            If ContactRoot.HasValue Then
                url.AppendFormat("&ContactRoot={0}", ContactRoot.Value.ToString())
            End If
            If ExcludeContacts IsNot Nothing AndAlso ExcludeContacts.Count() > 0 Then
                url.AppendFormat("&ExcludeContact={0}", HttpUtility.UrlEncode(JsonConvert.SerializeObject(ExcludeContacts)))
                url.AppendFormat("&ExcludeParentId={0}", HttpUtility.UrlEncode(JsonConvert.SerializeObject(ExcludeContacts)))
            End If
            If ExcludeRoleRoot.HasValue Then
                url.Append(String.Concat("&ExcludeRoleRoot=", ExcludeRoleRoot.Value.ToString()))
            End If
            If SearchAll Then
                url.Append("&ShowAll=True")
            Else
                url.Append("&ShowAll=False")
            End If

            If EnableFlagGroupChild Then
                url.Append("&EnableFlagGroupChild=True")
            End If

            If Not String.IsNullOrEmpty(CategoryContactsProcedureType) Then
                url.Append(String.Concat("&CategoryContactsProcedureType=", CategoryContactsProcedureType))
            End If

            If SearchInCategoryContacts.HasValue() Then
                url.Append(String.Concat("&SearchInCategoryContacts=", SearchInCategoryContacts.Value))
            End If
            If SearchInRoleContacts.HasValue() Then
                url.Append(String.Concat("&SearchInRoleContacts=", SearchInRoleContacts.Value))
            End If

            If Not String.IsNullOrEmpty(RoleContactsProcedureType) Then
                url.Append(String.Concat("&RoleContactsProcedureType=", RoleContactsProcedureType))
            End If

            callBack = "_CloseFunction"
        End If

        Return String.Format(FullSizeOpenWindowScript, ID, url.ToString(), "windowSelContact", callBack)
    End Function

    Private Function AddIpaCode() As String
        Dim url As New StringBuilder("../Comm/SelContattiIPA.aspx?")
        url.AppendFormat("ParentID={0}", ID)
        If ProtType Then
            url.Append("&ProtType=true")
        End If
        If btnAddManual.Visible = False Then
            url.Append("&OnlyManualDetail=true")
        End If

        Return String.Format(DefaultOpenWindowScript, ID, url.ToString(), "windowSelContactIPA", "_CloseIPAFunction")
    End Function

    ''' <summary> Script da chiamare nella pagina che ospita il controllo per aprire la finestra di scelta contatto. </summary>
    ''' <param name="description"> Inserire se si vuole precaricare il campo di ricerca descrizione. </param>
    <Obsolete("Veramente non c'è un'altro modo per scatenare l'evento? togliere appena si ha tempo")>
    Public Function GetOpenContactWindowScript(description As String) As String
        DefaultDescription = description
        ' lancio l'apertura della finestra
        Dim script As String = AddSelContactCode()
        DefaultDescription = ""
        Return script
    End Function

    Public Sub UpdateButtons()
        btnSelContact.OnClientClick = AddSelContactCode()
        btnSelContact2.OnClientClick = AddSelContactCode()
        InitializeContactRest()
        InitializeButtonSelContactOChart()
    End Sub

    Private Sub InitializeButtonSelContactOChart()
        If Not DocSuiteContext.Current.ProtocolEnv.OChartEnabled Then
            Return
        End If

        Dim url As String = String.Format("../UserControl/CommonSelContactOChart.aspx?Type={0}&ParentID={1}", BasePage.Type, ID)
        If APIDefaultProvider.HasValue Then
            url = String.Format("{0}&APIDefaultProvider={1}", url, APIDefaultProvider.Value)
        End If

        ButtonSelContactOChart.OnClientClick = String.Format(DefaultOpenWindowScript, ID, url, "windowSelContact", "_CloseOChart")
    End Sub

    Public Sub UpdateRoleUserType()
        'Selezione contatti per collaborazione
        Dim params As StringBuilder = New StringBuilder()
        params.Append("Titolo=Autorizzazioni&Action=SelResp")
        If Not String.IsNullOrEmpty(CollaborationType) Then
            params.AppendFormat("&CollaborationType={0}", CollaborationType)
        End If
        If Not String.IsNullOrEmpty(EnvironmentType) Then
            params.AppendFormat("&Type={0}", EnvironmentType)
        End If
        btnRoleUser.OnClientClick = String.Format(DefaultOpenWindowScript, ID, "../User/UserSelRoleUser.aspx?" & CommonShared.AppendSecurityCheck(params.ToString()), "windowSelRoleUser", "_CloseRole")
    End Sub

    Public Function EncodeContact(nodeAttributes As String) As String
        Dim contact As Contact = JsonConvert.DeserializeObject(Of Contact)(nodeAttributes)
        contact = contact.EscapingJSON(contact, Function(f) HttpUtility.UrlEncode(f))
        Dim contactJSon As String = JsonConvert.SerializeObject(contact).Replace("'", "\'").Replace("\", "")
        Return HttpUtility.UrlEncode(contactJSon)
    End Function

    Public Sub RemoveExcludeContact(contact As Integer)
        If (ExcludeContacts.Contains(contact)) Then
            ExcludeContacts.Remove(contact)
        End If
    End Sub

    Public Sub AddExcludeContact(contact As Integer)
        If (Not ExcludeContacts.Contains(contact)) Then
            ExcludeContacts.Add(contact)
        End If
    End Sub
#End Region

End Class