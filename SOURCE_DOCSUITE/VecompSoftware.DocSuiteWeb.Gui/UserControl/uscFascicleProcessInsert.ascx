<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscFascicleProcessInsert.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscFascicleProcessInsert" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscCategoryRest.ascx" TagName="uscCategoryRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscContattiSelRest.ascx" TagName="uscContattiSelRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscRoleRest.ascx" TagName="uscRoleRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscMetadataRepositorySel.ascx" TagName="uscMetadataRepositorySel" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscDynamicMetadataRest.ascx" TagName="uscDynamicMetadataRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscCustomActionsRest.ascx" TagName="uscCustomActionsRest" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
    <script type="text/javascript">
        var uscFascicleProcessInsert;
        require(["UserControl/uscFascicleProcessInsert"], function (UscFascicleProcessInsert) {
            $(function () {
                uscFascicleProcessInsert = new UscFascicleProcessInsert(tenantModelConfiguration.serviceConfiguration);
                uscFascicleProcessInsert.clientId = "<%= ClientID %>";
                uscFascicleProcessInsert.pageId = "<%= pnlContent.ClientID %>";
                uscFascicleProcessInsert.pnlFascProcessInsertId = "<%= pnlFascProcessInsert.ClientID %>";
                uscFascicleProcessInsert.uscCategoryId = "<%= uscCategory.MainContent.ClientID %>";
                uscFascicleProcessInsert.txtObjectId = "<%= txtObject.ClientID%>";
                uscFascicleProcessInsert.uscMetadataRepositorySelId = "<%= uscMetadataRepositorySel.PageContentDiv.ClientID %>";
                uscFascicleProcessInsert.uscDynamicMetadataRestId = "<%= uscDynamicMetadataRest.PageContent.ClientID %>";
                uscFascicleProcessInsert.uscContactId = "<%= uscContact.PanelContent.ClientID %>";
                uscFascicleProcessInsert.uscRoleMasterId = "<%= uscRoleMaster.TableContentControl.ClientID %>";
                uscFascicleProcessInsert.uscRoleId = "<%= uscRole.TableContentControl.ClientID %>";
                uscFascicleProcessInsert.txtConservationId = "<%= txtConservation.ClientID %>";
                uscFascicleProcessInsert.txtNoteId = "<%= txtNote.ClientID %>";
                uscFascicleProcessInsert.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscFascicleProcessInsert.pnlContactId = "<%= pnlContact.ClientID %>";
                uscFascicleProcessInsert.pnlRoleMasterId = "<%= pnlRoleMaster.ClientID %>";
                uscFascicleProcessInsert.pnlRoleId = "<%= pnlRole.ClientID %>";
                uscFascicleProcessInsert.pnlConservationId = "<%= pnlConservation.ClientID %>";
                uscFascicleProcessInsert.rcbFascicleTypeId = "<%= rcbFascicleType.ClientID %>";
                uscFascicleProcessInsert.activityFascicleEnabled = <%=ProtocolEnv.ActivityFascicleEnabled.ToString().ToLower() %>;
                uscFascicleProcessInsert.uscCustomActionsRestId = "<%= uscCustomActionsRest.PageContent.ClientID %>";
                uscFascicleProcessInsert.initialize();
            });
        });

    </script>
</telerik:RadScriptBlock>

<asp:Panel runat="server" ID="pnlContent">
    <asp:Panel runat="server" ID="pnlTemplateSelection">
        <telerik:RadPageLayout runat="server" Width="100%" Height="100%" HtmlTag="Div">
            <Rows>
                <telerik:LayoutRow HtmlTag="Div" CssClass="dsw-panel-content">
                    <Rows>
                        <telerik:LayoutRow runat="server" HtmlTag="Div" Style="padding: 5px;">
                            <Columns>
                                <telerik:LayoutColumn Span="4" CssClass="dsw-text-right">
                                    <b>Tipologia di fascicolo</b>
                                </telerik:LayoutColumn>
                                <telerik:LayoutColumn Span="7" CssClass="t-col-left-padding">
                                    <telerik:RadComboBox runat="server" ID="rcbFascicleType" Width="300px" />
                                </telerik:LayoutColumn>
                            </Columns>
                        </telerik:LayoutRow>
                        <telerik:LayoutRow runat="server" HtmlTag="Div">
                            <Content>
                                <usc:uscCategoryRest runat="server" ID="uscCategory" ShowAuthorizedFascicolable="true" />
                            </Content>
                        </telerik:LayoutRow>
                    </Rows>
                </telerik:LayoutRow>
            </Rows>
        </telerik:RadPageLayout>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlFascProcessInsert">
        <telerik:RadPageLayout runat="server" ID="fasciclePageContent" Width="100%" Height="100%" HtmlTag="Div">
            <Rows>
                <telerik:LayoutRow runat="server" HtmlTag="Div" ID="pnlRoleMaster">
                    <Content>
                        <div class="dsw-panel">
                            <usc:uscRoleRest runat="server" ID="uscRoleMaster" Expanded="true" ReadOnlyMode="false" Caption="Settore responsabile" Required="true" OnlyMyRoles="true" Collapsable="true" RequiredMessage="Campo settore responsabile obbligatorio" FascicleVisibilityTypeButtonEnabled="false"  />
                        </div>
                    </Content>
                </telerik:LayoutRow>
                <telerik:LayoutRow runat="server" HtmlTag="Div" ID="pnlContact">
                    <Content>
                        <div class="dsw-panel">
                            <div class="dsw-panel-title">
                                Responsabile di procedimento
                            </div>
                            <usc:uscContattiSelRest runat="server" ID="uscContact" Required="true" ConfirmAndNewEnabled="False" CreateManualContactEnabled="False" />
                        </div>
                    </Content>
                </telerik:LayoutRow>
                <telerik:LayoutRow runat="server" HtmlTag="Div" ID="pnlRole">
                    <Content>
                        <div class="dsw-panel">
                            <usc:uscRoleRest runat="server" ID="uscRole" ReadOnlyMode="false" Expanded="true" MultipleRoles="true" Caption="Settori con autorizzazioni" Required="false" OnlyMyRoles="false" Collapsable="true" RequiredMessage="Campo settori con autorizzazioni obbligatorio" RACIButtonEnabled="true" FascicleVisibilityTypeButtonEnabled="true" />
                        </div>
                    </Content>
                </telerik:LayoutRow>
                <telerik:LayoutRow ID="rowGeneral">
                    <Content>
                        <div class="dsw-panel">
                            <div class="dsw-panel-title">
                                Dati fascicolo
                            </div>
                            <div class="dsw-panel-content">
                                <telerik:RadPageLayout runat="server" HtmlTag="Div">
                                    <Rows>
                                        <telerik:LayoutRow HtmlTag="Div" Style="margin-top: 2px;">
                                            <Columns>
                                                <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                                    <b>Oggetto:</b>
                                                </telerik:LayoutColumn>
                                                <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                    <telerik:RadTextBox ID="txtObject" TextMode="MultiLine" MaxLength="511" Rows="3" runat="server" Width="100%" />
                                                    <asp:RequiredFieldValidator ControlToValidate="txtObject" Display="Dynamic" ErrorMessage="Campo Oggetto Obbligatorio" ID="rfvObject" runat="server" />
                                                </telerik:LayoutColumn>
                                            </Columns>
                                        </telerik:LayoutRow>
                                        <telerik:LayoutRow HtmlTag="Div" Style="margin-top: 2px;">
                                            <Columns>
                                                <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                                    <b>Note:</b>
                                                </telerik:LayoutColumn>
                                                <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                    <telerik:RadTextBox ID="txtNote" runat="server" TextMode="MultiLine" Rows="2" Width="100%" />
                                                </telerik:LayoutColumn>
                                            </Columns>
                                        </telerik:LayoutRow>
                                        <telerik:LayoutRow HtmlTag="Div" Style="margin-top: 2px;" ID="pnlConservation">
                                            <Columns>
                                                <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                                    <b>Conservazione anni:</b>
                                                </telerik:LayoutColumn>
                                                <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                    <telerik:RadNumericTextBox ID="txtConservation" NumberFormat-DecimalDigits="0" runat="server" Width="50px" />
                                                    <label>0 (zero) per nessun limite</label>

                                                </telerik:LayoutColumn>
                                            </Columns>
                                        </telerik:LayoutRow>
                                    </Rows>
                                </telerik:RadPageLayout>
                            </div>
                        </div>
                    </Content>
                </telerik:LayoutRow>
                <telerik:LayoutRow runat="server" HtmlTag="Div" ID="metadataRepositoryRow">
                    <Content>
                        <div class="dsw-panel">
                            <div class="dsw-panel-title">
                                Metadati
                            </div>
                            <div class="dsw-panel-content">
                                <telerik:RadPageLayout runat="server" HtmlTag="Div" Width="100%">
                                    <Rows>
                                        <telerik:LayoutRow HtmlTag="Div">
                                            <Columns>
                                                <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                                    <b></b>
                                                </telerik:LayoutColumn>
                                                <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                    <usc:uscMetadataRepositorySel runat="server" ID="uscMetadataRepositorySel" Caption="Dati aggiuntivi" Required="False" UseSessionStorage="true" />
                                                </telerik:LayoutColumn>
                                            </Columns>
                                        </telerik:LayoutRow>
                                    </Rows>
                                </telerik:RadPageLayout>
                                <usc:uscDynamicMetadataRest runat="server" ID="uscDynamicMetadataRest" />
                            </div>
                        </div>
                    </Content>
                </telerik:LayoutRow>
                <telerik:LayoutRow runat="server" HtmlTag="Div" ID="customActionsRow">
                    <Content>
                        <div class="dsw-panel">
                            <div class="dsw-panel-title">
                                Azioni personalizzate
                            </div>
                            <div class="dsw-panel-content">
                                <usc:uscCustomActionsRest runat="server" ID="uscCustomActionsRest" IsFromInsertPage="true" />
                            </div>
                        </div>
                    </Content>
                </telerik:LayoutRow>
            </Rows>
        </telerik:RadPageLayout>
    </asp:Panel>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Panel>
