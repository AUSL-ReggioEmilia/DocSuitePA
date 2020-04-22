Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class PECDestination
    Inherits PECBasePage




#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False

        If Not IsPostBack AndAlso PreviousPage IsNot Nothing Then
            If Not CurrentPecMailRights.IsDestinable Then
                Throw New DocSuiteException("Diritti non sufficienti per l'operazione richiesta.")
            End If

            txtDestinationNotes.Text = CurrentPECMail.DestinationNote

            cmdCancel.PostBackUrl = PreviousPageUrl
        End If
    End Sub

    Private Sub CmdSaveClick(sender As Object, e As EventArgs) Handles cmdSave.Click
        CurrentPECMail.DestinationNote = txtDestinationNotes.Text
        CurrentPECMail.IsDestinated = True

        Facade.PECMailFacade.Update(CurrentPECMail)

        Response.Redirect(PreviousPageUrl)
    End Sub

#End Region

    Private Sub CmdCancelClick(sender As Object, e As EventArgs) Handles cmdCancel.Click

    End Sub
End Class