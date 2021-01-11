<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TbltContenitoriGesGruppi.aspx.vb"
    Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltContenitoriGesGruppi" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Contenitori - Modifica" %>

<%@ Register Src="~/UserControl/uscGroup.ascx" TagPrefix="usc" TagName="SelGroup" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphHeader">
    <style>
        #divContent {
            height: 250px !important;
        }
    </style>
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            //restituisce un riferimento alla radwindow
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function UpdateGroups() {
               // GetRadWindow().BrowserWindow.UpdateGroups();
                GetRadWindow().close();
            }

            function OpenContenitoriCopyWindow() {
                var manager = $find("<%=RadWindowManagerContainerGroups.ClientID%>");
                var wnd = manager.open(null, "windowContainersCopy");
                wnd.setSize(500, 350);
                wnd.center();
            }

            function CloseContenitoriCopy(sender, args) {
                var ajaxManager = $find("<%= AjaxManager.ClientID%>");
                let node = $find("<%= rtvContainersCopy.ClientID%>").get_selectedNode();
                let selectedValue = node.get_value();
                ajaxManager.ajaxRequest("Copy|" + selectedValue);
                var window = $find("<%=windowContainersCopy.ClientID%>");
                window.close();
            }

        </script>
    </telerik:RadScriptBlock>
    <usc:SelGroup runat="server" ID="uscGruppi" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadWindowManager runat="server" EnableViewState="false" ID="RadWindowManagerContainerGroups" Modal="true">
        <Windows>
            <telerik:RadWindow ID="windowContainersCopy" runat="server">
                <ContentTemplate>
                    <asp:Panel runat="server">
                        <telerik:RadTreeView runat="server" ID="rtvContainersCopy" Height="25em" BorderStyle="Solid" BorderColor="Gray">
                            <Nodes>
                                <telerik:RadTreeNode Expanded="true" NodeType="Root" runat="server" Text="Contenitore" Value="" />
                            </Nodes>
                        </telerik:RadTreeView>
                    </asp:Panel>
                    <telerik:RadButton runat="server" Text="Conferma" ID="btnConfirmCopyContainer" OnClientClicked="CloseContenitoriCopy" AutoPostBack="false"></telerik:RadButton>
                </ContentTemplate>
            </telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>

    <asp:Panel ID="pnlPrivacy" runat="server" Width="100%" Visible="false">
        <table runat="server" width="100%">
            <tr class="tabella">
                <td>
                    <asp:Label ID="Label1" runat="server" Width="100%" Font-Bold="True"><%= PrivacyLabelTitle %></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblPrivacy" runat="server"></asp:Label>
                    <asp:DropDownList ID="ddlPrivacy" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlDiritti" runat="server" Width="100%" Height="100%">
        <table id="Table3" cellspacing="1" cellpadding="1" width="100%" border="0">
            <tr class="tabella">
                <td>
                    <asp:Label ID="lblProt" runat="server" Width="100%" Font-Bold="True">Protocollo</asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblDocm" runat="server" Width="100%" Visible="False" Font-Bold="True">Dossier e Pratiche</asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblResl" runat="server" Width="100%" Visible="False" Font-Bold="True">Label</asp:Label>
                </td>
                <td>
                    <asp:Label Font-Bold="True" ID="lblSeries" runat="server" Visible="False" Width="100%" />
                </td>
                <td>
                    <asp:Label Font-Bold="True" ID="lblDesks" runat="server" Visible="False" Width="100%">Tavoli</asp:Label>
                </td>
                <td>
                    <asp:Label Font-Bold="True" ID="lblUDS" runat="server" Visible="False" Width="100%">Unità documentarie</asp:Label>
                </td>
                <td>
                    <asp:Label Font-Bold="True" ID="lblFascicles" runat="server" Visible="False" Width="100%">Fascicoli</asp:Label>
                </td>
            </tr>
            <tr>
                <td valign="top" height="100%">
                    <asp:CheckBoxList Font-Names="verdana, helvetica, sans-serif" ID="cblProt" runat="server" Width="100%" />
                </td>
                <td valign="top" height="100%">
                    <asp:CheckBoxList Font-Names="verdana, helvetica, sans-serif" ID="cblDocm" runat="server" Visible="False" Width="100%" />
                </td>
                <td valign="top" height="100%">
                    <asp:CheckBoxList Font-Names="verdana, helvetica, sans-serif" ID="cblResl" runat="server" Visible="False" Width="100%" />
                </td>
                <td valign="top" height="100%">
                    <asp:CheckBoxList Font-Names="verdana, helvetica, sans-serif" ID="cblSeries" runat="server" Visible="False" Width="100%" />
                </td>
                <td valign="top" height="100%">
                    <asp:CheckBoxList Font-Names="verdana, helvetica, sans-serif" ID="cblDesks" runat="server" Visible="False" Width="100%" />
                </td>
                <td valign="top" height="100%">
                    <asp:CheckBoxList Font-Names="verdana, helvetica, sans-serif" ID="cblUDS" runat="server" Visible="False" Width="100%" />
                </td>
                <td valign="top" height="100%">
                    <asp:CheckBoxList Font-Names="verdana, helvetica, sans-serif" ID="cblFascicles" runat="server" Visible="False" Width="100%" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConfermaDiritti" runat="server" Text="Conferma" />
    <asp:Button ID="btnCopia" runat="server" Text="Copia da altro contenitore" />
</asp:Content>
