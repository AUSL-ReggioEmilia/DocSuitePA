Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade

Public Class TbltMetadataRepository
    Inherits CommonBasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not (CommonShared.HasGroupAdministratorRight OrElse CommonShared.UserConnectedBelongsTo(ProtocolEnv.MetadataRepositoryGroups)) Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

    End Sub

End Class