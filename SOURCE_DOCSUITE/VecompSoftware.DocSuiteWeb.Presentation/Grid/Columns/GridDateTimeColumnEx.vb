Imports Telerik.Web.UI

Public Class GridDateTimeColumnEx
    Inherits GridDateTimeColumn

#Region " Fields "

    Protected _sql As String = String.Empty

#End Region

#Region " Properties "

    Public Property SQLExpression() As String
        Get
            Return _sql
        End Get
        Set(ByVal value As String)
            _sql = value
        End Set
    End Property

#End Region

#Region " Methods "

    Public Overridable Function GetSQLExpression(ByVal sqlParameter As String, ByVal command As String)
        Dim op As String = GetFilterOperator(command)

        If String.IsNullOrEmpty(SQLExpression) And Not String.IsNullOrEmpty(op) Then
            Dim param As String = String.Format("{0:yyyyMMdd}", DateTime.Parse(sqlParameter))
            _sql = "{alias}.StartDate " & op & " '" & param & "' OR {alias}.EndDate " & op & " '" & param & "'"
        End If

        Return _sql
    End Function

    Private Function GetFilterOperator(ByVal command) As String
        Dim sRet As String
        Select Case command
            Case "NoFilter"
                sRet = ""
            Case "EqualTo"
                sRet = "="
            Case "GreaterThan"
                sRet = ">"
            Case "GreaterThanOrEqualTo"
                sRet = ">="
            Case "LessThan"
                sRet = "<"
            Case "LessThanOrEqualTo"
                sRet = "<="
            Case Else
                sRet = "="
        End Select

        Return sRet

    End Function

#End Region

End Class
