Imports System.Linq
Imports System.Collections.Generic
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Helpers.ExtensionMethods

Public Class BiblosChainInfoViewer
    Inherits CommBasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
            MasterDocSuite.TitleVisible = False
            BindViewerLight()
        End If
        ViewerLight.AlwaysDocumentTreeOpen = ProtocolEnv.ViewLightAlwaysOpenPages.Contains("BiblosChainInfoViewer")
    End Sub

    Private Sub BindViewerLight()

        Dim rootLabel As String = Request.QueryString.GetValueOrDefault(Of String)("Label", String.Empty)

        Dim idChain As Guid = Request.QueryString.GetValue(Of Guid)("ChainId")

        Dim docs As IList(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(idChain)
        If rootLabel.Eq("Allegati riservati") Then
            For Each item As BiblosDocumentInfo In docs
                item.AddAttribute(ViewerLight.BIBLOS_ATTRIBUTE_IsPublic, "True")
            Next
        End If

        If String.IsNullOrEmpty(rootLabel) Then
            ViewerLight.DataSource = CType(docs, IList(Of DocumentInfo))
        Else
            Dim doc As New FolderInfo(rootLabel, rootLabel)
            doc.Children.AddRange(docs)
            ViewerLight.DataSource = New List(Of DocumentInfo) From {doc}
        End If
    End Sub

End Class