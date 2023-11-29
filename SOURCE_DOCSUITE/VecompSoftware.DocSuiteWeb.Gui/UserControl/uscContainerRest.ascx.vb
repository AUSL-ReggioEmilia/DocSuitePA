Public Class uscContainerRest
    Inherits DocSuite2008BaseControl

#Region " Properties "
    Public ReadOnly Property TableContentControl() As HtmlTable
        Get
            Return tblContainers
        End Get
    End Property

#End Region

#Region " Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
#End Region

#Region " Methods "

#End Region

End Class