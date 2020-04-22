<Serializable()> _
Public Class PECMailBox
    Inherits DomainObject(Of Short)

#Region " Constructor "

    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub


#End Region

#Region " Properties "

    Public Overridable Property MailBoxName As String

    Public Overridable Property IncomingServerName As String

    Public Overridable Property IncomingServerProtocol As IncomingProtocol?

    Public Overridable Property IncomingServerPort As Integer?

    Public Overridable Property IncomingServerUseSsl As Boolean?

    Public Overridable Property OutgoingServerName As String

    Public Overridable Property OutgoingServerPort As Integer?

    Public Overridable Property OutgoingServerUseSsl As Boolean?

    Public Overridable Property Username As String

    Public Overridable Property Password As String

    Public Overridable Property Mails As IList(Of PECMail)

    Public Overridable Property Roles As IList(Of Role)

    ''' <summary>Collegamento tra caselle di posta e settori</summary>
    ''' <remarks>Aggiunto per poter accedere alla proprietà decorata <see>PECMailBoxRole.Priority</see></remarks>
    Public Overridable Property MailBoxRoles As IList(Of PECMailBoxRole)

    Public Overridable Property Managed As Boolean

    Public Overridable Property Unmanaged As Boolean

    Public Overridable Property Location As Location

    Public Overridable Property IsDestinationEnabled As Nullable(Of Boolean)

    Public Overridable Property IsForInterop As Boolean

    Public Overridable Property Configuration As PECMailBoxConfiguration

    Public Overridable Property IsHandleEnabled As Boolean?

    Public Overridable Property IsProtocolBox As Boolean?

    Public Overridable Property IsProtocolBoxExplicit As Boolean?

    Public Overridable Property IdJeepServiceIncomingHost As Guid?

    Public Overridable Property IdJeepServiceOutgoingHost As Guid?

    Public Overridable Property RulesetDefinition As String

    Public Overridable Property InvoiceType As InvoiceType?

    Public Overridable Property HumanEnabled As Boolean

#End Region

#Region " Methods "

    ''' <summary>
    ''' Indica se la mailbox corrente ha sia il server in ingresso che in uscita configurati
    ''' </summary>
    ''' <remarks>
    ''' Indica se GESTITA / NON GESTITA, soluzione temporanea perchè le colonne MANAGED e UNMANAGED sono usate e non è disponibile un'analisi.
    ''' </remarks>
    Public Overridable Function HasServers() As Boolean
        Return Not String.IsNullOrEmpty(OutgoingServerName) AndAlso Not String.IsNullOrEmpty(IncomingServerName)
    End Function

    Public Overridable Function IsSendingEnabled() As Boolean
        Return Not String.IsNullOrEmpty(OutgoingServerName)
    End Function

#End Region

End Class