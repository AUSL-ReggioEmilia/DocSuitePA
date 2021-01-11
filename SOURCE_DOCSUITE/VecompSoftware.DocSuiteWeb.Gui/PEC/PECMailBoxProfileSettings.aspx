<%@ Page Title="Gestione profili caselle PEC" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="PECMailBoxProfileSettings.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.PECMailBoxProfileSettings" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel runat="server" ID="pnlPageContent">
        <asp:Label CssClass="titolo" ID="lblPecMailBoxConfigurationsTitle" runat="server" Width="100%" Text="Caselle PEC / Profilo" />
        <telerik:RadGrid EnableScrolling="false" runat="server" AutoGenerateColumns="false" AllowMultiRowSelection="false" 
            GridLines="Both" ID="grdPecMailBoxesConfigurations">
            <MasterTableView AutoGenerateColumns="False" DataKeyNames="Id" TableLayout="Auto">
                <Columns>
                    <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn">
                        <ItemStyle Width="20px" />
                    </telerik:GridClientSelectColumn>
                    <telerik:GridTemplateColumn UniqueName="Id" DataField="Id" HeaderText="Id" HeaderStyle-Width="10px">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblIdConf" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn UniqueName="ConfigurationName" DataField="Name" HeaderText="Nome Configurazione" HeaderStyle-Width="100px" />
                    <telerik:GridBoundColumn UniqueName="MaxReadForSession" DataField="MaxReadForSession" HeaderText="Elementi scaricati per sessione" HeaderStyle-Width="10px" />
                    <telerik:GridBoundColumn UniqueName="MaxSendForSession" DataField="MaxSendForSession" HeaderText="Elementi inviati per sessione" HeaderStyle-Width="10px" />
                    <telerik:GridTemplateColumn UniqueName="UnzipAttachments" HeaderText="Estrazione ZIP" HeaderStyle-Width="10px">
                        <ItemTemplate>
                            <asp:CheckBox runat="server" ID="chkUnzipAttachments" Enabled="False" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn UniqueName="InboxFolder" DataField="InboxFolder" HeaderText="Cartella di download" HeaderStyle-Width="30px" />
                    <telerik:GridTemplateColumn UniqueName="ImapSearchFlag" HeaderText="Flag di ricerca IMAP" HeaderStyle-Width="10px">
                        <ItemTemplate>
                            <asp:DropDownList runat="server" ID="ddlImapSearchFlag" DataTextField="Value" DataValueField="Key" Enabled="False" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings Selecting-AllowRowSelect="true" />
        </telerik:RadGrid>
    </asp:Panel>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel ID="pnlButtons" runat="server">
        <asp:Button ID="cmdUpdatePECProfile" runat="server" Text="Modifica Profilo PEC" Width="150" />
        <asp:Button ID="cmdAddPECProfile" runat="server" Text="Aggiungi Profilo PEC" Width="150" />
    </asp:Panel>
</asp:Content>
