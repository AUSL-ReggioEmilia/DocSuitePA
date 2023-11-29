Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade

Public Class TbltRepositoryGes
    Inherits CommonBasePage

#Region " Properties "
    Public ReadOnly Property PageAction As String
        Get
            Return GetKeyValue(Of String, TbltRepositoryGes)("Action")
        End Get
    End Property
#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False
        If Not CommonShared.HasGroupAdministratorRight Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

    End Sub
#End Region

End Class