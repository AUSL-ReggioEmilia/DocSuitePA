Imports System.IO
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.UDSDesigner
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class UDSInvoiceSearch
    Inherits CommonBasePage

#Region " Fields "
    Private _UDSRoleUrl As String
    Private Const UDSROLEKEY As String = "UDSRole"
    Private _UDSContactUrl As String
    Private Const UDSCONTACTKEY As String = "UDSContact"

    Private _currentConverter As UDSConverter
    Private ctlTitle As String = "Title"
    Private ctlHeader As String = "Header"
    Private ctlStatus As String = "Status"
    Private ctlEnum As String = "Enum"
    Private ctlText As String = "Text"
    Private ctlNumber As String = "Number"
    Private ctlDate As String = "Date"
    Private ctlCheckbox As String = "Checkbox"
    Private ctlLookup As String = "Lookup"
    Private ctlContact As String = "Contact"
    Private ctlAuthorization As String = "Authorization"
    Private _currentUserLog As UserLog

#End Region

#Region " Properties "
    Private ReadOnly Property CurrentConverter As UDSConverter
        Get
            If _currentConverter Is Nothing Then
                _currentConverter = New UDSConverter()
            End If
            Return _currentConverter
        End Get
    End Property
    Public ReadOnly Property InvoiceDirection As String
        Get
            Return Request.QueryString.GetValueOrDefault("Direction", String.Empty)
        End Get
    End Property

    Public ReadOnly Property InvoiceKind As String
        Get
            Return Request.QueryString.GetValueOrDefault("InvoiceKind", String.Empty)
        End Get
    End Property

    Public ReadOnly Property InvoiceStatus As String
        Get
            Return Request.QueryString.GetValueOrDefault("InvoiceStatus", String.Empty)
        End Get
    End Property
    Public ReadOnly Property CopyToPEC As Boolean?
        Get
            Return Context.Request.QueryString.GetValueOrDefault(Of Boolean?)("CopyToPEC", Nothing)
        End Get
    End Property
    Public ReadOnly Property UDSInvoiceTypology As String
        Get
            Return ProtocolEnv.UDSInvoiceTypology
        End Get
    End Property
    Public ReadOnly Property UDSRoleUrl As String
        Get
            If _UDSRoleUrl Is Nothing Then
                Dim webApiController As String = String.Empty
                Dim webApiPath As String = DocSuiteContext.Current.CurrentTenant.ODATAUrl
                If Not webApiPath.EndsWith("/") Then webApiPath = String.Concat(webApiPath, "/")
                If DocSuiteContext.Current.CurrentTenant.Entities.ContainsKey(UDSROLEKEY) Then
                    webApiController = DocSuiteContext.Current.CurrentTenant.Entities.Item(UDSROLEKEY).ODATAControllerName.ToString
                End If
                _UDSRoleUrl = Path.Combine(webApiPath, webApiController)
            End If
            Return _UDSRoleUrl
        End Get
    End Property
    Public ReadOnly Property UDSContactUrl As String
        Get
            If _UDSContactUrl Is Nothing Then
                Dim webApiController As String = String.Empty
                Dim webApiPath As String = DocSuiteContext.Current.CurrentTenant.ODATAUrl
                If Not webApiPath.EndsWith("/") Then webApiPath = String.Concat(webApiPath, "/")
                If DocSuiteContext.Current.CurrentTenant.Entities.ContainsKey(UDSCONTACTKEY) Then
                    webApiController = DocSuiteContext.Current.CurrentTenant.Entities.Item(UDSCONTACTKEY).ODATAControllerName.ToString
                End If
                _UDSContactUrl = Path.Combine(webApiPath, webApiController)
            End If
            Return _UDSContactUrl
        End Get
    End Property
    Private ReadOnly Property CurrentUserLog As UserLog
        Get
            If _currentUserLog Is Nothing Then
                _currentUserLog = Facade.UserLogFacade.GetByUser(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _currentUserLog
        End Get
    End Property

    Public ReadOnly Property TenantCompanyName As String
        Get
            Dim companyname As String = String.Empty
            If CurrentTenant IsNot Nothing AndAlso ProtocolEnv.InvoiceDashboardFilterByTenantEnabled Then
                companyname = CurrentTenant.TenantName
            End If
            Return companyname
        End Get
    End Property
#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        btnInvoiceMove.Visible = DocSuiteContext.Current.ProtocolEnv.MultiTenantEnabled
    End Sub
#End Region

End Class