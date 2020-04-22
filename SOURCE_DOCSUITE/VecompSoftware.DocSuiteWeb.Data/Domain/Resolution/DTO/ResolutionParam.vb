<Serializable()> _
Public Class ResolutionParam
    Private _id As Integer
    Public Overridable Property Id() As Integer
        Get
            Return _id
        End Get
        Set(ByVal value As Integer)
            _id = value
        End Set
    End Property

    Private _number As Integer?
    Public Overridable Property Number() As Integer?
        Get
            Return _number
        End Get
        Set(ByVal value As Integer?)
            _number = value
        End Set
    End Property

    Private _serviceNumber As String
    Public Overridable Property ServiceNumber() As String
        Get
            Return _serviceNumber
        End Get
        Set(ByVal value As String)
            _serviceNumber = value
        End Set
    End Property

    Private _year As Nullable(Of Short)
    Public Overridable Property Year() As Nullable(Of Short)
        Get
            Return _year
        End Get
        Set(ByVal value As Nullable(Of Short))
            _year = value
        End Set
    End Property

    Private _type As Short
    Public Property IdType() As Short
        Get
            Return _type
        End Get
        Set(ByVal value As Short)
            _type = value
        End Set
    End Property
End Class
