<%@ Page AutoEventWireup="false" CodeBehind="UtltProtRicerca.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltProtRicerca" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Protocollo - Ricerca" %>

<%@ Register Src="../UserControl/uscClassificatore.ascx" TagName="uscClassificatore" TagPrefix="uc1" %>

<asp:Content ContentPlaceHolderID="cphHeader" ID="Content1" runat="server">
    <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
        <script type="text/javascript" language="javascript">
            //apertura della finestra Upload Document
            var wnd;

            function <%= Me.ID %>_OpenWindow(url, name, width, height, parameters) {
                var newUrl = url + ("?" + parameters);

                var manager = $find("<%= RadWindowManager.ClientID %>");
                wnd = manager.open(newUrl, name);
                wnd.setSize(width, height);
                wnd.center();

                return false;
            }

            function ClosePopUp() {
                wnd.Close();
            }

            function Refresh() {
                document.getElementById("<%=Search.ClientID %>").click();
            }

            function VisibleCategoryChild() {
                // nascondi riga per ricerca nei sottocontatti
                var row = document.getElementById("<%= trCategoryChild.ClientID %>");
                row.style.display = "";
            }

            function HideCategoryChild() {
                // nascondi riga per ricerca nei sottocontatti
                var row = document.getElementById("<%= trCategoryChild.ClientID %>");
                row.style.display = "none";
            }

            function Esegui(args) {
                var ajaxManager = <%= AjaxManager.ClientID %>;
                ajaxManager.ajaxRequest(args);
            }
            
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager EnableViewState="false" ID="RadWindowManager" runat="server">
        <Windows>
            <telerik:RadWindow ID="windowcontenitore" ReloadOnShow="false" runat="server" />
            <telerik:RadWindow ID="windowcategorie" ReloadOnShow="false" runat="server" />
            <telerik:RadWindow ID="windowautorizzazione" ReloadOnShow="false" runat="server" />
        </Windows>
    </telerik:RadWindowManager>

    <table class="datatable">
        <tr>
            <td class="label" style="width: 15%;">Data:
            </td>
            <td style="width: 75%;">
                <span class="miniLabel">Da</span>
                <telerik:RadDatePicker ID="RegistrationDateFrom" runat="server" />
                <span class="miniLabel">A</span>
                <telerik:RadDatePicker ID="RegistrationDateTo" runat="server" />
            </td>
            <td align="center">
                <b>Reg. Estratte</b></td>
        </tr>
        <tr>
            <td class="label" style="width: 15%;">Locazione:
            </td>
            <td style="width: 75%;">
                <asp:DropDownList ID="ddlLocation" runat="server" />
                <span class="miniLabel">Stato:</span>
                <asp:DropDownList ID="ddlStatus" runat="server">
                    <asp:ListItem></asp:ListItem>
                    <asp:ListItem Value="0">Attivo (0)</asp:ListItem>
                    <asp:ListItem Value="-2">Annullato (-2)</asp:ListItem>
                    <asp:ListItem Value="-1">Errato (-1)</asp:ListItem>
                    <asp:ListItem Value="-5">Errato (-5)</asp:ListItem>
                    <asp:ListItem Value="-3">Sospeso (-3)</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td align="center" style="width: 15%">
                <asp:Label ID="lblCounter" runat="server" Font-Bold="True" />
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 15%;">Contenitore:
            </td>
            <td style="width: 75%;">
                <asp:DropDownList ID="ddlContainer" runat="server" />
                <span class="miniLabel">BiblosDS:</span>
                <asp:TextBox ID="BiblosDS" runat="server" Width="96px" MaxLength="10" />
                <asp:RegularExpressionValidator ControlToValidate="BiblosDS" ErrorMessage="Errore formato" ID="vNumber" runat="server" ValidationExpression="\d*" />
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 15%;">Oggetto:
            </td>
            <td style="width: 75%;">
                <asp:TextBox ID="sObject" runat="server" Width="264px" MaxLength="30" />
                <span class="miniLabel">Utente:</span>
                <asp:TextBox ID="RegistrationUser" runat="server" Width="120px" MaxLength="30" />
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 15%;">Anno:
            </td>
            <td style="width: 75%;">
                <asp:TextBox ID="txtProtYear" runat="server" Width="72px" />
                <asp:RegularExpressionValidator ControlToValidate="txtProtYear" Display="Dynamic" ErrorMessage="Errore formato" ID="vYear" runat="server" ValidationExpression="\d{4}" />
                <span class="miniLabel">Numero:</span>
                <asp:TextBox ID="txtProtNumber" runat="server" Width="100px" />
                <asp:RegularExpressionValidator ControlToValidate="txtProtNumber" ErrorMessage="Errore formato" ID="Regularexpressionvalidator1" runat="server" ValidationExpression="\d*" Width="200px" />
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 15%; vertical-align: top; padding-top: 6px" align="right">Classificazione:
            </td>
            <td style="width: 75%;">
                <uc1:uscClassificatore HeaderVisible="false" ID="UscClassificatore1" Required="false" runat="server" />
            </td>
        </tr>
        <asp:Panel ID="pnlCategorySearch" runat="server">
            <tr runat="server" id="trCategoryChild">
                <td class="label" style="width: 15%;">
                </td>
                <td style="width: 75%;">
                    <asp:CheckBox ID="chbCategoryChild" runat="server" Text="Estendi ricerca alle sottocategorie." />
                </td>
            </tr>
        </asp:Panel>
    </table>
    <asp:Button ID="Search" runat="server" Text="Ricerca" />
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <div class="radGridWrapper">
        <DocSuite:BindGrid AllowSorting="True" AutoGenerateColumns="False" BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical" ID="DG" runat="server">
            <SelectedItemStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
            <MasterTableView NoMasterRecordsText="Nessun Protocollo trovato" AllowFilteringByColumn="True">
                <Columns>
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/fascicle_open.png" HeaderText="Fascicoli" UniqueName="cFascicle">
                        <HeaderStyle HorizontalAlign="Center" Width="25px" />
                        <ItemStyle HorizontalAlign="Center" Width="25px" />
                        <ItemTemplate>
                            <asp:Image runat="server" ID="imgFasc" ImageUrl='<%# SetFascicleImage(Eval("Fascicles"))%>' />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <DocSuite:YearNumberBoundColumn CurrentFilterFunction="EqualTo" Groupable="false" HeaderText="Protocollo" SortExpression="Id" UniqueName="Id">
                        <HeaderStyle HorizontalAlign="Center" Width="98px" />
                        <ItemStyle HorizontalAlign="center" Width="98px" />
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="lnkProtocol" Text='<%# Eval("Protocol")%>' CommandName='<%# "Prot:" & Eval("Year") & Eval("Number")%>'></asp:LinkButton>
                        </ItemTemplate>
                    </DocSuite:YearNumberBoundColumn>
                    <telerik:GridDateTimeColumn CurrentFilterFunction="EqualTo" DataField="RegistrationDate" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Data" SortExpression="RegistrationDate" UniqueName="RegistrationDate">
                        <HeaderStyle HorizontalAlign="Center" Width="125px" />
                        <ItemStyle HorizontalAlign="center" Width="125px" />
                    </telerik:GridDateTimeColumn>
                    <DocSuite:SuggestFilteringColumn CurrentFilterFunction="Contains" DataField="Type.ShortDescription" DataType="System.String" HeaderText="I/U" SortExpression="Type.Description" UniqueName="Type.Description">
                        <HeaderStyle HorizontalAlign="Center" Width="45px" />
                        <ItemStyle HorizontalAlign="Center" Width="45px" />
                    </DocSuite:SuggestFilteringColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="Location.Name" HeaderText="Locazione" SortExpression="Location.Name" UniqueName="Location.Name" />
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="Container.Name" HeaderText="Contenitore" SortExpression="Container.Name" UniqueName="Container.Name">
                        <HeaderStyle Wrap="False"></HeaderStyle>
                    </telerik:GridBoundColumn>
                    <DocSuite:CompositeBoundColumn CurrentFilterFunction="Contains" DataField="CategoryProjection" HeaderText="Class." SortExpression="Category.Name" UniqueName="Category.Name">
                        <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                        <ItemStyle HorizontalAlign="Center" />
                    </DocSuite:CompositeBoundColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="EqualTo" DataField="IdStatus" HeaderText="Stato" SortExpression="IdStatus" UniqueName="IdStatus">
                        <HeaderStyle HorizontalAlign="Center" Width="45px" Wrap="False" />
                        <ItemStyle HorizontalAlign="Center" Width="45px" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="RegistrationUser" HeaderText="Utente" SortExpression="RegistrationUser" UniqueName="RegistrationUser" />
                    <telerik:GridBoundColumn CurrentFilterFunction="EqualTo" DataField="IdDocument" DataType="System.Int32" HeaderText="Doc." SortExpression="IdDocument" UniqueName="IdDocument">
                        <HeaderStyle HorizontalAlign="Right" Width="45px" />
                        <ItemStyle HorizontalAlign="Right" Width="45px" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="EqualTo" DataField="IdAttachments" DataType="System.Int32" HeaderText="All." SortExpression="IdAttachments" UniqueName="IdAttachments">
                        <HeaderStyle HorizontalAlign="Right" Width="45px" />
                        <ItemStyle HorizontalAlign="Right" Width="45px" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="ProtocolObject" HeaderText="Oggetto" SortExpression="ProtocolObject" UniqueName="ProtocolObject" />
                </Columns>
            </MasterTableView>
            <ClientSettings>
                <Scrolling AllowScroll="True" SaveScrollPosition="True" ScrollHeight="0px" UseStaticHeaders="false" />
            </ClientSettings>
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
        </DocSuite:BindGrid>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlsearch" Width="100%" Visible="false">
        <asp:Button ID="btnContenitore" runat="server" Text="Cambia Contenitore" Width="150px" />
        <asp:Button ID="btnClassificazione" runat="server" Text="Cambia Classificazione" Width="150px" />
        <asp:Button ID="btnNuovaRicerca" runat="server" Text="Nuova Ricerca" Visible="False" Width="150px" />
        <asp:Button ID="btnAutorizza" runat="server" Text="Autorizza" Width="150px" />
    </asp:Panel>
</asp:Content>



