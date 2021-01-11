Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade

Partial Class TbltUDSTypology
    Inherits CommonBasePage

#Region "Fields"
#End Region

#Region "Properties"

#End Region

#Region "Events"
    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        If Not CommonShared.HasGroupAdministratorRight Then
            AjaxAlert("Sono necessari diritti amministrativi per vedere la pagina.")
            Exit Sub
        End If

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

