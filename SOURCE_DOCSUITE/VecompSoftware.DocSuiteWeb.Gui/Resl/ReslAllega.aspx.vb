Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Gui.Resl
Imports Telerik.Web.UI
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Newtonsoft.Json
Imports System.Web
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.Services.Biblos.Models

Public Class ReslAllega
    Inherits ReslBasePage

#Region " Fields "

    Private _allDocuments As List(Of DocumentInfo)

#End Region

#Region " Properties "
    Public Property SessionProtocolNumber As String
        Get
            If Not Session.Item("ResolutionNumberAttach") Is Nothing Then
                Return Session.Item("ResolutionNumberAttach").ToString()
            Else
                Return String.Empty
            End If
        End Get
        Set(value As String)
            If String.IsNullOrEmpty(value) Then
                Session.Remove("ResolutionNumberAttach")
            Else
                Session.Item("ResolutionNumberAttach") = value
            End If
        End Set
    End Property
    Public ReadOnly Property SearchWindowHeight As Integer
        Get
            Return DocSuiteContext.Current.ProtocolEnv.PECWindowHeight
        End Get
    End Property

    Public ReadOnly Property SearchWindowWidth As Integer
        Get
            Return DocSuiteContext.Current.ProtocolEnv.PECWindowWidth
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()

        '' Perdonatemi, so che non è bello, tuttavia funziona così anche nel modulo di ricerca e il cliente vuole lo stesso risultato
        Select Case DocSuiteContext.Current.ResolutionEnv.Configuration
            Case "AUSL-PC"
                txtNumber.Label = "Numero"
            Case Else
                txtNumber.Label = "Numero Servizio"
        End Select

        If Not IsPostBack Then
            MasterDocSuite.Title = String.Format("Allega da {0} o {1}", Facade.ResolutionTypeFacade.DeliberaCaption, Facade.ResolutionTypeFacade.DeterminaCaption)
            MasterDocSuite.TitleVisible = False
            txtYear.Value = DateTime.Today.Year

            Page.Form.DefaultButton = btnSeleziona.UniqueID
            txtNumber.Focus()

            rblProposta.Items.Add(New ListItem(Facade.ResolutionTypeFacade.DeliberaCaption, ResolutionType.IdentifierDelibera.ToString("D")))
            rblProposta.Items.Add(New ListItem(Facade.ResolutionTypeFacade.DeterminaCaption, ResolutionType.IdentifierDetermina.ToString("D")))
            rblProposta.SelectedValue = ResolutionEnv.ReslAllegaDefaultType.ToString()
        End If

    End Sub

    Protected Sub AjaxRequestHandler(ByVal seneder As Object, ByVal e As AjaxRequestEventArgs)
        Dim resolutionId As Integer
        If Integer.TryParse(e.Argument, resolutionId) Then
            Me.IdResolution = resolutionId
            LoadDocument(resolutionId)
        End If

    End Sub

    Protected Sub DocumentListGrid_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles DocumentListGrid.ItemDataBound
        If e.Item.ItemType <> GridItemType.Item And e.Item.ItemType <> GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim item As DocumentInfo = DirectCast(e.Item.DataItem, DocumentInfo)

        Dim fileImage As Image = DirectCast(e.Item.FindControl("fileImage"), Image)
        fileImage.ImageUrl = ImagePath.FromDocumentInfo(item)
        Dim fileName As LinkButton = DirectCast(e.Item.FindControl("fileName"), LinkButton)
        fileName.Text = item.Name
        Dim fileType As Label = DirectCast(e.Item.FindControl("fileType"), Label)
        fileType.Text = item.Parent.Name
    End Sub

    Protected Sub DocumentListGridCommand(ByVal sender As System.Object, ByVal e As GridCommandEventArgs) Handles DocumentListGrid.ItemCommand
        If Not e.CommandName.Eq("preview") Then
            Exit Sub
        End If

        Dim document As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(HttpUtility.ParseQueryString(DirectCast(e.Item, GridDataItem).GetDataKeyValue("Serialized").ToString()))
        AjaxManager.ResponseScripts.Add(String.Format("OpenGenericWindow('../Viewers/DocumentInfoViewer.aspx?{0}')", document.ToQueryString().AsEncodedQueryString()))
    End Sub

    ''' <summary> Gestisce l'evento onclick del pulsante seleziona. </summary>
    Private Sub BtnSelezionaClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnSeleziona.Click
        Dim idResolution? As Integer = SearchResolution(Short.Parse(rblProposta.SelectedValue), CType(txtYear.Value.Value, Short), txtNumber.Text)
        If idResolution.HasValue Then
            Me.IdResolution = idResolution.Value
            LoadDocument(idResolution.Value)
        End If

    End Sub

    ''' <summary> Gestisce l'evento onclick del pulsante Allega originali. </summary>
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
        CloseWindowScript(selected)
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf AjaxRequestHandler
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSeleziona, header, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSeleziona, DocumentListGrid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSeleziona, uscResolutionPreview, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSeleziona, btnAdd)

        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, header, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, DocumentListGrid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscResolutionPreview, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, btnAdd)

        AjaxManager.AjaxSettings.AddAjaxSetting(btnAdd, DocumentListGrid, MasterDocSuite.AjaxDefaultLoadingPanel)

    End Sub

    Public Function SearchResolution(reslType As Short, year As Short, serviceNumber As String) As Integer?
        ' Ricerca con i dati inseriti nella form
        Dim finder As New NHibernateResolutionFinder("ReslDB")

        Dim managedData As String = Facade.TabMasterFacade.GetFieldValue(TabMasterFacade.ManagedDataField, DocSuiteContext.Current.ResolutionEnv.Configuration, Short.Parse(rblProposta.SelectedValue))
        finder.InclusiveNumber = Facade.ResolutionFacade.ElaborateInclusiveNumber(year, serviceNumber.Trim(), managedData)
        finder.Year = year.ToString()

        Select Case reslType
            Case ResolutionType.IdentifierDetermina
                finder.Delibera = False
                finder.Determina = True
            Case ResolutionType.IdentifierDelibera
                finder.Delibera = True
                finder.Determina = False
        End Select

        '' Imposto il page size a 2 in modo da sapere se i risultati sono più di 1
        finder.PageSize = 2
        finder.EnablePaging = True

        Dim tor As Integer? = GetResolutionOrError(CType(finder.DoSearchHeader(), IList(Of ResolutionHeader)))
        If tor.HasValue Then
            Return tor
        ElseIf ResolutionEnv.CopyFromResolutionSearchByServiceNumberAsLike Then
            '' Solo se esplicitamente richiesto procedo con una ricerca più libera e avviso in caso di risultati molteplici
            finder.ServiceNumberEndsWith = serviceNumber
            finder.ServiceNumberEqual = Nothing
            tor = GetResolutionOrError(CType(finder.DoSearchHeader(), IList(Of ResolutionHeader)))
            Return tor
        End If

        Return Nothing
    End Function

    Private Function GetResolutionOrError(lst As IList(Of ResolutionHeader)) As Integer?
        If lst.Count > 0 Then
            If lst.Count = 1 Then
                '' Se il risultato è esattamente 1 allora lo ritorno
                Return lst.Item(0).Id
            Else
                '' Se è più di 1 invito ad usare il form di ricerca completo
                AjaxAlert(String.Format("La ricerca ha restituito più di 1 risultato.{0}{0}Ripetere la ricerca con valori più dettagliati{0}[inserire l'eventuale codice e gli zeri prima del numero]{0}oppure utilizzare il modulo di ricerca completo.", Environment.NewLine))
                Return -1
            End If
        End If
        Return Nothing
    End Function

    Public Function GetErrorLabel(resolution As Resolution) As String
        Dim fullNumber As String = ""
        Facade.ResolutionFacade.ReslFullNumber(resolution, resolution.Type.Id, "", fullNumber)
        Return String.Format("{0} {1} ", Facade.ResolutionTypeFacade.GetDescription(resolution.Type), fullNumber)
    End Function

    Public Sub LoadDocument(idResl As Integer?)
        btnAdd.Visible = False
        uscResolutionPreview.Visible = False
        DocumentListGrid.Visible = False

        If Not idResl.HasValue Then
            AjaxAlert("Nessun risultato trovato.")
            Exit Sub
        End If

        '' Se ho il valore -1 significa che l'Alert è già stato gestito
        If idResl.Value = -1 Then
            Exit Sub
        End If

        Dim resolution As Resolution = Facade.ResolutionFacade.GetById(idResl.Value)
        If Not DocSuiteContext.Current.ProtocolEnv.CopyReslAdoptFromCollaborationEnable Then
            If Not resolution.EffectivenessDate.HasValue Then
                AjaxAlert(GetErrorLabel(resolution) & "Non ancora esecutiva.")
                Exit Sub
            End If
        End If

        If Not ResolutionRights.CheckIsViewable(resolution) Then
            AjaxAlert(GetErrorLabel(resolution) & "Mancano i diritti necessari.")
            Exit Sub
        End If
        hf_selectYear.Value = resolution.Year
        hf_selectNumber.Value = If(resolution.Number.HasValue, resolution.Number, String.Empty)
        uscResolutionPreview.CurrentResolution = resolution
        uscResolutionPreview.Show()

        DocumentListGrid.DataSource = AllDocuments(resolution)
        DocumentListGrid.DataBind()

        btnAdd.Visible = True
        uscResolutionPreview.Visible = True
        DocumentListGrid.Visible = True
    End Sub

    Private Function AllDocuments(resl As Resolution) As List(Of DocumentInfo)
        Dim incremental As Short = Facade.ResolutionWorkflowFacade.GetActiveIncremental(resl.Id, 1S)

        If _allDocuments Is Nothing Then
            _allDocuments = New List(Of DocumentInfo)
            '' Verifico se esiste un documento privacy
            Dim privacyDocuments As BiblosDocumentInfo() = Facade.ResolutionFacade.GetPrivacyDocuments(resl)
            If privacyDocuments.Count > 0 Then
                '' Se esiste mostro quello
                _allDocuments.AddRange(ToSeries.GetWithDummyParent("Documenti Privacy", privacyDocuments))
            Else
                '' Se non esiste mostro il documento principale
                _allDocuments.AddRange(ToSeries.GetWithDummyParent("Documenti", Facade.ResolutionFacade.GetDocuments(resl, incremental)))
            End If

            ''Procedo poi ugualmente con gli altri documenti che non sono soggetti a privacy
            Dim omissisMainDocuments As BiblosDocumentInfo() = Facade.ResolutionFacade.GetDocumentsOmissis(resl, incremental, False)
            If omissisMainDocuments.Count > 0 Then
                _allDocuments.AddRange(ToSeries.GetWithDummyParent("Documenti Omissis", omissisMainDocuments))
            End If

            _allDocuments.AddRange(ToSeries.GetWithDummyParent("Allegati", Facade.ResolutionFacade.GetAttachments(resl, incremental, False)))

            Dim omissisAttachmentDocuments As BiblosDocumentInfo() = Facade.ResolutionFacade.GetAttachmentsOmissis(resl, incremental, False)
            If omissisAttachmentDocuments.Count > 0 Then
                _allDocuments.AddRange(ToSeries.GetWithDummyParent("Allegati Omissis", omissisAttachmentDocuments))
            End If

            _allDocuments.AddRange(ToSeries.GetWithDummyParent("Annessi", Facade.ResolutionFacade.GetAnnexes(resl, incremental)))
        End If

        Return _allDocuments
    End Function

    Private Sub CloseWindowScript(documents As IList(Of BiblosDocumentInfo))
        Dim list As New Dictionary(Of String, String)
        For Each item As BiblosDocumentInfo In documents
            list.Add(BiblosFacade.SaveUniqueToTemp(item).Name, item.Name)
        Next
        'Imposto StringEscapeHandling = EscapeHtml per evitare i caratteri che possono generare errore (es. apostrofo)
        Dim serialized As String = JsonConvert.SerializeObject(list)
        Dim jsStringEncoded As String = HttpUtility.JavaScriptStringEncode(serialized)

        Dim closeWindow As String = String.Format("CloseWindow('{0}');", jsStringEncoded)
        MasterDocSuite.AjaxManager.ResponseScripts.Add(closeWindow)
    End Sub

#End Region

End Class