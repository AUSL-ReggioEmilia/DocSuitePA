<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DocmAtti.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmAtti" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscResolutionSelect.ascx" TagName="UscResolutionSelect" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscDocumentFolder.ascx" TagName="UscDocumentFolder" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock2">
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
        </script>
    </telerik:RadScriptBlock>

    <table class="datatable">
        <tr>
            <th align="left" colspan="2">Informazioni</th>
        </tr>
        <tr>
            <td style="width: 155px" class="label">Motivo:</td>
            <td>
                <telerik:RadTextBox ID="txtReason" runat="server" Width="100%" MaxLength="255"></telerik:RadTextBox>
            </td>
        </tr>
        <tr>
            <td style="width: 155px" class="label">Note:</td>
            <td>
                <telerik:RadTextBox ID="txtNote" runat="server" Width="100%" MaxLength="255"></telerik:RadTextBox>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <usc:UscResolutionSelect runat="server" ID="uscResolutionSelect" />
    <table class="datatable" id="tbFolders" visible="false" runat="server">
        <tr>
            <th>Cartelle:</th>
        </tr>
        <tr>
            <td>
                <telerik:RadTreeView ID="Tvw" Width="100%" runat="server" EnableViewState="true">
                    <CollapseAnimation Duration="100" Type="OutQuint" />
                    <ExpandAnimation Duration="100" Type="None" />
                    <Nodes>
                    </Nodes>
                </telerik:RadTreeView>
                <br />
                <telerik:RadTreeView ID="tvwSettoriCC" ExpandAnimation-Type="None" LoadingStatusPosition="BeforeNodeText" Width="100%" runat="server" EnableViewState="true">
                    <CollapseAnimation Duration="100" Type="OutQuint" />
                    <ExpandAnimation Duration="100" Type="None" />
                </telerik:RadTreeView>
            </td>
        </tr>
    </table>

    <asp:Panel ID="pnlCartella" runat="server">
        <table style="border-right: gray 1px solid; border-top: gray 1px solid; border-left: gray 1px solid; width: 100%; border-bottom: gray 1px solid; border-collapse: collapse; border-color: Gray;"
            cellspacing="0" cellpadding="3" border="0" class="datatable">
            <tr>
                <th>Cartella Del Documento</th>
            </tr>

            <tr>
                <td>
                    <usc:UscDocumentFolder ID="uscDocumentFolderProt" runat="server" Type="Resl" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnInserimento" runat="server" Text="Conferma" Enabled="False" />
    <asp:Button ID="btnModifica" runat="server" Text="Modifica" />
    <asp:Button ID="btnCancella" runat="server" Text="Cancella" />
    <asp:TextBox runat="server" ID="idRole" CssClass="hiddenField" />
        <span class="hiddenField" runat="server" id="misteryBox">
        <asp:TextBox ID="txtUserStep" runat="server" CssClass="hiddenField" />
        <asp:TextBox ID="txtUserAccount" runat="server" CssClass="hiddenField" />
        <asp:TextBox ID="txtCCidOwner" runat="server" CssClass="hiddenField" />
        <asp:TextBox ID="txtPStep" runat="server" CssClass="hiddenField" />
        <asp:TextBox ID="txtPIdOwner" runat="server" CssClass="hiddenField" />
        <asp:TextBox ID="txtRRStep" runat="server" CssClass="hiddenField" />
        <asp:TextBox ID="txtRRIdOwner" runat="server" CssClass="hiddenField" />
        <asp:TextBox ID="txtPRStep" runat="server" CssClass="hiddenField" />
        <asp:TextBox ID="txtPRIdOwner" runat="server" CssClass="hiddenField" />
        <asp:TextBox ID="txtRStep" runat="server" CssClass="hiddenField" />
        <asp:TextBox ID="txtRIdOwner" runat="server" CssClass="hiddenField" />
        <asp:TextBox ID="txtRNStep" runat="server" CssClass="hiddenField" />
        <asp:TextBox ID="txtRNIdOwner" runat="server" CssClass="hiddenField" />
        <asp:TextBox ID="txtCCStep" runat="server" CssClass="hiddenField" />
        <asp:TextBox ID="txtIdRoleRightM" runat="server" CssClass="hiddenField" />
        <asp:TextBox ID="txtIdRoleRightW" runat="server" CssClass="hiddenField" />
        <asp:TextBox ID="txtIdRoleRight" runat="server" CssClass="hiddenField" />
    </span>
</asp:Content>