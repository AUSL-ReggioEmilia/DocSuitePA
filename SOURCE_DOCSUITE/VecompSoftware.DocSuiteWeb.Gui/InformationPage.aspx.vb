Imports System.Web
Imports VecompSoftware.DocSuiteWeb.Facade.Interfaces

Public Class InformationPage
    Inherits CommonBasePage

    Private Enum MessageType
        Info
    End Enum

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        SetResponseNoCache()

        If PreviousPage IsNot Nothing AndAlso TypeOf PreviousPage Is IHaveInformations Then
            Dim informations As InformationsMessage = CType(PreviousPage, IHaveInformations).InformationsMessage
            ' Gestione redirect se presente
            If informations.RedirectUriAddress IsNot Nothing Then Page.ClientScript.RegisterStartupScript(Page.GetType(), "PageRedirect", String.Format("DoRedirect({0},""{1}"");", informations.RedirectUriLatency * 1000, informations.RedirectUriAddress), True)
            ShowMessage(MessageType.Info, informations.InformationTitle, informations.InformationMessage)
        Else
            ShowMessage(MessageType.Info, "Nessun messaggio da visualizzare", String.Empty)
        End If

    End Sub

#End Region

#Region " Methods "

    Private Sub ShowMessage(type As MessageType, titleText As String, description As String)
        Select Case type
            Case MessageType.Info
                info.Text = "Informazione"
                image.ImageUrl = ImagePath.BigInfo
        End Select
        titolo.Text = titleText
        descrizione.Text = description
    End Sub
#End Region

End Class