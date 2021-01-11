<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="DocmFile.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmFile" %>

<%@ Register Src="../UserControl/uscDocumentUpload.ascx" TagName="uscDocumentUpload" TagPrefix="uc1" %>
<%@ Register Src="../UserControl/uscDocumentDati.ascx" TagName="uscDocumentDati" TagPrefix="uc2" %>
<%@ Register Src="../UserControl/uscDocumentFolder.ascx" TagName="uscDocumentFolder" TagPrefix="uc3" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript" language="javascript">
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow(value) {
                var oWindow = GetRadWindow();
                if (oWindow != null)
                    oWindow.close(value);
            }

            function <%= Me.ID %>_OpenWindowMailCC(name, param) {
                var url = "../Docm/DocmMailCC.aspx?" + param;
                CloseWindow();
                var parentPage = GetRadWindow().BrowserWindow;
                var parentRadWindowManager = parentPage.$find("<%=ManagerID %>");
               parentRadWindowManager.open(url, name);
               parentRadWindowManager.set_modal(true);
               return false;
           }
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager EnableViewState="False" ID="alertManager" runat="server">
        <Windows>
            <telerik:RadWindow Height="300" ID="wndMailCC" runat="server" Width="600" />
        </Windows>
    </telerik:RadWindowManager>

    <uc2:uscDocumentDati ID="UscDocumentDati1" runat="server" Type="Docm" HeaderText="Informazioni" />
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <uc1:uscDocumentUpload ID="UscDocumentUpload1" Caption="Documento" runat="server" Type="Docm" />
    <asp:Panel ID="pnlCartella" runat="server">
        <table style="border-right: gray 1px solid; border-top: gray 1px solid; border-left: gray 1px solid; width: 100%; border-bottom: gray 1px solid; border-collapse: collapse"
            bordercolor="gray"
            cellspacing="0" cellpadding="3" border="0" class="datatable">
            <tr>
                <th>Cartella Del Documento</th>
            </tr>
            <tr>
                <td>
                    <uc3:uscDocumentFolder ID="UscDocumentFolder1" runat="server" Type="Docm" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <DocSuite:PromptClickOnceButton ID="btnInserimento" runat="server" Text="Conferma" DisableAfterClick="true" ConfirmBeforeSubmit="false" />
    <asp:Button ID="btnModifica" runat="server" Text="Modifica" />
    <asp:Button ID="btnCancella" runat="server" Text="Cancella" />
    <asp:Button ID="btnCheckOut" runat="server" Text="Check Out" Visible="False" />
    <asp:Button ID="btnCancelCheckOut" runat="server" Text="Annulla Check Out" Visible="False" />
    <asp:Button ID="btnMailCC" runat="server" Text="Invia Mail" Visible="False" />
    <asp:Button ID="btnCheckIn" runat="server" Text="Check In" Visible="False" />
    <asp:TextBox ID="Documento" runat="server" Width="16px" AutoPostBack="True" />
    <asp:TextBox ID="DocumentoDes" runat="server" Width="16px" AutoPostBack="True" />
    <asp:ValidationSummary DisplayMode="List" ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" />
</asp:Content>
