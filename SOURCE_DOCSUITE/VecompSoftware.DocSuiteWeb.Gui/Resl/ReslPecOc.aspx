<%@ Page AutoEventWireup="false" CodeBehind="ReslPecOc.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslPecOc" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%-- Header --%>
<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="rsbPecOc" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            function OpenViewer(button, args) {
                if (args._commandArgument === null)
                    return false;
                return OpenWindow(window.radopen("../PEC/PECView.aspx?Type=Pec&vo=true&PECId=" + args._commandArgument, "pecWin"));
            }

            function OpenWindow(wnd) {
                wnd.setSize(700, 500);
                wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
                wnd.set_visibleStatusbar(false);
                wnd.set_modal(true);
                wnd.set_iconUrl("images/mail.gif");
                wnd.add_close(CloseMail);
                wnd.center();

                return wnd;
            }

            function CloseMail(sender, args) {
                sender.remove_close(CloseMail);
                if (args.get_argument() !== null) {
                    var manager = $find("<%= AjaxManager.ClientID%>");
                    manager.ajaxRequest(args.get_argument());
                }
            }
        </script>
    </telerik:RadScriptBlock>
    <telerik:RadWindowManager BackColor="Gray" EnableViewState="False" ID="radWinManager" runat="server">
        <Windows>
            <telerik:RadWindow ID="pecWin" OnClientClose="CloseMail" runat="server" Title="Visualizza Messaggio PEC" />
        </Windows>
    </telerik:RadWindowManager>
    <table style="width: 100%;">
        <tr id="trGridHeader" runat="server">
            <td>
                <div class="titolo" id="divTitolo">
                    <asp:Label ID="lblHeader" runat="server" />
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
<%-- Content --%>
<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <div style="overflow:hidden;width:100%;height:100%" runat="server">
        <DocSuite:BindGrid AllowCustomPaging="true" AllowMultiRowSelection="false" AllowSorting="True" AutoGenerateColumns="False" GridLines="Both" ID="PecOcGrid" runat="server" ShowGroupPanel="True" Visible="true">
            <MasterTableView NoMasterRecordsText="Nessuna richiesta PEC trovata" TableLayout="Auto">
                <Columns>
                    <telerik:GridBoundColumn HeaderStyle-Width="100px" ItemStyle-Width="100px" AllowFiltering="false" AllowSorting="false" DataField="ResolutionType.Description" HeaderText="Tipologia" UniqueName="tipoAtto" />
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../Comm/Images/File/Mail16.gif" HeaderText="Mail" UniqueName="allegato">
                        <HeaderStyle HorizontalAlign="Center" Width="25px" />
                        <ItemStyle HorizontalAlign="Center" Width="25px" />
                        <ItemTemplate>
                            <telerik:radbutton CommandName="Open" ID="allegato" runat="server" Height="16px" Width="16px" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Stato" UniqueName="stato" SortExpression="Status">
                        <HeaderStyle HorizontalAlign="Left" Width="150px" />
                        <ItemStyle HorizontalAlign="Left" Width="150px" />
                        <ItemTemplate>
                            <asp:Label ID="stato" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Date" SortExpression="FromDate" UniqueName="date">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:LinkButton ID="date" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn AllowFiltering="false" HeaderText="Estrai Allegati" UniqueName="allegati">
                        <HeaderStyle HorizontalAlign="Left" Width="100px" />
                        <ItemStyle HorizontalAlign="Left" Width="100px" />
                        <ItemTemplate>
                            <asp:Label ID="extractAttachments" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <SortingSettings SortedAscToolTip="Ordine crescente" SortedDescToolTip="Ordine descrescente" SortToolTip="Ordina" />
            <ClientSettings AllowDragToGroup="True" AllowGroupExpandCollapse="True">
                <Selecting AllowRowSelect="true" />
            </ClientSettings>
        </DocSuite:BindGrid>
        </div>
</asp:Content>
<%-- Footer --%>
<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlFooter">
        <asp:Button runat="server" ID="Nuovo" Text="Nuova richiesta" />
    </asp:Panel>
</asp:Content>
