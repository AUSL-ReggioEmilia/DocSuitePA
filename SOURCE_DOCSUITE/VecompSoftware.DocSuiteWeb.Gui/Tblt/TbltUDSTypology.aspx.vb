Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Entity.Workflows
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Extensions
Imports VecompSoftware.Helpers

Partial Class TbltUDSTypology
    Inherits CommonBasePage

#Region "Fields"
#End Region

#Region "Properties"

#End Region

#Region "Events"
    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub
#End Region

#Region "Methods"
    Private Sub InitializeAjax()

    End Sub

    Private Sub Initialize()
        grdUDSRepositories.DataSource = New List(Of String)
    End Sub
#End Region

End Class

