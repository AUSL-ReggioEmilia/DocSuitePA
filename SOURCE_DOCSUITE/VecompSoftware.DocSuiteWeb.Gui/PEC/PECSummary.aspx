<%@ Page AutoEventWireup="false" CodeBehind="PECSummary.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.PECSummary" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscProtocolPreview.ascx" TagName="uscProtocolPreview" TagPrefix="uc" %>
<%@ Register Src="~/PEC/uscInteropInfo.ascx" TagPrefix="usc" TagName="uscInteropInfo" %>
<%@ Register Src="~/UserControl/uscPecHistory.ascx" TagName="PecHistory" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function ShowLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= pnlMainContent.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);

                var ajaxFlatLoadingPanel = $find("<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>");
                var pnlButtons = "<%= pnlButtons.ClientID%>";
                ajaxFlatLoadingPanel.show(pnlButtons);
            }

            function HideLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= pnlMainContent.ClientID%>";
                currentLoadingPanel.hide(currentUpdatedControl);

                var ajaxFlatLoadingPanel = $find("<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>");
                var pnlButtons = "<%= pnlButtons.ClientID%>";
                ajaxFlatLoadingPanel.hide(pnlButtons);
            }

            function StartDownload(url) {
                setLink("link_download", url);
            }

            function setLink(key, value) {
                var link = document.getElementById(key);
                link.href = value;
                link.click();
            }

            function confirmFisicalRemovePEC(arg) {
                if (arg) {
                    let ajaxModel = {};
                    ajaxModel.ActionName = "FisicalRemovePEC";
                    $find('<% = MasterDocSuite.AjaxManager.ClientID%>').ajaxRequest(JSON.stringify(ajaxModel));
                } else {
                    HideLoadingPanel()
                }
             }     
        </script>
        <style>
            #ctl00_DefaultLoadingPanelctl00_cphContent_pnlMainContent {
            z-index: 1 !important;
            }
            </style>
    </telerik:RadCodeBlock>
    <asp:Panel runat="server" ID="pnlDestination" CssClass="hiddenField">
        <asp:Label ID="lblDestination" runat="server" />
    </asp:Panel>
    <telerik:RadWindowManager runat="server" ID="windowManager">

    </telerik:RadWindowManager>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel runat="server" ID="pnlMainContent">
        <table style="width: 100%; height: 100%">
            <tr>
                <td style="width: 100%; vertical-align: top;">
                    <table class="datatable" runat="server" id="tblMainData">
                        <tr>
                            <th colspan="2">
                                <asp:Label runat="server" ID="lblMainData" /></th>
                        </tr>
                        <tr class="Chiaro">
                            <td class="label">Mittente:</td>
                            <td style="width: 85%;">
                                <asp:Label ID="lblFrom" runat="server" />
                            </td>
                        </tr>
                        <tr class="Chiaro">
                            <td class="label">Destinatari:</td>
                            <td>
                                <asp:Label ID="lblTo" runat="server" />
                            </td>
                        </tr>
                        <tr class="Chiaro" id="trCc" runat="server" visible="False">
                            <td class="label">Destinatari Copia Conoscenza:</td>
                            <td>
                                <asp:Label ID="lblCc" runat="server" />
                            </td>
                        </tr>
                        <tr class="Chiaro">
                            <td class="label">Gestione:</td>
                            <td>
                                <asp:Label ID="lblHandler" runat="server" />
                            </td>
                        </tr>
                        <tr class="Chiaro">
                            <td class="label">Dimensione:</td>
                            <td>
                                <asp:Label ID="lblPecSize" runat="server" />
                            </td>
                        </tr>
                        <tr class="Chiaro">
                            <td class="label">Stato della PEC:</td>
                            <td>
                                <asp:Label ID="lblStatus" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="vertical-align: top;">Ricevute:
                            </td>
                            <td style="width: 85%">
                                <usc:PecHistory ID="uscPecHistory" runat="server" />
                            </td>
                        </tr>
                        <tr class="Chiaro">
                            <td class="label">Informazioni:</td>
                            <td>
                                <asp:Label ID="lblInfo" runat="server" />
                            </td>
                        </tr>
                    </table>
                    <table id="tblAnnullamento" class="datatable" runat="server">
                        <tr>
                            <th>Motivazione annullamento
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:Image ID="imgAnnullamento" ImageUrl="~/Comm/Images/Remove32.gif" runat="server" Height="32px" Width="32px" />
                                <asp:Label ID="lblDeleteInfo" runat="server" />
                            </td>
                        </tr>
                    </table>
                    <table class="datatable" runat="server" id="tblObject">
                        <tr>
                            <th colspan="2">Oggetto</th>
                        </tr>
                        <tr class="Chiaro">
                            <td>
                                <asp:Label runat="server" ID="lblSubject" Width="100%"></asp:Label>
                            </td>
                        </tr>
                    </table>

                    <table class="datatable" runat="server" id="tblBody">
                        <tr>
                            <th colspan="2">Messaggio</th>
                        </tr>
                        <tr class="Chiaro">
                            <td>
                                <telerik:RadEditor AutoResizeHeight="True" Enabled="false" ContentFilters="RemoveScripts,FixEnclosingP" EmptyMessage="Nessun testo trovato" runat="server" ID="lblBody" Style="overflow: auto; border: 0 none;" />
                            </td>
                        </tr>
                    </table>

                    <usc:uscInteropInfo ID="uscInteropInfo" runat="server" Visible="false" />

                    <asp:Panel runat="server" ID="DestinationPanel" CssClass="hiddenField">
                        <asp:Label ID="DestinationLabel" runat="server" />
                    </asp:Panel>

                    <uc:uscProtocolPreview ID="uscProtocolPreview" runat="server" Title="Protocollo collegato" />
                </td>
                <td class="center" style="width: 3%; height: 100%">
                    <asp:Table BorderColor="Gray" BorderStyle="Solid" BorderWidth="1px" CellPadding="3" CellSpacing="0" Height="100%" ID="TblButtons" runat="server">
                        <asp:TableRow Height="100%">
                            <asp:TableCell Height="100%" ID="IconsCell" VerticalAlign="Top" />
                        </asp:TableRow>
                    </asp:Table>
                </td>
            </tr>
        </table>
        <a id="link_download" href="#" style="display: none">Il documento richiesto è pronto per il download.</a>
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlButtons">
        <div>
            <asp:Button ID="btnDestinate" OnClientClick="ShowLoadingPanel();" PostBackUrl="~/PEC/PECDestination.aspx" runat="server" Text="Destina" Width="150" />
            <asp:Button ID="btnPECToDocumentUnit" OnClientClick="ShowLoadingPanel();" PostBackUrl="~/Pec/PECToDocumentUnit.aspx?isInWindow=true" runat="server" Text="Protocolla" Width="150" />
            <asp:Button ID="btnAttachPec" OnClientClick="ShowLoadingPanel();" PostBackUrl="~/PEC/PECAttachToDocumentUnit.aspx" runat="server" Text="Allega" Width="150" Visible="False" Enabled="true" />
            <asp:Button ID="btnLinkToProtocol" PostBackUrl="~/Pec/PECLinkProtocol.aspx" runat="server" Text="Collega" Width="150" Visible="False" OnClientClick="ShowLoadingPanel();" />
            <asp:Button ID="btnReply" OnClientClick="ShowLoadingPanel();" PostBackUrl="~/PEC/PECInsert.aspx" runat="server" Width="150" Visible="False" Enabled="true" />
            <asp:Button ID="btnReplyAll" OnClientClick="ShowLoadingPanel();" PostBackUrl="~/PEC/PECInsert.aspx" runat="server" Width="150" Visible="False" Enabled="true" />
            <asp:Button ID="btnMovePEC" OnClientClick="ShowLoadingPanel();" PostBackUrl="~/PEC/PECMoveMails.aspx" runat="server" Text="Sposta" Width="150" />
            <asp:Button ID="btnMail" OnClientClick="ShowLoadingPanel();" PostBackUrl="~/MailSenders/PecMailSender.aspx" runat="server" Text="Invia" Width="150" />
            <asp:Button ID="btnOriginalEml" runat="server" Width="150" Visible="False" />
        </div>
        <div>
            <asp:Button ID="cmdPECView" OnClientClick="ShowLoadingPanel();" runat="server" Text="Documenti" Width="150" />
            <asp:Button ID="btnUnhandle" runat="server" Text="Rilascia" Visible="False" Width="150" CommandArgument="False" Enabled="true" />
            <asp:Button ID="btnHandle" runat="server" Text="Prendi in carico" Visible="False" Width="150" CommandArgument="True" Enabled="true" />
            <asp:Button ID="btnDelete" OnClientClick="ShowLoadingPanel();" PostBackUrl="~/PEC/PECDelete.aspx?Type=PEC" runat="server" Text="Elimina" Width="150" />
            <asp:Button ID="btnFisicalRemovePEC" OnClientClick="ShowLoadingPanel();" runat="server" Text="Rimuovi PEC" Width="150" Enabled="false"/>
            <asp:Button ID="btnRestore" runat="server" Text="Ripristina" Width="150" />
            <asp:Button ID="btnForward" OnClientClick="ShowLoadingPanel();" PostBackUrl="~/PEC/PECInsert.aspx" runat="server" Width="150" />
            <asp:Button ID="btnReceipt" runat="server" Text="Ricevuta" Visible="False" Width="150" />
            <asp:Button ID="btnViewLog" OnClientClick="ShowLoadingPanel();" PostBackUrl="~/PEC/PecViewLog.aspx" runat="server" Text="LOG" Width="150" />
            <asp:Button ID="btnFix" OnClientClick="ShowLoadingPanel();" PostBackUrl="~/PEC/PecFix.aspx" runat="server" Text="Correggi" Width="150" />
            <asp:Button ID="btnRaccomandata" runat="server" Width="150" Text="Invia raccomandata" />
            <asp:Button ID="cmdResend" runat="server" Width="150px" Text="Reinvia PEC" />
        </div>
    </asp:Panel>
</asp:Content>
