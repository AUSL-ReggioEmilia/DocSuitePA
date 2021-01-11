<%@ Page Title="Gestione fascicolo" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="FascUDManager.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.FascUDManager" %>

<%@ Register Src="~/UserControl/uscFascicleSearch.ascx" TagName="uscFascicleSearch" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var fascUDManager;
            require(["Fasc/FascUDManager"], function (FascUDManager) {
                $(function () {
                    fascUDManager = new FascUDManager(tenantModelConfiguration.serviceConfiguration);
                    fascUDManager.documentUnitUniqueId = "<%= CurrentUDUniqueId %>";
                    fascUDManager.documentUnitRepositoryName = "<%= CurrentUDRepositoryName %>";
                    fascUDManager.documentUnitType = <%= UDTypeId %>;
                    fascUDManager.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    fascUDManager.pageContentId = "<%= pageContent.ClientID %>";
                    fascUDManager.lblUDSelectedId = "<%= lblUDSelected.ClientID %>";
                    fascUDManager.lblUDTitleId = "<%= lblUDTitle.ClientID %>";
                    fascUDManager.lblDocumentUnitTypeId = "<%= lblUDType.ClientID %>";
                    fascUDManager.lblContainerId = "<%= lblContainer.ClientID %>";
                    fascUDManager.lblObjectId = "<%= lblObject.ClientID %>";
                    fascUDManager.lblCategoryId = "<%= lblCategory.ClientID %>";
                    fascUDManager.signalRServerAddress = "<%= SignalRAddress %>";
                    fascUDManager.rgvAssociatedFasciclesId = "<%= rgvAssociatedFascicles.ClientID %>";
                    fascUDManager.btnInsertId = "<%= btnInsert.ClientID %>";
                    fascUDManager.btnRemoveId = "<%= btnRemove.ClientID %>";
                    fascUDManager.btnNewFascicleId = "<%= btnNew.ClientID %>";
                    fascUDManager.btnCategoryChangeId = "<%= btnCategoryChange.ClientID %>";
                    fascUDManager.validationModel = <%= ValidationModel %>;
                    fascUDManager.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    fascUDManager.radWindowManagerId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    fascUDManager.rowContainerId = "<%= rowContainer.ClientID %>";
                    fascUDManager.currentIdUDSRepository = "<%= CurrentIdUDSRepository %>";
                    fascUDManager.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    fascUDManager.availableFascicleRowId = "<%= availableFascicleRow.ClientID %>";
                    fascUDManager.maxNumberElements = "<%= ProtocolEnv.MaxNumberDropdownElements %>";
                    fascUDManager.currentSelectedFascicle = "<%= CurrentFascicle %>";
                    fascUDManager.uscFascicleSearchId = "<%= uscFascicleSearch.PageControl.ClientID %>";
                    fascUDManager.processEnabled = <%= ProtocolEnv.ProcessEnabled.ToString().ToLower() %>;
                    fascUDManager.folderSelectionEnabled = <%= FolderSelectionEnabled.ToString().ToLower() %>;
                    fascUDManager.initialize();
                });
            });
            function closeSearchFasciclesWindow(sender, args) {
                fascUDManager.closeSearchFasciclesWindow(sender, args);
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadFormDecorator RenderMode="Lightweight" ID="frmDecorator" EnableRoundedCorners="false" runat="server" DecoratedControls="Fieldset"></telerik:RadFormDecorator>
    <telerik:RadPageLayout ID="pageContent" HtmlTag="Div" Width="100%" Height="100%" runat="server">
        <Rows>
            <telerik:LayoutRow HtmlTag="Div">
                <Content>
                    <table class="datatable">
                        <tr>
                            <th colspan="2">
                                <asp:Label ID="lblUDSelected" runat="server"></asp:Label></th>
                        </tr>
                        <tr>
                            <td>
                                <telerik:RadPageLayout runat="server" HtmlTag="Div">
                                    <Rows>
                                        <telerik:LayoutRow HtmlTag="Div">
                                            <Columns>
                                                <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                    <b>
                                                        <asp:Label ID="lblUDType" runat="server" /></b>
                                                </telerik:LayoutColumn>
                                                <telerik:LayoutColumn Span="9">
                                                    <asp:Label ID="lblUDTitle" runat="server" />
                                                </telerik:LayoutColumn>
                                            </Columns>
                                        </telerik:LayoutRow>
                                        <telerik:LayoutRow HtmlTag="Div" ID="rowContainer">
                                            <Columns>
                                                <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                    <b>Contenitore:</b>
                                                </telerik:LayoutColumn>
                                                <telerik:LayoutColumn Span="9">
                                                    <asp:Label ID="lblContainer" runat="server" />
                                                </telerik:LayoutColumn>
                                            </Columns>
                                        </telerik:LayoutRow>
                                        <telerik:LayoutRow HtmlTag="Div">
                                            <Columns>
                                                <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                    <b>Oggetto:</b>
                                                </telerik:LayoutColumn>
                                                <telerik:LayoutColumn Span="9">
                                                    <asp:Label ID="lblObject" runat="server" />
                                                </telerik:LayoutColumn>
                                            </Columns>
                                        </telerik:LayoutRow>
                                        <telerik:LayoutRow HtmlTag="Div">
                                            <Columns>
                                                <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                    <b>Classificazione:</b>
                                                </telerik:LayoutColumn>
                                                <telerik:LayoutColumn Span="9">
                                                    <asp:Label ID="lblCategory" runat="server" />
                                                </telerik:LayoutColumn>
                                            </Columns>
                                        </telerik:LayoutRow>
                                    </Rows>
                                </telerik:RadPageLayout>
                            </td>
                        </tr>
                    </table>
                </Content>
            </telerik:LayoutRow>
            <telerik:LayoutRow runat="server" ID="availableFascicleRow" HtmlTag="Div">
                <Content>
                    <usc:uscFascicleSearch runat="server" ID="uscFascicleSearch"></usc:uscFascicleSearch>
                </Content>
            </telerik:LayoutRow>
            <telerik:LayoutRow HtmlTag="Div">
                <Content>
                    <table class="datatable">
                        <tr>
                            <th>Fascicoli associati</th>
                        </tr>
                    </table>
                    <div style="width: 100%;">
                        <telerik:RadGrid runat="server" ID="rgvAssociatedFascicles" AllowAutomaticUpdates="True" GridLines="None" AllowPaging="False" Skin="Office2010Blue" AllowMultiRowSelection="false" AllowFilteringByColumn="False">
                            <MasterTableView CommandItemDisplay="None" AutoGenerateColumns="False" DataKeyNames="UniqueId" NoMasterRecordsText="Nessun fascicolo collegato" NoDetailRecordsText="Nessun fascicolo collegato">
                                <Columns>
                                    <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn"></telerik:GridClientSelectColumn>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="20px" HeaderText="Stato" AllowFiltering="false" UniqueName="colOpenClose">
                                        <FilterTemplate>
                                            <telerik:RadComboBox runat="server" ID="cmbOpenClose" DataTextField="Text" DataValueField="Value" Width="100%" AutoPostBack="True">
                                            </telerik:RadComboBox>
                                        </FilterTemplate>
                                        <ClientItemTemplate>
                                            <span class="fascicleGridSpan">
                                                 <center>
                                                    <img src="#= ImageUrl #" title="#= OpenCloseTooltip #" align="middle"></img>
                                                 </center>
                                            </span>
                                        </ClientItemTemplate>
                                    </telerik:GridTemplateColumn>

                                    <telerik:GridTemplateColumn HeaderStyle-Width="25px" HeaderText="Collegamento" AllowFiltering="false" UniqueName="colReference">
                                        <FilterTemplate>
                                            <telerik:RadComboBox runat="server" ID="cmbReference" DataTextField="Text" DataValueField="Value" Width="100%" AutoPostBack="True">
                                            </telerik:RadComboBox>
                                        </FilterTemplate>
                                        <ClientItemTemplate>
                                            <span class="fascicleGridSpan">
                                                <center>
                                                    <img src="#= ReferenceImageUrl #" title="#= ReferenceTooltip #" ></img>
                                                </center>
                                            </span>
                                        </ClientItemTemplate>
                                    </telerik:GridTemplateColumn>

                                    <telerik:GridTemplateColumn HeaderStyle-Width="100%" HeaderText="Fascicolo" AllowFiltering="false">
                                        <FilterTemplate>
                                            <telerik:RadTextBox runat="server" ID="txtObject" DataTextField="Text" DataValueField="Value" Width="30%" AutoPostBack="True">
                                            </telerik:RadTextBox>
                                        </FilterTemplate>
                                        <ClientItemTemplate>
                                            <span class="fascicleGridSpan">
                                                <a href="../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=#= UniqueId #" class="ctrl">#= Name #</a>
                                            </span>
                                        </ClientItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                            </MasterTableView>
                            <ClientSettings EnableRowHoverStyle="False">
                                <Selecting AllowRowSelect="True" EnableDragToSelectRows="False" UseClientSelectColumnOnly="True"></Selecting>
                            </ClientSettings>
                        </telerik:RadGrid>
                    </div>
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server">
    <telerik:RadButton ID="btnInsert" runat="server" Text="Inserisci"></telerik:RadButton>
    <telerik:RadButton ID="btnRemove" runat="server" Text="Rimuovi"></telerik:RadButton>
    <telerik:RadButton ID="btnNew" runat="server" Width="100px" Text="Nuovo fascicolo"></telerik:RadButton>
    <telerik:RadButton ID="btnCategoryChange" runat="server" Width="150px" Text="Cambia classificazione"></telerik:RadButton>
</asp:Content>
