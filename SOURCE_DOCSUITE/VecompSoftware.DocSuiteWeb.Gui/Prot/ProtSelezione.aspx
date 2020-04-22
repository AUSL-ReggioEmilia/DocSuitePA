<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master"
    CodeBehind="ProtSelezione.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtSelezione" %>

    <asp:Content runat="server" ContentPlaceHolderID="cphHeader">
        <asp:Panel ID="pnFilter" runat="server">
            <table id="tblFilter" class="dataform">
                <tr>
                    <td class="label">
                        Contenitore:
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlContainer" />
                    </td>
                </tr>
                <tr style="vertical-align: middle;">
                    <td class="label">
                        Data:
                    </td>
                    <td>
                        <telerik:RadDatePicker runat="server" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" ID="dpFrom" />
                        <telerik:RadDatePicker runat="server" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" ID="dpTo" />
                    </td>
                </tr>
                
                <tr>
                    <td class="label">
                        Numero:
                    </td>
                    <td>
                        <telerik:RadTextBox runat="server" ID="txtNumber"></telerik:RadTextBox>
                        <asp:RangeValidator ControlToValidate="txtNumber" Display="Static" ErrorMessage="Inserire un valore tra 1 e 1000000" ID="txtNumberValidator" MaximumValue="1000000" MinimumValue="1" runat="server" Type="Integer" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button runat="server" ID="cmdFilter" Text="Filtra risultati" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript" language="javascript">

            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function <%= Me.ID %>_OpenWindow(url,name,width,height,parameters) {					
                var URL = url;
                URL += ( "?" + parameters );
            
                var manager = $find("<%= RadWindowManagerProt.ClientID %>");
                var wnd = manager.open(URL, name);
                wnd.setSize(width,height);
                wnd.center();

                return false;
            }

            function ToggleSelection(p_checked) {
                var grid = document.getElementById("<%= gvProtocols.ClientID %>");
                for (i=0; i<grid.getElementsByTagName("INPUT").length; i++)
                {
                    grid.getElementsByTagName("INPUT")[i].checked = p_checked;
                }
            }

            function CloseWindow(protocol) {
                var oWindow = GetRadWindow();
                oWindow.close(protocol);
            }
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerProt" runat="server">
        <Windows>
            <telerik:RadWindow Height="100" ID="windowPrintLabel" runat="server" Title="Protocollo - Stampa Etichetta" Width="300"></telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>
    <div style="overflow:hidden;width:100%;height:100%;">
        <DocSuite:BindGrid AllowMultiRowSelection="true" AutoGenerateColumns="False" ID="gvProtocols" runat="server" Width="100%" ShowGroupPanel="True">
            <MasterTableView CommandItemDisplay="None" AllowFilteringByColumn="False" AllowPaging="False" AllowCustomPaging="false" AllowMultiColumnSorting="false" GridLines="None" NoMasterRecordsText="Nessun Protocollo Trovato" TableLayout="Auto" Width="100%">
                <Columns>
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="False" UniqueName="ClientSelectColumn">
                        <HeaderStyle HorizontalAlign="Center" Width="25px" />
                        <ItemStyle HorizontalAlign="Center" Width="25px" />
                        <ItemTemplate>
                            <asp:CheckBox ID="cbSelect" AutoPostBack="False" runat="server"></asp:CheckBox>
                            <asp:HiddenField ID="hdId" runat="server" Value='<%# Eval("Year") & "|" & Eval("Number") %>' />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn AllowFiltering="false" CurrentFilterFunction="EqualTo" Groupable="false" HeaderText="Protocollo" SortExpression="Id" UniqueName="Id">
                        <HeaderStyle HorizontalAlign="Center"  width="150px"  />
                        <ItemStyle HorizontalAlign="center"  width="150px" Wrap="false"  />
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblProtocol" Text='<%# Eval("Protocol")%>'></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn AllowFiltering="false" CurrentFilterFunction="EqualTo" Groupable="false" HeaderText="Protocollo" SortExpression="Id" UniqueName="IdLink">
                        <HeaderStyle HorizontalAlign="Center"  width="150px"  />
                        <ItemStyle HorizontalAlign="center"  width="150px" Wrap="false"  />
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="lnkProtocol" Text='<%# Eval("Protocol")%>' CommandName="ShowProt"
                                CommandArgument='<%# Eval("Year") & "|" & Eval("Number") %>'></asp:LinkButton>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridDateTimeColumn AllowFiltering="false" CurrentFilterFunction="EqualTo" DataField="RegistrationDate" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Data registrazione" SortExpression="RegistrationDate" UniqueName="RegistrationDate">
                        <HeaderStyle HorizontalAlign="Center"  width="150px"  />
                        <ItemStyle HorizontalAlign="center"  width="150px" Wrap="false"  />
                    </telerik:GridDateTimeColumn>
                    <telerik:GridTemplateColumn AllowFiltering="false" CurrentFilterFunction="EqualTo" Groupable="false" HeaderText="Documento" SortExpression="Id" UniqueName="Document" Visible="true">
                        <HeaderStyle HorizontalAlign="Left"  width="100%"  />
                        <ItemStyle HorizontalAlign="Left"  width="100%" Wrap="false"  />
                        <ItemTemplate>
                            <asp:ImageButton ID="imgDocument" runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/file_extension_pdf.png" Visible='<%# HasDocument(Eval("Year"), Eval("Number")) %>' CommandName="View" />
                            <asp:Image runat="server" id="imgNoDocument" ImageUrl="~/Comm/Images/File/Missing16.gif" Visible='<%# Not HasDocument(Eval("Year"), Eval("Number")) %>'/>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </DocSuite:BindGrid>
    </div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel ID="pnAnnulla" runat="server">
        <table id="tblAnnullamento" class="datatable" runat="server">
            <tr>
                <th style="width:885px;">
                    Estremi del provvedimento di annullamento dei Protocolli
                </th>
            </tr>
            <tr>
                <td>
                    <telerik:RadTextBox ID="txtAnnulla" runat="server"  Width="100%" />
                </td>
            </tr>
        </table>
        <asp:Button ID="btnSelectAll" runat="server" Text="Seleziona tutti" Width="120px" CausesValidation="False" OnClientClick="ToggleSelection(true); return false;" />
        <asp:Button ID="btnDeselectAll" runat="server" Text="Annulla selezione" Width="120px" CausesValidation="False" OnClientClick="ToggleSelection(false); return false;" />
        <asp:Button ID="btnPrintDocumentLabel" runat="server" Text="Stampa Etichette Documento" />
        <asp:Button ID="btnPrintAttachmentLabel" runat="server" Text="Stampa Etichette Allegati" />
        <asp:Button ID="btnCancel" runat="server" Text="Annulla Protocolli" />
    </asp:Panel>
</asp:Content>
