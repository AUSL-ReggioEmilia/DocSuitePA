Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data

Public Class DgrooveSigns
    Inherits CommonBasePage

    Public ReadOnly Property CurrentUserDomain As String
        Get
            Return JsonConvert.SerializeObject(DocSuiteContext.Current.User.FullUserName)
        End Get
    End Property

    Public ReadOnly Property CurrentUserTenantName As String
        Get
            Return CurrentTenant.TenantName
        End Get
    End Property

    Public ReadOnly Property CurrentUserTenantId As Guid
        Get
            Return CurrentTenant.UniqueId
        End Get
    End Property

    Public ReadOnly Property CurrentUserTenantAOOId As Guid
        Get
            Return CurrentTenant.TenantAOO.UniqueId
        End Get
    End Property

    Public ReadOnly Property SignalrDgrooveSignerUrl As String
        Get
            Return DocSuiteContext.Current.DgrooveSignerUrl
        End Get
    End Property

    Protected ReadOnly Property SignalrWebApiUrl As String
        Get
            Return DocSuiteContext.SignalRServerAddress
        End Get
    End Property

    Public ReadOnly Property DSWUrl As String
        Get
            Return DocSuiteContext.Current.CurrentTenant.DSWUrl
        End Get
    End Property

    Protected ReadOnly Property WorkflowArchiveName As String
        Get
            Dim workflowLocation As Location = Facade.LocationFacade.GetById(ProtocolEnv.WorkflowLocation.Value)
            Return workflowLocation.ProtBiblosDSDB
        End Get
    End Property

    Protected ReadOnly Property CollaborationArchiveName As String
        Get
            Dim collaborationLocation As Location = Facade.LocationFacade.GetById(ProtocolEnv.CollaborationLocation)
            Return collaborationLocation.ProtBiblosDSDB
        End Get
    End Property

    Protected ReadOnly Property HasInfocert As Boolean
        Get
            Return DocSuiteContext.Current.HasInfocertProxySign
        End Get
    End Property

    Protected ReadOnly Property HasAruba As Boolean
        Get
            Return DocSuiteContext.Current.HasArubaActalisSign
        End Get
    End Property

    Protected ReadOnly Property DefaultSignType As String
        Get
            Return ProtocolEnv.DefaultSignType
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

End Class