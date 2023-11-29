Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade

Public Class WorkflowActivityInsert
    Inherits CommonBasePage

    Protected ReadOnly Property FullUserName As String
        Get
            Return JsonConvert.SerializeObject(DocSuiteContext.Current.User.FullUserName)
        End Get
    End Property

    Protected ReadOnly Property FullName As String
        Get
            Return JsonConvert.SerializeObject(CommonAD.GetDisplayName(DocSuiteContext.Current.User.FullUserName))
        End Get
    End Property

    Protected ReadOnly Property Email As String
        Get
            Return FacadeFactory.Instance.UserLogFacade.EmailOfUser(DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain, DocSuiteContext.Current.ProtocolEnv.UserLogEmail)
        End Get
    End Property

    Protected ReadOnly Property WorkflowArchiveName As String
        Get
            Dim workflowLocation As Location = Facade.LocationFacade.GetById(ProtocolEnv.WorkflowLocation.Value)
            Return workflowLocation.ProtBiblosDSDB
        End Get
    End Property

    Protected ReadOnly Property TenantName As String
        Get
            Return CurrentTenant.TenantName
        End Get
    End Property

    Protected ReadOnly Property TenantId As String
        Get
            Return CurrentTenant.UniqueId.ToString()
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not ProtocolEnv.WorkflowLocation.HasValue Then
            Throw New ArgumentNullException("Parameter WorkflowLocation is not defined")
        End If
    End Sub

End Class