Imports System.Collections.Generic
Imports System.IO
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports documentModelAPI = VecompSoftware.DocSuiteWeb.Model.Entities.Commons.DocumentModel

Public Class uscFascicleFolders
    Inherits DocSuite2008BaseControl

#Region " Fields "
    Private Const BIND_MISCELLANEA As String = "uscFascicleFolders.bindMiscellanea('{0}');"
#End Region

#Region " Properties "
    Public Property OnlySignEnabled As Boolean
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
    Public ReadOnly Property ArchiveName As String
        Get
            Return Facade.LocationFacade.GetById(ProtocolEnv.FascicleMiscellaneaLocation).ProtBiblosDSDB
        End Get
    End Property

    Public ReadOnly Property ScannerLightRestEnabled As Boolean
        Get
            Return ProtocolEnv.ScannerLightRestEnabled
        End Get
    End Property
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
        Dim model As String = e.Argument
        Dim arg As AjaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(model)
        Select Case arg.ActionName
            Case "Upload_document"
                Dim deserialized As Dictionary(Of String, String) = JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(arg.Value(0))
                For Each item As KeyValuePair(Of String, String) In deserialized
                    Dim name As String = item.Value
                    Dim chainId As Guid = If(arg.Value(1) Is Nothing, Guid.Empty, Guid.Parse(arg.Value(1)))
                    Dim biblosFunc As Func(Of DocumentInfo, BiblosDocumentInfo) = Function(d As DocumentInfo) SaveBiblosDocument(d, chainId)
                    Dim doc As New TempFileDocumentInfo(name, New FileInfo(Path.Combine(CommonUtil.GetInstance().AppTempPath, item.Key)))
                    Dim savedTemplate As BiblosDocumentInfo = biblosFunc(doc)
                    chainId = savedTemplate.ChainId

                    AjaxManager.ResponseScripts.Add(String.Format(BIND_MISCELLANEA, chainId))
                Next
            Case "Scan_document"
                Dim documents As List(Of documentModelAPI) = JsonConvert.DeserializeObject(Of List(Of documentModelAPI))(arg.Value(0))
                Dim tempDoc As TempFileDocumentInfo
                Dim pathInfo As String
                For Each document As documentModelAPI In documents
                    pathInfo = Path.Combine(CommonUtil.GetInstance().TempDirectory.FullName, FileHelper.UniqueFileNameFormat(document.FileName, DocSuiteContext.Current.User.UserName))
                    File.WriteAllBytes(pathInfo, document.ContentStream)
                    tempDoc = New TempFileDocumentInfo(document.FileName, New FileInfo(pathInfo))
                    Dim chainId As Guid = If(arg.Value(1) Is Nothing, Guid.Empty, Guid.Parse(arg.Value(1)))
                    Dim biblosFunc As Func(Of DocumentInfo, BiblosDocumentInfo) = Function(d As DocumentInfo) SaveBiblosDocument(d, chainId)
                    Dim savedTemplate As BiblosDocumentInfo = biblosFunc(tempDoc)
                    chainId = savedTemplate.ChainId

                    AjaxManager.ResponseScripts.Add(String.Format(BIND_MISCELLANEA, chainId))
                Next
        End Select
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf uscFascicleFoldersAjaxRequest
    End Sub

    Private Function SaveBiblosDocument(document As DocumentInfo, chainId As Guid) As BiblosDocumentInfo
        Dim storedBiblosDocumentInfo As BiblosDocumentInfo = document.ArchiveInBiblos(ArchiveName, chainId)
        Return storedBiblosDocumentInfo
    End Function

#End Region

End Class