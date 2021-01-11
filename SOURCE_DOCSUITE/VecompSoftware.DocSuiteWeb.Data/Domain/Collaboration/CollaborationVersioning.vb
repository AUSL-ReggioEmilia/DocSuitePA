<Serializable()>
Public Class CollaborationVersioning
    Inherits AuditableDomainObject(Of Guid)
    Implements ISupportLogicDelete

#Region " Fields "
#End Region

#Region " Properties "

    Public Overridable Property CollaborationIncremental As Short

    Public Overridable Property Incremental As Short

    Public Overridable Property IdDocument As Integer

    Public Overridable Property DocumentName As String

    ''' <summary> </summary>
    ''' <remarks> SUL DB È UN BIT </remarks>
    Public Overridable Property IsActive As Short Implements ISupportLogicDelete.IsActive

    Public Overridable Property Collaboration As Collaboration

    Public Overridable Property IdRepository As String

    Public Overridable Property CheckedOut As Boolean?

    Public Overridable Property CheckOutUser As String

    Public Overridable Property CheckOutSessionId As String

    Public Overridable Property CheckOutDate As DateTimeOffset?

    Public Overridable Property DocumentChecksum As String

    Public Overridable Property DocumentGroup As String


#End Region

#Region " Constructors "

    Public Sub New()
    End Sub

#End Region

End Class


