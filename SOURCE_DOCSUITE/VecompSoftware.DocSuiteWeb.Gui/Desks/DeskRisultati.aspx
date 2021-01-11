<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DeskRisultati.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DeskRisultati" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Tavoli - Risultati" %>

<%@ Import Namespace="VecompSoftware.DocSuiteWeb.Gui" %>


<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <style type="text/css">
            /*Colore Scuro*/
            .desk tr.Scuro {
                background-color: #dcdcdc;
            }

            /*Colore Chiaro*/
            .desk tr.Chiaro {
                background-color: #F5F5F5;
                vertical-align: middle;
            }
        </style>
        <script type="text/javascript">
            function ShowLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= dgDesk.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);
            }
        </script>
    </telerik:RadScriptBlock>
    <asp:Panel ID="searchTable" runat="server">
        <table class="datatable" border="1">
            <tr class="Chiaro">
                <td class="col-dsw-5">
                    <table cellspacing="0" cellpadding="2" class="col-dsw-10" style="border: 0;">
                        <tr>
                            <td class="label">Nome:
                            </td>
                            <td>
                                <asp:TextBox ID="txtDeskName" MaxLength="255" Width="300px" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Contenitore:
                            </td>
                            <td>
                                <telerik:RadDropDownList ID="rcbContainer" runat="server" Width="300px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Oggetto:
                            </td>
                            <td>
                                <asp:TextBox ID="txtDescription" MaxLength="255" runat="server" Width="300px" />
                            </td>
                        </tr>

                        <tr>
                            <td colspan="2">
                                <hr />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div>
                                    <asp:Button ID="btnSearch" runat="server" Width="200px" Text="Aggiorna visualizzazione" />
                                    <asp:Button ID="btnClearFilters" runat="server" Text="Azzera filtri" />
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
                <td class="col-dsw-5">
                    <table id="tblFilterState" runat="server" cellspacing="0" cellpadding="2" border="0" width="100%">
                        <tr>
                            <td class="label">Visualizza:</td>
                            <td style="vertical-align: middle; font-size: 8pt">
                                <asp:RadioButtonList ID="rdbShowRecorded" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Text="Tutti i tavoli" Value="0" />
                                    <asp:ListItem Text="Aperti" Value="1" />
                                    <asp:ListItem Text="Chiusi" Value="2" />
                                    <asp:ListItem Text="In approvazione" Value="4" />
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Tavoli non scaduti:</td>
                            <td>
                                <asp:CheckBox ID="chbDeskNotExpired" runat="server" />
                            </td>
                        </tr>
                    </table>

                </td>

            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <div class="radGridWrapper" runat="server">
        <DocSuite.WebComponent:BindGrid AllowFilteringByColumn="False" CssClass="deskGrid" AllowMultiRowSelection="false" AutoGenerateColumns="False" GridLines="Both" ShowGroupPanel="True" ID="dgDesk" PageSize="20" runat="server">
            <MasterTableView CommandItemDisplay="None" CurrentResetPageIndexAction="SetPageIndexToFirst" NoMasterRecordsText="Nessun registrazione" TableLayout="Auto">
                <ItemStyle CssClass="Scuro" />
                <AlternatingItemStyle CssClass="Chiaro" />
                <Columns>

                    <telerik:GridTemplateColumn UniqueName="DeskState" HeaderText="Stato" DataField="Status" SortExpression="Status" AllowSorting="True" GroupByExpression="DeskState Group By DeskState">
                        <HeaderStyle HorizontalAlign="Center" Wrap="false" Width="30" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:Image ID="imgDeskStatus" ImageUrl="~/App_Themes/DocSuite2008/images/desk/desk.png" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>

                    <telerik:GridTemplateColumn DataField="DeskName" HeaderText="Nome" UniqueName="Name" SortExpression="Name" AllowSorting="True" ShowSortIcon="True" GroupByExpression="DeskName Group By DeskName">
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="150" />
                        <ItemStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:HyperLink ID="linkSummary" onclick="return ShowLoadingPanel();" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>

                    <telerik:GridDateTimeColumn DataField="DeskSubject" HeaderText="Oggetto" SortExpression="Description" AllowSorting="True" ShowSortIcon="True" GroupByExpression="DeskSubject Group By DeskSubject">
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="150" />
                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                    </telerik:GridDateTimeColumn>

                    <telerik:GridDateTimeColumn DataField="DeskExpirationDate" DataType="System.DateTime" HeaderText="Data scadenza" UniqueName="DataScadenza" DataFormatString="{0:dd/MM/yyyy}" GroupByExpression="DeskExpirationDate Group By DeskExpirationDate"
                        SortExpression="Expirationdate" AllowSorting="True" ShowSortIcon="True">
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="150" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                    </telerik:GridDateTimeColumn>

                    <DocSuite:SuggestFilteringColumn UniqueName="Contenitore" HeaderText="Contenitore" DataField="ContainerName" SortExpression="ContainerName" AllowSorting="True" GroupByExpression="ContainerName Group By ContainerName">
                        <HeaderStyle HorizontalAlign="Center" Wrap="false" Width="100" />
                        <ItemStyle HorizontalAlign="Left" />
                    </DocSuite:SuggestFilteringColumn>

                </Columns>
            </MasterTableView>
            <ClientSettings>
                <Selecting AllowRowSelect="false" CellSelectionMode="None" UseClientSelectColumnOnly="false" EnableDragToSelectRows="False" />
            </ClientSettings>
            <SortingSettings SortedAscToolTip="Ordine crescente" SortedDescToolTip="Ordine descrescente" SortToolTip="Ordina" />
            <GroupingSettings ShowUnGroupButton="true" UnGroupButtonTooltip="Rimuovi" />

        </DocSuite.WebComponent:BindGrid>
    </div>
</asp:Content>
