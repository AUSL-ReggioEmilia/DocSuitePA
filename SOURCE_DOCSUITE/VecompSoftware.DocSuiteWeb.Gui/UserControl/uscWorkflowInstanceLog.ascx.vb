Imports Telerik.Web.UI

Public Class uscWorkflowInstanceLog
    Inherits DocSuite2008BaseControl

    Public ReadOnly Property PageContentPane As RadAjaxPanel
        Get
            Return pageContent
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

End Class