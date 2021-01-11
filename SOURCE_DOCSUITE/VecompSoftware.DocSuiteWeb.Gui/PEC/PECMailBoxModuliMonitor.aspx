<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="PECMailBoxModuliMonitor.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.PECMailBoxModuliMonitor" Title="PEC - Monitor" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Label CssClass="titolo" ID="lblPecMailBoxTitle" runat="server" Text="Monitoraggio su JeepService " />
    <telerik:RadGrid runat="server" ID="grdModuliPec" Width="100%">
        <MasterTableView AutoGenerateColumns="False" TableLayout="Auto">
            <ItemStyle Width="20px" />
            <Columns>
                <telerik:GridTemplateColumn UniqueName="ModuleName" HeaderText="Hostname JeepService">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblHostname" Enabled="true" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="MailBoxName" HeaderText="Casella PEC">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblCasellaPec" Enabled="true" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="LastJSProcessed" HeaderText="Ultimo Processo">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblLastProcess" Enabled="true" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="EvalTime" HeaderText="Durata">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblDurate" Enabled="true" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="Esito" HeaderText="Esito">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblEsito" Enabled="true" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings />
    </telerik:RadGrid>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
</asp:Content>
