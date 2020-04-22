Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Facade

Public Class uscMonitoraggio
    Inherits DocSuite2008BaseControl
    Public ReadOnly Property PageContentDiv As HtmlTable
        Get
            Return tblMonitoraggio
        End Get
    End Property

    Public ReadOnly Property EditButton As RadImageButton
        Get
            Return btnEdit
        End Get
    End Property

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        btnEdit.Visible = False
        If ProtocolEnv.TransparentMonitoringEnabled Then
            If CommonShared.HasGroupTransparentManagerRight() Then
                btnEdit.Visible = True
            End If
        End If
    End Sub

End Class