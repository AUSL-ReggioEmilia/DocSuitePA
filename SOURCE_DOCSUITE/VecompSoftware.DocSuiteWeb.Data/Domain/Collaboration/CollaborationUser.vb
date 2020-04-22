<Serializable()>
Public Class CollaborationUser
    Inherits AuditableDomainObject(Of Guid)

#Region " Fields "

#End Region

#Region " Properties "

    Public Overridable Property IdCollaboration As Integer

    Public Overridable Property Incremental As Short

    Public Overridable Property DestinationFirst As Boolean?

    ''' <summary> Tipo di entità che gestirà la collaborazione. </summary>
    ''' <value> <see cref="DestinatonType"/> </value>
    ''' <remarks> A seconda del tipo cambia come si interpreta la <see cref="Account"/> </remarks>
    Public Overridable Property DestinationType As String

    ''' <summary> Se <see cref="DestinationType"/> assume il valore P. L'oggetto è un utente </summary>
    Public Overridable Property Account As String

    ''' <summary> Se <see cref="DestinationType"/> assume il valore S. L'oggetto è un <see cref="Role.Id"/></summary>
    Public Overridable Property IdRole As Short?

    Public Overridable Property DestinationName As String

    Public Overridable Property DestinationEMail As String

    Public Overridable Property Collaboration As Collaboration

#End Region

#Region " Constructors "

    Public Sub New()
    End Sub

    Public Sub New(sourceRole As Role)
        Me.New()
        IdRole = sourceRole.Id
        DestinationName = sourceRole.Name
        DestinationEMail = sourceRole.EMailAddress
        DestinationType = DestinatonType.S.ToString()
    End Sub

#End Region

End Class