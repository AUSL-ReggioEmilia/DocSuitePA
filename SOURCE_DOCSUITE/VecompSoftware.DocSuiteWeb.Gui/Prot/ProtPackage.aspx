<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtPackage" Codebehind="ProtPackage.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Protocollo - Scatoloni" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="rsb" EnableViewState="false">
        <script language="javascript" type="text/javascript">

            function CloseData(sender, args) {
                if (args.get_argument() !== null) {
                    var ajaxManager = $find("<%= AjaxManager.ClientID%>");
                    ajaxManager.ajaxRequest('Refresh');
                }
            }

            function OnClientClicking(sender, args) {
                if (args._commandName === "DeleteItem") {
                    var callBackFunction = Function.createDelegate(sender, function(shouldSubmit) {
                        if (shouldSubmit) {
                            this.click();
                        }
                    });
                    radconfirm("Vuoi eliminare lo scatolone?", callBackFunction, 300, 100, null, "Eliminazione");
                    args.set_cancel(true);
                }
            }

            function EditClick(sender, args) {
                if (args._commandName === "EditItem" && args._commandArgument !== null) {
                    OpenEditWindow('windowEditPackage', 500, 260, args._commandArgument);
                }
            }

            function OpenEditWindow(name, width, height, params) {
                var action = params != undefined && params != "" ? "Modifica" : "Nuovo";
                return OpenWindow("../Prot/ProtPackageGes.aspx?Action=" + action + params, name, width, height);
            }

            function OpenWindow(url, name, width, height) {
                var manager = $find("<%=rwmPackages.ClientID %>");
                var wnd = manager.open(url, name);
                wnd.setSize(width, height);
                wnd.center();
                return false;
            }
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager EnableViewState="False" ID="rwmPackages" runat="server">
        <Windows>
            <telerik:RadWindow ID="windowEditPackage" OnClientClose="CloseData" runat="server" Title="Gestione Scatoloni" />
        </Windows>
    </telerik:RadWindowManager>  
        
    <%-- griglia --%>
    <div style="overflow:hidden;width:100%;height:100%;">
        <DocSuite:BindGrid AllowMultiRowSelection="False" AutoGenerateColumns="false" EnableViewState="true" ID="gvPackages" runat="server" Visible="true">
            <MasterTableView AllowFilteringByColumn="True" DataKeyNames="Origin,Package" GridLines="Both" NoMasterRecordsText="Nessun scatolone trovato" TableLayout="Auto">
                <Columns>
                    <DocSuite:SuggestFilteringColumn DataField="Origin" UniqueName="Origin" SortExpression="Origin" DataType="System.Char" CurrentFilterFunction="Contains" HeaderText="Tipo">
                        <HeaderStyle HorizontalAlign="Center" width="5%" />
                        <ItemStyle HorizontalAlign="Center" width="5%"/>                    
                    </DocSuite:SuggestFilteringColumn> 
                    <telerik:GridBoundColumn DataField="Account" UniqueName="Account" SortExpression="Account" DataType="System.String" CurrentFilterFunction="Contains" HeaderText="Utente">
                        <HeaderStyle HorizontalAlign="Center" width="10%" />
                        <ItemStyle HorizontalAlign="Left" width="10%"/>                    
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Package" UniqueName="Package" DataFormatString="{0:000000}" SortExpression="Package" DataType="System.Int32" CurrentFilterFunction="EqualTo" HeaderText="Scatolone">
                        <HeaderStyle HorizontalAlign="Center" width="10%" />
                        <ItemStyle HorizontalAlign="Right" width="10%"/>                    
                    </telerik:GridBoundColumn> 
                    <telerik:GridBoundColumn DataField="Lot" UniqueName="Lot" DataFormatString="{0:000000}" HeaderText="Lotto" SortExpression="Lot" DataType="System.Int32" CurrentFilterFunction="EqualTo">
                        <HeaderStyle HorizontalAlign="Center" width="10%" />
                        <ItemStyle HorizontalAlign="Right" width="10%" />                    
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Incremental" UniqueName="Incremental" HeaderText="Nr. Documenti per Lotto" DataFormatString="{0:000}" SortExpression="Incremental" DataType="System.Int32" CurrentFilterFunction="EqualTo">
                        <HeaderStyle HorizontalAlign="Center" width="10%" />
                        <ItemStyle HorizontalAlign="Right" width="10%" />                    
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="TotalIncremental" UniqueName="TotalIncremental" HeaderText="Nr. Documenti per Scat." DataFormatString="{0:000}" SortExpression="Incremental" DataType="System.Int32" CurrentFilterFunction="EqualTo">
                        <HeaderStyle HorizontalAlign="Center" width="10%" />
                        <ItemStyle HorizontalAlign="Right" width="10%" />                    
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="MaxDocuments" UniqueName="MaxDocuments" HeaderText="Documenti Consentiti" SortExpression="MaxDocuments" DataType="System.Int32" CurrentFilterFunction="EqualTo">
                        <HeaderStyle HorizontalAlign="Center" width="10%" />
                        <ItemStyle HorizontalAlign="Right" width="10%" />                    
                    </telerik:GridBoundColumn>
                    <DocSuite:SuggestFilteringColumn DataField="State" UniqueName="State" HeaderText="Stato" SortExpression="State" DataType="System.Char" CurrentFilterFunction="EqualTo">
                        <HeaderStyle HorizontalAlign="Center" width="5%" />
                        <ItemStyle HorizontalAlign="Center" width="5%" />                    
                    </DocSuite:SuggestFilteringColumn>
                    <telerik:GridTemplateColumn HeaderText="Elimina Scatolone" AllowFiltering="false" Groupable="False">
                        <HeaderStyle HorizontalAlign="Center" width="5%" />
                        <ItemStyle HorizontalAlign="Center" width="5%" />                
                        <ItemTemplate>
                            <telerik:radButton CommandName="DeleteItem" Height="16px" ID="btnDelete" OnClientClicking="OnClientClicking" Runat="server" ToolTip="Elimina" Width="16px" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Modifica Scatolone" AllowFiltering="false" Groupable="False">
                        <HeaderStyle HorizontalAlign="Center" width="5%" />
                        <ItemStyle HorizontalAlign="Center" width="5%"/>                
                        <ItemTemplate>
                            <telerik:RadButton CommandName="EditItem" Height="16px" ID="btnEdit" OnClientClicking="EditClick" runat="server" ToolTip="Modifica" Width="16px" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </DocSuite:BindGrid>
    </div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <input type="button" class="button" id="btnNuovo" value="Nuovo Scatolone" onclick="return OpenEditWindow('windowEditPackage',500,260,'');" causesvalidation="false" />
</asp:Content>