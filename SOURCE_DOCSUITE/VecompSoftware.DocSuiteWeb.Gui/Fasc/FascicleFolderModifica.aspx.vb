Public Class FascicleFolderModifica
    Inherits FascBasePage

#Region " Fields "
#End Region

#Region " Properties "


    Protected ReadOnly Property SessionUniqueKey As String
        Get
            Return GetKeyValueOrDefault(Of String)("SessionUniqueKey", String.Empty)
        End Get
    End Property

    Protected ReadOnly Property DoNotUpdateDatabase As Boolean
        Get
            Return GetKeyValueOrDefault(Of Boolean)("DoNotUpdateDatabase", False)
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma)
    End Sub
#End Region

End Class