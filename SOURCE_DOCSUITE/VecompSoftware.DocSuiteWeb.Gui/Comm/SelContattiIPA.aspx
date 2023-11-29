<%@ Page AutoEventWireup="false" CodeBehind="SelContattiIPA.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.SelContattiIPA" Language="vb" Title="Pubblica Amministrazione" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">
            var selContattiIPA;
            require(["Comm/SelContattiIPA"], function (SelContattiIPA) {
                $(function () {
                    selContattiIPA = new SelContattiIPA(tenantModelConfiguration.serviceConfiguration);
                    selContattiIPA.treeViewId = "<%= tvwIPA.ClientID %>";
                    selContattiIPA.btnSearchId = "<%= btnSearch.ClientID %>";
                    selContattiIPA.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    selContattiIPA.txtSearchId = "<%= txtSearch.ClientID %>";
                    selContattiIPA.btnConfermaId = "<%= btnConferma.ClientID %>";
                    selContattiIPA.btnConfermaNuovoId = "<%= btnConfermaNuovo.ClientID %>";
                    selContattiIPA.cmdDetailId = "<%= cmdDetail.ClientID %>";
                    selContattiIPA.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    selContattiIPA.callerId = "<%= CallerID %>";
                    selContattiIPA.windowDetailsId = "<%= windowDetails.ClientID %>";
                    selContattiIPA.windowManagerDetailId = "<%= RadWindowManagerDetail.ClientID %>";
                    selContattiIPA.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerDetail" runat="server">
        <Windows>
            <telerik:RadWindow Height="400" ID="windowDetails" ReloadOnShow="false" runat="server" Title="Contatto - Proprietà" Width="450">
                <ContentTemplate>
                    <table class="datatable">
                        <thead>
                            <tr>
                                <th class="head" style="width: 50%">Proprietà
                                </th>
                                <th class="head" style="width: 50%">Valore
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td class="label">Descrizione</td>
                                <td id="lblDescrizione"></td>
                            </tr>
                            <tr>
                                <td class="label">Indirizzo</td>
                                <td id="lblIndirizzo"></td>
                            </tr>
                            <tr>
                                <td class="label">CAP</td>
                                <td id="lblCAP"></td>
                            </tr>
                            <tr>
                                <td class="label">Provincia</td>
                                <td id="lblProvincia"></td>
                            </tr>
                            <tr>
                                <td class="label">Regione</td>
                                <td id="lblRegione"></td>
                            </tr>
                            <tr>
                                <td class="label">Telefono</td>
                                <td id="lblTelefono"></td>
                            </tr>
                            <tr>
                                <td class="label">eMail</td>
                                <td id="lblEmail"></td>
                            </tr>
                            <tr>
                                <td class="label">Sito web</td>
                                <td id="lblWebSite"></td>
                            </tr>
                            <tr>
                                <td class="label">Responsabile</td>
                                <td id="lblResponsabile"></td>
                            </tr>
                            <tr>
                                <td class="label">Codice amministrazione</td>
                                <td id="lblCodAmm"></td>
                            </tr>
                            <tr>
                                <td class="label">Tipo amministrazione</td>
                                <td id="lblTypeAmm"></td>
                            </tr>
                        </tbody>
                    </table>
                </ContentTemplate>
            </telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>

    <table class="datatable">
        <tr>
            <td class="label labelPanel col-dsw-3">Ricerca amministrazioni:</td>
            <td>
                <asp:Panel runat="server" ID="pnlCerca" DefaultButton="btnSearch" CssClass="dsw-display-inline">
                    <telerik:RadTextBox ID="txtSearch" MaxLength="30" runat="server" Width="200px" />
                </asp:Panel>
                <telerik:RadButton ID="btnSearch" runat="server" Text="Ricerca" UseSubmitBehavior="true" AutoPostBack="false">
                    <Icon PrimaryIconUrl="../App_Themes/DocSuite2008/images/search-transparent.png" />
                </telerik:RadButton>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadTreeView ID="tvwIPA" runat="server" Height="100%" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <table class="col-dsw-10">
        <tr>
            <td>
                <telerik:RadButton ID="btnConferma" runat="server" Text="Conferma" AutoPostBack="false" />
                <telerik:RadButton ID="btnConfermaNuovo" runat="server" Text="Conferma e Nuovo" AutoPostBack="false" />
            </td>

            <td class="dsw-text-right">
                <telerik:RadButton Enabled="False" ID="cmdDetail" runat="server" Text="Proprietà" AutoPostBack="false" />
            </td>
        </tr>
    </table>
</asp:Content>
