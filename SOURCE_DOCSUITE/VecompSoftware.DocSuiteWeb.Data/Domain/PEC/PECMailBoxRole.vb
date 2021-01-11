
<Serializable()> _
Public Class PECMailBoxRole
    Inherits DomainObject(Of PecMailBoxRoleCompositeKey)

#Region " Fields "
#End Region

#Region " Properties "

    ''' <summary>Stabilisce se la MailBox è quella di default quando si visualizzano legate al settore</summary>
    ''' <remarks>Se necessario ordinamento cambiare questa proprietà in intero</remarks>
    Public Overridable Property Priority As Boolean

    Public Overridable Property Role As Role

    Public Overridable Property PECMailBox As PECMailBox

#End Region

#Region " Constructors "

    Public Sub New()
        Id = New PecMailBoxRoleCompositeKey()
        _pecMailBox = New PECMailBox
        _role = New Role
    End Sub

#End Region

End Class
