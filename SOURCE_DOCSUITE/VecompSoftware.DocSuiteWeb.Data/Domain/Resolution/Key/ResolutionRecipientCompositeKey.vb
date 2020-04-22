<Serializable()> _
Public Class ResolutionRecipientCompositeKey

#Region "Private Fields"
    Private _idResolution As Integer
    Private _idRecipient As Short
#End Region

#Region "Public Properties"
    Public Property IdResolution() As Integer
        Get
            Return _idResolution
        End Get
        Set(ByVal value As Integer)
            _idResolution = value
        End Set
    End Property

    Public Property IdRecipient() As Integer
        Get
            Return _idRecipient
        End Get
        Set(ByVal value As Integer)
            _idRecipient = value
        End Set
    End Property
#End Region

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        Dim compareTo As ResolutionRecipientCompositeKey = DirectCast(obj, ResolutionRecipientCompositeKey)
        Return Me.IdResolution = compareTo.IdResolution And Me.IdRecipient = compareTo.IdRecipient()
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return Me.ToString().GetHashCode()
    End Function

    Public Overrides Function ToString() As String
        Return IdRecipient.ToString + "/" + IdResolution.ToString()
    End Function


End Class