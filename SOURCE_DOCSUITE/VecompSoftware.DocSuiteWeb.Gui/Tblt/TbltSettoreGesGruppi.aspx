<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TbltSettoreGesGruppi.aspx.vb"
    Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltSettoreGesGruppi" MasterPageFile="~/MasterPages/DocSuite2008.Master"
    Title="Settori - Modifica" %>

<%@ Register Src="~/UserControl/uscGroup.ascx" TagName="SelGroup" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            //restituisce un riferimento alla radwindow
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow(role) {
                var oRole = new Object();
                oRole.Operation = roleOperation;
                oRole.ID = roleID;
                oRole.NodeType = nodeType;

                var oWindow = GetRadWindow();
                oWindow.close(oRole);
            }

            function UpdateGroups() {
                GetRadWindow().BrowserWindow.UpdateGroups();
            }

        </script>
    </telerik:RadScriptBlock>

    <usc:SelGroup runat="server" ID="uscGruppi"/>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel ID="pnlDiritti" runat="server" Width="100%" Height="100%">
        <table class="datatable">
            <tr>
                <td width="25%" class="head">
                    <asp:Label ID="lblDocm" runat="server" Visible="False">Dossier e Pratiche</asp:Label>
                </td>
                <td width="25%" class="head">
                    <asp:Label ID="lblProt" runat="server" Visible="False">Protocollo</asp:Label>
                </td>
                <td width="25%" class="head">
                    <asp:Label ID="lblResl" runat="server" Visible="False" />
                </td>
                <td width="25%" class="head">
                    <asp:Label  ID="lblSeries" runat="server" Visible="True" />
                </td>
               
            </tr>
            <tr>
                <td valign="top" width="25%" height="100%">
                    <asp:CheckBox Font-Names="font-family:verdana" Font-Size="8pt" ID="ckbDocm" runat="server" Text="Dossier e Pratiche" OnCheckedChanged="onCheck" Visible="False" Width="100%" AutoPostBack="true" />
                    <asp:CheckBoxList Font-Names="font-family:verdana" Font-Size="8pt" ID="cblDocm" runat="server" Visible="False" Width="100%" OnSelectedIndexChanged="onCheck" AutoPostBack="true"/>
                </td>
                <td valign="top" width="25%" height="100%">
                    <asp:CheckBox Font-Names="font-family:verdana" Font-Size="8pt" ID="ckbProt" runat="server" Text="Protocollo" Visible="False" Width="100%" OnCheckedChanged="onCheck" AutoPostBack="true"/>
                    <asp:CheckBoxList Font-Names="font-family:verdana" Font-Size="8pt" ID="cblProt" runat="server" Visible="False" Width="100%" OnSelectedIndexChanged="onCheck" AutoPostBack="true"/>
                </td>
                <td valign="top" width="25%" height="100%">
                    <asp:CheckBox Font-Names="font-family:verdana" Font-Size="8pt" ID="ckbResl" runat="server" Visible="False" Width="100%" OnCheckedChanged="onCheck" AutoPostBack="true"/></td>
                <td valign="top" width="25%" height="100%">
                    <asp:CheckBox Font-Names="font-family:verdana" Font-Size="8pt" ID="chbSeries" runat="server" Visible="True" Width="100%" OnCheckedChanged="onCheck" AutoPostBack="true"/></td>
            </tr>
        </table>
        <asp:Button ID="btnConfermaDiritti" runat="server" Text="Conferma"/>
    </asp:Panel>
</asp:Content>
