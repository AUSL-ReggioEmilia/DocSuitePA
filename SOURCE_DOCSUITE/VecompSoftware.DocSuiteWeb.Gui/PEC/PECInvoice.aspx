<%@ Page Language="vb" Title="Cruscotto fatture" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="PECInvoice.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.PECInvoice" %>
<%@ Register Src="~/UserControl/uscTenantsSelector.ascx" TagName="uscTenantsSelector" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var pecInvoice;
            require(["PEC/PECInvoice"], function (PECInvoice) {
                $(function () {
                    pecInvoice = new PECInvoice(tenantModelConfiguration.serviceConfiguration);
                    pecInvoice.pecInvoiveGridId = "<%= pecInvoiveGrid.ClientID%>";
                    pecInvoice.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    pecInvoice.btnSearchId = "<%= btnSearch.ClientID%>";
                    pecInvoice.btnCleanId = "<%= btnClean.ClientID %>";
                    pecInvoice.btnTenantsId = "<%= btnTenants.ClientID %>";
                    pecInvoice.dpStartDateFromId = "<%= dtpDateFrom.ClientID%>";
                    pecInvoice.dpEndDateFromId = "<%= dtpDateTo.ClientID%>";
                    pecInvoice.cmbPecMailBoxId = "<%= cmbPecMailBox.ClientID %>";
                    pecInvoice.txtMittenteId = "<%= txtMittente.ClientID %>";
                    pecInvoice.txtDestinararioId = "<%= txtDestinarario.ClientID %>";
                    pecInvoice.cmbStatoId = "<%= cmbStato.ClientID %>";
                    pecInvoice.cmbTipologiaFatturaId = "<%= cmbTipologiaFattura.ClientID %>";
                    pecInvoice.rwTenantSelectorId = "<%= uscTenantsSelector.RadWindowTenantSelector.ClientID %>";
                    pecInvoice.cmbSelectPecMailBoxId = "<%= uscTenantsSelector.PecMailBoxCombo.ClientID %>";
                    pecInvoice.cmbWorkflowRepositoriesId = "<%= uscTenantsSelector.WorkflowCombo.ClientID %>";
                    pecInvoice.btnContainerSelectorOkId = "<%= uscTenantsSelector.btnAvviaFlusso.ClientID %>";
                    pecInvoice.cmbTenantsComboId = "<%= uscTenantsSelector.TenantsCombo.ClientID %>";
                    pecInvoice.maxNumberElements = "<%= ProtocolEnv.MaxNumberDropdownElements %>";


                    var directionType = '<%= InvoiceDirection %>';
                    var direction = "";
                    if (directionType !== "") {
                        direction = directionType.trim();
                    }
                    pecInvoice.direction = direction;
                    pecInvoice.initialize();
                });
            });

            function OnGridCommand(sender, args) {
                if (args.get_commandName() === "Page") {
                    args.set_cancel(true);
                    pecInvoice.onPageChanged();
                }
            }

            function OnGridDataBound(sender, args) {
                pecInvoice.onGridDataBound();
            }

            function OnGridRowSelected(sender, args) {          
                pecInvoice.onGridRowSelected();
            }

        </script>
    </telerik:RadScriptBlock>

    <table class="datatable">
        <tr>
            <td class="label labelPanel" style="width: 20%;">Periodo:
            </td>
            <td style="width: 80%;">
                <telerik:RadDatePicker DateInput-LabelWidth="30%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="300" DateInput-Label="Da data" ID="dtpDateFrom" runat="server" />
                <telerik:RadDatePicker DateInput-LabelWidth="30%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="300" DateInput-Label="A data" ID="dtpDateTo" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel" style="width: 20%;">Casella PEC:
            </td>
            <td style="width: 80%;">
                <telerik:RadComboBox runat="server" ID="cmbPecMailBox" DataTextField="Text" DataValueField="Value" Width="50%" AutoPostBack="false" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel" style="width: 20%;">Mittente:
            </td>
            <td style="width: 30%;">
                <telerik:RadTextBox ID="txtMittente" runat="server" Width="50%" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel" style="width: 20%;">Destinarario:
            </td>
            <td style="width: 30%;">
                <telerik:RadTextBox ID="txtDestinarario" runat="server" Width="50%" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel" style="width: 20%">Stato:
            </td>
            <td style="width: 30%">
                <telerik:RadComboBox runat="server" ID="cmbStato" DataTextField="Text" DataValueField="Value" Width="50%" AutoPostBack="false" EmptyMessage="" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel" style="width: 20%">Tipologia fattura:
            </td>
            <td style="width: 30%">
                <telerik:RadComboBox runat="server" ID="cmbTipologiaFattura" DataTextField="Text" DataValueField="Value" Width="50%" AutoPostBack="false" EmptyMessage="" />
            </td>
        </tr>
    </table>
    <div style="margin: 1px 1px 10px 1px;">
        <div>
            <telerik:RadButton ID="btnSearch" Text="Aggiorna visualizzazione" Width="150px" runat="server" TabIndex="1" AutoPostBack="False" />
            <telerik:RadButton ID="btnClean" Text="Azzera filtri" Width="150px" runat="server" TabIndex="1" AutoPostBack="False" />
        </div>
    </div>
</asp:Content>

<asp:Content ID="content" runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel runat="server" CssClass="radGridWrapper" ID="pageContent">
        <telerik:RadGrid runat="server" CssClass="pecInvoiveGrid" ID="pecInvoiveGrid"
            Skin="Office2010Blue" GridLines="None" Height="100%" AllowPaging="False" AllowMultiRowSelection="false"
            AllowFilteringByColumn="False">
            <ClientSettings EnablePostBackOnRowClick="false">
                <Selecting AllowRowSelect="True"></Selecting>
                <Scrolling AllowScroll="true" UseStaticHeaders="true" ScrollHeight="100%" />
                <Resizing AllowColumnResize="true" AllowRowResize="true" />
                <ClientEvents OnCommand="OnGridCommand" OnDataBound="OnGridDataBound" OnRowSelected="OnGridRowSelected"/>
            </ClientSettings>

            <MasterTableView CommandItemDisplay="None" AutoGenerateColumns="False" TableLayout="fixed"
                NoMasterRecordsText="Nessun elemento trovato nel perido indicato." PagerStyle-PagerTextFormat="{4} Visualizzati {3} su {5}" AllowMultiColumnSorting="True">
                <Columns>
                    <telerik:GridBoundColumn DataField="Icona" UniqueName="Icona" Visible="true" HeaderText="" HeaderStyle-Width="30px" ItemStyle-Width="30px" />
                    <telerik:GridBoundColumn DataField="InvoiceTypeDescription" HeaderText="Tipologia fattura" HeaderStyle-Width="10%" ItemStyle-Width="10%" />
                    <telerik:GridBoundColumn DataField="MailSubject" HeaderText="Oggetto" HeaderTooltip="Oggetto" HeaderStyle-Width="35%" ItemStyle-Width="20%" />
                    <telerik:GridBoundColumn DataField="MailSenders" HeaderText="Mittente" HeaderTooltip="Mittente" HeaderStyle-Width="15%" ItemStyle-Width="10%" />
                    <telerik:GridBoundColumn DataField="MailRecipients" HeaderText="Destinatario" HeaderTooltip="Destinatario" HeaderStyle-Width="15%" ItemStyle-Width="10%" />
                    <telerik:GridBoundColumn DataField="MailDate" HeaderText="Data di invio" HeaderTooltip="Data di invio" HeaderStyle-Width="5%" ItemStyle-Width="10%" />
                    <telerik:GridBoundColumn DataField="MailDate" HeaderText="Data di ricezione" HeaderTooltip="Data di ricezione" HeaderStyle-Width="5%" ItemStyle-Width="10%" />
                    <telerik:GridBoundColumn DataField="InvoiceStatusDescription" HeaderText="Stato fattura" HeaderTooltip="Stato fattura" HeaderStyle-Width="15%" ItemStyle-Width="10%" />
                </Columns>
            </MasterTableView>
            <GroupingSettings ShowUnGroupButton="true"></GroupingSettings>
        </telerik:RadGrid>
    </asp:Panel>
     
    <usc:uscTenantsSelector runat="server" ID="uscTenantsSelector" />
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <div style="margin: 1px 1px 1px 1px;">
        <div>
            <telerik:RadButton ID="btnTenants" Text="Avvia flusso di lavoro" Width="150px" runat="server" TabIndex="1" AutoPostBack="False"/>
        </div>
    </div>
</asp:Content>
