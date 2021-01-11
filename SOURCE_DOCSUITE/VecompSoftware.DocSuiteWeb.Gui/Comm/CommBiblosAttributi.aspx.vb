Imports System.Xml
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI

Partial Class CommBiblosAttributi
    Inherits CommBasePage


#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim reader As XmlTextReader
        Dim document As New XmlDocument
        Dim nt As New NameTable
        Dim nsmgr As New XmlNamespaceManager(nt)
        Dim context As New XmlParserContext(nt, nsmgr, "", XmlSpace.None)
        Dim Archivio As String = String.Empty
        Dim Catena As String = String.Empty
        Dim Server As String = String.Empty
        Dim sXMLString As String = String.Empty

        WebUtils.ExpandOnClientNodeAttachEvent(rtvAttributes)

        sXMLString = String.Concat(Session("XMLDOC"))

        If Left(sXMLString, 6) = "Error:" Then
            Throw New DocSuiteException("BiblosDS Lettura Attributi", Mid$(sXMLString, 7), Request.Url.ToString(), DocSuiteContext.Current.User.FullUserName)
        End If

        Try
            reader = New XmlTextReader(sXMLString, XmlNodeType.Element, context)
            document.Load(reader)

            Dim o As XmlNodeList = document.GetElementsByTagName("Object")
            Dim i As Short
            Dim j As Short

            Archivio = document.GetElementsByTagName("BiblosDS").Item(0).Attributes("Database").Value()
            Server = document.GetElementsByTagName("BiblosDS").Item(0).Attributes("Server").Value()
            Catena = document.GetElementsByTagName("Chain").Item(0).Attributes("Id").Value()

            rtvAttributes.Nodes.Clear()
            Dim tn As RadTreeNode = Nothing
            Dim tnPadre As RadTreeNode = Nothing
            Dim tnPadre1 As RadTreeNode = Nothing

            'Nodo Root
            tn = New RadTreeNode
            tn.ImageUrl = "../Comm/images/server16.gif"
            tn.Text = "Server: " & Server
            tn.Expanded = True
            tn.Font.Bold = True
            rtvAttributes.Nodes.Add(tn)

            tn = New RadTreeNode
            tn.ImageUrl = "../Comm/images/BiblosDS.gif"
            tn.Text = "Archivio: " & Archivio
            tn.Expanded = True
            tn.Font.Bold = True
            rtvAttributes.Nodes.Add(tn)
            tnPadre = tn

            tn = New RadTreeNode
            tn.ImageUrl = "../Comm/images/FolderOpen16.gif"
            tn.Text = "Catena: " & Catena
            tn.Expanded = True
            tn.Font.Bold = True
            tnPadre.Nodes.Add(tn)
            tnPadre = tn
            tnPadre1 = tnPadre

            For i = 0 To o.Count - 1
                tnPadre = tnPadre1
                tn = New RadTreeNode

                tn.ImageUrl = ImagePath.FromFile("." & o.Item(i).Attributes("Type").Value())
                tn.Text = "Documento: " & o.Item(i).Attributes("Enum").Value()
                tn.Font.Bold = True
                tn.Expanded = True
                tnPadre.Nodes.Add(tn)

                tnPadre = tn

                tn = New RadTreeNode
                tn.Text = "Dimensione: " & o.Item(i).Attributes("Size").Value()
                tn.Expanded = True
                tnPadre.Nodes.Add(tn)

                tn = New RadTreeNode
                tn.Text = "Background: " & o.Item(i).Attributes("Background").Value()
                tn.Expanded = True
                tnPadre.Nodes.Add(tn)

                tn = New RadTreeNode
                tn.Text = "Visibile: " & o.Item(i).Attributes("Visible").Value()
                tn.Expanded = True
                tnPadre.Nodes.Add(tn)

                tn = New RadTreeNode
                tn.Text = "Tipo: " & o.Item(i).Attributes("Type").Value()
                tn.Expanded = True
                tnPadre.Nodes.Add(tn)

                Dim oo As XmlNodeList = o.Item(i).ChildNodes
                For j = 0 To oo.Count - 1

                    tn = New RadTreeNode
                    tn.Text = oo.Item(j).Attributes("Name").Value() & ": " & oo.Item(j).InnerText
                    tn.Expanded = True
                    tnPadre.Nodes.Add(tn)

                Next
            Next

        Catch ex As Exception
            FileLogger.Warn(LoggerName, "Errore attributi biblos", ex)
        End Try
    End Sub

End Class


