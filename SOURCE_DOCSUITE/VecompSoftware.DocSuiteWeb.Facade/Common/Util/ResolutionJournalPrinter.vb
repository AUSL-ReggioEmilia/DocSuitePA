Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Web
Imports VecompSoftware.DocSuiteWeb.Data
Imports Microsoft.Reporting.WebForms
Imports VecompSoftware.Helpers.ExtensionMethods
Imports iText.Kernel.Pdf

Public Class ResolutionJournalPrinter

    ''' <summary>
    ''' Crea nella directory specificata il registro per il mese selezionato
    ''' </summary>
    ''' <param name="corporate">Azienda: per ora solo torino</param>
    ''' <param name="folder">Directory temporanea</param>
    ''' <param name="template"></param>
    ''' <param name="firstDay">Primo giorno del mese/anno da registrare</param>
    Public Shared Function GetReport(corporate As String, folder As DirectoryInfo, template As ResolutionJournalTemplate, firstDay As Date) As FileInfo
        folder.Refresh()
        If Not folder.Exists Then
            Throw New Exception(String.Format("Errore in accesso alla directory temporanea [{0}]", folder.FullName))
        End If

        Dim resolutionList As IList(Of Resolution) = GetResolutionsForReport(template, firstDay)
        Dim serviceCodeDescriptionFacade As ServiceCodeDescriptorFacade = New ServiceCodeDescriptorFacade()
        Dim serviceCodeList As IList(Of ServiceCodeDescriptor) = serviceCodeDescriptionFacade.GetDescriptorsByDate(Date.Now)

        Dim ds As DataSet = New DataSet("Resolutions")
        If (resolutionList.Count > 0) Then
            resolutionListToDataSet(resolutionList, ds, template.TemplateGroup)

            ''Carico i service code
            serviceCodeListToDataSet(serviceCodeList, ds)
        Else
            Throw New Exception("Nessun provvedimento disponibile.")
        End If

        Dim path As String = String.Format("~/Comm/Report/ResolutionJournal/{0}.rdlc", template.TemplateFile)
        Dim reportTemplate As New FileInfo(HttpContext.Current.Server.MapPath(path))

        If Not reportTemplate.Exists Then
            Throw New Exception(String.Format("Template [{0}] non esistente", reportTemplate.Name))
        End If

        Dim report As New ReportViewer()
        report.LocalReport.SetBasePermissionsForSandboxAppDomain(New System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted))
        report.LocalReport.ReportPath = reportTemplate.FullName
        For Each table As DataTable In ds.Tables
            report.LocalReport.DataSources.Add(New ReportDataSource(ds.DataSetName & "_" & table.TableName, table))
        Next

        Return writeReport(corporate, ResolutionJournalFacade.GetDescription(template, firstDay.Month, firstDay.Year), folder, report)
    End Function

    ''' <summary>
    ''' Dato un template e una data di inizio, ritorna tutti gli atti del mese.
    ''' </summary>
    ''' <param name="template">Template del registro di cui recuperare gli atti.</param>
    ''' <param name="startDate">Data di inizio</param>
    Public Shared Function GetResolutionsForReport(template As ResolutionJournalTemplate, startDate As DateTime) As IList(Of Resolution)
        Dim endDate As DateTime = startDate.AddMonths(1).AddSeconds(-1)
        Dim finder As New NHibernateResolutionFinder("ReslDB")
        finder.EagerLog = False
        With finder
            Select Case template.TemplateSource
                Case "WP"
                    Dim facade As New WebPublicationFacade()
                    ' Recupero elenco di 
                    Dim ids As IList(Of Integer) = facade.GetPublishedResolutionsID(startDate, endDate)
                    If ids Is Nothing OrElse ids.Count = 0 Then
                        ids = New List(Of Integer)({Integer.MinValue})
                    End If
                    .IdResolutionList = ids
                Case Else
                    .Year = startDate.Year.ToString()
                    .AdoptionDateFrom = startDate
                    .AdoptionDateTo = endDate
            End Select
            .EnablePaging = False
            If DocSuiteContext.Current.ResolutionEnv.Configuration.Eq("ASL3-TO") Then
                .NotNumber = 0 ' Escludo le resolution di test
            End If

            ' Verifico se visualizzare solo le resolution attive
            If DocSuiteContext.Current.ResolutionEnv.ResolutionJournalShowOnlyActive Then
                .IdStatus = 0
            End If
            .TemplateSpecifications = template.Specifications
            If template.Specifications Is Nothing Then
                ' Se non è definita nessuna specifica forzo ricerca nulla (fail-safe).
                .TemplateSpecifications = New List(Of ResolutionJournalTemplateSpecification)
            End If
        End With
        Return finder.DoSearch()
    End Function

    ''' <summary>
    ''' Aggiunge una lista di atti a un <see>DataSet</see>
    ''' </summary>
    ''' <param name="resolutionList">Lista degli atti</param>
    Private Shared Sub resolutionListToDataSet(resolutionList As IList(Of Resolution), ByRef resolutionDataSet As DataSet, Optional templateGroup As String = Nothing)
        'Se riceve un dataSet vuoto allora lo crea
        If (resolutionDataSet Is Nothing) Then resolutionDataSet = New DataSet("Resolutions")

        resolutionDataSet.Tables.Add("tblResolution")
        Dim latestTableNumber As Int32 = resolutionDataSet.Tables.Count - 1
        Dim latestTable As DataTable = resolutionDataSet.Tables(latestTableNumber)

        latestTable.Columns.Add("Numero", GetType(System.String))
        latestTable.Columns.Add("NumeroPubblicazione", GetType(System.String))
        latestTable.Columns.Add("Numero2", GetType(System.String))
        latestTable.Columns.Add("Controllo", GetType(System.String))
        latestTable.Columns.Add("Container", GetType(System.String))
        latestTable.Columns.Add("DataAdozione", GetType(DateTime))
        latestTable.Columns.Add("DataPubblicazione", GetType(DateTime))
        latestTable.Columns.Add("DataModificaPubblicazione", GetType(DateTime))
        latestTable.Columns.Add("Oggetto", GetType(System.String))
        latestTable.Columns.Add("OggettoFull", GetType(System.String))
        latestTable.Columns.Add("ImmediatelyExecutive", GetType(System.Boolean))
        latestTable.Columns.Add("InclusiveNumber", GetType(System.String))
        latestTable.Columns.Add("Tipo", GetType(System.String))
        latestTable.Columns.Add("TipoID", GetType(Int32))
        latestTable.Columns.Add("StatoPubblicazione", GetType(Int32))
        latestTable.Columns.Add("StatusID", GetType(Int32))
        latestTable.Columns.Add("StatusDescription", GetType(System.String))

        If DocSuiteContext.Current.ResolutionEnv.ResolutionJournalEndPublishingDateEnabled Then
            latestTable.Columns.Add("DataFinePubblicazione", GetType(DateTime))
        End If

        If DocSuiteContext.Current.ResolutionEnv.ResolutionJournalEffectivenessDateEnabled Then
            latestTable.Columns.Add("DataEsecutivita", GetType(System.String))
        End If

        For Each resolution As Resolution In resolutionList
            Dim dataRow As DataRow = latestTable.NewRow()
            resolutionToDataRow(resolution, dataRow, True, templateGroup)
            latestTable.Rows.Add(dataRow)
        Next
    End Sub

    ''' <summary>
    ''' Aggiunge una lista di service code in un <see>DataSet</see>
    ''' </summary>
    ''' <param name="serviceCodeList">Lista dei service code</param>
    Private Shared Sub serviceCodeListToDataSet(serviceCodeList As IList(Of ServiceCodeDescriptor), ByRef resolutionDataSet As DataSet)
        'Se riceve un dataSet vuoto allora lo crea
        If (resolutionDataSet Is Nothing) Then resolutionDataSet = New DataSet("Resolutions")

        resolutionDataSet.Tables.Add("tblServiceCodes")
        Dim latestTableNumber As Int32 = resolutionDataSet.Tables.Count - 1
        Dim latestTable As DataTable = resolutionDataSet.Tables(latestTableNumber)

        latestTable.Columns.Add("ServiceCode", GetType(System.String))
        latestTable.Columns.Add("Servizio", GetType(System.String))
        latestTable.Columns.Add("datacreazione", GetType(System.String))
        latestTable.Columns.Add("datadismissione", GetType(System.String))
        latestTable.Columns.Add("Ordine", GetType(System.Int32))

        For Each serviceCode As ServiceCodeDescriptor In serviceCodeList
            Dim dataRow As DataRow = latestTable.NewRow()
            serviceCodeToDataRow(serviceCode, dataRow)
            latestTable.Rows.Add(dataRow)
        Next
    End Sub

    ''' <summary>
    ''' Copia un'atto nel <see>DataRow</see>
    ''' </summary>
    ''' <param name="resolution">Atto di partenza</param>
    ''' <param name="dataRow">Riga di destinazione</param>
    ''' <param name="omissis">Indica se visualizzare gli omissis nell'oggetto</param>
    ''' <remarks></remarks>
    Private Shared Sub resolutionToDataRow(ByVal resolution As Resolution, ByRef dataRow As DataRow, omissis As Boolean, Optional templateGroup As String = Nothing)

        dataRow("Numero") = resolution.Number.Value
        dataRow("Numero2") = GetDetermina(resolution)
        dataRow("Controllo") = GetControllo(resolution)
        dataRow("Container") = resolution.Container.Name
        dataRow("DataAdozione") = resolution.AdoptionDate
        dataRow("Oggetto") = getOggetto(resolution, omissis, False)
        dataRow("OggettoFull") = getOggetto(resolution, omissis, True)
        dataRow("ImmediatelyExecutive") = resolution.ImmediatelyExecutive.GetValueOrDefault(False)
        dataRow("InclusiveNumber") = resolution.InclusiveNumber
        dataRow("StatusID") = resolution.Status.Id
        dataRow("StatusDescription") = resolution.Status.Description

        ' Pubblicazione
        Dim wpf As New WebPublicationFacade()
        Dim wp As List(Of WebPublication) = wpf.GetByResolution(resolution)
        If Not wp Is Nothing AndAlso wp.Count > 0 Then
            dataRow("NumeroPubblicazione") = wp(0).ExternalKey
            dataRow("DataPubblicazione") = wp(0).RegistrationDate.Date

            If DocSuiteContext.Current.ResolutionEnv.ResolutionJournalEndPublishingDateEnabled Then
                'Se  ho delle pubblicazioni Web dell'atto DataFinePubblicazione= DataRegistrazione+15 gg
                dataRow("DataFinePubblicazione") = DateAdd(DateInterval.Day, 15, wp(0).RegistrationDate.Date).ToString("dd/MM/yyyy")
            End If

            If wp(0).LastChangedDate.HasValue Then
                dataRow("DataModificaPubblicazione") = wp(0).LastChangedDate.Value.Date
            End If
            dataRow("StatoPubblicazione") = wp(0).Status
        Else
            dataRow("NumeroPubblicazione") = "0"
            If resolution.PublishingDate.HasValue Then
                dataRow("DataPubblicazione") = resolution.PublishingDate.Value.ToString("dd/MM/yyyy")

                If DocSuiteContext.Current.ResolutionEnv.ResolutionJournalEndPublishingDateEnabled Then
                    ' DataFinePubblicazione= DataPubblicazione+15 gg
                    dataRow("DataFinePubblicazione") = DateAdd(DateInterval.Day, 15, resolution.PublishingDate.Value).ToString("dd/MM/yyyy")
                End If

            End If
            dataRow("StatoPubblicazione") = -1
        End If

        If DocSuiteContext.Current.ResolutionEnv.ResolutionJournalEffectivenessDateEnabled Then
            If resolution.EffectivenessDate.HasValue Then
                dataRow("DataEsecutivita") = resolution.EffectivenessDate.Value.ToString("dd/MM/yyyy")
            Else
                dataRow("DataEsecutivita") = String.Empty
            End If
        End If

        dataRow("Tipo") = If(resolution.Type.Id = 0, "Atto", "Delib")
        dataRow("TipoID") = resolution.Type.Id

    End Sub

    ''' <summary>
    ''' Copia un service code nel <see>DataRow</see>
    ''' </summary>
    ''' <param name="serviceCode">ServiceCode di partenza</param>
    ''' <param name="dataRow">Riga di destinazione</param>
    ''' <remarks></remarks>
    Private Shared Sub serviceCodeToDataRow(ByVal serviceCode As ServiceCodeDescriptor, ByRef dataRow As DataRow)
        dataRow("ServiceCode") = serviceCode.ServiceCode
        dataRow("Servizio") = serviceCode.Name
        dataRow("datacreazione") = serviceCode.RegistrationDate.Value.Date
        dataRow("datadismissione") = serviceCode.DismissalDate
        dataRow("Ordine") = serviceCode.SortIndex
    End Sub

    Public Shared Function GetDetermina(ByVal resl As Resolution) As String
        If resl.Number.HasValue Then
            Dim proposerCode As String = String.Empty
            If resl.ResolutionContactProposers IsNot Nothing AndAlso resl.ResolutionContactProposers.Count > 0 Then
                proposerCode = resl.ResolutionContactProposers.ElementAt(0).Contact.Code
            End If
            Return String.Format("/{0}/{1}", proposerCode, resl.Year.ToString())
        Else
            Return String.Empty
        End If
    End Function

    Public Shared Function GetControllo(ByVal resl As Resolution) As String

        Dim controllo As New StringBuilder
        If resl.OCSupervisoryBoard.GetValueOrDefault(False) Then
            If controllo.Length <> 0 Then
                controllo.AppendLine()
            End If
            controllo.Append("art.14")
        End If
        If resl.OCRegion.GetValueOrDefault(False) Then
            If controllo.Length <> 0 Then
                controllo.AppendLine()
            End If
            controllo.Append("Regione")
        End If
        If resl.OCManagement.GetValueOrDefault(False) Then
            If controllo.Length <> 0 Then
                controllo.AppendLine()
            End If
            controllo.Append("Controllo Gestione")
        End If
        If resl.OCCorteConti.GetValueOrDefault(False) Then
            If controllo.Length <> 0 Then
                controllo.AppendLine()
            End If
            controllo.Append("Corte dei Conti")
        End If
        If resl.OCOther.GetValueOrDefault(False) Then
            If controllo.Length <> 0 Then
                controllo.AppendLine()
            End If
            controllo.Append(resl.OtherDescription)
        End If

        Return controllo.ToString()
    End Function

    Private Shared Function getOggetto(resolution As Resolution, omissis As Boolean, full As Boolean) As String
        Dim title As New StringBuilder
        title.Append(ResolutionUtil.OggettoPrivacy(resolution, omissis))
        If resolution.Status.Id = -2 Then
            title.AppendLine()
            title.Append("ERRATA REGISTRAZIONE")
            If Not IsNothing(resolution.LastChangedReason) AndAlso Not IsDBNull(resolution.LastChangedReason) AndAlso resolution.LastChangedReason.Length > 0 Then
                title.Append(": ").Append(resolution.LastChangedReason)
            End If
        Else
            If full Then
                If resolution.ImmediatelyExecutive.GetValueOrDefault(False) Then
                    title.AppendLine()
                    title.Append("IMMEDIATAMENTE ESEGUIBILE")
                End If
            End If
        End If
        Return title.ToString()
    End Function

    ''' <summary>
    ''' Scrive il report nella directory specificata
    ''' </summary>
    ''' <param name="folder">Directory temporanea</param>
    ''' <param name="report">Template da renderizzare</param>
    Private Shared Function writeReport(corporate As String, title As String, folder As DirectoryInfo, ByVal report As ReportViewer) As FileInfo
        ' Imposto i parametri
        Dim p(1) As ReportParameter
        p(0) = New ReportParameter("Azienda", corporate)
        p(1) = New ReportParameter("Titolo", title)
        report.LocalReport.SetParameters(p)

        ' Render del report
        Dim mimeType As String = ""
        Dim encoding As String = ""
        Dim extension As String = ""
        Dim streamids As String() = Nothing
        Dim warnings As Warning() = Nothing
        Dim bytes As Byte() = report.LocalReport.Render("PDF", Nothing, mimeType, encoding, extension, streamids, warnings)

        ' Salvataggio su file temporaneo
        Dim fileName As String = String.Format("{0}{1}-{2}.pdf", folder.FullName, DocSuiteContext.Current.User.UserName, Guid.NewGuid().ToString())
        Dim temporaryReport As New FileInfo(fileName)
        Dim pdfWriter As BinaryWriter = New BinaryWriter(temporaryReport.Open(FileMode.CreateNew))
        Try
            pdfWriter.Write(bytes)
        Catch ex As Exception
            Throw ex
        Finally
            pdfWriter.Close()
        End Try

        Return temporaryReport

    End Function

    Public Shared Function GetNumerOfPages(file As FileInfo) As Integer
        Dim i As Integer = 0

        If file.Exists Then
            Dim pdfDpcument As iText.Kernel.Pdf.PdfDocument = New iText.Kernel.Pdf.PdfDocument(New iText.Kernel.Pdf.PdfReader(file.FullName))
            i = pdfDpcument.GetNumberOfPages()
            pdfDpcument.Close()
        End If

        Return i
    End Function
End Class
