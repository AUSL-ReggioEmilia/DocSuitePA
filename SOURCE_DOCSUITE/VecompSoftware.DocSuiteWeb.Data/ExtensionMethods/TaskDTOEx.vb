Imports System.Runtime.CompilerServices
Imports VecompSoftware.DocSuiteWeb.API
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods

Public Module TaskDTOEx

    <Extension()>
    Public Function CopyFrom(source As TaskDTO, header As TaskHeader) As TaskDTO
        source.Id = header.Id

        source.Code = header.Code
        source.Title = header.Title
        source.Description = header.Description
        source.TaskType = header.TaskType
        source.Status = header.Status
        source.TaskDate = header.RegistrationDate.DateTime

        Return source
    End Function


    <Extension()>
    Public Function AddMail(source As TaskDTO, mail As IMailDTO) As TaskDTO
        Dim mailbox As MailboxDTO = DirectCast(mail.Mailbox, MailboxDTO)

        If mailbox.IsPEC() Then
            source.AddPECMail(mail)
        ElseIf mailbox.IsPOL() Then
            source.AddPOLMail(mail)
        End If

        Return source
    End Function

End Module