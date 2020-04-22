Imports VecompSoftware.Helpers.ExtensionMethods

Public Class ContainerExtensionCompositeKey

#Region " Fields "

    Private _idContainer As Integer
    Private _keyType As String
    Private _incremental As Short

#End Region

#Region " Properties "

    Public Overridable Property idContainer() As Integer
        Get
            Return _idContainer
        End Get
        Set(ByVal value As Integer)
            _idContainer = value
        End Set
    End Property

    ''' <summary> Boh </summary>
    ''' <remarks> 
    ''' TODO: Trasformare il tipo in <see cref="ContainerExtensionType"/>.
    ''' </remarks>
    Public Overridable Property KeyType() As String
        Get
            Return _keyType
        End Get
        Set(ByVal value As String)
            _keyType = value
        End Set
    End Property

    Public Overridable Property Incremental() As Short
        Get
            Return _incremental
        End Get
        Set(ByVal value As Short)
            _incremental = value
        End Set
    End Property

#End Region

#Region " Methods "

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf (obj) Is ContainerExtensionCompositeKey Then
            Dim compareTo As ContainerExtensionCompositeKey = DirectCast(obj, ContainerExtensionCompositeKey)
            Return idContainer = compareTo.idContainer And Incremental = compareTo.Incremental And KeyType.Eq(compareTo.KeyType)
        End If
        Return False

    End Function

    ''' <summary> ? </summary>
    ''' <remarks> 
    ''' TODO: Questa cosa non funziona ed è pericolosa, le collisioni che dovrebbe evitare questa funzione vengono moltiplicate in questo modo!
    ''' </remarks>
    Public Overrides Function GetHashCode() As Integer
        Return ToString().GetHashCode()
    End Function

    Public Overrides Function ToString() As String
        Return String.Format("{0}/{1}/{2}", idContainer, Incremental, KeyType)
    End Function

#End Region

End Class