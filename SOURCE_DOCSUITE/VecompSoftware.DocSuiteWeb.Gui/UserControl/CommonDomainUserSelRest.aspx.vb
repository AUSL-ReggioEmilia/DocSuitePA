Imports System.Web
Imports Telerik.Web.UI

Public Class CommonDomainUserSelRest
    Inherits CommonBasePage
#Region "Properties"
    Public ReadOnly Property PageContent As String
        Get
            Return HttpContext.Current.Request.QueryString("PageContentId")
        End Get
    End Property
#End Region
#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack() Then
            MasterDocSuite.TitleVisible = False
        End If
    End Sub

#End Region
End Class