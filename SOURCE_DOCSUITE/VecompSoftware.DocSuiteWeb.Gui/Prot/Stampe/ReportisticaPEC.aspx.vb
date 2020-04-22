Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports System.Globalization
Imports VecompSoftware.DocSuiteWeb.Data.PEC.Finder

Public Class ReportisticaPEC
    Inherits PECBasePage

#Region " Fields "

    Dim _pdfData As New MemoryStream
    Dim _doc As New iTextSharp.text.Document(PageSize.A4, 50, 50, 80, 50)
    Dim _writer As PdfWriter = PdfWriter.GetInstance(_doc, _pdfData)

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
            _writer.ViewerPreferences = PdfWriter.PageModeUseOutlines
            _doc.Open()

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
        For Each mailbox As PECMailBox In MailBoxes
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

    Protected Function GetParPDFBold(testo As String) As Paragraph
        Dim par As New Paragraph(testo, New Font(Font.FontFamily.HELVETICA, 10.0F, Font.BOLD))
        Return par
    End Function

    Protected Function GetParPDFNormal(testo As String) As Paragraph
        Dim par As New Paragraph(testo, New Font(Font.FontFamily.HELVETICA, 10.0F, Font.NORMAL))
        Return par
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

        Dim bigBoldFont As New Font(Font.FontFamily.HELVETICA, 12.0F, Font.BOLD)
        Dim boldFont As New Font(Font.FontFamily.HELVETICA, 10.0F, Font.BOLD)
        Dim normalFont As New Font(Font.FontFamily.HELVETICA, 10.0F, Font.NORMAL)

        If _pdfData.Length > 0 Then
            _doc.NewPage()
        End If

        _doc.Add(New Paragraph("REPORTISTICA PEC", bigBoldFont))
        _doc.Add(New Paragraph(" "))
        '"Casella di posta PEC: " + nameMBox

        Dim par As New Paragraph()
        par.Add(New Phrase("Casella di posta PEC: ", boldFont))
        par.Add(New Phrase(nameMBox, normalFont))
        _doc.Add(par)

        _doc.Add(New Paragraph(" "))

        par = New Paragraph()
        par.Add(New Phrase("Dal giorno: ", boldFont))
        'Anno e Mese selezionato
        Dim anno As Integer = Convert.ToInt32(Years.SelectedItem.Value)
        Dim mese As Integer = Convert.ToInt32(Months.SelectedIndex + 1)
        Dim dalGiorno As DateTime = New DateTime(anno, mese, 1)
        par.Add(New Phrase(dalGiorno.ToString("dd/MM/yyyy"), normalFont))
        par.Add("               ")
        par.Add(New Phrase("Al giorno: ", boldFont))
        Dim alGiorno As DateTime = New DateTime(anno, mese, DateTime.DaysInMonth(anno, mese))
        par.Add(New Phrase(alGiorno.ToString("dd/MM/yyyy"), normalFont))
        _doc.Add(par)
        _doc.Add(New Paragraph(" "))

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

        Dim datatable As New PdfPTable(2)

        datatable.HorizontalAlignment = Element.ALIGN_LEFT
        datatable.DefaultCell.Padding = 2
        Dim columnWidths As Integer() = {75, 25}
        datatable.SetWidths(columnWidths)
        datatable.WidthPercentage = 100

        datatable.DefaultCell.BorderWidth = 0.7F
        datatable.DefaultCell.GrayFill = 1.0F



        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT
        datatable.AddCell(GetParPDFBold("Numero totale di PEC ricevute"))
        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT
        datatable.AddCell(GetParPDFBold(nPecTotali.ToString))

        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT
        datatable.AddCell(GetParPDFNormal("Dirette"))
        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT
        datatable.AddCell(GetParPDFNormal((nPecValide + nAnomalie).ToString))

        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT
        datatable.AddCell(GetParPDFNormal("Da inoltro"))
        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT
        datatable.AddCell(GetParPDFNormal(nInoltrate.ToString))

        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT
        datatable.AddCell(GetParPDFBold("Numero totale di PEC inviate"))
        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT
        datatable.AddCell(GetParPDFBold(nPecInviate.ToString))

        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT
        datatable.AddCell(GetParPDFNormal("Da Protocollo"))
        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT
        datatable.AddCell(GetParPDFNormal(nInvDaProt.ToString))

        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT
        datatable.AddCell(GetParPDFNormal("Da Gestione"))
        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT
        datatable.AddCell(GetParPDFNormal(nInvDaGest.ToString))

        datatable.AddCell(" ")
        datatable.AddCell(" ")

        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT
        datatable.AddCell(GetParPDFBold("Categorie PEC ricevute"))
        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT
        datatable.AddCell(" ")

        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT
        datatable.AddCell(GetParPDFNormal("Anomalie"))
        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT
        datatable.AddCell(GetParPDFNormal(nAnomalie.ToString))

        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT
        datatable.AddCell(GetParPDFNormal("PEC Valide"))
        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT
        datatable.AddCell(GetParPDFNormal((nPecValide - nInterOper).ToString))

        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT
        datatable.AddCell(GetParPDFNormal("PEC Interoperabili"))
        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT
        datatable.AddCell(GetParPDFNormal(nInterOper.ToString))


        datatable.AddCell(" ")
        datatable.AddCell(" ")


        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT
        datatable.AddCell(GetParPDFBold("Esito PEC Ricevute"))
        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT
        datatable.AddCell(" ")

        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT
        datatable.AddCell(GetParPDFNormal("Protocollate"))
        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT
        datatable.AddCell(GetParPDFNormal(nProtocollate.ToString))

        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT
        datatable.AddCell(GetParPDFNormal("Gestite"))
        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT
        datatable.AddCell(GetParPDFNormal(nGestite.ToString))

        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT
        datatable.AddCell(GetParPDFNormal("Cancellate"))
        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT
        datatable.AddCell(GetParPDFNormal(nCancellate.ToString))

        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT
        datatable.AddCell(GetParPDFNormal("Spostate"))
        datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT
        datatable.AddCell(GetParPDFNormal(nSpostate.ToString))

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