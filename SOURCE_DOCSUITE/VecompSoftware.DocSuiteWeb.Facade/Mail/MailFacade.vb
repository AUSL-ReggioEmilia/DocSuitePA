Imports System.Web.UI
Imports System.Text
Imports System.Collections.Specialized
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.Helpers.ExtensionMethods
Imports System.Web
Imports VecompSoftware.DocSuiteWeb.Data

Public Class MailFacade

#Region " Fields "

    ''' <summary>Prefisso per distinguere il valore in sessione</summary>
    Const sessionPrefix As String = "mailFacade"

#End Region

#Region " Methods "

    ''' <summary>
    ''' Decide contestualmente al parametro <see>ProtocolEnv.QuerystringToSession</see> se inserire il parametro in querystring o in sessione
    ''' </summary>
    Public Shared Function AppendParam(ByVal seed As String, ByVal parameterName As String, ByVal parameterValue As String) As String
        Dim querystring As String = ""
        If DocSuiteContext.Current.ProtocolEnv.QuerystringToSession Then
            ' se attivata la scrittura in sessione torno una stringa vuota
            CommonShared.SetContextValue(String.Concat(sessionPrefix, seed, parameterName), parameterValue)
        Else
            querystring = String.Format("&{0}={1}", parameterName, parameterValue)
        End If
        Return querystring
    End Function

    ''' <summary>
    ''' Decide contestualmente al parametro <see>ProtocolEnv.QuerystringToSession</see> se prelevare il parametro da querystring o sessione
    ''' </summary>
    Public Shared Function RetrieveParam(ByVal querystring As NameValueCollection, ByVal parameterName As String) As String
        Dim parameterValue As String
        If DocSuiteContext.Current.ProtocolEnv.QuerystringToSession Then
            parameterValue = CommonShared.GetContextValue(String.Concat(sessionPrefix, querystring("seed"), parameterName))
        Else
            parameterValue = querystring(parameterName)
        End If
        Return parameterValue
    End Function

    ''' <summary></summary>
    ''' <remarks>Attenzione: i parametri vanno generati attraverso questa facade per consentire uso querystring/sessione</remarks>
    <Obsolete("Per togliere dalla facade la GUI è consigliabile spostare questi metodi.")>
    Public Shared Function GetOpenMailWindowScript(ByVal parameters As String) As String
        Dim URL As String = "../Comm/CommMessage.aspx?" & parameters
        Return String.Format("return OpenMailWindow(""{0}"",{1},{2});", URL, 800, 600)
    End Function

    ''' <summary></summary>
    ''' <remarks>Attenzione: i parametri vanno generati attraverso questa facade per consentire uso querystring/sessione</remarks>
    <Obsolete("Per togliere dalla facade la GUI è consigliabile spostare questi metodi.")>
    Public Shared Sub RegisterOpenerMailWindow(ByRef opener As WebControls.Button, ByVal parameters As String)
        opener.OnClientClick = GetOpenMailWindowScript(parameters)
    End Sub

    ''' <summary> Crea i parametri di base per l'invio di una mail </summary>
    ''' <param name="type">Sezione della DWS: Prot, Resl e Docm</param>
    ''' <param name="action">Azione: Mail, Link e CollInfo</param>
    ''' <param name="seed">Identificativo per differenziare la mail in sessione</param>
    Private Shared Function CreateMailParameters(ByVal type As String, ByVal action As String, ByVal seed As String) As StringBuilder
        Dim strMail As New StringBuilder()
        strMail.Append(AppendTypeParam(type))
        strMail.Append(AppendSeedParam(seed))
        strMail.Append(AppendActionParam(action, seed))
        Return strMail
    End Function

    ''' <summary></summary>
    ''' <remarks>Questo è sempre il primo parametro in query string</remarks>
    Private Shared Function AppendTypeParam(ByVal type As String) As String
        Return String.Format("Type={0}", type)
    End Function

    ''' <summary>Identificativo del link in sessione</summary>
    ''' <remarks>Se attivo <see>ProtocolEnv.QuerystringToSession</see>, altrimenti non è presente: la querystring già rappresenta un link univoco.</remarks>
    Private Shared Function AppendSeedParam(ByVal seed As String) As String
        Dim querystring As String
        If DocSuiteContext.Current.ProtocolEnv.QuerystringToSession Then
            querystring = String.Format("&seed={0}", seed)
        Else
            querystring = ""
        End If
        Return querystring
    End Function

    ''' <summary> Crea parametri compatibili con la gestione in sessione. </summary>
    Private Shared Function AppendActionParam(ByVal action As String, ByVal seed As String) As String
        Return AppendParam(seed, "Action", action)
    End Function

    ''' <summary> Crea parametri compatibili con la gestione in sessione. </summary>
    Private Shared Function AppendYearNumberParam(ByVal year As Short, ByVal number As Integer, ByVal seed As String) As String
        Dim yearNumber As New StringBuilder()
        yearNumber.Append(AppendParam(seed, "Year", year.ToString()))
        yearNumber.Append(AppendParam(seed, "Number", number.ToString()))
        Return yearNumber.ToString()
    End Function

    Private Shared Function AppendUniqueIdProtocolParam(uniqueId As Guid, ByVal seed As String) As String
        Dim builder As New StringBuilder()
        builder.Append(AppendParam(seed, "UniqueIdProtocol", uniqueId.ToString()))
        Return builder.ToString()
    End Function

    ''' <summary> Crea parametri compatibili con la gestione in sessione. </summary>
    Private Shared Function AppendResolutionParam(ByVal idResolution As Integer, ByVal seed As String) As String
        Return AppendParam(seed, "idResolution", idResolution.ToString())
    End Function

    ''' <summary> Crea parametri compatibili con la gestione in sessione. </summary>
    Private Shared Function AppendSeriesParam(ByVal idSeriesItem As Integer, ByVal seed As String) As String
        Return AppendParam(seed, "idSeriesItem", idSeriesItem.ToString())
    End Function

    ''' <summary> Crea parametri compatibili con la gestione in sessione. </summary>
    Private Shared Function AppendCollaborationParam(ByVal idCollaboration As Integer, ByVal seed As String) As String
        Return AppendParam(seed, "idColl", idCollaboration.ToString())
    End Function

    ''' <summary> Crea parametri compatibili con la gestione in sessione. </summary>
    Private Shared Function AppendMailToParam(ByVal mailTo As String, ByVal seed As String) As String
        Return AppendParam(seed, "MailTo", mailTo)
    End Function

#End Region

#Region " Protocol "

    ''' <summary> ??? arriva da codice vecchio </summary>
    ''' <remarks> Probabilmente esiste già ripetuto N volte questo codice, spostare o eliminare </remarks>
    Private Shared Function GetProtocolTitle(ByVal prot As Protocol) As String
        Return String.Format("Protocollo n. {0}/{1} del {2:dd/MM/yyyy}", prot.Year, prot.Number, prot.RegistrationDate.ToLocalTime())
    End Function

    ''' <summary> Oggetto per le mail con protocollo </summary>
    Public Shared Function GetProtocolSubject(ByVal prot As Protocol) As String
        Dim message As New StringBuilder()
        message.AppendFormat("{0} {1} - ", DocSuiteContext.ProductName, GetProtocolTitle(prot))
        Dim messageObjectMaxLength As Integer = 150
        If (DocSuiteContext.Current.ProtocolEnv.MessageObjectMaxLength > 0) Then
            messageObjectMaxLength = DocSuiteContext.Current.ProtocolEnv.MessageObjectMaxLength
        End If
        If (prot.ProtocolObject.Length > messageObjectMaxLength) Then
            message.Append(prot.ProtocolObject.Substring(0, messageObjectMaxLength - 1))
        Else
            message.Append(prot.ProtocolObject)
        End If
        Return message.ToString()
    End Function

    Public Shared Function GetProtocolBody(ByRef prot As Protocol, Optional withProtocolLinks As Boolean = True) As String
        Dim mailBody As New StringBuilder()
        mailBody.AppendFormat("Protocollo: <B>{0} del {1}</B>{5}Tipo: {2}{5}Contenitore: {3}{5}Oggetto: {4}{5}",
                              prot.FullNumber, prot.RegistrationDate.ToLocalTime().DefaultString(), prot.Type.Description, prot.Container.Name, prot.ProtocolObject, WebHelper.Br)

        'scrivi l'elenco contatti solo se inferiore a 50
        Dim countContacts As Long = New ProtocolContactFacade().GetCountByProtocol(prot, "")
        If countContacts < 50 Then
            'mittenti/destinatari
            Dim mittList As IList(Of Contact) = New List(Of Contact)
            Dim destList As IList(Of Contact) = New List(Of Contact)
            For Each contact As ProtocolContact In prot.Contacts

                If contact.ComunicationType.Eq(ProtocolContactCommunicationType.Sender) Then
                    mittList.Add(contact.Contact)
                ElseIf contact.ComunicationType.Eq(ProtocolContactCommunicationType.Recipient) Then
                    destList.Add(contact.Contact)
                End If

            Next
            For Each contact As ProtocolContactManual In prot.ManualContacts

                If contact.ComunicationType.Eq(ProtocolContactCommunicationType.Sender) Then
                    mittList.Add(contact.Contact)
                ElseIf contact.ComunicationType.Eq(ProtocolContactCommunicationType.Recipient) Then
                    destList.Add(contact.Contact)
                End If

            Next

            Dim mitt As String = MailFacade.GetContactMailBody(mittList)
            If Not String.IsNullOrEmpty(mitt) Then
                mailBody.AppendFormat("Mittenti: {1}{0}{1}", mitt, WebHelper.Br)
            End If

            Dim dest As String = MailFacade.GetContactMailBody(destList)
            If Not String.IsNullOrEmpty(dest) Then
                mailBody.AppendFormat("Destinatari: {1}{0}{1}", dest, WebHelper.Br)
            End If
        End If

        Dim cf As New CategoryFacade()
        mailBody.AppendFormat("Classificazione: {0}{1}", cf.GetFullIncrementalName(prot.Category), WebHelper.Br)
        If (withProtocolLinks) Then
            If mailBody.Length = 0 Then
                mailBody.AppendFormat(DocSuiteContext.Current.ProtocolEnv.ProtocolMailData, GetProtocolTitle(prot), prot.ProtocolObject, DocSuiteContext.Current.CurrentTenant.DSWUrl, prot.Year, prot.Number)
            Else
                mailBody.AppendFormat(DocSuiteContext.Current.ProtocolEnv.ProtocolMailDataWithBody, DocSuiteContext.Current.CurrentTenant.DSWUrl, prot.Year, prot.Number, GetProtocolTitle(prot))
            End If
        End If

        Return mailBody.ToString()
    End Function

    Private Shared Function CreateProtocolParameters(uniqueId As Guid, ByVal action As String, ByVal seed As String) As StringBuilder
        Dim strMail As StringBuilder = CreateMailParameters("Prot", action, seed)
        strMail.Append(AppendUniqueIdProtocolParam(uniqueId, seed))
        Return strMail
    End Function

    Public Shared Function CreateProtocolMailParameters(uniqueId As Guid, ByVal seed As String) As String
        Return CreateProtocolParameters(uniqueId, "Mail", seed).ToString()
    End Function

    Public Shared Function CreateProtocolMailToParameters(uniqueId As Guid, ByVal mailTo As String, ByVal seed As String) As String
        Dim strMail As StringBuilder = CreateProtocolParameters(uniqueId, "Mail", seed)
        strMail.Append(AppendMailToParam(mailTo, seed))
        Return strMail.ToString()
    End Function

    Public Shared Function CreateProtocolLinkMailParameters(uniqueId As Guid, ByVal seed As String) As String
        Return CreateProtocolParameters(uniqueId, "Link", seed).ToString()
    End Function

#End Region

#Region "Series"


    Public Shared Function CreateSeriesMailToBodyParameters(ByVal item As DocumentSeriesItem, ByVal mailTo As String, ByVal seed As String) As String

        Dim strMail As StringBuilder = CreateMailParameters("Series", "Series", seed)
        strMail.Append(AppendSeriesParam(item.Id, seed))
        strMail.Append(AppendMailToParam(mailTo, seed))
        Return strMail.ToString()

    End Function

#End Region

#Region " Resolution "

    Private Shared Function GetResolutionTitle(ByVal resolution As Resolution) As String
        ' Uso la metodologia ufficiale di creazione del nome/numero
        Dim rFacade As New ResolutionFacade()
        Return String.Format("{0}{1} {2} del {3}", If(resolution.Number.HasValue, "", "Proposta di "), rFacade.GetResolutionLabel(resolution), rFacade.GetResolutionNumber(resolution), resolution.AdoptionDate.DefaultString())
    End Function

    Public Shared Function GetResolutionSubject(ByVal resolution As Resolution) As String
        Return String.Format("{0} {1} - {2}", DocSuiteContext.ProductName, GetResolutionTitle(resolution), If(resolution.ResolutionObjectPrivacy IsNot Nothing, resolution.ResolutionObjectPrivacy, resolution.ResolutionObject))
    End Function

    Public Shared Function GetResolutionBody(ByVal resolution As Resolution) As String
        Return String.Format("<b>{0} - Gestione Documentale</b><BR><BR>Allego {1}<BR>Oggetto: {2}<BR><BR><a href=""{3}?Tipo=Resl&Azione=Apri&Identificativo={4}"">{1}</a>",
                             DocSuiteContext.ProductName, GetResolutionTitle(resolution), StringHelper.ReplaceCrLf(HttpUtility.HtmlEncode(If(resolution.ResolutionObjectPrivacy IsNot Nothing, resolution.ResolutionObjectPrivacy, resolution.ResolutionObject))), DocSuiteContext.Current.CurrentTenant.DSWUrl, resolution.Id)
    End Function

    Private Shared Function CreateResolutionParameters(ByVal idResolution As Integer, ByVal action As String, ByVal seed As String) As StringBuilder
        Dim strMail As StringBuilder = CreateMailParameters("Resl", action, seed)
        strMail.Append(AppendResolutionParam(idResolution, seed))
        Return strMail
    End Function

    Public Shared Function CreateResolutionMailParameters(ByVal idResolution As Integer, ByVal seed As String) As String
        Return CreateResolutionParameters(idResolution, "Mail", seed).ToString()
    End Function

    Public Shared Function CreateResolutionMailToParameters(ByVal idResolution As Integer, ByVal mailTo As String, ByVal seed As String) As String
        Dim strMail As StringBuilder = CreateResolutionParameters(idResolution, "Mail", seed)
        strMail.Append(AppendMailToParam(mailTo, seed))
        Return strMail.ToString()
    End Function

#End Region

#Region "Collaboration"

    Private Shared Function CreateCollaborationParameters(ByVal idCollaboration As Integer, ByVal action As String, ByVal seed As String) As StringBuilder
        Dim strMail As StringBuilder = CreateMailParameters("Prot", action, seed)
        strMail.Append(AppendCollaborationParam(idCollaboration, seed))
        Return strMail
    End Function

    Public Shared Function CreateCollaborationMailParameters(ByVal idCollaboration As Integer, ByVal seed As String) As String
        Return CreateCollaborationParameters(idCollaboration, "CollInfo", seed).ToString()
    End Function

#End Region

#Region "Documents"

    Public Shared Function CreateDocumentParameters(ByVal year As Short, ByVal number As Integer, ByVal action As String, ByVal seed As String) As StringBuilder
        Dim strMail As StringBuilder = CreateMailParameters("Docm", action, seed)
        strMail.Append(AppendYearNumberParam(year, number, seed))
        Return strMail
    End Function

#End Region

#Region " Contacts "

    Public Shared Function GetContactMailBody(ByRef contacts As IList(Of Contact)) As String
        If contacts Is Nothing Then
            Return String.Empty
        End If

        Dim contactDescription As New StringBuilder
        For Each contact As Contact In contacts
            FormatHierarchyContact(contact, contactDescription)
        Next

        Return contactDescription.ToString()
    End Function

    Private Shared Sub FormatHierarchyContact(ByRef contact As Contact, ByRef contactDescription As StringBuilder)
        If Not contact.Parent Is Nothing Then
            FormatHierarchyContact(contact.Parent, contactDescription)
        End If
        If contactDescription.Length <> 0 Then
            contactDescription.Append("<BR>")
        End If
        ' TODO: rivedere questo codice?
        Dim spazio As String = ""
        spazio = spazio.PadLeft(StringHelper.CountChar(contact.FullIncrementalPath, "|"c), "."c)
        contactDescription.Append(spazio)
        contactDescription.Append(Replace(contact.Description, "|"c, " "c))
    End Sub

    ''' <summary> Struttura un indirizzo email per rappresentarlo come contatto manuale. </summary>
    ''' <param name="email">Indirizzo email da strutturare.</param>
    Public Shared Function CreateManualContact(ByVal email As String, ByVal contactType As Char?, ByVal isManual As Boolean) As ContactDTO
        Return CreateManualContact(email, email, contactType, isManual, False)
    End Function

    ''' <summary> Struttura un indirizzo email per rappresentarlo come contatto manuale. </summary>
    ''' <param name="email">Indirizzo email da strutturare.</param>
    Public Shared Function CreateManualContact(ByVal description As String, ByVal email As String, ByVal contactType As Char?, ByVal isManual As Boolean, ByVal isCertified As Boolean) As ContactDTO

        Dim contact As New Contact
        contact.Description = description
        If isCertified Then
            contact.CertifiedMail = email
        Else
            contact.EmailAddress = email
        End If

        If contactType.HasValue Then
            contact.ContactType = New ContactType(contactType.Value)
        End If

        Dim dto As New ContactDTO
        dto.Contact = contact

        If isManual Then
            dto.ContactPart = ContactTypeEnum.Manual
            dto.Type = ContactDTO.ContactType.Manual
        End If

        Return dto
    End Function

#End Region

#Region "Fascicles"

    Private Shared Function GetFascicleTitle(fascicle As Entity.Fascicles.Fascicle) As String
        Return String.Format("Fascicolo n. {0} del {1}", fascicle.Title, fascicle.RegistrationDate.ToLocalTime().DefaultString())
    End Function

    Public Shared Function GetFascicleListSubject(fascicleList As IList(Of Entity.Fascicles.Fascicle)) As String
        Dim mailSubject As New StringBuilder()
        mailSubject.AppendFormat("{0}", DocSuiteContext.ProductName)
        For Each item As Entity.Fascicles.Fascicle In fascicleList
            mailSubject.AppendFormat(GetFascicleSubject(item))
        Next
        Return mailSubject.ToString()
    End Function

    Public Shared Function GetFascicleListBody(fascicleList As IList(Of Entity.Fascicles.Fascicle)) As String
        Dim mailBody As New StringBuilder()
        mailBody.AppendFormat(String.Format("<b>{0} - Gestione Documentale</b><br /><br />", DocSuiteContext.ProductName))
        For Each item As Entity.Fascicles.Fascicle In fascicleList
            mailBody.AppendFormat(GetFascicleBody(item))
        Next
        Return mailBody.ToString()
    End Function

    Public Shared Function GetFascicleSubject(fascicle As Entity.Fascicles.Fascicle) As String
        Dim message As New StringBuilder()
        message.AppendFormat(" - {0}", GetFascicleTitle(fascicle))
        Return message.ToString()
    End Function

    Public Shared Function GetFascicleBody(ByRef fascicle As Entity.Fascicles.Fascicle) As String
        Dim mailBody As New StringBuilder()

        mailBody.AppendFormat("<a href='{0}?Tipo=Fasc&Azione=Apri&IdFascicle={1}'>{2}</a><br />", DocSuiteContext.Current.CurrentTenant.DSWUrl, fascicle.UniqueId, GetFascicleTitle(fascicle))

        Return mailBody.ToString()
    End Function
#End Region

#Region "Dossiers"

    Private Shared Function GetDossierTitle(dossier As Entity.Dossiers.Dossier) As String
        Return String.Format("Dossier n. {0} del {1}", dossier.Number, dossier.RegistrationDate.ToLocalTime().DefaultString())
    End Function
    Private Shared Function GetDossierTitleFormatted(dossier As Entity.Dossiers.Dossier) As String
        Return String.Format("{0}/{1}", dossier.Year, String.Format("{0:0000000}", dossier.Number))
    End Function

    Public Shared Function GetDossierSubject(dossier As Entity.Dossiers.Dossier) As String
        Dim message As New StringBuilder()
        message.AppendFormat("{0} - {1}", DocSuiteContext.ProductName, GetDossierTitle(dossier))
        Return message.ToString()
    End Function

    Public Shared Function GetDossierBody(ByRef dossier As Entity.Dossiers.Dossier) As String
        Dim mailBody As New StringBuilder()
        mailBody.AppendFormat("<b>{0} - Gestione Documentale</b><br /><br /> <a href='{1}?Tipo=Dossier&Azione=Apri&IdDossier={2}&DossierTitle={3}>{4}</a><br />", DocSuiteContext.ProductName, DocSuiteContext.Current.CurrentTenant.DSWUrl, dossier.UniqueId, GetDossierTitleFormatted(dossier), GetDossierTitle(dossier))
        Return mailBody.ToString()
    End Function
#End Region

End Class
