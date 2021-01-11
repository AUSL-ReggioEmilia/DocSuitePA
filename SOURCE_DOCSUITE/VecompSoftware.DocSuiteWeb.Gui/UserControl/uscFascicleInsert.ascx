<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscFascicleInsert.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscFascicleInsert" %>

<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="uscContattiSel" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscOggetto.ascx" TagName="uscOggetto" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscCategoryRest.ascx" TagName="uscCategoryRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscRoleRest.ascx" TagName="uscRoleRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscMetadataRepositorySel.ascx" TagName="uscMetadataRepositorySel" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscDynamicMetadata.ascx" TagName="uscDynamicMetadata" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscCustomActionsRest.ascx" TagName="uscCustomActionsRest" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">        
        var uscFascicleInsert;
        require(["UserControl/uscFascicleInsert"], function (UscFascicleInsert) {
            $(function () {
                uscFascicleInsert = new UscFascicleInsert(tenantModelConfiguration.serviceConfiguration);
                uscFascicleInsert.clientId = "<%= ClientID %>";
                uscFascicleInsert.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                uscFascicleInsert.fasciclePageContentId = "<%= PageContentDiv.ClientID %>";
                uscFascicleInsert.fascicleDataRowId = "<%= fascicleDataRow.ClientID %>";
                uscFascicleInsert.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                uscFascicleInsert.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscFascicleInsert.currentUser = <%= CurrentUser %>;
                uscFascicleInsert.rdlFascicleTypeId = "<%= rdlFascicleType.ClientID %>";
                uscFascicleInsert.contattiRespRowId = "<%= contattiRespRow.ClientID %>";
                uscFascicleInsert.activityFascicleEnabled = JSON.parse("<%=ProtocolEnv.ActivityFascicleEnabled%>".toLowerCase());
                uscFascicleInsert.isMasterRowId = "<%= isMasterRow.ClientID %>";
                uscFascicleInsert.uscClassificatoreId = "<%= uscClassificatore.MainContent.ClientID%>";
                uscFascicleInsert.uscRoleMasterId = "<%= uscRoleMaster.TableContentControl.ClientID %>";
                uscFascicleInsert.uscRoleId = "<%= uscRole.TableContentControl.ClientID %>";
                uscFascicleInsert.uscContattiRespId = "<%= uscContattiResp.TableContent.ClientID%>";
                uscFascicleInsert.txtNoteId = "<%= txtNote.ClientID %>";
                uscFascicleInsert.radStartDateId = "<%= radStartDate.ClientID%>";
                uscFascicleInsert.txtConservationId = "<%= txtConservation.ClientID %>";
                uscFascicleInsert.uscOggettoId = "<%= uscObject.PanelControl.ClientID %>";
                uscFascicleInsert.pnlConservationId = "<%= pnlConservation.ClientID%>";
                uscFascicleInsert.fasciclesPanelVisibilities = <%=FasciclesPanelVisibilities%>;
                uscFascicleInsert.rowStartDateId = "<%= rowStartDate.ClientID %>";
                uscFascicleInsert.rfvConservationId = "<%= rfvConservation.ClientID%>";
                uscFascicleInsert.fascicleTypologyRowId = "<%= fascicleTypologyRow.ClientID%>";
                uscFascicleInsert.metadataRepositoryEnabled = JSON.parse("<%=ProtocolEnv.MetadataRepositoryEnabled%>".toLowerCase());
                uscFascicleInsert.metadataRepositoryRowId = "<%= metadataRepositoryRow.ClientID%>";
                uscFascicleInsert.uscMetadataRepositorySelId = "<%= uscMetadataRepositorySel.PageContentDiv.ClientID %>";
                uscFascicleInsert.uscDynamicMetadataId = "<%= uscDynamicMetadata.PageContentDiv.ClientID %>";
                uscFascicleInsert.containerRowId = "<%= containerRow.ClientID %>";
                uscFascicleInsert.ddlContainerId = "<%= ddlContainer.ClientID %>";
                uscFascicleInsert.fascicleContainerEnabled = <%= ProtocolEnv.FascicleContainerEnabled.ToString().ToLower() %>;
                uscFascicleInsert.rfvContainerId = "<%= rfvContainer.ClientID %>";
                uscFascicleInsert.uscCustomActionsRestId = "<%= uscCustomActionsRest.PageContent.ClientID %>";

                uscFascicleInsert.initialize();
            });
        });
    </script>
</telerik:RadScriptBlock>

<telerik:RadPageLayout runat="server" ID="fasciclePageContent" HtmlTag="Div" Width="100%">
    <Rows>
        <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="dsw-panel-content" ID="fascicleTypologyRow">
            <Content>
                <table class="datatable">
                    <tr>
                        <td class="label" style="width: 25%;">Tipologia di fascicolo:</td>
                        <td>
                            <telerik:RadDropDownList runat="server" ID="rdlFascicleType" Width="200px" AutoPostBack="false" selected="true">
                                <Items>
                                    <telerik:DropDownListItem Text="" Value="" />
                                    <telerik:DropDownListItem Text="Fascicolo di attività" Value="4" />
                                    <telerik:DropDownListItem Text="Fascicolo di procedimento" Value="1" />
                                </Items>
                            </telerik:RadDropDownList>
                        </td>
                    </tr>
                </table>
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow runat="server" ID="fascicleDataRow" Style="display: none">
            <Rows>
                <telerik:LayoutRow runat="server" HtmlTag="Div">
                    <Content>
                        <usc:uscCategoryRest runat="server" ID="uscClassificatore" ShowAuthorizedFascicolable="true" />
                    </Content>
                </telerik:LayoutRow>
                <telerik:LayoutRow runat="server" HtmlTag="Div" ID="containerRow">
                    <Content>
                        <div class="dsw-panel">
                            <div class="dsw-panel-title">Contenitore</div>
                            <div class="dsw-panel-content">
                                <telerik:RadPageLayout runat="server" HtmlTag="Div" Width="100%">
                                    <Rows>
                                        <telerik:LayoutRow HtmlTag="Div">
                                            <Columns>
                                                <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                    <telerik:RadDropDownList runat="server" ID="ddlContainer" DefaultMessage="--Seleziona un contenitore" Width="350px" Style="margin-bottom: 2px;"></telerik:RadDropDownList>
                                                    <asp:RequiredFieldValidator runat="server" ID="rfvContainer" ControlToValidate="ddlContainer" ErrorMessage="Campo contenitore obbligatorio" Display="Dynamic"></asp:RequiredFieldValidator>
                                                </telerik:LayoutColumn>
                                            </Columns>
                                        </telerik:LayoutRow>
                                    </Rows>
                                </telerik:RadPageLayout>
                            </div>
                        </div>
                    </Content>
                </telerik:LayoutRow>
                <telerik:LayoutRow runat="server" HtmlTag="Div" ID="contattiRespRow">
                    <Content>
                        <usc:uscContattiSel ID="uscContattiResp" ButtonImportVisible="false" ButtonManualVisible="false" ButtonSelectDomainVisible="false" FascicleContactEnabled="true"
                            ButtonPropertiesVisible="false" EnableCC="false" ForceAddressBook="true" ButtonSelectOChartVisible="false" HeaderVisible="true" IsFiscalCodeRequired="true"
                            Multiple="false" MultiSelect="false" runat="server" Type="Prot" ExcludeRoleRoot="true" />
                    </Content>
                </telerik:LayoutRow>
                <telerik:LayoutRow runat="server" HtmlTag="Div" ID="isMasterRow">
                    <Content>
                        <usc:uscRoleRest runat="server" ID="uscRoleMaster" Expanded="true" ReadOnlyMode="false" Caption="Settore responsabile" Required="true" OnlyMyRoles="true" Collapsable="true" RequiredMessage="Campo settore responsabile obbligatorio" FascicleVisibilityTypeButtonEnabled="true" />
                    </Content>
                </telerik:LayoutRow>
                <telerik:LayoutRow runat="server" HtmlTag="Div">
                    <Content>
                        <usc:uscRoleRest runat="server" ID="uscRole" ReadOnlyMode="false" Expanded="true" MultipleRoles="true" Caption="Settori con autorizzazioni" Required="false" OnlyMyRoles="false" Collapsable="true" RequiredMessage="Campo settori con autorizzazioni obbligatorio" RACIButtonEnabled="true" />
                    </Content>
                </telerik:LayoutRow>

                <telerik:LayoutRow ID="rowGeneral">
                    <Content>
                        <div class="dsw-panel">
                            <div class="dsw-panel-title">
                                Dati Fascicolo
                            </div>
                            <div class="dsw-panel-content">
                                <telerik:RadPageLayout runat="server" HtmlTag="Div">
                                    <Rows>
                                        <telerik:LayoutRow HtmlTag="Div" Style="margin-top: 2px;">
                                            <Columns>
                                                <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                                    <b>Oggetto:</b>
                                                </telerik:LayoutColumn>
                                                <telerik:LayoutColumn Span="10" CssClass="t-col-left-padding t-col-right-padding">
                                                    <usc:uscOggetto ID="uscObject" Required="true" RequiredMessage="Campo Oggetto Obbligatorio" MultiLine="True" runat="server" Type="Prot" />
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
                                        <telerik:LayoutRow HtmlTag="Div" Style="margin-top: 2px;" ID="rowStartDate">
                                            <Columns>
                                                <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                                    <b>Data apertura:</b>
                                                </telerik:LayoutColumn>
                                                <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                    <telerik:RadDatePicker ID="radStartDate" runat="server"></telerik:RadDatePicker>
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
                                                    <asp:RequiredFieldValidator ControlToValidate="txtConservation" Display="Dynamic" ErrorMessage="Campo Conservazione Obbligatorio" ID="rfvConservation" runat="server" />
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
                                <usc:uscDynamicMetadata runat="server" ID="uscDynamicMetadata" Required="False" UseSessionStorage="true" />
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
        </telerik:LayoutRow>
    </Rows>
</telerik:RadPageLayout>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
