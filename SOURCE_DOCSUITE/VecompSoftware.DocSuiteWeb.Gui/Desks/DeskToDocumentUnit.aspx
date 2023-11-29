<%@ Page Title="Tavoli -" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="DeskToDocumentUnit.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DeskToDocumentUnit" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="rsbContent" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            var deskToDocumentUnit;
            require(["Desks/DeskToDocumentUnit"], function (DeskToDocumentUnit) {
                $(function () {
                    deskToDocumentUnit = new DeskToDocumentUnit(tenantModelConfiguration.serviceConfiguration);
                    deskToDocumentUnit.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    deskToDocumentUnit.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    deskToDocumentUnit.pageContentId = "<%= pageContent.ClientID %>";
                    deskToDocumentUnit.rwmDocPreviewId = "<%= rwmDocPreview.ClientID %>";
                    deskToDocumentUnit.rblDocumentUnitId = "<%= rblDocumentUnit.ClientID %>";
                    deskToDocumentUnit.rblDocumentUnitUniqueId = "<%= rblDocumentUnit.UniqueID %>";
                    deskToDocumentUnit.dgvDocumentsId = "<%= dgvDocuments.ClientID %>";
                    deskToDocumentUnit.resolutionMainDocumentOmissisEnabled = <%= ResolutionMainDocumentOmissisEnabled.ToString().ToLower() %>;
                    deskToDocumentUnit.resolutionAttachmentsOmissisEnabled = <%= ResolutionAttachmentsOmissisEnabled.ToString().ToLower() %>;
                    deskToDocumentUnit.btnCancelId = "<%= btnCancel.ClientID %>";
                    deskToDocumentUnit.btnConfirmId = "<%= btnConfirm.ClientID %>";
                    deskToDocumentUnit.currentDeskId = "<%= CurrentDeskId %>";
                    deskToDocumentUnit.btnConfirmPostbackId = "<%= btnConfirmPostback.ClientID %>";
                    deskToDocumentUnit.ddlCollaborationTypeId = "<%= ddlCollaborationType.ClientID %>";
                    deskToDocumentUnit.pnlCollaborationTypeId = "<%= pnlCollaborationType.ClientID %>";
                    deskToDocumentUnit.rblSendToId = "<%= rblSendTo.ClientID %>";
                    deskToDocumentUnit.initialize();
                });
            });

            function ddlDocumentType_onClientItemSelected(sender, args) {
                deskToDocumentUnit.ddlDocumentType_onClientItemSelected(sender, args);
            }

            function dgvDocuments_onRowDeselect(sender, args) {
                deskToDocumentUnit.dgvDocuments_onRowDeselect(sender, args);
            }

            function ddlCollaborationType_onClientItemSelected(sender, args) {
                deskToDocumentUnit.ddlCollaborationType_onClientItemSelected(sender, args);
            }
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager DestroyOnClose="True" ReloadOnShow="True" ID="rwmDocPreview" runat="server">
        <Windows>
            <telerik:RadWindow Behaviors="Maximize,Close,Resize,Reload" DestroyOnClose="True" ID="windowPreviewDocument" ReloadOnShow="false" runat="server" Title="Anteprima documento" />
        </Windows>
    </telerik:RadWindowManager>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel runat="server" ID="pageContent">
        <telerik:RadPageLayout runat="server">
            <Rows>
                <telerik:LayoutRow HtmlTag="Div" Height="100%">
                    <Columns>
                        <telerik:LayoutColumn Span="12" CssClass="t-col-left-padding t-col-right-padding">
                            <asp:Panel runat="server">
                                <div class="dsw-panel" style="min-height: 100%;" id="mainContainer">
                                    <div class="dsw-panel-title">
                                        Dati tavolo
                                    </div>
                                    <div class="dsw-panel-content">
                                        <telerik:RadPageLayout runat="server" HtmlTag="Div" Width="100%">
                                            <Rows>
                                                <telerik:LayoutRow HtmlTag="Div">
                                                    <Columns>
                                                        <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                                            <b>Nome del tavolo:</b>
                                                        </telerik:LayoutColumn>
                                                        <telerik:LayoutColumn Span="10" CssClass="t-col-left-padding">
                                                            <asp:Label runat="server" ID="lblDeskName"></asp:Label>
                                                        </telerik:LayoutColumn>
                                                        <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                                            <b>Oggetto:</b>
                                                        </telerik:LayoutColumn>
                                                        <telerik:LayoutColumn Span="10" CssClass="t-col-left-padding">
                                                            <asp:Label runat="server" ID="lblDeskSubject"></asp:Label>
                                                        </telerik:LayoutColumn>
                                                        <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                                            <b>Data scadenza:</b>
                                                        </telerik:LayoutColumn>
                                                        <telerik:LayoutColumn Span="10" CssClass="t-col-left-padding">
                                                            <asp:Label runat="server" ID="lblExpireDate"></asp:Label>
                                                        </telerik:LayoutColumn>
                                                    </Columns>
                                                </telerik:LayoutRow>
                                            </Rows>
                                        </telerik:RadPageLayout>
                                    </div>
                                </div>
                            </asp:Panel>
                        </telerik:LayoutColumn>
                        <telerik:LayoutColumn Span="12" CssClass="t-col-left-padding t-col-right-padding">
                            <asp:Panel runat="server">
                                <div class="dsw-panel" style="min-height: 100%;">
                                    <div class="dsw-panel-title">
                                        Gestisci in
                                    </div>
                                    <div class="dsw-panel-content">
                                        <asp:RadioButtonList ID="rblDocumentUnit" CssClass="col-dsw-offset-1" runat="server">
                                            <asp:ListItem Text="Collaborazione" Value="0" Selected="True" />
                                            <asp:ListItem Text="Protocollo" Value="1" />
                                        </asp:RadioButtonList>
                                    </div>
                                </div>
                            </asp:Panel>
                        </telerik:LayoutColumn>
                        <telerik:LayoutColumn Span="12" CssClass="t-col-left-padding t-col-right-padding">
                            <asp:Panel runat="server" ID="pnlCollaborationType">
                                <div class="dsw-panel" style="min-height: 100%;">
                                    <div class="dsw-panel-title">
                                        Tipologia di collaborazione
                                    </div>
                                    <div class="dsw-panel-content">
                                        <telerik:RadPageLayout runat="server" HtmlTag="Div" Width="100%">
                                            <Rows>
                                                <telerik:LayoutRow HtmlTag="Div">
                                                    <Columns>
                                                        <telerik:LayoutColumn Span="2" CssClass="dsw-text-right">
                                                            <b>Tipologia documento:</b>
                                                        </telerik:LayoutColumn>
                                                        <telerik:LayoutColumn Span="10" CssClass="t-col-left-padding">
                                                            <telerik:RadDropDownList runat="server" ID="ddlCollaborationType" Width="300px" AutoPostBack="true" DefaultMessage="Seleziona una tipologia..." OnClientItemSelected="ddlCollaborationType_onClientItemSelected"></telerik:RadDropDownList>
                                                        </telerik:LayoutColumn>
                                                        <telerik:LayoutColumn Span="10" Offset="2" CssClass="t-col-left-padding">
                                                            <asp:RadioButtonList ID="rblSendTo" runat="server" Style="margin-top: 5px;">
                                                                <asp:ListItem Text="Inserisci alla visione/firma" Value="0" Selected="True" />
                                                                <asp:ListItem Text="Inserisci al protocollo" Value="1" />
                                                            </asp:RadioButtonList>
                                                        </telerik:LayoutColumn>
                                                    </Columns>
                                                </telerik:LayoutRow>
                                            </Rows>
                                        </telerik:RadPageLayout>
                                    </div>
                                </div>
                            </asp:Panel>
                        </telerik:LayoutColumn>
                        <telerik:LayoutColumn Span="12" CssClass="t-col-left-padding t-col-right-padding">
                            <asp:Panel runat="server">
                                <div class="dsw-panel" style="min-height: 100%;">
                                    <div class="dsw-panel-title">
                                        Documenti del tavolo
                                    </div>
                                    <div class="dsw-panel-content">
                                        <div class="radGridWrapper">
                                            <telerik:RadGrid runat="server" ID="dgvDocuments" Width="100%" AllowMultiRowSelection="true">
                                                <MasterTableView AutoGenerateColumns="False" DataKeyNames="BiblosSerializeKey">
                                                    <Columns>
                                                        <telerik:GridClientSelectColumn HeaderStyle-Width="16px" UniqueName="Select" />

                                                        <telerik:GridBoundColumn UniqueName="IsSigned" DataField="IsSigned" Display="false" />

                                                        <telerik:GridTemplateColumn UniqueName="RadioButtonTemplateColumn" HeaderStyle-Width="16px" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/documentPreview.png" HeaderText="Visualizza documento">
                                                            <ItemTemplate>
                                                                <asp:ImageButton ID="imgDocumentExtensionType" runat="server" />
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>

                                                        <telerik:GridTemplateColumn UniqueName="Name" HeaderText="Nome documento">
                                                            <ItemTemplate>
                                                                <asp:Label runat="server" ID="lblDocumentName"></asp:Label>
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>

                                                        <telerik:GridTemplateColumn AllowFiltering="False" HeaderText="Tipo documento" UniqueName="typeDoc">
                                                            <HeaderStyle Width="60%"></HeaderStyle>
                                                            <ItemTemplate>
                                                                <telerik:RadDropDownList runat="server" ID="ddlDocumentType" Width="200" AutoPostBack="False" OnClientItemSelected="ddlDocumentType_onClientItemSelected" />
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                                    </Columns>
                                                </MasterTableView>
                                                <ClientSettings EnableRowHoverStyle="False" Selecting-AllowRowSelect="true" Selecting-UseClientSelectColumnOnly="true" ClientEvents-OnRowDeselected="dgvDocuments_onRowDeselect" />
                                            </telerik:RadGrid>
                                        </div>
                                    </div>
                                </div>
                            </asp:Panel>
                        </telerik:LayoutColumn>
                    </Columns>
                </telerik:LayoutRow>
            </Rows>
        </telerik:RadPageLayout>
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" CssClass="footerButtons" ID="pnlButtons">
        <asp:Button runat="server" ID="btnConfirmPostback" Style="display: none;"></asp:Button>
        <telerik:RadButton runat="server" Width="170px" ID="btnConfirm" AutoPostBack="false" Text="Conferma"></telerik:RadButton>
        <telerik:RadButton runat="server" Width="150px" AutoPostBack="false" ID="btnCancel" Text="Annulla"></telerik:RadButton>
    </asp:Panel>
</asp:Content>
