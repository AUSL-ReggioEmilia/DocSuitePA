<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="WorkflowInstances.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.WorkflowInstances" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var workflowInstances;
            require(["Workflows/WorkflowInstances"], function (WorkflowInstances) {
                $(function () {
                    workflowInstances = new WorkflowInstances(tenantModelConfiguration.serviceConfiguration);
                    workflowInstances.workflowInstancesGridId = "<%=workflowInstanceGrid.ClientID %>";
                    workflowInstances.ajaxLoadingPanelId = "<%=MasterDocSuite.AjaxDefaultLoadingPanel.ClientId %>";
                    workflowInstances.btnSearchId = "<%=btnSearch.ClientID %>";
                    workflowInstances.btnCleanId = "<%=btnClean.ClientID %>";
                    workflowInstances.dtpWorkflowRepositoryActiveFromId = "<%=dtpWorkflowRepositoryActiveFrom.ClientID %>";
                    workflowInstances.dtpWorkflowRepositoryActiveToId = "<%=dtpWorkflowRepositoryActiveTo.ClientID %>";
                    workflowInstances.cmbWorkflowRepositoryStatusId = "<%=cmbWorkflowRepositoryStatus.ClientID %>";
                    workflowInstances.rwWorkflowActivitiesId = "<%=rwWorkflowActivities.ClientID %>";
                    workflowInstances.rwWorkflowInstanceLogsId = "<%=rwWorkflowInstanceLogs.ClientID %>";
                    workflowInstances.workflowActivityGridId = "<%=workflowActivityGrid.ClientID %>";
                    workflowInstances.workflowInstanceLogsGridId = "<%=workflowInstanceLogsGrid.ClientID %>";
                    workflowInstances.rcbWorkflowRepositoriesId = "<%= rcbWorkflowRepositories.ClientID %>";
                    workflowInstances.maxNumberElements = "<%= ProtocolEnv.MaxNumberDropdownElements %>";
                    workflowInstances.workflowRepositoryName = "<%=WorkflowRepositoryName%>";

                    workflowInstances.initialize();
                });
            });

            function OnGridCommand(sender, args) {
                if (args.get_commandName() === "Page") {
                    args.set_cancel(true);
                    workflowInstances.onPageChanged();
                }
            }

            function OnGridDataBound(sender, args) {
                workflowInstances.onGridDataBound();
            }
        </script>
    </telerik:RadScriptBlock>

    <table class="datatable">
        <tr>
            <td class="label labelPanel" style="width: 20%;">Nome flusso di lavoro:
            </td>
            <td style="width: 30%;">
                <telerik:RadComboBox runat="server" 
                    CausesValidation="false"
                    ID="rcbWorkflowRepositories" 
                    AutoPostBack="false" 
                    EnableLoadOnDemand="true" 
                    ItemRequestTimeout="500" 
                    Width="375px" 
                    ShowMoreResultsBox="true">
                </telerik:RadComboBox>
            </td>
        </tr>
        <tr>
            <td class="label labelPanel" style="width: 20%;">Periodo:
            </td>
            <td style="width: 80%;">
                <telerik:RadDatePicker 
                    DateInput-LabelWidth="20%" 
                    DateInput-LabelCssClass="strongRiLabel" 
                    Style="height: auto !important;" 
                    Width="300" 
                    DateInput-Label="Da data" 
                    ID="dtpWorkflowRepositoryActiveFrom" 
                    runat="server" />
                <telerik:RadDatePicker 
                    DateInput-LabelWidth="20%" 
                    DateInput-LabelCssClass="strongRiLabel" 
                    Style="height: auto !important;" 
                    Width="300" 
                    DateInput-Label="A data" 
                    ID="dtpWorkflowRepositoryActiveTo" 
                    runat="server" />
                <asp:CompareValidator ID="rangeDateValidator" runat="server"
                    ControlToValidate="dtpWorkflowRepositoryActiveTo"
                    ControlToCompare="dtpWorkflowRepositoryActiveFrom"
                    Operator="GreaterThan"
                    Type="Date"
                    ErrorMessage="La data di fine deve essere maggiore della data di inizio">
                </asp:CompareValidator>
            </td>
        </tr>
        <tr>
            <td class="label labelPanel" style="width: 20%">Stato:
            </td>
            <td style="width: 30%">
                <telerik:RadComboBox runat="server" ID="cmbWorkflowRepositoryStatus" DataTextField="Text" DataValueField="Value" Width="10%" AutoPostBack="false" EmptyMessage="" />
            </td>
        </tr>
    </table>
    <div style="margin: 1px 1px 10px 1px;">
        <div>
            <telerik:RadButton ID="btnSearch" Text="Aggiorna visualizzazione" Width="150px" runat="server" TabIndex="1" AutoPostBack="False" />
            <telerik:RadButton ID="btnClean" Text="Azzera filtri" Width="150px" runat="server" TabIndex="1" AutoPostBack="False" />
        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel runat="server" CssClass="radGridWrapper" ID="pageContent">
        <telerik:RadGrid runat="server" 
            CssClass="workflowInstanceGrid" 
            ID="workflowInstanceGrid" 
            Skin="Office2010Blue"
            GridLines="None" 
            Height="100%" 
            PageSize="30"
            AllowPaging="true" 
            AllowMultiRowSelection="false" 
            AllowFilteringByColumn="false">
            <ClientSettings EnablePostBackOnRowClick="false">
                <Scrolling AllowScroll="true" UseStaticHeaders="true" ScrollHeight="100%" />
                <Resizing AllowColumnResize="false" AllowRowResize="false" AllowResizeToFit="true"/>
                <ClientEvents OnCommand="OnGridCommand" OnDataBound="OnGridDataBound" />
            </ClientSettings>
            <MasterTableView CommandItemDisplay="None" 
                AutoGenerateColumns="false" 
                TableLayout="Fixed" 
                AllowMultiColumnSorting="true"
                NoMasterRecordsText="Nessuno elemento trovato nel periodo indicato." 
                PagerStyle-PagerTextFormat="{4} Visualizzati {3} su {5}">
                <Columns>
                    <telerik:GridBoundColumn DataField="Subject" HeaderText="Oggetto" HeaderStyle-Width="300px" />
                    <telerik:GridBoundColumn DataField="RegistrationDate" HeaderText="Creato il" HeaderStyle-Width="200px" />
                    <telerik:GridBoundColumn DataField="RegistrationUser" HeaderText="Creato da" HeaderStyle-Width="200px" />
                    <telerik:GridBoundColumn DataField="Status" HeaderText="Stato" HeaderStyle-Width="120px" />
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="In errore?" UniqueName="HasActivitiesInErrorLabel">
                        <HeaderStyle HorizontalAlign="Center" Width="120px" />
                        <ItemStyle HorizontalAlign="Center" Width="100px" />
                        <ItemTemplate>
                            <div style="position: relative">
                                <telerik:RadButton ButtonType="LinkButton" ID="btnWorkflowInstanceLogs" Style="display: none;" runat="server" AutoPostBack="false" />
                            </div>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Numero attività completate" UniqueName="WorkflowActivitiesDoneCount">
                        <HeaderStyle HorizontalAlign="Center" Width="200px" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <div style="position: relative">
                                <telerik:RadButton ButtonType="LinkButton" ID="btnWorkflowActivities" runat="server" AutoPostBack="false" />
                            </div>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <PagerStyle PageSizeControlType="None"></PagerStyle>
        </telerik:RadGrid>
    </asp:Panel>

    <telerik:RadWindow runat="server" ID="rwWorkflowActivities" Title="Attività" Width="750" Height="500" Behaviors="Close, Move, Maximize">
        <ContentTemplate>
            <asp:UpdatePanel runat="server" ID="ContainerSelectorUpdatePanel" UpdateMode="Conditional">
                <ContentTemplate>
                    <telerik:RadGrid runat="server" CssClass="workflowActivityGrid" ID="workflowActivityGrid" Skin="Office2010Blue"
                        GridLines="None" Height="100%" AllowPaging="false" AllowMultiRowSelection="false" AllowFilteringByColumn="false">
                        <ClientSettings EnablePostBackOnRowClick="false">
                            <Scrolling AllowScroll="true" UseStaticHeaders="true" ScrollHeight="100%" />
                            <Resizing AllowColumnResize="true" AllowRowResize="false" />
                            <ClientEvents OnCommand="OnGridCommand" />
                        </ClientSettings>
                        <MasterTableView CommandItemDisplay="None" AutoGenerateColumns="false" TableLayout="Fixed" AllowMultiColumnSorting="true"
                            NoMasterRecordsText="Nessuno elemento trovato." PagerStyle-PagerTextFormat="{4} Visualizzati {3} su {5}">
                            <Columns>
                                <telerik:GridBoundColumn DataField="Name" HeaderText="Nome" />
                                <telerik:GridBoundColumn DataField="ActivityTypeDescription" HeaderText="Tipo di attività" HeaderStyle-Width="" />
                                <telerik:GridBoundColumn DataField="RegistrationDateFormatted" HeaderText="Creato il"/>
                                <telerik:GridBoundColumn DataField="RegistrationUser" HeaderText="Creato da"/>
                                <telerik:GridBoundColumn DataField="StatusDescription" HeaderText="Stato" />
                                <telerik:GridBoundColumn DataField="Subject" HeaderText="Oggetto" HeaderStyle-Width="250px" />
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

    <telerik:RadWindow runat="server" ID="rwWorkflowInstanceLogs" Title="Log" Width="750" Height="500" Behaviors="Close, Move, Maximize">
        <ContentTemplate>
            <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                <ContentTemplate>
                    <telerik:RadGrid runat="server" CssClass="workflowActivityGrid" ID="workflowInstanceLogsGrid" Skin="Office2010Blue"
                        GridLines="None" Height="100%" AllowPaging="false" AllowMultiRowSelection="false" AllowFilteringByColumn="false">
                        <ClientSettings EnablePostBackOnRowClick="false">
                            <Scrolling AllowScroll="true" UseStaticHeaders="true" ScrollHeight="100%" />
                            <Resizing AllowColumnResize="true" AllowRowResize="true" />
                            <ClientEvents OnCommand="OnGridCommand" />
                        </ClientSettings>
                        <MasterTableView CommandItemDisplay="None" AutoGenerateColumns="false" TableLayout="Fixed" AllowMultiColumnSorting="true"
                            NoMasterRecordsText="Nessuno elemento trovato." PageStyle-PagerTextFormat="{4} Visualizzati {3} su {5}">
                            <Columns>
                                <telerik:GridBoundColumn DataField="LogUser" HeaderText="Utente" />
                                <telerik:GridBoundColumn DataField="Computer" HeaderText="Postazione" />
                                <telerik:GridBoundColumn DataField="TypeDescription" HeaderText="Tipo" />
                                <telerik:GridBoundColumn DataField="LogDate" HeaderText="Data"/>
                                <telerik:GridBoundColumn DataField="Description" HeaderText="Descrizione"/>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>
</asp:Content>
