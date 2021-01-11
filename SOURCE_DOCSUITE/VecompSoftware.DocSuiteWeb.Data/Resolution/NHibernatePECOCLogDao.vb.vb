Imports VecompSoftware.NHibernateManager.Dao
Imports System.Text

Public Class NHibernatePECOCLogDao
    Inherits BaseNHibernateDao(Of PECOCLog)

    ''' <summary>
    ''' Data la richiesta, genera il log corrispondente allo stato attuale della stessa.
    ''' </summary>
    Public Sub InsertLog(ByVal pecOc As PECOC, ByVal message As String)

        Dim description As New StringBuilder()

        Select Case pecOc.Status
            Case PECOCStatus.Aggiunto
                description.Append("Richiesta creata correttamente e pronta ad essere processata.")
            Case PECOCStatus.Errore
                description.Append("Errore nella creazione del file di zip o della mail, contattare l'assistenza.")
            Case PECOCStatus.Elaborazione
                description.Append("Richiesta in elaborazione.")
            Case PECOCStatus.Completo
                description.Append("Estrazione dello zip completata, in attesa di essere spedita.")
            Case PECOCStatus.Spedito
                description.Append("PEC inviata.")
            Case PECOCStatus.Cancellato
                description.Append("Richiesta cancellata da utente.")
            Case PECOCStatus.Vuoto
                description.Append("Nessun atto trovato nel periodo.")
            Case Else
                Throw New NotImplementedException("Caso non previsto.")
        End Select

        If Not String.IsNullOrEmpty(message) Then
            description.Append(" ")
            description.Append(message)
        End If

        Dim log As New PECOCLog()
        log.PecOc = pecOc
        log.Date = Date.Now
        log.Description = description.ToString()
        log.SystemComputer = DocSuiteContext.Current.UserComputer
        log.SystemUser = DocSuiteContext.Current.User.FullUserName

        Save(log)
    End Sub

End Class
