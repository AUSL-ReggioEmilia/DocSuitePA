Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class TbltTemplateCollaboration
    Inherits UserBasePage

    Protected ReadOnly Property ViewNotActive As Boolean
        Get
            Return Request.QueryString.GetValueOrDefault(Of Boolean)("ViewNotActive", False)
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not (CommonShared.HasGroupAdministratorRight OrElse CommonShared.UserConnectedBelongsTo(ProtocolEnv.TemplateCollaborationGroups)) Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub Initialize()
        If Not CommonShared.UserConnectedBelongsTo(ProtocolEnv.TemplateCollaborationGroups) AndAlso Not CommonShared.HasGroupAdministratorRight Then
            Throw New DocSuiteException("Utente non abilitato alla gestione del template di Collaborazione")
        End If

    End Sub
End Class