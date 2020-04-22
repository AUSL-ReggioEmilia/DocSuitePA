Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.DocSuiteWeb.Facade.Common.DocumentSeries
Imports System.IO

Namespace Series
    Public Class Export
        Inherits CommBasePage

#Region " Fields "
        Private _selectedDocumentSeriesSubsection As DocumentSeriesSubsection
        Private _currentSeries As DocumentSeries
        Private _documentSeriesAttributeEnumFacade As DocumentSeriesAttributeEnumFacade
#End Region

#Region " Properties "

        Public ReadOnly Property DocumentSeriesAttributeEnumFacade() As DocumentSeriesAttributeEnumFacade
            Get
                If _documentSeriesAttributeEnumFacade Is Nothing Then
                    _documentSeriesAttributeEnumFacade = New DocumentSeriesAttributeEnumFacade()
                End If
                Return _documentSeriesAttributeEnumFacade
            End Get
        End Property

        Private ReadOnly Property SelectedDocumentSeries As DocumentSeries
            Get
                If _currentSeries Is Nothing AndAlso Not String.IsNullOrEmpty(ddlDocumentSeries.SelectedValue) Then
                    _currentSeries = Facade.DocumentSeriesFacade.GetDocumentSeries(CType(ddlDocumentSeries.SelectedValue, Integer))
                End If
                Return _currentSeries
            End Get
        End Property

        Private ReadOnly Property DefaultDocumentSeriesId As Integer?
            Get
                Return Request.QueryString.GetValueOrDefault(Of Integer?)("DocumentSeries", Nothing)
            End Get
        End Property

        Private ReadOnly Property DefaultYear As Integer?
            Get
                Return Request.QueryString.GetValueOrDefault(Of Integer?)("Year", Nothing)
            End Get
        End Property

#End Region

#Region " Events "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

            InitializeAjax()

            ' Al primo caricamento della pagina devo caricare l'elenco delle Serie documentali disponibili per la ricerca
            If Not IsPostBack Then
                Dim page As String = " - Esporta"
                btnExport.Visible = True
                ddlDocumentSeries.CausesValidation = True
                Title = ProtocolEnv.DocumentSeriesName & page
                DocumentSeries.Text = ProtocolEnv.DocumentSeriesName & ":"

                SetContainerArchives(Nothing)

                txtYear.Value = If(DefaultYear.HasValue, DefaultYear.Value, DateTime.Now.Year)

                ' Carico la struttura della Serie
                If DefaultDocumentSeriesId.HasValue Then
                    Dim defaultDocumentSeries As DocumentSeries = Facade.DocumentSeriesFacade.GetDocumentByContainerId(DefaultDocumentSeriesId.Value)
                    If defaultDocumentSeries.Container IsNot Nothing AndAlso defaultDocumentSeries.Container.Archive IsNot Nothing Then
                        SetDocumentSeries(defaultDocumentSeries)
                    End If
                End If
            End If
        End Sub

        Private Sub BtnClearClick(sender As Object, e As EventArgs) Handles btnClear.Click
            ' Effettuo il clear dei controlli base nella pagina aspx corrente
            ClearControl(Form)
        End Sub
        Private Sub BtnExportClick() Handles btnExport.Click
            If Not (txtYear.Value.HasValue OrElse (rdpRegistrationTo.SelectedDate.HasValue AndAlso rdpRegistrationFrom.SelectedDate.HasValue)) Then
                AjaxAlert("Inserire campo Anno o intervallo di Data registrazione")
                Exit Sub
            End If
            Dim finder As DocumentSeriesItemFinder = GetFinder()
            Dim items As IList(Of DocumentSeriesItem) = finder.DoSearch()
            If items.Count = 0 Then
                AjaxAlert("Non sono stati trovati elementi corrispondenti alla ricerca")
                Exit Sub
            End If
            Dim ms As Byte() = ExportDocumentSeriesFacade.ExportToCsv(items)
            Response.Clear()
            'Response.Buffer = True
            Response.ContentType = "text/csv"
            Response.AddHeader("Content-Disposition", String.Concat("attachment", "; filename=Export_", ddlDocumentSeries.SelectedItem.Text, ".csv"))
            Response.AddHeader("Content-Length", ms.Length.ToString())
            Response.BinaryWrite(ms)
            Response.End()
        End Sub

        Private Sub ddlDocumentSeries_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlDocumentSeries.SelectedIndexChanged
            SetDocumentSeries(SelectedDocumentSeries)
        End Sub
#End Region

#Region " Methods "

        Private Sub InitializeAjax()
            AjaxManager.AjaxSettings.AddAjaxSetting(btnClear, MainContentWrapper)
            If SelectedDocumentSeries IsNot Nothing AndAlso SelectedDocumentSeries.PublicationEnabled.GetValueOrDefault(False) Then
                AjaxManager.AjaxSettings.AddAjaxSetting(ddlDocumentSeries, tblPublication)
            End If
        End Sub

        Private Sub SetContainerArchives(archive As ContainerArchive)

            Dim preselectedValue As String = ddlDocumentSeries.SelectedValue
            ' Carico l'elenco delle serie documentali su cui ho diritto di visualizzazione
            Dim series As List(Of DocumentSeries) = Facade.DocumentSeriesFacade.GetAllOrdered("Name ASC")
            Dim availableContainers As IList(Of Container) = series.Where(Function(S) archive Is Nothing OrElse (S.Container.Archive IsNot Nothing AndAlso S.Container.Archive.Id = archive.Id)).Select(Function(S) S.Container).ToList()
            availableContainers.Insert(0, New Container() With {.Id = -1, .Name = ""})

            ddlDocumentSeries.DataValueField = "Id"
            ddlDocumentSeries.DataTextField = "Name"
            ddlDocumentSeries.DataSource = availableContainers
            ddlDocumentSeries.DataBind()

            If ddlDocumentSeries.Items.FindByValue(preselectedValue) IsNot Nothing Then
                ddlDocumentSeries.SelectedValue = preselectedValue
            End If
        End Sub

        ''' <summary> Gestisce i controlli sensibili al cambio di Serie Documentale. </summary>
        Private Sub SetDocumentSeries(series As DocumentSeries)
            If series Is Nothing Then
                ddlDocumentSeries.SelectedValue = "-1"
                tblPublication.Visible = True
                Exit Sub
            End If

            If ddlDocumentSeries.Items.FindByValue(series.Container.Id.ToString()) IsNot Nothing Then
                ddlDocumentSeries.SelectedValue = series.Container.Id.ToString()
            End If

            tblPublication.Visible = series.PublicationEnabled.GetValueOrDefault(False)
        End Sub

        ''' <summary> Istanza il finder per la ricerca </summary>
        ''' <returns>Restituisce il Finder popolato con i valori inseriti dall'operatore</returns>
        Private Function GetFinder() As DocumentSeriesItemFinder
            Dim finder As New DocumentSeriesItemFinder()

            finder.ItemStatusIn = New List(Of DocumentSeriesItemStatus)()
            finder.ItemStatusIn.Add(DocumentSeriesItemStatus.Active)

            If chkIncludeDraft.Checked Then
                finder.ItemStatusIn.Add(DocumentSeriesItemStatus.Draft)
            End If

            If SelectedDocumentSeries IsNot Nothing Then
                Dim archive As ArchiveInfo = DocumentSeriesFacade.GetArchiveInfo(SelectedDocumentSeries)
                finder.IdDocumentSeriesIn = New List(Of Integer) From {SelectedDocumentSeries.Id}

            End If

            If txtYear.Value.HasValue Then
                finder.Year = CType(txtYear.Value, Integer?)
            End If

            If cblPublicationStatus.SelectedItem IsNot Nothing Then
                Select Case cblPublicationStatus.SelectedItem.Value
                    Case "NONE"
                        finder.IsPublished = False
                    Case "PUBLICATED"
                        finder.IsPublished = True
                    Case "RETIRED"
                        finder.IsRetired = True
                End Select
            End If

            If rdpRegistrationFrom.SelectedDate.HasValue Then
                finder.RegistrationDateFrom = rdpRegistrationFrom.SelectedDate.Value
            End If

            If rdpRegistrationTo.SelectedDate.HasValue Then
                finder.RegistrationDateTo = rdpRegistrationTo.SelectedDate.Value
            End If

            Return finder
        End Function

        ''' <summary> Clear dei controlli base di ASP e di Telerik </summary>
        ''' <param name="control">pagina in cui si vogliono sbiancare i controlli base</param>
        Private Sub ClearControl(control As Control)

            Select Case control.GetType()
                Case GetType(TextBox)
                    If CType(control, TextBox) IsNot Nothing Then
                        CType(control, TextBox).Text = String.Empty
                    End If
                Case GetType(RadNumericTextBox)
                    If CType(control, RadNumericTextBox) IsNot Nothing Then
                        CType(control, RadNumericTextBox).Text = String.Empty
                    End If
                Case GetType(RadTextBox)
                    If CType(control, RadTextBox) IsNot Nothing Then
                        CType(control, RadTextBox).Text = String.Empty
                    End If
                Case GetType(RadDatePicker)
                    If CType(control, RadDatePicker) IsNot Nothing Then
                        CType(control, RadDatePicker).SelectedDate = Nothing
                    End If
                Case GetType(DropDownList)
                    If CType(control, DropDownList) IsNot Nothing Then
                        CType(control, DropDownList).SelectedIndex = -1
                    End If
                Case GetType(CheckBox)
                    If CType(control, CheckBox) IsNot Nothing Then
                        CType(control, CheckBox).Checked = False
                    End If
                Case GetType(CheckBoxList)
                    If CType(control, CheckBoxList) IsNot Nothing Then
                        CType(control, CheckBoxList).ClearSelection()
                    End If
            End Select

            For Each childControl As Control In control.Controls
                ClearControl(childControl)
            Next

        End Sub

#End Region

    End Class
End Namespace