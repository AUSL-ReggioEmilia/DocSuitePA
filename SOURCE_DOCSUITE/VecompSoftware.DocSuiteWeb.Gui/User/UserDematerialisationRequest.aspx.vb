Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.DocumentUnits
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Workflows
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Workflow
Imports VecompSoftware.DocSuiteWeb.Gui.WebComponent.Grid
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Workflows
Imports VecompSoftware.DocSuiteWeb.DTO.Workflows
Imports Newtonsoft.Json
Imports System.Web
Imports VecompSoftware.DocSuiteWeb.Facade

Public Class UserDematerialisationRequest
    Inherits UserBasePage

#Region " Fields "
    Dim _finder As New DematerialisationFinder(New MapperDematerialisationStatementResult())
    Dim _documentUnitsIds As List(Of Guid)
#End Region

#Region " Properties "
    Public ReadOnly Property DocumentUnitIds() As String
        Get
            _documentUnitsIds = New List(Of Guid)
            For Each gridItem As GridDataItem In dgDematerialisationStatement.Items
                Dim chkSelezione As CheckBox = CType(gridItem("colClientSelect").Controls(1), CheckBox)
                If (chkSelezione Is Nothing) OrElse Not chkSelezione.Checked Then
                    Continue For
                End If
                Dim lblUDUniqueId As Label = CType(gridItem.FindControl("lblUDUniqueId"), Label)
                If lblUDUniqueId IsNot Nothing Then
                    _documentUnitsIds.Add(Guid.Parse(lblUDUniqueId.Text))
                End If
            Next
            Return JsonConvert.SerializeObject(_documentUnitsIds)
        End Get
    End Property
#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjaxSettings()
        dgDematerialisationStatement = DelegateForGrid(Of Protocol, DematerialisationStatementResult).Delegate(dgDematerialisationStatement)
        If Not IsPostBack Then
            Initialize()
            ReloadGrid()
        End If
    End Sub

    Protected Sub UserDematerialisationRequestAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Select Case e.Argument
            Case "ReloadPage"
                ReloadGrid()
        End Select
    End Sub

#End Region

#Region " Methods "
    Public Sub Initialize()

        rdpDateFrom.SelectedDate = DateTime.Today.AddDays(-30)
        rdpDateTo.SelectedDate = DateTime.Today
    End Sub

    Private Sub InitializeAjaxSettings()
        AddHandler AjaxManager.AjaxRequest, AddressOf UserDematerialisationRequestAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(dgDematerialisationStatement, dgDematerialisationStatement, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnUpdate, dgDematerialisationStatement, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, dgDematerialisationStatement, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub
    Private Sub SetFinder()
        _finder.EnablePaging = True
        _finder.PageSize = 30
        _finder.LogType = "SB"
        _finder.UserName = DocSuiteContext.Current.User.FullUserName
        _finder.DateFrom = rdpDateFrom.SelectedDate.Value
        _finder.DateTo = rdpDateTo.SelectedDate.Value.AddDays(1)
        _finder.Count()

        ' Associo il finder alla griglia
        dgDematerialisationStatement.Finder = _finder
    End Sub
#End Region

#Region " Events "
    Private Sub dgDematerialisationStatement_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles dgDematerialisationStatement.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If
        Dim row As DematerialisationStatementResult = DirectCast(e.Item.DataItem, DematerialisationStatementResult)
        If (row IsNot Nothing) Then
            With DirectCast(e.Item.FindControl("lbtViewUD"), LinkButton)
                .Text = String.Format("{0}/{1:0000000}", row.Year, row.Number)
            End With

            With DirectCast(e.Item.FindControl("lblViewUDName"), Label)
                .Text = "Protocollo"
            End With

            With DirectCast(e.Item.FindControl("lblUDUniqueId"), Label)
                .Text = row.UDId.ToString()
            End With
        End If
    End Sub

    Public Sub btnUpdate_OnClientClick(sender As Object, e As EventArgs) Handles btnUpdate.Click
        ReloadGrid()
    End Sub

    Public Sub btnStatement_OnClientClick(sender As Object, e As EventArgs) Handles btnStatement.Click
        Dim ids As List(Of Guid) = JsonConvert.DeserializeObject(Of List(Of Guid))(DocumentUnitIds)
        If ids Is Nothing OrElse ids.Count < 1 Then
            AjaxAlert("Selezionare almeno un documento")
            Exit Sub
        End If
        AjaxManager.ResponseScripts.Add(String.Format("return {0}_OpenWindowRequestStatement('{1}');", ClientID, DocumentUnitIds))
    End Sub

    Public Sub ReloadGrid()
        SetFinder()
        dgDematerialisationStatement.CurrentPageIndex = 0
        dgDematerialisationStatement.CustomPageIndex = 0
        dgDematerialisationStatement.PageSize = dgDematerialisationStatement.Finder.PageSize
        dgDematerialisationStatement.DataBindFinder(Of Protocol, DematerialisationStatementResult)()
    End Sub
#End Region

End Class
