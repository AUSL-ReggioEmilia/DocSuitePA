Imports Telerik.Web.UI

Public Class FascPeriodInserimento
    Inherits FascBasePage

#Region " Fields "
#End Region

#Region " Properties "
    Public ReadOnly Property PageContentDiv As RadPageLayout
        Get
            Return pageContent
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        uscFascicleInsert.IsPeriodic = True
        uscFascicleInsert.PageContentDiv = PageContentDiv
        InitializeAjax()
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma)
    End Sub

#End Region


End Class