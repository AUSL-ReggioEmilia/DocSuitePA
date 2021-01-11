<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ProtMultiAutorizza.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtMultiAutorizza"
    MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Protocollo - Multi-Autorizzazione di protocollo" %>

<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="uscSettori" TagPrefix="usc" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlockTRV">

        <script type="text/javascript" language="javascript">            	    

            function ShowLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= protocolContainer.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);
            }

            function callBackFunction(arg) {
                ShowLoadingPanel();
                window.location.href = document.referrer;
            }
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager ID="RadWindow" Width="300" Height="100" runat="server">
    </telerik:RadWindowManager>

    <asp:panel style="width: 100%" runat="server" ID="protocolContainer">
        <div>
            <asp:Label ID="lblProtocolListCount" runat="server" Font-Bold="true" />
        </div>
        <table id="tblProtocolList" cellspacing="1" cellpadding="1" width="100%" border="0" >
            <tr>
                <td>
                    <telerik:RadGrid runat="server" ID="GridProtocols" Width="100%" AllowMultiRowSelection="True" EnableViewState="true">
                        <MasterTableView AutoGenerateColumns="False">
                            <Columns>
                                <telerik:GridClientSelectColumn UniqueName="SelectColumn" HeaderText="Autorizza" HeaderTooltip="Protocolli da autorizzate" ItemStyle-Width="20px" />
                                <telerik:GridTemplateColumn UniqueName="ProtocolDescription" HeaderText="Protocollo da distribuire">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="ProtocolDescription" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn Visible="false">
                                    <ItemTemplate>
                                        <asp:HiddenField runat="server" ID="ProtocolId" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                        <ClientSettings EnableRowHoverStyle="true" Selecting-AllowRowSelect="true" Selecting-UseClientSelectColumnOnly="true" />
                    </telerik:RadGrid>
                </td>
            </tr>
        </table>
        <table id="tblDati" cellspacing="1" cellpadding="1" width="100%" border="0">
            <tr>
                <td>
                    <usc:uscSettori ID="uscAutorizza" runat="server" Caption="Autorizzazione" Type="Prot" MultiSelect="true" Required="false" MultipleRoles="true" />
                </td>
            </tr>
        </table>
        <table id="tblProtocolRoleUser" cellspacing="1" cellpadding="1" width="100%" border="0">
            <tr>
                <td>
                    <usc:uscSettori ID="uscProtocolRoleUser" runat="server" ReadOnly="true" MultipleRoles="true" />
                </td>
            </tr>
        </table>
    </asp:panel>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="cphFooter">
    <table cellspacing="1" cellpadding="1" width="100%" border="0">
        <tr>
            <td>
                <asp:Button ID="btnConferma" runat="server" Text="Conferma modifica" />
            </td>
        </tr>
    </table>
</asp:Content>
