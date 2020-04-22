Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports Newtonsoft.Json

Partial Public Class UtltProtRicerca
    Inherits CommonBasePage

#Region " Fields "

    Private _finder As NHibernateProtocolFinder
    Dim protocolHeaders As IList(Of ProtocolHeader)

#End Region

#Region " Properties "

    Public ReadOnly Property Finder() As NHibernateProtocolFinder
        Get
            If _finder Is Nothing Then
                _finder = New NHibernateProtocolFinder("ProtDB")
                Return _finder
            Else
                Return _finder
            End If
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_InitComplete(ByVal sender As Object, ByVal e As EventArgs) Handles Me.InitComplete
        ChkVerificaEnabled = False
    End Sub

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()

        If Not DocSuiteContext.Current.ProtocolEnv.FascicleEnabled Then
            DG.MasterTableView.Columns.FindByUniqueNameSafe("cFascicle").Visible = False
        End If

        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Private Sub ProtocolAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        ' rieseguo la ricerca
        ProtSearch()

        Dim protocols As IList(Of Protocol) = New List(Of Protocol)
        If protocolHeaders.Count > 0 Then
            For Each protHeader As ProtocolHeader In protocolHeaders
                protocols.Add(Facade.ProtocolFacade.GetById(protHeader.Year.Value, protHeader.Number.Value))
            Next
        End If

        Dim actionArg As String = e.Argument.Substring(0, e.Argument.IndexOf("|"c))
        Dim idArg As String = e.Argument.Substring(e.Argument.IndexOf("|"c) + 1)
        Select Case actionArg
            Case "Container"
                Facade.ProtocolFacade.UpdateProtocolsForContainerOrCategory(Integer.Parse(idArg), "", protocols, actionArg)
            Case "Category"
                Facade.ProtocolFacade.UpdateProtocolsForContainerOrCategory(0, idArg, protocols, actionArg)
            Case "Autorizzazione"
                Dim roles As IList(Of Role) = JsonConvert.DeserializeObject(Of IList(Of Role))(idArg)
                Facade.ProtocolFacade.UpdateProtocolRoles(roles, protocols)
        End Select

        AjaxManager.ResponseScripts.Add("ClosePopUp();")
        AjaxManager.ResponseScripts.Add("Refresh();")
    End Sub

    Private Sub DG_ItemCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles DG.ItemCommand
        If e.CommandName.Eq("Sort") Then
            Exit Sub
        End If

        Dim type2 As String = e.CommandName.Substring(0, 4)
        Dim year As String = e.CommandName.Substring(5, 4)
        Dim number As String = e.CommandName.Substring(9)
        Dim params As String = String.Format("Year={0}&Number={1}&Type={2}", year, number, type2)
        Select Case type2
            Case "Prot"
                params &= "&StatusCancel=on"
                Response.Redirect("../Prot/ProtVisualizza.aspx?" & CommonShared.AppendSecurityCheck(params))
            Case "Fasc"
                Response.Redirect("../Fasc/FascProtocolli.aspx?" & CommonShared.AppendSecurityCheck(params))
        End Select
    End Sub

    Private Sub DG_Init(ByVal sender As Object, ByVal e As EventArgs) Handles DG.Init
        For Each column As GridColumn In DG.Columns
            Select Case column.UniqueName
                Case "Type.Description"
                    Dim col As SuggestFilteringColumn = CType(column, SuggestFilteringColumn)
                    col.DataSourceCombo = Facade.ProtocolTypeFacade.GetAllSearch()
                    col.DataTextCombo = "ShortDescription"
                    col.DataFieldCombo = "Description"
                Case "Category.Name"
                    If DocSuiteContext.Current.ProtocolEnv.CategoryView Then
                        Dim col As CompositeBoundColumn = CType(column, CompositeBoundColumn)
                        col.BindingType = CompositeBoundColumn.ColumnBinding.CustomBinding
                        col.CustomExpressionDelegate = New CompositeBoundColumn.SqlExpressionDelegate(AddressOf Facade.CategoryFacade.CategoryFullCodeFilter)
                    End If
            End Select
        Next
    End Sub

    Private Sub btnNuovaRicerca_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnNuovaRicerca.Click
        lblCounter.Text = "0"
    End Sub

    Private Sub Search_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles Search.Click
        ProtSearch()
    End Sub

    Private Sub UscClassificatore1_CategoryAdded(ByVal sender As Object, ByVal e As EventArgs) Handles UscClassificatore1.CategoryAdded
        AjaxManager.ResponseScripts.Add("VisibleCategoryChild()")
    End Sub

    Private Sub UscClassificatore1_CategoryRemoved(ByVal sender As Object, ByVal e As EventArgs) Handles UscClassificatore1.CategoryRemoved
        If Not UscClassificatore1.HasSelectedCategories Then
            chbCategoryChild.Checked = False
            AjaxManager.ResponseScripts.Add("HideCategoryChild()")
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        'Put user code to initialize the page here
        Dim locations As IList(Of Location)
        Dim containers As IList(Of ContainerRightsDto)

        locations = Facade.LocationFacade.GetAll("ProtDB")
        If locations.Count > 0 Then
            WebUtils.ObjDropDownListAdd(ddlLocation, "", "")
            For Each location As Location In locations
                WebUtils.ObjDropDownListAdd(ddlLocation, location.Name, location.Id)
            Next
        End If

        containers = Facade.ContainerFacade.GetAllRights("Prot", 1)
        If containers.Count > 0 Then
            WebUtils.ObjDropDownListAdd(ddlContainer, "", "")
            For Each container As ContainerRightsDto In containers
                WebUtils.ObjDropDownListAdd(ddlContainer, container.Name, container.ContainerId.ToString())
            Next
        End If

        RegistrationDateFrom.Focus()
    End Sub

    Private Sub InitializeAjax()
        AddHandler MasterDocSuite.AjaxManager.AjaxRequest, AddressOf ProtocolAjaxRequest

        MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(DG, DG, MasterDocSuite.AjaxDefaultLoadingPanel)
        MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(Search, DG, MasterDocSuite.AjaxDefaultLoadingPanel)
        MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(Search, lblCounter)
        MasterDocSuite.AjaxManager.AjaxSettings.AddAjaxSetting(Search, pnlsearch)
    End Sub

    Private Sub ProtSearch()
        pnlsearch.Visible = True

        With Finder
            .RegistrationDateFrom = RegistrationDateFrom.SelectedDate
            .RegistrationDateTo = RegistrationDateTo.SelectedDate
            .IdContainer = ddlContainer.SelectedValue
            .IdLocation = ddlLocation.SelectedValue
            .ProtocolObject = sObject.Text

            Dim year As Short
            If Short.TryParse(txtProtYear.Text, year) Then
                .Year = year
            End If

            Dim number As Integer
            If Integer.TryParse(txtProtNumber.Text, number) Then
                .Number = number
            End If

            .RegistrationUser = RegistrationUser.Text

            If Not String.IsNullOrEmpty(ddlStatus.SelectedValue) Then
                .IdStatus = Integer.Parse(ddlStatus.SelectedValue)
            Else
                .NoStatus = True
            End If

            If UscClassificatore1.HasSelectedCategories Then
                .Classifications = UscClassificatore1.SelectedCategories.First().FullIncrementalPath
            End If
            .IncludeChildClassifications = chbCategoryChild.Checked

            Dim chain As Integer
            If Integer.TryParse(BiblosDS.Text, chain) Then
                .IdAttachement = chain
                .IdDocument = chain
            End If

            .PageSize = DocSuiteContext.Current.ProtocolEnv.SearchMaxRecords
            .ProtocolObjectSearch = NHibernateBaseFinder(Of Protocol, ProtocolHeader).ObjectSearchType.AtLeastWord

            .SortExpressions.Add("Id", "ASC")
        End With

        ' Esegue la ricerca
        lblCounter.Text = Finder.Count.ToString()
        DG.Finder = Finder
        DG.PageSize = Finder.PageSize
        DG.DataBindFinder()

        protocolHeaders = DG.DataSource

        If protocolHeaders.IsNullOrEmpty() Then
            lblCounter.Text = "0"
            btnContenitore.Enabled = False
            btnClassificazione.Enabled = False
            btnAutorizza.Enabled = False
            pnlsearch.Visible = False
            Exit Sub
        End If

        btnContenitore.Enabled = Not String.IsNullOrEmpty(ddlContainer.SelectedValue)
        btnClassificazione.Enabled = UscClassificatore1.HasSelectedCategories
        btnAutorizza.Enabled = Not String.IsNullOrEmpty(ddlContainer.SelectedValue) AndAlso Not protocolHeaders.IsNullOrEmpty()
        If btnContenitore.Enabled Then
            Dim parameters As String = CommonShared.AppendSecurityCheck("Titolo=Utilità Protocollo&AddButton=Search&Action=Container&idContainer=" & ddlContainer.SelectedValue)
            btnContenitore.OnClientClick = String.Format("return {0}_OpenWindow('../Utlt/UtltProtCambia.aspx','windowcontenitore',500,200,'{1}');", ID, parameters)
        End If

        If btnClassificazione.Enabled Then
            Dim parameters As String = CommonShared.AppendSecurityCheck(String.Format("Titolo=Utilità Protocollo&AddButton=Search&Action=Category&idCategory={0}&idContainer={1}", UscClassificatore1.SelectedCategories.First().Id, ddlContainer.SelectedItem.Value))
            btnClassificazione.OnClientClick = String.Format("return {0}_OpenWindow('../Utlt/UtltProtCambia.aspx','windowcategorie',600,600,'{1}');", ID, parameters)
        End If

        If btnAutorizza.Enabled Then
            Dim parameters As String = CommonShared.AppendSecurityCheck(String.Format("Type={0}&Titolo=Autorizza Protocollo&Action=Container&idContainer={1}", Type, ddlContainer.SelectedValue))
            btnAutorizza.OnClientClick = String.Format("return {0}_OpenWindow('../Utlt/UtltProtAutorizza.aspx','windowautorizzazione',600,600,'{1}');", ID, parameters)
        End If
    End Sub

    Public Function SetFascicleImage(ByRef fascicles As Integer) As String
        If fascicles > 0 Then
            Return "../App_Themes/DocSuite2008/imgset16/fascicle_open.png"
        Else
            Return ImagePath.SmallEmpty
        End If
    End Function

#End Region

End Class