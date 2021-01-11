Imports Telerik.Web.UI

Public Class uscWorkflowDesignerValidations
    Inherits DocSuite2008BaseControl
#Region "Properties"
    Public ReadOnly Property PageContent As RadTreeView
        Get
            Return rtvValidationRules
        End Get
    End Property
    Public ReadOnly Property TableContentControl() As HtmlTable
        Get
            Return tblValidationRules
        End Get
    End Property
    Public Property Caption() As String
        Get
            Return lblCaption.Text
        End Get
        Set(ByVal value As String)
            lblCaption.Text = value
        End Set
    End Property
#End Region
#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
#End Region

End Class