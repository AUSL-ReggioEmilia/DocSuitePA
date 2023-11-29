Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Services.Biblos.Models

Public Class ReslJournalAdd
    Inherits ReslBasePage

#Region " Fields "
    Private _template As ResolutionJournalTemplate
#End Region

#Region " Properties "
    ''' <summary>
    ''' Template selezionato
    ''' </summary>
    Private Property CurrentTemplate() As ResolutionJournalTemplate
        Get

            If _template IsNot Nothing Then
                Return _template
            End If

            ' prelevo dalla drop down
            Dim idt As Integer
            If Templates.SelectedValue <> "0" Then
                Integer.TryParse(Templates.SelectedValue, idt)
            End If

            Return Facade.ResolutionJournalTemplateFacade.GetById(idt)
        End Get
        Set(value As ResolutionJournalTemplate)
            _template = value
        End Set
    End Property

    Protected ReadOnly Property TemplateGroup() As String
        Get
            Return Request.QueryString("Group")
        End Get
    End Property

    Private ReadOnly Property TitleString() As String
        Get
            Return Request.QueryString("TitleString")
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Initialize()
        InitializeAjax()

        ' TODO: evitare la sovrascrittura in ajax request
        If Not Page.IsPostBack Then
            InitializeTemplates()
            setCurrentTemplate(Templates.SelectedValue)
        End If
    End Sub


    Private Sub InitializeTemplates()
        Dim list As IList(Of ResolutionJournalTemplate) = Facade.ResolutionJournalTemplateFacade.GetTemplatesByGroup(TemplateGroup, True)

        Templates.DataValueField = "Id"
        Templates.DataTextField = "Description"
        Templates.DataSource = list
        Templates.DataBind()

        Templates.SelectedValue = Request.QueryString.Item("Template")
    End Sub

    Private Sub ReslJornalAdd_AjaxRequest(sender As Object, e As Telerik.Web.UI.AjaxRequestEventArgs)
        Dim arguments As String() = Split(e.Argument, "|", 2)

        Select Case arguments(0).ToLower()
            Case "preview"
                GetPreview()
            Case "confirm"
                Dim file As New FileInfo(CommonUtil.GetInstance.AppTempPath & arguments(1))
                SaveJournal(file)
        End Select

    End Sub

    Private Sub Templates_SelectedIndexChanged(sender As Object, e As EventArgs)
        setCurrentTemplate(Templates.SelectedValue)
    End Sub

    Private Sub Years_SelectedIndexChanged(sender As Object, e As EventArgs)
        ' Carico tutti i mesi
        loadMonths()
        Months.SelectedIndex = 0
        Months.Enabled = True

    End Sub

#End Region

#Region " Methods "
    Private Sub Initialize()
        Me.Title = "Inserimento nuovo Registro"
        Preview.OnClientClick = "return Preview_Click();"
        Conferma.Visible = False
    End Sub

    Private Sub InitializeAjax()
        AddHandler MasterDocSuite.AjaxManager.AjaxRequest, AddressOf ReslJornalAdd_AjaxRequest
        AddHandler Templates.SelectedIndexChanged, AddressOf Templates_SelectedIndexChanged
        AddHandler Years.SelectedIndexChanged, AddressOf Years_SelectedIndexChanged

        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, AjaxManager)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, Buttons)

        AjaxManager.AjaxSettings.AddAjaxSetting(Templates, Years)
        AjaxManager.AjaxSettings.AddAjaxSetting(Templates, Months)
        AjaxManager.AjaxSettings.AddAjaxSetting(Years, Months)
        AjaxManager.AjaxSettings.AddAjaxSetting(Years, PagFrom)
        AjaxManager.AjaxSettings.AddAjaxSetting(Years, PagTo)

        AjaxManager.AjaxSettings.AddAjaxSetting(Buttons, Templates)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, AjaxDataPanel, MasterDocSuite.AjaxLoadingPanel)
    End Sub

    ''' <summary>
    ''' Imposto nella pagina il template corrente
    ''' </summary>
    Private Sub setCurrentTemplate(id As String)
        Dim parsed As Integer
        Integer.TryParse(id, parsed)
        setCurrentTemplate(parsed)
    End Sub

    ''' <summary>
    ''' Imposto nella pagina il template corrente
    ''' </summary>
    Private Sub setCurrentTemplate(id As Integer)
        Dim template As ResolutionJournalTemplate = Facade.ResolutionJournalTemplateFacade.GetById(id)
        CurrentTemplate = template

        If template IsNot Nothing Then
            ' Creo il registro adatto al template selezionato
            Dim resolutionJournal As ResolutionJournal = Facade.ResolutionJournalFacade.BuildNext(template)
            If IsLastResolutionJournal(template, resolutionJournal) Then
                resolutionJournal = LastResolutionJournal(CurrentTemplate, resolutionJournal)
            End If

            loadYears(resolutionJournal.Year)
            Years.Enabled = True
            Years.SelectedValue = resolutionJournal.Year

            loadMonths(resolutionJournal.Month)
            Months.Enabled = True
            Months.SelectedValue = resolutionJournal.Month

            CalculateFirstPage(resolutionJournal, template)
        Else
            ' se non l'ho trovato parto dal 2011
            loadYears()
            Years.Enabled = False

            loadMonths()
            Months.Enabled = False

            PagFrom.Text = 0
        End If
        PagTo.Text = ""
    End Sub

    ''' <summary>
    ''' Carica gli anni dal 2011
    ''' </summary>
    Private Sub loadYears()
        loadYears(2011)
    End Sub

    ''' <summary>
    ''' Carica gli anni da quello richiesto a quello corrente
    ''' </summary>
    Private Sub loadYears(ByVal year As Integer)
        Years.Items.Clear()
        For i As Integer = year To DateTime.Now.Year
            Years.Items.Add(i)
        Next
    End Sub

    ''' <summary>
    ''' Carica tutti i mesi
    ''' </summary>
    Private Sub loadMonths()
        loadMonths(1)
    End Sub

    ''' <summary>
    ''' Carica i mesi da quello richiesto
    ''' </summary>
    Private Sub loadMonths(ByVal month As Integer)
        ' se il template corrente è selezionato, non permetto la creazione nello stesso anno di template precedenti
        If CurrentTemplate IsNot Nothing Then
            Dim resolutionJournal As ResolutionJournal = Facade.ResolutionJournalFacade.BuildNext(CurrentTemplate)
            If IsLastResolutionJournal(CurrentTemplate, resolutionJournal) Then
                AjaxManager.Alert("Si sta tentando di inserire un registro precedente all'ultimo registro inserito. Vengono corrette le date del registro.")
                resolutionJournal = LastResolutionJournal(CurrentTemplate, resolutionJournal)
            End If

            Dim selectedYear As Short = 0
            Short.TryParse(Years.SelectedValue, selectedYear)
            If (selectedYear = 0) Then
                Throw New DocSuiteException("Creazione nuovo registro", "Tutti i registri dell'anno corrente sono stati creati. Per creare i registri del prossimo anno è necessario attendere il cambio anno.", Request.Url.LocalPath, DocSuiteContext.Current.User.FullUserName)
            End If
            If selectedYear = resolutionJournal.Year Then
                month = resolutionJournal.Month
            End If

            CalculateFirstPage(resolutionJournal, CurrentTemplate)
        End If
        ' carico i mesi
        Months.Items.Clear()
        For i As Integer = month To 12
            Dim monthName As String = CultureInfo.CurrentCulture.DateTimeFormat.MonthGenitiveNames(i - 1)
            Months.Items.Add(New ListItem(StringHelper.UppercaseFirst(monthName), i))
        Next
    End Sub

    Private Sub GetPreview()
        Dim tempFolder As New DirectoryInfo(CommonUtil.GetInstance.AppTempPath)
        Dim label As String

        If CurrentTemplate Is Nothing Then
            AjaxManager.Alert("Template non selezionato")
            Return
        End If

        Dim firstDay As DateTime
        If Not Date.TryParse(String.Format("1/{0}/{1}", Months.SelectedValue, Years.SelectedValue), firstDay) Then
            AjaxManager.Alert("Anno/Mese non validi")
            Return
        End If

        Dim file As FileInfo
        Try
            file = ResolutionJournalPrinter.GetReport(DocSuiteContext.Current.ProtocolEnv.CorporateName, tempFolder, CurrentTemplate, firstDay)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, ex.Message, ex)
            AjaxManager.Alert("Errore in generazione report: " & ex.Message)
            Return
        End Try

        If CurrentTemplate.Pagination Then
            PagTo.Text = Integer.Parse(PagFrom.Text) + ResolutionJournalPrinter.GetNumerOfPages(file) - 1
        End If

        'Calcolo il label
        label = String.Format("{0}.pdf", ResolutionJournalFacade.GetDescription(CurrentTemplate, firstDay.Month, firstDay.Year))

        If (ResolutionEnv.UseWindowPreview) Then
            AjaxManager.ResponseScripts.Add(String.Format("{0}_OpenPreview('{1}','{2}','{3}','{4}');", Me.ID, file.Name, label, DocSuiteContext.Current.ResolutionEnv.WindowWidthRegistri, DocSuiteContext.Current.ResolutionEnv.WindowHeightRegistri))
        Else
            Dim document As TempFileDocumentInfo = New TempFileDocumentInfo(file)
            document.Caption = label
            uscViewerLight.DataSource = New List(Of DocumentInfo) From {document}
            uscViewerLight.Refresh()
        End If

        Preview.Visible = False
        Templates.Enabled = False
        Years.Enabled = False
        Months.Enabled = False
        Conferma.Visible = True
        Conferma.Enabled = True
        Conferma.OnClientClick = String.Format("return Conferma_Click('{0}');", file.Name)
    End Sub

    ''' <summary>
    ''' Salvo il registro
    ''' </summary>
    Private Sub SaveJournal(file As FileInfo)

        Dim d As DateTime
        DateTime.TryParse(String.Format("1/{0}/{1}", Months.SelectedValue, Years.SelectedValue), d)

        Dim item As ResolutionJournal = New ResolutionJournal()
        item.SetMonthYear(d)
        item.Template = CurrentTemplate

        If CurrentTemplate.Pagination Then
            item.FirstPage = Integer.Parse(PagFrom.Text)
            item.LastPage = Integer.Parse(PagTo.Text)
        End If

        item.IsActive = True

        ' Salvo tutte le Resolution
        For Each r As Resolution In ResolutionJournalPrinter.GetResolutionsForReport(CurrentTemplate, d)
            item.AddResolution(r)
        Next

        'Salvo gli id di massimo e di minimo, se WP
        If (CurrentTemplate.TemplateSource = "WP") Then
            item.StartID = Facade.WebPublicationFacade.GetFirstPublishedResolutionID(d, d.AddMonths(1).AddSeconds(-1))
            item.EndID = Facade.WebPublicationFacade.GetLastPublishedResolutionID(d, d.AddMonths(1).AddSeconds(-1))
        End If

        Facade.ResolutionJournalFacade.Save(item)

        ' Salvo su biblos
        item.IDDocument = Facade.ResolutionJournalFacade.SaveDocument(item, file)

        ' Aggiorno
        Facade.ResolutionJournalFacade.UpdateOnly(item)

        AjaxManager.ResponseScripts.Add(String.Format("location.href = '{0}/Resl/ReslJournalSummary.aspx?Type=Resl&ResolutionJournal={1}&TitleString={2}'", DocSuiteContext.Current.CurrentTenant.DSWUrl, item.Id, Server.HtmlEncode(TitleString)))
    End Sub

    ''' <summary>
    ''' Verifico che la data attesa del prossimo registro (mese successivo all'ultimo registro inserito) sia più grande o uguale a quella che si sta inserendo
    ''' </summary>
    ''' <param name="template"></param>
    ''' <param name="resolutionJournal"></param>
    ''' <returns></returns>
    Private Function IsLastResolutionJournal(template As ResolutionJournalTemplate, resolutionJournal As ResolutionJournal) As Boolean
        Dim dateResolutionJournal As DateTime = New DateTime(resolutionJournal.Year, resolutionJournal.Month, 1)
        Dim resolutionJournalBefore As ResolutionJournal = Facade.ResolutionJournalFacade.GetLastBeforeYearAndMonth(template, resolutionJournal.Year, resolutionJournal.Month)
        If resolutionJournalBefore Is Nothing Then
            Return False
        End If
        Dim dateResolutionJournalBefore As DateTime = New DateTime(resolutionJournalBefore.Year, resolutionJournalBefore.Month, 1).AddMonths(+1)
        If DateTime.Compare(dateResolutionJournal, dateResolutionJournalBefore) < 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function LastResolutionJournal(template As ResolutionJournalTemplate, resolutionJournal As ResolutionJournal) As ResolutionJournal
        Dim dateResolutionJournal As DateTime = New DateTime(resolutionJournal.Year, resolutionJournal.Month, 1)
        Dim resolutionJournalBefore As ResolutionJournal = Facade.ResolutionJournalFacade.GetLastBeforeYearAndMonth(template, resolutionJournal.Year, resolutionJournal.Month)
        If resolutionJournalBefore Is Nothing Then
            Return resolutionJournal
        End If
        Dim dateResolutionJournalBefore As DateTime = New DateTime(resolutionJournalBefore.Year, resolutionJournalBefore.Month, 1).AddMonths(+1)
        If DateTime.Compare(dateResolutionJournal, dateResolutionJournalBefore) < 0 Then
            Return resolutionJournal
        Else
            Return resolutionJournalBefore
        End If
    End Function

    Private Sub CalculateFirstPage(resolutionJournal As ResolutionJournal, template As ResolutionJournalTemplate)
        If template.Pagination.GetValueOrDefault(False) Then
            PagFromRow.Style.Remove("Display")
            PagToRow.Style.Remove("Display")

            If Years.SelectedValue = resolutionJournal.Year Then
                PagFrom.Text = resolutionJournal.FirstPage
            Else
                PagFrom.Text = 1
            End If

        Else
            PagFromRow.Style.Add("Display", "None")
            PagToRow.Style.Add("Display", "None")
        End If
    End Sub

#End Region

End Class