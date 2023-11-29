Imports System.Linq
Imports System.Collections.Generic
Imports System.Text
Imports VecompSoftware.Helpers.ExtensionMethods
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade

Public Class TbltOChart
    Inherits CommonBasePage

#Region " Constants "
    Private Const SharedGroupName As String = "Condivise"
#End Region

#Region " Properties "
    Private Property _currentOChart As OChart = Nothing
    Private Property _currentOChartItem As OChartItem = Nothing
    Private Property _currentOChartId As Guid? = Nothing
    Private Property _currentOChartItemId As Guid? = Nothing
    Private Property DisabledRiseContactControl As Boolean


    Private Property SessionOChart() As OChart
        Get
            Return DirectCast(Session("selectedOChart"), OChart)
        End Get
        Set(value As OChart)
            Session("selectedOChart") = value
        End Set
    End Property

    Private ReadOnly Property AvailableContainers() As IList(Of Container)
        Get
            If Session("TbltOChart.AvailableContainers") Is Nothing Then
                Dim right As Integer?
                Session("TbltOChart.AvailableContainers") = Facade.ContainerFacade.GetContainers(DSWEnvironment.Protocol, right, True)

            End If
            Return DirectCast(Session("TbltOChart.AvailableContainers"), IList(Of Container))

        End Get
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
                Throw New Exception("Nessun OChart selezionato")
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
                ClearOChartItemDetails()
                If OChartTree.SelectedNode IsNot Nothing Then
                    OChartTree.SelectedNode.Selected = False
                End If
                Return
            End If
            ' Imposto valore selezionato
            Dim node As RadTreeNode = OChartTree.FindNodeByValue(value.Id.ToString())
            If (node IsNot Nothing) Then
                node.Selected = True
                ' DataBind dei dettgli
                DataBindOChartItemDetails(value)
            End If
        End Set
    End Property

    Public ReadOnly Property MoveOChart As RadToolBarButton
        Get
            Dim toolBarItem As RadToolBarItem = tbItem.FindItemByValue("moveChart")
            Return DirectCast(toolBarItem, RadToolBarButton)
        End Get
    End Property

#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not CommonShared.HasGroupOChartAdminRight Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

        MasterDocSuite.TitleVisible = False
        InitializeAjax()

        tbItem.OnClientButtonClicking = String.Format("{0}_tbItemButtonClicking", ID)
        If Not IsPostBack Then
            DirectCast(tbContainersControl.FindButtonByCommandName("REMOVE"), RadToolBarButton).ImageUrl = ImagePath.SmallRemove
            DirectCast(tbContainersControl.FindButtonByCommandName("REJECTION"), RadToolBarButton).ImageUrl = ImagePath.SmallReject
            MoveOChart.Visible = DocSuiteContext.Current.ProtocolEnv.ProtocolRejectionEnabled
            DataBindOCharts()
            SelectedOChart = Facade.OChartFacade.GetEffective()
            Dim oChartFollowing As OChart = Facade.OChartFacade.GetFollowingOrDefault(SelectedOChart)
            DirectCast(tbItem.FindButtonByCommandName("MOVECHART"), RadToolBarButton).Visible = SelectedOChart.IsEffective AndAlso oChartFollowing Is Nothing
        End If
    End Sub

    Private Sub OChartAjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)

        Dim args() As String = e.Argument.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)

        If args.Length <> 3 Then
            Throw New Exception("AjaxRequest non valida: " + e.Argument)
        End If

        Dim senderName As String = args(0)
        Dim actionName As String = args(1)
        Dim argValue As String = args(2)

        Select Case senderName
            Case "ROLES"
                Select Case actionName
                    Case "ADD"
                        Dim roles As IList(Of Role) = New List(Of Role)
                        For Each arg As String In argValue.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                            Dim role As Role = Facade.RoleFacade.GetById(Integer.Parse(arg))
                            roles.Add(role)
                        Next
                        myRolecontrol.AddItems(roles, Function(r) Not r.IsActive OrElse Not r.IsActiveRange())
                    Case "NEW"
                        Dim alreadyExists As Boolean = Facade.RoleFacade.AlreadyExists(argValue, CurrentTenant.TenantAOO.UniqueId)
                        If alreadyExists Then
                            Me.AjaxAlert("Settore già esistente con questo nome.")
                            Return
                        End If

                        Dim role As New Role()
                        role.Name = argValue
                        role.IdTenantAOO = CurrentTenant.TenantAOO.UniqueId
                        Facade.RoleFacade.Save(role)
                        myRolecontrol.AddItem(role, Not role.IsActive OrElse Not role.IsActiveRange())
                End Select
            Case "CONTAINERS"
                Select Case actionName
                    Case "NEW"
                        Dim alreadyExists As Boolean = Facade.ContainerFacade.AlreadyExists(argValue)
                        If alreadyExists Then
                            Me.AjaxAlert("Contenitore già esistente con questo nome.")
                            Return
                        End If

                        Dim cont As New Container()
                        cont.Name = argValue
                        Facade.ContainerFacade.Save(cont)
                        myContainersControl.AddItem(cont, Not cont.IsActive)
                End Select
            Case "CONTACTS"
                For Each arg As String In argValue.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    Dim contact As Contact = Facade.ContactFacade.GetById(Integer.Parse(arg))
                    myContactControl.AddItem(contact, Not contact.IsActive OrElse Not contact.IsActiveRange())
                Next
            Case "NODES"
                Select Case actionName
                    Case "MOVE"
                        Dim idNodeTo As Guid
                        If Not argValue.Eq("NULL") Then
                            For Each arg As String In argValue.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                idNodeTo = Guid.Parse(arg)
                            Next
                            MoveOChartItems(idNodeTo)
                            Refresh()
                        End If

                End Select
        End Select


    End Sub

    Private Sub CbContainerItemsRequested(sender As Object, e As RadComboBoxItemsRequestedEventArgs) Handles cbContainer.ItemsRequested
        'Dim right As Integer?
        'Dim availableContainers As IList(Of Container) = Facade.ContainerFacade.GetContainers(DSWEnvironment.Protocol, right, True)
        Dim items As IList(Of Container) = AvailableContainers
        If Not String.IsNullOrEmpty(e.Text) Then
            items = Facade.ContainerFacade.FilterContainers(AvailableContainers, e.Text)
        End If
        cbContainer.Items.Clear()
        For Each container As Container In items
            cbContainer.Items.Add(New RadComboBoxItem(container.Name, container.Id.ToString()))
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
            Dim sortedRoots As IEnumerable(Of OChartItem) = SelectedOChart.Roots.OrderBy(Function(x) x.Title)
            OChartTree.DataSource = sortedRoots
            OChartTree.DataBind()

            SelectedOChartItem = sortedRoots.FirstOrDefault()
        End If
        tbItem.Enabled = Not SelectedOChart.IsEnded
        tbContactControl.Enabled = Not SelectedOChart.IsEnded
        tbContainersControl.Enabled = Not SelectedOChart.IsEnded
        tbRoleControl.Enabled = Not SelectedOChart.IsEnded
    End Sub

    Private Sub OChartTreeNodeClick(sender As Object, e As RadTreeNodeEventArgs) Handles OChartTree.NodeClick
        DataBindOChartItemDetails(SelectedOChartItem)
    End Sub

    Private Sub OChartTreeNodeDataBound(sender As Object, e As RadTreeNodeEventArgs) Handles OChartTree.NodeDataBound
        Dim item As OChartItem = CType(e.Node.DataItem, OChartItem)
        FillNodeFromOChartItem(e.Node, item)
    End Sub

    Private Sub OChartTreeNodeExpand(sender As Object, e As RadTreeNodeEventArgs) Handles OChartTree.NodeExpand
        Dim guidItem As Guid = Guid.Parse(e.Node.Value)
        Dim item As OChartItem = Facade.OChartItemFacade.GetById(guidItem)
        e.Node.Expanded = True
        e.Node.Nodes.Clear()
        If (item.HasItems) Then
            For Each child As OChartItem In item.Items.OrderBy(Function(x) x.Title).ToList()
                Dim filledNode As New RadTreeNode
                FillNodeFromOChartItem(filledNode, child)
                e.Node.Nodes.Add(filledNode)
            Next
            ' Imposto Expand su Client: i nodi sono già caricati, evito il passaggio sul Server.
            e.Node.ExpandMode = TreeNodeExpandMode.ClientSide
        End If

    End Sub

    Private Sub TbItemButtonClick(sender As Object, e As RadToolBarEventArgs) Handles tbItem.ButtonClick
        Dim sourceControl As RadToolBarButton = DirectCast(e.Item, RadToolBarButton)

        ' Verifico requisiti
        Select Case sourceControl.CommandName
            Case "ADDCHILD", "EDIT", "REMOVE"
                If SelectedOChartItem Is Nothing Then
                    AjaxAlert("Selezionare il nodo padre.")
                    Return
                End If
        End Select

        ' Reset della finestra interessata
        Select Case sourceControl.CommandName
            Case "ADDROOT", "ADDCHILD"
                ResetItemDetailWindow(False)
            Case "EDIT"
                ResetItemDetailWindow(SelectedOChartItem.IsImported)
            Case "ADDCHART"
                dpChartDetailDateFrom.MinDate = DateTime.Today
                dpChartDetailDateFrom.Enabled = True
                cmdChartDetailAdd.CommandArgument = "SAVE"
                txtChartDetailTitle.Text = String.Empty
                txtChartDetailDescription.Text = String.Empty
                dpChartDetailDateFrom.SelectedDate = Nothing
                dpChartDetailDateTo.SelectedDate = Nothing
        End Select

        Dim windowTitle As String
        Dim windowName As String
        ' Imposto i valori della finestra
        Select Case sourceControl.CommandName
            Case "ADDROOT"
                lblItemDetailSource.Text = "NODO RADICE"
                cmdItemDetailAdd.Visible = True
                cmdItemDetailAdd.CommandArgument = "ROOT"
                windowTitle = "Aggiungi Unità Organizzativa radice"
                windowName = rwItemDetail.ClientID
            Case "ADDCHILD"
                lblItemDetailSource.Text = String.Format("{0} [{1}]", SelectedOChartItem.Title, SelectedOChartItem.Acronym)
                cmdItemDetailAdd.Visible = True
                windowTitle = "Aggiungi Unità Organizzativa"
                windowName = rwItemDetail.ClientID
            Case "EDIT"
                If SelectedOChartItem.Parent Is Nothing Then
                    lblItemDetailSource.Text = "NODO RADICE"
                Else
                    lblItemDetailSource.Text = String.Format("{0} [{1}]", SelectedOChartItem.Parent.Title, SelectedOChartItem.Parent.Acronym)
                End If
                txtItemDetailTitle.Text = SelectedOChartItem.Title
                txtItemDetailDescription.Text = SelectedOChartItem.Description
                txtItemDetailAcronym.Text = SelectedOChartItem.Acronym
                txtItemDetailCode.Text = SelectedOChartItem.Code
                cbItemDetailEnabled.Checked = SelectedOChartItem.Enabled.GetValueOrDefault(False)
                cmdItemDetailEditOk.Visible = True
                windowTitle = "Modifica Unità Organizzativa"
                windowName = rwItemDetail.ClientID
            Case "REMOVE"
                lblItemRemoveConfirmTitle.Text = SelectedOChartItem.Title
                lblItemRemoveConfirmCode.Text = SelectedOChartItem.Code
                If SelectedOChartItem.Parent IsNot Nothing Then
                    lblItemRemoveConfirmParentCode.Text = SelectedOChartItem.Parent.Code
                Else
                    lblItemRemoveConfirmParentCode.Text = "-"
                End If
                windowTitle = "Conferma eliminazione Unità Organizzativa"
                windowName = rwItemRemoveConfirm.ClientID
            Case "ADDCHART"
                windowTitle = "Aggiungi nuovo Organigramma"
                'focusItem = txtChartDetailTitle.ClientID
                windowName = rwChartDetail.ClientID
            Case "EDITCHART"
                dpChartDetailDateFrom.MinDate = Nothing
                dpChartDetailDateFrom.Enabled = False
                cmdChartDetailAdd.CommandArgument = "EDIT"
                windowTitle = "Modifica Organigramma"
                txtChartDetailTitle.Text = SelectedOChart.Title
                txtChartDetailDescription.Text = SelectedOChart.Description
                dpChartDetailDateFrom.SelectedDate = SelectedOChart.StartDate
                dpChartDetailDateTo.SelectedDate = SelectedOChart.EndDate
                windowName = rwChartDetail.ClientID
            Case "REMOVECHART"
                If (SelectedOChart.IsEffective) Then
                    AjaxAlert("Impossibile eliminare l'organigramma effettivo")
                    Return
                End If
                If (SelectedOChart.IsEnded) Then
                    AjaxAlert("Impossibile eliminare gli organigrammi chiusi")
                    Return
                End If
                lblChartRemoveTitle.Text = SelectedOChart.Title
                lblChartRemoveDate.Text = SelectedOChart.StartDate.GetValueOrDefault().ToString("dd/MM/yyyy")
                windowTitle = "Conferma eliminazione Organigramma"
                windowName = rwChartRemove.ClientID
        End Select

        Dim script As String = String.Format(String.Format("OpenWindow('{0}', '{1}');", windowName, windowTitle))
        AjaxManager.ResponseScripts.Add(script)
    End Sub
    Private Sub TbRoleControlButtonClick(sender As Object, e As RadToolBarEventArgs) Handles tbRoleControl.ButtonClick
        Dim sourceControl As RadToolBarButton = DirectCast(e.Item, RadToolBarButton)

        Select Case sourceControl.CommandName
            Case "REMOVE"
                Dim role As Role = myRolecontrol.GetSelectedItem()
                If myRolecontrol.IsExplicit(role) Then
                    myRolecontrol.RemoveItem(role)
                End If
        End Select
    End Sub

    Private Sub TbContainersControlButtonClick(sender As Object, e As RadToolBarEventArgs) Handles tbContainersControl.ButtonClick
        Dim container As Container = myContainersControl.GetSelectedItem()
        If container Is Nothing Then
            AjaxAlert("Selezionare un Contenitore.")
            Return
        End If

        Dim senderButton As RadToolBarButton = DirectCast(e.Item, RadToolBarButton)
        Select Case senderButton.CommandName
            Case "REMOVE"
                myContainersControl.RemoveItem(container)

            Case "MASTER"
                Dim attr As String = myContainersControl.SelectedNode.Attributes("IsMaster")
                If String.IsNullOrEmpty(attr) Then
                    SessionOChart = Facade.OChartItemContainerFacade.MasterTrasformer(SelectedOChartItem, container)
                    UpdateContainers()
                End If

            Case "REJECTION"
                Dim itemContainer As OChartItemContainer = SelectedOChartItem.Containers.First(Function(c) c.Container.Id = container.Id)
                If itemContainer.IsRejection AndAlso itemContainer.Container.Id.Equals(DocSuiteContext.Current.ProtocolEnv.ProtocolRejectionContainerId) Then
                    AjaxAlert("Errore di configurazione contenitore", "Il contenitore dei protocolli rigettati non può essere a sua volta soggetto a rigetto.")
                    Return
                End If
                SessionOChart = Facade.OChartItemContainerFacade.RejectionTrasformer(SelectedOChartItem, container, Not itemContainer.IsRejection)
                UpdateContainers()
        End Select
    End Sub

    Private Sub UpdateContainers()
        ' Contenitori
        myContainersControl.Clear()
        If Not SelectedOChartItem.Containers.IsNullOrEmpty() Then
            myContainersControl.LoadItems(SelectedOChartItem.Containers.Distinct().Select(Function(ci) ci.Container), Function(c) Not c.IsActive)
        End If
    End Sub

    Private Sub tbContactControl_ButtonClick(sender As Object, e As RadToolBarEventArgs) Handles tbContactControl.ButtonClick
        Dim sourceControl As RadToolBarButton = DirectCast(e.Item, RadToolBarButton)

        Select Case sourceControl.CommandName
            Case "REMOVE"
                Dim contact As Contact = myContactControl.GetSelectedItem()
                myContactControl.RemoveItem(contact)
        End Select
    End Sub

    Private Sub CmdHeadAddClick(sender As Object, e As EventArgs) Handles cmdChartDetailAdd.Click
        Select Case cmdChartDetailAdd.CommandArgument
            Case "EDIT"
                Dim ochart As OChart = SelectedOChart
                ochart.Title = txtChartDetailTitle.Text
                ochart.Description = txtChartDetailDescription.Text

                Facade.OChartFacade.Update(ochart)
                DataBindOCharts()
                SelectedOChart = ochart
            Case "SAVE"
                Dim draft As New OChart() With {
                    .StartDate = dpChartDetailDateFrom.SelectedDate,
                    .EndDate = dpChartDetailDateTo.SelectedDate,
                    .Title = txtChartDetailTitle.Text,
                    .Description = txtChartDetailDescription.Text,
                    .Enabled = True
                }

                Dim hierarchy As OChart = Facade.OChartFacade.AddOChart(draft)
                DataBindOCharts()

                SelectedOChart = hierarchy
                If hierarchy.HasItems Then
                    SelectedOChartItem = hierarchy.Items.FirstOrDefault()
                End If
        End Select

        AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}');", rwChartDetail.ClientID))
    End Sub

    Private Sub CmdItemDetailClick(sender As Object, e As EventArgs) Handles cmdItemDetailAdd.Click, cmdItemDetailEditOk.Click
        Dim cmd As Button = CType(sender, Button)
        ItemDetailCodeValidator.IsValid = True

        Dim isAdding As Boolean = cmd.CommandName.Eq("add")

        Dim item As OChartItem = SelectedOChartItem
        Dim oldCode As String = String.Empty
        If Not isAdding Then
            oldCode = SelectedOChartItem.Code
        End If

        ' Se è una aggiunta
        If isAdding Then
            item = New OChartItem(DocSuiteContext.Current.User.FullUserName)
            item.Enabled = True
            item.OrganizationChart = SelectedOChart
            item.Code = txtItemDetailCode.Text
            If Not cmd.CommandArgument.Eq("root") Then
                item.Parent = SelectedOChartItem
            End If
        End If

        item.Title = txtItemDetailTitle.Text
        item.Description = txtItemDetailDescription.Text
        item.Acronym = txtItemDetailAcronym.Text

        ' Verifica del codice inserito
        If Not oldCode.Eq(txtItemDetailCode.Text) OrElse isAdding Then
            If DoesCodeExist(txtItemDetailCode.Text) Then
                ItemDetailCodeValidator.IsValid = False
                AjaxAlert(String.Format("Il codice [{0}] risulta già in uso.", txtItemDetailCode.Text))
                Return
            End If
        End If
        item.Code = txtItemDetailCode.Text

        ' Refresh della GUI e selezione del nodo creato/modificato
        If isAdding Then
            Dim oChart As OChart = Facade.OChartItemFacade.AddOChartItem(item)
            If cmd.CommandArgument.Eq("root") Then
                SelectedOChart.AddChild(item)
                OChartTree.DataSource = SelectedOChart.Roots.OrderBy(Function(x) x.Title).ToList()
                OChartTree.DataBind()
            Else
                OChartTreeNodeExpand(OChartTree, New RadTreeNodeEventArgs(OChartTree.SelectedNode))
                OChartTree.SelectedNode.Expanded = True
            End If
        Else
            Facade.OChartItemFacade.UpdateOChartItem(item, oldCode)
        End If

        SelectedOChartItem = item

        ' Chiudo la finestra
        AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}');", rwItemDetail.ClientID))
    End Sub

    Private Sub CmdItemRemoveConfirmOkClick(sender As Object, e As EventArgs) Handles cmdItemRemoveConfirmOk.Click
        Dim item As OChartItem = SelectedOChartItem

        If (item.HasItems OrElse item.HasResources) Then
            AjaxAlert(String.Format("Impossibile eliminare il nodo '{0}' ({1}) se contiente dei sottonodi o se contiente 'Risorse' associate", item.Title, item.FullCode))
            Return
        End If

        SelectedOChart = Facade.OChartItemFacade.RemoveOChartItem(item)

        If item.IsRoot Then
            If Not SelectedOChart.Roots.IsNullOrEmpty() Then
                SelectedOChartItem = SelectedOChart.Roots.OrderBy(Function(x) x.Title).FirstOrDefault()
            Else
                SelectedOChartItem = Nothing
            End If
        Else
            SelectedOChartItem = item.Parent
            OChartTreeNodeExpand(OChartTree, New RadTreeNodeEventArgs(OChartTree.SelectedNode))
            OChartTree.SelectedNode.Expanded = True
        End If

        ' Chiudo la finestra
        AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}');", rwItemRemoveConfirm.ClientID))
    End Sub

    Private Sub CmdChartRemoveOkClick(sender As Object, e As EventArgs) Handles cmdChartRemoveOk.Click
        If (SelectedOChart.IsEffective) Then
            AjaxAlert("Impossibile eliminare l'organigramma effettivo")
            Return
        End If
        If (SelectedOChart.IsEnded) Then
            AjaxAlert("Impossibile eliminare gli organigrammi chiusi")
            Return
        End If

        SelectedOChart.Enabled = False
        Facade.OChartFacade.UpdateOnly(SelectedOChart)

        DataBindOCharts()
        SelectedOChart = Facade.OChartFacade.GetEffective()
        SelectedOChartItem = Nothing

        AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}');", rwChartRemove.ClientID))
    End Sub

    Private Sub CmdContainerSelectorOkClick(sender As Object, e As EventArgs) Handles cmdContainerSelectorOk.Click
        Dim selectedContainer As String = cbContainer.SelectedValue
        If String.IsNullOrWhiteSpace(selectedContainer) Then
            Return
        End If

        Dim idContainer As Integer = Integer.Parse(selectedContainer)
        If Not SelectedOChartItem.Containers.Any(Function(ic) ic.Container.Id.Equals(idContainer)) Then
            Dim container As Container = Facade.ContainerFacade.GetById(idContainer)
            myContainersControl.AddItem(container, Not container.IsActive)
        End If

        AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}');", rwContainerSelector.ClientID))
    End Sub

    Private Sub CmdContainerSelectorCancelClick(sender As Object, e As EventArgs) Handles cmdContainerSelectorCancel.Click
        AjaxManager.ResponseScripts.Add(String.Format("CloseWindow('{0}');", rwContainerSelector.ClientID))
    End Sub

    Private Sub MyRolecontrolRolesAdding(sender As Object, args As ItemControlEventArgs(Of Role)) Handles myRolecontrol.ItemsAdding
        If SelectedOChartItem.Roles.Any(Function(r) r.Role.Id.Equals(args.Item.Id)) Then
            AjaxAlert(String.Format("il settore [{0}] è già in uso.", args.Item.Name))
            args.Cancel = True
        End If
    End Sub

    Private Sub MyRolecontrolRolesAdded(sender As Object, args As ItemControlEventArgs(Of Role)) Handles myRolecontrol.ItemsAdded
        If args.Item Is Nothing Then
            args.Cancel = True
            Return
        End If

        Dim contacts As IList(Of Contact) = Facade.RoleFacade.GetContacts(New List(Of Role) From {args.Item})

        SessionOChart = Facade.OChartItemFacade.AddRole(args.Item, SelectedOChartItem, contacts)

        If Not contacts.IsNullOrEmpty() Then
            DisabledRiseContactControl = True
            myContactControl.AddItems(contacts, Function(r) Not r.IsActive OrElse Not r.IsActiveRange())
            DisabledRiseContactControl = False
            AjaxAlert(String.Format("Aggiunti {0} contatti collegati", contacts.Count))
        End If

    End Sub

    Private Sub MyRolecontrolRolesRemoved(sender As Object, args As ItemControlEventArgs(Of Role)) Handles myRolecontrol.ItemsRemoved
        If args.Item Is Nothing Then
            args.Cancel = True
            Return
        End If

        SessionOChart = Facade.OChartItemFacade.RemoveRole(args.Item, SelectedOChartItem)
    End Sub


    Private Sub myContactControl_ItemsAdding(sender As Object, args As ItemControlEventArgs(Of Contact)) Handles myContactControl.ItemsAdding
        If SelectedOChartItem.Contacts.Any(Function(c) c.Contact.Id.Equals(args.Item.Id)) Then
            AjaxAlert(String.Format("Il contatto [{0}] risulta già essere assegnato.", args.Item.DescriptionFormatByContactType))
            args.Cancel = True
        End If
    End Sub

    Private Sub myContactControl_ItemsAdded(sender As Object, args As ItemControlEventArgs(Of Contact)) Handles myContactControl.ItemsAdded
        If args.Item Is Nothing Then
            args.Cancel = True
            Return
        End If
        If (DisabledRiseContactControl) Then
            Return
        End If
        SessionOChart = Facade.OChartItemFacade.AddContact(args.Item, SelectedOChartItem)
    End Sub

    Private Sub myContactControl_ItemsRemoved(sender As Object, args As ItemControlEventArgs(Of Contact)) Handles myContactControl.ItemsRemoved
        If args.Item Is Nothing Then
            args.Cancel = True
            Return
        End If

        SessionOChart = Facade.OChartItemFacade.RemoveContact(args.Item, SelectedOChartItem)
    End Sub

    Private Sub MyContainersControlContainersAdded(sender As Object, args As ItemControlEventArgs(Of Container)) Handles myContainersControl.ItemsAdded
        If args.Item Is Nothing Then
            args.Cancel = True
            Return
        End If

        If SelectedOChartItem.Containers.Any(Function(ic) ic.Container.Id.Equals(args.Item.Id)) Then
            AjaxAlert(String.Format("Il contenitore [{0}] risulta già collegato al nodo [{1}].", args.Item.Name, SelectedOChartItem.Description))
            args.Cancel = True
            Return
        End If

        SessionOChart = Facade.OChartItemFacade.AddContainer(args.Item, SelectedOChartItem)
    End Sub

    Private Sub MyContainersControlNodeCreated(sender As Object, args As ItemNodeEventArgs(Of Container)) Handles myContainersControl.NodeCreated
        Dim itemContainer As OChartItemContainer = SelectedOChartItem.Containers.FindByResource(args.Item).First()

        If itemContainer.IsMaster Then
            args.Group = "Proprietario"
            args.Node.Font.Bold = True
            args.Node.Attributes.Add("IsMaster", "true")
        Else
            args.Group = SharedGroupName
            args.Node.Font.Italic = True
        End If

        If itemContainer.IsRejection Then
            args.Node.Text = String.Format("{0} [Rigetto attivo]", args.Node.Text)
            args.Node.Attributes.Add("IsRejection", "true")
        End If
    End Sub

    Private Sub MyContainersControlContainersRemoved(sender As Object, args As ItemControlEventArgs(Of Container)) Handles myContainersControl.ItemsRemoved
        If args.Item Is Nothing Then
            args.Cancel = True
            Return
        End If

        SessionOChart = Facade.OChartItemFacade.RemoveContainer(args.Item, SelectedOChartItem)
    End Sub

    Private Sub RequiresCacheReset(sender As Object, args As Object) _
        Handles cmdChartDetailAdd.Click, cmdChartRemoveOk.Click,
            cmdItemDetailAdd.Click, cmdItemDetailEditOk.Click, cmdItemRemoveConfirmOk.Click,
            myContactControl.ItemsAdded, myContactControl.ItemsRemoved,
            myRolecontrol.ItemsAdded, myRolecontrol.ItemsRemoved,
            myContainersControl.ItemsAdded, myContainersControl.ItemsRemoved,
            tbContainersControl.ButtonClick

        If TypeOf sender Is RadToolBar Then
            Dim tbSender As RadToolBar = DirectCast(sender, RadToolBar)
            If tbSender.ID.Equals(tbContainersControl.ID) Then
                Dim tbArgs As RadToolBarEventArgs = DirectCast(args, RadToolBarEventArgs)
                Dim tbButton As RadToolBarButton = DirectCast(tbArgs.Item, RadToolBarButton)
                Select Case tbButton.CommandName
                    Case "MASTER", "REJECTION"
                        CommonShared.ClearEffectiveOChart()
                        Return
                    Case Else
                        Return
                End Select
            End If
        End If

        CommonShared.ClearEffectiveOChart()
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf OChartAjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(tbItem, windowSelOChart, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(windowSelOChart, OChartTree, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlOCharts, DetailPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(ddlOCharts, treeSplitter, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(OChartTree, DetailPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(OChartTree, OChartTree)

        ' Toolbar
        AjaxManager.AjaxSettings.AddAjaxSetting(tbItem, DetailPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(tbItem, treeSplitter, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(tbItem, ItemDetailUpdatePanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(tbItem, ItemRemoveConfirmPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(tbItem, ChartDetailUpdatePanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(tbItem, ChartRemoveUpdatePanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, DetailPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, treeSplitter, MasterDocSuite.AjaxDefaultLoadingPanel)

        ' Pulsante di conferma aggiunta nodo
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdItemDetailAdd, ItemDetailUpdatePanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdItemDetailAdd, DetailPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdItemDetailAdd, treeSplitter)
        ' Pulsante di conferma modifica nodo
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdItemDetailEditOk, ItemDetailUpdatePanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdItemDetailEditOk, DetailPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdItemDetailEditOk, treeSplitter)
        ' Pulsante di eliminazione nodo
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdItemRemoveConfirmOk, ItemRemoveConfirmPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdItemRemoveConfirmOk, DetailPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdItemRemoveConfirmOk, treeSplitter)
        ' Pulsante di aggiunta Chart
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdChartDetailAdd, ChartDetailUpdatePanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdChartDetailAdd, DetailPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdChartDetailAdd, treeSplitter)
        ' Pulsante di eliminazion Chart
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdChartRemoveOk, ChartRemoveUpdatePanel, MasterDocSuite.AjaxFlatLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdChartRemoveOk, DetailPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdChartRemoveOk, treeSplitter)

        AjaxManager.AjaxSettings.AddAjaxSetting(tbRoleControl, myRolecontrol, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, myRolecontrol, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(tbContactControl, myContactControl, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, myContactControl, MasterDocSuite.AjaxDefaultLoadingPanel)

        AjaxManager.AjaxSettings.AddAjaxSetting(tbContainersControl, myContainersControl, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, myContainersControl, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdContainerSelectorOk, myContainersControl, MasterDocSuite.AjaxDefaultLoadingPanel)

    End Sub

    Private Sub ResetItemDetailWindow(imported As Boolean)
        ' Valori di default
        cmdItemDetailAdd.Visible = False
        cmdItemDetailAdd.CommandArgument = String.Empty
        cmdItemDetailEditOk.Visible = False

        txtItemDetailTitle.Text = String.Empty
        txtItemDetailDescription.Text = String.Empty
        txtItemDetailAcronym.Text = String.Empty
        cbItemDetailEnabled.Checked = True
        txtItemDetailCode.Text = String.Empty

        txtItemDetailTitle.ReadOnly = imported
        txtItemDetailCode.ReadOnly = imported
    End Sub

    Private Sub DataBindOCharts()
        ddlOCharts.Items.Clear()
        For Each chart As OChart In Facade.OChartFacade.GetEnabled().OrderChronologically()
            Dim item As New ListItem()
            item.Text = String.Format("[{0:dd/MM/yyyy}] {1}", chart.StartDateOrDefault, chart.Title)
            item.Value = chart.Id.ToString()
            ddlOCharts.Items.Add(item)
        Next
    End Sub

    Private Sub ClearOChartItemDetails()
        lblItemName.Text = String.Empty
        lblItemDescription.Text = String.Empty
        lblItemAcronym.Text = String.Empty
        lblItemCode.Text = String.Empty
        lblItemParentCode.Text = String.Empty

        ' Contenitori
        myContainersControl.Clear()
        ' Roles
        myRolecontrol.Clear()
        ' Contatti
        myContactControl.Clear()
    End Sub

    Private Sub DataBindOChartItemDetails(ByVal item As OChartItem)
        DetailPanel.Enabled = True

        Dim node As RadTreeNode = OChartTree.FindNodeByValue(item.Id.ToString())
        node.Text = String.Format("{0} [{1}]", item.Title, item.Acronym)

        lblItemName.Text = item.Title
        lblItemDescription.Text = item.Description
        lblItemAcronym.Text = item.Acronym
        lblItemCode.Text = item.Code

        If item.Parent IsNot Nothing Then
            lblItemParentCode.Text = item.Parent.Code
        Else
            lblItemParentCode.Text = "-"
        End If

        ' Contatti
        myContactControl.Clear()
        If Not item.Contacts.IsNullOrEmpty() Then
            myContactControl.LoadItems(item.Contacts.Distinct().Select(Function(ci) ci.Contact), Function(r) Not r.IsActive OrElse Not r.IsActiveRange())
        End If
        ' Contenitori
        myContainersControl.Clear()
        If Not item.Containers.IsNullOrEmpty() Then
            myContainersControl.LoadItems(item.Containers.Distinct().Select(Function(ci) ci.Container), Function(r) Not r.IsActive)
        End If

        ' Roles
        myRolecontrol.Clear()
        If Not item.Roles.IsNullOrEmpty() Then
            myRolecontrol.LoadItems(item.Roles.Distinct().Select(Function(ri) ri.Role), Function(r) Not r.IsActive OrElse Not r.IsActiveRange())
        End If

    End Sub

    Private Shared Sub FillNodeFromOChartItem(ByRef node As RadTreeNode, item As OChartItem)
        node.Text = String.Format("{0} [{1}]", item.Title, item.Acronym)
        node.Value = item.Id.ToString()
        node.ExpandMode = CType(If(item.HasItems, TreeNodeExpandMode.ServerSide, TreeNodeExpandMode.ClientSide), TreeNodeExpandMode)
        node.ImageUrl = CType(If(item.IsRoot, ImagePath.SmallNetworkShareStar, ImagePath.SmallNetworkShare), String)
    End Sub

    Public Function GetWindowParameters() As String
        Dim parameters As New StringBuilder
        parameters.AppendFormat("DSWEnvironment={0}", DSWEnvironment.Protocol)

        If Not String.IsNullOrEmpty(Type) Then
            parameters.AppendFormat("&Type={0}", Type)
        End If
        parameters.AppendFormat("&MultiSelect={0}", True)
        'parameters.AppendFormat("&RoleRestiction={0}", RoleRestictions)
        'parameters.AppendFormat("&Rights={0}", Rights)
        parameters.AppendFormat("&isActive={0}", True)

        Return parameters.ToString()
    End Function

    Public Function GetContactSelectorParameters() As String
        Dim parameters As New StringBuilder

        parameters.Append("ShowAll=False")
        If ProtocolEnv.OChartContactRoot.HasValue Then
            parameters.AppendFormat("&ContactRoot={0}", ProtocolEnv.OChartContactRoot.Value)
        End If
        If Not String.IsNullOrEmpty(Type) Then
            parameters.AppendFormat("&Type={0}", Type)
        End If

        Return parameters.ToString()
    End Function

    Public Function GetOChartSelectorParameters() As String
        Dim parameters As New StringBuilder
        parameters.Append("Type=Prot")
        If SelectedOChartItem IsNot Nothing Then
            parameters.AppendFormat("&OChartItemSelectedId={0}", SelectedOChartItem.Id)
        End If

        Return parameters.ToString()
    End Function

    Private Function DoesCodeExist(code As String) As Boolean
        If SelectedOChart.Items Is Nothing Then
            Return False
        End If
        Return SelectedOChart.Items.Any(Function(i) i.Code.Eq(code))
    End Function

    Private Sub MoveOChartItems(nodeTo As Guid)
        Dim ochartItemFrom As OChartItem = Facade.OChartItemFacade.GetById(SelectedOChartItem.Id)
        Dim ochartItemTo As OChartItem = Facade.OChartItemFacade.GetById(nodeTo)
        If ochartItemTo IsNot Nothing Then
            ochartItemTo.Items.Add(ochartItemFrom)
        End If

        ochartItemFrom.Parent = ochartItemTo
        Facade.OChartItemFacade.CalculateFullCode(ochartItemFrom)

        If ochartItemTo IsNot Nothing Then
            Facade.OChartItemFacade.UpdateOnly(ochartItemTo)
        End If

        Facade.OChartItemFacade.UpdateOnly(ochartItemFrom)
        Facade.OChartItemFacade.CalculateFullCodeRecursive(ochartItemFrom.Items)

    End Sub


    Public Sub Refresh()
        OChartTree.Nodes(0).Nodes.Clear()
        SelectedOChart = Facade.OChartFacade.GetEffective()
    End Sub
#End Region

End Class