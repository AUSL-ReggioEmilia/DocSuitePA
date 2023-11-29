Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Facade.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Biblos
Imports Telerik.Web.UI
Imports System.Web
Imports VecompSoftware.Services.Biblos.Models
Imports System.IO

Public Class uscDocumentList
    Inherits DocSuite2008BaseControl

#Region " Fields "

    Private _caption As String
    Private _documents As IList(Of DocumentInfo)

#End Region

#Region " Properties "

    ''' <summary>
    ''' Rappresenta il data source utilizzato in fase di data binding della griglia
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected ReadOnly Property DataSource As IList(Of Tuple(Of DocumentInfo, DocumentInfoLoadOptions)) = New List(Of Tuple(Of DocumentInfo, DocumentInfoLoadOptions))

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
            Return DocumentListGrid.MasterTableView.Items.Count
        End Get
    End Property

    Public Property DocumentSelectionEnabled As Boolean = True

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

    Protected Sub CheckedSelectAllOriginalsChanged(sender As Object, e As EventArgs)
        For Each item As GridDataItem In DocumentListGrid.MasterTableView.Items()
            Dim chkOriginal As CheckBox = DirectCast(item.FindControl("chkOriginal"), CheckBox)
            chkOriginal.Checked = chkSelectAllOriginals.Checked
        Next
    End Sub

    Protected Sub CheckedSelectAllCertifiedCopiesChanged(sender As Object, e As EventArgs)
        For Each item As GridDataItem In DocumentListGrid.MasterTableView.Items()
            Dim chkPdf As CheckBox = DirectCast(item.FindControl("chkPdf"), CheckBox)
            chkPdf.Checked = chkSelectAllCertifiedCopies.Checked
        Next
    End Sub

    Protected Sub DocumentListGridItemDataBound(ByVal sender As System.Object, ByVal e As GridItemEventArgs) Handles DocumentListGrid.ItemDataBound
        If e.Item.ItemType <> GridItemType.Item AndAlso e.Item.ItemType <> GridItemType.AlternatingItem Then
            Exit Sub
        End If

        '' Documento da processare
        Dim item As Tuple(Of DocumentInfo, DocumentInfoLoadOptions) = DirectCast(e.Item.DataItem, Tuple(Of DocumentInfo, DocumentInfoLoadOptions))
        '' icona
        Dim documentType As ImageButton = DirectCast(e.Item.FindControl("documentType"), ImageButton)
        documentType.ImageUrl = ImagePath.FromDocumentInfo(item.Item1)
        '' nome del file
        Dim lblFileName As Label = DirectCast(e.Item.FindControl("lblFileName"), Label)
        lblFileName.Text = item.Item1.Caption
        '' aggiungo il size se richiesto
        If ShowDocumentsSize Then
            lblFileName.Text &= String.Format(" ({0})", item.Item1.Size.ToByteFormattedString(0))
        End If

        '' Colonna Originale
        Dim chkOriginal As CheckBox = DirectCast(e.Item.FindControl("chkOriginal"), CheckBox)
        chkOriginal.Enabled = DocumentSelectionEnabled

        If item.Item2.Selected Then
            Dim chkOriginalChecked As Boolean = Not UncheckAll AndAlso
            (CheckedDefaultOutputTypes.Contains(OutputType.Original) _
             OrElse
             (CheckedOutputTypeByExtension.ContainsKey(item.Item1.Extension.ToLower()) AndAlso
              CheckedOutputTypeByExtension(item.Item1.Extension.ToLower()).Equals(OutputType.Original)) _
             OrElse item.Item1.IsSigned)
            chkOriginal.Checked = chkOriginalChecked
        End If

        '' Colonna Copia conforme
        Dim chkPdf As CheckBox = DirectCast(e.Item.FindControl("chkPdf"), CheckBox)
        chkPdf.Enabled = DocumentSelectionEnabled
        If item.Item2.Selected Then
            Dim chkPdfChecked As Boolean = Not UncheckAll AndAlso
            (CheckedDefaultOutputTypes.Contains(OutputType.PrintVersion) _
             OrElse
            (CheckedOutputTypeByExtension.ContainsKey(item.Item1.Extension.ToLower()) AndAlso
             CheckedOutputTypeByExtension(item.Item1.Extension.ToLower()).Equals(OutputType.PrintVersion)))
            chkPdf.Checked = chkPdfChecked
        End If
    End Sub

    Private Sub DocumentListGrid_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs) Handles DocumentListGrid.NeedDataSource
        DocumentListGrid.DataSource = New List(Of String)
        UpdateTotalSize()
    End Sub

    Protected Sub DocumentListGridCommand(ByVal sender As System.Object, ByVal e As GridCommandEventArgs) Handles DocumentListGrid.ItemCommand
        If Not e.CommandName.Eq("preview") Then
            Exit Sub
        End If

        Dim document As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(DirectCast(e.Item, GridDataItem).GetDataKeyValue("Item1.Serialized").ToString()))
        AjaxManager.ResponseScripts.Add(String.Format("OpenGenericWindow('../Viewers/DocumentInfoViewer.aspx?{0}')", document.ToQueryString().AsEncodedQueryString()))
    End Sub

    Public Overloads Sub DataBind()
        If Not DataSource.IsNullOrEmpty() Then
            '' Carico la griglia
            DocumentListGrid.DataSource = DataSource
            DocumentListGrid.DataBind()
        Else
            DocumentsPanel.Visible = False
        End If

        UpdateTotalSize()
    End Sub

    Public Sub LoadDocumentInfo(doc As DocumentInfo, Optional options As Action(Of DocumentInfoLoadOptions) = Nothing)
        Dim instance As DocumentInfoLoadOptions = New DocumentInfoLoadOptions()
        options?.Invoke(instance)

        DataSource.Add(New Tuple(Of DocumentInfo, DocumentInfoLoadOptions)(doc, instance))
    End Sub

    Public Sub LoadDocumentInfos(docs As IList(Of DocumentInfo), Optional options As Action(Of DocumentInfoLoadOptions) = Nothing)
        For Each doc As DocumentInfo In docs
            LoadDocumentInfo(doc, options)
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
                Dim document As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(CType(item.GetDataKeyValue("Item1.Serialized"), String)))
                If originalChecked Then
                    Dim docInMemory As MemoryDocumentInfo
                    If TypeOf document Is DocumentProxyDocumentInfo Then
                        Dim documentProxyDoc As DocumentProxyDocumentInfo = DirectCast(document, DocumentProxyDocumentInfo)
                        docInMemory = New MemoryDocumentInfo(documentProxyDoc.Stream, documentProxyDoc.Name)
                    Else
                        docInMemory = BiblosDocumentInfo.CopyDocumentInMemory(document)
                    End If
                    tor.Add(docInMemory)
                End If
                If pdfChecked Then
                    If TypeOf document Is BiblosDocumentInfo Then
                        tor.Add(New BiblosPdfDocumentInfo(CType(document, BiblosDocumentInfo)))
                    ElseIf TypeOf document Is DocumentProxyDocumentInfo Then
                        Dim documentProxyDoc As DocumentProxyDocumentInfo = DirectCast(document, DocumentProxyDocumentInfo)
                        Dim newTempDoc As FileInfo = documentProxyDoc.SaveUniquePdfToTemp()
                        tor.Add(New TempFileDocumentInfo($"CC_{documentProxyDoc.PDFName}", newTempDoc))
                    End If
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