<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="PECMailBoxCaselleMonitor.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.PECMailBoxCaselleMonitor" Title="PEC - Monitor" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Label CssClass="titolo" ID="lblPecMailBoxTitle" runat="server" Text="Monitoraggio su Caselle PEC " />
    <telerik:RadGrid runat="server" ID="grdInfoPecMail" Width="100%">
        <MasterTableView AutoGenerateColumns="False" TableLayout="Auto">
            <Columns>
                <telerik:GridTemplateColumn UniqueName="CasellaPec" HeaderText="Casella PEC">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblCasellaPec" Enabled="true" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="LastProcess" HeaderText="Ultimo Processo">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblLastProcess" Enabled="true" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="LastProcessNoError" HeaderText="Ultimo Processo senza errori">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblLastProcessNoErrors" Enabled="true" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="PECInDrop" HeaderText="Numero PEC in Drop">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblPECInDrop" Enabled="true" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="PECInError" HeaderText="Numero PEC in Errore">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblPECInError" Enabled="true" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings />
    </telerik:RadGrid>

</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
</asp:Content>
