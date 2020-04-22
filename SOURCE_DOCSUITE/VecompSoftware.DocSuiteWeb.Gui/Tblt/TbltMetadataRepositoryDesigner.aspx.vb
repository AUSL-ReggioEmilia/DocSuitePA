Imports System.Web
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class TbltMetadataRepositoryDesigner
    Inherits CommonBasePage

    Public ReadOnly Property IdMetadataRepository As String
        Get
            Return HttpContext.Current.Request.QueryString.GetValueOrDefault(Of String)("IdMetadtaRepository", Nothing)
        End Get
    End Property

    Public ReadOnly Property IsEditPage As Boolean
        Get
            Return HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Boolean)("IsEditPage", Nothing)
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

End Class