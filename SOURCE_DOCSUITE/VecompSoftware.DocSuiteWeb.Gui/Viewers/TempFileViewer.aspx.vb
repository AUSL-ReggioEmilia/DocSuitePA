Imports System.IO
Imports System.Linq
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos.Models

Namespace Viewers
    Public Class TempFileViewer
        Inherits CommBasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
                BindViewerLight()
                MasterDocSuite.TitleVisible = False
            End If
            ViewerLight.AlwaysDocumentTreeOpen = ProtocolEnv.ViewLightAlwaysOpenPages.Contains("TempFileViewer")
        End Sub

        Private Sub BindViewerLight()
            Dim file As String = Request.QueryString.GetValue(Of String)("DownloadFile")
            Dim label As String = Request.QueryString.GetValueOrDefault(Of String)("label", file)
            Dim destination As String = Path.Combine(CommonUtil.GetInstance().TempDirectory.FullName, file)
            Dim doc As New FileDocumentInfo(New FileInfo(destination))
            doc.Name = label

            Dim main As New FolderInfo() With {.Name = "Anteprima documento", .ID = "Temp"}
            main.AddChild(doc)

            ViewerLight.DataSource = New List(Of DocumentInfo) From {main}
        End Sub
    End Class
End Namespace