Imports Telerik.Web.UI

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
#End Region
#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
#End Region

End Class