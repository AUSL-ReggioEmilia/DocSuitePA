Imports System.ComponentModel

<Serializable()>
Public Class PECMailReceipt
    Inherits AuditableDomainObject(Of Int32)

    Public Overridable Property PECMail As PECMail

    Public Overridable Property Parent As PECMail

    Public Overridable Property ReceiptType As String

    Public Overridable Property ErrorShort As String

    Public Overridable Property ErrorDescription As String

    Public Overridable Property DateZone As String

    Public Overridable Property ReceiptDate As DateTime

    Public Overridable Property Sender As String

    Public Overridable Property Receiver As String

    Public Overridable Property ReceiverType As String

    Public Overridable Property Subject As String

    Public Overridable Property Provider As String

    Public Overridable Property Identification As String

    Public Overridable Property MSGID As String

#Region " Methods "
    Public Overrides Function Clone() As Object
        Dim clonedPECMailReceipt As PECMailReceipt = CType(MyBase.Clone(), PECMailReceipt)
        '' Rimuovo l'id per evitare sovrapposizione
        clonedPECMailReceipt.Id = Nothing
        Return clonedPECMailReceipt
    End Function
#End Region


#Region " Constructor "
    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub
#End Region
End Class
