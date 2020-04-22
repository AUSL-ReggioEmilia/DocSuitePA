Imports VecompSoftware.DocSuiteWeb.Data
Imports Newtonsoft.Json

Partial Public Class UtltProtAutorizza1
    Inherits UtltBasePage

#Region " Fields "

#End Region

#Region " Properties "

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False

        If Not IsPostBack Then
            UscSettori1.RoleRestictions = RoleRestrictions.None
        End If

        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma)
        AjaxManager.AjaxSettings.AddAjaxSetting(UscSettori1, UscSettori1)

    End Sub

    Protected Sub btnConferma_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnConferma.Click
        Dim jsonstring As String = "Autorizzazione|" & JsonConvert.SerializeObject(UscSettori1.GetRoles())

        MasterDocSuite.AjaxManager.ResponseScripts.Add("OnClick('" & jsonstring & "');")

    End Sub

#End Region

#Region " Methods "

#End Region

End Class