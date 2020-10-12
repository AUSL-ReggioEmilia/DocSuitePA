Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Linq
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Web
Imports VecompSoftware.DocSuiteWeb.DTO.Collaborations
Imports VecompSoftware.Services.Biblos.Models

Public Class ReslJournal
    Inherits ReslBasePage
    Implements ISignMultipleDocuments

#Region " Properties "

    Public ReadOnly Property DocumentsToSign() As IList(Of MultiSignDocumentInfo) Implements ISignMultipleDocuments.DocumentsToSign
        Get
            Dim list As New List(Of MultiSignDocumentInfo)

            For Each item As GridDataItem In Journals.Items
                ' Registri selezionati
                If Not DirectCast(item.FindControl("chkSelect"), CheckBox).Checked Then
                    Continue For
                End If

                Dim resolutionJournal As ResolutionJournal = Facade.ResolutionJournalFacade.GetById(DirectCast(item.GetDataKeyValue("Id"), Integer))
                If resolutionJournal Is Nothing Then
                    Throw New DocSuiteException("Errore firma registri selezionati", String.Format("Impossibile estrarre il registro [{0}]", item.GetDataKeyValue("Id")))
                End If

                Dim doc As BiblosDocumentInfo
                If resolutionJournal.IDSignedDocument.HasValue Then
                    doc = BiblosDocumentInfo.GetDocuments(resolutionJournal.Template.Location.ConsBiblosDSDB, resolutionJournal.IDSignedDocument.Value).LastOrDefault()
                Else
                    doc = BiblosDocumentInfo.GetDocuments(resolutionJournal.Template.Location.ReslBiblosDSDB, resolutionJournal.IDDocument).FirstOrDefault()
                End If

                ' Creazione oggetto per pagina di firma multipla
                Dim msdi As New MultiSignDocumentInfo(doc)
                msdi.GroupCode = TemplateGroup
                msdi.Mandatory = True
                msdi.DocType = resolutionJournal.Template.Description
                msdi.Description = ResolutionJournalFacade.GetDescription(resolutionJournal)
                msdi.IdOwner = resolutionJournal.Id.ToString()

                list.Add(msdi)

            Next

            Return list
        End Get
    End Property

    Public ReadOnly Property ReturnUrl() As String Implements ISignMultipleDocuments.ReturnUrl
        Get
            Return String.Format("~/Resl/ReslJournal.aspx?Type={0}&TitleString={1}&Group={2}", Type, TitleString, TemplateGroup)
        End Get
    End Property
    Public ReadOnly Property SignAction As String Implements ISignMultipleDocuments.SignAction
        Get
            Return String.Empty
        End Get
    End Property

    ''' <summary> Directory temporanea dove si trovano i files da firmare </summary>
    Protected Property TempDir() As String
        Get
            Return CType(ViewState("TempDir"), String)
        End Get
        Set(ByVal value As String)
            ViewState("TempDir") = value
        End Set
    End Property

    Protected ReadOnly Property TemplateGroup() As String
        Get
            Dim val As String
            If ViewState("Group") Is Nothing Then
                val = HttpContext.Current.Request.QueryString("Group")
                ViewState("Group") = val
            Else
                val = ViewState("Group").ToString()
            End If
            Return val
        End Get
    End Property

    Private ReadOnly Property SelectedTemplate() As ResolutionJournalTemplate
        Get
            Dim templateId As Integer
            If Integer.TryParse(Templates.SelectedValue, templateId) Then
                Return Facade.ResolutionJournalTemplateFacade.GetById(templateId)
            End If
            Return Nothing
        End Get
    End Property

    Private ReadOnly Property TitleString() As String
        Get
            Dim val As String
            If ViewState("TitleString") Is Nothing Then
                val = HttpContext.Current.Request.QueryString("TitleString")
                ViewState("TitleString") = val
            Else
                val = ViewState("TitleString").ToString()
            End If
            Return val
        End Get
    End Property

    Private ReadOnly Property DefaultPagination() As Boolean
        Get
            If ViewState("DefaultPagination") IsNot Nothing Then
                Return DirectCast(ViewState("DefaultPagination"), Boolean)
            End If

            Dim list As IList(Of ResolutionJournalTemplate) = Facade.ResolutionJournalTemplateFacade.GetTemplatesByGroup(TemplateGroup, True)
            If list.Any(Function(template) template.Pagination.GetValueOrDefault(False)) Then
                ViewState("DefaultPagination") = True
                Return True
            End If

            ViewState("DefaultPagination") = False
            Return False
        End Get
    End Property

    Private ReadOnly Property DefaultMultisign() As Boolean
        Get
            If ViewState("DefaultMultisign") IsNot Nothing Then
                Return DirectCast(ViewState("DefaultMultisign"), Boolean)
            End If

            Dim list As IList(Of ResolutionJournalTemplate) = Facade.ResolutionJournalTemplateFacade.GetTemplatesByGroup(TemplateGroup, True)
            If list.Any(Function(template) template.MultipleSign.GetValueOrDefault(False)) Then
                ViewState("DefaultMultisign") = True
                Return True
            End If

            ViewState("DefaultMultisign") = False
            Return False
        End Get
    End Property

    Private Property ReslJournalIndexYear As Integer?
        Get
            If Session("ReslJournalIndexYear" & TemplateGroup) Is Nothing Then
                Return Nothing
            End If
            Return DirectCast(Session("ReslJournalIndexYear" & TemplateGroup), Integer?)
        End Get
        Set(value As Integer?)
            Session("ReslJournalIndexYear" & TemplateGroup) = value
        End Set
    End Property


#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjaxSettings()

        If Not Page.IsPostBack Then
            Dim sourcePage As MultipleSignBasePage = TryCast(PreviousPage, MultipleSignBasePage)
            If sourcePage IsNot Nothing Then
                SaveSignedDocuments(sourcePage.SignedDocuments)
            End If

            Initialize()
            Search()
        End If
    End Sub

    Private Sub NuovoClick(sender As Object, e As EventArgs) Handles Nuovo.Click
        If SelectedTemplate Is Nothing Then
            AjaxAlert("È necessario selezionare un registro.")
            Return
        End If

        Dim path As New StringBuilder("ReslJournalAdd.aspx?Type=Resl")
        path.Append("&Template=").Append(SelectedTemplate.Id.ToString())
        path.Append("&Group=").Append(TemplateGroup)
        path.Append("&TitleString=").Append(TitleString)

        Response.Redirect(path.ToString())
    End Sub

    Private Sub YearsSelectedIndexChanged(sender As Object, e As EventArgs) Handles Years.SelectedIndexChanged
        Dim year As Integer
        If Integer.TryParse(Years.SelectedValue, year) Then
            ReslJournalIndexYear = year
        End If

        Search()
    End Sub

    Private Sub TemplatesSelectedIndexChanged(sender As Object, e As EventArgs) Handles Templates.SelectedIndexChanged
        Search()
    End Sub

    Private Sub JournalsItemDataBound(sender As Object, e As GridItemEventArgs) Handles Journals.ItemDataBound
        If e.Item.ItemType <> GridItemType.Item And e.Item.ItemType <> GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim resolutionJournal As ResolutionJournal = DirectCast(e.Item.DataItem, ResolutionJournal)

        With DirectCast(e.Item.FindControl("imgType"), RadButton)
            .NavigateUrl = Facade.ResolutionJournalFacade.ResolutionJournalViewerUrl(resolutionJournal)
            .Image.ImageUrl = If(resolutionJournal.Signdate.HasValue, ImagePath.SmallSigned, ImagePath.SmallPdf)
        End With

        With DirectCast(e.Item.FindControl("Description"), LinkButton)
            .Text = ResolutionJournalFacade.GetDescription(resolutionJournal)
            .PostBackUrl = String.Format("../Resl/ReslJournalSummary.aspx?Type=Resl&ResolutionJournal={0}&TitleString={1}", resolutionJournal.Id, TitleString)
        End With

        With DirectCast(e.Item.FindControl("Month"), Label)
            .Text = StringHelper.UppercaseFirst(CultureInfo.CurrentCulture.DateTimeFormat.MonthGenitiveNames(resolutionJournal.Month - 1))
        End With

        With DirectCast(e.Item.FindControl("chkSelect"), CheckBox)
            .Enabled = (Not resolutionJournal.IDSignedDocument.HasValue OrElse resolutionJournal.Template.MultipleSign.GetValueOrDefault(False))
        End With

        With DirectCast(e.Item.FindControl("publicationId"), Label)
            .Text = String.Format("{0} - {1}", resolutionJournal.StartID, resolutionJournal.EndID)
        End With
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        Title = TitleString
        Journals.Visible = False
        btnMultiSign.PostBackUrl = MultipleSignBasePage.GetMultipleSignUrl()

        InitializeTemplates()
        InitializeYears()
    End Sub

    ''' <summary> Popolo gli anni del registro dal 2011 ad oggi </summary>
    Private Sub InitializeYears()
        Years.Items.Clear()
        Years.Items.Add(New ListItem("Tutti", ""))
        For i As Integer = 2011 To DateTime.Now.Year
            Years.Items.Add(i.ToString())
        Next
        If ReslJournalIndexYear.HasValue Then
            Years.SelectedValue = ReslJournalIndexYear.ToString()
        Else
            ' Seleziono l'anno corrente
            Years.SelectedValue = DateTime.Now.Year.ToString()
        End If
    End Sub

    ''' <summary> Carico i template nella combo </summary>
    Private Sub InitializeTemplates()
        Dim list As IList(Of ResolutionJournalTemplate) = Facade.ResolutionJournalTemplateFacade.GetTemplatesByGroup(TemplateGroup, True)

        If list.Count > 1 Then
            Dim zeroSelection As New ResolutionJournalTemplate With {.Description = "Tutti"}
            list.Insert(0, zeroSelection)
        End If

        Templates.DataValueField = "Id"
        Templates.DataTextField = "Description"
        Templates.DataSource = list
        Templates.DataBind()

        ' Se ho un unico template non visualizzo la ROW che contiene i Templates e la relativa colonna in griglia
        If list.Count = 1 Then
            TemplatesRow.Style.Add("Display", "None")
            Journals.Columns.FindByUniqueName("Template").Visible = False
        End If
    End Sub

    Public Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(Journals, Journals, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(Years, Journals, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(Templates, Journals, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(Years, lblHeader)
        AjaxManager.AjaxSettings.AddAjaxSetting(Templates, lblHeader)
        AjaxManager.AjaxSettings.AddAjaxSetting(Years, pnlFooter)
        AjaxManager.AjaxSettings.AddAjaxSetting(Templates, pnlFooter)

        If ProtocolEnv.EnableMultiSign AndAlso CommonShared.UserConnectedBelongsTo(ResolutionEnv.ResolutionJournalSigners) Then
            btnMultiSign.Visible = True
            AjaxManager.AjaxSettings.AddAjaxSetting(btnMultiSign, Journals, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, Journals, MasterDocSuite.AjaxDefaultLoadingPanel)
        Else
            btnMultiSign.Visible = False
        End If
    End Sub

    Private Function MultisignEnabled() As Boolean
        Dim template As ResolutionJournalTemplate = SelectedTemplate
        If template IsNot Nothing Then
            Return template.MultipleSign.GetValueOrDefault(False)
        End If
        ' TUTTI
        Return DefaultMultisign
    End Function

    Private Sub Search()
        If ResolutionEnv.SearchMaxRecords <> 0 Then
            Journals.PageSize = ResolutionEnv.SearchMaxRecords
        End If

        Dim finder As NHibernateResolutionJournalFinder = Facade.ResolutionJournalFinder

        Dim year As Short
        If Short.TryParse(Years.SelectedValue, year) Then
            finder.Year = year
        End If

        finder.TemplateGroup = TemplateGroup
        finder.Template = SelectedTemplate()


        ' Nascondo le colonne sulla paginazione
        Journals.Columns.FindByUniqueName("FirstPage").Visible = False
        Journals.Columns.FindByUniqueName("LastPage").Visible = False

        If MultisignEnabled() Then
            Journals.Columns.FindByUniqueName("SignDate").HeaderText = "Data ultima firma"
        Else
            Journals.Columns.FindByUniqueName("SignDate").HeaderText = "Data firma"
        End If

        'Verifico se necessari i riferimenti agli id di pubblicazione (solo se si tratta di elenco di registri di pubblicazione)
        Journals.Columns.FindByUniqueName("publicationIds").Visible = (TemplateGroup = "RP")

        Journals.Finder = finder
        Journals.DataBindFinder()
        Journals.Visible = True

        ' Imposto titolo a seconda dei risultati della ricerca
        If Journals.DataSource IsNot Nothing Then
            lblHeader.Text = String.Format(" Registri - Risultati ({0}/{1})", Journals.DataSource.Count, Journals.VirtualItemCount)
        Else
            lblHeader.Text = " Registri - Nessun Risultato"
        End If
    End Sub

    ''' <summary> Documenti firmati. </summary>
    Private Sub SaveSignedDocuments(ByVal signedDocuments As List(Of MultiSignDocumentInfo))
        For Each document As MultiSignDocumentInfo In signedDocuments
            Dim resolutionJournal As ResolutionJournal = Facade.ResolutionJournalFacade.GetById(Integer.Parse(document.IdOwner))
            ' Salvo su biblos
            Dim file As FileInfo = DirectCast(document.DocumentInfo, FileDocumentInfo).FileInfo
            Dim idDocument As Integer = Facade.ResolutionJournalFacade.SaveSignedDocument(resolutionJournal, file)
            resolutionJournal.IDSignedDocument = idDocument
            ' Imposto la firma
            Facade.ResolutionJournalFacade.SetSign(resolutionJournal, DocSuiteContext.Current.User.FullUserName, DateTime.Now)
        Next

        ' Ricarico pagina
        Response.Redirect(ReturnUrl, True)
    End Sub

#End Region

End Class
