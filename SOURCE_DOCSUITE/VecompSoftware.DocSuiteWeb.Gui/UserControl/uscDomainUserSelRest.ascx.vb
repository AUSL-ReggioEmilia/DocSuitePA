Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade

Public Class uscDomainUserSelRest
    Inherits DocSuite2008BaseControl

#Region "Properties"
    Public ReadOnly Property PageContent As RadTreeView
        Get
            Return RadTreeContact
        End Get
    End Property

    Public Property TreeViewCaption() As String
        Get
            Return RadTreeContact.Nodes(0).Text
        End Get
        Set(ByVal value As String)
            RadTreeContact.Nodes(0).Text = value
        End Set
    End Property
    Protected ReadOnly Property CurrentUserDescription As String
        Get
            Return CommonAD.GetDisplayName(DocSuiteContext.Current.User.FullUserName)
        End Get
    End Property

    Protected ReadOnly Property CurrentUserEmail As String
        Get
            Return FacadeFactory.Instance.UserLogFacade.EmailOfUser(DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain, DocSuiteContext.Current.ProtocolEnv.UserLogEmail)
        End Get
    End Property
#End Region
#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
#End Region

End Class