Imports VecompSoftware.Helpers.ExtensionMethods

Public Class CollaborationHeaderMatcher

#Region " Fields "

    Private _propertyName As String
    Private _propertyValue As String

#End Region

#Region " Properties "

    Public Property PropertyName() As String
        Get
            Return _propertyName
        End Get
        Set(ByVal value As String)
            _propertyName = value
        End Set
    End Property

    Public Property PropertyValue() As String
        Get
            Return _propertyValue
        End Get
        Set(ByVal value As String)
            _propertyValue = value
        End Set
    End Property

#End Region

#Region " Constructors "

    Public Sub New()
        _propertyName = String.Empty
        _propertyValue = String.Empty
    End Sub

#End Region

#Region " Methods "

    Public Function CollaborationHeaderMatch(ByVal coll As CollaborationHeader) As Boolean
        Dim value As String = DirectCast(coll.GetType().GetProperty(_propertyName).GetValue(coll, Nothing), String)
        Return value.ContainsIgnoreCase(_propertyValue)

    End Function

#End Region

End Class