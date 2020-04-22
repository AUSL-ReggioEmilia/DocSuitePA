Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class uscFascicleFolders
    Inherits DocSuite2008BaseControl

#Region " Fields "

#End Region

#Region " Properties "
    Public ReadOnly Property PageContentDiv As RadPageLayout
        Get
            Return pageContent
        End Get
    End Property

    Public Property IsVisibile As Boolean

    Public Property ViewOnlyFolders As Boolean

    Protected ReadOnly Property FoldersToDisabledSerialized As String
        Get
            If FoldersToDisabled.IsNullOrEmpty() Then
                Return JsonConvert.SerializeObject(New List(Of Guid))
            End If
            Return JsonConvert.SerializeObject(FoldersToDisabled)
        End Get
    End Property

    Public Property FoldersToDisabled As ICollection(Of Guid)

    Public Property DoNotUpdateDatabase As Boolean
#End Region

#Region " Events"

    Protected Sub uscFascicleFolders_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            If ViewOnlyFolders Then
                pnlTitle.SetDisplay(False)
                FolderToolBar.SetDisplay(False)
            End If
        End If
    End Sub

    Protected Sub uscFascicleFoldersAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)

    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf uscFascicleFoldersAjaxRequest
    End Sub

#End Region

End Class