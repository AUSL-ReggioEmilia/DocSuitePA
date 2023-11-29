Imports System.Collections.Generic
Imports System.Web
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Partial Class TbltUDSRepositoriesTypologyGes
    Inherits CommonBasePage

#Region "Fields"

    Private _idUDSTypology As Guid?
#End Region

#Region "Properties"

    Public ReadOnly Property IdUDSTypology As Guid?
        Get
            If _idUDSTypology Is Nothing Then
                _idUDSTypology = HttpContext.Current.Request.QueryString.GetValueOrDefault(Of Guid?)("IdUDSTypology", Nothing)
            End If
            Return _idUDSTypology
        End Get
    End Property
#End Region

#Region "Events"
    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        MasterDocSuite.TitleVisible = False
        If Not CommonShared.HasGroupAdministratorRight Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

        If Not IsPostBack Then
            grdUDSRepositories.DataSource = New List(Of String)
        End If
    End Sub
#End Region

#Region "Methods"

#End Region

End Class

