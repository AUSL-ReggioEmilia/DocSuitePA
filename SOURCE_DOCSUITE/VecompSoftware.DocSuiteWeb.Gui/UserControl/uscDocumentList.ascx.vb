Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Biblos
Imports Telerik.Web.UI
Imports System.Web
Imports VecompSoftware.Services.Biblos.Models

Public Class uscDocumentList
    Inherits DocSuite2008BaseControl

#Region " Fields "

    Private _caption As String
    Private _documents As IList(Of DocumentInfo)

#End Region

#Region " Properties "

    ''' <summary>
    ''' Contiene i documenti che devono essere visualizzati in tabella
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected ReadOnly Property Documents As IList(Of DocumentInfo)
        Get
            If _documents Is Nothing Then
                If ViewState("Documents") Is Nothing Then
                    ViewState("Documents") = New String(-1) {}
                End If
                _documents = (From s In CType(ViewState("Documents"), String()) Select DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(s))).ToList()
            End If
            Return _documents
        End Get
    End Property

    ''' <summary>
    ''' Contiene un elenco di tipologie di output che devono essere selezionate di default all'avvio
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CheckedDefaultOutputTypes As IList(Of OutputType) = New List(Of OutputType)()

    ''' <summary>
    ''' Contiene un mapping tra le estensioni (key) e l'OutputType da impostare di default
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CheckedOutputTypeByExtension As Dictionary(Of String, OutputType) = New Dictionary(Of String, OutputType)()

    ''' <summary>
    ''' Definisce se devono essere ignorare le impostazioni di autocheck
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UncheckAll As Boolean

    Public Property Caption As String
        Get
            If String.IsNullOrEmpty(_caption) Then
                If ViewState("Caption") IsNot Nothing Then
                    _caption = CType(ViewState("Caption"), String)
                End If
            End If
            Return _caption
        End Get
        Set(ByVal value As String)
            _caption = value
            ViewState("Caption") = value
        End Set
    End Property

    ''' <summary>
    ''' Calcola la grandezza totale dello UserControl
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property TotalSize As Long
        Get
            Return GetSelectedDocuments().Sum(Function(documentInfo) documentInfo.Size)
        End Get
    End Property

    ''' <summary>
    ''' Definisce se deve essere mostrata la grandezza di ogni allegato
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ShowDocumentsSize As Boolean

    ''' <summary>
    ''' Definisce se deve essere mostrata la grandezza totale degli allegati
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ShowTotalSize As Boolean

    Public ReadOnly Property DocumentsCount As Integer
        Get
            Return Documents.Count
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        ''Comportamenti di default:
        '' Tutte le stame confommi sempre selezionati
        CheckedDefaultOutputTypes.Add(OutputType.PrintVersion)

        '' p7m selezionati sempre anche in Copia conforme
        CheckedOutputTypeByExtension.Add(FileHelper.P7M, OutputType.Original)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
    End Sub

    Protected Sub RadAjaxManagerAjaxRequest(sender As Object, e As AjaxRequestEventArgs)
        If e.Argument.Eq("UpdateTotalSize") Then
            UpdateTotalSize()
        End If
    End Sub

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf RadAjaxManagerAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, DocumentListGrid)
        AjaxManager.AjaxSettings.AddAjaxSetting(DocumentListGrid, DocumentsCaption)
        AjaxManager.AjaxSettings.AddAjaxSetting(DocumentListGrid, DocumentListGrid, DefaultLoadingPanel)
    End Sub

    Protected Sub CheckChanged(sender As Object, e As EventArgs)
        UpdateTotalSize()
    End Sub

    Protected Sub DocumentListGridItemDataBound(ByVal sender As System.Object, ByVal e As GridItemEventArgs) Handles DocumentListGrid.ItemDataBound
        If e.Item.ItemType <> GridItemType.Item AndAlso e.Item.ItemType <> GridItemType.AlternatingItem Then
            Exit Sub
        End If

        '' Documento da processare
        Dim item As DocumentInfo = DirectCast(e.Item.DataItem, DocumentInfo)
        '' icona
        Dim documentType As ImageButton = DirectCast(e.Item.FindControl("documentType"), ImageButton)
        documentType.ImageUrl = ImagePath.FromDocumentInfo(item)
        '' nome del file
        Dim lblFileName As Label = DirectCast(e.Item.FindControl("lblFileName"), Label)
        lblFileName.Text = item.Caption
        '' aggiungo il size se richiesto
        If ShowDocumentsSize Then
            lblFileName.Text &= String.Format(" ({0})", item.Size.ToByteFormattedString(0))
        End If

        '' Colonna Originale
        Dim chkOriginal As CheckBox = DirectCast(e.Item.FindControl("chkOriginal"), CheckBox)
        If Not ProtocolEnv.SendPECDocumentEnabled Then
            chkOriginal.Enabled = False
        End If
        Dim chkOriginalChecked As Boolean = Not UncheckAll AndAlso
            (CheckedDefaultOutputTypes.Contains(OutputType.Original) _
             OrElse
             (CheckedOutputTypeByExtension.ContainsKey(item.Extension.ToLower()) AndAlso
              CheckedOutputTypeByExtension(item.Extension.ToLower()).Equals(OutputType.Original)) _
             OrElse item.IsSigned)
        chkOriginal.Checked = chkOriginalChecked

        '' Colonna Copia conforme
        Dim chkPdf As CheckBox = DirectCast(e.Item.FindControl("chkPdf"), CheckBox)
        If Not ProtocolEnv.SendPECDocumentEnabled Then
            chkPdf.Enabled = False
        End If
        Dim chkPdfChecked As Boolean = Not UncheckAll AndAlso
            (CheckedDefaultOutputTypes.Contains(OutputType.PrintVersion) _
             OrElse
            (CheckedOutputTypeByExtension.ContainsKey(item.Extension.ToLower()) AndAlso
             CheckedOutputTypeByExtension(item.Extension.ToLower()).Equals(OutputType.PrintVersion)))
        chkPdf.Checked = chkPdfChecked
    End Sub

    Private Sub DocumentListGrid_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs) Handles DocumentListGrid.NeedDataSource
        If Not Documents.IsNullOrEmpty() Then
            DocumentListGrid.DataSource = Documents
        End If
        UpdateTotalSize()
    End Sub

    Protected Sub DocumentListGridCommand(ByVal sender As System.Object, ByVal e As GridCommandEventArgs) Handles DocumentListGrid.ItemCommand
        If Not e.CommandName.Eq("preview") Then
            Exit Sub
        End If

        Dim document As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(DirectCast(e.Item, GridDataItem).GetDataKeyValue("Serialized").ToString()))
        AjaxManager.ResponseScripts.Add(String.Format("OpenGenericWindow('../Viewers/DocumentInfoViewer.aspx?{0}')", document.ToQueryString().AsEncodedQueryString()))
    End Sub

    Public Overloads Sub DataBind()
        If Not Documents.IsNullOrEmpty() Then
            '' Salvo in ViewState i documenti
            ViewState("Documents") = (From documentInfo In Documents Select documentInfo.ToQueryString().AsEncodedQueryString).ToArray()

            '' Carico la griglia
            DocumentListGrid.DataSource = Documents
            DocumentListGrid.DataBind()
        Else
            DocumentsPanel.Visible = False
        End If

        UpdateTotalSize()
    End Sub

    Public Sub LoadDocumentInfo(doc As DocumentInfo)
        Documents.Add(doc)
    End Sub

    Public Sub LoadDocumentInfos(docs As IList(Of DocumentInfo))
        For Each doc As DocumentInfo In docs
            LoadDocumentInfo(doc)
        Next
    End Sub

    Public Function GetSelectedDocuments() As IList(Of DocumentInfo)
        ' Allego i documenti
        Dim tor As New List(Of DocumentInfo)
        For Each item As GridDataItem In DocumentListGrid.MasterTableView.Items()
            Dim originalChecked As Boolean = DirectCast(item.FindControl("chkOriginal"), CheckBox).Checked
            Dim pdfChecked As Boolean = DirectCast(item.FindControl("chkPdf"), CheckBox).Checked
            '' Se c'è almeno 1 dei 2 check allora considero l'elemento
            If originalChecked OrElse pdfChecked Then
                '' Rigenero il documento                
                Dim document As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(CType(item.GetDataKeyValue("Serialized"), String)))
                If originalChecked Then
                    Dim docInMemory As MemoryDocumentInfo = BiblosDocumentInfo.CopyDocumentInMemory(document)
                    tor.Add(docInMemory)
                End If
                If pdfChecked Then
                    tor.Add(New BiblosPdfDocumentInfo(CType(document, BiblosDocumentInfo)))
                End If
            End If
        Next
        Return tor
    End Function

    Public Sub RemoveDocumentInfo(doc As DocumentInfo)
        For Each item As GridDataItem In DocumentListGrid.Items()
            If item.GetDataKeyValue("Serialized").Equals(doc.ToQueryString()) Then
                DirectCast(item.FindControl("chkOriginal"), CheckBox).Checked = False
                DirectCast(item.FindControl("chkPdf"), CheckBox).Checked = False
                Exit For
            End If
        Next

        UpdateTotalSize()
    End Sub

    Public Sub SetDocument(caption As String)
        DocumentsCaption.Text = caption
    End Sub

    Private Sub UpdateTotalSize()
        '' Aggiorno il label se richiesto
        If ShowTotalSize Then
            Dim size As Long = TotalSize
            If size > 0 Then
                '' Aggiorno la root dei nodi con la grandezza aggiornata
                DocumentsCaption.Text = String.Format("{0} ({1})", Caption, size.ToByteFormattedString(0))
            Else
                '' Se la grandezza è 0 ripristino la scritta originale
                DocumentsCaption.Text = Caption
            End If
        End If
    End Sub

#End Region

#Region " Enums "

    Public Enum OutputType
        Original = 0
        PrintVersion = 1
    End Enum

#End Region

End Class