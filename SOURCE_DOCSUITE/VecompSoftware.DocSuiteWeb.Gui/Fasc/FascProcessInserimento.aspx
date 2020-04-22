<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="FascProcessInserimento.aspx.vb"
    Inherits="VecompSoftware.DocSuiteWeb.Gui.FascProcessInserimento"
    MasterPageFile="~/MasterPages/DocSuite2008.Master"
    Title="Fascicolo - Inserimento" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscOggetto.ascx" TagName="uscOggetto" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscCategoryRest.ascx" TagName="uscCategoryRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscContattiSelRest.ascx" TagName="uscContattiSelRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscRoleRest.ascx" TagName="uscRoleRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscMetadataRepositorySel.ascx" TagName="uscMetadataRepositorySel" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscDynamicMetadata.ascx" TagName="uscDynamicMetadata" TagPrefix="usc" %>


<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript">
            var fascInserimento;
            require(["Fasc/FascProcessInserimento"], function (FascProcessInserimento) {
                $(function () {
                    fascInserimento = new FascProcessInserimento(tenantModelConfiguration.serviceConfiguration);
                    fascInserimento.ddlProcessId = "<%= ddlProcess.ClientID %>";
                    fascInserimento.rtvProcessFoldersId = "<%=rtvProcessFolders.ClientID%>";
                    fascInserimento.ddlTemplateId = "<%= ddlTemplate.ClientID %>";
                    fascInserimento.rowTemplateId = "<%= rowTemplate.ClientID %>";
                    fascInserimento.rowDossierFoldersId = "<%= rowDossierFolders.ClientID %>";
                    fascInserimento.ajaxLoadingPanelId = "<%=MasterDocSuite.AjaxDefaultLoadingPanel.ClientId %>";
                    fascInserimento.pnlFascProcessInsertId = "<%= pnlFascProcessInsert.ClientID %>";
                    fascInserimento.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    fascInserimento.uscCategoryId = "<%= uscCategory.MainContent.ClientID %>";
                    fascInserimento.txtObjectId = "<%= txtObject.ClientID%>";
                    fascInserimento.uscMetadataRepositorySelId = "<%= uscMetadataRepositorySel.PageContentDiv.ClientID %>";
                    fascInserimento.uscDynamicMetadataId = "<%= uscDynamicMetadata.PageContentDiv.ClientID %>";
                    fascInserimento.uscContactId = "<%= uscContact.PanelContent.ClientID %>";
                    fascInserimento.uscRoleMasterId = "<%= uscRoleMaster.TableContentControl.ClientID %>";
                    fascInserimento.uscRoleId = "<%= uscRole.TableContentControl.ClientID %>";
                    fascInserimento.txtConservationId = "<%= txtConservation.ClientID %>";
                    fascInserimento.txtNoteId = "<%= txtNote.ClientID %>";
                    fascInserimento.radStartDateId = "<%= radStartDate.ClientID %>";
                    fascInserimento.btnInsertId = "<%= btnInsert.ClientID %>";
                    fascInserimento.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    fascInserimento.initialize();
                });
            });

        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel runat="server" ID="pnlFascProcessInsert">
        <telerik:RadPageLayout runat="server" ID="fasciclePageContent" Width="100%" Height="100%" HtmlTag="Div">
        <Rows>
            <telerik:LayoutRow HtmlTag="Div" CssClass="dsw-panel-content">
                <Rows>
                    <telerik:LayoutRow HtmlTag="Div" Style="margin-top: 2px;">
                        <Columns>
                            <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                <b>Serie:</b>
                            </telerik:LayoutColumn>
                            <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                <telerik:RadComboBox runat="server" ID="ddlProcess"></telerik:RadComboBox>
                            </telerik:LayoutColumn>
                        </Columns>
                    </telerik:LayoutRow>
                </Rows>
            </telerik:LayoutRow>
            <telerik:LayoutRow HtmlTag="Div" ID="rowDossierFolders" CssClass="dsw-panel-content">
                <Rows>
                    <telerik:LayoutRow HtmlTag="Div" Style="margin-top: 2px;">
                        <Columns>
                            <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                <b>Volume:</b>
                            </telerik:LayoutColumn>
                            <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                <telerik:RadTreeView runat="server" ID="rtvProcessFolders"></telerik:RadTreeView>
                            </telerik:LayoutColumn>
                        </Columns>
                    </telerik:LayoutRow>
                </Rows>
            </telerik:LayoutRow>            
            <telerik:LayoutRow HtmlTag="Div" ID="rowTemplate" CssClass="dsw-panel-content">
                <Rows>
                    <telerik:LayoutRow HtmlTag="Div" Style="margin-top: 2px;">
                        <Columns>
                            <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                <b>Template:</b>
                            </telerik:LayoutColumn>
                            <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                <telerik:RadComboBox runat="server" ID="ddlTemplate"></telerik:RadComboBox>
                            </telerik:LayoutColumn>
                        </Columns>
                    </telerik:LayoutRow>
                </Rows>
            </telerik:LayoutRow>
            <telerik:LayoutRow runat="server" HtmlTag="Div">
                <Content>
                    <usc:uscCategoryRest runat="server" ID="uscCategory" ShowAuthorizedFascicolable="true" />
                </Content>
            </telerik:LayoutRow>
            <telerik:LayoutRow runat="server" HtmlTag="Div">
                <Content>
                    <div class="dsw-panel">
                        <div class="dsw-panel-title">
                            Responsabile di procedimento
                        </div>
                        <usc:uscContattiSelRest runat="server" ID="uscContact" Required="true"/>
                    </div>
                </Content>
            </telerik:LayoutRow>
            <telerik:LayoutRow runat="server" HtmlTag="Div">
                <Content>                    
                    <div class="dsw-panel">
                        <usc:uscRoleRest runat="server" ID="uscRoleMaster" ReadOnlyMode="false" Caption="Settore Responsabile" Required="true" Collapsable="true" RequiredMessage="Campo Settore Responsabile obbligatorio" />
                    </div>
                </Content>
            </telerik:LayoutRow>
            <telerik:LayoutRow runat="server" HtmlTag="Div">
                <Content>
                    <div class="dsw-panel">
                        <usc:uscRoleRest runat="server" ID="uscRole" ReadOnlyMode="false" Expanded="true" MultipleRoles="true" Caption="Settori con Autorizzazioni" Required="true" Collapsable="true" RequiredMessage="Campo Settori con Autorizzazioni obbligatorio" />
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
        </Rows>
    </telerik:RadPageLayout>
    </asp:Panel>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnInsert" AutoPostBack="false" runat="server" CausesValidation="true" Width="150px" Text="Conferma inserimento" />
</asp:Content>
