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
End Class