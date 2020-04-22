<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscFascicolo.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscFascicolo" %>
<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="uscContatti" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="uscSettori" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscDynamicMetadata.ascx" TagName="uscDynamicMetadata" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscFascSummary.ascx" TagName="uscFascSummary" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscAuthorizations.ascx" TagName="uscAuthorizations" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscFascicleFolders.ascx" TagName="uscFascicleFolders" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscDocumentUnitReferences.ascx" TagName="uscDocumentUnitReferences" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">        
        var uscFascicolo;
        require(["UserControl/uscFascicolo"], function (UscFascicolo) {
            $(function () {
                uscFascicolo = new UscFascicolo(tenantModelConfiguration.serviceConfiguration);

                uscFascicolo.pageId = "<%= pageContent.ClientID %>";
                uscFascicolo.isEditPage = JSON.parse("<%= IsEditPage %>".toLowerCase());
                uscFascicolo.isAuthorizePage = JSON.parse("<%= IsAuthorizePage %>".toLowerCase());
                uscFascicolo.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                uscFascicolo.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscFascicolo.grdUDId = "<%= grdUD.ClientID %>";
                uscFascicolo.rowManagerId = "<%= rowManager.ClientID %>";
                uscFascicolo.rcbUDId = "<%= rcbUD.ClientID %>";
                uscFascicolo.rowRolesId = "<%= rowRoles.ClientID %>";
                uscFascicolo.rowAccountedRolesId = "<%= rowAccountedRoles.ClientID %>";
                uscFascicolo.pnlUDId = "<%= pnlFoldersAndUD.ClientID %>";
                uscFascicolo.rcbReferenceTypeId = "<%= rcbReferenceType.ClientID %>";
                uscFascicolo.txtTitleId = "<%= txtTitle.ClientID %>";
                uscFascicolo.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                uscFascicolo.btnExpandUDFascicleId = "<%= btnExpandUDFascicle.ClientID %>";
                uscFascicolo.UDFascicleGridId = "<%= UDFascicleGrid.ClientID %>";
                uscFascicolo.deliberaCaption = "<%= DeliberaCaption %>";
                uscFascicolo.determinaCaption = "<%= DeterminaCaption %>";
                uscFascicolo.metadataRepositoryEnabled = JSON.parse("<%= ProtocolEnv.MetadataRepositoryEnabled%>".toLowerCase());
                uscFascicolo.fasciclesPanelVisibilities = <%= FasciclesPanelVisibilities %>;
                uscFascicolo.workflowActivityId = "<%= CurrentWorkflowActivityId%>";
                uscFascicolo.rowDynamicMetadataId = "<%= rowDynamicMetadata.ClientID %>";
                uscFascicolo.btnExpandDynamicMetadataId = "<%= btnExpandDynamicMetadata.ClientID %>";
                uscFascicolo.dynamicMetadataContentId = "<%= dynamicMetadataContent.ClientID %>";
                uscFascicolo.pnlGrdSearchId = "<%= pnlGridSearch.ClientID %>"
                uscFascicolo.currentFascicleId = "<%= CurrentFascicleId %>"
                uscFascicolo.uscFascSummaryId = "<%= uscFascSummary.PageContentDiv.ClientID %>";
                uscFascicolo.uscFascFoldersId = "<%= uscFascicleFolders.PageContentDiv.ClientID %>";
                uscFascicolo.rwmDocPreviewId = "<%=rwmDocPreview.ClientID%>";
                uscFascicolo.lblUDGridTitleId = "<%= lblUDGridTitle.ClientID %>";
                uscFascicolo.rsPnlFoldersId = "<%=rsPnlFolders.ClientID%>";
                uscFascicolo.rszFolderId = "<%=rszFolder.ClientID%>";
                uscFascicolo.splitterId = "<%=rdSplitter.ClientID%>";
                uscFascicolo.uscSettoriAccountedId = "<%=uscSettoriAccounted.TableContentControl.ClientID %>";
                uscFascicolo.racUDDataSourceId = "<%= racUDDataSource.ClientID %>";
                uscFascicolo.initialize();

                new ResizeSensor($("#<%= pageContent.ClientID %>")[0], function () {
                    resizeUDDetails();
                });

                new ResizeSensor($("#<%= UDFascicleGrid.ClientID %>")[0], function () {
                    resizeUDDetails();
                });

                new ResizeSensor($("#<%= UscFascicleFolder.PageContentDiv.ClientID %>")[0], function () {
                    resizeUDDetails();
                });

                new ResizeSensor($("#<%= rdSplitter.ClientID %>")[0], function () {
                    resizeUDDetails();
                });

                $("#divContent").scroll(function () {
                    var panelPosition = $("#RAD_SLIDING_PANE_CONTENT_<%= rsPnlFolders.ClientID %>").offset();
                    uscFascicleFolders_refreshStickifyPosition(panelPosition);
                });

                $("#RAD_SLIDING_PANE_CONTENT_<%= rsPnlFolders.ClientID %>").scroll(function () {
                    var panelPosition = $(this).offset();
                    uscFascicleFolders_refreshStickifyPosition(panelPosition);
                });

                var panelPosition = $("#RAD_SLIDING_PANE_CONTENT_<%= rsPnlFolders.ClientID %>").offset();
                uscFascicleFolders_refreshStickifyPosition(panelPosition);
            });
        });

        function GridOnCommand(sender, args) {
            uscFascicolo.gridOnCommand(sender, args);
        }

        function RcbReferenceType_OnSelectedIndexChanged(sender, args) {
            uscFascicolo.rcbReferenceType_OnSelectedIndexChanged(sender, args);
        }

        function RcbUd_OnSelectedIndexChanged(sender, args) {
            uscFascicolo.rcbUd_OnSelectedIndexChanged(sender, args);
        }

        function TxtTitle_OnTextChanged(sender, args) {
            uscFascicolo.udSubject_OnKeyPressed(sender, args);
        }

        function resizeUDDetails() {
            var height = $(".page-content").height() - $("#<%= pnlFoldersAndUD.ClientID %>").position().top;
            var width = $("#<%= pageContent.ClientID %>").outerWidth();
            var paneGrid = $("#<%= grdUD.ClientID %>").height() + 32;
            var paneFolders = $("#<%= uscFascicleFolders.ClientID %>").height() + 32;
            if (paneGrid > height || paneFolders > height) {
                height = paneGrid > paneFolders ? paneGrid : paneFolders;
            }
            var splitter = $find("<%= rdSplitter.ClientID %>");
            splitter.resize(width, height);
            var panelPosition = $("#RAD_SLIDING_PANE_CONTENT_<%= rsPnlFolders.ClientID %>").offset();
            uscFascicleFolders_refreshStickifyPosition(panelPosition);
        }

        function ExpandCollapseFolders() {
            uscFascicolo.expandCollapseFolders();
        }

        function OnPaneDocked(sender, args) {
            var panelPosition = $("#RAD_SLIDING_PANE_CONTENT_<%= rsPnlFolders.ClientID %>").offset();
            uscFascicleFolders_refreshStickifyPosition(panelPosition);
            uscFascicleFolders_showStickifyControls();
        }

        function OnPaneUndocked(sender, args) {
            uscFascicleFolders_hideStickifyControls();
        }

        function SetMetadataSessionStorage(metadatas) {
            sessionStorage.setItem('DocumentMetadatas', metadatas);
        }
    </script>
</telerik:RadScriptBlock>

<telerik:RadWindowManager EnableViewState="False" ID="rwmDocPreview" runat="server">
    <Windows>
        <telerik:RadWindow Behaviors="Maximize,Close,Resize,Reload" DestroyOnClose="True" ID="windowPreviewDocument" ReloadOnShow="false" runat="server" Title="Gestione inserti" />
    </Windows>
</telerik:RadWindowManager>

<telerik:RadPageLayout runat="server" ID="pageContent" HtmlTag="Div" Width="100%">
    <Rows>
        <telerik:LayoutRow HtmlTag="Div">
            <Content>
                <usc:uscFascSummary ID="uscFascSummary" runat="server" />
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow ID="rowManager">
            <Content>
                <uc:uscContatti ID="uscResponsabili" ButtonDeleteVisible="false" ButtonImportVisible="false" ButtonSelectDomainVisible="false"
                    ButtonManualVisible="false" ButtonPropertiesVisible="true" ButtonSelectAdamVisible="false" ButtonSelectVisible="false"
                    Caption="Riferimento" EnableCompression="true" ButtonSelectOChartVisible="false" EnableCC="false" IsRequired="false" Multiple="true"
                    ProtType="true" runat="server" Type="Comm" />
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow HtmlTag="Div" ID="rowAuthorizations" runat="server">
            <Content>
                <usc:uscAuthorizations ID="uscAuthorizations" runat="server" />
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow ID="rowRoles" runat="server">
            <Content>
                <uc:uscSettori ID="uscSettori" ReadOnly="true" runat="server" Caption="Autorizzazioni" Visible="false" />
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow ID="rowDocumentUnitReference" runat="server">
            <Content>
                <usc:uscDocumentUnitReferences Visible="true" ID="uscDocumentUnitReferences" runat="server" ShowFasciclesLinks="true" ShowDossierLinks="true" />
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow ID="rowAccountedRoles" runat="server">
            <Content>
                <uc:uscSettori ID="uscSettoriAccounted" ReadOnly="false" runat="server" MultipleRoles="true" MultiSelect="true" Caption="Settori con Autorizzazione" Visible="false" />
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow ID="rowDynamicMetadata">
            <Content>
                <div class="dsw-panel">
                    <div class="dsw-panel-title">
                        Metadati
                        <telerik:RadButton ID="btnExpandDynamicMetadata" CssClass="dsw-vertical-middle" runat="server" Width="16px" Height="16px" Visible="true">
                            <Image EnableImageButton="true" />
                        </telerik:RadButton>
                    </div>
                    <div class="dsw-panel-content" runat="server" id="dynamicMetadataContent">
                        <usc:uscDynamicMetadata runat="server" ID="uscDynamicMetadata" Required="False" UseSessionStorage="true" />
                    </div>
                </div>
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow HtmlTag="Div" ID="pnlFoldersAndUD" runat="server">
            <Content>
                <telerik:RadSplitter runat="server" Width="100%" ID="rdSplitter">
                    <telerik:RadPane runat="server" Collapsed="false" Width="350" Height="100%" Scrolling="None" CssClass="hideRspSlideHeader paneFolders">
                        <telerik:RadSlidingZone runat="server" DockedPaneId="rsPnlFolders" Height="100%" ID="rszFolder" ClickToOpen="true">
                            <telerik:RadSlidingPane ID="rsPnlFolders" runat="server" Height="100%" Width="350" Title="Cartelle del fascicolo" DockOnOpen="true" OnClientDocked="OnPaneDocked" OnClientUndocked="OnPaneUndocked">
                                <telerik:RadPageLayout runat="server" ID="pnlFolders" HtmlTag="Div" Height="100%">
                                    <Rows>
                                        <telerik:LayoutRow runat="server">
                                            <Content>
                                                <usc:uscFascicleFolders ID="uscFascicleFolders" runat="server"></usc:uscFascicleFolders>
                                            </Content>
                                        </telerik:LayoutRow>
                                    </Rows>
                                </telerik:RadPageLayout>
                            </telerik:RadSlidingPane>
                        </telerik:RadSlidingZone>
                    </telerik:RadPane>
                    <telerik:RadSplitBar runat="server">
                    </telerik:RadSplitBar>
                    <telerik:RadPane runat="server" Collapsed="false" Width="100%">
                        <telerik:RadPageLayout runat="server" ID="pnlUD" Height="100%" HtmlTag="Div" Width="100%">
                            <Rows>
                                <telerik:LayoutRow runat="server">
                                    <Content>
                                        <telerik:RadClientDataSource runat="server" ID="racUDDataSource"></telerik:RadClientDataSource>
                                        <div class="dsw-panel">
                                            <div class="dsw-panel-title">
                                                <asp:Label runat="server" ID="lblUDGridTitle" Text="Documenti nel fascicolo"></asp:Label>
                                                <telerik:RadButton ID="btnExpandUDFascicle" CssClass="dsw-vertical-middle" runat="server" Width="16px" Height="16px" Visible="true">
                                                    <Image EnableImageButton="true" />
                                                </telerik:RadButton>
                                            </div>
                                            <div id="UDFascicleGrid" runat="server" class="dsw-panel-content">
                                                <div class="dsw-panel" id="pnlGridSearch" runat="server">
                                                    <div class="dsw-panel-content" style="width: 100%;">
                                                        <telerik:RadComboBox ID="rcbUD" Width="150px" EmptyMessage="-- Documenti" runat="server" />
                                                        <telerik:RadComboBox ID="rcbReferenceType" Width="100px" EmptyMessage="-- Tipologia" Style="margin-left: 10px;" runat="server">
                                                            <Items>
                                                                <telerik:RadComboBoxItem Text="" Value="" />
                                                                <telerik:RadComboBoxItem Text="Fascicolato" Value="0" />
                                                                <telerik:RadComboBoxItem Text="Riferimento" Value="1" />
                                                            </Items>
                                                        </telerik:RadComboBox>
                                                        <telerik:RadTextBox ID="txtTitle" Width="500px" Label="Oggetto:" CausesValidation="false" LabelWidth="55px" Style="margin-left: 10px" runat="server"></telerik:RadTextBox>
                                                    </div>
                                                </div>
                                                <telerik:RadGrid AllowSorting="True" Skin="Office2010Blue" AllowFilteringByColumn="true" AllowPaging="False"
                                                    AutoGenerateColumns="False" GridLines="none" ID="grdUD" runat="server" Width="100%" AllowMultiRowSelection="True">
                                                    <GroupingSettings ShowUnGroupButton="true">
                                                    </GroupingSettings>
                                                    <MasterTableView EnableGroupsExpandAll="True" NoDetailRecordsText="Nessuna documenti presente"
                                                        NoMasterRecordsText="Nessun documento presente" AllowFilteringByColumn="true" GroupLoadMode="Client" AllowMultiColumnSorting="true" AllowSorting="true">
                                                        <GroupByExpressions>
                                                            <telerik:GridGroupByExpression>
                                                                <SelectFields>
                                                                    <telerik:GridGroupByField FieldAlias=":" FieldName="DocumentUnitName"></telerik:GridGroupByField>
                                                                </SelectFields>
                                                                <GroupByFields>
                                                                    <telerik:GridGroupByField FieldName="DocumentUnitName" FieldAlias="DocumentUnitName"></telerik:GridGroupByField>
                                                                </GroupByFields>
                                                            </telerik:GridGroupByExpression>
                                                        </GroupByExpressions>
                                                        <Columns>
                                                            <telerik:GridBoundColumn DataField="UniqueId" ReadOnly="True" UniqueName="UniqueId" Display="False"></telerik:GridBoundColumn>
                                                            <telerik:GridBoundColumn DataField="DocumentUnitName" ReadOnly="True" UniqueName="DocumentUnitName" AllowFiltering="true" Display="False"></telerik:GridBoundColumn>
                                                            <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn"></telerik:GridClientSelectColumn>
                                                            <telerik:GridTemplateColumn HeaderImageUrl="~/App_Themes/DocSuite2008/imgset16/folder_document.png" HeaderStyle-HorizontalAlign="Center" HeaderTooltip="Fascicolazione"
                                                                ItemStyle-Width="16px" HeaderStyle-Width="16px" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle"
                                                                AllowFiltering="true" UniqueName="ReferenceType" AutoPostBackOnFilter="false">
                                                                <ItemTemplate>
                                                                    <asp:Image ID="imgReferenceType" runat="server" />
                                                                </ItemTemplate>
                                                                <FilterTemplate>
                                                                    <telerik:RadComboBox Visible="true" ID="rcbFilterReferenceType" Width="100px" EmptyMessage="-- Tipologia" Style="margin-left: 10px;" runat="server" OnClientSelectedIndexChanged="RcbReferenceType_OnSelectedIndexChanged">
                                                                        <Items>
                                                                            <telerik:RadComboBoxItem Text="" Value="" />
                                                                            <telerik:RadComboBoxItem Text="Fascicolato" Value="0" />
                                                                            <telerik:RadComboBoxItem Text="Riferimento" Value="1" />
                                                                        </Items>
                                                                    </telerik:RadComboBox>
                                                                </FilterTemplate>
                                                            </telerik:GridTemplateColumn>
                                                            <telerik:GridTemplateColumn HeaderImageUrl="~/App_Themes/DocSuite2008/imgset16/linked_folder.png" HeaderStyle-HorizontalAlign="Center" HeaderTooltip="Fascicolo in cui risulta fascicolato"
                                                                ItemStyle-Width="16px" HeaderStyle-Width="16px" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle"
                                                                AllowFiltering="false" UniqueName="UDFascicle" AutoPostBackOnFilter="false">
                                                                <ItemTemplate>
                                                                    <telerik:RadButton ButtonType="LinkButton" ID="imgUDFascicle" OnClientClicking="uscFascicolo.imgUDFascicle_OnClick" runat="server" Width="16px" Height="16px">
                                                                    </telerik:RadButton>
                                                                </ItemTemplate>
                                                            </telerik:GridTemplateColumn>
                                                            <telerik:GridTemplateColumn AllowFiltering="true" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="left" AutoPostBackOnFilter="false" HeaderStyle-Width="15%"
                                                                UniqueName="Title" HeaderText="Documenti" AllowSorting="true" SortExpression="Title">
                                                                <ItemTemplate>
                                                                    <telerik:RadButton ID="btnUDLink" ButtonType="LinkButton" OnClientClicking="uscFascicolo.btnUDLink_OnClick" runat="server"></telerik:RadButton>
                                                                </ItemTemplate>
                                                                <FilterTemplate>
                                                                    <telerik:RadDropDownList Visible="true" ID="rcbFilterUD" Width="150px" CssClass="udComboFilter" DefaultMessage="-- Documenti" runat="server"
                                                                        ClientDataSourceID="racUDDataSource" AutoPostBack="false" DataTextField="Name" DataValueField="Value" OnClientSelectedIndexChanged="RcbUd_OnSelectedIndexChanged" />
                                                                </FilterTemplate>
                                                            </telerik:GridTemplateColumn>
                                                            <telerik:GridTemplateColumn AllowFiltering="false" ItemStyle-Width="13%" ItemStyle-HorizontalAlign="center" AutoPostBackOnFilter="false" HeaderStyle-Width="13%"
                                                                UniqueName="UDRegistrationDate" HeaderText="Data registrazione" AllowSorting="true" SortExpression="RegistrationDate">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblUDRegistrationDate" runat="server"></asp:Label>
                                                                </ItemTemplate>
                                                            </telerik:GridTemplateColumn>
                                                            <telerik:GridTemplateColumn AllowFiltering="true" ItemStyle-Width="40%" AutoPostBackOnFilter="false" HeaderStyle-Width="40%"
                                                                UniqueName="UDObject" HeaderText="Oggetto/Note">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblUDObject" runat="server"></asp:Label>
                                                                </ItemTemplate>
                                                                <FilterTemplate>
                                                                    <telerik:RadTextBox ID="subjectFilter" Width="100%" runat="server" ClientEvents-OnKeyPress="TxtTitle_OnTextChanged"></telerik:RadTextBox>
                                                                </FilterTemplate>
                                                            </telerik:GridTemplateColumn>
                                                            <telerik:GridTemplateColumn AllowFiltering="false" ItemStyle-Width="40%" AutoPostBackOnFilter="false" HeaderStyle-Width="40%"
                                                                UniqueName="Title" HeaderText="Classificazione">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblCategory" runat="server" />
                                                                </ItemTemplate>
                                                            </telerik:GridTemplateColumn>
                                                        </Columns>
                                                    </MasterTableView>
                                                    <ClientSettings AllowGroupExpandCollapse="true" Selecting-AllowRowSelect="true" ClientEvents-OnCommand="GridOnCommand">
                                                    </ClientSettings>
                                                    <GroupingSettings ShowUnGroupButton="true"></GroupingSettings>
                                                </telerik:RadGrid>
                                            </div>
                                        </div>
                                    </Content>
                                </telerik:LayoutRow>
                            </Rows>
                        </telerik:RadPageLayout>
                    </telerik:RadPane>
                </telerik:RadSplitter>
            </Content>
        </telerik:LayoutRow>
    </Rows>
</telerik:RadPageLayout>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
