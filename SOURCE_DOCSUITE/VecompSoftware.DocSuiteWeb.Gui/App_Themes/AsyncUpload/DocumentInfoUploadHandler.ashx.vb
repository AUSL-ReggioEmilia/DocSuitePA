Imports System.Web
Imports System.IO
Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers
Imports Telerik.Web.UI
Imports VecompSoftware.Services.Biblos.Models

Public Class DocumentInfoUploadHandler
    Inherits AsyncUploadHandler
    Implements SessionState.IRequiresSessionState

    Protected Overrides Function Process(ByVal file As UploadedFile, ByVal context As HttpContext, ByVal configuration As IAsyncUploadConfiguration, ByVal tempFileName As String) As IAsyncUploadResult

        Dim result As DocumentInfoUploadResult = CreateDefaultUploadResult(Of DocumentInfoUploadResult)(file)

        If TypeOf configuration Is BiblosDocumentInfoUploadConfiguration Then
            Dim config As BiblosDocumentInfoUploadConfiguration = TryCast(configuration, BiblosDocumentInfoUploadConfiguration)
            Dim doc As New MemoryDocumentInfo(file.InputStream.ToByteArray(), file.GetName())
            Dim tor As BiblosDocumentInfo = doc.ArchiveInBiblos(config.Server, config.Archive)

            result.DocumentInfoSerialized = HttpUtility.UrlEncode(tor.ToQueryString().AsEncodedQueryString())

        ElseIf TypeOf configuration Is TempFileDocumentInfoUploadConfiguration Then
            Dim config As TempFileDocumentInfoUploadConfiguration = TryCast(configuration, TempFileDocumentInfoUploadConfiguration)
            Dim fileName As String = Path.GetFileName(file.GetName())
            Dim targetFileName As String = FileHelper.UniqueFileNameFormat(fileName, config.Owner)
            Dim targetPath As String = Path.Combine(config.TempFolder, targetFileName)
            file.SaveAs(targetPath, True)

            Dim doc As New TempFileDocumentInfo(New FileInfo(targetPath))
            doc.Name = file.GetName()

            result.DocumentInfoSerialized = HttpUtility.UrlEncode(doc.ToQueryString().AsEncodedQueryString())
        End If


        Return result
    End Function

End Class