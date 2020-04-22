Public Class ChangeObjectParameter

#Region "Fields"
    Private _group As String = String.Empty
    Private _maxRecords As String = String.Empty
    Private _year As String = String.Empty
    Private _object As String = String.Empty
#End Region

#Region "Properties"
    Public ReadOnly Property Group() As String
        Get
            Return _group
        End Get
    End Property

    Public ReadOnly Property MaxRecords() As Integer
        Get
            If Not String.IsNullOrEmpty(_maxRecords) Then
                Return Convert.ToInt32(_maxRecords)
            Else
                Return DocSuiteContext.Current.ProtocolEnv.SearchMaxRecords
            End If
        End Get
    End Property

    Public ReadOnly Property Year() As Short
        Get
            If Not String.IsNullOrEmpty(_year) Then
                Return Convert.ToInt16(_year)
            Else
                Return 1000
            End If
        End Get
    End Property

    Public ReadOnly Property [Object]() As String
        Get
            If Not String.IsNullOrEmpty(_object) Then
                Return _object
            Else
                Return "Da Archiviare"
            End If
        End Get
    End Property
#End Region

#Region "Constructor"
    Public Sub New(ByVal paramStr As String)
        If Not String.IsNullOrEmpty(paramStr) Then
            Dim params As String() = paramStr.Split("|"c)
            _group = params(0)
            If params.Length > 1 Then
                _maxRecords = params(1)
            End If
            If params.Length > 2 Then
                _year = params(2)
            End If
            If params.Length > 1 Then
                _object = params(3)
            End If
        End If
    End Sub
#End Region

End Class
