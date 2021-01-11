Public Class FascicleFolderModifica
    Inherits FascBasePage

#Region " Fields "

    Private _idFascicleFolder As Guid?
#End Region

#Region " Properties "
    Protected ReadOnly Property IdFascicleFolder As Guid
        Get
            If _idFascicleFolder Is Nothing Then
                _idFascicleFolder = GetKeyValueOrDefault(Of Guid?)("IdFascicleFolder", Nothing)
            End If
            If _idFascicleFolder.HasValue Then
                Return _idFascicleFolder.Value
            Else
                Return Guid.Empty
            End If
        End Get
    End Property

    Protected ReadOnly Property SessionUniqueKey As String
        Get
            Return GetKeyValueOrDefault(Of String)("SessionUniqueKey", String.Empty)
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