<%@ Page Title="Inserimento nuovo Template di Protocollo" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="TemplateProtocolInsert.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TemplateProtocolInsert" %>

<%@ Register Src="~/UserControl/uscClassificatore.ascx" TagPrefix="usc" TagName="SelCategory" %>
<%@ Register Src="~/UserControl/uscSettori.ascx" TagPrefix="usc" TagName="Settori" %>
<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagPrefix="usc" TagName="SelContatti" %>
<%@ Register Src="~/UserControl/uscOggetto.ascx" TagPrefix="usc" TagName="SelOggetto" %>
<%@ Register Src="~/UserControl/uscServiceCategory.ascx" TagPrefix="usc" TagName="SelServiceCategory" %>
<%@ Register Src="~/UserControl/uscContattiSelText.ascx" TagPrefix="usc" TagName="SelContattiTesto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock">
        <script type="text/javascript" language="javascript">
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow() {
                var oWindow = GetRadWindow();
                oWindow.close();
            }

            function OnRequestStart(sender, args) {
                ShowLoadingPanel();
            }

            function OnResponseEnd(sender, args) {
                HideLoadingPanel();
            }

            function TemplateProtocolAjaxRequest(val) {
                $find("<%= AjaxManager.ClientID %>").ajaxRequest(val);
                return false;
            }

            function <%= Me.ID %>_CloseContactFunction(sender, args) {
                sender.remove_close(<%= Me.ID %>_CloseContactFunction);
                if (args.get_argument() !== null) {
                    TemplateProtocolAjaxRequest("TextMode" + "|" + args.get_argument());
                }
            }

            function ShowLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= protocolContainer.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);
            }

            function HideLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= protocolContainer.ClientID%>";
                currentLoadingPanel.hide(currentUpdatedControl);
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel runat="server" ID="protocolContainer">
        <asp:Panel runat="server" ID="pnlTemplateName">
            <table class="datatable">
                <tr>
                    <th colspan="2">Dati Template</th>
                </tr>
                <tr class="Chiaro">
                    <td style="width: 150px; vertical-align: middle; font-size: 11px; text-align: right">
                        <div runat="server" style="font-weight: bold;">Nome Template:</div>
                    </td>
                    <td>
                        <div runat="server">
                            <asp:TextBox ID="templateName" runat="server" CausesValidation="True" MaxLength="255" Width="400px"></asp:TextBox>
                        </div>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <!-- Tipologia di Protocollo -->
        <telerik:RadAjaxPanel runat="server" ID="UpdatePanelProtocollo" ClientEvents-OnRequestStart="OnRequestStart" ClientEvents-OnResponseEnd="OnResponseEnd">
            <table class="datatable">
                <tr class="Chiaro">
                    <td style="width: 150px; vertical-align: middle; text-align: right;">
                        <b>Tipo di protocollo:</b>
                    </td>
                    <td>
                        <asp:RadioButtonList ID="rblTipoProtocollo" runat="server" AutoPostBack="True" Font-Names="Verdana" CssClass="autoWidth" DataTextField="Description" DataValueField="Id" />
                    </td>
                </tr>
            </table>
        </telerik:RadAjaxPanel>
        <%-- Sezione Dati Fatture --%>
        <telerik:RadAjaxPanel runat="server" ID="radAjaxPnlInvoice">
            <asp:Panel ID="pnlInvoice" runat="server">
                <table class="datatable">
                    <tr>
                        <th colspan="2">Dati Contabilità</th>
                    </tr>
                    <telerik:RadAjaxPanel runat="server" ID="radAjaxPnlSectionalType">
                        <asp:Panel ID="pnlSectionalType" runat="server">
                            <tr class="Chiaro">
                                <td style="width: 148px; text-align: right; vertical-align: middle;">
                                    <b>Sezionale:</b>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlAccountingSectional" runat="server" />
                                </td>
                            </tr>
                        </asp:Panel>
                    </telerik:RadAjaxPanel>
                </table>
            </asp:Panel>
        </telerik:RadAjaxPanel>
        <asp:Panel ID="pnlContenitore" runat="server">
            <table id="TblContenitore" class="datatable">
                <tr>
                    <th colspan="2">Contenitore</th>
                </tr>
                <tr>
                    <td>
                        <div style="padding-left: 150px;">
                            <telerik:RadComboBox runat="server" CausesValidation="false"
                                ID="rcbContainer" AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true"
                                ItemRequestTimeout="500" Visible="false" Width="300px">
                            </telerik:RadComboBox>
                            <asp:DropDownList ID="cboIdContainer" runat="server" AutoPostBack="false" />
                        </div>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlProtocolKind">
            <table class="datatable">
                <tr>
                    <th colspan="2">Modello del protocollo</th>
                </tr>
                <tr class="Chiaro">
                    <td style="padding-left: 150px; padding-top: 5px; padding-bottom: 5px;">
                        <asp:DropDownList ID="ddlProtKindList" runat="server" AutoPostBack="False"></asp:DropDownList>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <%-- Sezione Tipologia spedizione --%>
        <asp:Panel ID="pnlIdDocType" runat="server">
            <table class="datatable">
                <%-- Sezione Scelta Tipologia spedizione --%>
                <tr>
                    <th colspan="2">
                        <span style="margin-right: 50px;">Tipologia spedizione</span>
                        <asp:CheckBox ID="cbClaim" Text="Reclamo" TextAlign="Left" runat="server" />
                    </th>
                </tr>
                <tr class="Chiaro">
                    <td style="padding-left: 150px; padding-top: 5px; padding-bottom: 5px;">
                        <asp:DropDownList ID="cboIdDocType" runat="server" AutoPostBack="False">
                            <asp:ListItem Value="" Text="" />
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <%-- Sezione Scelta Stato Protocollo --%>
        <asp:Panel ID="pnlProtocolStatus" runat="server">
            <table class="datatable">
                <tr>
                    <th colspan="2">Stato del protocollo</th>
                </tr>
                <tr class="Chiaro">
                    <td style="width: 150px">&nbsp;
                    </td>
                    <td>
                        <asp:DropDownList ID="cboProtocolStatus" DataValueField="Id" DataTextField="Description" runat="server" />
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <%-- Scelta Contatti --%>
        <table class="datatable">
            <tr>
                <td id="MittCell" style="width: 50%; white-space: nowrap;" runat="server">
                    <usc:SelContatti ButtonImportManualVisible="true" Caption="Mittenti" EnableCC="false" ID="uscMittenti" Multiple="true" MultiSelect="true" ProtType="True" runat="server" TreeViewCaption="Mittenti" Type="Prot" />
                </td>
                <td id="DestCell" style="width: 50%; white-space: nowrap;" runat="server">
                    <usc:SelContatti ButtonImportManualVisible="true" Caption="Destinatari" EnableCC="true" ID="uscDestinatari" IsRequired="False" Multiple="true" MultiSelect="true" ProtType="True" runat="server" TreeViewCaption="Destinatari" Type="Prot" />
                </td>
            </tr>
        </table>
        <%-- Sezione Autorizzazioni --%>
        <asp:Panel ID="pnlAutorizzazioni" runat="server" Visible="False">
            <usc:Settori Caption="Autorizzazioni" ID="uscAutorizzazioni" MultipleRoles="True" MultiSelect="True" Required="False" RoleRestictions="None" runat="server" Type="Prot" />
        </asp:Panel>
        <%-- Sezione Oggetto --%>
        <table class="datatable">
            <tr>
                <th>Oggetto</th>
            </tr>
            <tr class="Chiaro">
                <td>
                    <usc:SelOggetto runat="server" ID="uscOggetto" EditMode="true" MultiLine="true" MaxLength="255" Required="False" Type="Prot" />
                </td>
            </tr>
        </table>
        <%-- Sezione Classificatore --%>
        <usc:SelCategory runat="server" ID="uscClassificatori" Required="False" Type="Prot" Caption="Classificazione" Multiple="false" />
        <table class="datatable">
            <%-- Sezione Note --%>
            <tr class="Chiaro">
                <td style="width: 150px; vertical-align: middle; font-size: 11px; text-align: right">
                    <div id="divLblNote" runat="server" style="font-weight: bold;">Note:</div>
                </td>
                <td>
                    <div id="divTxtNote" runat="server">
                        <asp:TextBox ID="txtNote" runat="server" MaxLength="255" Width="300px" onBlur="javascript:ChangeStrWithValidCharacter(this);"></asp:TextBox>
                    </div>
                </td>
            </tr>
            <%-- Sezione Assegnatario --%>
            <tr class="Chiaro">
                <td style="width: 150px; vertical-align: middle; font-size: 11px; text-align: right;">
                    <b>
                        <asp:Label ID="lblAssegnatario" runat="server">Assegnatario:</asp:Label></b>
                </td>
                <td>
                    <usc:SelContattiTesto ForceAddressBook="True" ID="uscContactAssegnatario" MaxLunghezzaTesto="1000" runat="server" TextBoxWidth="300px" Type="Prot" />
                </td>
            </tr>
            <%-- Sezione Categoria di servizio --%>
            <tr class="Chiaro">
                <td style="width: 150px; vertical-align: middle; font-size: 11px; text-align: right">
                    <div id="divLblServiceCategory" runat="server" style="font-weight: bold;">Categoria di servizio:</div>
                </td>
                <td>
                    <div id="divSelServiceCategory" runat="server">
                        <usc:SelServiceCategory runat="server" ID="SelServiceCategory" TextBoxWidth="300px" EditMode="true" MultiLine="false" MaxLength="100" Required="false" Type="Prot" />
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server">
    <div class="footer-buttons-wrapper">
        <asp:Button runat="server" ID="btnConfirm" Width="150" Text="Conferma" OnClientClick="return TemplateProtocolAjaxRequest('save');" />
        <asp:Button runat="server" ID="btnAnnulla" Text="Annulla" Width="150" OnClientClick="return TemplateProtocolAjaxRequest('close');" />
    </div>
</asp:Content>
