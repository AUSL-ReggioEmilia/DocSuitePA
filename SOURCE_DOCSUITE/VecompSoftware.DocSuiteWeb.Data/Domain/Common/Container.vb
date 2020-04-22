Imports VecompSoftware.Helpers.ExtensionMethods

<Serializable()> _
Public Class Container
    Inherits AuditableDomainObject(Of Int32)
    Implements ISupportLogicDelete, ISupportRangeDelete

#Region " Fields "

    Private _name As String
    Private _note As String
    Private _docmLocation As Location
    Private _reslLocation As Location
    Private _protLocation As Location
    Private _protAttachLocation As Location
    Private _massive As Nullable(Of Short)
    Private _idProtocolType As Nullable(Of Short)
    Private _registrationUser As String
    Private _registrationDate As DateTimeOffset
    Private _lastChangedUser As String
    Private _lastChangedDate As DateTimeOffset?
    Private _containerGroups As IList(Of ContainerGroup)
    Private _containerExtensions As IContainerExtensionCollection = New ContainerExtensionCollection()
    Private _conservation As Byte = 0
    Private _containerDocTypes As IList(Of ContainerDocType)
    Private _containerProperties As IList(Of ContainerProperty)
    Private _privacy As Nullable(Of Short)
    Private _headingFrontalino As String
    Private _headingLetter As String

#End Region

#Region " Constructor "
    Public Sub New()
        _containerGroups = New List(Of ContainerGroup)
        UniqueId = Guid.NewGuid()
    End Sub
#End Region

#Region " Properties "

    Overridable Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    Overridable Property Note() As String
        Get
            Return _note
        End Get
        Set(ByVal value As String)
            _note = value
        End Set
    End Property

    Overridable Property DocmLocation() As Location
        Get
            Return _docmLocation
        End Get
        Set(ByVal value As Location)
            _docmLocation = value
        End Set
    End Property

    Overridable Property ReslLocation() As Location
        Get
            Return _reslLocation
        End Get
        Set(ByVal value As Location)
            _reslLocation = value
        End Set
    End Property

    Overridable Property ProtLocation() As Location
        Get
            Return _protLocation
        End Get
        Set(ByVal value As Location)
            _protLocation = value
        End Set
    End Property

    Overridable Property ProtAttachLocation() As Location
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

    Public Overridable Property IsActive() As Short Implements ISupportLogicDelete.IsActive

    Public Overridable Property ActiveFrom() As DateTime? Implements ISupportRangeDelete.ActiveFrom

    Public Overridable Property ActiveTo() As DateTime? Implements ISupportRangeDelete.ActiveTo

    Overridable Property Massive() As Nullable(Of Short)
        Get
            Return _massive
        End Get
        Set(ByVal value As Nullable(Of Short))
            _massive = value
        End Set
    End Property

    Overridable Property IdProtocolType() As Nullable(Of Short)
        Get
            Return _idProtocolType
        End Get
        Set(ByVal value As Nullable(Of Short))
            _idProtocolType = value
        End Set
    End Property

    Overridable Property ContainerGroups() As IList(Of ContainerGroup)
        Get
            Return _containerGroups
        End Get
        Protected Set(ByVal value As IList(Of ContainerGroup))
            _containerGroups = value
        End Set
    End Property

    Public Overridable Property ContainerExtensions() As IContainerExtensionCollection
        Get
            Return _containerExtensions
        End Get
        Set(ByVal value As IContainerExtensionCollection)
            _containerExtensions = value
        End Set
    End Property

    Public Overridable Property ContainerProperties() As IList(Of ContainerProperty)
        Get
            Return _containerProperties
        End Get
        Protected Set(ByVal value As IList(Of ContainerProperty))
            _containerProperties = value
        End Set
    End Property

    Public Overridable ReadOnly Property AccountingSectionals() As IList(Of String)
        Get
            Dim vList As New List(Of String)
            For Each vExt As ContainerExtension In _containerExtensions.FilterType(ContainerExtensionType.SC)
                vList.Add(vExt.KeyValue)
            Next
            Return vList
        End Get
    End Property

    Public Overridable ReadOnly Property IsInvoiceEnable() As Boolean
        Get
            Dim vList As IContainerExtensionCollection = _containerExtensions.FilterType(ContainerExtensionType.FT)
            If Not vList.IsNullOrEmpty() Then
                Return vList(0).KeyValue.Eq("1"c)
            End If
            Return False
        End Get
    End Property

    ''' <summary> ? </summary>
    ''' <remarks> Non funzionava in originale per i casi A e B. </remarks>
    Public Overridable ReadOnly Property ContainerType() As ContainerType
        Get
            Dim vList As IContainerExtensionCollection = _containerExtensions.FilterType(ContainerExtensionType.TP)
            If vList.Count > 1 Then
                Throw New DocSuiteException("Tipo Contenitore", "Tipo Contenitore non univoco")
            End If

            If vList.Count = 0 Then
                Return ContainerType.None
            End If

            ' TODO: rivedere tutto, da test non sembra arrivare qui, fare attenzione
            ' Se l'estensione è solo una
            If vList(0).KeyType.Eq("A"c) Then
                Return ContainerType.A
            ElseIf vList(0).KeyType.Eq("B"c) Then
                Return ContainerType.P
            End If

            Throw New DocSuiteException("Tipo Contenitore", "Tipo Contenitore non gestito, Tipologie valide A/P")

        End Get
    End Property

    Public Overridable ReadOnly Property IsMailCCEnable() As Boolean
        Get
            Dim vList As IContainerExtensionCollection = _containerExtensions.FilterType(ContainerExtensionType.CM)
            If vList.Count > 0 Then
                Return vList(0).KeyValue = "1"
            Else
                Return False
            End If
        End Get
    End Property

    Public Overridable Property Conservation() As Byte
        Get
            Return _conservation
        End Get
        Set(ByVal value As Byte)
            _conservation = value
        End Set
    End Property

    Public Overridable Property ContainerDocTypes() As IList(Of ContainerDocType)
        Get
            Return _containerDocTypes
        End Get
        Set(ByVal value As IList(Of ContainerDocType))
            _containerDocTypes = value
        End Set
    End Property

    Public Overridable Property DocumentSeriesLocation() As Location

    Public Overridable Property DocumentSeriesAnnexedLocation() As Location

    ''' <summary> Abilita il rifiuto di protocollo. </summary>
    Public Overridable Property DocumentSeriesUnpublishedAnnexedLocation() As Location

    ''' <summary></summary>
    ''' <remarks>Nato per ASL-TO2</remarks>
    Public Overridable Property Privacy() As Nullable(Of Short)
        Get
            Return _privacy
        End Get
        Set(ByVal value As Nullable(Of Short))
            _privacy = value
        End Set
    End Property

    ''' <summary></summary>
    ''' <remarks>Nato per ASL-TO2</remarks>
    Public Overridable Property HeadingFrontalino() As String
        Get
            Return _headingFrontalino
        End Get
        Set(ByVal value As String)
            _headingFrontalino = value
        End Set
    End Property

    ''' <summary></summary>
    ''' <remarks>Nato per ASL-TO2</remarks>
    Public Overridable Property HeadingLetter() As String
        Get
            Return _headingLetter
        End Get
        Set(ByVal value As String)
            _headingLetter = value
        End Set
    End Property
    
    Public Overridable Property Fascicles As IList(Of Fascicle)

    Public Overridable Property Archive As ContainerArchive

    Public Overridable Property DeskLocation As Location

    Public Overridable Property UDSLocation As Location

    Public Overridable Property ManageSecureDocument As Boolean

    Public Overridable Property PrivacyLevel As Integer

    Public Overridable Property PrivacyEnabled As Boolean
#End Region

#Region " Methods "

    Public Overridable Function IsActiveRange() As Boolean Implements ISupportRangeDelete.IsActiveRange
        Return (Not ActiveFrom.HasValue AndAlso Not ActiveTo.HasValue) OrElse (ActiveFrom.Value < Date.Now AndAlso Date.Now < ActiveTo.Value)
    End Function

#End Region

End Class
