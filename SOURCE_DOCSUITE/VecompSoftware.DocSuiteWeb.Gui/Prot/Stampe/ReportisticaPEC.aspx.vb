Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.IO
Imports System.Globalization
Imports VecompSoftware.DocSuiteWeb.Data.PEC.Finder
Imports iText.Kernel.Geom
Imports iText.Kernel.Pdf
Imports iText.Layout.Element
Imports iText.Kernel.Font
Imports iText.Layout.Properties
Imports iText.IO.Font
Imports iText.Kernel.Colors
Imports iText.Layout.Borders
Imports iText.IO.Font.Constants

Public Class ReportisticaPEC
    Inherits PECBasePage

#Region " Fields "

    Dim _pdfData As New MemoryStream
    Dim _pdfWriter As PdfWriter = New PdfWriter(_pdfData)
    Dim _writer As PdfDocument = New PdfDocument(_pdfWriter)
    Dim _doc As iText.Layout.Document = New iText.Layout.Document(_writer, PageSize.A4)
    Private _mailboxes As IList(Of PECMailBox)

#End Region

#Region " Properties "

    Public ReadOnly Property Mailboxes As IList(Of PECMailBox)
        Get
            If _mailboxes Is Nothing Then
                _mailboxes = New List(Of PECMailBox)
            End If

            If _mailboxes.Count = 0 Then
                _mailboxes = Facade.PECMailboxFacade.GetVisibleMailBoxes()
            End If

            Return _mailboxes
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Title = String.Format("{0} - Reportistica", PecLabel)
            DataBindMailboxes(ddlMailbox)

            ' Carico i dati per le dropdownlist di selezione mese e anno 
            loadMonths()
            loadYears(DocSuiteContext.Current.ProtocolEnv.PECYearCanStartReport()) 'recupero anno dal quale dare la possibilità di fare report
            ' Imposto mese e anno corrente di default
            Months.SelectedIndex = DateTime.Now.Month - 1
            Years.SelectedValue = DateTime.Now.Year.ToString()
        End If
    End Sub

    Protected Sub cmdStampa_Click(sender As Object, e As EventArgs) Handles cmdStampa.Click

        If ddlMailbox.Items.Count >= 1 AndAlso ddlMailbox.SelectedValue <> "" AndAlso ddlMailbox.SelectedValue <> "--" Then
            _doc.SetMargins(50, 50, 80, 50)
            Dim viewerPreferences As PdfViewerPreferences = New PdfViewerPreferences()
            If viewerPreferences Is Nothing Then
                _writer.GetCatalog().SetViewerPreferences(New PdfViewerPreferences())
            End If
            viewerPreferences.SetDuplex(PdfViewerPreferences.PdfViewerPreferencesConstants.USE_OUTLINES)
            _writer.GetCatalog().SetViewerPreferences(viewerPreferences)

            Dim mails As IList(Of PECMail) = Nothing
            Dim mailsSpostate As IList(Of PECMail) = Nothing

            Dim ids(1) As Short
            If ddlMailbox.SelectedValue <> "ALL" Then
                ids(0) = CShort(ddlMailbox.SelectedValue)
                GetMailboxMails(ids, ddlMailbox.SelectedItem.Text.Substring(4), mails, mailsSpostate)
                PrintPDFMailBox(ddlMailbox.SelectedItem.Text.Substring(4), mails, mailsSpostate)
            Else
                For Each item As System.Web.UI.WebControls.ListItem In ddlMailbox.Items
                    If Not item.Value.Eq("ALL") Then
                        ids(0) = CShort(item.Value)
                        GetMailboxMails(ids, item.Text.Substring(4), mails, mailsSpostate)
                        PrintPDFMailBox(item.Text.Substring(4), mails, mailsSpostate)
                    End If
                Next
            End If

            _doc.Close()

            _pdfData.Flush()

            _pdfData.Close()

            Dim g As String = Guid.NewGuid().ToString().Replace("-", "")

            Dim filename As String = DocSuiteContext.Current.User.UserName & "-ReportisticaPEC-" + g + ".pdf"
            Dim tempFile As String = String.Format("~/temp/{0}", filename)

            Dim fs As New FileStream(Server.MapPath(tempFile), FileMode.Create)
            Dim b As Byte() = _pdfData.ToArray()
            fs.Write(b, 0, b.Length)
            fs.Flush()
            fs.Close()
            fs.Dispose()

            If File.Exists(Server.MapPath(tempFile)) Then

                ' Demando la visualizzazione del report generato al nuovo visualizzatore light
                Dim querystring As String = String.Format("DataSourceType=Prot&DownloadFile={0}", filename)
                Dim temp As String = String.Format("{0}/viewers/TempFileViewer.aspx?{1}", DocSuiteContext.Current.CurrentTenant.DSWUrl, CommonShared.AppendSecurityCheck(querystring))
                Response.Redirect(temp, False)
                ' Per evitare ThreadAbortException
                Return
            End If

        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub DataBindMailboxes(ByVal ddlMBoxes As DropDownList)
        ddlMBoxes.Items.Clear()
        For Each mailbox As PECMailBox In Mailboxes
            If Facade.PECMailboxFacade.IsRealPecMailBox(mailbox) Then
                ddlMBoxes.Items.Add(New System.Web.UI.WebControls.ListItem(Facade.PECMailboxFacade.MailBoxRecipientLabel(mailbox), mailbox.Id.ToString()))
            End If
        Next

        If CommonShared.HasGroupAdministratorRight Then
            ddlMBoxes.SelectedIndex = ddlMBoxes.Items.Count - 1
        End If
    End Sub

    Protected Function isMailMoved(ByVal mail As PECMail) As Boolean
        Dim retval As Boolean = False

        If mail IsNot Nothing Then
            Dim logs As IList(Of PECMailLog) = mail.LogEntries

            If logs IsNot Nothing AndAlso logs.Count > 0 Then
                For Each log As PECMailLog In logs
                    If Not String.IsNullOrEmpty(log.Type) AndAlso log.Type = PECMailLogType.Move.ToString() Then
                        retval = True
                        Exit For
                    End If
                Next
            End If
        End If

        Return retval
    End Function
    Protected Function CreateCustomFont(fontName As String) As String
        Return PdfFontFactory.CreateFont(fontName).ToString()
    End Function
    Protected Function GetParPDFBold(testo As String) As Paragraph
        Dim paragraph As Paragraph = New Paragraph()
        paragraph.SetMultipliedLeading(10.0F).SetBold()
        Return paragraph
    End Function

    Protected Function GetParPDFNormal(testo As String) As Paragraph
        Dim paragraph As Paragraph = New Paragraph()
        paragraph.SetMultipliedLeading(10.0F)
        Return paragraph
    End Function
    Protected Function AddParagraph(doc As iText.Layout.Document, space As Single, isBold As Boolean, content As Text, additionalContent As Text) As iText.Layout.Document
        Dim paragraph As Paragraph = New Paragraph()
        paragraph.SetMultipliedLeading(space)
        paragraph.SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
        content = If(isBold, content.SetBold(), content)
        paragraph.Add(content)
        paragraph.Add(additionalContent)
        Dim document As iText.Layout.Document = doc.Add(paragraph)
        Return document
    End Function
    Protected Sub PrintPDFMailBox(nameMBox As String, mails As IList(Of PECMail), mailsSpostate As IList(Of PECMail))

        Dim nPecTotali As Integer = 0
        Dim nPecValide As Integer = 0
        Dim nAnomalie As Integer = 0
        Dim nInterOper As Integer = 0
        Dim nInoltrate As Integer = 0
        Dim nProtocollate As Integer = 0
        Dim nGestite As Integer = 0
        Dim nSpostate As Integer = 0
        Dim nCancellate As Integer = 0
        Dim nPecInviate As Integer = 0
        Dim nInvDaProt As Integer = 0
        Dim nInvDaGest As Integer = 0

        AddParagraph(_doc, 4, True, New Text("REPORTISTICA PEC"), New Text(""))
        '"Casella di posta PEC: " + nameMBox
        AddParagraph(_doc, 3, True, New Text("Casella di posta PEC: "), New Text(nameMBox))

        'Anno e Mese selezionato
        Dim anno As Integer = Convert.ToInt32(Years.SelectedItem.Value)
        Dim mese As Integer = Convert.ToInt32(Months.SelectedIndex + 1)
        Dim dalGiorno As DateTime = New DateTime(anno, mese, 1)
        AddParagraph(_doc, 3, True, New Text("Dal giorno: "), New Text(dalGiorno.ToString("dd/MM/yyyy")))

        Dim alGiorno As DateTime = New DateTime(anno, mese, DateTime.DaysInMonth(anno, mese))
        AddParagraph(_doc, 2, True, New Text("Al giorno: "), New Text(alGiorno.ToString("dd/MM/yyyy")))
        AddParagraph(_doc, 3, True, New Text(""), New Text(""))

        For Each mail As PECMail In mails
            If mail.Direction = PECMailDirection.Ingoing Then
                If mail.MailRecipients IsNot Nothing AndAlso mail.XTrasporto IsNot Nothing Then
                    nPecTotali += 1

                    If mail.RecordedInDocSuite IsNot Nothing AndAlso mail.RecordedInDocSuite = 1 Then
                        nProtocollate += 1
                    End If

                    If mail.XTrasporto.Trim.Eq("errore") Then
                        nAnomalie += 1
                    Else
                        nPecValide += 1
                    End If

                    If mail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Delete) Then
                        nCancellate += 1
                    End If

                    If mail.IsValidForInterop Then
                        nInterOper += 1
                    End If

                    If nameMBox.Substring(4).Trim().ContainsIgnoreCase(mail.MailRecipients) Then
                        nInoltrate += 1
                    End If
                End If
            Else
                nPecInviate += 1
                If mail.RecordedInDocSuite IsNot Nothing AndAlso mail.RecordedInDocSuite = 1 Then
                    nInvDaProt += 1
                End If

            End If
        Next

        For Each ms As PECMail In mailsSpostate
            If Not String.IsNullOrEmpty(ms.Handler) Then
                nGestite += 1
            Else
                nSpostate += 1
            End If
        Next

        nInvDaGest = nPecInviate - nInvDaProt

        Dim datatable As New Table(2)
        datatable.SetPadding(2)
        datatable.SetWidth(UnitValue.CreatePercentValue(100))
        datatable.SetMarginTop(5)
        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph("Numero totale di PEC ricevute")).SetBold().SetTextAlignment(TextAlignment.LEFT).SetWidth(UnitValue.CreatePercentValue(75)))
        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph(nPecTotali.ToString)).SetBold().SetTextAlignment(TextAlignment.RIGHT).SetWidth(UnitValue.CreatePercentValue(25)))

        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph("Dirette")).SetTextAlignment(TextAlignment.LEFT))
        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph((nPecValide + nAnomalie).ToString)).SetTextAlignment(TextAlignment.RIGHT))

        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph("Da inoltro")).SetTextAlignment(TextAlignment.LEFT))
        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph(nInoltrate.ToString)).SetTextAlignment(TextAlignment.RIGHT))

        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph("Numero totale di PEC inviate")).SetBold().SetTextAlignment(TextAlignment.LEFT))
        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph(nPecInviate.ToString)).SetTextAlignment(TextAlignment.RIGHT))

        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph("Da Protocoll")).SetTextAlignment(TextAlignment.LEFT))
        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph(nInvDaProt.ToString)).SetTextAlignment(TextAlignment.RIGHT))

        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph("Da Gestione")).SetTextAlignment(TextAlignment.LEFT))
        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph(nInvDaGest.ToString)).SetTextAlignment(TextAlignment.RIGHT))

        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph("")).SetTextAlignment(TextAlignment.LEFT))
        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph("")).SetTextAlignment(TextAlignment.RIGHT))

        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph("Categorie PEC ricevute")).SetBold().SetTextAlignment(TextAlignment.LEFT))
        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph("")).SetTextAlignment(TextAlignment.RIGHT))

        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph("Anomalie")).SetTextAlignment(TextAlignment.LEFT))
        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph(nAnomalie.ToString)).SetTextAlignment(TextAlignment.RIGHT))

        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph("PEC Valide")).SetTextAlignment(TextAlignment.LEFT))
        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph((nPecValide - nInterOper).ToString)).SetTextAlignment(TextAlignment.RIGHT))

        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph("PEC Interoperabili")).SetTextAlignment(TextAlignment.LEFT))
        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph(nInterOper.ToString)).SetTextAlignment(TextAlignment.RIGHT))

        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph("   ")).SetTextAlignment(TextAlignment.LEFT))
        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph("   ")).SetTextAlignment(TextAlignment.RIGHT))

        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph("Esito PEC Ricevute")).SetBold().SetTextAlignment(TextAlignment.LEFT))
        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph("")).SetTextAlignment(TextAlignment.RIGHT))

        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph("Protocollate")).SetTextAlignment(TextAlignment.LEFT))
        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph(nProtocollate.ToString)).SetTextAlignment(TextAlignment.RIGHT))

        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph("Gestite")).SetTextAlignment(TextAlignment.LEFT))
        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph(nGestite.ToString)).SetTextAlignment(TextAlignment.RIGHT))

        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph("Cancellate")).SetTextAlignment(TextAlignment.LEFT))
        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph(nCancellate.ToString)).SetTextAlignment(TextAlignment.RIGHT))

        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph("Spostate")).SetTextAlignment(TextAlignment.LEFT))
        datatable.AddCell(New Cell().SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).Add(New Paragraph(nSpostate.ToString)).SetTextAlignment(TextAlignment.RIGHT))
        _doc.Add(datatable)

    End Sub

    Protected Sub GetMailboxMails(mboxID As Short(), mboxName As String, ByRef mails As IList(Of PECMail), ByRef mailsSpostate As IList(Of PECMail))
        Dim pecMailFinder As NHibernatePECMailFinder = Facade.PECMailFinder

        pecMailFinder.EnablePaging = False
        pecMailFinder.TopMaxRecords = 0
        pecMailFinder.Direction = PECMailDirection.Ingoing
        pecMailFinder.Actives = Nothing
        pecMailFinder.MailboxIds = mboxID
        pecMailFinder.IncludeNormalAndMultiples = True
        Dim anno As Integer = Convert.ToInt32(Years.SelectedItem.Value)
        Dim mese As Integer = Convert.ToInt32(Months.SelectedIndex + 1)
        pecMailFinder.MailDateFrom = New DateTime(anno, mese, 1)
        pecMailFinder.MailDateTo = New DateTime(anno, mese, DateTime.DaysInMonth(anno, mese))


        mails = pecMailFinder.DoSearch()

        pecMailFinder.Direction = PECMailDirection.Outgoing
        Dim mailsOut As IList(Of PECMail) = pecMailFinder.DoSearch()

        If mailsOut IsNot Nothing And mailsOut.Count > 0 Then
            For Each m As PECMail In mailsOut
                mails.Add(m)
            Next
        End If

        pecMailFinder.MailboxIds = Nothing
        pecMailFinder.Recipient = mboxName
        pecMailFinder.Direction = PECMailDirection.Ingoing

        mailsSpostate = New List(Of PECMail)
        Dim mailsSp As IList(Of PECMail) = pecMailFinder.DoSearch()
        If mailsSp IsNot Nothing And mailsSp.Count > 0 Then
            For Each m As PECMail In mailsSp
                If m.MailBox.Id <> mboxID(0) Then
                    mailsSpostate.Add(m)
                End If
            Next
        End If
    End Sub

    Private Sub loadMonths()
        ' carico i mesi
        Months.Items.Clear()
        For i As Integer = 1 To 12
            Dim monthName As String = CultureInfo.CurrentCulture.DateTimeFormat.MonthGenitiveNames(i - 1)
            Months.Items.Add(New System.Web.UI.WebControls.ListItem(StringHelper.UppercaseFirst(monthName), i.ToString()))
        Next
    End Sub

    ''' <param name="year">Anno da dove iniziare a popolare la dropdownlist Years</param>
    Sub loadYears(ByVal year As Integer)
        Years.Items.Clear()
        For i As Integer = year To DateTime.Now.Year
            Years.Items.Add(i.ToString())
        Next
    End Sub

#End Region

End Class