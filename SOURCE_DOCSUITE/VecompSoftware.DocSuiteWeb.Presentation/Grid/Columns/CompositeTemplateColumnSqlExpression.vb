Imports System.Web.UI.WebControls
Imports Telerik.Web.UI
Public Class CompositeTemplateColumnSqlExpression
    Inherits GridTemplateColumn

    Public Enum ColumnBinding
        DefaultBinding = 0
        CustomBinding = 1
    End Enum

    Public Delegate Function SqlExpressionDelegate(ByVal sqlParameter As String) As Object

#Region " Fields "

    Private _sql As String
    Private _type As ColumnBinding = ColumnBinding.DefaultBinding


    Private _delegate As SqlExpressionDelegate

#End Region

#Region " Properties "

    Public Property SQLQuery() As String
        Get
            Return _sql
        End Get
        Set(ByVal value As String)
            _sql = value
        End Set
    End Property

    Public Property CustomExpressionDelegate() As SqlExpressionDelegate
        Get
            Return _delegate
        End Get
        Set(ByVal value As SqlExpressionDelegate)
            _delegate = value
        End Set
    End Property

    Public Property BindingType() As ColumnBinding
        Get
            Return _type
        End Get
        Set(ByVal value As ColumnBinding)
            _type = value
        End Set
    End Property

#End Region

#Region " Methods "

    Public Overridable Function GetExpression(ByVal sqlParameter As String) As Object
        If _delegate IsNot Nothing Then
            Return _delegate.DynamicInvoke(sqlParameter)
        Else
            Return String.Format(_sql, sqlParameter)
        End If
    End Function

#End Region

End Class

