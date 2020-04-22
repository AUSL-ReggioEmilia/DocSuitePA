Public Class ContactModifyParameter

#Region "Fields"
    Private _enable As Boolean
    Private _protYear As Nullable(Of Short)
#End Region

#Region "Properties"
    Public Property Enable() As Boolean
        Get
            Return _enable
        End Get
        Set(ByVal value As Boolean)
            _enable = value
        End Set
    End Property

    Public Property ProtocolYear() As Nullable(Of Short)
        Get
            Return _protYear
        End Get
        Set(ByVal value As Nullable(Of Short))
            _protYear = value
        End Set
    End Property
#End Region

#Region "Constructors"
    Public Sub New(ByVal value As String)
        Dim values As String() = Split(value, "|")
        If CheckParameter(values) Then
            _enable = (Int32.Parse(values(0)) = 1)
            If values.Length > 1 Then
                ProtocolYear = Short.Parse(values(1))
            End If
        Else
            Enable = False
            ProtocolYear = Nothing
        End If
    End Sub
#End Region

#Region "Public Functions"
    Public Function CheckYearModify(ByVal protocolYear As Short) As Boolean
        If Me.Enable Then
            If Me.ProtocolYear.HasValue Then
                Return _protYear.Equals(protocolYear)
            Else
                Return True
            End If
        Else
            Return False
        End If
    End Function
#End Region

#Region "Private Functions"
    Private Function CheckParameter(ByVal values As String()) As Boolean
        Return (values.Length > 0 AndAlso values(0) <> "")
    End Function
#End Region

End Class
