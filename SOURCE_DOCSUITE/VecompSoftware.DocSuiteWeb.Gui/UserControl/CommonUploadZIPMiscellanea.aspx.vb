Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Fascicles
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.Compress
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging

Public Class CommonUploadZIPMiscellanea
    Inherits CommonBasePage

#Region " Fields "

    Public Const ADD_ACTION As String = "Add"
    Private _location As Location
    Private Const CONFIRM_CALLBACK As String = "commonUploadZIPMiscellanea.confirmCallback('{0}')"

#End Region

#Region " Properties "

    Public ReadOnly Property DocumentUpload As uscDocumentUpload
        Get
            Return uscDocumentUpload
        End Get
    End Property

    Public ReadOnly Property IdArchiveChain As Guid?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Guid)("IdArchiveChain", Nothing)
        End Get
    End Property

    Public ReadOnly Property IdMiscellaneaLocation As Integer?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Integer)("IdLocation", Nothing)
        End Get
    End Property

    Protected ReadOnly Property MiscellaneaLocation As Location
        Get
            If _location Is Nothing Then
                _location = Facade.LocationFacade.GetById(IdMiscellaneaLocation.Value)
            End If
            Return _location
        End Get
    End Property

    Public ReadOnly Property ArchiveName As String
        Get
            Return Request.QueryString.GetValueOrDefault(Of String)("ArchiveName", String.Empty)
        End Get
    End Property

    Public ReadOnly Property MultiDoc As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("MultiDoc", False)
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
    End Sub

    Protected Sub btnConfirm_OnClick(sender As Object, e As EventArgs) Handles btnSave.Click
        Try
            If DocumentUpload.DocumentsCount = 0 Then
                AjaxAlert("Nessun documento presente. Inserire almeno un documento.")
                Exit Sub
            End If

            Dim chainId As Guid = If(IdArchiveChain.Equals(Guid.Empty) AndAlso Action.Eq(ADD_ACTION), Guid.Empty, IdArchiveChain.Value)
            If DocumentUpload.DocumentInfosAdded.Count > 0 Then
                Dim addedZipDocument As DocumentInfo = DocumentUpload.DocumentInfos.First()
                Dim zipCompress As ZipCompress = New ZipCompress()
                Dim extractedFiles As List(Of CompressItem) = zipCompress.InMemoryExtract(New MemoryStream(addedZipDocument.Stream))
                Dim counter As Integer = 1
                For Each extractedFile As CompressItem In extractedFiles
                    Dim memoryInfo As New MemoryDocumentInfo(extractedFile.Data, extractedFile.Filename)
                    memoryInfo.AddAttribute(BiblosFacade.REGISTRATION_USER_ATTRIBUTE, DocSuiteContext.Current.User.FullUserName)

                    memoryInfo.Name = If(txtPrefix.Text <> String.Empty, String.Format("{0}{1}{2}", txtPrefix.Text, counter.ToString("000"), If(memoryInfo.Name.EndsWith(FileHelper.P7M, StringComparison.InvariantCultureIgnoreCase), FileHelper.GetAllExtensions(memoryInfo.Name), memoryInfo.Extension)), memoryInfo.Name)

                    Dim savedTemplate As BiblosDocumentInfo = SaveBiblosDocument(memoryInfo, chainId)
                    chainId = savedTemplate.ChainId
                    counter += 1
                Next
                AjaxManager.ResponseScripts.Add(String.Format(CONFIRM_CALLBACK, chainId))
            End If
        Catch ex As Exception
            FileLogger.Error(LoggerName, "Errore durante il salvataggio del documento su Biblos", ex)
            AjaxAlert("Errore durante il salvataggio del documento su Biblos")
        End Try
    End Sub

    Private Sub uscDocumentUpload_DocumentUploaded(sender As Object, e As DocumentEventArgs) Handles uscDocumentUpload.DocumentUploaded
        If Not FileHelper.MatchExtension(e.Document.Name, FileHelper.ZIP) Then
            AjaxAlert("E' possibile processare solo file con estensione .zip")
            uscDocumentUpload.RemoveDocumentInfo(e.Document)
            Return
        End If
    End Sub
#End Region

#Region " Methods "

    Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(uscDocumentUpload, uscDocumentUpload)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSave, btnSave)
    End Sub

    Private Function SaveBiblosDocument(document As DocumentInfo, chainId As Guid) As BiblosDocumentInfo
        If MiscellaneaLocation Is Nothing Then
            AjaxAlert("Nessun location presente con questo ID. Errore di configurazione, contattatare Assistenza.")
            AjaxManager.ResponseScripts.Add(String.Format(CONFIRM_CALLBACK, String.Empty))
            Exit Function
        End If

        If String.IsNullOrEmpty(ArchiveName) Then
            AjaxAlert("Nessun archivio defininito. Errore di configurazione, contattatare Assistenza.")
            AjaxManager.ResponseScripts.Add(String.Format(CONFIRM_CALLBACK, String.Empty))
            Exit Function
        End If

        Dim storedBiblosDocumentInfo As BiblosDocumentInfo = document.ArchiveInBiblos(MiscellaneaLocation.DocumentServer, ArchiveName, chainId)
        Return storedBiblosDocumentInfo
    End Function

#End Region

End Class