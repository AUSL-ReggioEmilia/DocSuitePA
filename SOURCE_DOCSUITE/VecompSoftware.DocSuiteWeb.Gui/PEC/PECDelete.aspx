<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="PECDelete.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.PECDelete" %>

<%@ Register Src="~/PEC/uscPECInfo.ascx" TagPrefix="usc" TagName="uscPECInfo" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <table class="datatable">
        <tr>
            <th>Motivazione della cancellazione:</th>
        </tr>
        <tr>
            <td>
                <telerik:RadTextBox runat="server" TextMode="MultiLine" Width="100%" ID="txtDeleteNotes" EmptyMessage="Inserisci qui la motivazione" />
            </td>
        </tr>
    </table>
    <asp:HiddenField runat="server" ID="hiddenPecId" />
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            function openWarningWindow() {
                radalert('Alcuni elementi selezionati non possono essere eliminati e sono stati tolti dalla lista.', 300, 100, 'Info', '', "../App_Themes/DocSuite2008/imgset32/information.png");
            }
        </script>        
    </telerik:RadScriptBlock>
    <asp:Label ID="ErrLabel" runat="server" Style="color: Red;" Text="" Visible="False" />
    <telerik:RadGrid runat="server" ID="PecGrid" Width="100%">
        <MasterTableView AutoGenerateColumns="False" DataKeyNames="ID">
            <Columns>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Dettagli" UniqueName="deletable">
                    <ItemTemplate>
                        <usc:uscPECInfo ID="uscPECInfo" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Cancellabile" UniqueName="deletable">
                    <HeaderStyle HorizontalAlign="Center" Width="1%" />
                    <ItemStyle HorizontalAlign="Center" Width="1%" />
                    <ItemTemplate>
                        <asp:Image runat="server" ID="imgDeletable" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>

        <ClientSettings EnableRowHoverStyle="False">
            <Selecting AllowRowSelect="False"></Selecting>
        </ClientSettings>
    </telerik:RadGrid>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Button runat="server" ID="cmdOk" Text="Conferma" Width="150" />
</asp:Content>
