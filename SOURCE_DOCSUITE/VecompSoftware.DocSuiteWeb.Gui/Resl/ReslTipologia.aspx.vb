Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade

Public Class ReslTipologia
    Inherits ReslBasePage

#Region "Fields"

#End Region

#Region "Properties"

#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblRoleTypeResolutionRight) Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If
    End Sub
#End Region

#Region " Methods "

#End Region

End Class
