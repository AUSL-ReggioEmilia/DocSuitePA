Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging

Public Class TemplateDocumentViewer
    Inherits CommBasePage

#Region " Fields "
    Private Const HIDE_LOADING_FUNCTION As String = "hideLoadingPanel()"
    Private Const SHOW_LOADING_FUNCTION As String = "showLoadingPanel()"
#End Region

#Region " Properties "
    Public ReadOnly Property IsPreview As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("IsPreview", False)
        End Get
    End Property

    Public ReadOnly Property IdChain As Guid
        Get
            Return Request.QueryString.GetValueOrDefault("IdChain", Guid.Empty)
        End Get
    End Property

    Public ReadOnly Property LabelName As String
        Get
            Return Request.QueryString.GetValueOrDefault("Label", String.Empty)
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub btnEnablePreview_Click(sender As Object, e As EventArgs) Handles btnEnablePreview.Click
        Try
            BindViewerLight()
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            AjaxAlert("Errore in recupero documenti: {0}", ex.Message)
        Finally
            AjaxManager.ResponseScripts.Add(HIDE_LOADING_FUNCTION)
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
            If IsPreview Then
                ViewerLight.ToolBarVisible = False
                ViewerLight.LeftPaneStartWidth = 0
                ViewerLight.LeftPaneVisible = False
                AjaxManager.ResponseScripts.Add(SHOW_LOADING_FUNCTION)
            Else
                btnEnablePreview_Click(sender, e)
            End If
        End If
    End Sub
#End Region

#Region " Methods "
    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnEnablePreview, ViewerLight)
    End Sub

    Private Sub BindViewerLight()
        Dim docs As List(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(String.Empty, IdChain)
        Dim temp As List(Of DocumentInfo) = docs.Cast(Of DocumentInfo)().ToList()

        Dim main As New FolderInfo() With {.Name = LabelName}
        main.AddChildren(temp)

        ViewerLight.DataSource = New List(Of DocumentInfo) From {main}
        ViewerLight.DataBind()
    End Sub
#End Region
End Class