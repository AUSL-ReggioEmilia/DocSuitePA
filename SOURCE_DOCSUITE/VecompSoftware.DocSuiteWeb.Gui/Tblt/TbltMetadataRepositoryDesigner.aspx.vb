Imports System.Web
Imports VecompSoftware.DocSuiteWeb.Facade
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
        If Not CommonShared.HasGroupAdministratorRight Then
            AjaxAlert("Sono necessari diritti amministrativi per vedere la pagina.")
            Exit Sub
        End If

    End Sub

End Class