Imports Telerik.Web.UI

Partial Public Class uscFascSummary
    Inherits DocSuite2008BaseControl

#Region " Fields "

#End Region

#Region " Properties "

    Public ReadOnly Property PageContentDiv As RadPageLayout
        Get
            Return pageContent
        End Get
    End Property

    Public Property IsEditPage As Boolean
    Public Property IsAuthorizePage As Boolean
    Public Property IsSummaryLink As Boolean

    Public Property CurrentFascicleId As Guid
        Get
            If ViewState(String.Format("{0}_CurrentFascicleId", ID)) IsNot Nothing Then
                Return DirectCast(ViewState(String.Format("{0}_CurrentFascicleId", ID)), Guid)
            End If
            Return Guid.Empty
        End Get
        Set(value As Guid)
            ViewState(String.Format("{0}_CurrentFascicleId", ID)) = value
        End Set
    End Property

    Public Property CurrentWorkflowActivityId As String

#End Region

#Region " Events "

    Private Sub uscFascicolo_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
        End If
    End Sub
#End Region

#Region " Methods "

    Private Sub InitializeAjax()

    End Sub

#End Region

End Class