Imports Telerik.Web.UI

Public Class uscDossierSummary
    Inherits DocSuite2008BaseControl

    Public ReadOnly Property PageContentDiv As RadPageLayout
        Get
            Return pageContent
        End Get
    End Property
    Public Property DossierStatusEnabled As Boolean = True

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

End Class