Imports System.Web.UI.WebControls
Imports Telerik.Web.UI

Public Class CompositeTemplateColumn
    Inherits GridTemplateColumn

#Region " Methods "

    Protected Overrides Sub SetupFilterControls(ByVal cell As TableCell)
        MyBase.SetupFilterControls(cell)
    End Sub

    Public Overrides Function Clone() As GridColumn
        Return MyBase.Clone()
    End Function

    Protected Overrides Function GetFilterDataField() As String
        Return Me.DataField
    End Function

    Public Overrides Function SupportsFiltering() As Boolean
        Return True
    End Function

    Public Overridable Function GetSQLExpression(ByVal sqlParameter As String)
        Return "(convert(varchar,{alias}." & Me.DataField & ")) LIKE '%" & sqlParameter & "%'"
    End Function

#End Region

End Class
