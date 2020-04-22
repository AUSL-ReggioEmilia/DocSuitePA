Imports Telerik.Web.UI
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Facade

Public Class uscAmmTraspMonitorLog
    Inherits DocSuite2008BaseControl

#Region " Fields "
    Private Const LOAD_OWNER_ROLE As String = "LoadOwnerRole"

    Private Const LOAD_ROLE_ID As String = "LoadRoleId"

#End Region
#Region " Properties "
    Public ReadOnly Property PageContent As RadWindow
        Get
            Return rwAmmTraspMonitorLog
        End Get
    End Property

    Public Property DocumentUnitId As Label
        Get
            Return txtAmmTraspMonitorLogDocumentUnitId
        End Get
        Set
            txtAmmTraspMonitorLogDocumentUnitId = Value
        End Set
    End Property

    Public Property DocumentUnitName As Label
        Get
            Return txtAmmTraspMonitorLogDocumentUnitName
        End Get
        Set
            txtAmmTraspMonitorLogDocumentUnitName = Value
        End Set
    End Property
    Public ReadOnly Property MonitoringTransparentRatings As String
        Get
            Return JsonConvert.SerializeObject(ProtocolEnv.MonitoringTransparentRatings)
        End Get
    End Property

    Protected ReadOnly Property CurrentDisplayName As String
        Get
            Return CommonAD.GetDisplayName(DocSuiteContext.Current.User.FullUserName)
        End Get
    End Property

    Public ReadOnly Property uscMonitoringEditButton As RadImageButton
        Get
            Return uscMonitoraggio.EditButton
        End Get
    End Property

    Public Property OwnerRoleId As Integer?
#End Region

#Region " Events "
    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            dpAmmTraspMonitorLogDate.Text = Date.Now.ToString("dd/MM/yyyy hh:mm")
            txtAmmTraspMonitorLogName.Text = CommonAD.GetDisplayName(DocSuiteContext.Current.User.FullUserName)
        End If
    End Sub

    Private Sub uscAmmTraspMonitorLog_AjaxRequest(sender As Object, e As AjaxRequestEventArgs)
        Dim ajaxModel As AjaxModel
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch
            Exit Sub
        End Try

        If ajaxModel Is Nothing Then
            Return
        End If

        If String.Equals(ajaxModel.ActionName, LOAD_OWNER_ROLE) AndAlso OwnerRoleId.HasValue Then
            uscOwnerRole.RemoveAllRoles()
            Dim role As Role = Facade.RoleFacade.GetById(OwnerRoleId.Value)
            uscOwnerRole.AddRole(role, True, False, False, False)
        End If

        If String.Equals(ajaxModel.ActionName, LOAD_ROLE_ID) Then
            uscOwnerRole.RemoveAllRoles()
            Dim role As Role = Facade.RoleFacade.GetById(Integer.Parse(ajaxModel.Value(0)))
            uscOwnerRole.AddRole(role, True, False, False, False)
        End If

    End Sub

#End Region
#Region " Methods "
    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf uscAmmTraspMonitorLog_AjaxRequest
    End Sub
#End Region
End Class
