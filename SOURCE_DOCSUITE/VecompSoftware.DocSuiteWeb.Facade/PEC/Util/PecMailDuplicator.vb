Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Facade.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Logging

Namespace PEC.Util

    Public Class PecMailDuplicator
        Public Shared ReadOnly Property LoggerName As String
            Get
                Return LogName.FileLog
            End Get
        End Property

        Public Shared ReadOnly Property PecMailFacade As PECMailFacade
            Get
                Return FacadeFactory.Instance.PECMailFacade
            End Get
        End Property

        Public Shared Function SplitMultiPecByRecipients(ByRef sourcePecMail As PECMail) As IList(Of PECMail)
            Dim tor As New List(Of PECMail)

            ''Ciclo sui destinatari ed effettuo la clonazione su quelli
            Dim recipients As String() = sourcePecMail.MailRecipients.Split(";"c)
            FileLogger.Info(LoggerName, String.Format("Duplicazione PEC [{0}] in {1} nuove PEC in corso.", sourcePecMail.Id, recipients.Length))

            Dim i As Integer = 1
            For Each mailRecipient As String In recipients
                '' Clono la PEC sorgente
                Dim clonedPecMail As PECMail = CType(sourcePecMail.Clone(), PECMail)
                clonedPecMail.MailRecipients = mailRecipient
                '' Carico i RecipientsCc solo sulla prima PEC
                clonedPecMail.MailRecipientsCc = If(i = 1, sourcePecMail.MailRecipientsCc, String.Empty)
                clonedPecMail.Multiple = False
                clonedPecMail.SplittedFrom = sourcePecMail.Id
                clonedPecMail.RegistrationUser = sourcePecMail.RegistrationUser
                PecMailFacade.Save(clonedPecMail)

                ' LOG inserimento nuova pec
                FacadeFactory.Instance.PECMailLogFacade.Created(clonedPecMail)
                FileLogger.Info(LoggerName, String.Format("PEC {0}/{1} [source:{2}] correttamente salvata --> {3}.", i, recipients.Count, sourcePecMail.Id, clonedPecMail.Id))

                i += 1
                tor.Add(clonedPecMail)
            Next

            Return tor
        End Function

        Public Shared Function SplitMultiPecBySize(ByRef sourcePecMail As PECMail) As IList(Of PECMail)
            FileLogger.Info(LoggerName, String.Format("Separazione allegati PEC [{0}] in nuove PEC in corso.", sourcePecMail.Id))

            Dim tor As IList(Of PECMail) = New List(Of PECMail)

            '' Calcolo la grandezza massima consentita per PEC
            Dim maxSize As Long = FacadeFactory.Instance.PECMailboxFacade.GetMaxSendSize(sourcePecMail.MailBox)

            '' Ordino gli allegati per grandezza crescente e li duplico per poterli poi salvare su DB senza rompere vincoli
            Dim attachments As IList(Of PECMailAttachment) = (From pecMailAttachment In sourcePecMail.Attachments.OrderBy(Function(a) a.GetSize()) Select CType(pecMailAttachment.Clone(), PECMailAttachment)).ToList()

            Dim remainingSize As Long = maxSize
            Dim partIndex As Integer = 1
            Dim currentPecMail As PECMail = InitializePecPart(tor, sourcePecMail, partIndex)
            For Each pecMailAttachment As PECMailAttachment In attachments
                '' Se l'allegato non sta nel size
                '' devo creare una nuova PEC
                If pecMailAttachment.Size > remainingSize Then
                    '' Creo la nuova PEC
                    partIndex += 1
                    currentPecMail = InitializePecPart(tor, sourcePecMail, partIndex)

                    '' Ripristino la grandezza disponibile
                    remainingSize = maxSize
                End If

                '' Attacco l'allegato alla PEC corrente
                AddAttachment(currentPecMail, pecMailAttachment)
                '' aggiorno la dimensione corrente
                remainingSize -= pecMailAttachment.Size.Value
            Next

            '' Aggiorno con il totale delle parti
            For Each pecMail As PECMail In tor
                pecMail.MailSubject = String.Format(pecMail.MailSubject, partIndex)
                PecMailFacade.UpdateNoLastChange(pecMail)
            Next
            Return tor
        End Function

        Private Shared Function InitializePecPart(ByRef pecList As IList(Of PECMail), ByRef sourcePecMail As PECMail, ByVal partId As Integer) As PECMail
            Dim newPec As PECMail = CreateFromPec(sourcePecMail, String.Format(" - parte [{0} di {{0}}]", partId))
            pecList.Add(newPec)
            Return newPec
        End Function

        Private Shared Function CreateFromPec(ByRef sourcePecMail As PECMail, ByVal subjectAppend As String) As PECMail
            Dim newPec As PECMail = sourcePecMail.Clone(False, False, sourcePecMail)
            newPec.MailSubject &= subjectAppend
            newPec.RegistrationUser = sourcePecMail.RegistrationUser
            FacadeFactory.Instance.PECMailFacade.Save(newPec)

            ' LOG inserimento nuova pec
            FacadeFactory.Instance.PECMailLogFacade.Created(newPec)
            Return newPec
        End Function

        Private Shared Sub AddAttachment(ByRef pec As PECMail, ByRef pecAttachment As PECMailAttachment)
            pecAttachment.Mail = pec
            pec.Attachments.Add(pecAttachment)
            PecMailFacade.Update(pec)
        End Sub
    End Class
End Namespace