Public Class uscReportDesignerToolbox
    Inherits DocSuite2008BaseControl

#Region " Fields "

#End Region

#Region " Properties "
    Public ReadOnly Property PageContentId As String
        Get
            Return pnlContent.ClientID
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

    End Sub
#End Region

#Region " Methods "

#End Region

End Class