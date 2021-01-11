Public Class uscCustomActionsRest
    Inherits DocSuite2008BaseControl

#Region " Fields "
#End Region

#Region " Properties "
    Public ReadOnly Property PageContent As HtmlGenericControl
        Get
            Return pageContentDiv
        End Get
    End Property

    Public Property IsSummary As Boolean = False
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
#End Region

#Region "Methods "
#End Region

End Class