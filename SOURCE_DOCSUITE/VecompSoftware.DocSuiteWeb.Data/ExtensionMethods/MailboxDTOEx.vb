Imports System.Runtime.CompilerServices
Imports VecompSoftware.DocSuiteWeb.API
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods

Public Module MailboxDTOEx

    <Extension()>
    Public Function CopyFrom(source As MailboxDTO, mailbox As PECMailBox) As MailboxDTO
        source.TypeName = mailbox.GetType().Name
        source.Id = mailbox.Id
        source.Address = mailbox.MailBoxName
        If String.IsNullOrWhiteSpace(source.Name) Then
            source.Name = source.Address
        End If

        Return source
    End Function

    <Extension()>
    Public Function CopyFrom(source As MailboxDTO, mailbox As POLAccount) As MailboxDTO
        source.TypeName = mailbox.GetType().Name
        source.Id = mailbox.Id
        source.Name = mailbox.Name
        Return source
    End Function

    <Extension()>
    Public Function IsPEC(source As MailboxDTO) As Boolean
        Return If(source Is Nothing, False, source.IsTypeName(Of PECMailBox)())
    End Function

    <Extension()>
    Public Function IsPOL(source As MailboxDTO) As Boolean
        Return If(source Is Nothing, False, source.IsTypeName(Of POLAccount)())
    End Function

    <Extension()>
    Public Function IsMessage(source As MailboxDTO) As Boolean
        Return If(source Is Nothing, False, source.IsTypeName(Of DSWMessage)())
    End Function

    <Extension()>
    Private Function IsTypeName(Of T)(source As MailboxDTO) As Boolean
        Return source.TypeName.Eq(GetType(T).Name)
    End Function

End Module