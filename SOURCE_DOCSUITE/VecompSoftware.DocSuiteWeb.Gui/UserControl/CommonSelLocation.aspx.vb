Imports System.Collections.Generic
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class CommonSelLocation
    Inherits CommBasePage

#Region " Fields "
    Private _locations As IList(Of Location)

#End Region

#Region "Properties"

    Private ReadOnly Property DatabaseName() As String
        Get
            Select Case Type
                Case "Docm"
                    Return "DocmDB"
                Case "Prot", "ProtAttach", "Comm"
                    Return "ProtDB"
                Case "Resl"
                    Return "ReslDB"
            End Select
            Return String.Empty
        End Get
    End Property

    Private ReadOnly Property Locations() As IList(Of Location)
        Get
            If _locations Is Nothing Then
                _locations = Facade.LocationFacade.GetAll(DatabaseName)
            End If
            Return _locations
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False
        If Not Me.IsPostBack Then
            Initialize()
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        Dim node As RadTreeNode
        Dim biblosDSDB As String = String.Empty
        For Each location As Location In Locations
            node = New RadTreeNode()
            Select Case Type
                Case "Docm"
                    biblosDSDB = location.DocmBiblosDSDB
                Case "Prot"
                    biblosDSDB = location.ProtBiblosDSDB
                Case "Resl"
                    biblosDSDB = location.ReslBiblosDSDB
            End Select
            node.Text = $"{location.Id} {location.Name} ({biblosDSDB})"
            node.Value = location.Id.ToString()
            node.Attributes.Add("Description", $"{location.Id} {location.Name}")
            node.ImageUrl = "../Comm/images/BiblosDS.gif"
            RadTreeViewLocation.Nodes(0).Nodes.Add(node)
        Next
    End Sub

#End Region

End Class