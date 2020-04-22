<%@ Page Title="Dossier - Inserimento" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="DossierInserimento.aspx.vb"
    Inherits="VecompSoftware.DocSuiteWeb.Gui.DossierInserimento" %>

<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="uscSettori" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="uscContattiSel" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscOggetto.ascx" TagName="uscOggetto" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscDynamicMetadataClient.ascx" TagName="uscDynamicMetadataClient" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscMetadataRepositorySel.ascx" TagName="uscMetadataRepositorySel" TagPrefix="usc" %>


<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript">
            var dossierInserimento;
            require(["Dossiers/DossierInserimento"], function (DossierInserimento) {
                $(function () {
                    dossierInserimento = new DossierInserimento(tenantModelConfiguration.serviceConfiguration);
                    dossierInserimento.dossierPageContentId = "<%= dossierPageContent.ClientID %>";
                    dossierInserimento.btnConfirmId = "<%= btnInserimento.ClientID %>";
                    dossierInserimento.btnConfirmUniqueId = "<%= btnInserimento.UniqueID %>";
                    dossierInserimento.txtObjectId = "<%= uscObject.TextBoxControl.ClientID %>";
                    dossierInserimento.txtNoteId = "<%= txtNote.ClientID %>";
                    dossierInserimento.rdpStartDateId = "<%=rdpStartDate.ClientID %>";
                    dossierInserimento.rfvStartDateId = "<%= rfvStartDate.ClientID %>";
                    dossierInserimento.rdlContainerId = "<%= rdlContainer.ClientID %>";
                    dossierInserimento.rfvContainerId = "<%= rfvContainer.ClientID %>";
                    dossierInserimento.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    dossierInserimento.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    dossierInserimento.radWindowManagerId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    dossierInserimento.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    dossierInserimento.uscDynamicMetadataId = "<%= uscDynamicMetadataClient.PageContent.ClientID%>";
                    dossierInserimento.uscMetadataRepositorySelId = "<%= uscMetadataRepositorySel.PageContentDiv.ClientID %>";
                    dossierInserimento.metadataRepositoryEnabled = JSON.parse("<%=ProtocolEnv.MetadataRepositoryEnabled%>".toLowerCase());
                    dossierInserimento.rowMetadataId = "<%= rowMetadata.ClientID%>";
                    dossierInserimento.initialize();
                });
            });

        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadPageLayout runat="server" ID="dossierPageContent" Width="100%" Height="100%" HtmlTag="Div">
        <Rows>
            <%-- Sezione Contenitore --%>
            <telerik:LayoutRow>
                <Content>
                    <div class="dsw-panel">
                        <div class="dsw-panel-title">
                            Contenitore
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
                                                <telerik:RadDropDownList runat="server" ID="rdlContainer"  Width="300px" DropDownHeight="200px" AutoPostBack="false" selected="true" CausesValidation="false" />
                                                <asp:RequiredFieldValidator ControlToValidate="rdlContainer" Display="Dynamic" ErrorMessage="Campo contenitore obbligatorio" ID="rfvContainer" runat="server" />
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>
                                </Rows>
                            </telerik:RadPageLayout>
                        </div>
                    </div>
                </Content>
            </telerik:LayoutRow>
            <%-- Sezione Riferimento --%>
            <telerik:LayoutRow ID="contattiRespRow" HtmlTag="Div">
                <Content>
                    <usc:uscContattiSel ButtonImportVisible="false" ButtonManualVisible="false" ButtonSelectDomainVisible="false" IsRequired="false"
                        ButtonPropertiesVisible="false" Caption="Riferimenti" EnableCC="false" ForceAddressBook="true" ButtonSelectOChartVisible="false" HeaderVisible="true"
                        ID="uscContattiSel" IsFiscalCodeRequired="false" Multiple="true" MultiSelect="True" runat="server" ExcludeRoleRoot="true" Type="Dossier" />
                </Content>
            </telerik:LayoutRow>
            <%-- Sezione Settore --%>
            <telerik:LayoutRow ID="rowRoles" HtmlTag="Div">
                <Content>
                    <usc:uscSettori runat="server" ID="uscSettori" Caption="Autorizzazioni" Required="true" />
                </Content>
            </telerik:LayoutRow>
            <%-- Sezione dati Dossier--%>
            <telerik:LayoutRow ID="rowGeneral">
                <Content>
                    <div class="dsw-panel">
                        <div class="dsw-panel-title">
                            Dati
                        </div>
                        <div class="dsw-panel-content">
                            <telerik:RadPageLayout runat="server" HtmlTag="Div" ID="contents">
                                <Rows>
                                    <telerik:LayoutRow HtmlTag="Div">
                                        <Columns>
                                            <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                                <b>Oggetto:</b>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="10" CssClass="t-col-left-padding t-col-right-padding">
                                                <usc:uscOggetto ID="uscObject" Required="True" MultiLine="True" RequiredMessage="Campo Oggetto Obbligatorio" runat="server" Type="Dossier" />
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>
                                    <telerik:LayoutRow HtmlTag="Div" Style="margin-top: 2px;">
                                        <Columns>
                                            <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                                <b>Note:</b>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="10" CssClass="t-col-left-padding t-col-right-padding">
                                                <telerik:RadTextBox ID="txtNote" runat="server" Width="100%" />
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>
                                    <telerik:LayoutRow HtmlTag="Div" Style="margin-top: 2px;">
                                        <Columns>
                                            <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                                <b>Data Apertura:</b>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                <telerik:RadDatePicker ID="rdpStartDate" runat="server" ClientIDMode="AutoID" />
                                                <asp:RequiredFieldValidator ControlToValidate="rdpStartDate" Display="Dynamic" ErrorMessage="Data apertura obbligatoria" ID="rfvStartDate" runat="server" />
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>
                                </Rows>
                            </telerik:RadPageLayout>
                        </div>
                    </div>
                </Content>
            </telerik:LayoutRow>
            <telerik:LayoutRow ID="rowMetadata" runat="server">
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
                                                <usc:uscMetadataRepositorySel runat="server" ID="uscMetadataRepositorySel" Caption="Metadati dinamici" Required="False" UseSessionStorage="true" />
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>
                                </Rows>
                            </telerik:RadPageLayout>
                            <usc:uscDynamicMetadataClient runat="server" ID="uscDynamicMetadataClient"></usc:uscDynamicMetadataClient>
                        </div>
                    </div>
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>

    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>


</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnInserimento" runat="server" CausesValidation="true" Width="150px" Text="Conferma inserimento" />
</asp:Content>
