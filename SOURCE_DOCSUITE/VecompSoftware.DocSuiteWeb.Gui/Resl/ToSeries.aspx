<%@ Page Title="Selezione documenti da esportare" Language="vb" AutoEventWireup="false" CodeBehind="ToSeries.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.Resl.ToSeries" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock ID="radScriptBlock" runat="server" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            function OpenGenericWindow(button, args) {
                if (args._commandArgument === null)
                    return false;

                var wnd = DSWOpenGenericWindow(args._commandArgument, WindowTypeEnum.NORMAL);
                wnd.set_visibleStatusbar(false);
                wnd.set_modal(true);
                wnd.set_showOnTopWhenMaximized(false);
                wnd.set_destroyOnClose(true);
                return wnd;
            }
        </script>
    </telerik:RadScriptBlock>
    <table class="dataform">
        <tr>
            <td class="label" style="width: 30%;">
                <asp:Label runat="server" ID="Name" />
            </td>
            <td style="width: 70%;">
                <asp:Label runat="server" ID="Number" />
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 30%;">
                <asp:Label ID="DocumentSeries" runat="server" />
            </td>
            <td style="width: 70%;">
                <asp:DropDownList runat="server" ID="ddlDocumentSeries" Visible="True" Width="300px" AutoPostBack="true" EnableViewState="True" />
                <asp:RequiredFieldValidator ID="rfvDocumentSeries" ControlToValidate="ddlDocumentSeries" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadGrid runat="server" ID="DocumentListGrid" Width="100%" AllowMultiRowSelection="True" EnableViewState="true">
        <MasterTableView AutoGenerateColumns="False" DataKeyNames="Serialized">
            <Columns>
                <telerik:GridClientSelectColumn UniqueName="selectColumn" HeaderText="Imp." HeaderTooltip="Documenti da esportare" ItemStyle-Width="20px" />
                <telerik:GridTemplateColumn UniqueName="Type" HeaderText="Tipo">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="fileType" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="Description" HeaderText="Documento">
                    <ItemTemplate>
                        <telerik:RadButton Height="16px" ID="fileImage" runat="server" ToolTip="Visualizza anteprima" Width="16px" />
                        <asp:Label runat="server" ID="fileName" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="exportType" HeaderText="Copia conforme">
                    <ItemTemplate>
                        <telerik:RadButton AutoPostBack="false" ButtonType="ToggleButton" Checked="True" ID="pdf" runat="server" ToggleType="CheckBox" ToolTip="Seleziona per la Copia conforme, altrimenti verrà usato l'originale" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="chainType" HeaderText="Catena Documentale">
                    <ItemTemplate>
                        <asp:DropDownList runat="server" ID="chainTypes" Visible="False" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings EnableRowHoverStyle="true" Selecting-AllowRowSelect="true" Selecting-UseClientSelectColumnOnly="true" />
    </telerik:RadGrid>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button runat="server" ID="btnConfirm" Text="Conferma" />
</asp:Content>
