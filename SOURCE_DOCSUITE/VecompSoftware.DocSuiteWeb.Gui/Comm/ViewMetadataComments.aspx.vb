Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class ViewMetadataComments
    Inherits CommonBasePage

    Protected ReadOnly Property DiscussionLabel As String
        Get
            Return Request.QueryString.GetValueOrDefault("Label", String.Empty)
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Title = String.Concat("Metadati - ", DiscussionLabel)
        MasterDocSuite.TitleVisible = False
    End Sub

End Class