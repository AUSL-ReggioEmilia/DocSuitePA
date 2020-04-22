Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade.PEC
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Services.Biblos
Imports System.Security.Cryptography
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.DocSuiteWeb.BusinessRule.Rules.Rights.PEC

Public Class SelectPolAccount
    Inherits CommonBasePage

    Private _pecMails As IList(Of PECMail) = Nothing
    Protected Function GetSelectedPecMailIds() As List(Of Integer)
        Dim ids As List(Of Integer) = New List(Of Integer)
        If Request.QueryString.HasKeys() AndAlso Request.QueryString.AllKeys().SingleOrDefault(Function(k) "keys".Eq(k)) Is Nothing Then
            Throw New ArgumentNullException("Selezionare delle PEC non valida")
        End If
        ids = Request.QueryString("keys").Split("|").Where(Function(k) Not String.IsNullOrEmpty(k)).Select(Function(f) Int32.Parse(f)).ToList()
        Return ids
    End Function

    Public ReadOnly Property PecMails As IList(Of PECMail)
        Get
            If _pecMails Is Nothing Then
                _pecMails = Facade.PECMailFacade.GetListByIds(GetSelectedPecMailIds())
            End If
            Return _pecMails
        End Get
    End Property

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSendRaccomandata, panelDatatable, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()

        If Not IsPostBack Then
            If DocSuiteContext.Current.ProtocolEnv.IsRaccomandataEnabled AndAlso PecMails IsNot Nothing Then
                Dim check As Boolean = True
                Dim script As String = String.Empty
                For Each pec As PECMail In PecMails
                    If Not (pec.HasDocumentUnit() AndAlso pec.DocumentUnitType = DSWEnvironment.Protocol) Then
                        check = False
                        script = "closeDialogWithError('Impossibile inviare raccomandate da PEC non protocollate')"
                    End If

                    Dim protocol As Protocol = Facade.ProtocolFacade.GetById(pec.Year.Value, pec.Number.Value)
                    If check AndAlso Not New ProtocolRights(protocol).IsPecSendable Then
                        check = False
                        script = "closeDialogWithError('Contenitore del protocollo errato o diritti insufficienti per l'utente')"
                    End If
                    If check AndAlso Not New PECMailRightsUtil(pec, DocSuiteContext.Current.User.FullUserName).IsResendable Then
                        check = False
                        script = "closeDialogWithError('Impossibile inviare raccomandate se la PEC non ha avuto esiti negativi')"
                    End If
                    If Not String.IsNullOrEmpty(script) Then
                        ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "id_dyn_cmds", script, True)
                        Return
                    End If
                Next

                Dim accounts As IList(Of POLAccount) = Facade.PosteOnLineAccountFacade.GetUserAccounts()
                ddlPolAccount.DataSource = accounts
                ddlPolAccount.DataBind()
                If accounts.Count > 1 Then
                    ddlPolAccount.Items.Insert(0, New ListItem("Selezionare Account PosteWeb", ""))
                End If
            End If
        End If
    End Sub

    Protected Sub btnSendRaccomandata_Click(sender As Object, e As EventArgs) Handles btnSendRaccomandata.Click
        Try
            For Each pec As PECMail In PecMails
                Dim msg As POLRequest = New ROLRequest
                Dim protocol As Protocol = Facade.ProtocolFacade.GetById(pec.Year.Value, pec.Number.Value)

                msg.Id = Guid.NewGuid()
                msg.Status = POLRequestStatusEnum.RequestQueued
                msg.StatusDescrition = "In attesa di Invio a Poste Online"
                msg.ProtocolYear = protocol.Year
                msg.ProtocolNumber = protocol.Number

                ' Imposto il mittente.
                Dim selectedSenders As IList(Of ContactDTO) = protocol.GetSenders()
                If selectedSenders.IsNullOrEmpty() OrElse Not selectedSenders.Count = 1 Then
                    Throw New ArgumentException("Il mittente deve essere univoco.")
                    Return
                End If
                msg.Sender = New POLRequestSender()
                msg.Sender.Id = Guid.NewGuid()
                msg.Sender.Name = Replace(selectedSenders(0).Contact.Description, "|", " ")

                Try
                    ' Imposto l'indirizzo del mittente.
                    FacadeFactory.Instance.PosteOnLineContactFacade.RecursiveSetSenderAddress(msg.Sender, selectedSenders(0).Contact)
                Catch ex As Exception
                    msg.Sender = FacadeFactory.Instance.PosteOnLineContactFacade.GetDefaultSender()
                End Try

                msg.Sender.PhoneNumber = String.Format("{0}", selectedSenders(0).Contact.TelephoneNumber).Trim()
                msg.Sender.RegistrationDate = Date.Now
                msg.Sender.RegistrationUser = DocSuiteContext.Current.User.FullUserName
                msg.Sender.Request = msg


                Dim contacts As List(Of Contact) = protocol.Contacts _
                                                   .Where(Function(f) f.ComunicationType.Eq(ProtocolContactCommunicationType.Recipient)) _
                                                   .Select(Function(f) f.Contact).ToList()

                'Dim manualContacts As List(Of Contact) = pec.Protocol.ManualContacts _
                '.Where(Function(f) f.ComunicationType.Eq(ProtocolContactCommunicationType.Recipient)) _
                '.Select(Function(f) f.Contact).ToList()
                'contacts.AddRange(manualContacts)

                ' Imposto i destinatari.
                For Each currentRecipient As Contact In contacts

                    Dim rcp As New POLRequestRecipient()
                    rcp.Id = Guid.NewGuid()
                    rcp.Name = Replace(currentRecipient.Description, "|", " ")

                    Try
                        ' Imposto l'indirizzo del destinatario.
                        FacadeFactory.Instance.PosteOnLineContactFacade.RecursiveSetRecipientAddress(rcp, currentRecipient)
                    Catch ex As Exception
                        FileLogger.Warn(LoggerName, "Errore indirizzo destinatario", ex)
                        Throw New ArgumentNullException(ex.Message)
                        Return
                    End Try

                    rcp.PhoneNumber = String.Format("{0}", currentRecipient.TelephoneNumber).Trim()
                    rcp.RegistrationDate = Date.Now
                    rcp.RegistrationUser = DocSuiteContext.Current.User.FullUserName
                    rcp.Status = POLMessageContactEnum.Created
                    rcp.StatusDescrition = "In Attesa di Invio"

                    msg.Recipients.Add(rcp)
                    rcp.Request = msg

                Next

                If msg.Recipients.Count <= 0 Then
                    Throw New ArgumentException("Nessun destinatario selezionato")
                    Return
                End If

                Dim rq As LOLRequest = DirectCast(msg, LOLRequest)
                Dim docs As List(Of DocumentInfo) = New List(Of DocumentInfo)

                Dim doc As BiblosDocumentInfo = New BiblosDocumentInfo(protocol.Location.DocumentServer, protocol.Location.ProtBiblosDSDB, protocol.IdDocument.Value)
                Dim memory As MemoryDocumentInfo = New MemoryDocumentInfo(doc.GetPdfStream(), doc.Name)
                docs.Add(memory)
                Dim chain As BiblosChainInfo = New BiblosChainInfo(docs)
                rq.IdArchiveChain = chain.ArchiveInBiblos(protocol.Location.DocumentServer, protocol.Location.ProtBiblosDSDB)
                Dim md5 As MD5 = New MD5CryptoServiceProvider()
                rq.DocumentMD5 = BitConverter.ToString(md5.ComputeHash(memory.GetPdfStream())).Replace("-", String.Empty)
                rq.DocumentName = doc.PDFName
                msg.RegistrationDate = Date.Now
                msg.RegistrationUser = DocSuiteContext.Current.User.FullUserName
                msg.Account = Facade.PosteOnLineAccountFacade.GetById(Integer.Parse(ddlPolAccount.SelectedValue))

                Facade.PosteOnLineRequestFacade.SaveAll(msg)

            Next
            ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "id_dyn_cmds", "closeDialog()", True)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore durante il salvataggio del mittente", ex)
            Throw New ArgumentException(String.Format("Errore durante il salvataggio del mittente: {0}", ex.Message))
        End Try

    End Sub


End Class