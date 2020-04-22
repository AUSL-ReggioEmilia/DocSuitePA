Partial Public Class CommonContactGes
    Inherits CommBasePage

    ' TODO: è uguale alla CommonSelContactManual

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False

        uscContattiGes.SaveToDb = True
    End Sub

End Class