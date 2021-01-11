<%@ Page Title="Gestione modulo JeepService" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="PECMailBoxJeepService.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.PecMailBoxJeepService" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel runat="server" ID="pnlPageContent">
        <asp:Label CssClass="titolo" ID="lblPecMailBoxTitle" Width="100%" runat="server" Text="Elenco JeepService disponibili" />
        <telerik:RadGrid runat="server" ID="grdJeepService">
            <MasterTableView AutoGenerateColumns="False" DataKeyNames="Id" TableLayout="Auto">
                <Columns>
                    <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn">
                        <ItemStyle Width="20px" />
                    </telerik:GridClientSelectColumn>
                    <telerik:GridTemplateColumn UniqueName="id" HeaderText="ID JeepService">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblId" Enabled="true" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="hostname" HeaderText="Hostname JeepService">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblHostname" Enabled="true" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="isActive" HeaderText="Stato JeepService" HeaderStyle-Width="10px">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:Image ID="chkIsActive" ImageUrl="~/App_Themes/DocSuite2008/imgset16/cancel.png" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="isDefault" HeaderText="Default JeepService" HeaderStyle-Width="10px">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:Image ID="chkIsDefault" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings Selecting-AllowRowSelect="true" />
        </telerik:RadGrid>
    </asp:Panel>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnActivate" runat="server" Text="Attiva" Width="150" Visible="false" />
    <asp:Button ID="btnDeactivate" runat="server" Text="Disattiva" Width="150" Visible="false" />
    <asp:Button ID="btnSetAsDefault" runat="server" Text="Imposta default" Width="150" />
</asp:Content>
