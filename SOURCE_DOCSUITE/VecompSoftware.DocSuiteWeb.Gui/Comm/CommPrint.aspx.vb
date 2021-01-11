Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.IO
Imports System.Text
Imports System.Web.UI
Imports VecompSoftware.Helpers.Web
Imports Microsoft.Reporting.WebForms
Imports Telerik.Web.UI


Partial Public Class CommPrint
    Inherits CommBasePage

#Region " Fields "

    Private _reference As String = String.Empty
    Private _printDate As Boolean = True

#End Region

#Region " Properties "

    Private ReadOnly Property IdRef() As String
        Get
            Return Request.QueryString("IdRef")
        End Get
    End Property

    Private ReadOnly Property PrintName() As String
        Get
            Return Request.QueryString("PrintName")
        End Get
    End Property

    Private Property Reference() As String
        Get
            Return _reference
        End Get
        Set(ByVal value As String)
            _reference = value
        End Set
    End Property

    Private ReadOnly Property Titolo() As String
        Get
            Return Request.QueryString("Titolo")
        End Get
    End Property

    Private ReadOnly Property PrintDate() As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)("PrintDate", True)
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjaxSettings()
        If Not IsPostBack Then
            tblStampa.Visible = False
        End If

    End Sub
    
#End Region

#Region " Methods "
    Private Sub InitializeAjaxSettings()
        AddHandler ajaxManager.AjaxRequest, AddressOf Report_AjaxRequest
        ajaxManager.AjaxSettings.AddAjaxSetting(ajaxManager, pnlReport, defaultLoadingPanel)

    End Sub

    Private Sub Report_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        If e.Argument.Eq("InitialPageLoad") Then
            InitReport()
        End If
    End Sub

    Private Sub InitReport()
        Dim tablePrint As IPrint = Nothing
        Dim rptPrint As IPrintRpt = Nothing
        tblStampa.Visible = True
        Try
            ' Selezione della stampa
            Select Case PrintName
                Case "RolePrint"
                    tablePrint = Session("Printer")
                    Reference = "StampaRuoli"
                Case "RoleSecurityPrint"
                    tablePrint = Session("Printer")
                    Reference = "StampaRuoliSecurity"
                Case "ContainerPrint"
                    tablePrint = Session("Printer")
                    Reference = "StampaContenitori"
                Case "ContainerSecurityPrint"
                    tablePrint = Session("Printer")
                    Reference = "StampaContenitoriSecurity"
                Case "LogPrint"
                    tablePrint = Session("Printer")
                Case "CategoryPrint"
                    tablePrint = Session("Printer")
                Case "SingleRolePrint"
                    tablePrint = New SingleRolePrint(IdRef)
                Case "SingleContainerPrint"
                    tablePrint = New SingleContainerPrint(IdRef)
                Case "SingleContactPrint"
                    tablePrint = New SingleContactPrint(IdRef)
                Case "Protocol"
                    tablePrint = Session("Printer")
                Case "ProtConcoursePrint"
                    tablePrint = Session("Printer")
                Case "ProtJournalPrint"
                    If ProtocolEnv.PdfPrint() Then
                        rptPrint = Session("Printer")
                        rptPrint.RdlcPrint = Server.MapPath("Report/" & PrintName & ".rdlc")
                    Else
                        tablePrint = Session("Printer")
                    End If
                Case "Frontalino"
                    rptPrint = Session("Printer")
                    rptPrint.RdlcPrint = Server.MapPath("Report/" & PrintName & ".rdlc")
                Case "Document"
                    tablePrint = Session("Printer")
                Case "Contract"
                    tablePrint = Session("Printer")
                Case "Fascicle"
                    tablePrint = Session("Printer")
                Case "FascCopertina"
                    tablePrint = Session("Printer")
                Case "Resolution"
                    tablePrint = CType(Session("Printer"), IPrint)
                    tablePrint.MantainResultsOrder = True
                    If ResolutionEnv.Configuration.Eq("ASL3-TO") Then
                        _printDate = False
                    End If
                Case "ReslDisposPrint"
                    tablePrint = Session("Printer")
                Case "ReslMemoExecutivePrint"
                    tablePrint = New ReslMemoExecutivePrint()
                Case "ReslMemoPublicationPrint"
                    tablePrint = New ReslMemoPublicationPrint()
                Case "ReslElencoDelPrint"
                    rptPrint = Session("Printer")
                    rptPrint.RdlcPrint = Server.MapPath("Report/" & PrintName & ".rdlc")
                Case "ReslElencoDetPrint"
                    rptPrint = Session("Printer")
                    rptPrint.RdlcPrint = Server.MapPath("Report/" & PrintName & ".rdlc")
                Case "ReslJournalDelPrint"
                    rptPrint = Session("Printer")
                    rptPrint.RdlcPrint = Server.MapPath("Report/" & PrintName & ".rdlc")
                Case "ReslJournalDetPrint"
                    rptPrint = Session("Printer")
                    rptPrint.RdlcPrint = Server.MapPath("Report/" & PrintName & ".rdlc")
                Case "RESL00001" ' Registro delle pubblicazioni (delibere).
                    rptPrint = Session("Printer")

                    ' Cerco se esiste l'RDLC personalizzato
                    Dim filePath As String = Server.MapPath("Report/RegistroPubblicazioni_Delibere_" & ProtocolEnv.CorporateAcronym & ".rdlc")
                    If Not File.Exists(filePath) Then
                        ' In mancanza di personalizzazione uso quello standard
                        filePath = Server.MapPath("Report/" & PrintName & ".rdlc")
                    End If
                    rptPrint.RdlcPrint = filePath
                Case "RESL00002" ' Registro delle pubblicazioni (determine).
                    rptPrint = Session("Printer")

                    ' Cerco se esiste l'RDLC personalizzato
                    Dim filePath As String = Server.MapPath("Report/RegistroPubblicazioni_Determine_" & ProtocolEnv.CorporateAcronym & ".rdlc")
                    If Not File.Exists(filePath) Then
                        ' In mancanza di personalizzazione uso quello standard
                        filePath = Server.MapPath("Report/" & PrintName & ".rdlc")
                    End If
                    rptPrint.RdlcPrint = filePath
                Case "ReslElencoProposedDelPrint"
                    rptPrint = Session("Printer")
                    rptPrint.RdlcPrint = Server.MapPath("Report/" & PrintName & ".rdlc")
                Case "ReslElencoProposedDetPrint"
                    rptPrint = Session("Printer")
                    rptPrint.RdlcPrint = Server.MapPath("Report/" & PrintName & ".rdlc")
                Case Else
                    Throw New DocSuiteException("Selezione stampa", String.Format("Stampa [{0}] non prevista, {1}", PrintName, ProtocolEnv.DefaultErrorMessage))
            End Select

            ' Creo prima la stampa e poi setto il titolo, perchè in alcune
            ' classi il titolo viene settato in base ai risultati della ricerca
            If tablePrint IsNot Nothing Then
                tablePrint.DoPrint()
                tablePrint.TablePrint.CloseBuffer()
                'Recupero il titolo della pagina se è settato
                If Not String.IsNullOrEmpty(tablePrint.TitlePrint) Then
                    Title = tablePrint.TitlePrint
                End If

                'stampa intestazione
                PrintTitle()

                AddTableControl(tablePrint.TablePrint)

            ElseIf rptPrint IsNot Nothing Then
                If Not ProtocolEnv.PdfPrint() Then
                    Throw New DocSuiteException("Creazione stampa", String.Format("Stampe PDF non abilitate, {0}", ProtocolEnv.DefaultErrorMessage))
                End If

                rptPrint.DoPrint()
                'Recupero il titolo della pagina se è settato
                If Not String.IsNullOrEmpty(rptPrint.TitlePrint) Then
                    Title = rptPrint.TitlePrint
                End If

                AddTableControl(rptPrint.TablePrint())

            Else
                Throw New DocSuiteException("Creazione stampa", "Errore in ricerca stampa")
            End If

        Catch ex As DocSuiteException
            Throw
        Catch ex As Exception
            Throw New DocSuiteException("Creazione Stampa " & Reference, ex.Message, ex)
        Finally
            Session.Remove("Printer")
        End Try
    End Sub

    Private Overloads Sub AddTableControl(ByRef dstable As DSTable)
        phNoData.Visible = False
        If dstable.RowCount <= 0 Then
            Dim noResultsTemplate As New DSNoRecordsTemplate("Nessun Risultato nella Stampa")
            noResultsTemplate.InstantiateIn(phNoData)
            phNoData.Visible = True
            Exit Sub
        End If

        Dim table As Table = dstable.Table
        'Setto le proprietà della tabella
        table.BorderWidth = Unit.Pixel(1)
        table.BorderStyle = BorderStyle.Solid
        table.Width = Unit.Percentage(100)
        table.CellSpacing = 0
        table.CellPadding = 1
        table.GridLines = GridLines.Both

        'se non viene utilizzato il buffer
        If Not dstable.UseBuffer Then
            'aggiungo il controllo direttamente
            tblCellPrint.Controls.Add(table)
            Exit Sub
        End If

        'Istanzio lo StringBuilder che conterrà il file HTML bufferizzato
        Dim sb As New StringBuilder()
        'Creo i writer
        Using writer As New StringWriter(sb), htmlwriter As New HtmlTextWriter(writer)

            'Scrivo il tag di apertura della Table nello StringBuilder
            table.RenderBeginTag(htmlwriter)

            'Carico il file nell StringBuilder riga per riga
            Using sr As StreamReader = File.OpenText(dstable.BufferFilePath)
                While Not sr.EndOfStream
                    sb.AppendLine(sr.ReadLine())
                End While
            End Using

            'Scrivo il tag di chiusura della Table nello StringBuilder
            table.RenderEndTag(htmlwriter)
        End Using

        'Creo il template e lo istanzio nel controllo
        Dim template As New DSNoRecordsTemplate(sb.ToString())
        template.InstantiateIn(tblCellPrint)
    End Sub

    Private Overloads Sub AddTableControl(ByVal report As ReportViewer)

        tblStampa.Visible = False
        pnlPrint.Visible = True
        pnlNoPrintDate.Visible = True
        pnlNoPrintDate.Visible = False

        Try

            Dim parameters As New List(Of ReportParameter)(2)
            parameters.Add(New ReportParameter("Azienda", ProtocolEnv.CorporateName))
            parameters.Add(New ReportParameter("Titolo", Title))
            report.LocalReport.SetParameters(parameters)

            Dim mimeType As String = ""
            Dim encoding As String = ""
            Dim extension As String = ""
            Dim streamids As String() = Nothing
            Dim warnings As Warning() = Nothing
            Dim bytes As Byte() = report.LocalReport.Render("PDF", Nothing, mimeType, encoding, extension, streamids, warnings)

            Dim pdfguid As Guid = Guid.NewGuid()
            Dim fileName As String = String.Format("{0}{1}.pdf", CommonInstance.AppTempPath, pdfguid.ToString())

            Using myPdF As New BinaryWriter(File.Open(fileName, FileMode.CreateNew))
                myPdF.Write(bytes)
                myPdF.Close()
            End Using

            ' Demando la visualizzazione del report generato al nuovo visualizzatore light
            Dim querystring As String = String.Format("DataSourceType=prot&DownloadFile={0}", fileName)
            Dim temp As String = String.Format("{0}/viewers/TempFileViewer.aspx?{1}", DocSuiteContext.Current.CurrentTenant.DSWUrl, CommonShared.AppendSecurityCheck(querystring))
            Response.Redirect(temp, False)

        Catch ex As Exception
            Throw New DocSuiteException("Creazione Stampa", String.Format("Impossibile creare la stampa, verificare i dati. {0}", ProtocolEnv.DefaultErrorMessage), ex)
        Finally
            report.Dispose()
        End Try

    End Sub

    Private Sub PrintTitle()
        If PrintDate Then
            lblAziendaT.Text = ProtocolEnv.CorporateName
            lblTitoloT.Text = Replace(Title, "|", WebHelper.Br)
            Dim serverDate As Date = DateTime.Now
            If _printDate Then
                lblOra.Text = String.Format("Ora<BR>{0:HH:mm}", serverDate)
                lblData.Text = String.Format("Data<BR>{0:dd/MM/yyyy}", serverDate)
            End If
            pnlNoPrintDate.Visible = False
        Else
            lblAziendaF.Text = ProtocolEnv.CorporateName
            lblTitoloF.Text = Replace(Titolo, "|", WebHelper.Br)
            pnlPrintDate.Visible = False
        End If
    End Sub

#End Region


End Class

