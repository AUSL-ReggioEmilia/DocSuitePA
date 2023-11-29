Imports System.Collections.Generic
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Entity.Fascicles
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Services.Logging

Partial Public Class CommonSelMiscellanea
    Inherits CommBasePage

#Region " Fields "
    Public Const ADD_ACTION As String = "Add"
    Public Const EDIT_ACTION As String = "Edit"
    Private Const CONFIRM_CALLBACK As String = "commonSelMiscellanea.confirmCallback('{0}')"
    Private Const UPDATE_FASCICLEDOCUMENT As String = "Modificato documento con nome {0} ed id {1}. Nome: {2}, Id {3}, Note {4}"
    Private Const INSERT_FASCICLEDOCUMENT As String = "Inserito documento. Nome: {0}, Id {1}"
    Private _location As Location
    Private _fascicleLogFacade As Facade.WebAPI.Fascicles.FascicleLogFacade

#End Region

#Region " Properties "
    Public ReadOnly Property MultiDoc As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault("MultiDoc", False)
        End Get
    End Property

    Public ReadOnly Property PageContentDiv As RadPageLayout
        Get
            Return insertsPageContent
        End Get
    End Property

    Public ReadOnly Property DocumentUpload As uscDocumentUpload
        Get
            Return uscDocumentUpload
        End Get
    End Property

    Public ReadOnly Property IdMiscellaneaLocation As Integer?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Integer)("IdLocation", Nothing)
        End Get
    End Property

    Public ReadOnly Property IdDocument As Guid?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Guid)("IdDocument", Guid.Empty)
        End Get
    End Property

    Public ReadOnly Property DocumentName As String
        Get
            Return Request.QueryString.GetValueOrDefault(Of String)("DocumentName", String.Empty)
        End Get
    End Property

    Public ReadOnly Property IdArchiveChain As Guid?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Guid)("IdArchiveChain", Nothing)
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

    Protected ReadOnly Property Environment As DSWEnvironment
        Get
            Return Request.QueryString.GetValueOrDefault(Of DSWEnvironment)("Environment", DSWEnvironment.Any)
        End Get
    End Property

    Public ReadOnly Property DocumentUnitId As Guid?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Guid)("DocumentUnitId", Nothing)
        End Get
    End Property

    Public ReadOnly Property FascicleLogFacade As Facade.WebAPI.Fascicles.FascicleLogFacade
        Get
            If _fascicleLogFacade Is Nothing Then
                _fascicleLogFacade = New Facade.WebAPI.Fascicles.FascicleLogFacade(DocSuiteContext.Current.Tenants, Nothing)
            End If
            Return _fascicleLogFacade
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        Page.DataBind()
        If Not IsPostBack() Then
            Initialize()
        End If
    End Sub

    Protected Sub btnConfirm_OnClick(sender As Object, e As EventArgs) Handles btnSave.Click
        Try
            Dim senderControl As RadButton = DirectCast(sender, RadButton)
            If DocumentUpload.DocumentsCount = 0 Then
                AjaxAlert("Nessun documento presente. Inserire almeno un documento.")
                Exit Sub
            End If

            Dim chainId As Guid = If(IdArchiveChain.Equals(Guid.Empty) AndAlso Action.Eq(ADD_ACTION), Guid.Empty, IdArchiveChain.Value)
            If DocumentUpload.DocumentInfosAdded.Count > 0 OrElse NoteAttributeHasChanged() Then
                For Each addedDocument As DocumentInfo In DocumentUpload.DocumentInfos
                    addedDocument.AddAttribute(BiblosFacade.NOTE_ATTRIBUTE, txtNote.Text)
                    addedDocument.AddAttribute(BiblosFacade.REGISTRATION_USER_ATTRIBUTE, DocSuiteContext.Current.User.FullUserName)
                    Dim biblosFunc As Func(Of DocumentInfo, BiblosDocumentInfo) = If(Action.Eq(ADD_ACTION), Function(d As DocumentInfo) SaveBiblosDocument(d, chainId), Function(d As DocumentInfo) UpdateBiblosDocument(d))
                    Dim savedTemplate As BiblosDocumentInfo = biblosFunc(addedDocument)
                    chainId = savedTemplate.ChainId


                    If Environment.Equals(DSWEnvironment.Fascicle) Then
                        If Action.Eq(ADD_ACTION) Then
                            FascicleLogFacade.InsertFascicleDocumentLog(DocumentUnitId, FascicleLogType.Insert, String.Format(INSERT_FASCICLEDOCUMENT, savedTemplate.Name, savedTemplate.DocumentId))
                        Else
                            FascicleLogFacade.InsertFascicleDocumentLog(DocumentUnitId, FascicleLogType.Insert, String.Format(UPDATE_FASCICLEDOCUMENT, DocumentName, IdDocument, savedTemplate.Name, savedTemplate.DocumentId, txtNote.Text))
                        End If
                    End If
                Next
            End If
            AjaxManager.ResponseScripts.Add(String.Format(CONFIRM_CALLBACK, chainId))
        Catch ex As Exception
            FileLogger.Error(LoggerName, "Errore durante il salvataggio del documento su Biblos", ex)
            AjaxAlert("Errore durante il salvataggio del documento su Biblos")
        End Try
    End Sub


#End Region

#Region " Methods "
    Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(uscDocumentUpload, uscDocumentUpload)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSave, btnSave)
    End Sub

    Private Sub Initialize()
        Dim documentId As Guid = IdDocument.Value
        Dim idchain As Guid = IdArchiveChain.Value
        If Action.Eq(EDIT_ACTION) Then
            LoadDocuments(documentId, idchain)
        End If
    End Sub
    Private Function UpdateBiblosDocument(document As DocumentInfo) As BiblosDocumentInfo
        Try
            Dim toUpdate As BiblosDocumentInfo = BiblosDocumentInfo.GetDocumentInfo(IdDocument.Value, Nothing, True).First()
            toUpdate.Name = document.Name
            toUpdate.Stream = document.Stream
            toUpdate.Attributes(BiblosFacade.NOTE_ATTRIBUTE) = txtNote.Text
            toUpdate.Attributes(BiblosFacade.REGISTRATION_USER_ATTRIBUTE) = DocSuiteContext.Current.User.FullUserName
            Dim newDocument As BiblosDocumentInfo = Service.UpdateDocument(toUpdate, DocSuiteContext.Current.User.FullUserName)
            Return newDocument
        Catch ex As Exception
            FileLogger.Error(LoggerName, ex.Message, ex)
            Throw ex
        End Try
    End Function

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

        Dim storedBiblosDocumentInfo As BiblosDocumentInfo = document.ArchiveInBiblos(ArchiveName, chainId)
        Return storedBiblosDocumentInfo
    End Function

    Private Sub LoadDocuments(idDocument As Guid, idChain As Guid)
        Dim docsList As IList(Of BiblosDocumentInfo) = New List(Of BiblosDocumentInfo)
        Dim docInfo As BiblosDocumentInfo = New BiblosDocumentInfo(idDocument, idChain)
        DocumentUpload.LoadDocumentInfo(docInfo, False, True, False, False)
        If docInfo.GetAttributeValue(BiblosFacade.NOTE_ATTRIBUTE) IsNot Nothing AndAlso Not String.IsNullOrEmpty(docInfo.Attributes(BiblosFacade.NOTE_ATTRIBUTE)) Then
            txtNote.Text = docInfo.GetAttributeValue(BiblosFacade.NOTE_ATTRIBUTE).ToString()
        End If
    End Sub

    Private Function NoteAttributeHasChanged() As Boolean
        If Action.Eq(ADD_ACTION) Then
            Return True
        End If
        Dim result As Boolean = False
        If DocumentUpload.DocumentInfos.Count > 0 Then
            Dim doc As DocumentInfo = DocumentUpload.DocumentInfos.First()
            If doc.Attributes.Any(Function(a) a.Key.Eq(BiblosFacade.NOTE_ATTRIBUTE)) Then
                If Not doc.Attributes(BiblosFacade.NOTE_ATTRIBUTE).Eq(txtNote.Text) Then
                    Return True
                End If
                Return False
            End If
            Return True
        End If
        Return result
    End Function
#End Region

End Class