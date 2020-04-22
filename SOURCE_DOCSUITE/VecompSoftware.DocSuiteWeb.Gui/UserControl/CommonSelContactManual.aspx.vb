Imports VecompSoftware.Helpers.Web.ExtensionMethods

Partial Public Class CommonSelContactManual
    Inherits CommonBasePage

    ' TODO: è uguale alla CommonContactGes
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False

        uscContattiManualGes.ManualContactMode = True
        uscContattiManualGes.ProtType = Not String.IsNullOrEmpty(Request.QueryString("ProtType"))
        uscContattiManualGes.IsReadOnly = Request.QueryString.GetValueOrDefault(Of Boolean)("ReadOnly", False)
        uscContattiManualGes.IsFiscalCodeRequired = Request.QueryString.GetValueOrDefault(Of Boolean)("FCRequired", False)
        uscContattiManualGes.SimpleMode = Request.QueryString.GetValueOrDefault(Of Boolean)("SimpleMode", False)
    End Sub

End Class