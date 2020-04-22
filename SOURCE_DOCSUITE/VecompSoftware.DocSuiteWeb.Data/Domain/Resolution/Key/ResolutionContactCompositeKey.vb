<Serializable()> _
Public Class ResolutionContactCompositeKey

#Region "Private Fields"
    Private _idResolution As Integer
    Private _idContact As Integer
    Private _comunicationType As String
#End Region

#Region "Public Properties"
    Public Overridable Property IdResolution() As Integer
        Get
            Return _idResolution
        End Get
        Set(ByVal value As Integer)
            _idResolution = value
        End Set
    End Property
    Public Overridable Property IdContact() As Integer
        Get
            Return _idContact
        End Get
        Set(ByVal value As Integer)
            _idContact = value
        End Set
    End Property
    Public Overridable Property ComunicationType() As String
        Get
            Return _comunicationType
        End Get
        Set(ByVal value As String)
            _comunicationType = value
        End Set
    End Property
#End Region

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        Dim compareTo As ResolutionContactCompositeKey = DirectCast(obj, ResolutionContactCompositeKey)
        Return Me.ComunicationType = compareTo.ComunicationType And Me.IdContact = compareTo.IdContact() And Me.IdResolution = compareTo.IdResolution
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return Me.ToString().GetHashCode()
    End Function

    Public Overrides Function ToString() As String
        Return IdContact.ToString + "/" + IdResolution.ToString() + "/" + ComunicationType.ToString()
    End Function


End Class