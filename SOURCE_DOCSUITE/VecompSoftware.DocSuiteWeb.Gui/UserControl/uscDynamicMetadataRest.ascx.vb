Public Class uscDynamicMetadataRest
    Inherits DocSuite2008BaseControl

    Public ReadOnly Property PageContent As HtmlGenericControl
        Get
            Return pageContentDiv
        End Get
    End Property

    Public Property ValidationEnabled As Boolean = True

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

End Class