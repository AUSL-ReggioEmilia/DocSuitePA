<%@ Page Title="Massimario di scarto" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="TbltMassimarioScartoGes.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltMassimarioScartoGes" %>

<%@ Register Src="~/UserControl/uscMassimarioScarto.ascx" TagName="MassimarioScarto" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            var tbltMassimarioScartoGes;
            require(["Tblt/TbltMassimarioScartoGes"], function (TbltMassimarioScartoGes) {
                $(function () {
                    tbltMassimarioScartoGes = new TbltMassimarioScartoGes(tenantModelConfiguration.serviceConfiguration);
                    tbltMassimarioScartoGes.uscMassimarioScartoId = "<%= uscMassimarioScarto.TreeMassimarioScarto.ClientID %>";
                    tbltMassimarioScartoGes.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    tbltMassimarioScartoGes.splitterPageId = "<%= splContent.ClientID %>";
                    tbltMassimarioScartoGes.lblConservationPeriodId = "<%= lblConservationPeriod.ClientID %>";
                    tbltMassimarioScartoGes.pnlDetailsId = "<%= pnlDetails.ClientID %>";
                    tbltMassimarioScartoGes.folderToolBarId = "<%= uscMassimarioScarto.FolderToolBar_Grid.ClientID %>";
                    tbltMassimarioScartoGes.txtNameId = "<%= txtName.ClientID %>";
                    tbltMassimarioScartoGes.txtCodeId = "<%= txtCode.ClientID %>";
                    tbltMassimarioScartoGes.txtNoteId = "<%= txtNote.ClientID %>";
                    tbltMassimarioScartoGes.btnInfiniteId = "<%= btnInfinite.ClientID %>";
                    tbltMassimarioScartoGes.txtPeriodId = "<%= txtPeriod.ClientID %>";
                    tbltMassimarioScartoGes.btnSaveMassimarioId = "<%= btnSaveMassimario.ClientID %>";
                    tbltMassimarioScartoGes.rwEditMassimarioId = "<%= rwEditMassimario.ClientID %>";
                    tbltMassimarioScartoGes.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    tbltMassimarioScartoGes.rgvCategoriesId = "<%= rgvCategories.ClientID %>";
                    tbltMassimarioScartoGes.lblNoteId = "<%= lblNote.ClientID %>";
                    tbltMassimarioScartoGes.lblStartDateId = "<%= lblStartDate.ClientID %>";
                    tbltMassimarioScartoGes.lblEndDateId = "<%= lblEndDate.ClientID %>";
                    tbltMassimarioScartoGes.rdpStartDateId = "<%= rdpStartDate.ClientID %>";
                    tbltMassimarioScartoGes.rdpEndDateId = "<%= rdpEndDate.ClientID %>";
                    tbltMassimarioScartoGes.pnlMetadataId = "<%= pnlMetadata.ClientID %>";
                    tbltMassimarioScartoGes.rfvNameId = "<%= rfvName.ClientID %>";
                    tbltMassimarioScartoGes.rfvCodeId = "<%= rfvCode.ClientID %>";
                    tbltMassimarioScartoGes.rfvEndDateId = "<%= rfvEndDate.ClientID %>";
                    tbltMassimarioScartoGes.managerId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    tbltMassimarioScartoGes.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadWindow ID="rwEditMassimario" runat="server" DestroyOnClose="true" ShowContentDuringLoad="false" ReloadOnShow="true">
        <ContentTemplate>
            <asp:Panel runat="server" ID="pnlMetadata">
                <table class="datatable">
                    <tr>
                        <th colspan="2" class="tabella">Dati Massimario
                        </th>
                    </tr>
                    <tr class="Chiaro" id="rowName">
                        <td class="label" style="width: 25%">Nome:</td>
                        <td>
                            <telerik:RadTextBox ID="txtName" runat="server" Width="100%" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtName" EnableViewState="false" ErrorMessage="Il campo Nome è obbligatorio" Display="Dynamic" ID="rfvName" ValidationGroup="massimarioValidationGroup" />
                        </td>
                    </tr>
                    <tr class="Chiaro" id="rowCode">
                        <td class="label" style="width: 25%">Codice:</td>
                        <td>
                            <telerik:RadNumericTextBox ID="txtCode" NumberFormat-DecimalDigits="0" runat="server" Width="100%" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCode" EnableViewState="false" ErrorMessage="Il campo Codice è obbligatorio" Display="Dynamic" ID="rfvCode" ValidationGroup="massimarioValidationGroup" />
                        </td>
                    </tr>
                    <tr class="Chiaro" id="rowNote">
                        <td class="label" style="width: 25%">Specifiche:</td>
                        <td>
                            <telerik:RadTextBox ID="txtNote" Rows="3" TextMode="MultiLine" runat="server" Width="100%" />
                        </td>
                    </tr>
                    <tr class="Chiaro" id="rowConservationPeriod">
                        <td class="label" style="width: 25%">Tempo conservazione:</td>
                        <td>
                            <telerik:RadNumericTextBox runat="server" ID="txtPeriod" Width="50px" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox>
                            <telerik:RadButton runat="server" ID="btnInfinite" Enabled="false" AutoPostBack="false" ToggleType="CheckBox" Text="Illimitato" ButtonType="ToggleButton"></telerik:RadButton>
                        </td>
                    </tr>
                    <tr class="Chiaro" id="rowStartDate">
                        <td class="label" style="width: 25%">Data attivazione:</td>
                        <td style="padding-bottom: 4px;">
                            <telerik:RadDatePicker runat="server" ID="rdpStartDate"></telerik:RadDatePicker>
                        </td>
                    </tr>
                    <tr class="Chiaro" id="rowEndDate">
                        <td class="label" style="width: 25%;">Data disattivazione:</td>
                        <td style="padding-bottom: 4px;">
                            <telerik:RadDatePicker runat="server" ID="rdpEndDate"></telerik:RadDatePicker>
                            <br />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="rdpEndDate" EnableViewState="false" ErrorMessage="Il campo Data disattivazione è obbligatorio" Display="Dynamic" ID="rfvEndDate" ValidationGroup="massimarioValidationGroup" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <div class="window-footer-wrapper">
                <telerik:RadButton runat="server" ID="btnSaveMassimario" Text="Conferma" ValidationGroup="massimarioValidationGroup" />
            </div>
        </ContentTemplate>
    </telerik:RadWindow>

    <div class="splitterWrapper">
        <telerik:RadSplitter runat="server" Width="100%" Height="100%" ResizeWithParentPane="False" Orientation="Horizontal" ID="splPage">
            <telerik:RadPane runat="server" Width="100%" Height="100%" Scrolling="None">
                <telerik:RadSplitter runat="server" Width="100%" Height="100%" ResizeWithParentPane="False" ID="splContent">
                    <telerik:RadPane runat="server" Width="50%">
                        <usc:MassimarioScarto runat="server" ID="uscMassimarioScarto"></usc:MassimarioScarto>
                    </telerik:RadPane>
                    <telerik:RadSplitBar runat="server" CollapseMode="None"></telerik:RadSplitBar>
                    <telerik:RadPane runat="server" Width="50%">
                        <asp:Panel runat="server" ID="pnlDetails" CssClass="dsw-panel">
                            <div class="dsw-panel-content">
                                <telerik:RadPanelBar runat="server" AllowCollapseAllItems="true" ExpandMode="MultipleExpandedItems" Width="100%">
                                    <Items>
                                        <telerik:RadPanelItem Text="Informazioni" Expanded="true">
                                            <ContentTemplate>
                                                <div class="col-dsw-10" id="detailPeriodSection">
                                                    <b>Tempo di conservazione:</b>
                                                    <asp:Label runat="server" ID="lblConservationPeriod"></asp:Label>
                                                </div>
                                                <div class="col-dsw-10">
                                                    <b>Specifiche:</b>
                                                    <asp:Label runat="server" ID="lblNote"></asp:Label>
                                                </div>
                                                <div class="col-dsw-10">
                                                    <b>Data attivazione:</b>
                                                    <asp:Label runat="server" ID="lblStartDate"></asp:Label>
                                                </div>
                                                <div class="col-dsw-10">
                                                    <b>Data disattivazione:</b>
                                                    <asp:Label runat="server" ID="lblEndDate"></asp:Label>
                                                </div>
                                            </ContentTemplate>
                                        </telerik:RadPanelItem>
                                        <telerik:RadPanelItem Text="Classificatori associati" Expanded="true">
                                            <ContentTemplate>
                                                <telerik:RadGrid runat="server" ID="rgvCategories" GridLines="None" AllowPaging="false" AllowMultiRowSelection="false" AllowSorting="false">
                                                    <MasterTableView AutoGenerateColumns="false" NoMasterRecordsText="Nessun classificatore associato">
                                                        <Columns>
                                                            <telerik:GridTemplateColumn HeaderStyle-Width="16px">
                                                                <ClientItemTemplate>
                                                                    <img src="../Comm/images/Classificatore.gif" />
                                                                </ClientItemTemplate>
                                                            </telerik:GridTemplateColumn>
                                                            <telerik:GridTemplateColumn UniqueName="Name" HeaderStyle-Width="40%" HeaderText="Classificatore">
                                                                <ClientItemTemplate>
                                                                    <span>#=Name #</span>
                                                                </ClientItemTemplate>
                                                            </telerik:GridTemplateColumn>
                                                            <telerik:GridTemplateColumn UniqueName="FullCode" HeaderText="Codice">
                                                                <ClientItemTemplate>
                                                                    <span>#=FullCode #</span>
                                                                </ClientItemTemplate>
                                                            </telerik:GridTemplateColumn>
                                                        </Columns>
                                                    </MasterTableView>
                                                    <ClientSettings Selecting-AllowRowSelect="true" />
                                                </telerik:RadGrid>
                                            </ContentTemplate>
                                        </telerik:RadPanelItem>
                                    </Items>
                                </telerik:RadPanelBar>
                            </div>
                        </asp:Panel>
                    </telerik:RadPane>
                </telerik:RadSplitter>
            </telerik:RadPane>
        </telerik:RadSplitter>
    </div>
     <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>