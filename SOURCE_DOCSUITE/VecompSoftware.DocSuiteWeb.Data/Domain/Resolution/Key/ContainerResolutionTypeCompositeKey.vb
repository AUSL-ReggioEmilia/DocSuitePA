<Serializable()> _
Public Class ContainerResolutionTypeCompositeKey

#Region " Fields "

    ''' <summary>  </summary>
    ''' <remarks> Questo dovrebbe essere uno short </remarks>
    Private _idResolutionType As Integer
    Private _idContainer As Short

#End Region

#Region " Properties "

    ''' <summary>  </summary>
    ''' <remarks> Questo dovrebbe essere uno short </remarks>
    Public Property idResolutionType() As Integer
        Get
            Return _idResolutionType
        End Get
        Set(ByVal value As Integer)
            _idResolutionType = value
        End Set
    End Property

    Public Property idContainer() As Integer
        Get
            Return _idContainer
        End Get
        Set(ByVal value As Integer)
            _idContainer = value
        End Set
    End Property

#End Region

#Region " Methods "

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        ' NOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
        Dim compareTo As ContainerResolutionTypeCompositeKey = DirectCast(obj, ContainerResolutionTypeCompositeKey)
        Return idResolutionType = compareTo.idResolutionType And idContainer = compareTo.idContainer()
    End Function

    Public Overrides Function GetHashCode() As Integer
        ' NOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
        Return ToString().GetHashCode()
    End Function

    Public Overrides Function ToString() As String
        Return String.Format("{0}/{1}", idContainer.ToString, idResolutionType.ToString())
    End Function

#End Region

End Class
