Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class TbltPECMailBox
    Inherits CommonBasePage

    Public ReadOnly Property ViewLoginError As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)("ViewLoginError", False)
        End Get
    End Property

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not CommonShared.HasGroupAdministratorRight Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

        If Not IsPostBack Then
            uscPECMailBoxSettings.IsInsertAction = False
            uscPECMailBoxSettingsInsert.IsInsertAction = True
            uscPECMailBoxSettings.ValidationGroupName = "PecMailBoxUpdate"
            uscPECMailBoxSettingsInsert.ValidationGroupName = "PecMailBoxInsert"
        End If
    End Sub

End Class