
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ResetParameters
    Inherits CommonBasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        DocSuiteContext.Current.Reset()
        VecompSoftware.Services.StampaConforme.Service.InitializeSignatureTemplateXml(DocSuiteContext.Current.ProtocolEnv.SignatureTemplate)

        LastUpdate.Text = "Ultimo aggiornamento parametri da DB: " & DocSuiteContext.Current.LastUpdate.ToLongDateString() & " " & DocSuiteContext.Current.LastUpdate.ToLongTimeString
    End Sub

End Class