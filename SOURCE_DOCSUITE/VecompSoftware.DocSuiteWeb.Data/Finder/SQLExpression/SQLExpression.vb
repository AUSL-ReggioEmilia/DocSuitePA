<Serializable()> _
Public Class SQLExpression
    Implements ISQLExpression

#Region "Fields"
    Private _sqlExpr As String
    Private _alias As IAliasFinder
#End Region

#Region "Properties"
    Public Property [Alias]() As IAliasFinder Implements ISQLExpression.Alias
        Get
            Return _alias
        End Get
        Set(ByVal value As IAliasFinder)
            _alias = value
        End Set
    End Property

    Public Property SQLExpr() As String Implements ISQLExpression.SQLExpr
        Get
            Return _sqlExpr
        End Get
        Set(ByVal value As String)
            _sqlExpr = value
        End Set
    End Property
#End Region

#Region "Constructor"
    Public Sub New()
        _sqlExpr = String.Empty
        _alias = Nothing
    End Sub

    Public Sub New(ByVal sqlExpr As String)
        _sqlExpr = sqlExpr
        _alias = Nothing
    End Sub

    Public Sub New(ByVal sqlExpr As String, ByVal [alias] As IAliasFinder)
        _sqlExpr = sqlExpr
        _alias = [alias]
    End Sub
#End Region

End Class