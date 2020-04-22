Imports System.IO

<Serializable()> _
Public Class FilterExpression
    Implements IFilterExpression

    Public Enum FilterType
        NoFilter = 0
        Contains = 1
        EqualTo = 2
        GreaterThan = 3
        GreaterThanOrEqualTo = 4
        LessThan = 5
        LessThanOrEqualTo = 6
        IsNull = 7
        IsNotNull = 8
        StartsWith = 9
        SQL = 10
        Criteria = 11
        IsEnum = 12
    End Enum

#Region " Fields "

    Private _propertyName As String
    Private _filterValue As Object
    Private _filterExpression As FilterType
    Private _propertyType As Type
    Private _sqlExpression As String
    Private _criteria As Byte()

#End Region

#Region " Properties "

    Public Property PropertyName() As String Implements IFilterExpression.PropertyName
        Get
            Return _propertyName
        End Get
        Set(ByVal value As String)
            _propertyName = value
        End Set
    End Property

    Public Property FilterExpression() As FilterType Implements IFilterExpression.FilterExpression
        Get
            Return _filterExpression
        End Get
        Set(ByVal value As FilterType)
            _filterExpression = value
        End Set
    End Property

    Public Property FilterValue() As Object Implements IFilterExpression.FilterValue
        Get
            Return _filterValue
        End Get
        Set(ByVal value As Object)
            _filterValue = value
        End Set
    End Property

    Public Property PropertyType() As Type Implements IFilterExpression.PropertyType
        Get
            Return _propertyType
        End Get
        Set(ByVal value As Type)
            _propertyType = value
        End Set
    End Property

    Public Property SQLExpression() As String Implements IFilterExpression.SQLExpression
        Get
            Return _sqlExpression
        End Get
        Set(ByVal value As String)
            _sqlExpression = value
        End Set
    End Property

    Public Property CriteriaImpl() As Object Implements IFilterExpression.CriteriaImpl
        Get
            Return DeSerialize(_criteria)
        End Get
        Set(ByVal value As Object)
            _criteria = Serialize(value)
        End Set
    End Property

#End Region

#Region " Constructors "

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal propertyName As String, ByVal propertyType As Type, ByVal filterValue As Object, ByVal filterExpression As FilterType)
        _propertyName = propertyName
        _propertyType = propertyType
        _filterValue = filterValue
        _filterExpression = filterExpression
    End Sub

#End Region

#Region " Methods "

    Private Function Serialize(ByVal obj As Object) As Byte()
        If obj Is Nothing Then
            Return Nothing
        End If

        Using ms As New MemoryStream
            Dim bf As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
            bf.Serialize(ms, obj)
            Return ms.ToArray()
        End Using
    End Function

    Private Function DeSerialize(ByVal buffer As Byte()) As Object
        If buffer Is Nothing Then
            Return Nothing
        End If

        Using ms As New MemoryStream(buffer)
            Dim bf As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
            Return bf.Deserialize(ms)
        End Using
    End Function

#End Region

End Class
