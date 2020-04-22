Public Class uscProcessDetails
    Inherits DocSuite2008BaseControl

#Region " Properties "

    Public ReadOnly Property PanelDetails As Panel
        Get
            Return pnlDetails
        End Get
    End Property

#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        uscContattiSelRest.FilterByParentId = ProtocolEnv.FascicleContactId
    End Sub

#End Region

End Class