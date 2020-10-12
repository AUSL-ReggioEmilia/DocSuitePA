<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UtltProtSincronizza.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltProtSincronizza" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Protocollo - Allineamento Protocolli TD"  %>

<asp:Content runat="server" ContentPlaceHolderID="cphcontent">
    <telerik:RadWindowManager EnableViewState="False" ID="windowManager" PreserveClientState="True" runat="server">
        <Windows>
            <telerik:RadWindow Height="550" ID="wndResult" runat="server" Title="Importazione - Risultati" Width="700" />
            <telerik:RadWindow Behaviors="None" Height="550" ID="wndProgress" runat="server" Width="700" />
        </Windows>
    </telerik:RadWindowManager>
   <telerik:RadScriptBlock runat="server" ID="RadScriptBlockTRV">
        <script type="text/javascript" language="javascript">
            function onTaskCompleted(sender, args) {
                var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                ajaxManager.ajaxRequest("");
            }
        </script>
    </telerik:RadScriptBlock>

<asp:Panel runat="server" ID="pnlGrid" CssClass="radGridWrapper">
    <DocSuite:BindGrid ID="gvProtocols" runat="server" Width="100%" AutoGenerateColumns="False" GridLines="Both">
        <MasterTableView TableLayout="Auto" NoMasterRecordsText="Nessun Protocollo Trovato" GridLines="Both">
                <Columns>
                    <DocSuite:YearNumberBoundColumn HeaderText="Protocollo" CurrentFilterFunction="EqualTo" UniqueName="Id" SortExpression="Id">
                        <HeaderStyle horizontalalign="Center" width="98px" />
                        <ItemStyle horizontalalign="center" width="98px" />
                        <ItemTemplate>
		    		        <asp:LinkButton runat="server" ID="lnkProtocol" Text='<%# Eval("Protocol")%>' CommandName="ShowProt" CommandArgument='<%# Eval("UniqueId") %>'></asp:LinkButton>
				        </ItemTemplate>
                    </DocSuite:YearNumberBoundColumn>
                    <telerik:GridDateTimeColumn DataField="RegistrationDate" HeaderText="Data registrazione" DataFormatString="{0:dd/MM/yyyy}" CurrentFilterFunction="EqualTo" UniqueName="RegistrationDate" SortExpression="RegistrationDate"> 
                        <HeaderStyle horizontalalign="Center" wrap="false" width="125px"/>
                        <ItemStyle horizontalalign="Center" width="125px"/>
                    </telerik:GridDateTimeColumn>
			        <DocSuite:SuggestFilteringColumn DataField="Type.ShortDescription" HeaderText="I/U" CurrentFilterFunction="EqualTo" UniqueName="Type.Id" SortExpression="Type.Description" DataType="System.Int32">
			            <HeaderStyle  horizontalalign="Center" width="45px" />
			            <ItemStyle horizontalalign="Center" width="45px"/>
			        </DocSuite:SuggestFilteringColumn>
			         <telerik:GridBoundColumn DataField="ContainerName" HeaderText="Contenitore" CurrentFilterFunction="Contains" UniqueName="Container.Name" SortExpression="Container.Name">
	    			    <HeaderStyle horizontalalign="Center" wrap="true" width="200px" />
	    			    <ItemStyle horizontalalign="Left" />
			        </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="ProtocolObject" HeaderText="Oggetto" CurrentFilterFunction="Contains" UniqueName="ProtocolObject" SortExpression="ProtocolObject">
                        <HeaderStyle horizontalalign="Center" wrap="true" width="200px" />
                        <ItemStyle horizontalalign="Left" width="200px" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="TDError" HeaderText="TD Errore" CurrentFilterFunction="Contains" UniqueName="TDError" SortExpression="TDError">
                        <HeaderStyle horizontalalign="Center" wrap="false" />
                        <ItemStyle horizontalalign="Left" />
                    </telerik:GridBoundColumn>
                </Columns>
        </MasterTableView>
        <clientsettings>
            <Selecting AllowRowSelect="true" />
         </clientsettings>
    </DocSuite:BindGrid>
</asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:button id="btnImport" runat="server" Width="150px" Text="Importa"></asp:button>
</asp:Content>
