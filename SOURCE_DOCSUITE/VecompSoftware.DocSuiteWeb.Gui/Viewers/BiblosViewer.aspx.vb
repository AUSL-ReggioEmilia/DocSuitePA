Imports System.Collections.Generic
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.ExtensionMethods
Imports System.Linq
Imports VecompSoftware.Services.Biblos.Models

Namespace Viewers
    Public Class BiblosViewer
        Inherits CommBasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            If Request.QueryString.AllKeys.FirstOrDefault(Function(f) f.Eq("prefixFileName")) IsNot Nothing Then
                ViewerLight.PrefixFileName = Request.QueryString.GetValue(Of String)("prefixFileName")
            End If
            If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
                MasterDocSuite.TitleVisible = False
                BindViewerLight()
            End If
            ViewerLight.AlwaysDocumentTreeOpen = ProtocolEnv.ViewLightAlwaysOpenPages.Contains("BiblosViewer")
        End Sub

        Private Sub BindViewerLight()
            Dim servername As String = Request.QueryString.GetValue(Of String)("servername")
            Dim guid As Guid = Request.QueryString.GetValue(Of Guid)("guid")
            Dim label As String = Request.QueryString.GetValue(Of String)("label")
            Dim document As Guid = Request.QueryString.GetValueOrDefault(Of Guid)("document", Guid.Empty)
            Dim ignoreState As Boolean = Request.QueryString.GetValueOrDefault(Of Boolean)("ignorestate", False)

            Dim docs As List(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocumentChildren(servername, guid, ignoreState)

            For Each doc As BiblosDocumentInfo In docs
                If doc.DocumentId = document Then
                    ViewerLight.DefaultDocument = doc.ToQueryString().AsEncodedQueryString()
                End If
            Next

            Dim temp As List(Of DocumentInfo) = docs.Cast(Of DocumentInfo)().ToList()
            Dim main As New FolderInfo() With {.Name = label}
            main.AddChildren(temp)

            ViewerLight.DataSource = New List(Of DocumentInfo) From {main}

        End Sub
    End Class
End Namespace