Imports System.Linq
Imports System.Runtime.CompilerServices
Imports VecompSoftware.DocSuiteWeb.API
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports System.Security.Cryptography
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.Services.Logging

Public Module MailDTOEx

    <Extension()>
    Public Function ToPOLRequest(source As MailDTO, protocol As ProtocolDTO) As POLRequest
        Dim pol As POLRequest = Nothing
        If source.RegisteredLetter Then
            pol = New ROLRequest()
        Else
            pol = New LOLRequest()
        End If

        pol.Id = Guid.NewGuid()
        pol.Status = POLRequestStatusEnum.RequestQueued
        pol.StatusDescrition = "In attesa di Invio a Poste Online"

        pol.Sender = FacadeFactory.Instance.PosteOnLineContactFacade.GetDefaultSender()
        pol.Sender.Request = pol

        Dim posteWebLocation As Location = FacadeFactory.Instance.LocationFacade.GetById(DocSuiteContext.Current.ProtocolEnv.PosteWebRequestLocation)
        If posteWebLocation Is Nothing Then
            Throw New BiblosServiceException("Attenzione! Nessuna Location definita per la gestione delle PosteWebOnline")
        End If

        For Each item As API.ContactDTO In source.Recipients
            Dim dbRecipient As Contact = FacadeFactory.Instance.ProtocolFacade.GetContact(item)
            Dim recipient As New POLRequestRecipient()
            recipient.Id = Guid.NewGuid()
            recipient.Name = dbRecipient.Description.Replace("|", " ")
            recipient.PhoneNumber = dbRecipient.TelephoneNumber
            recipient.Status = POLMessageContactEnum.Created
            recipient.StatusDescrition = "In Attesa di Invio"

            FacadeFactory.Instance.PosteOnLineContactFacade.RecursiveSetRecipientAddress(recipient, dbRecipient)
            recipient.Request = pol
            pol.Recipients.Add(recipient)
        Next

        pol.Account = FacadeFactory.Instance.PosteOnLineAccountFacade.GetById(source.Mailbox.Id.Value)

        Dim currentProtocol As Protocol = FacadeFactory.Instance.ProtocolFacade.GetById(New YearNumberCompositeKey(protocol.Year, protocol.Number))
        Dim mergedDocuments As MergeDocumentResult = New MergeDocumentResult()
        If source.IncludeAttachments Then
            Dim attachments As ICollection(Of BiblosDocumentInfo) = ProtocolFacade.GetAttachments(currentProtocol).Where(Function(x) source.PolAttachments.Any(Function(xx) xx.Name.Eq(x.Name))).ToList()
            mergedDocuments = FacadeFactory.Instance.ProtocolFacade.GetMergedDocuments(currentProtocol, attachments, New List(Of BiblosDocumentInfo))
        Else
            mergedDocuments = FacadeFactory.Instance.ProtocolFacade.GetMergedDocuments(currentProtocol, New List(Of BiblosDocumentInfo), New List(Of BiblosDocumentInfo))
        End If

        If mergedDocuments.HasErrors Then
            Dim errorMessage As String = String.Concat("Sono avvenuti degli errori nella gestione dei documenti da spedire:", Environment.NewLine)
            errorMessage += String.Join(Environment.NewLine, mergedDocuments.Errors.Select(Function(s) s))
            FileLogger.Error(LogName.FileLog, errorMessage)
            Throw New DocSuiteException(errorMessage)
        End If

        Dim protocolDocuments As List(Of DocumentInfo) = New List(Of DocumentInfo)
        protocolDocuments.Add(mergedDocuments.MergedDocument)
        Dim rq As LOLRequest = DirectCast(pol, LOLRequest)
        Dim chain As BiblosChainInfo = New BiblosChainInfo(protocolDocuments)
        rq.IdArchiveChain = chain.ArchiveInBiblos(posteWebLocation.DocumentServer, posteWebLocation.ProtBiblosDSDB)

        Dim md5Service As MD5CryptoServiceProvider = New MD5CryptoServiceProvider()
        rq.DocumentMD5 = BitConverter.ToString(md5Service.ComputeHash(mergedDocuments.MergedDocument.Stream)).Replace("-", String.Empty)
        rq.DocumentName = "Documento principale.pdf"

        Return pol
    End Function

End Module