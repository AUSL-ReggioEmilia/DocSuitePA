Imports VecompSoftware.DocSuiteWeb.Gui.WebComponent.Grid
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder.Desks
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Desks
Imports VecompSoftware.DocSuiteWeb.DTO.Desks
Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.ExtensionMethods.EnumEx
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder
Imports VecompSoftware.DocSuiteWeb.EntityMapper.Desks


Public Class DeskRisultati
    Inherits DeskBasePage
    Dim _finder As New DeskFinder(New MapperDeskResult(), DocSuiteContext.Current.User.FullUserName)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        dgDesk = DelegateForGrid(Of Desk, DeskResult).Delegate(dgDesk)

        If Not IsPostBack Then
            InitializeFilters()
            SearchContainer()
            SetFinder()
            dgDesk.CurrentPageIndex = 0
            dgDesk.CustomPageIndex = 0
            dgDesk.PageSize = dgDesk.Finder.PageSize

            dgDesk.DataBindFinder(Of Desk, DeskResult)()
        End If
    End Sub

#Region " Fields"
    Private Const DESK_SUMMARY_PATH As String = "~/Desks/DeskSummary.aspx?Type=Desk&Action=View&DeskId={0}"
#End Region

#Region " Events"

    ''' <summary>
    ''' Decoro i dati nella griglia
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub dgDesk_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles dgDesk.ItemDataBound
        If Not e.Item.ItemType = GridItemType.Item AndAlso Not e.Item.ItemType = GridItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim row As DeskResult = DirectCast(e.Item.DataItem, DeskResult)
        Dim imageStatus As Image = DirectCast(e.Item.FindControl("imgDeskStatus"), Image)
        '' Status
        If row.DeskState.HasValue Then
            Select Case row.DeskState.Value
                Case DeskState.Approve
                    imageStatus.ImageUrl = Page.ResolveUrl("~/App_Themes/DocSuite2008/images/desk/desk_approving.png")
                Case DeskState.Closed
                    imageStatus.ImageUrl = Page.ResolveUrl("~/App_Themes/DocSuite2008/images/desk/desk_closed.png")
                Case DeskState.Open
                    imageStatus.ImageUrl = Page.ResolveUrl("~/App_Themes/DocSuite2008/images/desk/desk_open.png")
            End Select
            imageStatus.ToolTip = String.Format("Il tavolo è {0}", row.DeskState.Value.GetDescription())
        End If

        '' Summary
        With DirectCast(e.Item.FindControl("linkSummary"), HyperLink)
            .NavigateUrl = String.Format(DESK_SUMMARY_PATH, row.DeskId)
            .Text = row.DeskName
        End With
    End Sub

    Private Sub btnClearFiltersClick(sender As Object, e As EventArgs) Handles btnClearFilters.Click
        Try
            InitializeFilters()
            SetFinder()
            dgDesk.DataBindFinder(Of Desk, DeskResult)()
        Catch ex As Exception
            AjaxAlert(ex)
        End Try

    End Sub
    ''' <summary>
    ''' Esegue la ricerca all'interno di tavoli
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click
        If Not Page.IsValid Then
            Return
        End If
        Try
            SetFinder()
            dgDesk.DataBindFinder(Of Desk, DeskResult)()
        Catch ex As DocSuiteException
            AjaxAlert(ex)
        End Try
    End Sub
#End Region

#Region " Methods "
    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(dgDesk, dgDesk, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, dgDesk, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnClearFilters, dgDesk, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, searchTable, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    ''' <summary>
    ''' Imposto i valori di default dei filtri
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeFilters()
        rdbShowRecorded.SelectedIndex = 0
        rcbContainer.SelectedIndex = 0
        txtDeskName.Text = String.Empty
        txtDescription.Text = String.Empty
        chbDeskNotExpired.Checked = False
    End Sub

    Protected Overridable Sub SearchContainer()
        Dim containers As IList(Of Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.Desk, DeskRightPositions.Insert, True)

        rcbContainer.Items.Clear()
        rcbContainer.Items.Add(New DropDownListItem("Visualizza Tutti", "-100000"))
        If containers IsNot Nothing Then
            Dim coll As SortedDictionary(Of String, DropDownListItem) = New SortedDictionary(Of String, DropDownListItem)
            For Each container As Container In containers
                coll.Add(container.Name, New DropDownListItem(container.Name, container.Id.ToString()))
            Next
            For Each dropDownListItem As DropDownListItem In coll.Values
                rcbContainer.Items.Add(dropDownListItem)
            Next

        End If
        rcbContainer.DataBind()
    End Sub

    ''' <summary>
    ''' Imposto i filtri per il finder
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetFinder()
        _finder.EnablePaging = True
        _finder.PageSize = 30
        _finder.SortExpressions.Add(New SortExpression(Of Desk)() With {.Direction = SortDirection.Descending, .Expression = Function(x) x.RegistrationDate})
        _finder.UserContainers = Facade.ContainerFacade.GetContainers(Data.DSWEnvironment.Desk, DeskRightPositions.Read, True)

        If (Not rcbContainer.SelectedValue.Eq("-100000")) Then
            _finder.DeskContainerId = rcbContainer.SelectedValue
        End If

        If Not String.IsNullOrEmpty(txtDeskName.Text) Then
            _finder.DeskName = txtDeskName.Text
        End If

        If Not String.IsNullOrEmpty(txtDescription.Text) Then
            _finder.DeskDescription = txtDescription.Text
        End If

        If chbDeskNotExpired.Checked Then
            _finder.DeskNotExprired = True
        End If

        If Not String.IsNullOrEmpty(CurrentFinderQueryString) AndAlso "My".Eq(CurrentFinderQueryString) Then
            _finder.ExplicitPermission = True
        End If
        tblFilterState.Visible = True
        If Not String.IsNullOrEmpty(CurrentFinderQueryString) AndAlso "OpenDesk".Eq(CurrentFinderQueryString) Then
            tblFilterState.Visible = False
            _finder.IsOpen = True
        End If

        If rdbShowRecorded.SelectedItem IsNot Nothing AndAlso Not rdbShowRecorded.SelectedValue.Eq("0") Then
            _finder.DeskStates = New List(Of DeskState)()
            _finder.DeskStates.Add(DirectCast(Int32.Parse(rdbShowRecorded.SelectedValue), DeskState))
        End If


        ' Associo il finder alla griglia
        dgDesk.Finder = _finder
    End Sub

#End Region
End Class