Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Facade

Public Class UtltADProperties
    Inherits UtltBasePage

#Region " Properties "

    Public ReadOnly Property SearchAccountTextBox As RadTextBox
        Get
            Dim toolBarItem As RadToolBarItem = ToolBarSearch.FindItemByValue("search")
            Return DirectCast(toolBarItem.FindControl("txtAccount"), RadTextBox)
        End Get
    End Property
#End Region
#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        MasterDocSuite.TitleVisible = False

    End Sub

#End Region

#Region " Methods "
    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(ToolBarSearch, phProperty, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, phProperty, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub LoadProperties(ByVal userAccount As String)
        'Visualizzo tutte le property
        Dim foundAdUser As AccountModel = CommonAD.GetAccount(userAccount)
        phProperty.Text = $"<pre><code>{foundAdUser.JsonFormat}</code></pre>"
    End Sub

#End Region

    Protected Sub ToolBarSearch_ButtonClick(ByVal sender As Object, ByVal e As RadToolBarEventArgs) Handles ToolBarSearch.ButtonClick
        Dim btn As RadToolBarButton = TryCast(e.Item, RadToolBarButton)

        If btn.Value = "search" Then
            LoadProperties(SearchAccountTextBox.Text)
        Else
        End If
    End Sub

End Class