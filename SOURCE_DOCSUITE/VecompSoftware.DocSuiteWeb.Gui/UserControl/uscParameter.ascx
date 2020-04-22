<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscParameter.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscParameter" %>


    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">
            function openModalMessage() {
                var oWnd = $find("<%=modalMessage.ClientID%>");
                oWnd.show();
            }
            
            function openEditWindow() {
                var oWnd = $find("<%= parameterModal.ClientID%>");
                oWnd.setSize(700, 550);
                oWnd.show();
            }

            function closeEditWindow() {
                var oWnd = $find("<%= parameterModal.ClientID%>");
                oWnd.close();
            }
            
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindow runat="server" ID="parameterModal" Title="Modifica parametro">
        <ContentTemplate>
            <table class="datatable">
                <tr>
                    <td class="label">Chiave:</td>
                    <td>
                        <asp:Label runat="server" ID="lblEditKey" /></td>
                </tr>
                <tr>
                    <td class="label">Default:</td>
                    <td>
                        <asp:Label runat="server" ID="lblDefaultValue" />
                    </td>
                </tr>
                <tr>
                    <td class="label">Note:</td>
                    <td>
                        <asp:Label ID="lblNote" runat="server" />
                    </td>
                </tr>
            </table>
            <div class="titolo">Nuovo Valore</div>
            <telerik:RadEditor AutoResizeHeight="true" EditModes="Html" EnableResize="true" Height="150px" ID="txtEditValue" runat="server" Width="100%">
                <Tools>
                    <telerik:EditorToolGroup>
                    </telerik:EditorToolGroup>
                </Tools>
            </telerik:RadEditor>
            <asp:Button Text="Cancella Chiave" ID="btnDelete" runat="server" />
            <asp:Button Text="Salva" ID="btnClose" runat="server" />
        </ContentTemplate>
    </telerik:RadWindow>
    
    <telerik:RadWindow runat="server" ID="modalMessage" Title="Parametri ridondanti">
        <ContentTemplate>
            <asp:UpdatePanel ID="messagePanel" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    I parametri ridondanti, compresi quelli di istanza, che <span style="font-weight: bold;">potrebbero</span> essere eliminati dal database: 
                    <asp:Repeater ID="rptParams" runat="server">
                        <HeaderTemplate>
                            <ul>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li><%# Container.DataItem.ToString()%></li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                    </asp:Repeater>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

<div style="width: 100%;">
    <asp:Panel runat="server" DefaultButton="refresh">
        <table class="dataform">
            <tr>
                <td class="label" style="width: 20%">
                    Istanza
                </td>
                <td>
                    <asp:label runat="server" ID="lblInstance" />
                </td>
                <td rowspan="3" colspan="4">
                    <ul>
                        <li><i>Corsivo</i>: Chiave non impostata, vale il valore di default indicato.</li>
                        <li>Normale: Chiave impostata sul valore di default.</li>
                        <li><strong>Grassetto</strong>: Chiave impostata sul valore differente dal default.</li>
                    </ul>
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 20%">
                    Ambiente
                </td>
                <td>
                    <asp:RadioButtonList AutoPostBack="True" ID="environmentsRadioButtonList" RepeatDirection="Horizontal" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 20%">
                    Testo
                </td>
                <td>
                    <asp:TextBox ID="searchText" runat="server" />
                    <asp:CheckBox ID="includeKey" runat="server" Text="Chiave" Checked="true" />
                    <asp:CheckBox ID="includeDescription" runat="server" Text="Descrizione" />
                    <asp:CheckBox ID="includeNote" runat="server" Text="Note" />
                </td>
            </tr>
            <tr>
                <td class="label">
                    Gruppo
                </td>
                <td>
                    <asp:DropDownList ID="groups" runat="server" />
                </td>
                 <td class="label">
                    Rilasciato in Versione
                </td>
                <td>
                    <asp:DropDownList ID="versions" runat="server" />
                </td>
                 <td class="label">
                    Cliente
                </td>
                <td>
                    <asp:DropDownList ID="customers" runat="server" />
                </td>               
              
            </tr>
        </table>
        <telerik:radButton ID="refresh" runat="server" Text="Cerca" />
        <telerik:radbutton AutoPostBack="true" ID="showWrong" runat="server" Text="Mostra parametri ridondanti" OnClientClicked="openModalMessage" />
    </asp:Panel>

        <telerik:RadGrid AutoGenerateColumns="False" GridLines="Vertical" ID="dgParameterEnv" Width="100%" runat="server">
            <MasterTableView TableLayout="Fixed">
                <Columns>
                    <telerik:GridTemplateColumn HeaderText="">
                        <ItemTemplate>
                            <telerik:RadButton AutoPostBack="true" CommandName="prova" ID="btnEdit" runat="server" Text="Modifica" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Chiave" UniqueName="Key">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblKey" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Valore" UniqueName="Value">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblValue" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="Description" ForceExtractValue="None" HeaderText="Descrizione" UniqueName="Description" />
                    <telerik:GridBoundColumn DataField="Note" ForceExtractValue="None" HeaderText="Note" UniqueName="Note" />
                    <telerik:GridBoundColumn DataField="Version" ForceExtractValue="None" HeaderText="Rilasciato in Versione" UniqueName="Version" />
                    <telerik:GridBoundColumn DataField="Customer" ForceExtractValue="None" HeaderText="Cliente" UniqueName="Customer"/>
                </Columns>
            </MasterTableView>
            <ClientSettings>
            </ClientSettings>
        </telerik:RadGrid>
</div>