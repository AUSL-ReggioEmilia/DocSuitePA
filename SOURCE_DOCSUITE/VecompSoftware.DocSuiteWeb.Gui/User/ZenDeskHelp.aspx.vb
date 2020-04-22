Public Class ZenDeskHelp
    Inherits CommonBasePage

#Region " Fields "
#End Region

#Region " Properties "
    Public ReadOnly Property IsButtonPressed As String
        Get
            Return Request.QueryString("IsButtonPressed")
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        uscZenDeskHelp.AjaxDefaultLoadingPanel = MasterDocSuite.AjaxDefaultLoadingPanel
        uscZenDeskHelp.IsButtonPressed = IsButtonPressed
    End Sub
#End Region

#Region " Methods "
#End Region

End Class