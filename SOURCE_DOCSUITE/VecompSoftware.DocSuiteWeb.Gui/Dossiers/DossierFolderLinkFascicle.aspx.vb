Imports System.Web
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class DossierFolderLinkFascicle
    Inherits DossierBasePage

#Region " Fields "

    Private _idDossierFolder As Guid?

    Private Const DOSSIERFOLDER_INSERT_CALLBACK As String = "dossierFolderLinkFascicle.insertCallback('{0}');"
    Private Const CATEGORY_CHANGE_HANDLER As String = "dossierFolderLinkFascicle.onCategoryChanged({0});"
#End Region

#Region " Properties "

    Public ReadOnly Property PageContentDiv As RadPageLayout
        Get
            Return pageContent
        End Get
    End Property

    Public ReadOnly Property IdDossierFolder As Guid
        Get
            If _idDossierFolder Is Nothing Then
                _idDossierFolder = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Guid?)("IdDossierFolder", Nothing)
            End If
            If _idDossierFolder.HasValue Then
                Return _idDossierFolder.Value
            Else
                Return Guid.Empty
            End If
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()

        If Not IsPostBack() Then
            Initialize()
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma)
    End Sub

    Private Sub Initialize()
    End Sub

#End Region

End Class