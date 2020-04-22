<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="PECMailBoxSettings.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.PecMailBoxSettings" Title="PEC - Gestione caselle PEC" %>

<%@ Register Src="~/UserControl/uscSelLocation.ascx" TagName="SelLocation" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadCodeBlock ID="RadCodeBlock" runat="server">
        <script type="text/javascript" language="javascript">
            function getAjaxManager() {
                return $find("<%= RadAjaxManager.GetCurrent(Page).ClientID%>");
            }

            function AjaxRequest(request) {
                var manager = getAjaxManager();
                manager.ajaxRequest(request);
                return false;
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>
<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadScriptBlock runat="server">
        <style>
            div.RadGrid_Office2007 input {
                background-color: white;
            }
        </style>
    </telerik:RadScriptBlock>
    <asp:Panel runat="server" ID="pnlPageContent">
        <asp:Label CssClass="titolo" ID="lblPecMailBoxTitle" Width="100%" runat="server" Text="Caselle PEC / Protocollazione" />
        <telerik:RadGrid ID="grdPecMailBoxes" EnableScrolling="true" runat="server" AllowFilteringByColumn="true" AutoGenerateColumns="false" AllowMultiRowSelection="false" GridLines="Both">
            <MasterTableView AutoGenerateColumns="False" DataKeyNames="Id" TableLayout="Auto" GridLines="Both">
                <Columns>
                    <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn" HeaderStyle-Width="25px">
                        <ItemStyle Width="20px" />
                    </telerik:GridClientSelectColumn>

                    <telerik:GridTemplateColumn UniqueName="Id" DataField="Id" HeaderText="Id" HeaderStyle-Width="25px" AllowFiltering="false">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblId" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>

                    <telerik:GridTemplateColumn UniqueName="PECCount" HeaderText="Traffico" HeaderStyle-Width="40px" AllowFiltering="false">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblPECCount" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>

                    <telerik:GridBoundColumn UniqueName="MailBoxName" DataField="MailBoxName" HeaderText="Nome Casella" HeaderStyle-Width="150px" FilterControlWidth="90%" 
                        ShowFilterIcon="false" CurrentFilterFunction="Contains" AutoPostBackOnFilter="true" />
                    <telerik:GridTemplateColumn UniqueName="IncomingServerType" HeaderText="IN: Tipo Server" HeaderStyle-Width="70px" AllowFiltering="false">
                        <ItemTemplate>
                            <asp:DropDownList runat="server" ID="ddlProtocolIn" Width="100%" Enabled="False">
                                <Items>
                                    <asp:ListItem Text="" Value="" />
                                    <asp:ListItem Text="POP3" Value="0" />
                                    <asp:ListItem Text="IMAP" Value="1" />
                                </Items>
                            </asp:DropDownList>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn UniqueName="IncomingServerName" DataField="IncomingServerName" HeaderText="IN: Nome server" HeaderStyle-Width="90px" AllowFiltering="false" />
                    <telerik:GridBoundColumn UniqueName="IncomingServerPort" DataField="IncomingServerPort" HeaderText="IN: Porta" HeaderStyle-Width="35px" AllowFiltering="false" />
                    <telerik:GridBoundColumn UniqueName="OutgoingServerName" DataField="OutgoingServerName" HeaderText="OUT: Nome server" HeaderStyle-Width="90px" AllowFiltering="false" />
                    <telerik:GridBoundColumn UniqueName="OutgoingServerPort" DataField="OutgoingServerPort" HeaderText="OUT: Porta" HeaderStyle-Width="35px" AllowFiltering="false" />
                    <telerik:GridTemplateColumn UniqueName="MailBoxProfile" HeaderText="Profilo" HeaderStyle-Width="150px" AllowFiltering="false">
                        <ItemTemplate>
                            <asp:DropDownList runat="server" ID="ddlProfile" Width="100%" Enabled="False" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="IncomingHost" HeaderText="IN: Modulo JeepService Associato" HeaderStyle-Width="75px" AllowFiltering="false">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblModuleJeepServiceIncoming" Enabled="true" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="OutgoingHost" HeaderText="OUT: Modulo JeepService Associato" HeaderStyle-Width="75px" AllowFiltering="false">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblModuleJeepServiceOutgoing" Enabled="true" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="HumanEnabled" HeaderText="HumanEnabled" AllowFiltering="False" HeaderStyle-Width="75px">
                        <ItemTemplate>
                            <asp:CheckBox runat="server" ID="chkHumanEnabled" Enabled="False" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings Selecting-AllowRowSelect="true" Resizing-AllowColumnResize="true" />
        </telerik:RadGrid>
    </asp:Panel>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel ID="pnlButtons" runat="server">
        <asp:Button ID="cmdUpdatePEC" runat="server" Text="Modifica casella PEC" Width="150" />
        <asp:Button ID="cmdAddPECMailBox" runat="server" Text="Aggiungi casella PEC" Width="150" />
    </asp:Panel>

</asp:Content>
