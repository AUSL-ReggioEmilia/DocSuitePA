Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports System.Web
Imports VecompSoftware.Services.Biblos.Models
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.DTO.DocumentUnits
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade.Common.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.UDS
Imports VecompSoftware.DocSuiteWeb.Facade.Common.WebAPI

Namespace Viewers
    Public Class ProtocolViewer
        Inherits ProtBasePage
        Implements ISendMail, IHaveViewerLight

#Region " Fields "

        Private _protocolsKeys As List(Of Guid) = Nothing
        Private _multipleProtocols As Boolean?
        Private _protocolRightsList As IList(Of ProtocolRights)
        Private _protocols As List(Of Protocol) = Nothing
        Private _currentUDSFacade As UDSFacade
        Private _currentUDSDocumentUnitFinder As UDSDocumentUnitFinder
        Private Const ODATA_EQUAL_UDSID As String = "$filter=UDSId eq {0}"

#End Region

#Region " Properties "
        Public ReadOnly Property CheckedDocuments() As List(Of DocumentInfo) Implements IHaveViewerLight.CheckedDocuments
            Get
                Return ViewerLight.CheckedDocuments
            End Get
        End Property
        Public ReadOnly Property SelectedDocument() As DocumentInfo Implements IHaveViewerLight.SelectedDocument
            Get
                Return ViewerLight.SelectedDocument
            End Get
        End Property

        Private ReadOnly Property ProtocolsKeys As List(Of Guid)
            Get
                If _protocolsKeys.IsNullOrEmpty() Then
                    If ViewState("keys") Is Nothing Then
                        _protocolsKeys = HttpContext.Current.Request.QueryString.GetValueOrDefault("keys", New List(Of Guid))
                        ViewState("keys") = _protocolsKeys
                    Else
                        _protocolsKeys = DirectCast(ViewState("keys"), List(Of Guid))
                    End If
                End If
                Return _protocolsKeys
            End Get
        End Property

        Private ReadOnly Property MultipleProtocols As Boolean
            Get
                If Not _multipleProtocols.HasValue Then
                    If ViewState("MultipleProtocols") Is Nothing Then
                        _multipleProtocols = HttpContext.Current.Request.QueryString.GetValueOrDefault("multiple", False)
                        ViewState("MultipleProtocols") = _multipleProtocols.Value
                    Else
                        _multipleProtocols = DirectCast(ViewState("MultipleProtocols"), Boolean)
                    End If
                End If
                Return _multipleProtocols.Value
            End Get
        End Property

        Private ReadOnly Property ProtocolList As List(Of Protocol)
            Get
                If Not ProtocolsKeys.IsNullOrEmpty() AndAlso _protocols Is Nothing Then
                    _protocols = ProtocolsKeys.Select(Function(k) FacadeFactory.Instance.ProtocolFacade.GetById(k)).ToList()
                End If
                If _protocols Is Nothing Then
                    _protocols = New List(Of Protocol)()
                End If
                Return _protocols
            End Get
        End Property
        Private ReadOnly Property ProtocolRightsList As IList(Of ProtocolRights)
            Get
                If _protocolRightsList Is Nothing Then
                    _protocolRightsList = New List(Of ProtocolRights)
                    Dim statusCancel As Boolean = ProtocolEnv.ProtocolDocumentHandlerStatusCancel
                    For Each prot As Protocol In ProtocolList
                        _protocolRightsList.Add(New ProtocolRights(prot, statusCancel))
                    Next
                End If
                Return _protocolRightsList
            End Get
        End Property

        Public ReadOnly Property SenderDescription() As String Implements ISendMail.SenderDescription
            Get
                Return CommonInstance.UserDescription
            End Get
        End Property

        Public ReadOnly Property SenderEmail() As String Implements ISendMail.SenderEmail
            Get
                Return CommonInstance.UserMail
            End Get
        End Property

        Public ReadOnly Property Recipients() As IList(Of ContactDTO) Implements ISendMail.Recipients
            Get
                Return New List(Of ContactDTO)()
            End Get
        End Property

        Public ReadOnly Property Documents() As IList(Of DocumentInfo) Implements ISendMail.Documents
            Get

                If MultipleProtocols Then
                    Dim list As New List(Of DocumentInfo)
                    For Each prot As Protocol In ProtocolList
                        list.AddRange(Facade.DocumentFacade.FilterAllowedDocuments(ProtocolFacade.GetAllDocuments(prot), prot.Container.Id,
                                                                                   prot.Roles.Where(Function(r) String.IsNullOrEmpty(r.Type) OrElse Not r.Type.Eq(ProtocolRoleTypes.Privacy)).Select(Function(t) t.Role.UniqueId).ToArray(),
                                                                                   prot.Roles.Where(Function(r) Not String.IsNullOrEmpty(r.Type) AndAlso r.Type.Eq(ProtocolRoleTypes.Privacy)).Select(Function(t) t.Role.UniqueId).ToArray(), DSWEnvironment.Protocol,
                                                                                   prot.Users.Any(Function(u) u.Account.Eq(DocSuiteContext.Current.User.FullUserName) AndAlso u.Type = ProtocolUserType.Authorization)))
                    Next
                    Return list
                End If

                Return Facade.DocumentFacade.FilterAllowedDocuments(ProtocolFacade.GetAllDocuments(CurrentProtocol), CurrentProtocol.Container.Id,
                                                                    CurrentProtocol.Roles.Where(Function(r) String.IsNullOrEmpty(r.Type) OrElse Not r.Type.Eq(ProtocolRoleTypes.Privacy)).Select(Function(t) t.Role.UniqueId).ToArray(),
                                                                    CurrentProtocol.Roles.Where(Function(r) Not String.IsNullOrEmpty(r.Type) AndAlso r.Type.Eq(ProtocolRoleTypes.Privacy)).Select(Function(t) t.Role.UniqueId).ToArray(),
                                                                    DSWEnvironment.Protocol, CurrentProtocol.Users.Any(Function(u) u.Account.Eq(DocSuiteContext.Current.User.FullUserName) AndAlso u.Type = ProtocolUserType.Authorization))
            End Get
        End Property

        Public ReadOnly Property Subject() As String Implements ISendMail.Subject
            Get
                If MultipleProtocols Then
                    Return String.Empty
                End If

                Return MailFacade.GetProtocolSubject(CurrentProtocol)
            End Get
        End Property

        Public ReadOnly Property Body() As String Implements ISendMail.Body
            Get
                If MultipleProtocols Then
                    Return String.Empty
                End If

                Return MailFacade.GetProtocolBody(CurrentProtocol, withProtocolLinks:=ProtocolEnv.MailBodyProtocolLinksEnabled)
            End Get
        End Property

        Private ReadOnly Property CurrentUDSFacade As UDSFacade
            Get
                If _currentUDSFacade Is Nothing Then
                    _currentUDSFacade = New UDSFacade()
                End If
                Return _currentUDSFacade
            End Get
        End Property

        Private ReadOnly Property CurrentUDSDocumentUnitFinder As UDSDocumentUnitFinder
            Get
                If _currentUDSDocumentUnitFinder Is Nothing Then
                    _currentUDSDocumentUnitFinder = New UDSDocumentUnitFinder(DocSuiteContext.Current.Tenants)
                End If
                Return _currentUDSDocumentUnitFinder
            End Get
        End Property
#End Region

#Region " Events "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            'SetResponseNoCache()
            If Not MultipleProtocols Then
                ViewerLight.PrefixFileName = String.Concat("PROT_", CurrentProtocol.Year, "_", CurrentProtocol.Number.ToString("0000000"))
            End If
            InitializeAjax()
            If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
                MasterDocSuite.TitleVisible = False
                If Not MultipleProtocols Then
                    btnSend.PostBackUrl = String.Format("{0}?recipients=false&UniqueId={1}&overridepreviouspageurl=true&Type=Prot&FromViewer=true", btnSend.PostBackUrl, CurrentProtocol.Id)
                End If
                BindViewerLight()
                If ProtocolEnv.SendProtocolMessageFromViewerEnabled Then
                    btnMailProtocol.Visible = True
                    btnPEC.Visible = True
                    btnPECProtocol.Visible = True
                    Dim serialized As String = String.Empty
                    If MultipleProtocols Then
                        serialized = JsonConvert.SerializeObject(ProtocolsKeys)
                    Else
                        serialized = JsonConvert.SerializeObject(New List(Of Guid) From {CurrentProtocol.Id})
                    End If
                    btnMailProtocol.PostBackUrl = String.Format("~/MailSenders/AggregateToProtocol.aspx?multiple=true&keys={0}&ToSendProtocolType={1}&FromViewer=true", HttpUtility.UrlEncode(serialized), SendDocumentUnitType.ToMail)
                    btnPECProtocol.PostBackUrl = String.Format("~/MailSenders/AggregateToProtocol.aspx?multiple=true&keys={0}&ToSendProtocolType={1}&FromViewer=true", HttpUtility.UrlEncode(serialized), SendDocumentUnitType.ToPec)
                    btnPEC.PostBackUrl = "~/PEC/PECInsert.aspx?multiple=true&FromViewer=true"
                End If
            End If
            If ProtocolEnv.AssignProtocolEnabled Then
                btnAssegna.Visible = ProtocolRightsList.Any(Function(p) p.IsEditable OrElse p.IsEditableAttachment.GetValueOrDefault(False))
            End If

            If DocSuiteContext.Current.PrivacyLevelsEnabled AndAlso Not MultipleProtocols AndAlso CurrentProtocolRights.IsEditable AndAlso CurrentProtocol.Container.PrivacyEnabled Then
                ViewerLight.ModifyPrivacyEnabled = True
                ViewerLight.CurrentDocumentUnitID = CurrentProtocol.Id
                ViewerLight.CurrentLocationId = CurrentProtocol.Location.Id
            End If
            ViewerLight.CheckViewableRight = True
            ViewerLight.AlwaysDocumentTreeOpen = ProtocolEnv.ViewLightAlwaysOpenPages.Contains("ProtocolViewer")
        End Sub
        Private Sub BtnAssegnaClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnAssegna.Click
            Dim checkedDocuments As List(Of DocumentInfo) = Me.CheckedDocuments
            Dim selected As List(Of Guid) = New List(Of Guid)()
            Dim protocolRights As ProtocolRights = Nothing
            Dim uniqueId As Guid
            Dim protocol As Protocol = Nothing
            Dim documentEnvironment As DSWEnvironment
            Dim haveEnvironment As Boolean
            ' Registro il log di visualizzazione dei documenti
            For Each doc As DocumentInfo In checkedDocuments.Where(Function(f) f.Attributes.ContainsKey(ViewerLight.BIBLOS_ATTRIBUTE_UniqueId))
                haveEnvironment = doc.Attributes.ContainsKey(ViewerLight.BIBLOS_ATTRIBUTE_Environment) AndAlso [Enum].TryParse(doc.Attributes(ViewerLight.BIBLOS_ATTRIBUTE_Environment), documentEnvironment)
                If Guid.TryParse(doc.Attributes(ViewerLight.BIBLOS_ATTRIBUTE_UniqueId), uniqueId) AndAlso (Not haveEnvironment OrElse documentEnvironment = DSWEnvironment.Protocol) Then
                    protocol = FacadeFactory.Instance.ProtocolFacade.GetById(uniqueId)
                    If protocol IsNot Nothing AndAlso Facade.DocumentFacade.CheckPrivacy(doc, protocol.Container.Id,
                                                                                         protocol.Roles.Where(Function(r) String.IsNullOrEmpty(r.Type) OrElse Not r.Type.Eq(ProtocolRoleTypes.Privacy)).Select(Function(t) t.Role.UniqueId).ToArray(),
                                                                                         protocol.Roles.Where(Function(r) Not String.IsNullOrEmpty(r.Type) AndAlso r.Type.Eq(ProtocolRoleTypes.Privacy)).Select(Function(t) t.Role.UniqueId).ToArray(),
                                                                                         DSWEnvironment.Protocol, protocol.Users.Any(Function(u) u.Account.Eq(DocSuiteContext.Current.User.FullUserName) AndAlso u.Type = ProtocolUserType.Authorization)) Then
                        protocolRights = New ProtocolRights(protocol)
                        If protocolRights.IsEditable OrElse protocolRights.IsEditableAttachment.GetValueOrDefault(False) Then
                            selected.Add(protocol.UniqueId)
                        End If
                    End If
                End If
            Next
            Dim serialized As String = JsonConvert.SerializeObject(selected)
            Dim encoded As String = HttpUtility.UrlEncode(serialized)
            Dim redirectUrl As String = "~/Prot/ProtAssegna.aspx?multiple=true&selectedKeys={0}&keys={1}"
            redirectUrl = String.Format(redirectUrl, encoded, HttpUtility.UrlEncode(JsonConvert.SerializeObject(ProtocolsKeys)))
            Response.Redirect(redirectUrl)
        End Sub
#End Region

#Region " Methods "

        Protected Sub ManagerAjaxRequests(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
            Dim arguments As String() = Split(e.Argument, "|")
            ViewerLight.ReloadViewer(arguments, GetProtocolDocuments())
        End Sub

        Private Sub InitializeAjax()
            AddHandler MasterDocSuite.AjaxManager.AjaxRequest, AddressOf ManagerAjaxRequests
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, pnlMainContent, MasterDocSuite.AjaxDefaultLoadingPanel)
        End Sub

        Private Sub BindViewerLight()
            ViewerLight.DataSource = GetProtocolDocuments()
            ViewerLight.DataBind()
            FileLogger.Debug(LoggerName, String.Format("{0} - BindViewerLight", Request.RawUrl))
        End Sub

        Private Function GetProtocolDocuments() As List(Of DocumentInfo)
            Dim datasource As List(Of DocumentInfo)
            If MultipleProtocols Then
                datasource = ProtocolList.Select(Function(f) ProtocolFacade.GetProtocolDocuments(f)).ToList()
                Dim currentRelatedUDS As UDSDto
                For Each prot As Protocol In ProtocolList
                    currentRelatedUDS = GetRelatedUDS(prot)
                    If currentRelatedUDS IsNot Nothing Then
                        datasource.Add(UDSFacade.GetUDSTreeDocuments(currentRelatedUDS, Sub(document As BiblosDocumentInfo, documentType As Helpers.UDS.UDSDocumentType)
                                                                                            document.AddAttribute(ViewerLight.BIBLOS_ATTRIBUTE_UniqueId, prot.Id.ToString())
                                                                                            document.AddAttribute(ViewerLight.BIBLOS_ATTRIBUTE_Environment, DirectCast(DSWEnvironment.UDS, Integer).ToString())
                                                                                        End Sub))
                    End If
                Next
            Else
                datasource = New List(Of DocumentInfo) From {ProtocolFacade.GetProtocolDocuments(CurrentProtocol)}
                Dim currentRelatedUDS As UDSDto = GetRelatedUDS(CurrentProtocol)
                If currentRelatedUDS IsNot Nothing Then
                    datasource.Add(UDSFacade.GetUDSTreeDocuments(currentRelatedUDS, Sub(document As BiblosDocumentInfo, documentType As Helpers.UDS.UDSDocumentType)
                                                                                        document.AddAttribute(ViewerLight.BIBLOS_ATTRIBUTE_UniqueId, CurrentProtocol.Id.ToString())
                                                                                        document.AddAttribute(ViewerLight.BIBLOS_ATTRIBUTE_Environment, DirectCast(DSWEnvironment.UDS, Integer).ToString())
                                                                                    End Sub))
                End If
            End If

            Return datasource
        End Function

        Public Function GetRelatedUDS(protocol As Protocol) As UDSDto
            If Not DocSuiteContext.Current.ProtocolEnv.ShowUDSChainsInProtocolViewer Then
                Return Nothing
            End If

            Dim result As ICollection(Of WebAPIDto(Of Entity.UDS.UDSDocumentUnit)) = WebAPIImpersonatorFacade.ImpersonateFinder(CurrentUDSDocumentUnitFinder,
                            Function(impersonationType, finder)
                                finder.ResetDecoration()
                                finder.EnablePaging = False
                                finder.ExpandRepository = True
                                finder.IdDocumentUnit = protocol.UniqueId
                                Return finder.DoSearch()
                            End Function)

            If result Is Nothing Then
                Return Nothing
            End If

            Dim relatedUDS As Entity.UDS.UDSDocumentUnit = result.Select(Function(s) s.Entity).SingleOrDefault()
            If relatedUDS Is Nothing Then
                Return Nothing
            End If
            Dim currentRepository As Data.Entity.UDS.UDSRepository = CurrentUDSRepositoryFacade.GetById(relatedUDS.Repository.UniqueId)
            Return CurrentUDSFacade.GetUDSSource(currentRepository, String.Format(ODATA_EQUAL_UDSID, relatedUDS.IdUDS))
        End Function
#End Region

    End Class
End Namespace