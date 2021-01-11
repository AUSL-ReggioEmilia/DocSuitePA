Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Extensions

Public Class TbltMassimarioScartoGes
    Inherits CommonBasePage

#Region "Fields"
    Private Const PAGE_TITLE As String = "Massimario di scarto"
#End Region

#Region "Properties"

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

#End Region

#Region "Methods"

    Private Sub Initialize()
        If Not CommonShared.HasGroupTblCategoryRight Then
            Throw New DocSuiteException("Utente non autorizzato alla visualizzazione del massimario di scarto.")
        End If

        MasterDocSuite.Title = PAGE_TITLE
        rgvCategories.DataSource = New List(Of Category)
        rgvCategories.DataBind()
    End Sub

    Private Sub InitializeAjax()

    End Sub

#End Region

End Class