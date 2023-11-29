Imports VecompSoftware.Helpers.ExtensionMethods

<Serializable()> _
Public Class Container
    Inherits AuditableDomainObject(Of Int32)
    Implements ISupportBooleanLogicDelete, ISupportRangeDelete

#Region " Fields "

    Private _protLocation As Location
    Private _protAttachLocation As Location

#End Region

#Region " Constructor "
    Public Sub New()
        ContainerGroups = New List(Of ContainerGroup)
        UniqueId = Guid.NewGuid()
    End Sub
#End Region

#Region " Properties "

    Overridable Property Name As String

    Overridable Property Note As String

    Overridable Property DocmLocation As Location

    Overridable Property ReslLocation As Location

    Overridable Property ProtLocation As Location
        Get
            Return _protLocation
        End Get
        Set(ByVal value As Location)
            _protLocation = value
        End Set
    End Property

    Overridable Property ProtAttachLocation As Location
        Get
            If _protAttachLocation Is Nothing Then
                Return _protLocation
            End If
            Return _protAttachLocation
        End Get
        Set(ByVal value As Location)
            _protAttachLocation = value
        End Set
    End Property

    Public Overridable Property IsActive As Boolean Implements ISupportBooleanLogicDelete.IsActive

    Overridable Property ContainerGroups As IList(Of ContainerGroup)

    Public Overridable Property ContainerProperties As IList(Of ContainerProperty)

    Public Overridable Property DocumentSeriesLocation As Location

    Public Overridable Property DocumentSeriesAnnexedLocation As Location

    ''' <summary> Abilita il rifiuto di protocollo. </summary>
    Public Overridable Property DocumentSeriesUnpublishedAnnexedLocation As Location

    ''' <summary></summary>
    ''' <remarks>Nato per ASL-TO2</remarks>
    Public Overridable Property Privacy As Short?

    ''' <summary></summary>
    ''' <remarks>Nato per ASL-TO2</remarks>
    Public Overridable Property HeadingFrontalino As String

    ''' <summary></summary>
    ''' <remarks>Nato per ASL-TO2</remarks>
    Public Overridable Property HeadingLetter As String

    Public Overridable Property Archive As ContainerArchive

    Public Overridable Property DeskLocation As Location

    Public Overridable Property UDSLocation As Location

    Public Overridable Property PrivacyLevel As Integer

    Public Overridable Property PrivacyEnabled As Boolean
#End Region

#Region " Methods "

    Public Overridable Function IsActiveRange() As Boolean Implements ISupportRangeDelete.IsActiveRange
        Return IsActive = True
    End Function

#End Region

End Class
