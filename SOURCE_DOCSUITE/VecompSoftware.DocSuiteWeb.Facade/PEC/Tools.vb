Imports System.IO
Imports VecompSoftware.Helpers
Imports Limilabs.Mail.MIME
Imports Limilabs.Mail.Headers
Imports Limilabs.Mail
Imports System.Net.Mail
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Text
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Helpers.Compress

Namespace PEC
    ' TODO: spostare questi metodi nelle facade appropriate
    <Obsolete("Non aggiungere metodi qui.")>
    Public Class Tools

        Private Shared Function ConvertAttachmentToMail(attachment As MimeData) As IMail
            Dim eml As String
            Using stream As New MemoryStream()
                attachment.Save(stream)
                eml = Encoding.UTF8.GetString(stream.ToArray())

                stream.Close()
            End Using

            Dim bld As New Limilabs.Mail.MailBuilder()
            Return bld.CreateFromEml(eml)
        End Function

        Public Shared Function GetAttachments(file As FileInfo) As List(Of DocumentInfo)
            Dim builder As New Limilabs.Mail.MailBuilder
            Dim mail As IMail = builder.CreateFromEmlFile(file.FullName)
            Return GetAttachments(mail, file.Name)
        End Function

        Public Shared Function GetAttachments(doc As DocumentInfo) As List(Of DocumentInfo)
            Dim builder As New Limilabs.Mail.MailBuilder
            Dim mail As IMail = builder.CreateFromEml(Encoding.UTF8.GetString(doc.Stream))
            Return GetAttachments(mail, doc.Name)
        End Function

        Public Shared Function GetEviredMail(mail As IMail, name As String) As DocumentInfo
            ' Eviro l'email e la salvo in temp

            Dim cfg As New AttachmentRemoverConfiguration()
            cfg.RemoveVisuals = False
            cfg.RemoveAlternatives = True
            mail.RemoveAttachments(cfg)

            'Dim evired As IMail = mail.RemoveAttachments(False, True).Create()
            Dim eviredBytes() As Byte = Encoding.GetEncoding(1252).GetBytes(mail.RenderEml())
            Dim eviredDoc As New MemoryDocumentInfo(eviredBytes, name)
            Dim eviredFile As FileInfo = BiblosFacade.SaveUniqueToTemp(eviredDoc)
            Dim eviredTempDoc As New TempFileDocumentInfo(eviredFile)
            eviredTempDoc.Name = name
            Return eviredTempDoc
        End Function

        Public Shared Function GetAttachments(mail As IMail, name As String) As List(Of DocumentInfo)
            Dim tor As New List(Of DocumentInfo)
            Dim root As DocumentInfo = GetEviredMail(mail, name)
            tor.Add(root)

            For Each mime As MimeData In mail.Attachments
                If FileHelper.MatchExtension(mime.FileName, FileHelper.EML) Then
                    Dim inner As IMail = ConvertAttachmentToMail(mime)

                    ' Recupero gli allegati della mail interna e li aggiungo come figli all'attuale
                    Dim innerAttachments As List(Of DocumentInfo) = GetAttachments(inner, mime.SafeFileName)
                    root.Children.AddRange(innerAttachments)
                    Continue For
                End If

                If FileHelper.MatchExtension(mime.FileName, FileHelper.ZIP) OrElse FileHelper.MatchExtension(mime.FileName, FileHelper.RAR) Then
                    Dim zipDoc As DocumentInfo = New FolderInfo(mime.SafeFileName, mime.SafeFileName)
                    root.Children.Add(zipDoc)
                    zipDoc.Children.AddRange(GetAttachmentsFromCompress(mime, If(FileHelper.MatchExtension(mime.FileName, FileHelper.ZIP), FileHelper.ZIP, FileHelper.RAR)))
                    Continue For
                End If

                Dim doc As New MemoryDocumentInfo(mime.Data, mime.SafeFileName)
                Dim fi As FileInfo = BiblosFacade.SaveUniqueToTemp(doc)
                Dim file As New TempFileDocumentInfo(fi)
                file.Name = mime.SafeFileName
                root.Children.Add(file)
            Next
            Return tor
        End Function

        Public Shared Function GetAttachmentsFromCompress(data As Byte(), type As String) As List(Of DocumentInfo)
            Dim compressManager As ICompress = New ZipCompress()
            If type = FileHelper.RAR Then
                compressManager = New RarCompress()
            End If

            Dim tor As New List(Of DocumentInfo)
            Using memoryStream As New MemoryStream(data)
                For Each item As CompressItem In compressManager.InMemoryExtract(memoryStream)
                    Dim memoryDocumentInfo As MemoryDocumentInfo = New MemoryDocumentInfo(item.Data, item.Filename)
                    Dim fileInfo As FileInfo = BiblosFacade.SaveUniqueToTemp(memoryDocumentInfo)
                    Dim tempFileDocumentInfo As New TempFileDocumentInfo(fileInfo)
                    tempFileDocumentInfo.Name = item.Filename
                    tor.Add(tempFileDocumentInfo)
                Next
            End Using
            Return tor
        End Function

        Public Shared Function GetAttachmentsFromCompress(mime As MimeData, type As String) As List(Of DocumentInfo)
            Return GetAttachmentsFromCompress(mime.Data, type)
        End Function

        ''' <summary>
        ''' A partire da una IMail genera una PECMail
        ''' </summary>
        ''' <param name="mail">Istanza di IMail da analizzare</param>
        ''' <returns>Restituisce un'istanza di PECMail</returns>
        Public Shared Function PecMailFactory(mail As IMail, size As Int64?) As PECMail
            Dim pec As New PECMail()
            pec.Attachments = New List(Of PECMailAttachment)()
            pec.Direction = PECMailDirection.Ingoing
            pec.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Delete)
            pec.MailUID = Guid.NewGuid().ToString()
            If Not String.IsNullOrEmpty(mail.Subject) Then
                pec.MailSubject = mail.Subject.Trim()
            Else
                pec.MailSubject = String.Empty
            End If
            pec.MailSenders = GetMailAddressFromMailboxes(mail.From).Trim()
            pec.MailRecipients = GetMailAddressFromMailAddress(mail.To).Trim()

            pec.PECType = PECMailType.Anomalia

            Dim dateMail As DateTime
            If Not DateTime.TryParse(mail.Document.Root.Headers("Date"), dateMail) Then
                dateMail = DateTime.Now
            End If
            pec.MailDate = dateMail

            pec.XTrasporto = mail.Document.Root.Headers("X-Trasporto")
            pec.MessageID = mail.Document.Root.Headers("Message-ID")
            pec.XRiferimentoMessageID = mail.Document.Root.Headers("X-Riferimento-Message-ID")

            ' priorità da mail PEC
            If mail.Priority = MimePriority.NonUrgent Then
                pec.MailPriority = CType(MailPriority.Low, Short?)
            ElseIf (mail.Priority = MimePriority.Normal) Then
                pec.MailPriority = CType(MailPriority.Normal, Short?)
            ElseIf (mail.Priority = MimePriority.Urgent) Then
                pec.MailPriority = CType(MailPriority.High, Short?)
            Else
                pec.MailPriority = CType(MailPriority.Normal, Short?)
            End If

            If size.HasValue Then
                pec.Size = size.Value
            End If

            Return pec
        End Function

        Public Shared Function PecMailFactory(file As FileInfo) As PECMail
            Dim builder As New Limilabs.Mail.MailBuilder
            Dim mail As IMail = builder.CreateFromEmlFile(file.FullName)
            Return PecMailFactory(mail, file.Length)
        End Function

        Public Shared Function PecMailFactory(doc As DocumentInfo) As PECMail
            Dim builder As New Limilabs.Mail.MailBuilder
            Dim mail As IMail = builder.CreateFromEml(Encoding.UTF8.GetString(doc.Stream))
            Return PecMailFactory(mail, doc.Size)
        End Function

        Public Shared Function RemoveAttachments(data As IMail) As IMail
            Dim cfg As New AttachmentRemoverConfiguration()
            cfg.RemoveVisuals = False
            cfg.RemoveAlternatives = True
            data.RemoveAttachments(cfg)

            Return data
        End Function

        Public Shared Function GetMailAddressFromMailAddress(mailBoxList As IList(Of Headers.MailAddress)) As String
            If mailBoxList Is Nothing OrElse mailBoxList.Count = 0 Then
                Return String.Empty
            End If

            Dim mailAddresses As New StringBuilder()
            For Each mb As Headers.MailAddress In mailBoxList
                mailAddresses.Append(GetMailAddressFromMailboxes(mb.GetMailboxes()))
            Next

            Return mailAddresses.ToString()
        End Function

        Public Shared Function GetMailAddressFromMailboxes(mailBoxList As IList(Of Limilabs.Mail.Headers.MailBox)) As String
            If mailBoxList Is Nothing OrElse mailBoxList.Count = 0 Then
                Return String.Empty
            End If

            Dim mailAddresses As New StringBuilder()
            For Each mb As Limilabs.Mail.Headers.MailBox In mailBoxList
                If [String].IsNullOrEmpty(mb.Address) Then
                    Continue For
                End If

                If mailAddresses.Length <> 0 Then
                    mailAddresses.Append(", ")
                End If

                mailAddresses.Append(If([String].IsNullOrEmpty(mb.Name), mb.Address, String.Format("{0} ({1})", mb.Name, mb.Address)))
            Next

            Return mailAddresses.ToString()
        End Function
    End Class
End Namespace