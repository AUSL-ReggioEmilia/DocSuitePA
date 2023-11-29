Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class TbltProcess
    Inherits CommBasePage

#Region " Properties "
    Protected ReadOnly Property ProcessId As String
        Get
            Dim _processId As String = Request.QueryString.GetValueOrDefault("IdProcess", String.Empty)
            Return _processId
        End Get
    End Property
    Protected ReadOnly Property CategoryId As String
        Get
            Dim _categoryId As String = Request.QueryString.GetValueOrDefault("IdCategory", String.Empty)
            Return _categoryId
        End Get
    End Property
#End Region

#Region "Methods"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not ProtocolEnv.ProcessEnabled OrElse (Not CommonShared.HasGroupAdministratorRight AndAlso CommonShared.HasGroupProcessesViewsManageableRight) Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

    End Sub
#End Region
End Class