Imports System.Net.Mail
Imports System.Text
Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web

''' <summary> Classe di utilità per la gestione pratiche che racchiude alcuni metodi comuni </summary>
<Obsolete("Spostare tutto nelle facade appropriate")>
Public Class DocumentUtil

#Region "Fields"
    Private Shared _instance As DocumentUtil
    Private _envGrouSelected As String = Nothing
#End Region

#Region "Context"
    Private ReadOnly Property DocumentEnv() As DocumentEnv
        Get
            Return DocSuiteContext.Current.DocumentEnv
        End Get
    End Property
#End Region

#Region "Singleton"
    Public Shared Function GetInstance() As DocumentUtil
        If _instance Is Nothing Then
            _instance = New DocumentUtil
        End If
        Return (_instance)
    End Function

#End Region

#Region "Properties"

#End Region

#Region "Calcola Path"
    Public Function FncCalcolaPath(ByRef idRole As Integer, ByRef idRoleIncremental As Integer, _
            ByRef fullIncremental As String, ByRef titolo As String, _
            ByVal year As Short, ByVal number As Integer, ByVal newIncremental As Short, ByVal accesso As Boolean) As Boolean
        
        Dim yearnumberincr As New YearNumberIncrCompositeKey()
        With yearnumberincr
            .Year = year
            .Number = number

            If accesso Then
                .Incremental = newIncremental
            End If

        End With

        Dim documentFolderFacade As New DocumentFolderFacade()
        Dim documentfolder As DocumentFolder = documentFolderFacade.GetById(yearnumberincr)

        If Not documentfolder Is Nothing Then
            If Not String.IsNullOrEmpty(titolo) Then
                titolo = "/" & titolo
            End If
            If Not String.IsNullOrEmpty(fullIncremental) Then
                fullIncremental = "|" & fullIncremental
            End If
            fullIncremental = documentfolder.Incremental & fullIncremental
            If documentfolder.Role Is Nothing Then
                titolo = documentfolder.FolderName & titolo
            Else
                titolo = documentfolder.Role.Name & titolo
                If idRole = 0 Then
                    idRole = documentfolder.Role.Id
                    idRoleIncremental = documentfolder.Incremental
                End If
            End If
            If documentfolder.IncrementalFather.HasValue Then
                FncCalcolaPath(idRole, idRoleIncremental, fullIncremental, titolo, year, number, documentfolder.IncrementalFather.Value, True)
            End If
        End If
    End Function
#End Region

#Region "Calcola Cartella"
    Public Sub FncCalcolaCartella(ByVal Year As Short, ByVal Number As Integer, ByRef Titolo As String, ByRef NewIncremental As Integer)
        Dim YearNumberInc As YearNumberIncrCompositeKey = New YearNumberIncrCompositeKey()

        With YearNumberInc
            .Year = Year
            .Number = Number
            .Incremental = NewIncremental

        End With

        Dim folder As DocumentFolder = New DocumentFolderFacade().GetById(YearNumberInc)
        If Not folder Is Nothing Then
            If Titolo <> "" Then Titolo = "/" & Titolo
            If folder.Role Is Nothing Then
                Titolo = folder.FolderName.ToString() & Titolo
            Else
                Titolo = folder.Role.Name.ToString() & Titolo
            End If
            If folder.IncrementalFather.HasValue Then
                FncCalcolaCartella(Year, Number, Titolo, folder.IncrementalFather)
            End If
        End If
    End Sub
#End Region

#Region "Send Mail"
    Public Function FncSendMail(ByRef source As IList(Of Role), ByRef destination As IList(Of Role), ByRef tokenType As DocumentTabToken, _
                                ByVal Year As Integer, ByVal Number As Long, _
                                ByVal ExpiryDate As Nullable(Of Date), ByVal sObject As String, _
                                ByVal Reason As String, ByVal Note As String, _
                                ByVal Response As String, ByVal ReasonResponse As String,
                                Optional IsDispositionNotification As Boolean = False) As Boolean
        Dim s As String
        Dim DocmDate As Date
        Dim DocmObject As String
        Dim RoleFrom As String = String.Empty
        Dim RoleTo As String = String.Empty
        Dim ResponseDescription As String
        Dim ResponseLabel As String

        'Controlla se abilitato flag per invio Mail
        If Not DocumentEnv.IsMailEnabled Then
            Return True
        End If

        'Controlla se abilitata invio Mail per quella tipo di token
        If tokenType IsNot Nothing Then
            If tokenType.SendMail = 0 Then
                Return False
            End If
        Else
            Return False
        End If

        'Controlla l'esistenza della pratica
        Dim facade As New DocumentFacade
        Dim document As Document = facade.GetById(Year, Number)
        If document Is Nothing Then
            Return False
        End If

        'Verifica se impostato SMTP
        If String.IsNullOrEmpty(DocumentEnv.MailSmtpServer) Then
            Throw New DocSuiteException("Impossibile inviare") With {.Descrizione = "Manca la definizione: MailSmtpServer"}
        End If

        DocmDate = document.StartDate
        DocmObject = document.DocumentObject

        Dim mm As New MailMessage
        Dim from As String = String.Empty
        mm.IsBodyHtml = True
        For Each mittente As Role In source
            If mittente IsNot Nothing Then
                'Appendo al campo from
                If Not String.IsNullOrEmpty(from) Then
                    from &= ";"
                End If
                'Appendo al campo RoleFrom
                If Not String.IsNullOrEmpty(RoleFrom) Then
                    RoleFrom &= ";"
                End If
                'verifico esistenza indirizzo email
                If String.IsNullOrEmpty(mittente.EMailAddress) Then
                    Throw New DocSuiteException("Impossibile inviare") With {.Descrizione = String.Format("Nel Settore {0} manca l'indirizzo Email", mittente.Name)}
                End If
                'imposto indirizzo email
                from &= mittente.EMailAddress
                s = ""
                SetRoleString(s, mittente.FullIncrementalPath)
                RoleFrom &= s
            End If
        Next

        'aggiungo elenco mittenti alla email
        mm.From = New MailAddress(from)

#If DEBUG Then
        mm.From = New MailAddress("fabrizio.gaiardo@vecompsw.emea.microsoftonline.com")
#End If

        For Each destinatario As Role In destination
            If destinatario IsNot Nothing Then
                'Appendo al campo RoleTo
                If Not String.IsNullOrEmpty(RoleTo) Then
                    RoleTo &= ";"
                End If
                'verifico esistenza indirizzo email
                If String.IsNullOrEmpty(destinatario.EMailAddress) Then
                    Throw New DocSuiteException("Impossibile inviare") With {.Descrizione = String.Format("Nel Settore {0} manca l'indirizzo Email", destinatario.Name)}
                ElseIf Not RegexHelper.IsValidEmail(destinatario.EMailAddress) Then
                    Throw New DocSuiteException("Impossibile inviare") With {.Descrizione = String.Format("Nel Settore {0} l'indirizzo Email non è valido. Verificare l'indirizzo associato al settore e riprovare", destinatario.Name)}
                End If
                'imposto indirizzo email
                mm.To.Add(New MailAddress(destinatario.EMailAddress))
                s = ""
                SetRoleString(s, destinatario.FullIncrementalPath)
                RoleTo &= s
            End If
        Next

#If DEBUG Then
        mm.To.Clear()
        mm.To.Add(New MailAddress("fabrizio.gaiardo@vecompsw.emea.microsoftonline.com"))
#End If

        Select Case Response
            Case "N"
                ResponseDescription = "<B>RIFIUTATA</B><BR><BR>"
                ResponseLabel = "Motivo del Rifiuto:"
            Case "A"
                ResponseDescription = "<B>RICHIAMO</B><BR><BR>"
                ResponseLabel = "Motivo del Richiamo:"
            Case Else
                ResponseDescription = String.Empty
                ResponseLabel = "Motivo della Restituzione:"
        End Select

        ' Creazione oggetto della mail
        mm.Subject = String.Format("{0} Pratica n. {1} {2:0000000} del {3:dd/MM/yyyy}", DocSuiteContext.ProductName, Year, Number, DocmDate)

        ' Creazione corpo della mail
        Dim sb As New StringBuilder
        sb.AppendFormat("Oggetto della Pratica: <b>{0}</b><br/><br/>Tipologia Richiesta: <b>{1}</b><br/><br/>{2}Mittente: <b>{3}</b><br/>",
            DocmObject, tokenType.Description, ResponseDescription, RoleFrom)
        If destination.Count > 1 Then
            sb.AppendFormat("Destinatari: <br/>")
        Else
            sb.Append("Destinatario: ")
        End If
        sb.AppendFormat("<b>{0}</b><br /><br />", RoleTo)
        If ExpiryDate.HasValue Then
            sb.AppendFormat("Data scadenza: {0:dd/MM/yyyy}<br/>", ExpiryDate.Value)
        End If
        If Not String.IsNullOrEmpty(sObject) Then
            sb.AppendFormat("Oggetto: {0}<br/>", sObject)
        End If
        If Not String.IsNullOrEmpty(Reason) Then
            sb.AppendFormat("Motivo: {0}<br/>", Reason)
        End If
        If Not String.IsNullOrEmpty(Note) Then
            sb.AppendFormat("Note: {0}<br/>", Note)
        End If
        If Not String.IsNullOrEmpty(ReasonResponse) Then
            sb.AppendFormat("{0} <b>{1}</b><br/>", ResponseLabel, ReasonResponse)
        End If
        sb.AppendFormat("<br/><a href='{0}?Tipo=Docm&Azione=Apri&Anno={1}&Numero={2}'>", DocSuiteContext.Current.CurrentTenant.DSWUrl, Year, Number)
        sb.AppendFormat("Pratica n. {0} {1:0000000} del {2:dd/MM/yyyy}</a>", Year, Number, DocmDate)
        mm.Body = sb.ToString()

        ' Invio mail
        Dim smtp As SmtpClient = New SmtpClient(DocumentEnv.MailSmtpServer)
        smtp.UseDefaultCredentials = True
        smtp.Send(mm)

        Return True
    End Function

    Public Sub SetRoleString(ByRef Testo As String, ByVal FullIncremental As String)
        Dim facade As RoleFacade = New RoleFacade("DocmDB")
        Dim role As Role = Nothing

        'Uscita funzione ricorsiva
        If String.IsNullOrEmpty(FullIncremental) Then
            Exit Sub
        End If

        Dim r As Array = Split(FullIncremental, "|")
        role = facade.GetById(r(0))
        If role IsNot Nothing Then
            If Not String.IsNullOrEmpty(Testo) Then
                Testo &= "/"
            End If
            Testo &= role.Name
            If r.Length > 1 Then
                SetRoleString(Testo, Mid$(FullIncremental, InStr(FullIncremental, "|") + 1))
            End If
        End If
    End Sub
#End Region

#Region "Document Util"

    Public Shared Function DocmFull(ByVal Year As Short, ByVal Number As Integer, Optional ByVal Slash As String = "/") As String
        Dim s As String = String.Empty
        s = Year & Slash & String.Format("{0:0000000}", Number)
        Return s
    End Function

#End Region

    Public Shared Function DocLinkVerify(ByRef form As System.Web.UI.Page, ByVal link As String, ByVal incremental As Short, ByVal year As Short, ByVal number As Integer) As Boolean

        Dim dobjFacade As New DocumentObjectFacade

        Select Case DocSuiteContext.Current.DocumentEnv.EnvLinkVerify
            Case "0", ""
            Case "1"
                If dobjFacade.DocumentObjectVerifyLink(year, number, link, incremental) Then
                    WebHelper.Alert(form, "Collegamento già inserito nella Pratica")
                    Return True
                End If
            Case "2"
                If dobjFacade.DocumentObjectVerifyLink(year, number, link) Then
                    WebHelper.Alert(form, "Collegamento già inserito nella Pratica")
                    Return True
                End If
        End Select
        Return False
    End Function

    Public Sub SendMailCC(ByRef destination As IList(Of Role), ByVal year As Integer, ByVal number As Long, ByVal folder As String, ByVal file As String)

        Dim DocmDate As Date
        Dim DocmObject As String
        Dim RoleFrom As String = String.Empty
        Dim RoleTo As String = String.Empty

        'Verifica se impostato SMTP
        If String.IsNullOrEmpty(DocumentEnv.MailSmtpServer) Then
            Throw New Exception("In ParameterEnv manca la definizione: MailSmtpServer")
            'Return False
        End If

        'Controlla l'esistenza della pratica
        Dim facade As New DocumentFacade
        Dim document As Document = facade.GetById(year, number)
        If document Is Nothing Then
            Throw New Exception("Pratica non trovata.#" & year & "-" & number)
        End If

        DocmDate = document.StartDate.Value
        DocmObject = document.DocumentObject

        Dim mm As New MailMessage
        Dim from As String = String.Empty
        mm.IsBodyHtml = True

        'verifico esistenza indirizzo email
        If String.IsNullOrEmpty(CommonUtil.GetInstance().UserMail) Then
            Throw New Exception(String.Concat("All'utente ", DocSuiteContext.Current.User.FullUserName, " manca l\'indirizzo Email"))
            'Return False
        End If
        'imposto indirizzo email
        from = CommonUtil.GetInstance().UserMail
        RoleFrom = DocSuiteContext.Current.User.FullUserName

        mm.From = New MailAddress(from)

        For Each destinatario As Role In destination
            If destinatario IsNot Nothing Then
                'Appendo al campo RoleTo
                If Not String.IsNullOrEmpty(RoleTo) Then
                    RoleTo &= ";"
                End If
                'verifico esistenza indirizzo email
                If String.IsNullOrEmpty(destinatario.EMailAddress) Then
                    Throw New Exception("Nel Settore " & destinatario.Name & " manca l\'indirizzo Email")
                    'Return False
                End If
                'imposto indirizzo email
                If destinatario.EMailAddress.IndexOf(";"c) <> -1 Or destinatario.EMailAddress.IndexOf(","c) <> -1 Then
                    For Each mailAddr As String In destinatario.EMailAddress.Split(";"c, ","c)
                        mm.To.Add(New MailAddress(mailAddr.Trim()))
                    Next
                Else
                    mm.To.Add(New MailAddress(destinatario.EMailAddress.Trim()))
                End If

                Dim s As String = ""
                SetRoleString(s, destinatario.FullIncrementalPath)
                RoleTo &= s
            End If
        Next

        'creazione oggetto della mail
        mm.Subject = DocSuiteContext.ProductName & " Pratica n. " & year & " " &
                     String.Format("{0:0000000}", number) &
                     " del " & String.Format("{0:dd/MM/yyyy}", DocmDate)

        'creazione corpo della mail
        Dim dest As String = "Destinatari:<BR>"
        If destination.Count = 1 Then
            dest = "Destinatario: "
        End If
        mm.Body = "Oggetto della Pratica: <B>" & DocmObject & "</B><BR><BR>" &
                  "Mittente: <B>" & RoleFrom & "</B><BR>" &
                  dest & "<B>" & RoleTo & "</B><BR>"

        mm.Body &=
            "<BR>" &
            "<a href='" & DocSuiteContext.Current.CurrentTenant.DSWUrl & "?Tipo=Docm&Azione=Apri&Anno=" & year & "&Numero=" & number & "'>" &
            "Pratica n. " & year & " " & String.Format("{0:0000000}", number) & " del " & String.Format("{0:dd/MM/yyyy}", DocmDate) & "</a><BR>" &
            "Cartella: <B>" & folder & "</B><br>Documento: <B>" & file & "</B>"

        Try
            'Invio mail
            Dim ss As SmtpClient = New SmtpClient(DocumentEnv.MailSmtpServer)
            ss.UseDefaultCredentials = True
            ss.Send(mm)
        Catch exc As Exception
            Throw New DocSuiteException("Si sono verificati degli errori durante l'invio della mail.", exc)
        End Try

    End Sub

    Public Shared Sub FncCalcolaCartella(ByVal docFolder As DocumentFolder, ByRef Titolo As String, ByRef NewIncremental As Integer, Optional ByVal SLASH As String = "/")

        Dim YearNumberInc As New YearNumberIncrCompositeKey
        Dim folder As DocumentFolder

        With YearNumberInc
            .Year = docFolder.Year
            .Number = docFolder.Number
            .Incremental = NewIncremental
        End With

        Dim dfFacade As New DocumentFolderFacade()
        folder = dfFacade.GetById(YearNumberInc)

        If Not folder Is Nothing Then
            If Titolo <> "" Then Titolo = SLASH & Titolo
            If folder.Role Is Nothing Then
                Titolo = folder.FolderName.ToString() & Titolo
            Else
                Titolo = folder.Role.Name.ToString() & Titolo
            End If
            If folder.IncrementalFather.HasValue Then
                FncCalcolaCartella(folder, Titolo, folder.IncrementalFather)
            End If
        End If

    End Sub

End Class
