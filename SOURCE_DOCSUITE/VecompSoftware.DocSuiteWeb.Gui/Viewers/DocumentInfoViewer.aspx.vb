Imports System.Linq
Imports System.Collections.Generic
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class DocumentInfoViewer
    Inherits CommBasePage

#Region "Property"
    Public ReadOnly Property ViewOriginal As Boolean
        Get
            Dim param As String = Request.QueryString("ViewOriginal")
            If String.IsNullOrEmpty(param) Then
                Return False
            End If
            Dim result As Boolean = False
            Boolean.TryParse(param, result)
            Return result
        End Get
    End Property
#End Region

#Region "Event"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
            MasterDocSuite.TitleVisible = False
            BindViewerLight()
        End If
    End Sub
#End Region

#Region "Methods"
    Private Sub BindViewerLight()
        Dim doc As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(Request.QueryString)
        Dim isPublic As Boolean = Request.QueryString.GetValueOrDefault("Public", False)
        If isPublic Then
            doc.AddAttribute(ViewerLight.BIBLOS_ATTRIBUTE_IsPublic, "True")
        End If
        If ViewOriginal Then
            ViewerLight.ViewOriginal = True
        End If
        ViewerLight.DataSource = New List(Of DocumentInfo) From {doc}
        ViewerLight.AlwaysDocumentTreeOpen = ProtocolEnv.ViewLightAlwaysOpenPages.Contains("DocumentInfoViewer")
    End Sub
#End Region

End Class