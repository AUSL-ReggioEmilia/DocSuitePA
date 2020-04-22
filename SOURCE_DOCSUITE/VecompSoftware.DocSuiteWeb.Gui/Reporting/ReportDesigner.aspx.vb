Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class ReportDesigner
    Inherits CommonBasePage

#Region " Fields "

#End Region

#Region " Properties "
    Protected ReadOnly Property ToolboxItems As String
        Get
            Return My.Resources.ReportDesignerToolboxItems
        End Get
    End Property

    Protected ReadOnly Property ReportUniqueId As String
        Get
            Return Request.QueryString.GetValueOrDefault(Of String)("ReportUniqueId", Nothing)
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

    End Sub
#End Region

#Region " Methods "

#End Region

End Class