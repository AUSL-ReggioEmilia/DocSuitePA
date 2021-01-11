Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports Telerik.Web.UI
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Newtonsoft.Json
Imports System.Web
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.Services.Biblos.Models

Public Class ProtAllega
    Inherits ProtBasePage

#Region " Properties "
    ''' <summary>
    ''' Elenco dei protocolli collegati che han passato i documenti al protocollo in inserimento.
    ''' </summary>
    ''' <remarks>Stringa di ID separata da virgole.</remarks>
    Public Property SessionProtInserimentoLinks As IList(Of Guid)
        Get
            If Session("ProtInserimento-Link") IsNot Nothing Then
                Return DirectCast(Session("ProtInserimento-Link"), IList(Of Guid))
            End If
            Return Nothing
        End Get
        Set(value As IList(Of Guid))
            Session("ProtInserimento-Link") = value
        End Set
    End Property

    Public Property SessionProtocolNumber As String
        Get
            If Not Session.Item("ProtocolNumberAttach") Is Nothing Then
                Return Session.Item("ProtocolNumberAttach").ToString()
            Else
                Return String.Empty
            End If
        End Get
        Set(value As String)
            If String.IsNullOrEmpty(value) Then
                Session.Remove("ProtocolNumberAttach")
            Else
                Session.Item("ProtocolNumberAttach") = value
            End If
        End Set
    End Property


#End Region


#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()

        If Not IsPostBack Then
            DocumentListGrid.Visible = False
            txtYear.Text = DateTime.Today.Year.ToString()

            txtNumber.Focus()
            MasterDocSuite.TitleVisible = False
            Title = "Copia da protocollo"
        End If

    End Sub

    Private Sub btnSelezionaClick(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnSeleziona.Click
        LoadDocuments(Short.Parse(txtYear.Text), Integer.Parse(txtNumber.Text))
    End Sub

    Protected Sub AjaxRequestHandler(ByVal seneder As Object, ByVal e As AjaxRequestEventArgs)
        Dim splitted As String() = e.Argument.Split("|"c)
        LoadDocuments(Short.Parse(splitted(0)), Integer.Parse(splitted(1)))
    End Sub

    Private Sub BtnAddClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnAdd.Click

        Dim selected As New List(Of BiblosDocumentInfo)
        For Each item As GridDataItem In DocumentListGrid.MasterTableView.GetSelectedItems()
            Dim copiaConforme As RadButton = DirectCast(item.FindControl("CopiaConforme"), RadButton)

            Dim documentInfo As BiblosDocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(CType(item.GetDataKeyValue("Serialized"), String)))

            If copiaConforme.Checked AndAlso TypeOf documentInfo Is BiblosDocumentInfo Then
                documentInfo = New BiblosPdfDocumentInfo(DirectCast(documentInfo, BiblosDocumentInfo))
            Else
                documentInfo.Signature = String.Empty
            End If
            selected.Add(documentInfo)
        Next
        If selected.Count = 0 Then
            AjaxAlert("Devi selezionare almeno un file.")
            Exit Sub
        End If
        closeWindowScript(selected, False)
    End Sub
    Protected Sub DocumentListGridCommand(ByVal sender As System.Object, ByVal e As GridCommandEventArgs) Handles DocumentListGrid.ItemCommand
        If Not e.CommandName.Equals("preview") Then
            Exit Sub
        End If

        Dim document As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(DirectCast(e.Item, GridDataItem).GetDataKeyValue("Serialized").ToString()))
        AjaxManager.ResponseScripts.Add(String.Format("OpenGenericWindow('../Viewers/DocumentInfoViewer.aspx?{0}')", document.ToQueryString().AsEncodedQueryString()))
    End Sub


    Protected Sub DocumentListGrid_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles DocumentListGrid.ItemDataBound
        If e.Item.ItemType <> GridItemType.Item And e.Item.ItemType <> GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim item As ProtocolDocumentDTO = DirectCast(e.Item.DataItem, ProtocolDocumentDTO)
        Dim fileImage As Image = DirectCast(e.Item.FindControl("fileImage"), Image)
        fileImage.ImageUrl = ImagePath.FromFile(item.DocumentName)
        Dim fileName As LinkButton = DirectCast(e.Item.FindControl("fileName"), LinkButton)
        fileName.Text = item.DocumentName
        Dim fileType As Label = DirectCast(e.Item.FindControl("fileType"), Label)
        fileType.Text = item.DocumentType
    End Sub


#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf AjaxRequestHandler
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, headerTable)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, DocumentListGrid, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscProtocolPreview, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, footerTable)

        AjaxManager.AjaxSettings.AddAjaxSetting(txtNumber, AjaxManager)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnSeleziona, headerTable)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSeleziona, DocumentListGrid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSeleziona, uscProtocolPreview, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSeleziona, footerTable)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnAdd, DocumentListGrid, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    ''' <summary>Carica lista dei documenti relativi al protocollo selezionato</summary>
    Private Sub LoadDocuments(year As Short, number As Integer)
        DocumentListGrid.DataSource = Nothing

        uscProtocolPreview.Visible = False

        Dim protocol As Protocol = Facade.ProtocolFacade.GetById(year, number, False)
        If protocol Is Nothing Then
            AjaxAlert("Protocollo {0}/{1} non trovato.", year, number)
            Exit Sub
        End If
        hf_selectYear.Value = protocol.Year.ToString()
        hf_selectNumber.Value = protocol.Number.ToString()
        Dim rights As New ProtocolRights(protocol)
        If Not rights.IsDocumentReadable Then
            AjaxAlert("Protocollo n. {0}{1}Mancano i diritti necessari", protocol.FullNumber, Environment.NewLine)
            Exit Sub
        End If

        DocumentListGrid.DataSource = Facade.ProtocolFacade.GetProtocolDocument(protocol)
        DocumentListGrid.DataBind()



        uscProtocolPreview.CurrentProtocol = protocol
        uscProtocolPreview.Initialize()
        uscProtocolPreview.Visible = True

        ' altro
        DocumentListGrid.Visible = True
        btnAdd.Visible = True
        btnAdd.Enabled = True

    End Sub



    Private Sub closeWindowScript(documents As IList(Of BiblosDocumentInfo), pdf As Boolean)
        Dim list As New Dictionary(Of String, String)
        For Each item As BiblosDocumentInfo In documents
            If pdf Then
                list.Add(BiblosFacade.SaveUniquePdfToTemp(item).Name, item.PDFName)
                Continue For
            End If
            list.Add(BiblosFacade.SaveUniqueToTemp(item).Name, item.Name)
        Next
        'Imposto StringEscapeHandling = EscapeHtml per evitare i caratteri che possono generare errore (es. apostrofo)
        Dim serialized As String = JsonConvert.SerializeObject(list)
        Dim jsStringEncoded As String = HttpUtility.JavaScriptStringEncode(serialized)

        Dim closeWindow As String = String.Format("CloseWindow('{0}');", jsStringEncoded)
        MasterDocSuite.AjaxManager.ResponseScripts.Add(closeWindow)

        Dim idProt As Guid = uscProtocolPreview.CurrentProtocolId.Value

        Dim protocolNumber As String = WebHelper.UploadDocumentRename("prot", Int16.Parse(hf_selectYear.Value), Int32.Parse(hf_selectNumber.Value))
        If SessionProtInserimentoLinks Is Nothing Then
            SessionProtInserimentoLinks = New List(Of Guid)
        End If

        If Not SessionProtInserimentoLinks.Contains(idProt) Then
            SessionProtInserimentoLinks.Add(idProt)
        End If

        If Not SessionProtocolNumber.Contains(protocolNumber) Then
            SessionProtocolNumber = protocolNumber
        End If

    End Sub
#End Region

End Class