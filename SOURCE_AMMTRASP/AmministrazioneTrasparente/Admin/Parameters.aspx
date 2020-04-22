<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Admin.Master" AutoEventWireup="true" CodeBehind="Parameters.aspx.cs" Inherits="AmministrazioneTrasparente.Admin.Parameters" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="content" runat="server">
    <telerik:RadCodeBlock ID="radCode" runat="server">
        <script type="text/javascript">
            function ShowEditForm(id, rowIndex) {
                var grid = $find("<%= parameters.ClientID %>");

                var rowControl = grid.get_masterTableView().get_dataItems()[rowIndex].get_element();
                grid.get_masterTableView().selectItem(rowControl, true);

                window.radopen("EditParameter.aspx?Id=" + id, "parameterEditing");
                return false;
            }

            function refreshGrid(arg) {
                var masterTable = $find("<%= parameters.ClientID %>").get_masterTableView();
                masterTable.rebind();
            }
        </script>
    </telerik:RadCodeBlock>
    <telerik:RadAjaxManager ID="ajaxManager" runat="server" UpdateInitiatorPanelsOnly="true">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="parameters">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="parameters" LoadingPanelID="loadingPanel"></telerik:AjaxUpdatedControl>
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel runat="server" ID="loadingPanel"></telerik:RadAjaxLoadingPanel>
    <div class="panel panel-default">
        <div class="panel-heading">
            <div class="panel-title">
                <h3><span class="glyphicon glyphicon-pencil"></span>&nbsp;Modifica parametri</h3>
            </div>
        </div>
        <div class="panel-body" style="padding: 9px;">
            <telerik:RadGrid AutoGenerateColumns="False" AllowSorting="True" Skin="MetroTouch" OnNeedDataSource="parameters_OnNeedDataSource" OnItemDataBound="parameters_OnItemDataBound" 
                AllowFilteringByColumn="True" GridLines="Vertical" ID="parameters" runat="server" Width="100%">
                <GroupingSettings ShowUnGroupButton="true"></GroupingSettings>
                <MasterTableView EnableGroupsExpandAll="True">
                    <GroupByExpressions>
                        <telerik:GridGroupByExpression>
                            <SelectFields>
                                <telerik:GridGroupByField FieldAlias="Gruppo" FieldName="ItemGroup"></telerik:GridGroupByField>
                            </SelectFields>
                            <GroupByFields>
                                <telerik:GridGroupByField FieldName="ItemGroup"></telerik:GridGroupByField>
                            </GroupByFields>
                        </telerik:GridGroupByExpression>
                    </GroupByExpressions>
                    <EditFormSettings>
                        <PopUpSettings Modal="true" />
                    </EditFormSettings>
                    <Columns>
                        <telerik:GridBoundColumn DataField="Id" HeaderText="Id" ReadOnly="True" UniqueName="Id" Display="False"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderStyle-Width="20%" DataField="KeyName" FilterControlWidth="130px" ItemStyle-Font-Bold="true" ItemStyle-Width="10%" HeaderText="Chiave"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderStyle-Width="50px" DataField="KeyValue" FilterControlWidth="250px" HeaderText="Valore" ItemStyle-Width="45%"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Note" ForceExtractValue="None" FilterControlWidth="200px" HeaderStyle-Width="25%" HeaderText="Note" ItemStyle-Width="25%" />
                        <telerik:GridTemplateColumn AllowFiltering="False" UniqueName="TemplateEditColumn">
                            <ItemTemplate>
                                <button runat="server" ID="edit" style="width: 35px;" class="btn btn-block btn-sm btn-primary">
                                    <span class="glyphicon glyphicon-edit"></span>
                                </button>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
                <GroupingSettings ShowUnGroupButton="true" />
            </telerik:RadGrid>
        </div>
    </div>
    <telerik:RadWindowManager ID="popupEditing" Height="600" Width="500" Behaviors="Close, Move" ShowContentDuringLoad="True" Skin="MetroTouch" VisibleStatusbar="False" Top="-10" Left="-10" KeepInScreenBounds="False" runat="server" EnableShadow="true">
        <Windows>
            <telerik:RadWindow ID="parameterEditing" runat="server" Title="Modifica parametro" ReloadOnShow="true" ShowContentDuringLoad="false"
                Modal="true">
            </telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>
</asp:Content>
