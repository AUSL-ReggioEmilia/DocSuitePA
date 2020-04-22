Imports Telerik.Web.UI

Public Class CompositeTemplateExportableColumn
    Inherits GridTemplateColumn
    Implements IExportableColumn

#Region " Fields "
    Public Delegate Function ExportExpressionDelegate(dataItem As GridDataItem) As String
#End Region

#Region " Properties "
    Public Property CustomExportDelegate As ExportExpressionDelegate
#End Region

#Region " Constructor "
    Public Sub New()
        MyBase.New()

    End Sub
#End Region

#Region " Methods "
    Public Function GetExportableValue(dataItem As GridDataItem) As String Implements IExportableColumn.GetExportableValue
        If CustomExportDelegate Is Nothing Then
            Return String.Empty
        End If
        Return CustomExportDelegate.Invoke(dataItem)
    End Function
#End Region

End Class
