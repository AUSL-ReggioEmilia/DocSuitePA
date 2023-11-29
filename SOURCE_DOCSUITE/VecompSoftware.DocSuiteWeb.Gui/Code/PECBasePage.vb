Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.BusinessRule.Rules.Rights.PEC
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.WorkflowsElsa
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.UDS
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons
Imports VecompSoftware.DocSuiteWeb.Facade.WebAPI
Imports VecompSoftware.DocSuiteWeb.Model.Parameters
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class PECBasePage
    Inherits CommonBasePage

#Region " Fields "

    Public Const EmailLabel As String = "E-mail"

    Private _facade As FacadeFactory

    Private _currentPecMail As PECMail
    Private _currentBiblosPecMailWrapper As BiblosPecMailWrapper
    Private _currentPecMailRights As PECMailRightsUtil
    Private _currentPecList As IList(Of PECMail)

    Private _currentPecMailId As Integer? = Nothing
    Private _currentPecMailIdList As IList(Of Integer)

    Private _protocolBoxEnabled As Boolean?
    Private _pecLabel As String

    Private Const SessionUnhandleCommand As String = "PecUnhandled_{0}_User_{1}"
    Private _currentJeepServiceHostFacade As JeepServiceHostFacade

    Protected Const SHOW_PROT_COMMAND_NAME As String = "ShowProt"
    Protected Const SHOW_UDS_COMMAND_NAME As String = "ShowUDS"

    Private _currentUDSFacade As UDSFacade

    Private _currentUserHasProtocolContainersRights As Boolean?
    Private _currentUserHasUDSsRights As Boolean?

#End Region

#Region " Properties "

    Public Overrides ReadOnly Property Facade As FacadeFactory
        Get
            If _facade Is Nothing Then
                _facade = New FacadeFactory("ProtDB")
            End If
            Return _facade
        End Get
    End Property

    Public Overridable Property CurrentPecMailId As Integer?
        Get
            If Not _currentPecMailId.HasValue Then
                '' Ricerca diretta
                _currentPecMailId = GetKeyValue(Of Integer?)("PECId")

                '' Ricerca su Lista
                If Not _currentPecMailId.HasValue AndAlso _currentPecMailIdList IsNot Nothing AndAlso _currentPecMailIdList.HasSingle() Then
                    _currentPecMailId = _currentPecMailIdList.First()
                End If
            End If

            Return _currentPecMailId
        End Get
        Protected Set(value As Integer?)
            ViewState("PECId") = value
        End Set
    End Property

    Public Overridable Property CurrentPecMailIdList As IList(Of Integer)
        Get
            If _currentPecMailIdList.IsNullOrEmpty() Then
                If CurrentPecMailId.HasValue Then
                    ' Se ho un elemento singolo uso quello
                    _currentPecMailIdList = New List(Of Integer)() From {CurrentPecMailId.Value}
                Else
                    ' Se non ho elementi singoli provo a recuperare la lista
                    _currentPecMailIdList = New List(Of Integer)

                    Dim pecIds As List(Of Integer) = GetKeyValue(Of List(Of Integer))("PECIds")
                    If Not pecIds.IsNullOrEmpty() Then
                        pecIds.ForEach(Sub(i) _currentPecMailIdList.Add(i))
                    End If
                End If
            End If
            Return _currentPecMailIdList
        End Get
        Protected Set(value As IList(Of Integer))
            ViewState("PECIds") = value
        End Set
    End Property

    Public ReadOnly Property CurrentPecMail As PECMail
        Get
            If _currentPecMail Is Nothing AndAlso CurrentPecMailId.HasValue Then
                _currentPecMail = Facade.PECMailFacade.GetById(CurrentPecMailId.Value, False)
            End If
            Return _currentPecMail
        End Get
    End Property

    Public ReadOnly Property CurrentPecMailWrapper As BiblosPecMailWrapper
        Get
            If _currentBiblosPecMailWrapper Is Nothing AndAlso CurrentPecMail IsNot Nothing Then
                _currentBiblosPecMailWrapper = New BiblosPecMailWrapper(CurrentPecMail, ProtocolEnv.CheckSignedEvaluateStream)
            End If
            Return _currentBiblosPecMailWrapper
        End Get
    End Property

    Public ReadOnly Property CurrentPecMailRights As PECMailRightsUtil
        Get
            If _currentPecMailRights Is Nothing AndAlso CurrentPecMail IsNot Nothing Then
                _currentPecMailRights = New PECMailRightsUtil(CurrentPecMail, DocSuiteContext.Current.User.FullUserName, CurrentTenant.TenantAOO.UniqueId)
            End If
            Return _currentPecMailRights
        End Get
    End Property

    Public ReadOnly Property CurrentPecMailList As IList(Of PECMail)
        Get
            If _currentPecList.IsNullOrEmpty() AndAlso CurrentPecMailIdList IsNot Nothing Then
                _currentPecList = Facade.PECMailFacade.GetListByIds(CurrentPecMailIdList)
            End If
            Return _currentPecList
        End Get
    End Property

    Public ReadOnly Property ProtocolBoxEnabled As Boolean
        Get
            If _protocolBoxEnabled Is Nothing Then
                _protocolBoxEnabled = Request.QueryString.GetValueOrDefault(Of Boolean)("ProtocolBox", False)
            End If
            Return _protocolBoxEnabled.Value
        End Get
    End Property

    Public ReadOnly Property PecLabel As String
        Get
            If String.IsNullOrEmpty(_pecLabel) Then
                _pecLabel = If(ProtocolBoxEnabled, EmailLabel, "PEC")
            End If
            Return _pecLabel
        End Get
    End Property

    Public Property PecUnhandled As Boolean
        Get
            Dim retval As Boolean? = CType(Session(String.Format(SessionUnhandleCommand, CurrentPecMail, DocSuiteContext.Current.User.FullUserName)), Boolean?)
            Return retval.HasValue AndAlso retval.Value
        End Get
        Set(value As Boolean)
            Session(String.Format(SessionUnhandleCommand, CurrentPecMail, DocSuiteContext.Current.User.FullUserName)) = value
        End Set
    End Property

    Public ReadOnly Property CurrentJeepServiceHostFacade As JeepServiceHostFacade
        Get
            If _currentJeepServiceHostFacade Is Nothing Then
                _currentJeepServiceHostFacade = New JeepServiceHostFacade()
            End If
            Return _currentJeepServiceHostFacade
        End Get
    End Property

    Public ReadOnly Property CurrentUDSFacade As UDSFacade
        Get
            If _currentUDSFacade Is Nothing Then
                _currentUDSFacade = New UDSFacade()
            End If
            Return _currentUDSFacade
        End Get
    End Property


    Public ReadOnly Property CurrentUserHasProtocolContainersRights As Boolean
        Get
            If _currentUserHasProtocolContainersRights Is Nothing Then
                _currentUserHasProtocolContainersRights = CommonShared.UserProtocolCheckRight(ProtocolContainerRightPositions.Insert)
            End If
            Return _currentUserHasProtocolContainersRights.Value
        End Get
    End Property

    Public ReadOnly Property CurrentUserHasUDSsRights As Boolean
        Get
            If _currentUserHasUDSsRights Is Nothing Then
                _currentUserHasUDSsRights = CommonShared.UserUDSCheckRight(ProtocolContainerRightPositions.Insert)
            End If
            Return _currentUserHasUDSsRights.Value
        End Get
    End Property


#End Region

#Region " Events "

    Private Sub PageLoad(sender As Object, e As EventArgs) Handles Me.Load
        If ProtocolBoxEnabled Then
            Form.Attributes("class") = Form.Attributes("class") + " protocolBox"
        End If
    End Sub

#End Region

#Region " Methods "
    Protected Function CheckAllSelectedPecMailIsActive(dgMail As BindGrid) As Boolean
        CheckAllSelectedPecMailIsActive = dgMail.MasterTableView.GetSelectedItems() _
            .FirstOrDefault(Function(f) ActiveType.Cast(ActiveType.PECMailActiveType.Error) = DirectCast(f.GetDataKeyValue("IsActive"), Short) OrElse
                                ActiveType.Cast(ActiveType.PECMailActiveType.Processing) = DirectCast(f.GetDataKeyValue("IsActive"), Short)) Is Nothing
        If Not CheckAllSelectedPecMailIsActive Then
            AjaxAlert("Selezionare solo PEC valide.")
        End If
        Return CheckAllSelectedPecMailIsActive
    End Function

    Protected Function CheckAllSelectedPecMailIsActive(ByRef pecMails As IList(Of PECMail)) As Boolean
        For Each pec As PECMail In pecMails
            If Not New PECMailRightsUtil(pec, DocSuiteContext.Current.User.FullUserName, CurrentTenant.TenantAOO.UniqueId).IsActive Then
                AjaxAlert("Selezionare solo PEC valide.")
                Return False
            End If
        Next
        Return True
    End Function

    Public Sub EvaluatePecStatus(ByRef item As PecMailHeader, ByRef row As GridDataItem)
        If item.IsActive.HasValue AndAlso (item.IsActive.Value = ActiveType.Cast(ActiveType.PECMailActiveType.Error)) Then
            row.BackColor = Drawing.Color.Red
        End If
        If item.IsActive.HasValue AndAlso (item.IsActive.Value = ActiveType.Cast(ActiveType.PECMailActiveType.Processing)) Then
            row.BackColor = Drawing.Color.LightYellow
        End If

    End Sub
    Public Function GetKeyValue(Of T)(key As String) As T
        If ViewState(key) Is Nothing Then

            If HttpContext.Current.Request.QueryString(key) IsNot Nothing Then
                ViewState(key) = HttpContext.Current.Request.QueryString.GetValue(Of T)(key)
            Else
                If PreviousPage IsNot Nothing AndAlso TypeOf PreviousPage Is PECBasePage Then
                    ViewState(key) = DirectCast(PreviousPage, PECBasePage).GetKeyValue(Of T)(key)
                End If
            End If
        End If

        Return CType(ViewState(key), T)
    End Function

    Protected Function GetSelectedPecMails(dgMail As BindGrid) As IList(Of PECMail)
        Return Facade.PECMailFacade.GetListByIds(GetSelectedPecMailIds(dgMail))
    End Function

    Protected Function GetSelectedPecMailIds(dgMail As BindGrid) As List(Of Integer)
        Return (From selectedItem In dgMail.MasterTableView.GetSelectedItems() Select DirectCast(selectedItem.GetDataKeyValue("Id"), Integer)).ToList()
    End Function

    Protected Function GetSelectedPecMail(Of T As New)(ByRef getter As Func(Of BindGrid, IList(Of T)), ByRef dgMail As BindGrid, ByRef results As IList(Of T)) As Boolean
        GetSelectedPecMail = True
        results = getter(dgMail)
        If results.IsNullOrEmpty() Then
            AjaxAlert("Selezionare almeno una PEC.")
            GetSelectedPecMail = False
        End If
    End Function


    Public Sub FordwardMail(CurrentMailBox As PECMailBox, ByRef dgMail As BindGrid)
        Dim selectedIds As IList(Of Integer) = Nothing
        If Not GetSelectedPecMail(Of Integer)(Function(f) GetSelectedPecMailIds(f), dgMail, selectedIds) OrElse Not CheckAllSelectedPecMailIsActive(dgMail) Then
            Exit Sub
        End If

        Dim url As New StringBuilder
        url.AppendFormat("~/PEC/PECInsert.aspx?Type=PEC&ForwardPECMode=True&SimpleMode=True")
        url.AppendFormat("&PECIds={0}&ProtocolBox={1}", JsonConvert.SerializeObject(selectedIds.ToList()), ProtocolBoxEnabled)

        If FacadeFactory.Instance.PECMailboxFacade.IsRealPecMailBox(CurrentMailBox) Then
            url.AppendFormat("&SelectedMailboxId={0}", CurrentMailBox.Id.ToString())
        End If

        Server.Transfer(url.ToString())
    End Sub

    Protected Sub SetColumnVisibility(viewName As String, PECViewModel As List(Of GridViewModel), dgMail As BindGrid)
        If PECViewModel.Any(Function(x) x.ViewName.Eq(viewName)) Then
            Dim colVisibility As Dictionary(Of String, Boolean) = PECViewModel.Single(Function(x) x.ViewName.Eq(viewName)).ColumnsVisibility
            For Each col As KeyValuePair(Of String, Boolean) In colVisibility
                dgMail.Columns.FindByUniqueName(col.Key).Visible = col.Value
            Next
        End If
    End Sub

    Protected Function IsPecResendable(pec As PECMail) As Boolean
        If pec Is Nothing Then
            Throw New ArgumentNullException("pec")
        End If

        If Not pec.HasDocumentUnit() Then
            Throw New DocSuiteException("Si possono reinviare solo PEC gestite.")
        End If

        If Not New PECMailRightsUtil(pec, DocSuiteContext.Current.User.FullUserName, CurrentTenant.TenantAOO.UniqueId).IsResendable Then
            Dim message As String = String.Format("La PEC ""{0}"" [{1}] non risulta essere reinviabile.", pec.MailSubject, pec.Id)
            Throw New DocSuiteException(message)
        End If

        Return True
    End Function
    Protected Function GetPrimaryIconUrl(ByVal DocumentUnitType As DSWEnvironment) As String
        Dim primaryIconUrl As String = String.Empty
        Select Case DocumentUnitType
            Case DSWEnvironment.Protocol
                primaryIconUrl = "../Comm/Images/DocSuite/Protocollo16.png"
            Case Else
                primaryIconUrl = ImagePath.SmallDocumentSeries
        End Select

        Return primaryIconUrl
    End Function

    Protected Function GetCommandName(ByVal DocumentUnitType As DSWEnvironment) As String
        Dim commandName As String = String.Empty
        Select Case DocumentUnitType
            Case DSWEnvironment.Protocol
                commandName = SHOW_PROT_COMMAND_NAME
            Case Else
                If DocumentUnitType >= 100 Then
                    commandName = SHOW_UDS_COMMAND_NAME
                End If
        End Select
        Return commandName
    End Function

    Protected Function GetCommandArgument(ByVal item As PecMailHeader) As String
        Dim commandArgument As String = String.Empty
        Select Case item.DocumentUnitType
            Case DSWEnvironment.Protocol
                commandArgument = String.Format("{0}|{1}|{2}", item.IdDocumentUnit, item.Year, item.Number)
            Case Else
                If item.DocumentUnitType >= 100 Then
                    commandArgument = String.Format("{0}|{1}", item.IdDocumentUnit, item.IdUDSRepository)
                End If
        End Select
        Return commandArgument
    End Function

    Protected Function GetToolTip(ByVal item As PecMailHeader) As String
        Dim toolTip As String = String.Empty
        Select Case item.DocumentUnitType
            Case DSWEnvironment.Protocol
                toolTip = String.Format("Protocollo {0}/{1:0000000}", item.Year, item.Number)
            Case Else
                If item.DocumentUnitType >= 100 Then
                    Dim repository As UDSRepository = CurrentUDSRepositoryFacade.GetById(item.IdUDSRepository.Value)
                    toolTip = String.Format("{0} {1}/{2:0000000}", repository.Name, item.Year, item.Number)
                End If
        End Select
        Return toolTip
    End Function

    Protected Sub CleanCurrentPecMailRights()
        _currentPecMailRights = Nothing
    End Sub

    Protected Sub DelegateElsaInitialize(pec As PECMail)
        If pec.ProcessStatus = PECMailProcessStatus.StoredInDocumentManager AndAlso Not String.IsNullOrEmpty(pec.MailContent) Then
            Dim str As String = New FacadeElsaWebAPI(ProtocolEnv.DocSuiteNextElsaBaseURL).StartPreparePECMailDocumentsWorkflow(pec.UniqueId, JsonConvert.DeserializeObject(Of List(Of DocumentInfoModel))(pec.MailContent))
        End If
    End Sub

#End Region

End Class
