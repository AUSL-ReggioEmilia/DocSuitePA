<%@ Page Title="Altre attività sui documenti" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="UserWorkflow.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UserWorkflow" %>
<asp:Content ContentPlaceHolderID="cphHeader" runat="server">

    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            function ShowLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= wfGrid.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);
            }
        </script>

    </telerik:RadScriptBlock>
    <asp:Panel ID="searchTable" runat="server">
        <table class="datatable" border="1">
            <tr class="Chiaro">
                <td class="col-dsw-5">
                    <table cellspacing="0" class="col-dsw-10" style="border: 0;">
                        <tr runat="server" id="trType" visible="false">
                            <td class="label">Tipo attività:
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlType" AppendDataBoundItems="True" runat="server">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr runat="server" id="trEnvironment" visible="false">
                            <td class="label">Ambito:
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlEnvironment" AppendDataBoundItems="True" runat="server">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Attività:
                            </td>
                            <td>
                                <asp:TextBox ID="txtWfNameActivity" MaxLength="255" Width="300px" runat="server" />
                            </td>
                        </tr>
                        <tr style="display: none">
                            <td class="label">Flusso di lavoro:
                            </td>
                            <td>
                                <asp:TextBox ID="txtWfInstanceName" MaxLength="255" runat="server" Width="300px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Oggetto:
                            </td>
                            <td>
                                <asp:TextBox ID="txtWfSubject" MaxLength="255" Width="300px" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Filtro date:
                            </td>
                            <td>
                                <telerik:RadDatePicker DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" ID="rdpDateFilterFrom" runat="server" />
                                <telerik:RadDatePicker DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" ID="rdpDateFilterTo" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Stato:
                            </td>
                            <td>
                                <table id="tblFilterState" runat="server" cellspacing="0" cellpadding="2" border="0" width="100%">
                                    <tr>
                                        <td style="vertical-align: middle; font-size: 8pt">
                                            <asp:RadioButtonList ID="rdbWfStatus" runat="server" RepeatDirection="Horizontal">
                                                <asp:ListItem Text="Tutti" Value="0" />
                                                <asp:ListItem Text="Da lavorare" Value="1" />
                                                <asp:ListItem Text="In corso" Value="2" />
                                                <asp:ListItem Text="Completata" Value="8" />
                                            </asp:RadioButtonList>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Modalità visualizzazione:
                            </td>
                            <td>
                                <table id="Table1" runat="server" cellspacing="0" cellpadding="2" border="0" width="100%">
                                    <tr>
                                        <td style="vertical-align: middle; font-size: 8pt">
                                            <asp:RadioButtonList ID="rdbViewer" runat="server" RepeatDirection="Horizontal">
                                                <asp:ListItem Text="Tabella" Value="0" Selected="True" />
                                                <asp:ListItem Text="Raggruppata" Value="1" />
                                            </asp:RadioButtonList>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div>
                                    <telerik:RadButton ID="btnSearch" runat="server" Width="200px" Text="Aggiorna visualizzazione" />
                                    <telerik:RadButton ID="btnClearFilters" runat="server" Text="Azzera filtri" />
                                </div>
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
        <telerik:RadGrid AllowFilteringByColumn="False" CssClass="deskGrid" AllowMultiRowSelection="false" AutoGenerateColumns="False" GridLines="Both" ShowGroupPanel="True" ID="wfGrid" PageSize="20" runat="server" Height="100%">
            <MasterTableView CommandItemDisplay="None" CurrentResetPageIndexAction="SetPageIndexToFirst" NoMasterRecordsText="Nessun registrazione" TableLayout="Auto"
                DataKeyNames="WorkflowActivityId,WorkflowInstanceId">
                <SortExpressions>
                    <telerik:GridSortExpression FieldName="WorkflowActivityPublicationDate" SortOrder="Descending" />
                </SortExpressions>
                <ItemStyle CssClass="Scuro" />
                <AlternatingItemStyle CssClass="Chiaro" />
                <Columns>
                    <telerik:GridBoundColumn DataField="WorkflowActivityId" HeaderText="WorkflowActivityId" Visible="false" />
                    <telerik:GridBoundColumn DataField="WorkflowInstanceId" HeaderText="WorkflowInstanceId" Visible="false"/>
                    <telerik:GridTemplateColumn DataField="WorkflowActivityStart" HeaderText="I" UniqueName="WorkflowActivityStart" SortExpression="Name" AllowSorting="False" ItemStyle-Width="" Groupable="false">
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <ItemStyle HorizontalAlign="Left" Width="7px" />
                        <ItemTemplate>
                            <asp:Image runat="server" ID="imgWorkflowActivityStart" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn DataField="WorkflowActivityName" HeaderText="Attività" UniqueName="WorkflowActivityName" SortExpression="Name" AllowSorting="True" ItemStyle-Width="" GroupByExpression="WorkflowActivityName Group By WorkflowActivityName">
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <ItemStyle HorizontalAlign="Left" Width="10%" />
                        <ItemTemplate>
                            <asp:HyperLink ID="lnkWorkflowActivityName" onclick="return ShowLoadingPanel();" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Richiedente/Mittente" FooterStyle-HorizontalAlign="Left" UniqueName="WorkflowProposerRoleName" AllowSorting="False" Groupable="false">
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="15%" />
                        <ItemTemplate>
                            <asp:Label ID="lblWorkflowProposerRoleName" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridDateTimeColumn DataField="WorkflowActivityPublicationDate" DataType="System.DateTime" SortExpression="WorkflowActivityPublicationDate" HeaderText="Data richiesta" DataFormatString="{0:dd/MM/yyyy}" UniqueName="WorkflowActivityPublicationDate">
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="5%" />
                    </telerik:GridDateTimeColumn>
                    <telerik:GridTemplateColumn DataField="WorkflowSubject" HeaderText="Note" FooterStyle-HorizontalAlign="Left" UniqueName="WorkflowSubject" SortExpression="Subject" AllowSorting="True" GroupByExpression="WorkflowSubject Group By WorkflowSubject">
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="30%" />
                        <ItemTemplate>
                            <asp:Label ID="lblWorkflowSubject" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Destinatario" FooterStyle-HorizontalAlign="Left" UniqueName="WorkflowReceiverRoleName" AllowSorting="False" Groupable="false">
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="12%" />
                        <ItemTemplate>
                            <asp:Label ID="lblWorkflowReceiverRoleName" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="In carico a" FooterStyle-HorizontalAlign="Left" UniqueName="WorkflowIsHandler" AllowSorting="False" Groupable="false">
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="12%" />
                        <ItemTemplate>
                            <asp:Label ID="lblWorkflowIsHandler" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn DataField="WorkflowActivityStatus" HeaderText="Stato" SortExpression="Status" UniqueName="WorkflowActivityStatus" AllowSorting="True" GroupByExpression="WorkflowActivityStatus Group By WorkflowActivityStatus">
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <ItemTemplate>
                            <asp:Label ID="lblWorkflowActivityStatus" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridDateTimeColumn DataField="WorkflowActivityLastChangedDate" DataType="System.DateTime" SortExpression="LastChangedDate" HeaderText="Data ultimo aggiornamento" DataFormatString="{0:dd/MM/yyyy}" UniqueName="WorkflowActivityLastChangedDate">
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="5%" />
                    </telerik:GridDateTimeColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings>
                <Selecting AllowRowSelect="false" CellSelectionMode="None" UseClientSelectColumnOnly="false" EnableDragToSelectRows="False" />
                <Scrolling AllowScroll="true" ScrollHeight="100%" />
            </ClientSettings>
            <SortingSettings SortedAscToolTip="Ordine crescente" SortedDescToolTip="Ordine descrescente" SortToolTip="Ordina" />
            <GroupingSettings ShowUnGroupButton="false" UnGroupButtonTooltip="Rimuovi" />
        </telerik:RadGrid>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <telerik:RadPageLayout runat="server" ID="btnCreate" HtmlTag="Div" Width="100%">
        <Rows>
            <telerik:LayoutRow>
                <Content>
                    <telerik:RadButton Text="Avvia attività" runat="server" ID="cmdCreate" />
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>
