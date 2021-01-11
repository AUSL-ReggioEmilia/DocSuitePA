Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Gui.FrameSet
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging


Public Class UtltHelp
    Inherits CommonBasePage


#Region " Constants "

    Private Const CommImages As String = "../Comm/Images/"


#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AjaxManager.AjaxSettings.AddAjaxSetting(trvHelp, userSplitter)
        If Not IsPostBack Then
            Dim list As New List(Of NodeLink)
            Me.CreateMenuHelp(list)
            Me.AddTreeViewItem(String.Empty, list, contentPane.ClientID)
        End If
    End Sub

    Private Sub trvHelp_NodeClick(sender As Object, e As RadTreeNodeEventArgs) Handles trvHelp.NodeClick
        Dim url As String = e.Node.Value
        If String.IsNullOrEmpty(url) Then
            Exit Sub
        End If
        contentPane.ContentUrl = url
    End Sub

#End Region

#Region " Methods "

    Private Sub CreateMenuHelp(ByRef alMenu As List(Of NodeLink))
        Dim helpFolder As String = Path.Combine(CommonInstance.AppPath, "Help")
        If Not Directory.Exists(helpFolder) Then
            FileLogger.Warn(LoggerName, "Cartella Help mancate o non valida: " & helpFolder)
            Return
        End If

        Dim helpDirectory As New DirectoryInfo(helpFolder)
        For Each subDirectory As DirectoryInfo In helpDirectory.GetDirectories()
            Dim files As FileInfo() = subDirectory.GetFiles("*.aspx")
            If files.IsNullOrEmpty() Then
                FileLogger.Warn(LoggerName, "Files di help mancanti o non validi: " & subDirectory.Name)
                Continue For
            End If

            Dim parameters As String = CommonShared.AppendSecurityCheck("Type=Comm")
            Dim parentName As String = "Documentazione " & subDirectory.Name.Split("-"c)(2)
            alMenu.Add(New NodeLink(NodeLevel.First, NodeImage.Help, parentName, String.Empty, parameters) With {.Expanded = False})
            For Each aspx As FileInfo In files
                Dim childName As String = Path.GetFileNameWithoutExtension(aspx.FullName).Split("-"c).Last()
                Dim childPage As String = Path.Combine("../" & helpDirectory.Name, subDirectory.Name, aspx.Name)
                alMenu.Add(New NodeLink(NodeLevel.Second, NodeImage.Point, childName, childPage, parameters))
            Next
        Next
    End Sub

    Private Sub AddTreeViewItem(ByVal treeId As String, ByVal menuList As IList(Of NodeLink), ByVal targetWindow As String)
        trvHelp.CssClass = "DocSuiteMenu"
        trvHelp.ShowLineImages = False
        trvHelp.SingleExpandPath = False
        'apertura singolo click
        trvHelp.OnClientNodeClicked = "NodeClicked"
        'nodo padre per di secondo livello
        Dim treeNode1 As RadTreeNode = Nothing
        ' Creo tutti i nodi
        For Each node As NodeLink In menuList

            Dim treeNode As New RadTreeNode()
            Select Case node.Image
                Case NodeImage.Line
                    treeNode.ImageUrl = CommImages & "Home/Linea.gif"
                Case NodeImage.Point
                    treeNode.ImageUrl = CommImages & "Home/Punto.gif"
                Case NodeImage.Desktop
                    treeNode.ImageUrl = ImagePath.SmallDesktop
                Case NodeImage.Help
                    treeNode.ImageUrl = ImagePath.SmallHelp
                Case Else
                    treeNode.ImageUrl = CommImages & "Home/Punto.gif"
            End Select

            treeNode.Text = node.Name
            treeNode.Expanded = node.Expanded
            If Not String.IsNullOrEmpty(node.Url) Then
                treeNode.Value = node.Url
            End If

            Select Case node.Level
                Case NodeLevel.First
                    trvHelp.Nodes.Add(treeNode)
                    treeNode1 = treeNode
                Case NodeLevel.Second
                    If treeNode1 IsNot Nothing Then
                        treeNode1.Nodes.Add(treeNode)
                    Else
                        Throw New DocSuiteException("Errore inizializzazione", String.Format("Impossibile aggiungere il nodo [{0}] al menu.", treeNode.Text))
                    End If
            End Select
        Next node

    End Sub

#End Region

End Class