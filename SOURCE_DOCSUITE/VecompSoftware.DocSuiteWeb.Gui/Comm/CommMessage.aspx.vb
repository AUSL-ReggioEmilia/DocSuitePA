Imports System.Linq
Imports System.Web
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Text
Imports System.Collections.Generic

Public Class CommMessage
    Inherits CommBasePage

#Region " Fields "

    Private _mailTo As String
    Private _mailBody As String
    Private _subject As String
    Private _body As String
    Private _uniqueIdProtocol As Guid?

#End Region

#Region " Properties "

    Protected ReadOnly Property Action As String
        Get
            Return MailFacade.RetrieveParam(Request.QueryString, "Action")
        End Get
    End Property

    Protected ReadOnly Property UniqueIdProtocol As Guid
        Get
            If Not _uniqueIdProtocol.HasValue Then
                Dim tmp As Guid = Guid.Empty
                Guid.TryParse(MailFacade.RetrieveParam(Request.QueryString, "UniqueIdProtocol"), tmp)
                _uniqueIdProtocol = tmp
            End If
            Return _uniqueIdProtocol.Value
        End Get
    End Property

    Protected ReadOnly Property Year As Short
        Get
            Dim temp As Short
            Short.TryParse(MailFacade.RetrieveParam(Request.QueryString, "Year"), temp)
            Return temp
        End Get
    End Property

    Protected ReadOnly Property Number As Integer
        Get
            Dim temp As Integer
            Integer.TryParse(MailFacade.RetrieveParam(Request.QueryString, "Number"), temp)
            Return temp
        End Get
    End Property

    Protected ReadOnly Property IdResolution As Integer
        Get
            Dim temp As Integer
            Integer.TryParse(MailFacade.RetrieveParam(Request.QueryString, "idResolution"), temp)
            Return temp
        End Get
    End Property

    Protected ReadOnly Property IdCollaboration As Integer
        Get
            Dim temp As Integer
            Integer.TryParse(MailFacade.RetrieveParam(Request.QueryString, "idColl"), temp)
            Return temp
        End Get
    End Property

    Protected ReadOnly Property IdSeriesItem As Integer
        Get
            Dim temp As Integer
            Integer.TryParse(MailFacade.RetrieveParam(Request.QueryString, "idSeriesItem"), temp)
            Return temp
        End Get
    End Property

    Protected ReadOnly Property MailTo As String
        Get
            If String.IsNullOrEmpty(_mailTo) Then
                _mailTo = MailFacade.RetrieveParam(Request.QueryString, "MailTo")
            End If
            Return _mailTo
        End Get
    End Property

    ''' <summary> MailBody impostato da QueryString. </summary>
    Protected ReadOnly Property MailBody As String
        Get
            If String.IsNullOrEmpty(_mailBody) Then
                _mailBody = MailFacade.RetrieveParam(Request.QueryString, "MailBody")
                ' Aggiungo una decodifica quando non passo per l'URL (retrocompatibilità)
                If ProtocolEnv.QuerystringToSession Then
                    _mailBody = HttpUtility.UrlDecode(_mailBody)
                End If
            End If
            Return _mailBody
        End Get
    End Property

    ''' <summary>Tipo atto</summary>
    ''' <remarks>0 Determina   1 Delibera</remarks>
    Protected ReadOnly Property IdTipo As Integer
        Get
            Dim temp As Integer
            Integer.TryParse(MailFacade.RetrieveParam(Request.QueryString, "idTipo"), temp)
            Return temp
        End Get
    End Property

    ''' <summary> Oggetto della Mail, composto a seconda dei parametri action e tipo passati in QuaryString. </summary>
    Public Property Subject As String
        Get
            If String.IsNullOrEmpty(_subject) Then
                _subject = String.Empty
            End If
            Return _subject
        End Get
        Set(value As String)
            _subject = value
        End Set
    End Property

    ''' <summary> Corpo della Mail, composto a seconda dei parametri action e tipo passati in QuaryString. </summary>
    Public Property Body As String
        Get
            If String.IsNullOrEmpty(_body) Then
                _body = String.Empty
            End If
            Return _body
        End Get
        Set(value As String)
            _body = value
        End Set
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False

        ' Se l'utente corrente appartiene al gruppo degli admin allora abilito anche l'editing del html
        If SuperAdminAuthored Then
            MessageBodyEditor.EditModes = EditModes.All
        End If

        If Not Page.IsPostBack Then
            ' Inizializza il sistema di invio mail lato server.
            InizializeSender()
            ComposeMail()
        End If
    End Sub

    Private Sub CmdSendClick(sender As Object, e As EventArgs) Handles cmdSend.Click

        If MessageDest.GetContacts(False).Count = 0 Then
            FileLogger.Warn(LoggerName, "Selezionare almeno un destinatario.")
            AjaxManager.Alert("Selezionare almeno un destinatario.")
            Exit Sub
        End If

        Dim contacts As New List(Of MessageContactEmail)
        contacts.Add(Facade.MessageContactEmailFacade.CreateEmailContact(SenderDescription.Text, DocSuiteContext.Current.User.FullUserName, SenderEmail.Text, MessageContact.ContactPositionEnum.Sender))

        ' Aggiungo i contatti da CONTACTS
        For Each contact As ContactDTO In MessageDest.GetContacts(False)
            Dim temp As String = contact.Contact.EmailAddress
            If String.IsNullOrEmpty(temp) Then
                temp = contact.Contact.CertifiedMail
            End If
            If String.IsNullOrEmpty(temp) Then
                Throw New DocSuiteException("Errore invio", String.Format("Il destinatario ({0}) non ha un indirizzo email.", contact.Contact.DescriptionFormatByContactType))
            End If
            contacts.Add(Facade.MessageContactEmailFacade.CreateEmailContact(contact.Contact.DescriptionFormatByContactType, DocSuiteContext.Current.User.FullUserName, temp, MessageContact.ContactPositionEnum.Recipient))
        Next

        Dim email As MessageEmail = Facade.MessageEmailFacade.CreateEmailMessage(contacts, MessageSubject.Text, MessageBodyEditor.Content, ProtocolEnv.DispositionNotification)

        Try
            Facade.MessageEmailFacade.SendEmailMessage(email)
            AjaxManager.Alert("Email inviata con successo")
            ' Chiudo la finestra
            AjaxManager.ResponseScripts.Add("GetRadWindow().Close()")
        Catch ex As Exception
            FileLogger.Warn(LoggerName, ex.Message, ex)
            AjaxManager.Alert("Errore impossibile inviare Email. Contattare l'assistenza")
        End Try
    End Sub

    Private Sub CmdUndoClick(sender As Object, e As EventArgs) Handles cmdUndo.Click
        MessageSubject.Text = String.Empty
        MessageBodyEditor.Content = String.Empty
    End Sub

#End Region

#Region " Methods "

    ''' <summary> Inizializza il Mittente della mail con l'indirizzo di posta dell'utente corrente. </summary>
    Public Sub InizializeSender()
        SenderDescription.Text = CommonInstance.UserDescription
        SenderEmail.Text = Facade.UserLogFacade.EmailOfUser(DocSuiteContext.Current.User.UserName, DocSuiteContext.Current.User.Domain, True)
    End Sub

    Private Function GetManualContact(email As String) As ContactDTO
        Return MailFacade.CreateManualContact(email, ContactType.Aoo, True)
    End Function

    Public Sub ComposeMail()
        Dim recipients As New List(Of ContactDTO)()

        Select Case Type
            Case "Series"
                GetSeriesItemMailData()

            Case "Docm"
                GetDocumentMailData()

            Case "Prot"
                Select Case Action
                    Case "Mail"
                        Body = GetProtocolMailData()
                        If String.IsNullOrEmpty(MailTo) Then
                            ProtocolLog("invio generico dei riferimenti del protocollo")
                        Else
                            ProtocolLog("invio dei riferimenti del protocollo ai settori")
                        End If

                    Case "Link"
                        GetLinkMailData()
                        If String.IsNullOrEmpty(MailTo) Then
                            ProtocolLog("invio generico del link")
                        Else
                            ProtocolLog("invio del link ai settori")
                        End If
                End Select

            Case "Resl"
                Select Case Action
                    Case "", "Mail"
                        Dim resolution As Resolution = Facade.ResolutionFacade.GetById(IdResolution)
                        If (resolution Is Nothing) Then
                            Exit Sub
                        End If

                        Subject = MailFacade.GetResolutionSubject(resolution)
                        Body = MailFacade.GetResolutionBody(resolution)
                End Select

        End Select

        ' Recupero i destinatari dal parametro "MailTo"
        If Not String.IsNullOrEmpty(MailTo) Then
            Dim addresses As String() = MailTo.Split(";"c)
            addresses.ToList().ForEach(Sub(a) recipients.Add(GetManualContact(a)))
        End If

        MessageDest.DataSource = recipients
        MessageDest.DataBind()

        If Not String.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.MailDisclaimer) Then
            Body = String.Format("{0}<br /><br />{1}", Body, DocSuiteContext.Current.ProtocolEnv.MailDisclaimer)
        End If

        Subject = HttpUtility.HtmlEncode(Subject)
        Subject = Subject.Replace(vbNewLine, " ")
        Body = Body

        MessageSubject.Text = Subject
        MessageBodyEditor.Content = Body


    End Sub

    Private Sub GetDocumentMailData()

        Dim docFacade As New DocumentFacade
        Dim docum As Document = docFacade.GetById(Year, Number)

        If (docum Is Nothing) Then
            Exit Sub
        End If

        Dim sIntestazione As String = "Pratica n. " & _
            Year & "/" & Number & " del " & String.Format("{0:dd/MM/yyyy}", docum.StartDate)

        Subject = Left(DocSuiteContext.ProductName & " " & sIntestazione & " - " & docum.DocumentObject, 150)

        Body = "<b>" & DocSuiteContext.ProductName & " - Gestione Documentale</b><BR>" &
                "<BR>Allego la " & sIntestazione &
                "<BR>Oggetto: " & StringHelper.ReplaceCrLf(HttpUtility.HtmlEncode(docum.DocumentObject)) & "<BR>" &
                "<BR><a href=""" & DocSuiteContext.Current.CurrentTenant.DSWUrl & "?Tipo=Docm&Azione=Apri&Anno=" & Year & "&Numero=" & Number & """>" &
                sIntestazione & "</a>"

    End Sub

    Private Function GetProtocolMailData() As String
        Dim prot As Protocol = Facade.ProtocolFacade.GetById(UniqueIdProtocol)
        If prot Is Nothing Then
            Throw New DocSuiteException("Errore protocollo", String.Format("Impossibile trovare protocollo {0}", UniqueIdProtocol))
        End If

        Dim intestazione As String = String.Format("Protocollo n. {0} del {1:dd/MM/yyyy}", prot.FullNumber, prot.RegistrationDate.ToLocalTime())

        Dim tempSubject As String = String.Format("{2} {0} - {1}", intestazione, prot.ProtocolObject, DocSuiteContext.ProductName)
        If tempSubject.Length > 150 Then
            tempSubject = tempSubject.Substring(0, 150)
        End If
        Subject = tempSubject

        If Not String.IsNullOrEmpty(MailBody) Then
            Return String.Concat(String.Format(ProtocolEnv.ProtocolMailDataWithBody, DocSuiteContext.Current.CurrentTenant.DSWUrl, prot.Year, prot.Number, intestazione), MailBody)
        End If

        Return String.Format(ProtocolEnv.ProtocolMailData, intestazione, StringHelper.ReplaceCrLf(HttpUtility.HtmlEncode(prot.ProtocolObject)), DocSuiteContext.Current.CurrentTenant.DSWUrl, prot.Year, prot.Number)

    End Function

    ''' <summary> Dato un documentSeriesItem creo Oggetto e Body della mail con i dati di esso. </summary>
    Private Sub GetSeriesItemMailData()
        Dim seriesItem As DocumentSeriesItem = Facade.DocumentSeriesItemFacade.GetById(IdSeriesItem)
        If seriesItem Is Nothing Then
            Return
        End If

        If seriesItem.Status = DocumentSeriesItemStatus.Draft OrElse (seriesItem.Year Is Nothing AndAlso seriesItem.Number Is Nothing) Then
            Subject = String.Format("{3}: Bozza N {0} del {1:dd/MM/yyyy} registrata da {2}", seriesItem.Id, seriesItem.RegistrationDate, seriesItem.RegistrationUser, ProtocolEnv.DocumentSeriesName)
        Else
            Subject = String.Format("{2}: {0}/{1:0000000}", seriesItem.Year, seriesItem.Number, ProtocolEnv.DocumentSeriesName)
        End If

        Dim tempBody As New StringBuilder()
        tempBody.Append(Subject)

        Dim retireDate As String = If(seriesItem.RetireDate.HasValue, seriesItem.RetireDate.Value.ToString("dd/MM/yyyy"), "")
        Dim publicationDate As String = If(seriesItem.PublishingDate.HasValue, seriesItem.PublishingDate.Value.ToString("dd/MM/yyyy"), "")
        Dim link As String = String.Format("<a href=""{0}?Tipo=DocumentSeries&Azione=Apri&IdDocumentSeriesItem={1}"">{2}</a>", DocSuiteContext.Current.CurrentTenant.DSWUrl, seriesItem.Id, Subject)
        tempBody.AppendFormat("<br>Contenitore: {0} <br> Classificazione: {1} <br> Oggetto: {2} <br> Pubblicata: {3} <br> Ritirata: {4} <br><br> {5} <br>", seriesItem.DocumentSeries.Container.Name, seriesItem.Category.Name, seriesItem.Subject, publicationDate, retireDate, link)

        Subject = String.Format("{0} {1}", DocSuiteContext.ProductName, Subject)
        Body = tempBody.ToString()

    End Sub

    Private Sub GetLinkMailData()

        Dim protFacade As New ProtocolFacade()
        Dim prot As Protocol = protFacade.GetById(UniqueIdProtocol)

        If (prot Is Nothing) Then
            Exit Sub
        End If

        Dim sIntestazione As String = $"Collegamenti del protocollo n. {prot.FullNumber} del {prot.RegistrationDate:dd/MM/yyyy}"

        Subject = Left(String.Concat(DocSuiteContext.ProductName, " ", sIntestazione, " - ", prot.ProtocolObject), 150)

        Body = "<b>" & DocSuiteContext.ProductName & " - Gestione Documentale</b><BR>" &
                "<BR>Allego il " & sIntestazione &
                "<BR>Oggetto: " & StringHelper.ReplaceCrLf(HttpUtility.HtmlEncode(prot.ProtocolObject)) & "<BR>" &
                "<BR><a href=""" & DocSuiteContext.Current.CurrentTenant.DSWUrl & $"?Tipo=Prot&Azione=Apri&Anno={prot.Year}&Numero={prot.Number}"">" &
                sIntestazione & "</a>"

    End Sub

    ''' <summary> Permette di inserire il log visualizzabile dalla UI. </summary>
    Private Sub ProtocolLog(ByVal motivazione As String)
        Dim currentProtocol As Protocol = Facade.ProtocolFacade.GetById(UniqueIdProtocol)
        Facade.ProtocolLogFacade.Insert(currentProtocol, ProtocolLogEvent.PO, String.Format("Generato nuovo messaggio e-mail per {0}", motivazione))
    End Sub

#End Region

End Class

