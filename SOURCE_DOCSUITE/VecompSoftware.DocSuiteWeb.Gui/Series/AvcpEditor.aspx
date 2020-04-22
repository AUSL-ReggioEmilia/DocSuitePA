<%@ Page Title="Pubblicazione - Modifica" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="AvcpEditor.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.Series.AvcpEditor" %>

<%@ Register Src="uscAvcpLotto.ascx" TagPrefix="uc" TagName="uscAvcpLotto" %>
<%@ Register Src="~/UserControl/uscSettori.ascx" TagPrefix="uc" TagName="uscSettori" %>
<%@ Register Src="~/UserControl/uscSelezionaAziende.ascx" TagPrefix="uc" TagName="uscSelezionaAziende" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">

    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            function openBandoGaraPopup() {
                $find("<%=windowsBandoGara.ClientID%>").show();
                return false;
            }

            function closeBandoGaraPopup() {
                $find("<%=windowsBandoGara.ClientID%>").show();
                return false;
            }

            $(function () {
                if ($telerik.isIE8) {
                    Sys.Application.add_load(function (e, args) {
                        setTimeout(function () {
                            resizeSplitter();
                        }, 1);
                    });
                } else {
                    resizeSplitter();
                }                
            });

            function resizeSplitter() {
                var panelIntestazioneHeight = $("#<%= panelIntestazione.ClientID %>").height();
                var panelInsertInAVCPHeight = $("#<%= panelInsertInAVCP.ClientID %>").height();
                var pageContentHeight = $(".page-content").height();

                $find("<%=metadataPane.ClientID%>").set_height(panelIntestazioneHeight + panelInsertInAVCPHeight + 5);
                $find("<%=configurationPane.ClientID%>").set_height(pageContentHeight - $find("<%=metadataPane.ClientID%>").get_height());
            }
        </script>
    </telerik:RadScriptBlock>

</asp:Content>


<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <!-- Seleziona partecipanti del bando di gara-->
    <telerik:RadWindow ID="windowsBandoGara" runat="server" Width="700px" Height="350px" Modal="true">
        <ContentTemplate>
            <uc:uscSelezionaAziende ID="uscAziendeBando" runat="server" SearchEnabled="True"></uc:uscSelezionaAziende>
        </ContentTemplate>
    </telerik:RadWindow>

    <telerik:RadWindow ID="RadWindow1" runat="server" Width="300px" Height="130px" Modal="true" IconUrl="~/Comm/Images/warning.gif">
        <ContentTemplate>
            <div style="display: block;">
                <asp:Image ImageUrl="~/Comm/Images/warning.gif" Width="32" Height="32" runat="server" />
            </div>
            <div style="display: block; text-align: center; margin: 5px;">
                <asp:Label ID="txtMessageWindow" runat="server" />
            </div>
            <div style="display: block; text-align: center; position: absolute; bottom: 5px; width: 100%">
                <telerik:RadButton ID="btnAlertClose" runat="server" Text="OK" />
            </div>
        </ContentTemplate>
    </telerik:RadWindow>

    <asp:Panel runat="server" CssClass="splitterWrapper">
        <telerik:RadSplitter runat="server" ID="pageSplitter" ResizeWithParentPane="false" Width="100%" Height="100%" Orientation="Horizontal">
            <telerik:RadPane Height="50%" runat="server" ID="metadataPane" Width="100%" Scrolling="Y">
                <asp:Panel runat="server" ID="panelIntestazione">

                    <table class="datatable">
                        <tr>
                            <th colspan="2">
                                <asp:Label ID="lblTitle" runat="server" />
                            </th>
                        </tr>
                        <tr>
                            <td class="label" style="width: 20%">Anno riferimento</td>
                            <td>
                                <telerik:RadNumericTextBox ID="txtYear" IncrementSettings-InterceptArrowKeys="True" IncrementSettings-InterceptMouseWheel="True" MaxLength="4" NumberFormat-DecimalDigits="0" NumberFormat-GroupSeparator="" runat="server" Width="56px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Titolo</td>
                            <td>
                                <telerik:RadTextBox ID="txtTitle" runat="server" Width="100%" />
                            </td>
                        </tr>
                        <tr id="trAbstract" runat="server" visible="false">
                            <td class="label">Abstract</td>
                            <td>
                                <telerik:RadTextBox ID="txtAbstract" runat="server" Width="100%" />
                            </td>
                        </tr>
                        <tr id="trpublishingDate" runat="server" visible="false">
                            <td class="label">Data pubblicazione</td>
                            <td>
                                <telerik:RadDatePicker runat="server" ID="publishingDate" Enabled="false" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Ente Pubblicatore</td>
                            <td>
                                <telerik:RadTextBox ID="publisher" runat="server" />
                            </td>
                        </tr>
                        <tr id="trlastUpdateDate" runat="server" visible="false">
                            <td class="label">Data Ultimo Aggiornamento</td>
                            <td>
                                <telerik:RadDatePicker runat="server" ID="lastUpdateDate" Enabled="false" />
                            </td>
                        </tr>
                        <tr id="trurl" runat="server" visible="false">
                            <td class="label">Url</td>
                            <td>
                                <telerik:RadTextBox runat="server" ID="url" InputType="Url" Width="100%" EmptyMessage="Inserire l'url" />
                            </td>
                        </tr>
                        <tr id="trlicence" runat="server" visible="false">
                            <td class="label">Licenza</td>
                            <td>
                                <telerik:RadTextBox ID="licence" runat="server" />
                            </td>
                        </tr>
                    </table>
                    <table class="datatable">
                        <tr>
                            <th colspan="2">Intestazione di gara</th>
                        </tr>
                        <tr>
                            <td class="label col-dsw-2">CF Struttura Appaltante:</td>
                            <td>
                                <asp:Label ID="lbCFStrutturaAppaltante" runat="server" Width="100%" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label col-dsw-2">Ente Appaltante:</td>
                            <td>
                                <asp:Label ID="lbEnteAppaltante" runat="server" Width="100%" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label col-dsw-2">Struttura proponente:</td>
                            <td>
                                <uc:uscSettori runat="server" ID="uscProponente" MultiSelect="False" MultipleRoles="False" Required="false" Visible="true" />
                            </td>
                        </tr>
                    </table>
                    <table class="datatable">
                        <tr>
                            <th colspan="2">Aziende</th>
                        </tr>
                        <tr>
                            <td class="label col-dsw-2">Aziende partecipanti:</td>
                            <td style="margin: 5px; vertical-align: central;">
                                <asp:Label ID="lblSumAziendeInvitate" runat="server" Width="20" Height="20" />
                                <telerik:RadButton ID="btnElencoAziendeInvitate" runat="server" Text="Elenco" />
                            </td>
                        </tr>
                    </table>

                </asp:Panel>

                <asp:Panel runat="server" ID="panelInsertInAVCP" Visible="false">
                    <table class="datatable">
                        <tr>
                            <th colspan="2">Inserimento Serie Documentale in AVCP</th>
                        </tr>
                        <tr>
                            <td class="label col-dsw-2">Inserisci anche in AVCP:</td>
                            <td>
                                <asp:CheckBox ID="chbInsertAVCP" runat="server" Checked="false" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </telerik:RadPane>
            <telerik:RadSplitBar Width="100%" runat="server"></telerik:RadSplitBar>
            <telerik:RadPane Height="100%" runat="server" ID="configurationPane" Scrolling="Y">
                <asp:Panel runat="server" ID="panelAVCPLotto" CssClass="splitterWrapper">
                    <telerik:RadSplitter runat="server" ID="lotSplitter" PanesBorderSize="0" BorderSize="0" Width="100%" Height="100%" ResizeWithParentPane="False">
                        <telerik:RadPane ID="lotTreePane" Scrolling="Y" runat="server" Width="50%" Height="100%">
                            <%-- Treeview Lotti --%>
                            <telerik:RadTreeView ID="lotsTree" runat="server" />
                        </telerik:RadPane>
                        <telerik:RadSplitBar runat="server" ID="splitBar" />
                        <telerik:RadPane ID="detailPane" runat="server" Width="50%" Scrolling="None" Height="100%">
                            <telerik:RadSplitter runat="server" ID="treeSplitter" Orientation="Horizontal" ResizeWithParentPane="True" Width="100%" Height="100%">
                                <telerik:RadPane runat="server" ID="toolbarTopPane" BorderStyle="None" Height="34px" Scrolling="None" Width="100%">
                                    <%-- Toolbar Opzioni --%>
                                    <telerik:RadToolBar AutoPostBack="true" CssClass="ToolBarContainer" Height="34px" EnableRoundedCorners="False" EnableShadows="False" ID="toolBar" runat="server" Width="100%">
                                        <Items>
                                            <telerik:RadToolBarButton ImageUrl="~/App_Themes/DocSuite2008/imgset16/disk.png" ToolTip="Salva" Value="save" />
                                            <telerik:RadToolBarButton ImageUrl="~/App_Themes/DocSuite2008/imgset16/add.png" ToolTip="Aggiungi" Value="add" />
                                            <telerik:RadToolBarButton ToolTip="Rimuovi" Value="remove" />
                                        </Items>
                                    </telerik:RadToolBar>
                                </telerik:RadPane>
                                <telerik:RadPane runat="server" ID="contactInnerTreePane" BorderStyle="None" Width="100%">
                                    <%-- Edit Dettagli --%>
                                    <uc:uscAvcpLotto runat="server" ID="lot" Visible="false" />
                                </telerik:RadPane>
                            </telerik:RadSplitter>
                        </telerik:RadPane>
                    </telerik:RadSplitter>
                </asp:Panel>
            </telerik:RadPane>
        </telerik:RadSplitter>
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Button ID="btnSave" runat="server" Text="Salva" />
</asp:Content>
