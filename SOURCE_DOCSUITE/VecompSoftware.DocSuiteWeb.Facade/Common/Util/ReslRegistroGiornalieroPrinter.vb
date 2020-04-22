Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports System.IO
Imports System.Web
Imports Microsoft.Reporting.WebForms

''' <summary>Classe per la creazione del registro giornaliero degli atti</summary>
''' <remarks>Copiato in parte da ResolutionJournalPrinter in attesa della standardizzazione</remarks>
Public Class ReslRegistroGiornalieroPrinter

#Region " Methods "

    ''' <summary>Crea il report</summary>
    ''' <param name="finder">Criteri di ricerca degli atti</param>
    ''' <param name="folder">Directory temporanea</param>
    ''' <param name="containerNames">Contenitori selezionati nella ricerca</param>
    ''' <param name="type">Tipo di atto</param>
    ''' <returns>File creato</returns>
    Public Shared Function CreatePrint(ByRef finder As NHibernateResolutionFinder, folder As DirectoryInfo, containerNames As String, type As String) As FileInfo
        folder.Refresh()
        If Not folder.Exists Then
            Throw New DirectoryNotFoundException(String.Format("Errore in accesso alla directory temporanea [{0}]", folder.FullName))
        End If

        Dim resolutionList As IList(Of RegistroGiornaliero) = GetData(finder)
        If resolutionList.Count = 0 Then
            Throw New DocSuiteException(String.Format("Nessuna [{0}] trovata. Contenitori selezionati per la ricerca: {1}", type, containerNames))
        End If

        Dim reportTemplate As New FileInfo(HttpContext.Current.Server.MapPath("~/Resl/Stampe/ResolutionRegistroGiornaliero.rdlc"))
        If Not reportTemplate.Exists Then
            Throw New FileNotFoundException(String.Format("Template [{0}] non esistente", reportTemplate.Name))
        End If

        Dim report As New ReportViewer()
        report.LocalReport.ReportPath = reportTemplate.FullName
        report.LocalReport.DataSources.Add(New ReportDataSource("RegistroGiornalieroDataSet", resolutionList))

        ' Imposto i parametri
        Dim parameters As New List(Of ReportParameter)(3)
        parameters.Add(New ReportParameter("Azienda", DocSuiteContext.Current.ProtocolEnv.CorporateName))
        parameters.Add(New ReportParameter("Tipologia", type))
        parameters.Add(New ReportParameter("Contenitori", containerNames))
        report.LocalReport.SetParameters(parameters)

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

    ''' <summary>Estrae dagli atti passati le righe di stampa</summary>
    ''' <remarks>Public perchè deve poter essere vista dal componente "Report Data" del reportviewer</remarks>
    Public Shared Function GetData(ByRef finder As NHibernateResolutionFinder) As List(Of RegistroGiornaliero)

        Dim registri As New List(Of RegistroGiornaliero)

        Dim recipientFacade As New RecipientFacade("ReslDB")
        Dim resolutionFacade As New ResolutionFacade()
        Dim number As Boolean
        Select Case True
            Case resolutionFacade.IsManagedProperty("ServiceNumber", ResolutionType.IdentifierDetermina)
                number = False
            Case resolutionFacade.IsManagedProperty("Number", ResolutionType.IdentifierDetermina)
                number = True
        End Select

        For Each item As Resolution In finder.DoSearch()
            Dim registro As New RegistroGiornaliero
            With registro
                ' Data adozione
                .DataAdozione = item.AdoptionDate.Value

                ' Calcolo numero dell'atto
                Dim fullId As String = ""
                Dim fullNumber As String = ""
                resolutionFacade.ReslFullNumber(item, item.Type.Id, fullId, fullNumber)
                .NumeroDelibera = If(String.IsNullOrEmpty(fullNumber), fullId, fullNumber)

                .DataPubblicazione = If(number, item.EffectivenessDate, item.PublishingDate)

                .Oggetto = StringHelper.ReplaceCrLf(item.ResolutionObject)

                ' Proponenti
                Dim esi As String = String.Empty
                GetContacts(item.ResolutionContacts, "P", esi)
                esi &= getRecipientFullName(recipientFacade, item.Id, item.IdProposer, Not String.IsNullOrEmpty(esi))
                If Not String.IsNullOrEmpty(item.AlternativeProposer) Then
                    esi &= If(Not String.IsNullOrEmpty(esi), "<BR>", "") & item.AlternativeProposer
                End If
                .Esibitore = esi

                ' Destinatari
                Dim des As String = String.Empty
                GetContacts(item.ResolutionContacts, "D", des)
                If Not String.IsNullOrEmpty(item.AlternativeRecipient) Then
                    des &= If(Not String.IsNullOrEmpty(des), "<BR>", "") & item.AlternativeRecipient
                End If
                .Destinatari = des

                Dim s As String = calcolaStato(item.Status.Id)
                If s <> "Attivo" Then
                    s &= "<BR>" & StringHelper.ReplaceCrLf(item.LastChangedReason)
                End If
                .Stato = s
            End With
            registri.Add(registro)
        Next

        Return registri
    End Function

    Private Shared Sub GetContacts(ByVal contacts As Object, ByVal communicationType As String, ByRef s As String)
        ContactFacade.FormatContacts(contacts, communicationType, s)

        Dim sRet As String = String.Empty
        Dim arr As String() = s.Split("#"c)
        For i As Integer = 0 To arr.Length - 1
            If arr(i) <> "§§§" Then
                If Not String.IsNullOrEmpty(arr(i)) Then sRet &= arr(i) & "<BR>"
                For j As Integer = i + 1 To arr.Length - 1
                    If arr(i) = arr(j) Then
                        arr(j) = "§§§"
                    End If
                Next
            End If
        Next i

        If sRet.Contains("<BR>") Then
            s = sRet.Substring(0, sRet.Length - 4)
        Else
            s = sRet
        End If

    End Sub

    Private Shared Function getRecipientFullName(ByRef recipientFacade As RecipientFacade, ByVal idResolution As Integer, ByVal idRecipient As Nullable(Of Short), ByVal br As Boolean) As String
        Dim sRet As String = String.Empty

        If idRecipient.HasValue Then
            Dim rec As Recipient = recipientFacade.GetById(idRecipient)
            If rec IsNot Nothing Then
                If br Then sRet &= "<BR>"
                sRet &= rec.FullName
            End If
        End If

        Return sRet
    End Function

    Private Shared Function calcolaStato(ByVal iStato As Integer) As String
        Dim s As String = String.Empty
        Select Case iStato
            Case -5 : s = "Temporaneo"
            Case -4 : s = "Rettificato"
            Case -3 : s = "Revocato"
            Case -2 : s = "Annullato"
            Case -1 : s = "Errato"
            Case 0 : s = "Attivo"
        End Select
        Return s
    End Function

#End Region

End Class
