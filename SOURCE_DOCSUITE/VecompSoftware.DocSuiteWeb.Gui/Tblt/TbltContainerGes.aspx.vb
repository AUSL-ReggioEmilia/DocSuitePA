Imports System.Collections.Generic
Imports System.Web
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Public Class TbltContainerGes
    Inherits CommonBasePage
#Region "Fields"

    Private _idUCategory As String
#End Region

#Region "Properties"
    Public ReadOnly Property IdUCategory As String
        Get
            If _idUCategory Is Nothing Then
                _idUCategory = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of String)("CategoryID", Nothing)
            End If
            Return _idUCategory
        End Get
    End Property
#End Region
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblContainerAdminRight) Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

        MasterDocSuite.TitleVisible = False
        If Not IsPostBack Then
            grdContainers.DataSource = New List(Of String)
        End If
    End Sub

End Class