Imports System.IO
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI

Partial Public Class CommonSelTemplate
    Inherits CommBasePage

#Region "QueryString"
    Private ReadOnly Property Path() As String
        Get
            Return Request.QueryString("Path")
        End Get
    End Property

    Private ReadOnly Property Ext() As String
        Get
            Return Request.QueryString("Ext")
        End Get
    End Property
#End Region

#Region "Load Page"
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Not Me.IsPostBack Then
            Inizializza()
        End If
    End Sub
#End Region

#Region "Initialize"
    Private Sub Inizializza()

        Dim tn As New radTreeNode
        tn.Text = "Template"
        RadTreeTemplate.Nodes.Add(tn)

        Dim s As String
        Dim directory As DirectoryInfo = New DirectoryInfo(Path)
        Dim fileCount As FileSystemInfo() = directory.GetFiles
        Dim count As Integer = fileCount.Length - 1
        Dim i As Integer
        For i = 0 To count
            If InStr(fileCount(i).Extension, Ext) > 0 Then
                tn = New RadTreeNode
                tn.Text = fileCount(i).Name
                tn.Value = fileCount(i).Name
                tn.ImageUrl = ImagePath.FromFile(tn.Text)
                tn.Style.Add("font-weight", "bold")

                RadTreeTemplate.Nodes(0).Nodes.Add(tn)
            End If
        Next i
        RadTreeTemplate.Nodes(0).Expanded = True
    End Sub
#End Region

#Region "Events"
    Private Sub tvwFile_SelectedIndexChange(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadTreeNodeEventArgs) Handles RadTreeTemplate.NodeClick
        Dim nodo As RadTreeNode = e.Node
        If nodo.Value = "" Then Exit Sub
        Dim idFile As String = nodo.Text
        Ritorna(idFile)
    End Sub

    Private Sub Ritorna(ByVal id As String)
        Me.MasterDocSuite.AjaxManager.ResponseScripts.Add("CloseWindow('" & id & "');")
    End Sub
#End Region

End Class