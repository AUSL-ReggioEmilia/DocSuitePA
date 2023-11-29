Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports Telerik.Web.UI
Imports System.Collections.Generic
Imports System.Linq

Public Class UserAuthorizedDocuments
    Inherits UserBasePage

#Region " Fields "

    Dim _pageTitle As String
    Dim _protocolFinder As NHibernateProtocolFinder

#End Region

#Region " Properties "

    Private ReadOnly Property PageTitle As String
        Get
            If String.IsNullOrEmpty(_pageTitle) Then
                _pageTitle = Request.QueryString("Title")
            End If
            Return _pageTitle
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjaxSettings()

        If Action.Equals("PANA") Then
            uscProtocolGrid.ColumnAcceptanceRolesVisible = True
        End If

        If Not IsPostBack Then
            Initialize()
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        LoadProtocols()
    End Sub


    Private Function GetChecked(item As GridDataItem) As Boolean
        Dim chk As CheckBox = CType(item("colClientSelect").Controls(1), CheckBox)
        Return chk IsNot Nothing AndAlso chk.Checked
    End Function

    Private Function GetProtocolKey(item As GridDataItem) As YearNumberCompositeKey
        Dim lbtViewProtocol As LinkButton = CType(item.FindControl("lbtViewProtocol"), LinkButton)
        If lbtViewProtocol Is Nothing Then
            Return Nothing
        End If

        Dim splitted As String() = lbtViewProtocol.CommandArgument.Split("|"c)
        Dim year As Short = CShort(splitted(0))
        Dim number As Integer = CInt(splitted(1))
        Return New YearNumberCompositeKey(year, number)
    End Function


#End Region

#Region " Methods "

    Private Sub Initialize()
        Title = String.Format("Protocollo - {0}", PageTitle)
        rdpDateFrom.SelectedDate = Date.Today.AddDays(-ProtocolEnv.DesktopDayDiff).Date
        rdpDateTo.SelectedDate = Date.Today
        If Not Action.Eq("PDA") Then
            LoadContainers()
        End If
        LoadProtocols()
    End Sub

    Private Sub LoadContainers()
        rowContainer.Visible = True
        ddlContainer.Items.Clear()
        ddlContainer.Items.Add(New DropDownListItem(String.Empty, String.Empty))
        Dim containers As IList(Of Container) = Facade.ContainerFacade.GetContainers(DocSuiteContext.Current.CurrentDomainName, DocSuiteContext.Current.User.UserName, DSWEnvironment.Protocol, ProtocolContainerRightPositions.Insert, Nothing)
        For Each container As Container In containers
            ddlContainer.Items.Add(New DropDownListItem(container.Name, container.Id.ToString()))
        Next

    End Sub

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearch, uscProtocolGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscProtocolGrid.Grid, uscProtocolGrid.Grid, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Sub LoadProtocols()
        _protocolFinder = Facade.ProtocolFinder
        _protocolFinder.RegistrationDateFrom = rdpDateFrom.SelectedDate.Value
        _protocolFinder.RegistrationDateTo = rdpDateTo.SelectedDate.Value

        Dim commonUtil As CommonUtil = New CommonUtil()
        commonUtil.ApplyProtocolFinderSecurity(_protocolFinder, SecurityType.Read, CurrentTenant.TenantAOO.UniqueId)

        If Not String.IsNullOrEmpty(ddlContainer.SelectedValue) Then
            _protocolFinder.IdContainer = ddlContainer.SelectedValue
        End If

        Select Case Me.Action
            Case "PDA"
                _protocolFinder.RestrictionOnlyRoles = True
                _protocolFinder.OnlyExplicitRoles = True
                _protocolFinder.ProtocolRoleStatus = ProtocolRoleStatus.ToEvaluate
            Case "PANA"
                _protocolFinder.RestrictionOnlyRoles = False
                _protocolFinder.OnlyExplicitRoles = False
                _protocolFinder.ProtocolRoleStatus = ProtocolRoleStatus.ToEvaluate
                _protocolFinder.IsInRefusedProtocolRoleGroup = True
            Case "PRS"
                _protocolFinder.RestrictionOnlyRoles = False
                _protocolFinder.OnlyExplicitRoles = False
                _protocolFinder.ProtocolRoleStatus = ProtocolRoleStatus.Refused
                _protocolFinder.IsInRefusedProtocolRoleGroup = True
        End Select

        uscProtocolGrid.Grid.Finder = _protocolFinder
        uscProtocolGrid.Grid.DataBindFinder()
        Title = String.Format("{0} ({1})", Title, uscProtocolGrid.Grid.DataSource.Count)

    End Sub


#End Region

End Class
