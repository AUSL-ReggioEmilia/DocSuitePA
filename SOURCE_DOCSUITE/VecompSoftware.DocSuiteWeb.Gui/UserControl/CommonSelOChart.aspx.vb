Imports System.Collections.Generic
Imports System.Drawing
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.ExtensionMethods
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Model.Parameters
Imports System.Web
Imports Newtonsoft.Json

Partial Public Class CommonSelOChart
    Inherits CommBasePage

#Region " Fields "

    Private _currentItemSelectedId As Guid = Guid.Empty
#End Region

#Region " Properties "


    Protected ReadOnly Property OChartItemSelectedId As Guid
        Get
            If _currentItemSelectedId = Guid.Empty Then
                _currentItemSelectedId = Request.QueryString.GetValue(Of Guid)("OChartItemSelectedId")
            End If
            Return _currentItemSelectedId
        End Get
    End Property

    Private Property _currentOChart As OChart = Nothing
    Private Property _currentOChartItem As OChartItem = Nothing

    Private Property _currentOChartId As Guid? = Nothing
    Private Property _currentOChartItemId As Guid? = Nothing


    Private Property SessionOChart() As OChart
        Get
            Return DirectCast(Session("selectedOChart"), OChart)
        End Get
        Set(value As OChart)
            Session("selectedOChart") = value
        End Set
    End Property

    Private Property SelectedOChart() As OChart
        Get
            If (Not _currentOChartId.HasValue OrElse _currentOChart Is Nothing OrElse (_currentOChartId.HasValue AndAlso _currentOChartId.Value <> SelectedOChartId)) Then
                _currentOChartId = SelectedOChartId
                _currentOChart = Facade.OChartFacade.GetHierarchy(SelectedOChartId)
            End If
            Return _currentOChart
        End Get
        Set(value As OChart)
            If value IsNot Nothing Then
                SessionOChart = value
                ddlOCharts.SelectedValue = value.Id.ToString()
                DdlOChartsSelectedIndexChanged(ddlOCharts, New EventArgs())
            Else
                SessionOChart = Nothing
            End If
        End Set
    End Property

    Private ReadOnly Property SelectedOChartId() As Guid
        Get
            If String.IsNullOrEmpty(ddlOCharts.SelectedValue) Then
                Return Nothing
            End If

            Dim temp As Guid
            If Not Guid.TryParse(ddlOCharts.SelectedValue, temp) Then
                Throw New Exception("Nessun organigramma selezionato")
            End If
            Return temp
        End Get
    End Property

    Private Property SelectedOChartItem As OChartItem
        Get
            If String.IsNullOrEmpty(OChartTree.SelectedValue) Then
                Return Nothing
            End If
            Dim temp As Guid
            If Not Guid.TryParse(OChartTree.SelectedValue, temp) Then
                Return Nothing
            End If

            If (Not _currentOChartItemId.HasValue OrElse _currentOChartItem Is Nothing OrElse (_currentOChartItemId.HasValue AndAlso _currentOChartItemId.Value <> temp)) Then
                _currentOChartItemId = temp
                _currentOChartItem = Facade.OChartItemFacade.GetById(_currentOChartItemId.Value, False)
            End If

            Return _currentOChartItem
        End Get
        Set(value As OChartItem)
            If value Is Nothing Then
                If OChartTree.SelectedNode IsNot Nothing Then
                    OChartTree.SelectedNode.Selected = False
                End If
                Return
            End If
            ' Imposto valore selezionato
            Dim node As RadTreeNode = OChartTree.FindNodeByValue(value.Id.ToString())
            If (node IsNot Nothing) Then
                node.Selected = False
            End If
        End Set
    End Property
#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False
        InitializeAjaxSettings()
        OChartTree.OnClientNodeClicked = "ReturnValueOnClick"
        If Not IsPostBack Then
            DataBindOCharts()
            SelectedOChart = Facade.OChartFacade.GetEffective()
        End If
    End Sub

    Private Sub OChartTreeNodeExpand(sender As Object, e As RadTreeNodeEventArgs) Handles OChartTree.NodeExpand
        Dim guidItem As Guid = Guid.Parse(e.Node.Value)
        Dim item As OChartItem = Facade.OChartItemFacade.GetById(guidItem)
        e.Node.Expanded = True
        e.Node.Nodes.Clear()
        If item.HasItems Then
            Dim filledNode As New RadTreeNode
            For Each child As OChartItem In item.Items.Where(Function(f) f.Id <> OChartItemSelectedId).OrderBy(Function(x) x.Title).ToList()
                FillNodeFromOChartItem(filledNode, child)
                e.Node.Nodes.Add(filledNode)
            Next
            ' Imposto Expand su Client: i nodi sono già caricati, evito il passaggio sul Server.
            e.Node.ExpandMode = TreeNodeExpandMode.ClientSide
        End If

    End Sub

#End Region

#Region " Methods "
    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(OChartTree, OChartTree)
    End Sub
    Private Sub OChartTreeNodeDataBound(sender As Object, e As RadTreeNodeEventArgs) Handles OChartTree.NodeDataBound
        Dim item As OChartItem = CType(e.Node.DataItem, OChartItem)
        FillNodeFromOChartItem(e.Node, item)
    End Sub

    Private Sub DataBindOCharts()
        ddlOCharts.Items.Clear()
        Dim item As New ListItem()
        For Each chart As OChart In Facade.OChartFacade.GetEnabled().OrderChronologically()
            item.Text = String.Format("[{0:dd/MM/yyyy}] {1}", chart.StartDateOrDefault, chart.Title)
            item.Value = chart.Id.ToString()
            ddlOCharts.Items.Add(item)
        Next
    End Sub

    Private Sub DdlOChartsSelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlOCharts.SelectedIndexChanged
        ' Visualizzo i dettagli
        lblHeadName.Text = SelectedOChart.Title
        lblHeadDescription.Text = SelectedOChart.Description
        ' Stato
        Select Case True
            Case SelectedOChart.Enabled.GetValueOrDefault(False) = False
                lblHeadStatus.Text = "NON ATTIVO"
            Case SelectedOChart.IsEffective
                lblHeadStatus.Text = "Effettivo"
            Case SelectedOChart.IsEnded
                lblHeadStatus.Text = "Chiuso"
            Case Else
                lblHeadStatus.Text = "Attivo"
        End Select

        If SelectedOChart.StartDate.HasValue Then
            lblHeadDateFrom.Text = SelectedOChart.StartDate.Value.ToString("dd/MM/yyyy")
        End If
        lblHeadDateTo.Text = String.Empty
        If SelectedOChart.EndDate.HasValue Then
            lblHeadDateTo.Text = SelectedOChart.EndDate.Value.ToString("dd/MM/yyyy")
        End If
        SelectedOChartItem = Nothing
        OChartTree.Nodes.Clear()
        If SelectedOChart.HasItems Then
            Dim sortedRoots As IEnumerable(Of OChartItem) = SelectedOChart.Roots.Where(Function(f) f.Id <> OChartItemSelectedId).OrderBy(Function(x) x.Title)
            OChartTree.DataSource = sortedRoots
            OChartTree.DataBind()

            SelectedOChartItem = sortedRoots.FirstOrDefault()
        End If
    End Sub

    Private Shared Sub FillNodeFromOChartItem(ByRef node As RadTreeNode, item As OChartItem)
        node.Text = String.Format("{0} [{1}]", item.Title, item.Acronym)
        node.Value = item.Id.ToString()
        node.ExpandMode = CType(If(item.HasItems, TreeNodeExpandMode.ServerSide, TreeNodeExpandMode.ClientSide), TreeNodeExpandMode)
        node.ImageUrl = CType(If(item.IsRoot, ImagePath.SmallNetworkShareStar, ImagePath.SmallNetworkShare), String)
    End Sub

#End Region

End Class