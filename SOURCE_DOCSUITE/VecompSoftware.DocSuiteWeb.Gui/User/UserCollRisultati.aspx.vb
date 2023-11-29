Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Linq
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports Telerik.Web.UI.Calendar
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Collaborations
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Templates
Imports VecompSoftware.DocSuiteWeb.DTO.Collaborations
Imports VecompSoftware.DocSuiteWeb.DTO.SignDocuments
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.DTO.Workflows
Imports VecompSoftware.DocSuiteWeb.Entity.Templates
Imports VecompSoftware.DocSuiteWeb.Entity.Workflows
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Collaborations
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations
Imports VecompSoftware.DocSuiteWeb.Model.Parameters
Imports VecompSoftware.DocSuiteWeb.Model.Workflow
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.WebAPI
Imports VecompSoftware.Helpers.Workflow
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging
Imports WebAPIFacade = VecompSoftware.DocSuiteWeb.Facade.WebAPI.Collaborations
Imports WebAPIFinder = VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Collaborations

Partial Public Class UserCollRisultati
    Inherits UserBasePage
    Implements ISignMultipleDocuments

#Region " Fields "

    Public Const CollaborationIdentifierName As String = "IdCollaboration"
    Private _collaborationsToSign As New List(Of Collaboration)
    Private _uoiaCollaborations As New List(Of Collaboration)
    Private Const OPEN_AUTH_WINDOW As String = "return OpenAuthorizeWindow('{0}');"
    Private Const PROTOCOL_GIF_URL As String = "~/Comm/Images/DocSuite/Protocollo16.png"
    Private Const DEL_GIF_URL As String = "~/Comm/Images/DocSuite/Delibera16.png"
    Private Const ATTO_GIF_URL As String = "~/Comm/images/Docsuite/Atto16.png"
    Private _realTitle As String
    Private _currentTemplateCollaborationFinder As TemplateCollaborationFinder
    Private _currentSigner As CollaborationSign

#End Region

#Region " Properties "

    Public ReadOnly Property TitleStep As String
        Get
            If String.IsNullOrEmpty(_realTitle) Then
                _realTitle = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of String)("Title", "")
            End If
            Return _realTitle
        End Get
    End Property

    Public Property SessionList() As List(Of Collaboration)
        Get
            Return _uoiaCollaborations
        End Get
        Set(ByVal value As List(Of Collaboration))
            For Each Uoiacollaboration As Collaboration In value
                _uoiaCollaborations.Add(Uoiacollaboration)
            Next
            Session("SessionCollaborationUoia") = _uoiaCollaborations
        End Set
    End Property

    Private Property CurrentDocumentType() As String
        Get
            Return CType(Session("DocumentType"), String)
        End Get
        Set(value As String)
            Session("DocumentType") = value
        End Set
    End Property

    Private Property FromDate() As DateTime
        Get
            If Session("ProtDate_From") Is Nothing Then
                Session("ProtDate_From") = DateTime.Now.AddDays(-7)
            End If
            Return DirectCast(Session("ProtDate_From"), DateTime)
        End Get
        Set(value As DateTime)
            Session("ProtDate_From") = value
        End Set
    End Property

    Private Property ToDate() As DateTime
        Get
            If Session("ProtDate_To") Is Nothing Then
                Session("ProtDate_To") = DateTime.Now
            End If
            Return DirectCast(Session("ProtDate_To"), DateTime)
        End Get
        Set(value As DateTime)
            Session("ProtDate_To") = value
        End Set
    End Property

    Public ReadOnly Property DocumentsToSign() As IList(Of MultiSignDocumentInfo) Implements ISignMultipleDocuments.DocumentsToSign
        Get
            Dim list As New List(Of MultiSignDocumentInfo)

            'TODO: Da rimuovere il filtro sul DocumentType = W. Anche in firma multipla deve essere possibile gestire collaborazioni di tipo Workflow
            Dim collaborationSigns As ICollection(Of String)
            Dim coll As Collaboration
            Dim dictionary As IDictionary(Of Guid, BiblosDocumentInfo)
            Dim isSignRequired As Boolean
            Dim loadAlsoOmissis As Boolean
            Dim effectiveSigner As String = String.Empty
            Dim listDelegations As List(Of String) = Facade.UserLogFacade.GetDelegationsSign()
            For Each collResult As CollaborationResult In SelectedCollaborations
                collaborationSigns = Facade.CollaborationSignsFacade.GetEffectiveSigners(collResult.IdCollaboration).Select(Function(s) s.SignUser).ToList()
                coll = Facade.CollaborationFacade.GetById(collResult.IdCollaboration)
                dictionary = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(coll, VersioningDocumentGroup.MainDocument)
                isSignRequired = True
                If ProtocolEnv.CollaborationFilterEnabled Then
                    isSignRequired = Not Action.Eq(CollaborationMainAction.DaVisionareFirmare)
                    Dim requiredSign As IList(Of CollaborationSign) = coll.GetRequiredSigns()
                    Select Case Action
                        Case CollaborationMainAction.DaFirmareInDelega
                            Dim collaborationSign As CollaborationSign = coll.CollaborationSigns.Where(Function(x) x.IsActive).FirstOrDefault()
                            If listDelegations.Any(Function(x) x.Eq(collaborationSign.SignUser)) Then
                                effectiveSigner = collaborationSign.SignUser
                            End If
                            isSignRequired = requiredSign.Any(Function(x) x.SignUser.Eq(collaborationSign.SignUser))
                        Case Else
                            If Not requiredSign.IsNullOrEmpty() Then
                                isSignRequired = requiredSign.Any(Function(x) x.SignUser.Eq(DocSuiteContext.Current.User.FullUserName))
                            End If
                    End Select
                End If

                If Not dictionary.IsNullOrEmpty() Then
                    For Each key As Guid In dictionary.Keys
                        Dim msdi As New MultiSignDocumentInfo(dictionary(key))
                        msdi.GroupCode = coll.Id.ToString()
                        msdi.Mandatory = isSignRequired OrElse ProtocolEnv.CollaborationMultiSignDocumentsDefaultFlagged
                        msdi.MandatorySelectable = Not isSignRequired
                        msdi.DocType = "Doc. Principale"
                        msdi.Description = coll.CollaborationObject
                        msdi.IdOwner = key.ToString().Replace("/"c, "§"c)
                        msdi.Signers = collaborationSigns
                        msdi.EffectiveSigner = effectiveSigner
                        list.Add(msdi)
                    Next
                End If

                loadAlsoOmissis = Not String.IsNullOrEmpty(coll.DocumentType) AndAlso (DocSuiteContext.Current.IsResolutionEnabled AndAlso (coll.DocumentType.Eq(CollaborationDocumentType.D.ToString()) OrElse coll.DocumentType.Eq(CollaborationDocumentType.A.ToString())))

                If loadAlsoOmissis AndAlso ResolutionEnv.MainDocumentOmissisEnable Then
                    dictionary = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(coll, VersioningDocumentGroup.MainDocumentOmissis)

                    If Not dictionary.IsNullOrEmpty() Then

                        For Each key As Guid In dictionary.Keys
                            Dim msdi As New MultiSignDocumentInfo(dictionary(key))
                            msdi.GroupCode = coll.Id.ToString()
                            msdi.Mandatory = ProtocolEnv.CollaborationMultiSignDocumentsDefaultFlagged
                            msdi.MandatorySelectable = True
                            msdi.Description = coll.CollaborationObject
                            msdi.DocType = "Doc. Omissis"
                            msdi.IdOwner = key.ToString().Replace("/"c, "§"c)
                            msdi.Signers = collaborationSigns
                            msdi.EffectiveSigner = effectiveSigner
                            list.Add(msdi)
                        Next
                    End If
                End If

                dictionary = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(coll, VersioningDocumentGroup.Attachment)

                If Not dictionary.IsNullOrEmpty() Then

                    For Each key As Guid In dictionary.Keys
                        Dim msdi As New MultiSignDocumentInfo(dictionary(key))
                        msdi.GroupCode = coll.Id.ToString()
                        msdi.Mandatory = ProtocolEnv.CollaborationMultiSignDocumentsDefaultFlagged
                        msdi.MandatorySelectable = True
                        msdi.Description = coll.CollaborationObject
                        msdi.DocType = "Allegato"
                        msdi.IdOwner = key.ToString().Replace("/"c, "§"c)
                        msdi.Signers = collaborationSigns
                        msdi.EffectiveSigner = effectiveSigner
                        list.Add(msdi)
                    Next
                End If

                If loadAlsoOmissis AndAlso ResolutionEnv.AttachmentOmissisEnable Then
                    dictionary = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(coll, VersioningDocumentGroup.AttachmentOmissis)

                    If Not dictionary.IsNullOrEmpty() Then
                        For Each key As Guid In dictionary.Keys

                            Dim msdi As New MultiSignDocumentInfo(dictionary(key))
                            msdi.GroupCode = coll.Id.ToString()
                            msdi.Mandatory = ProtocolEnv.CollaborationMultiSignDocumentsDefaultFlagged
                            msdi.MandatorySelectable = True
                            msdi.Description = coll.CollaborationObject
                            msdi.DocType = "Allegato Omissis"
                            msdi.IdOwner = key.ToString().Replace("/"c, "§"c)
                            msdi.Signers = collaborationSigns
                            msdi.EffectiveSigner = effectiveSigner
                            list.Add(msdi)
                        Next
                    End If
                End If

                dictionary = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(coll, VersioningDocumentGroup.Annexed)

                If Not dictionary.IsNullOrEmpty() Then
                    For Each key As Guid In dictionary.Keys

                        Dim msdi As New MultiSignDocumentInfo(dictionary(key))
                        msdi.GroupCode = coll.Id.ToString()
                        msdi.Mandatory = ProtocolEnv.CollaborationMultiSignDocumentsDefaultFlagged
                        msdi.MandatorySelectable = True
                        msdi.Description = coll.CollaborationObject
                        msdi.DocType = "Annesso"
                        msdi.IdOwner = key.ToString().Replace("/"c, "§"c)
                        msdi.Signers = collaborationSigns
                        msdi.EffectiveSigner = effectiveSigner
                        list.Add(msdi)
                    Next
                End If

            Next

            Return list
        End Get
    End Property

    Public ReadOnly Property ReturnUrl() As String Implements ISignMultipleDocuments.ReturnUrl
        Get
            Return String.Format("~/User/UserCollRisultati.aspx?Type={0}&Titolo=Inserimento&Action={1}", Type, Action)
        End Get
    End Property
    Public ReadOnly Property SignAction As String Implements ISignMultipleDocuments.SignAction
        Get
            Return String.Format(Action)
        End Get
    End Property
    Private ReadOnly Property SelectedCollaborations As IList(Of CollaborationResult)
        Get
            Dim selectedItems As IEnumerable(Of GridDataItem) = uscCollaborationGrid.Grid.Items.Cast(Of GridDataItem)().Where(Function(x) DirectCast(x.FindControl("cbSelect"), CheckBox).Checked)
            Return selectedItems.Select(Function(s) New CollaborationResult() With {
                                                .IdCollaboration = Convert.ToInt32(s.GetDataKeyValue("Entity.IdCollaboration")),
                                                .DocumentType = (s(uscCollGrid.COLUMN_ENTITY_DOCUMENT_TYPE).Text)
                                            }).ToList()
        End Get
    End Property

    Private ReadOnly Property SelectedCollaborationIds As IList(Of Integer)
        Get
            Return SelectedCollaborations.Select(Function(s) s.IdCollaboration).ToList()
        End Get
    End Property

    Private ReadOnly Property SelectedFilterType As CollaborationFinderFilterType
        Get
            Dim filterChecked As RadButton = filters.Controls.OfType(Of RadButton)().SingleOrDefault(Function(x) x.Checked AndAlso x.GroupName.Eq("FilterButton"))
            If filterChecked IsNot Nothing Then
                Return CType([Enum].Parse(GetType(CollaborationFinderFilterType), filterChecked.Attributes("filterType").ToString()), CollaborationFinderFilterType)
            End If
            Return CollaborationFinderFilterType.AllCollaborations
        End Get
    End Property

    Private Property SelectedCollaborationsToSign As List(Of Collaboration)
        Get
            If _collaborationsToSign.Any() Then
                Return _collaborationsToSign
            End If

            Dim sessionObj As Object = Session("collaborationsToSigns")
            If sessionObj IsNot Nothing Then
                _collaborationsToSign = DirectCast(sessionObj, List(Of Collaboration))
            End If
            Return _collaborationsToSign
        End Get
        Set(value As List(Of Collaboration))
            _collaborationsToSign = value
            Session("collaborationsToSigns") = value
        End Set
    End Property

    Private Property SignedComplete As Boolean
        Get
            Dim sessionParam As Object = Session("signedComplete")
            If sessionParam IsNot Nothing Then
                Return DirectCast(sessionParam, Boolean)
            End If
            Return False
        End Get
        Set(value As Boolean)
            If value = Nothing Then
                Session.Remove("signedComplete")
            Else
                Session("signedComplete") = value
            End If
        End Set
    End Property


    Private ReadOnly Property CurrentTemplateCollaborationFinder As TemplateCollaborationFinder
        Get
            If _currentTemplateCollaborationFinder Is Nothing Then
                _currentTemplateCollaborationFinder = New TemplateCollaborationFinder(DocSuiteContext.Current.Tenants)
                _currentTemplateCollaborationFinder.ResetDecoration()
                _currentTemplateCollaborationFinder.EnablePaging = False
            End If
            Return _currentTemplateCollaborationFinder
        End Get
    End Property

    Public ReadOnly Property HasDgrooveSigner As Boolean
        Get
            Return DocSuiteContext.Current.HasDgrooveSigner
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            InitializePage()
            Initialize()
        End If
    End Sub

    Protected Sub UserCollRisultatiAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arguments As String() = Split(e.Argument.Replace("~", "'"), "|", 2)
        Select Case arguments(0)
            Case "InitialPageLoad"
                InitializeFinder()
                InitializeFromSign()
                btnChangeSigner.Enabled = True
                If ProtocolEnv.CollaborationRightsEnabled Then
                    btnChangeSigner.Enabled = Not String.IsNullOrEmpty(ddlDocType.SelectedValue)
                End If
                CheckAuthorization()
                If btnAbsence.Visible Then
                    btnAbsence.Enabled = Not String.IsNullOrEmpty(ddlDocType.SelectedValue) AndAlso ddlDocType.SelectedValue = CollaborationDocumentType.D.ToString()
                End If

                AjaxManager.ResponseScripts.Add("resizeGrid();")

            Case "UPDATE"
                Response.Redirect(Request.RawUrl)
            Case "UpdateData"
                InitializeFinder()
            Case "CHANGESIGNER"
                ExecuteChangeSigner(arguments(1))
            Case "NEXT"
                ExecuteNext()
            Case "DELETECOLLABORATION"
                DeleteCollaboration(CType(arguments(1), Integer))
            Case "SIGNNEXT"
                BtnSignAndNext_Click(sender, e)
            Case "SIGN"
            Case "AUTOMATICNEXT"
                BtnAutomaticNextConfirm_Click(sender, e)
            Case "ABSENTMANAGERS"
                If arguments(1) IsNot Nothing Then
                    Dim deserialized As AbsentManager() = JsonConvert.DeserializeObject(Of AbsentManager())(arguments(1))
                    SetManagersAbsence(deserialized)
                End If
        End Select
    End Sub

    Private Sub uscCollaborationGrid_OnRefresh(ByVal sender As Object, ByVal e As CollaborationEventArgs) Handles uscCollaborationGrid.OnRefresh
        UpdateActionCp()
    End Sub

    Private Sub CheckAuthorization()
        Dim environment As DSWEnvironment = DSWEnvironment.Any
        Select Case CurrentDocumentType
            Case CollaborationDocumentType.P.ToString(), CollaborationDocumentType.U.ToString()
                environment = DSWEnvironment.Protocol
                Exit Select
            Case CollaborationDocumentType.A.ToString(), CollaborationDocumentType.D.ToString()
                environment = DSWEnvironment.Resolution
                Exit Select
            Case CollaborationDocumentType.S.ToString(), CollaborationDocumentType.UDS.ToString()
                environment = DSWEnvironment.DocumentSeries
                Exit Select
        End Select
        Dim udsEnv As Integer = -1
        If Integer.TryParse(CurrentDocumentType, udsEnv) AndAlso udsEnv > 99 Then
            environment = DSWEnvironment.DocumentSeries
        End If

        Dim secretary As IList(Of Role) = New List(Of Role)

        If ProtocolEnv.CollaborationRightsEnabled AndAlso Not String.IsNullOrEmpty(ddlDocType.SelectedValue) Then
            secretary = Facade.RoleUserFacade.GetSecretaryRolesByAccount(DocSuiteContext.Current.User.FullUserName, environment, CurrentTenant.TenantAOO.UniqueId)

        ElseIf Not ProtocolEnv.CollaborationRightsEnabled Then
            secretary = Facade.RoleUserFacade.GetSecretaryRolesByAccount(DocSuiteContext.Current.User.FullUserName, Nothing, CurrentTenant.TenantAOO.UniqueId)
        End If
        If Not secretary.Any() Then
            btnChangeSigner.Enabled = False
            btnChangeSigner.ToolTip = "Non si dispongono dei diritti necessari per la tipologia di documento selezionato."
        End If

    End Sub
    Private Sub uscCollaborationGrid_OnSelectedCollaboration(ByVal sender As Object, ByVal e As CollaborationEventArgs) Handles uscCollaborationGrid.OnSelectedCollaboration
        Dim mainAction As String = String.Empty
        Select Case Action
            Case CollaborationMainAction.AllaVisioneFirma
                mainAction = CollaborationSubAction.AllaVisioneFirma

            Case CollaborationMainAction.DaVisionareFirmare
                mainAction = CollaborationSubAction.DaVisionareFirmare

            Case CollaborationMainAction.DaFirmareInDelega
                mainAction = CollaborationSubAction.DaFirmareInDelega

            Case CollaborationMainAction.AlProtocolloSegreteria
                mainAction = CollaborationSubAction.AlProtocolloSegreteria

            Case CollaborationMainAction.AttivitaInCorso
                mainAction = CollaborationSubAction.AttivitaInCorso

            Case CollaborationMainAction.DaProtocollareGestire
                mainAction = CollaborationSubAction.DaProtocollareGestire

            Case CollaborationMainAction.ProtocollatiGestiti
                mainAction = CollaborationSubAction.ProtocollatiGestiti

            Case CollaborationMainAction.MieiCheckOut
                mainAction = CollaborationSubAction.DaVisionareFirmare

        End Select

        Dim url As String = String.Format("~/User/UserCollGestione.aspx?Type={0}&Titolo=Inserimento&Action={1}&idCollaboration={2}&Action2={3}&Title2={4}",
                                        CollaborationFacade.GetPageTypeFromDocumentType(e.DocumentType), mainAction, e.IdCollaboration, Action, TitleStep)

        Dim collaborationSelected As Collaboration = Facade.CollaborationFacade.GetById(e.IdCollaboration)

        Dim message As String = String.Format("Visualizzata la collaborazione numero"" [{0}]", collaborationSelected.Id.ToString("N"))
        Facade.CollaborationLogFacade.Insert(collaborationSelected, Nothing, Nothing, Nothing, CollaborationLogType.CV, message)

        If collaborationSelected Is Nothing Then
            AjaxAlert(String.Format("Collaborazione [{0}] non trovata.", e.IdCollaboration))
            Exit Sub
        End If

        If ProtocolEnv.DeleteCollaborationsIfNoDocuments Then
            Dim versioning As List(Of CollaborationVersioning) = collaborationSelected.GetDocumentVersioning()
            If versioning Is Nothing OrElse Not versioning.Any() Then
                AjaxManager.ResponseScripts.Add(String.Format("ConfirmDeleteCollaboration({0});", e.IdCollaboration))
                Exit Sub
            End If
        End If

        Response.Redirect(ResolveUrl(url))
    End Sub

    Protected Sub NextClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnNext.Click
        Dim selectedIds As IList(Of Integer) = SelectedCollaborationIds
        Dim toRemove As New List(Of Integer)

        If selectedIds.IsNullOrEmpty Then
            AjaxAlert("Nessuna collaborazione selezionata.")
            Exit Sub
        End If

        For Each item As Collaboration In From idItem In selectedIds Select Facade.CollaborationFacade.GetById(idItem)
            If HasCheckOut(item) Then
                toRemove.Add(item.Id)
                Continue For
            End If
            If Not DocSuiteContext.Current.ProtocolEnv.ForceProsecutable AndAlso Not item.Prosecutable Then
                AjaxAlert("I documenti selezionati non risultano tutti firmati.{0}'{1}' deve essere firmato.", Environment.NewLine, item.CollaborationObject)
                Exit Sub
            End If

        Next

        selectedIds = selectedIds.Where(Function(f) Not toRemove.Any(Function(c) c = f)).ToList()
        SelectGridItems(SelectedCollaborationIds.Where(Function(f) Not toRemove.Any(Function(c) c = f)).ToList())

        If Not Facade.CollaborationVersioningFacade.CheckUserDocumentsSign(selectedIds.ToArray()) Then
            Dim confirmMessage As String = "I documenti selezionati non risultano tutti firmati. Si desidera proseguire comunque?"
            If DocSuiteContext.Current.ProtocolEnv.ForceProsecutable Then
                confirmMessage &= "\nQuesta operazione rimuoverà eventuali obbligatorietà di firma."
            End If

            AjaxAlertConfirm(confirmMessage, "ExecuteAjaxRequest('NEXT');", Nothing)
        Else
            AjaxManager.ResponseScripts.Add("ExecuteAjaxRequest('NEXT');")
        End If

    End Sub

    Protected Sub InsertClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnInsert.Click
        Dim insertAction As String = String.Empty
        Select Case Action
            Case CollaborationMainAction.AllaVisioneFirma
                insertAction = "Add"
            Case CollaborationMainAction.AlProtocolloSegreteria
                insertAction = "Apt"
        End Select

        If ddlDocType.SelectedValue.Eq(CollaborationDocumentType.W.ToString()) Then
            AjaxAlert("Non è possibile inserire una collaborazione di tipologia flusso di lavoro")
            Exit Sub
        End If

        Response.Redirect(String.Format("UserCollGestione.aspx?Titolo=Inserimento&Action={3}&Title2={0}&Action2={1}&Type=Prot&docType={2}", Request.QueryString("Title"), Action, ddlDocType.SelectedValue, insertAction))
    End Sub


    Protected Sub UpdateClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnUpdate.Click
        UpdateActionCp()
    End Sub

    Protected Sub DocumentTypeSelectedChange(ByVal sender As Object, ByVal e As EventArgs) Handles ddlDocType.SelectedIndexChanged
        CurrentDocumentType = ddlDocType.SelectedValue
        btnUoia.Enabled = (ddlDocType.SelectedValue.Equals("U") OrElse ddlDocType.SelectedValue.Equals(""))
        btnChangeSigner.Enabled = True
        If ProtocolEnv.CollaborationRightsEnabled Then
            btnChangeSigner.Enabled = Not String.IsNullOrEmpty(ddlDocType.SelectedValue)
        End If
        If btnAbsence.Visible Then
            btnAbsence.Enabled = Not String.IsNullOrEmpty(ddlDocType.SelectedValue) AndAlso ddlDocType.SelectedValue = CollaborationDocumentType.D.ToString()
        End If
        CheckAuthorization()
        InitializeFinder()
    End Sub

    Protected Sub FilterSelectedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles allCollaborations.CheckedChanged, activeCollaborations.CheckedChanged, pastCollaborations.CheckedChanged, signRequired.CheckedChanged, onlyVision.CheckedChanged
        If (Action = CollaborationMainAction.AttivitaInCorso OrElse Action = CollaborationMainAction.DaVisionareFirmare) AndAlso CType(sender, RadButton).Checked Then
            InitializeFinder()
        End If
    End Sub

    Private Sub cmdPreviewDocuments_Click(sender As Object, e As EventArgs) Handles cmdPreviewDocuments.Click
        Dim identifiers As IList(Of Integer) = SelectedCollaborationIds

        If identifiers.IsNullOrEmpty Then
            AjaxAlert("Nessuna collaborazione selezionata.")
            Exit Sub
        End If

        Dim serializedIds As String = Server.UrlEncode(JsonConvert.SerializeObject(identifiers))
        Dim queryString As String = CommonShared.AppendSecurityCheck(String.Format("DataSourceType=coll&ids={0}", serializedIds))
        Response.Redirect(String.Format("~/Viewers/CollaborationViewer.aspx?{0}", queryString))
    End Sub

    Private Sub cmdCollaborationVersioningManagement_Click(sender As Object, e As EventArgs) Handles cmdCollaborationVersioningManagement.Click
        Dim identifiers As IList(Of Integer) = SelectedCollaborationIds

        If identifiers.IsNullOrEmpty Then
            AjaxAlert("Nessuna collaborazione selezionata.")
            Exit Sub
        End If

        Dim backurl As New Uri(ConvertRelativeUrlToAbsoluteUrl(Page.Request.Url.OriginalString), UriKind.Absolute)
        Dim serializedIds As String = Server.UrlEncode(JsonConvert.SerializeObject(identifiers))
        Response.Redirect(String.Format("~/User/CollaborationVersioningManagement.aspx?ids={0}&backurl={1}", serializedIds, Server.UrlEncode(backurl.PathAndQuery)))
    End Sub

    ''' <summary>
    ''' metodo per la generazione della collaborazione unica Uoia
    ''' Il metodo crea una nuova collaborazione di procollo con 
    ''' 1- Destinazione il direttore del servizio UOIA e contenente: 
    ''' 2- Nel documento principale una lettera di accompagnamento con elenco dei verbali selezionati 
    ''' 3- In allegato i verbali selezionati
    ''' 4- Oggetto “Trasmissione Verbali UOIA azienda XXXX” 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub btnUoia_Click(sender As Object, e As EventArgs) Handles btnUoia.Click
        Dim collaborations As New List(Of Collaboration)
        Dim selectedIds As IList(Of Integer) = SelectedCollaborationIds

        If selectedIds.IsNullOrEmpty Then
            AjaxAlert("Nessuna collaborazione selezionata.")
            Exit Sub
        End If

        'Controllo se non sono tutti firmati
        If Not Facade.CollaborationVersioningFacade.CheckUserDocumentsSign(selectedIds.ToArray()) Then
            AjaxAlert("I documenti selezionati non risultano tutti firmati.")
            Exit Sub
        End If

        collaborations.AddRange(selectedIds.Select(Function(s) Facade.CollaborationFacade.GetById(s)))

        If collaborations.IsNullOrEmpty() Then
            AjaxAlert("Nessuna collaborazione selezionata.")
            Exit Sub
        End If

        If collaborations.Any(Function(x) Not x.DocumentType.Eq(CollaborationDocumentType.U.ToString())) Then
            AjaxAlert("Le collaborazioni selezionate devono essere tutte di tipo UOIA.")
            Exit Sub
        End If

        For Each coll As Collaboration In collaborations
            If HasCheckOut(coll) Then
                Exit Sub
            End If
        Next

        SessionList = collaborations

        Response.Redirect("../User/UserCollGestione.aspx?Titolo=Inserimento&Action=Add&Title2=Alla Visione/Firma&Action2=CI&Type=Prot&Uoia=True")

    End Sub

    Private Sub btnSelectAll_Click(sender As Object, e As EventArgs) Handles btnSelectAll.Click
        AllGridItemSelection(True)
    End Sub

    Private Sub btnDeselectAll_Click(sender As Object, e As EventArgs) Handles btnDeselectAll.Click
        AllGridItemSelection(False)
    End Sub

    ''' <summary> Indica se la pagina è tornata dalla firma multipla senza operazioni </summary>
    Private ReadOnly Property BackFromMultiSign As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)(MultipleSign.MultiSignUndoQuerystring, False)
        End Get
    End Property

    Protected Sub BtnDgrooveSigns_Click(sender As Object, e As EventArgs) Handles btnDgrooveSigns.Click
        Dim selectedIds As IList(Of Integer) = SelectedCollaborationIds
        If selectedIds.IsNullOrEmpty Then
            AjaxAlert("Nessuna collaborazione selezionata.")
            Exit Sub
        End If

        Dim collaborationToSign As IList(Of CollaborationResult) = SelectedCollaborations.Where(Function(x) Not Facade.CollaborationSignsFacade.IsCollaborationSignedByActiveSigner(x.IdCollaboration)).ToList()

        If collaborationToSign.Count = 0 Then
            AjaxAlert("Tutti i documenti risultano già firmati dall'utente.")
            Exit Sub
        End If

        Dim documents As List(Of DocumentRootFolder) = GetDocumentToSign(collaborationToSign)
        Dim script As String = String.Format("SaveToSessionStorageAndRedirect('{0}');", HttpUtility.JavaScriptStringEncode(JsonConvert.SerializeObject(documents)))

        If collaborationToSign.Count < SelectedCollaborationIds.Count Then
            Dim confirmMessage As String = "Alcuni documenti risultano firmati dall\'utente e verrano scartati.\nSi desidera proseguire comunque?"
            AjaxAlertConfirm(confirmMessage, script, Nothing)
        Else
            AjaxManager.ResponseScripts.Add(script)
        End If
    End Sub

    Protected Sub BtnSignAndNext_Click(sender As Object, e As EventArgs)
        Dim collaborations As New List(Of Collaboration)
        For Each collId As Integer In SelectedCollaborations.Select(Function(s) s.IdCollaboration)
            Dim collaboration As Collaboration = Facade.CollaborationFacade.GetById(collId)
            collaborations.Add(collaboration)
        Next

        SelectedCollaborationsToSign = collaborations
    End Sub

    Protected Sub BtnAutomaticNextConfirm_Click(sender As Object, e As EventArgs)
        Dim selectedIds As IList(Of Integer) = SelectedCollaborationIds

        If selectedIds.IsNullOrEmpty Then
            AjaxAlert("Nessuna collaborazione selezionata.")
            Exit Sub
        End If

        For Each item As Collaboration In From idItem In selectedIds Select Facade.CollaborationFacade.GetById(idItem)
            If HasCheckOut(item) Then
                Exit Sub
            End If
            If Not DocSuiteContext.Current.ProtocolEnv.ForceProsecutable AndAlso Not item.Prosecutable Then
                AjaxAlert("I documenti selezionati non risultano tutti firmati.{0}'{1}' deve essere firmato.", Environment.NewLine, item.CollaborationObject)
                Exit Sub
            End If
        Next

        If DocSuiteContext.Current.ProtocolEnv.ForceProsecutable AndAlso Not Facade.CollaborationVersioningFacade.CheckUserDocumentsSign(selectedIds.ToArray()) Then
            Dim confirmMessage As String = "I documenti selezionati non risultano tutti firmati. Si desidera proseguire comunque?"
            If DocSuiteContext.Current.ProtocolEnv.ForceProsecutable Then
                confirmMessage &= "\nQuesta operazione rimuoverà eventuali obbligatorietà di firma."
            End If

            AjaxAlertConfirm(confirmMessage, "ExecuteAjaxRequest('NEXT');", Nothing)
        Else
            ExecuteNext()
        End If
    End Sub

    Private Sub grdCollaborations_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles grdCollaborations.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Return
        End If

        Dim collaboration As Collaboration = DirectCast(e.Item.DataItem, Collaboration)

        Dim lblCollaborationId As Label = TryCast(e.Item.FindControl("idCollaboration"), Label)
        If lblCollaborationId IsNot Nothing Then
            lblCollaborationId.Text = collaboration.Id.ToString()
        End If

        Dim lblCollaborationObject As Label = TryCast(e.Item.FindControl("collaborationObject"), Label)
        If lblCollaborationObject IsNot Nothing Then
            lblCollaborationObject.Text = collaboration.CollaborationObject
        End If
    End Sub

    Protected Sub btnChangeSigner_Click(sender As Object, e As EventArgs) Handles btnChangeSigner.Click

        If Not CheckSelectedItems() Then
            Exit Sub
        End If

        If ProtocolEnv.CollaborationRightsEnabled AndAlso String.IsNullOrEmpty(ddlDocType.SelectedValue) Then
            AjaxAlert("E' necessario selezionare una Tipologia di Documento per procedere con la modifica.")
            Exit Sub
        End If

        If ddlDocType.SelectedValue.Eq(CollaborationDocumentType.W.ToString()) Then
            AjaxAlert("Non è possibile gestire collaborazioni di tipologia flusso di lavoro")
            Exit Sub
        End If

        Dim parameters As String = "Type=Prot&Action=SelResp"
        Dim environment As DSWEnvironment = DSWEnvironment.Any
        Select Case CurrentDocumentType
            Case CollaborationDocumentType.P.ToString(), CollaborationDocumentType.U.ToString()
                environment = DSWEnvironment.Protocol
                Exit Select
            Case CollaborationDocumentType.A.ToString(), CollaborationDocumentType.D.ToString()
                environment = DSWEnvironment.Resolution
                Exit Select
            Case CollaborationDocumentType.S.ToString(), CollaborationDocumentType.UDS.ToString()
                environment = DSWEnvironment.DocumentSeries
                Exit Select
        End Select

        Dim udsEnv As Integer = -1
        If Integer.TryParse(CurrentDocumentType, udsEnv) AndAlso udsEnv > 99 Then
            environment = DSWEnvironment.DocumentSeries
        End If
        parameters = String.Concat(parameters, String.Format("&DSWEnvironment={0}", environment))
        parameters = String.Concat(parameters, "&CollaborationType=", ddlDocType.SelectedValue)
        Dim url As String = String.Concat("../User/UserCambioResponsabile.aspx?", CommonShared.AppendSecurityCheck(parameters))
        AjaxManager.ResponseScripts.Add(String.Format("OpenWindowsChangeSigner('{0}');", url))
    End Sub

    Protected Sub btnAbsence_Click(sender As Object, e As EventArgs) Handles btnAbsence.Click
        If Not CheckSelectedItems() Then
            Exit Sub
        End If

        If ProtocolEnv.CollaborationRightsEnabled AndAlso (String.IsNullOrEmpty(ddlDocType.SelectedValue) OrElse Not ddlDocType.SelectedValue = CollaborationDocumentType.D.ToString()) Then
            AjaxAlert("Non è possibile gestire le assenze per la tipologia di collaborazione selezionata.")
            Exit Sub
        End If

        Dim url As String = "../User/UserAbsentManagers.aspx?Type=Resl"
        AjaxManager.ResponseScripts.Add(String.Concat("OpenWindow('", url, "','200','100',OnAbsenseClose);"))
    End Sub

    Private Sub ImpersonationFinderDelegate(ByVal source As Object, ByVal e As EventArgs)
        uscCollaborationGrid.Grid.SetImpersonationAction(AddressOf ImpersonateGridCallback)
        uscCollaborationGrid.Grid.SetImpersonationCounterAction(AddressOf ImpersonateGridCallback)
    End Sub

    Public Sub SelectedDateFrom(ByVal sender As Object, ByVal e As SelectedDateChangedEventArgs) Handles rdpDateFrom.SelectedDateChanged
        RemoveHandler AjaxManager.AjaxRequest, AddressOf UserCollRisultatiAjaxRequest
        UpdateClick(btnUpdate, New EventArgs())
    End Sub

    Public Sub SelectedDateTo(ByVal sender As Object, ByVal e As SelectedDateChangedEventArgs) Handles rdpDateTo.SelectedDateChanged
        RemoveHandler AjaxManager.AjaxRequest, AddressOf UserCollRisultatiAjaxRequest
        UpdateClick(btnUpdate, New EventArgs())
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf UserCollRisultatiAjaxRequest
        AddHandler uscCollaborationGrid.Grid.NeedImpersonation, AddressOf ImpersonationFinderDelegate

        'si previene di poter toccare il filtro
        AjaxManager.AjaxSettings.AddAjaxSetting(btnNext, pnlHeaderDiv, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSelectAll, pnlHeaderDiv, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnDeselectAll, pnlHeaderDiv, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdPreviewDocuments, pnlHeaderDiv, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscCollaborationGrid.Grid, pnlHeaderDiv, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSelectAll, pnlHeaderDiv, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnDeselectAll, pnlHeaderDiv, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdCollaborationVersioningManagement, pnlHeaderDiv, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnChangeSigner, pnlHeaderDiv, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnAbsence, pnlHeaderDiv, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRoles, pnlHeaderDiv, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnUoia, pnlHeaderDiv, MasterDocSuite.AjaxFlatLoadingPanel)
        ''si previene l'utilizzo della pulsantiera
        AjaxManager.AjaxSettings.AddAjaxSetting(uscCollaborationGrid.Grid, buttons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdPreviewDocuments, buttons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnDgrooveSigns, buttons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnMultiSign, buttons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSignAndNext, buttons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnNext, buttons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSelectAll, buttons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnDeselectAll, buttons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdCollaborationVersioningManagement, buttons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnChangeSigner, buttons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnAbsence, buttons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRoles, buttons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnUoia, buttons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlDocType, buttons, MasterDocSuite.AjaxFlatLoadingPanel)
        'Si previene di poter toccare le altre collaborazioni
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSelectAll, uscCollaborationGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnDeselectAll, uscCollaborationGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnUpdate, uscCollaborationGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdPreviewDocuments, uscCollaborationGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdCollaborationVersioningManagement, uscCollaborationGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlDocType, uscCollaborationGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(allCollaborations, uscCollaborationGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(activeCollaborations, uscCollaborationGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(pastCollaborations, uscCollaborationGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(signRequired, uscCollaborationGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(onlyVision, uscCollaborationGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(rdpDateFrom, uscCollaborationGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(rdpDateTo, uscCollaborationGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscCollaborationGrid.Grid, uscCollaborationGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnNext, uscCollaborationGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdCollaborationVersioningManagement, uscCollaborationGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnChangeSigner, uscCollaborationGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnAbsence, uscCollaborationGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnRoles, uscCollaborationGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnUoia, uscCollaborationGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)

        'caricamenti
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscCollaborationGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, buttons, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlHeaderDiv, MasterDocSuite.AjaxFlatLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnChangeSigner, btnChangeSigner)

        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, grdCollaborations, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(grdCollaborations, grdCollaborations, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(exitNextAction, grdCollaborations, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnUoia, grdCollaborations, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlDocType, btnUoia)
    End Sub

    Private Sub InitializeActionView()
        Dim collGridViewModel As List(Of GridViewModel) = ProtocolEnv.CollaborationColumnsVisibility
        Select Case Action
            Case CollaborationMainAction.AllaVisioneFirma
                'TitleStep = "Alla Visione/Firma"
                SetColumnVisibility("AllaVisioneFirma", collGridViewModel)
                btnInsert.Visible = True ' ProtocolEnv.InserimentoAllaVisioneFirmaEnabled

            Case CollaborationMainAction.DaVisionareFirmare
                'TitleStep = "Da Visionare/Firmare"
                SetColumnVisibility("DaVisionareFirmare", collGridViewModel)

                rowFilter.Visible = ProtocolEnv.CollaborationFilterEnabled
                allCollaborations.Attributes.Add("filterType", CollaborationFinderFilterType.AllCollaborations.ToString())
                signRequired.Attributes.Add("filterType", CollaborationFinderFilterType.SignRequired.ToString())
                onlyVision.Attributes.Add("filterType", CollaborationFinderFilterType.OnlyVision.ToString())
                activeCollaborations.Visible = False
                pastCollaborations.Visible = False
                lblFilter.Text = "Modalità firma"

                btnDgrooveSigns.Visible = HasDgrooveSigner
                btnMultiSign.Visible = ProtocolEnv.EnableMultiSign AndAlso Not ProtocolEnv.ShowOnlySignAndNextEnabled

                btnSignAndNext.Visible = ProtocolEnv.EnableMultiSign AndAlso ProtocolEnv.EnableNextAfterMultiSign AndAlso Not HasDgrooveSigner
                btnNext.Visible = Not ProtocolEnv.ShowOnlySignAndNextEnabled
                cmdPreviewDocuments.Visible = True
                btnSelectAll.Visible = True
                btnDeselectAll.Visible = True
                cmdCollaborationVersioningManagement.Visible = False 'Not DocSuiteContext.Current.ProtocolEnv.VersioningShareCheckOutEnabled

                Dim uoiaRole As Role = Facade.RoleFacade.GetById(ProtocolEnv.CollaborationRoleUoia)
                If uoiaRole IsNot Nothing Then
                    Dim isChildRole As Boolean = Facade.RoleFacade.CurrentUserIsRoleChildCheck(DSWEnvironment.Protocol, uoiaRole)
                    btnUoia.Visible = ProtocolEnv.CollaborationAggregateEnabled AndAlso isChildRole
                    btnUoia.Enabled = ProtocolEnv.CollaborationAggregateEnabled AndAlso isChildRole _
                                        AndAlso (ddlDocType.SelectedValue.Equals("U") OrElse ddlDocType.SelectedValue.Equals(""))
                End If
            Case CollaborationMainAction.DaFirmareInDelega
                SetColumnVisibility("DaVisionareFirmare", collGridViewModel)

                rowFilter.Visible = ProtocolEnv.CollaborationFilterEnabled
                allCollaborations.Attributes.Add("filterType", CollaborationFinderFilterType.AllCollaborations.ToString())
                signRequired.Attributes.Add("filterType", CollaborationFinderFilterType.SignRequired.ToString())
                onlyVision.Attributes.Add("filterType", CollaborationFinderFilterType.OnlyVision.ToString())
                activeCollaborations.Visible = False
                pastCollaborations.Visible = False
                lblFilter.Text = "Modalità firma"

                btnMultiSign.Visible = ProtocolEnv.EnableMultiSign AndAlso Not ProtocolEnv.ShowOnlySignAndNextEnabled
                btnSignAndNext.Visible = ProtocolEnv.EnableMultiSign AndAlso ProtocolEnv.EnableNextAfterMultiSign
                btnNext.Visible = Not ProtocolEnv.ShowOnlySignAndNextEnabled
                cmdPreviewDocuments.Visible = True
                btnSelectAll.Visible = True
                btnDeselectAll.Visible = True
                cmdCollaborationVersioningManagement.Visible = False 'Not DocSuiteContext.Current.ProtocolEnv.VersioningShareCheckOutEnabled

                Dim uoiaRole As Role = Facade.RoleFacade.GetById(ProtocolEnv.CollaborationRoleUoia)
                If uoiaRole IsNot Nothing Then
                    Dim isChildRole As Boolean = Facade.RoleFacade.CurrentUserIsRoleChildCheck(DSWEnvironment.Protocol, uoiaRole)
                    btnUoia.Visible = ProtocolEnv.CollaborationAggregateEnabled AndAlso isChildRole
                    btnUoia.Enabled = ProtocolEnv.CollaborationAggregateEnabled AndAlso isChildRole _
                                        AndAlso (ddlDocType.SelectedValue.Equals("U") OrElse ddlDocType.SelectedValue.Equals(String.Empty))
                End If


            Case CollaborationMainAction.AlProtocolloSegreteria
                'TitleStep = "Al Protocollo/Segreteria"
                SetColumnVisibility("AlProtocolloSegreteria", collGridViewModel)

            Case CollaborationMainAction.AttivitaInCorso
                'TitleStep = "Attività in Corso"
                SetColumnVisibility("AttivitaInCorso", collGridViewModel)
                ' Pannello filtri
                rowFilter.Visible = ProtocolEnv.CollaborationFilterEnabled
                allCollaborations.Attributes.Add("filterType", CollaborationFinderFilterType.AllCollaborations.ToString())
                activeCollaborations.Attributes.Add("filterType", CollaborationFinderFilterType.ActiveCollaborations.ToString())
                pastCollaborations.Attributes.Add("filterType", CollaborationFinderFilterType.PastCollaborations.ToString())
                onlyVision.Visible = False
                signRequired.Visible = False

                Dim accounts As IList(Of RoleUser) = Facade.RoleUserFacade.GetByUserType(RoleUserType.S, DocSuiteContext.Current.User.FullUserName, False, Nothing, CurrentTenant.TenantAOO.UniqueId)
                If accounts.Count > 0 Then
                    btnChangeSigner.Visible = True
                    btnChangeSigner.Enabled = True
                    If ProtocolEnv.CollaborationRightsEnabled Then
                        btnChangeSigner.Enabled = Not String.IsNullOrEmpty(ddlDocType.SelectedValue)
                    End If
                    CheckAuthorization()
                    cmdPreviewDocuments.Visible = True
                    btnSelectAll.Visible = True
                    btnDeselectAll.Visible = True
                Else
                    uscCollaborationGrid.DisableColumn(uscCollGrid.COLUMN_CLIENT_SELECT)
                End If

                If DocSuiteContext.Current.ProtocolEnv.AbsentManagersCertificates IsNot Nothing AndAlso DocSuiteContext.Current.ProtocolEnv.AbsentManagersCertificates.Managers IsNot Nothing Then
                    btnAbsence.Visible = Facade.CollaborationUsersFacade.IsDirectorSecretary()
                    btnAbsence.Enabled = Not String.IsNullOrEmpty(ddlDocType.SelectedValue) AndAlso Not ddlDocType.SelectedValue = CollaborationDocumentType.D.ToString()
                End If

            Case CollaborationMainAction.DaProtocollareGestire
                'TitleStep = "Da Protocollare/Gestire"
                SetColumnVisibility("DaProtocollareGestire", collGridViewModel)
                cmdPreviewDocuments.Visible = True
                btnSelectAll.Visible = True
                btnDeselectAll.Visible = True
            Case CollaborationMainAction.ProtocollatiGestiti
                'TitleStep = "Protocollati/Gestiti"
                SetColumnVisibility("ProtocollatiGestiti", collGridViewModel)
                cmdPreviewDocuments.Visible = True
                rowDate.Visible = True
                btnUpdate.Visible = True
                btnSelectAll.Visible = True
                btnDeselectAll.Visible = True
            Case CollaborationMainAction.MieiCheckOut
                'TitleStep = "Miei Check Out"
                SetColumnVisibility("MieiCheckOut", collGridViewModel)
                cmdPreviewDocuments.Visible = True
                btnSelectAll.Visible = True
                btnDeselectAll.Visible = True
                cmdCollaborationVersioningManagement.Visible = False 'Not DocSuiteContext.Current.ProtocolEnv.VersioningShareCheckOutEnabled

        End Select
    End Sub

    Private Sub InitializePage()

        ' Pulsante Autorizza
        btnRoles.Visible = False
        If DocSuiteContext.Current.ProtocolEnv.IsCollaborationGroupEnabled Then
            Dim roles As IList(Of RoleUser) = Facade.RoleUserFacade.GetByUserType(RoleUserType.D, DocSuiteContext.Current.User.FullUserName, True, Nothing, CurrentTenant.TenantAOO.UniqueId)
            If Not roles.IsNullOrEmpty() Then
                btnRoles.Visible = False
                Dim url As String = String.Concat("../User/UserRoles.aspx?", CommonShared.AppendSecurityCheck(String.Concat("Account=", True)))
                btnRoles.OnClientClick = String.Format(OPEN_AUTH_WINDOW, url)
            End If
        End If

        ' Pulsanti relativi a Firma
        btnDgrooveSigns.Visible = False
        btnMultiSign.Visible = False
        btnNext.Visible = False
        btnSignAndNext.Visible = False

        ' Altre impostazioni
        rowDate.Visible = False
        btnUpdate.Visible = False

        InitializeActionView()

        If DocSuiteContext.Current.ProtocolEnv.CollaborationManagementExpired Then
            uscCollaborationGrid.RenameColumn(uscCollGrid.COLUMN_MEMORANDUM_DATE, "Data scadenza")
        End If

        ' Imposto titolo della pagina
        Title = String.Concat("Collaborazione - ", TitleStep)
        MasterDocSuite.Title = Title
        MasterDocSuite.HistoryTitle = Title

        uscCollaborationGrid.Grid.DiscardFinder()
    End Sub

    Private Function GetDropDownListItem(template As TemplateCollaboration) As DropDownListItem
        Dim listItem As New DropDownListItem(template.Name, template.DocumentType)

        Select Case template.DocumentType
            Case CollaborationDocumentType.D.ToString()
                listItem.ImageUrl = DEL_GIF_URL
            Case CollaborationDocumentType.A.ToString()
                listItem.ImageUrl = ATTO_GIF_URL
            Case CollaborationDocumentType.S.ToString(),
                 CollaborationDocumentType.UDS.ToString()
                listItem.ImageUrl = ImagePath.SmallDocumentSeries
            Case CollaborationDocumentType.W.ToString()
                listItem.ImageUrl = PROTOCOL_GIF_URL
                listItem.Text = "Attività"
            Case Else
                listItem.ImageUrl = PROTOCOL_GIF_URL
        End Select

        Return listItem
    End Function

    Private Sub BindDdlDocType()
        ddlDocType.Items.Add(New DropDownListItem(String.Empty, String.Empty))
        Dim templates As ICollection(Of WebAPIDto(Of TemplateCollaboration)) = New List(Of WebAPIDto(Of TemplateCollaboration))

        Try
            templates = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentTemplateCollaborationFinder,
                    Function(impersonationType, finder)
                        finder.ResetDecoration()
                        finder.OnlyAuthorized = True
                        finder.Locked = True
                        finder.UserName = DocSuiteContext.Current.User.UserName
                        finder.Domain = DocSuiteContext.Current.User.Domain
                        finder.SortExpressions.AddSafe("Entity.Name", "ASC")
                        Return finder.DoSearch()
                    End Function)
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            AjaxAlert("E' avvenuto un errore durante la ricerca delle tipologie di collaborazione. Provare a ricaricare la pagina.")
            Exit Sub
        End Try

        Dim listItems As List(Of DropDownListItem) = New List(Of DropDownListItem)
        If templates IsNot Nothing AndAlso templates.Count > 0 Then
            listItems = templates.Select(Function(t) Me.GetDropDownListItem(t.Entity)).ToList()
        End If

        listItems.ForEach(Sub(i) ddlDocType.Items.Add(i))
    End Sub

    Private Sub Initialize()
        Session("SessionCollaborationUoia") = Nothing
        Dim sourcePage As MultipleSignBasePage = TryCast(PreviousPage, MultipleSignBasePage)
        If sourcePage IsNot Nothing Then
            Facade.CollaborationVersioningFacade.CheckSignedDocuments(sourcePage.SignedDocuments)
            Response.Redirect(ReturnUrl, True)
        End If

        rdpDateFrom.SelectedDate = FromDate
        rdpDateTo.SelectedDate = ToDate

        btnMultiSign.PostBackUrl = MultipleSignBasePage.GetMultipleSignUrl()
        btnSignAndNext.PostBackUrl = String.Format("{0}?saveResponse=True", MultipleSignBasePage.GetMultipleSignUrl())

        Me.BindDdlDocType()

        If Me.CurrentDocumentType IsNot Nothing _
            AndAlso Me.ddlDocType.Items.Any(Function(i) i.Value.Eq(Me.CurrentDocumentType)) Then
            Me.ddlDocType.SelectedValue = Me.CurrentDocumentType
        End If
    End Sub

    Private Sub InitializeFromSign()

        'Mi assicuro che la firma sia andata a buon fine e che non ho premuto annulla
        If Not BackFromMultiSign AndAlso SignedComplete Then
            If Action.Eq(CollaborationMainAction.DaVisionareFirmare) Then
                If Not SelectedCollaborationsToSign.Any() Then
                    Exit Sub
                End If
                If Not SelectedCollaborations.Any() Then
                    Dim selectedCollaborations As List(Of Integer) = SelectedCollaborationsToSign.Select(Function(s) s.Id).ToList()
                    SelectGridItems(selectedCollaborations)
                End If

                BindNextCollaborationsGrid()
                SignedComplete = Nothing
                AjaxManager.ResponseScripts.Add("AutomaticNextCollaborationsWindow();")
                Exit Sub
            End If
        End If

        SelectedCollaborationsToSign.Clear()
    End Sub

    Private Sub InitializeFinder()
        Dim currentCollaborationFinder As WebAPIFinder.CollaborationFinder = InitCollaborationFinder()
        currentCollaborationFinder.UserName = DocSuiteContext.Current.User.UserName
        currentCollaborationFinder.Domain = DocSuiteContext.Current.User.Domain
        currentCollaborationFinder.FromPostMethod = True


        If String.IsNullOrWhiteSpace(Me.ddlDocType.SelectedValue) Then
            currentCollaborationFinder.CollaborationFinderModel.DocumentType = Nothing
        Else
            currentCollaborationFinder.CollaborationFinderModel.DocumentType = Me.ddlDocType.SelectedValue
        End If

        If Not uscCollaborationGrid.Grid.Finder Is Nothing Then
            Dim collFinder As WebAPIFinder.CollaborationFinder = TryCast(uscCollaborationGrid.Grid.Finder, WebAPIFinder.CollaborationFinder)
            Dim actionTypeEnum As CollaborationFinderActionType = GetActionTypeEnum()
            If Not collFinder Is Nothing Then
                If Not collFinder.CollaborationFinderActionType.Equals(actionTypeEnum) Then
                    uscCollaborationGrid.Grid.CustomPageIndex = 0
                End If
            End If
        End If

        'Imposto Finder e Colonne della griglia
        Select Case Action
            Case CollaborationMainAction.AllaVisioneFirma
                currentCollaborationFinder.CollaborationFinderActionType = CollaborationFinderActionType.AtVisionSign
                If ProtocolEnv.DescendingCollaborationOrder Then
                    currentCollaborationFinder.SortExpressions.Add("Entity.IdCollaboration", "ASC")
                Else
                    currentCollaborationFinder.SortExpressions.Add("Entity.LastChangedDate", "DESC")
                End If

            Case CollaborationMainAction.DaVisionareFirmare
                currentCollaborationFinder.CollaborationFinderActionType = CollaborationFinderActionType.ToVisionSign
                currentCollaborationFinder.CollaborationFinderFilterType = SelectedFilterType
                If ProtocolEnv.DescendingCollaborationOrder Then
                    currentCollaborationFinder.SortExpressions.Add("Entity.IdCollaboration", "ASC")
                Else
                    currentCollaborationFinder.SortExpressions.Add("Entity.LastChangedDate", "DESC")
                End If
            Case CollaborationMainAction.DaFirmareInDelega
                currentCollaborationFinder.CollaborationFinderActionType = CollaborationFinderActionType.ToDelegateVisionSign
                currentCollaborationFinder.CollaborationFinderFilterType = SelectedFilterType
                If ProtocolEnv.DescendingCollaborationOrder Then
                    currentCollaborationFinder.SortExpressions.Add("Entity.IdCollaboration", "ASC")
                Else
                    currentCollaborationFinder.SortExpressions.Add("Entity.LastChangedDate", "DESC")
                End If

            Case CollaborationMainAction.AlProtocolloSegreteria
                currentCollaborationFinder.CollaborationFinderActionType = CollaborationFinderActionType.AtProtocolAdmission
                If ProtocolEnv.DescendingCollaborationOrder Then
                    currentCollaborationFinder.SortExpressions.Add("Entity.IdCollaboration", "ASC")
                Else
                    currentCollaborationFinder.SortExpressions.Add("Entity.LastChangedDate", "DESC")
                End If

            Case CollaborationMainAction.AttivitaInCorso
                currentCollaborationFinder.CollaborationFinderFilterType = SelectedFilterType
                currentCollaborationFinder.CollaborationFinderActionType = CollaborationFinderActionType.Running
                If ProtocolEnv.DescendingCollaborationOrder Then
                    currentCollaborationFinder.SortExpressions.Add("Entity.IdCollaboration", "ASC")
                Else
                    currentCollaborationFinder.SortExpressions.Add("Entity.LastChangedDate", "DESC")
                End If

            Case CollaborationMainAction.DaProtocollareGestire
                currentCollaborationFinder.CollaborationFinderActionType = CollaborationFinderActionType.ToManage
                If ProtocolEnv.DescendingCollaborationOrder Then
                    currentCollaborationFinder.SortExpressions.Add("Entity.IdCollaboration", "ASC")
                Else
                    currentCollaborationFinder.SortExpressions.Add("Entity.LastChangedDate", "DESC")
                End If

            Case CollaborationMainAction.ProtocollatiGestiti
                currentCollaborationFinder.CollaborationFinderModel.DateFrom = rdpDateFrom.SelectedDate.Value.BeginOfTheDay()
                currentCollaborationFinder.CollaborationFinderModel.DateTo = rdpDateTo.SelectedDate.Value.EndOfTheDay()
                currentCollaborationFinder.CollaborationFinderActionType = CollaborationFinderActionType.Registered
                currentCollaborationFinder.SortExpressions.Add("Entity.PublicationDate", "ASC")

            Case CollaborationMainAction.MieiCheckOut
                currentCollaborationFinder.CollaborationFinderActionType = CollaborationFinderActionType.CheckedOut
                If ProtocolEnv.DescendingCollaborationOrder Then
                    currentCollaborationFinder.SortExpressions.Add("Entity.IdCollaboration", "ASC")
                Else
                    currentCollaborationFinder.SortExpressions.Add("Entity.LastChangedDate", "DESC")
                End If
        End Select

        currentCollaborationFinder.PageSize = DocSuiteContext.Current.ProtocolEnv.SearchMaxRecords
        uscCollaborationGrid.Grid.Finder = currentCollaborationFinder
        uscCollaborationGrid.Grid.PageSize = currentCollaborationFinder.PageSize
        uscCollaborationGrid.Grid.DataBindFinder()

        ' disabilito dopo il click del pulsante solo se ci sono documenti selezionati;
        ' la colonna di check è presente quando action è diversa dai casi considerati nell'if
        Dim bDocSelected As Boolean = False
        If Action.Eq(CollaborationMainAction.DaVisionareFirmare) Then
            bDocSelected = Not SelectedCollaborations.IsNullOrEmpty()
        End If

        btnNext.DisableAfterClick = bDocSelected
    End Sub

    Private Function ImpersonateGridCallback(Of TResult)(finder As IFinder, callback As Func(Of TResult)) As TResult
        Return WebAPIImpersonatorFacade.ImpersonateFinder(Of CollaborationFinder, TResult)(finder,
                        Function(impersonationType, wfinder)
                            Return callback()
                        End Function)
    End Function

    Private Sub ExecuteNext(Optional managersAccounts As AbsentManager() = Nothing)
        Try
            Dim selectedItems As IList(Of Integer) = SelectedCollaborationIds
            For Each id As Integer In SelectedCollaborationIds
                Dim collaboration As Collaboration = Facade.CollaborationFacade.GetById(id)
                If collaboration.IdWorkflowInstance.HasValue Then
                    WorkflowConfirmed(collaboration)
                    selectedItems.Remove(id)
                End If
            Next
            FileLogger.Debug(Facade.CollaborationFacade.LoggerName, String.Format("{0}: ExecuteNext su {1} record selezionati", DocSuiteContext.Current.User.FullUserName, selectedItems.Count))
            Facade.CollaborationFacade.NextStep(selectedItems, CurrentTenant.TenantAOO.UniqueId, managersAccounts)
            UpdateActionCp()
        Catch ex As Exception
            FileLogger.Warn(Facade.CollaborationFacade.LoggerName, "Errore su ExecuteNext di Collaborazione", ex)
            AjaxAlert("Errore in Aggiornamento Dati.")
        End Try
    End Sub

    Private Function PushWorkflowNotify(collaboration As Collaboration, cancelCollaboration As Boolean, isComplete As Boolean, hasChangeSigner As Boolean, ByRef lastSignerHasApproved As Boolean) As Boolean
        Dim mapper As MapperCollaborationModel = New MapperCollaborationModel()
        Dim collaborationModel As CollaborationModel = mapper.MappingDTO(collaboration)
        Dim serializedCollaborationModel As String = JsonConvert.SerializeObject(collaborationModel, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)

        Dim result As WebAPIDto(Of WorkflowActivity) = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentWorkflowActivityFinder,
                Function(impersonationType, finder)
                    finder.ResetDecoration()
                    finder.WorkflowInstanceId = collaboration.IdWorkflowInstance.Value
                    finder.ActivityType = Entity.Workflows.WorkflowActivityType.CollaborationSign
                    finder.Statuses = New List(Of WorkflowStatus) From {WorkflowStatus.Todo, WorkflowStatus.Progress}
                    Return finder.DoSearch().FirstOrDefault()
                End Function)

        If result Is Nothing OrElse result.Entity Is Nothing Then
            Return False
        End If
        Dim workflowActivity As WorkflowActivity = result.Entity

        If workflowActivity IsNot Nothing AndAlso (workflowActivity.Status = WorkflowStatus.Todo OrElse workflowActivity.Status = WorkflowStatus.Progress) Then
            Dim resultInstance As WebAPIDto(Of WorkflowInstance) = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentWorkflowInstanceFinder,
                    Function(impersonationType, finder)
                        finder.ResetDecoration()
                        finder.UniqueId = collaboration.IdWorkflowInstance.Value
                        finder.ExpandRepository = True
                        finder.ExpandProperties = False
                        Return finder.DoSearch().FirstOrDefault()
                    End Function)

            Dim currentCollaborationSign As CollaborationSign = Facade.CollaborationSignsFacade.SearchFull(collaboration.Id, True).FirstOrDefault()
            Dim dsw_p_SignerModel As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_MODEL, workflowActivity.UniqueId)
            Dim dsw_p_SignerPosition As WorkflowProperty = GetActivityWorkflowProperty(WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_POSITION, workflowActivity.UniqueId)

            Dim collaborationSignerModels As List(Of CollaborationSignerWorkflowModel) = JsonConvert.DeserializeObject(Of List(Of CollaborationSignerWorkflowModel))(dsw_p_SignerModel.ValueString, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)
            lastSignerHasApproved = True
            If collaborationSignerModels.Count >= currentCollaborationSign.Incremental Then
                lastSignerHasApproved = collaborationSignerModels(currentCollaborationSign.Incremental).HasApproved
            End If
            If cancelCollaboration Then
                lastSignerHasApproved = False
            End If

            Dim workflowNotify As WorkflowNotify = New WorkflowNotify(workflowActivity.UniqueId) With {
                    .WorkflowName = resultInstance?.Entity?.WorkflowRepository?.Name}

            If isComplete Then
                collaborationSignerModels = WorkflowBuildApprovedModel(currentCollaborationSign.Incremental, lastSignerHasApproved, collaborationSignerModels)
                dsw_p_SignerModel.ValueString = JsonConvert.SerializeObject(collaborationSignerModels, DocSuiteContext.DefaultWebAPIJsonSerializerSettings)

                workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_MODEL, New WorkflowArgument() With {
                                            .Name = WorkflowPropertyHelper.DSW_PROPERTY_MODEL,
                                            .PropertyType = ArgumentType.Json,
                                            .ValueString = serializedCollaborationModel})

                workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_MODEL, New WorkflowArgument() With {
                                            .Name = WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_MODEL,
                                            .PropertyType = ArgumentType.Json,
                                            .ValueString = dsw_p_SignerModel.ValueString})
                workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_ACTION_ACTIVITY_MANUAL_COMPLETE, New WorkflowArgument() With {
                                            .Name = WorkflowPropertyHelper.DSW_ACTION_ACTIVITY_MANUAL_COMPLETE,
                                            .PropertyType = ArgumentType.PropertyBoolean,
                                            .ValueBoolean = True})
                workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_POSITION, New WorkflowArgument() With {
                                            .Name = WorkflowPropertyHelper.DSW_PROPERTY_SIGNER_POSITION,
                                            .PropertyType = ArgumentType.PropertyInt,
                                            .ValueInt = dsw_p_SignerPosition.ValueInt.Value + 1})
            End If
            If hasChangeSigner Then
                workflowNotify.OutputArguments.Add(WorkflowPropertyHelper.DSW_ACTION_COLLABORATION_CHANGE_SIGNER, New WorkflowArgument() With {
                                             .Name = WorkflowPropertyHelper.DSW_ACTION_COLLABORATION_CHANGE_SIGNER,
                                             .PropertyType = ArgumentType.Json,
                                             .ValueString = JsonConvert.SerializeObject(currentCollaborationSign)})
            End If
            Dim webApiHelper As WebAPIHelper = New WebAPIHelper()
            If Not WebAPIImpersonatorFacade.ImpersonateSendRequest(webApiHelper, workflowNotify, DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, DocSuiteContext.Current.CurrentTenant.OriginalConfiguration) Then
                AjaxAlert("Completamento attività di workflow non riuscita.")
                Return False
            End If
        End If
        Return True
    End Function

    Protected Sub WorkflowConfirmed(collaboration As Collaboration)
        If Not collaboration.IdWorkflowInstance.HasValue Then
            Return
        End If

        Try
            Dim lastSignerHasApproved As Boolean
            If PushWorkflowNotify(collaboration, False, True, False, lastSignerHasApproved) Then
                Facade.CollaborationFacade.Evict(collaboration)
                Dim reloadedCollaboration As Collaboration = Facade.CollaborationFacade.GetById(collaboration.Id)
                If Not lastSignerHasApproved Then
                    Facade.CollaborationDraftFacade.DeleteFromCollaboration(reloadedCollaboration)
                    Facade.CollaborationFacade.Delete(reloadedCollaboration)
                Else
                    Facade.CollaborationFacade.NextStep(New List(Of Integer) From {reloadedCollaboration.Id}, CurrentTenant.TenantAOO.UniqueId)
                End If
            End If
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            AjaxAlert("Completamento attività di workflow non riuscita.")
        End Try
    End Sub

    Private Sub ExecuteChangeSigner(ByVal serializedSigner As String)
        Try
            Dim changeSigner As ChangeSignerDTO = JsonConvert.DeserializeObject(Of ChangeSignerDTO)(HttpUtility.HtmlDecode(serializedSigner))
            Dim countChanged As Integer = 0
            Dim countTotal As Integer = 0
            Dim lastSignerHasApproved As Boolean
            Dim pushNotify As Boolean = False
            Dim coll As Collaboration

            For Each id As Integer In SelectedCollaborationIds
                coll = Facade.CollaborationFacade.GetById(id)
                Facade.CollaborationFacade.ChangeSigner(coll, changeSigner, countChanged, countTotal, pushNotify, CurrentTenant.TenantAOO.UniqueId)
                If pushNotify AndAlso coll.IdWorkflowInstance.HasValue Then
                    PushWorkflowNotify(coll, False, False, True, lastSignerHasApproved)
                End If
            Next

            AjaxAlert("Aggiornate {0} Collaborazioni su {1}", countChanged, countTotal)
        Catch ex As Exception
            FileLogger.Error(Facade.CollaborationFacade.LoggerName, "Errore in cambio Responsabile", ex)
            AjaxAlert("Errore in Aggiornamento Dati. Contattare l'assistenza")
        End Try

        ' Azzero gli elementi selezionati
        AllGridItemSelection(False)
        ' Aggiorno la griglia
        UpdateActionCp()
    End Sub

    Private Sub UpdateActionCp()
        If Action.Eq(CollaborationMainAction.DaVisionareFirmare) Then
            If SelectedCollaborationsToSign.Any() Then
                Session.Remove("collaborationsToSigns")
                AjaxManager.ResponseScripts.Add("CloseWindow('rebind');")
                Exit Sub
            End If
        End If

        If Action.Eq(CollaborationMainAction.ProtocollatiGestiti) Then
            FromDate = rdpDateFrom.SelectedDate.Value
            ToDate = rdpDateTo.SelectedDate.Value
        End If
        InitializeFinder()
    End Sub

    Private Sub SelectGridItems(collaborationIds As IList(Of Integer))
        For Each item As GridDataItem In uscCollaborationGrid.Grid.Items
            Dim checkBox As CheckBox = CType(item.FindControl("cbSelect"), CheckBox)
            If checkBox Is Nothing Then
                Continue For
            End If
            Dim itemId As Integer = DirectCast(item.GetDataKeyValue("Entity.IdCollaboration"), Integer)

            checkBox.Checked = collaborationIds.Contains(itemId)
        Next
    End Sub

    Private Sub AllGridItemSelection(check As Boolean)
        For Each item As GridDataItem In uscCollaborationGrid.Grid.Items
            Dim checkBox As CheckBox = CType(item.FindControl("cbSelect"), CheckBox)
            If (checkBox IsNot Nothing) AndAlso checkBox.Visible Then
                checkBox.Checked = check
            End If
        Next
    End Sub

    Private Function GetActionTypeEnum() As CollaborationFinderActionType
        Select Case Action
            Case CollaborationMainAction.AllaVisioneFirma
                Return CollaborationFinderActionType.AtVisionSign

            Case CollaborationMainAction.DaVisionareFirmare
                Return CollaborationFinderActionType.ToVisionSign

            Case CollaborationMainAction.AlProtocolloSegreteria
                Return CollaborationFinderActionType.AtProtocolAdmission

            Case CollaborationMainAction.AttivitaInCorso
                Return CollaborationFinderActionType.Running

            Case CollaborationMainAction.DaProtocollareGestire
                Return CollaborationFinderActionType.ToManage

            Case CollaborationMainAction.ProtocollatiGestiti
                Return CollaborationFinderActionType.Registered

            Case CollaborationMainAction.MieiCheckOut
                Return CollaborationFinderActionType.CheckedOut

            Case Else
                Return CollaborationFinderActionType.ToVisionSign
        End Select
    End Function

    Private Sub DeleteCollaboration(ByVal idCollaboration As Integer)
        Try
            Dim collaboration As Collaboration = Facade.CollaborationFacade.GetById(idCollaboration)
            If collaboration.IdWorkflowInstance.HasValue Then
                Dim lastSignerHasApproved As Boolean
                If Not PushWorkflowNotify(collaboration, True, True, False, lastSignerHasApproved) Then
                    AjaxAlert("Errore in annullamento della registrazione")
                    Return
                End If
            End If

            Facade.CollaborationLogFacade.Insert(collaboration, Nothing, Nothing, Nothing, CollaborationLogType.CA, String.Format("Annullamento Collaborazione {0}", collaboration.Id))
            FacadeFactory.Instance.TableLogFacade.Insert("Collaboration", LogEvent.DL, $"Annullata collaborazione {collaboration.CollaborationObject} del {collaboration.RegistrationDate} ({collaboration.Id} - da {DocSuiteContext.Current.User.FullUserName})", collaboration.UniqueId)
            Facade.CollaborationDraftFacade.DeleteFromCollaboration(collaboration)
            Facade.CollaborationFacade.Delete(collaboration)

            InitializeFinder()
        Catch ex As Exception
            FileLogger.Warn(LoggerName, ex.Message, ex)
            AjaxAlert("Errore in annullamento della registrazione")
        End Try
    End Sub

    Private Sub BindNextCollaborationsGrid()
        If Not SelectedCollaborationsToSign.Any() Then
            AjaxAlert("Nessuna collaborazione selezionata.")
            Exit Sub
        End If

        grdCollaborations.DataSource = SelectedCollaborationsToSign
        grdCollaborations.DataBind()
    End Sub

    Private Sub SetColumnVisibility(viewName As String, collGridViewModel As List(Of GridViewModel))
        If collGridViewModel.Any(Function(x) x.ViewName.Eq(viewName)) Then
            Dim colVisibility As Dictionary(Of String, Boolean) = collGridViewModel.Single(Function(x) x.ViewName.Eq(viewName)).ColumnsVisibility
            For Each col As KeyValuePair(Of String, Boolean) In colVisibility.Where(Function(x) Not x.Value)
                uscCollaborationGrid.DisableColumn(col.Key)
            Next
        End If
    End Sub

    Private Function InitCollaborationFinder() As WebAPIFinder.CollaborationFinder
        Return New WebAPIFinder.CollaborationFinder(DocSuiteContext.Current.CurrentTenant)
    End Function

    Private Sub SetManagersAbsence(managersAccounts As AbsentManager())

        Dim selectedCollaborationsCount As Integer = SelectedCollaborationIds.Count
        Dim updatedCollaborationsCount As Integer = 0

        For Each id As Integer In SelectedCollaborationIds
            Dim collaboration As Collaboration = Facade.CollaborationFacade.GetById(id)
            If Facade.CollaborationSignsFacade.SetAbsentManagers(collaboration, managersAccounts) Then
                updatedCollaborationsCount = updatedCollaborationsCount + 1
                Dim currentCollaborationSign As CollaborationSign = collaboration.CollaborationSigns.Where(Function(s) s.IsActive).FirstOrDefault()
                If currentCollaborationSign IsNot Nothing AndAlso currentCollaborationSign.IsAbsent.HasValue AndAlso currentCollaborationSign.IsAbsent.Value Then
                    ExecuteNext(managersAccounts)
                End If
            End If
        Next

        AjaxAlert("Aggiornate {0} Collaborazioni su {1}", updatedCollaborationsCount, selectedCollaborationsCount)
    End Sub

    Private Function CheckSelectedItems() As Boolean
        If SelectedCollaborationIds.Count = 0 Then
            AjaxAlert("Nessuna collaborazione selezionata.")
            Return False
        End If
        Return True
    End Function

    Private Function GetDocumentToSign(collaborationToSign As IList(Of CollaborationResult)) As List(Of DocumentRootFolder)
        Dim docsToSign As New List(Of DocumentRootFolder)

        Dim collaborationSigns As ICollection(Of String)
        Dim coll As Collaboration
        Dim dictionary As IDictionary(Of Guid, BiblosDocumentInfo)
        Dim isSignRequired As Boolean
        Dim loadAlsoOmissis As Boolean
        Dim effectiveSigner As String = String.Empty
        Dim listDelegations As List(Of String) = Facade.UserLogFacade.GetDelegationsSign()

        Dim commentVisibile As Boolean = False
        Dim commentChecked As Boolean = False
        Dim comment As String = String.Empty

        If Not ProtocolEnv.DefaultFVicario.IsNullOrEmpty() Then
            commentVisibile = True
            commentChecked = Facade.RoleUserFacade.GetHighestUserType(CurrentTenant.TenantAOO.UniqueId) = RoleUserType.V
            comment = ProtocolEnv.DefaultFVicario
        End If

        For Each collResult As CollaborationResult In collaborationToSign

            collaborationSigns = Facade.CollaborationSignsFacade.GetEffectiveSigners(collResult.IdCollaboration).Select(Function(s) s.SignUser).ToList()
            coll = Facade.CollaborationFacade.GetById(collResult.IdCollaboration)

            Dim collRootFolder As DocumentRootFolder = New DocumentRootFolder() With {
                .Id = coll.Id,
                .UniqueId = coll.UniqueId,
                .Name = coll.CollaborationObject,
                .SignBehaviour = DocumentSignBehaviour.Collaboration,
                .CommentVisibile = commentVisibile,
                .CommentChecked = commentChecked,
                .Comment = comment
            }

            dictionary = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(coll, VersioningDocumentGroup.MainDocument)
            isSignRequired = True
            If ProtocolEnv.CollaborationFilterEnabled Then
                isSignRequired = Not Action.Eq(CollaborationMainAction.DaVisionareFirmare)
                Dim requiredSign As IList(Of CollaborationSign) = coll.GetRequiredSigns()
                Select Case Action
                    Case CollaborationMainAction.DaFirmareInDelega
                        Dim collaborationSign As CollaborationSign = coll.CollaborationSigns.Where(Function(x) x.IsActive).FirstOrDefault()
                        If listDelegations.Any(Function(x) x.Eq(collaborationSign.SignUser)) Then
                            effectiveSigner = collaborationSign.SignUser
                        End If
                        isSignRequired = requiredSign.Any(Function(x) x.SignUser.Eq(collaborationSign.SignUser))
                    Case Else
                        If Not requiredSign.IsNullOrEmpty() Then
                            isSignRequired = requiredSign.Any(Function(x) x.SignUser.Eq(DocSuiteContext.Current.User.FullUserName))
                        End If
                End Select
            End If

            If Not dictionary.IsNullOrEmpty() Then
                Dim docFolder As DTO.SignDocuments.DocumentFolder = New DTO.SignDocuments.DocumentFolder() With {
                    .Name = "Principale",
                    .ChainType = Model.Entities.DocumentUnits.ChainType.MainChain
                }

                For Each key As Guid In dictionary.Keys
                    Dim doc As DTO.SignDocuments.Document = New DTO.SignDocuments.Document(dictionary(key))
                    doc.CollaborationVersioningId = key
                    doc.CollaborationId = coll.UniqueId
                    doc.Mandatory = isSignRequired OrElse ProtocolEnv.CollaborationMultiSignDocumentsDefaultFlagged
                    doc.MandatorySelectable = Not isSignRequired
                    docFolder.Documents.Add(doc)
                Next
                collRootFolder.DocumentFolders.Add(docFolder)
            End If

            loadAlsoOmissis = Not String.IsNullOrEmpty(coll.DocumentType) AndAlso (DocSuiteContext.Current.IsResolutionEnabled AndAlso (coll.DocumentType.Eq(CollaborationDocumentType.D.ToString()) OrElse coll.DocumentType.Eq(CollaborationDocumentType.A.ToString())))

            If loadAlsoOmissis AndAlso ResolutionEnv.MainDocumentOmissisEnable Then
                dictionary = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(coll, VersioningDocumentGroup.MainDocumentOmissis)

                If Not dictionary.IsNullOrEmpty() Then
                    Dim docFolder As DTO.SignDocuments.DocumentFolder = New DTO.SignDocuments.DocumentFolder() With {
                        .Name = "Omissis",
                        .ChainType = Model.Entities.DocumentUnits.ChainType.MainOmissisChain
                    }

                    For Each key As Guid In dictionary.Keys
                        Dim doc As DTO.SignDocuments.Document = New DTO.SignDocuments.Document(dictionary(key))
                        doc.CollaborationVersioningId = key
                        doc.CollaborationId = coll.UniqueId
                        doc.Mandatory = ProtocolEnv.CollaborationMultiSignDocumentsDefaultFlagged
                        doc.MandatorySelectable = True
                        docFolder.Documents.Add(doc)
                    Next
                    collRootFolder.DocumentFolders.Add(docFolder)
                End If
            End If

            dictionary = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(coll, VersioningDocumentGroup.Attachment)

            If Not dictionary.IsNullOrEmpty() Then
                Dim docFolder As DTO.SignDocuments.DocumentFolder = New DTO.SignDocuments.DocumentFolder() With {
                        .Name = "Allegati",
                        .ChainType = Model.Entities.DocumentUnits.ChainType.AttachmentsChain
                }

                For Each key As Guid In dictionary.Keys
                    Dim doc As DTO.SignDocuments.Document = New DTO.SignDocuments.Document(dictionary(key))
                    doc.CollaborationVersioningId = key
                    doc.CollaborationId = coll.UniqueId
                    doc.Mandatory = ProtocolEnv.CollaborationMultiSignDocumentsDefaultFlagged
                    doc.MandatorySelectable = True
                    docFolder.Documents.Add(doc)
                Next
                collRootFolder.DocumentFolders.Add(docFolder)
            End If

            If loadAlsoOmissis AndAlso ResolutionEnv.AttachmentOmissisEnable Then
                dictionary = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(coll, VersioningDocumentGroup.AttachmentOmissis)

                If Not dictionary.IsNullOrEmpty() Then
                    Dim docFolder As DTO.SignDocuments.DocumentFolder = New DTO.SignDocuments.DocumentFolder() With {
                        .Name = "Allegato Omissis",
                        .ChainType = Model.Entities.DocumentUnits.ChainType.AttachmentOmissisChain
                    }

                    For Each key As Guid In dictionary.Keys
                        Dim doc As DTO.SignDocuments.Document = New DTO.SignDocuments.Document(dictionary(key))
                        doc.CollaborationVersioningId = key
                        doc.CollaborationId = coll.UniqueId
                        doc.Mandatory = ProtocolEnv.CollaborationMultiSignDocumentsDefaultFlagged
                        doc.MandatorySelectable = True
                        docFolder.Documents.Add(doc)
                    Next
                    collRootFolder.DocumentFolders.Add(docFolder)
                End If
            End If

            dictionary = Facade.CollaborationVersioningFacade.GetLastVersionDocuments(coll, VersioningDocumentGroup.Annexed)

            If Not dictionary.IsNullOrEmpty() Then
                Dim docFolder As DTO.SignDocuments.DocumentFolder = New DTO.SignDocuments.DocumentFolder() With {
                        .Name = "Annessi",
                        .ChainType = Model.Entities.DocumentUnits.ChainType.AnnexedChain
                }

                For Each key As Guid In dictionary.Keys
                    Dim doc As DTO.SignDocuments.Document = New DTO.SignDocuments.Document(dictionary(key))
                    doc.CollaborationVersioningId = key
                    doc.CollaborationId = coll.UniqueId
                    doc.Mandatory = ProtocolEnv.CollaborationMultiSignDocumentsDefaultFlagged
                    doc.MandatorySelectable = True
                    docFolder.Documents.Add(doc)
                Next
                collRootFolder.DocumentFolders.Add(docFolder)
            End If
            docsToSign.Add(collRootFolder)
        Next

        Return docsToSign
    End Function

#End Region

End Class