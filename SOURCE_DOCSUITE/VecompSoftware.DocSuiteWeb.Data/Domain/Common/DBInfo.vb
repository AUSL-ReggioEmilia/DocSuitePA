
Public Class DBInfo
    Inherits DomainObject(Of DBInfoCompositeKey)

    Private _datatype As String
    Private _maxlength As Integer?

    Overridable Property DataType() As String
        Get
            Return _datatype
        End Get
        Set(ByVal value As String)
            _datatype = value
        End Set
    End Property

    Overridable Property MaxLength() As Integer?
        Get
            Return _maxlength
        End Get
        Set(ByVal value As Integer?)
            _maxlength = value
        End Set
    End Property

End Class
