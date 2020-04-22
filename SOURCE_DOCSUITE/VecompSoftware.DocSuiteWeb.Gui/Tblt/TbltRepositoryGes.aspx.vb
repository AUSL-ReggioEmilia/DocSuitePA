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
    End Sub
#End Region

End Class