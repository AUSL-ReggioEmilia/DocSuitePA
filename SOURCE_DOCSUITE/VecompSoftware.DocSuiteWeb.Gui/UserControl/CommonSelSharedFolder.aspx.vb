Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Helpers.ExtensionMethods
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Newtonsoft.Json
Imports System.Web
Imports VecompSoftware.Services.Biblos.Models

Partial Public Class CommonSelSharedFolder
    Inherits CommBasePage

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        MasterDocSuite.TitleVisible = False

        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub RadGridFiles_ItemCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles RadGridFiles.ItemCommand
        If e.CommandName.Eq("Select") Then
            Dim sharedFile As New FileInfo(DocSuiteContext.Current.ProtocolEnv.ImportSharedFolder & RadGridFiles.Items(e.Item.ItemIndex)("cName").Text)
            SelectFile(sharedFile)
        End If
    End Sub

    Private Sub RadGridFiles_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles RadGridFiles.ItemDataBound
        If e.Item.ItemType <> GridItemType.Item AndAlso e.Item.ItemType <> GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim item As SharedFile = DirectCast(e.Item.DataItem, SharedFile)

        With DirectCast(e.Item.FindControl("btnSelFile"), RadButton)
            .Image.ImageUrl = ImagePath.FromFile(item.Name)
        End With
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        Dim impersonator As Impersonator = CommonAD.ImpersonateSuperUser()
        Dim sharedFolder As New DirectoryInfo(DocSuiteContext.Current.ProtocolEnv.ImportSharedFolder)
        Dim files As List(Of FileInfo) = sharedFolder.GetFiles().Where(Function(file) ((file.Attributes And FileAttributes.Hidden) <> FileAttributes.Hidden) AndAlso (file.Attributes And FileAttributes.System) <> FileAttributes.System).ToList()
        impersonator.ImpersonationUndo()

        Dim fileList As New List(Of SharedFile)
        For Each fileInfo As FileInfo In files
            Dim file As New SharedFile()
            file.Name = fileInfo.Name
            file.Dimension = (Int(fileInfo.Length / 1024) + 1).ToString() + " KB"
            file.CreationTime = Format(fileInfo.CreationTime, "dd/MM/yyyy hh:mm:ss")
            file.LastWriteTime = Format(fileInfo.LastWriteTime, "dd/MM/yyyy hh:mm:ss")
            fileList.Add(file)
        Next

        RadGridFiles.DataSource = fileList
        RadGridFiles.DataBind()
    End Sub

    Private Sub SelectFile(ByRef sharedFile As FileInfo)
        Dim impersonator As Impersonator = Nothing
        Try
            CommonShared.SelectedSharedFile = sharedFile

            impersonator = CommonAD.ImpersonateSuperUser()
            If Not sharedFile.Exists Then
                AjaxAlert("Documento non valido")
                Exit Sub
            End If

            Dim uniquename As String = FileHelper.UniqueFileNameFormat(sharedFile.Name, DocSuiteContext.Current.User.UserName)
            Dim doc As New TempFileDocumentInfo(sharedFile.CopyTo(CommonInstance.AppTempPath & uniquename))
            doc.Name = sharedFile.Name

            Dim list As New Dictionary(Of String, String)
            list.Add(doc.FileInfo.Name, sharedFile.Name)
            Dim serialized As String = JsonConvert.SerializeObject(list)
            Dim jsStringEncoded As String = HttpUtility.JavaScriptStringEncode(serialized)
            MasterDocSuite.AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}');", jsStringEncoded))

        Catch ex As Exception
            FileLogger.Error(LoggerName, "Errore in selezione file su Cartella Condivisa", ex)
            AjaxAlert("Errore selezione file, contattare l'assistenza.")
            Exit Sub
        Finally
            If impersonator IsNot Nothing Then
                impersonator.ImpersonationUndo()
            End If

        End Try

    End Sub

#End Region

End Class