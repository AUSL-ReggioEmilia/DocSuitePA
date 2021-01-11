<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.FascModifica"
    CodeBehind="FascModifica.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master"
    Title="Fascicolo - Modifica" %>

<%@ Register Src="../UserControl/uscFascicolo.ascx" TagName="uscFascicolo" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="uscContattiSel" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscDynamicMetadataRest.ascx" TagName="uscDynamicMetadataRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscSetiContactSel.ascx" TagName="uscSetiContactSel" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscContattiSelRest.ascx" TagName="uscContattiSelRest" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var fascModifica;
            require(["Fasc/FascModifica"], function (FascModifica) {
                $(function () {
                    fascModifica = new FascModifica(tenantModelConfiguration.serviceConfiguration);
                    fascModifica.currentFascicleId = "<%= IdFascicle %>";
                    fascModifica.txtNameId = "<%= txtName.ClientID %>";
                    fascModifica.txtObjectId = "<%= txtObject.ClientID %>";
                    fascModifica.txtManagerId = "<%= txtManager.ClientID %>";
                    fascModifica.txtRackId = "<%= txtRack.ClientID %>";
                    fascModifica.txtNoteId = "<%= txtNote.ClientID %>";
                    fascModifica.rowRackId = "<%= rowRacks.ClientID %>";
                    fascModifica.rowNameId = "<%= rowName.ClientID %>";
                    fascModifica.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    fascModifica.btnConfermaId = "<%= btnConferma.ClientID %>";
                    fascModifica.radWindowManagerId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    fascModifica.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    fascModifica.pageContentId = "<%= pageContent.ClientID %>";
                    fascModifica.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    fascModifica.uscFascicoloId = "<%= uscFascicolo.PageContentDiv.ClientID %>";
                    fascModifica.rowManagerId = "<%= rowManager.ClientID %>";
                    fascModifica.rowLegacyManagerId = "<%= rowLegacyManager.ClientID %>";
                    fascModifica.fasciclesPanelVisibilities = <%=FasciclesPanelVisibilities%>;
                    fascModifica.rowDynamicMetadataId = "<%= rowDynamicMetadata.ClientID %>";
                    fascModifica.metadataRepositoryEnabled = JSON.parse("<%=ProtocolEnv.MetadataRepositoryEnabled%>".toLowerCase());
                    fascModifica.processEnabled = JSON.parse("<%=ProtocolEnv.ProcessEnabled%>".toLowerCase());
                    fascModifica.processPanelId = "<%= processPanel.ClientID %>";
                    fascModifica.ddlProcessId = "<%= processesDDL.ClientID %>";
                    fascModifica.rtvProcessFoldersId = "<%=rtvProcessFolders.ClientID%>";
                    fascModifica.uscSetiContactId = "<%= uscSetiContact.PageContentDiv.ClientID %>";
                    fascModifica.setiContactEnabledId = <%=ProtocolEnv.SETIIntegrationEnabled.ToString().ToLower()%>;
                    fascModifica.rowTransformIntoProcessFascicleId = "<%= rowTransformIntoProcessFascicle.ClientID %>";
                    fascModifica.chkTransformIntoProcessFascicleId = "<%= chkTransformIntoProcessFascicle.ClientID %>";
                    fascModifica.uscContactId = "<%= uscContact.PanelContent.ClientID %>";
                    fascModifica.uscContactDivId = "<%= uscContactDiv.ClientID %>";
                    fascModifica.uscDynamicMetadataId = "<%= UscDynamicMetadataRest.PageContent.ClientID%>";
                    fascModifica.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadPageLayout runat="server" ID="pageContent" HtmlTag="Div" Height="100%">
        <Rows>
            <telerik:LayoutRow runat="server" HtmlTag="Div">
                <Content>
                    <usc:uscFascicolo ID="uscFascicolo" runat="server" IsEditPage="true" />
                </Content>
            </telerik:LayoutRow>
            <telerik:LayoutRow ID="rowManager" runat="server" HtmlTag="Div">
                <Content>
                    <usc:uscContattiSel ID="uscContattiResp" ButtonImportVisible="false" ButtonManualVisible="false" ButtonSelectDomainVisible="false" FascicleContactEnabled="true"
                        ButtonPropertiesVisible="false" EnableCC="false" ForceAddressBook="true" ButtonSelectOChartVisible="false" HeaderVisible="true"
                        IsFiscalCodeRequired="true" Multiple="false" MultiSelect="false" runat="server"
                        ExcludeRoleRoot="true" Type="Prot" />
                </Content>
            </telerik:LayoutRow>
            <telerik:LayoutRow runat="server" HtmlTag="Div">
                <Content>
                    <div class="dsw-panel">
                        <div class="dsw-panel-title">
                            Altri
                        </div>
                        <div class="dsw-panel-content">
                            <telerik:RadPageLayout runat="server" HtmlTag="Div">
                                <Rows>
                                    <telerik:LayoutRow HtmlTag="Div" ID="rowName" Style="margin-top: 2px;" Visible="false">
                                        <Columns>
                                            <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                                <b>Nome:</b>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="10" CssClass="t-col-left-padding t-col-right-padding">
                                                <telerik:RadTextBox ID="txtName" runat="server" Width="100%"></telerik:RadTextBox>
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>
                                    <telerik:LayoutRow HtmlTag="Div" ID="rowSubject" Style="margin-top: 2px;">
                                        <Columns>
                                            <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                                <b>Oggetto:</b>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="10" CssClass="t-col-left-padding t-col-right-padding">
                                                <telerik:RadTextBox ID="txtObject" runat="server" Width="100%" TextMode="MultiLine" Rows="3"></telerik:RadTextBox>
                                                <asp:RequiredFieldValidator ID="rfvObject" runat="server" ControlToValidate="txtObject"
                                                    ErrorMessage="Campo Oggetto Obbligatorio" Display="Dynamic"></asp:RequiredFieldValidator>
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>
                                    <telerik:LayoutRow HtmlTag="Div" ID="rowLegacyManager" Style="margin-top: 2px;">
                                        <Columns>
                                            <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                                <b>Responsabile:</b>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="10" CssClass="t-col-left-padding t-col-right-padding">
                                                <telerik:RadTextBox ID="txtManager" runat="server" Width="100%"></telerik:RadTextBox>
                                                <asp:RequiredFieldValidator ID="rfvManager" runat="server" ControlToValidate="txtManager"
                                                    ErrorMessage="Campo Responsabile Obbligatorio" Display="Dynamic"></asp:RequiredFieldValidator>
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>
                                    <telerik:LayoutRow HtmlTag="Div" ID="rowRacks" Style="margin-top: 2px;">
                                        <Columns>
                                            <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                                <b>N. Scaffale:</b>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="10" CssClass="t-col-left-padding t-col-right-padding">
                                                <telerik:RadTextBox ID="txtRack" runat="server" Width="100%"></telerik:RadTextBox>
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>
                                    <telerik:LayoutRow HtmlTag="Div" ID="rowNote" Style="margin-top: 2px;">
                                        <Columns>
                                            <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                                <b>Note:</b>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="10" CssClass="t-col-left-padding t-col-right-padding">
                                                <telerik:RadTextBox ID="txtNote" runat="server" Width="100%"></telerik:RadTextBox>
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>
                                    <telerik:LayoutRow ID="processPanel" Style="margin-top: 2px;">
                                        <Content>
                                            <div class="dsw-panel">
                                                <div class="dsw-panel-title">
                                                    Serie e volume
                                                </div>
                                                <div class="dsw-panel-content" runat="server" id="processPanelContent">
                                                    <telerik:RadPageLayout runat="server" HtmlTag="Div">
                                                        <telerik:LayoutRow>
                                                            <Rows>
                                                                <telerik:LayoutRow HtmlTag="Div">
                                                                    <Columns>
                                                                        <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                                                            <b>Serie:</b>
                                                                        </telerik:LayoutColumn>
                                                                        <telerik:LayoutColumn Span="10" CssClass="t-col-left-padding">
                                                                            <telerik:RadComboBox runat="server"
                                                                                ID="processesDDL"
                                                                                AllowCustomText="false"
                                                                                CausesValidation="false"
                                                                                AutoPostBack="false">
                                                                            </telerik:RadComboBox>
                                                                        </telerik:LayoutColumn>
                                                                    </Columns>
                                                                </telerik:LayoutRow>
                                                            </Rows>
                                                        </telerik:LayoutRow>
                                                        <telerik:LayoutRow HtmlTag="Div">
                                                            <Rows>
                                                                <telerik:LayoutRow>
                                                                    <Columns>
                                                                        <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                                                            <b>Volume:</b>
                                                                        </telerik:LayoutColumn>
                                                                        <telerik:LayoutColumn Span="10" CssClass="t-col-left-padding">
                                                                            <telerik:RadTreeView runat="server" ID="rtvProcessFolders"></telerik:RadTreeView>
                                                                        </telerik:LayoutColumn>
                                                                    </Columns>
                                                                </telerik:LayoutRow>
                                                            </Rows>
                                                        </telerik:LayoutRow>
                                                    </telerik:RadPageLayout>
                                                </div>
                                            </div>
                                        </Content>
                                    </telerik:LayoutRow>
                                </Rows>
                            </telerik:RadPageLayout>
                        </div>
                    </div>
                </Content>
            </telerik:LayoutRow>
            <telerik:LayoutRow runat="server" HtmlTag="Div" ID="rowDynamicMetadata">
                <Content>
                    <div class="dsw-panel">
                        <div class="dsw-panel-title">
                            Metadati
                        </div>
                        <div class="dsw-panel-content">
                            <div style="margin-left: 16%;">
                                <usc:uscSetiContactSel runat="server" ID="uscSetiContact" />
                            </div>
                            <usc:uscDynamicMetadataRest runat="server" ID="UscDynamicMetadataRest"></usc:uscDynamicMetadataRest>
                        </div>
                    </div>
                </Content>
            </telerik:LayoutRow>
            <telerik:LayoutRow runat="server" HtmlTag="Div" ID="rowTransformIntoProcessFascicle">
                <Content>
                    <div class="dsw-panel">
                        <div class="dsw-panel-title">
                            Trasforma in fascicolo di procedimento
                        <input type="checkbox" runat="server" id="chkTransformIntoProcessFascicle" />
                        </div>
                        <div class="dsw-panel-content" runat="server" id="uscContactDiv">
                            <usc:uscContattiSelRest runat="server" ID="uscContact" Required="true" ConfirmAndNewEnabled="False" CreateManualContactEnabled="False" />
                        </div>
                    </div>
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnConferma" runat="server" Width="150px" Text="Conferma"></telerik:RadButton>
</asp:Content>
