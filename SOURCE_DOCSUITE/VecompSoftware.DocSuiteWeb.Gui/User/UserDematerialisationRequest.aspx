<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.UserDematerialisationRequest" CodeBehind="UserDematerialisationRequest.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Attestazioni di conformità di Dematerializzazione" %>

<%@ Import Namespace="VecompSoftware.DocSuiteWeb.Gui" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
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
                var currentUpdatedControl = "<%= dgDematerialisationStatement.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);
            }

            function ExecuteAjaxRequest(operationName) {
                var manager = $find("<%= AjaxManager.ClientID %>");
                manager.ajaxRequest(operationName);
            }

            function <%=ClientID%>_OpenWindowRequestStatement(ids) {
                var manager = $find("<%=RadWindowManagerProt.ClientID %>");
                var wnd = manager.open("../Workflows/RequestStatement.aspx?Type=Prot&FromPageMultiple=True&DocumentUnitIds=" + ids, "windowRequestStatement");
                wnd.setSize(720, 560);
                wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Maximize + Telerik.Web.UI.WindowBehaviors.Resize + Telerik.Web.UI.WindowBehaviors.Close);
                wnd.set_visibleStatusbar(false);
                wnd.set_modal(true);
                wnd.center();
                return false;
            }

            function CloseRequestStatement(sender, args) {
                sender.remove_close(CloseRequestStatement);
                if (args.get_argument() !== null) {
                }
                ExecuteAjaxRequest("ReloadPage");
            }
        </script>


    </telerik:RadScriptBlock>

    <telerik:RadPageLayout runat="server" HtmlTag="None" CssClass="col-dsw-10">
        <Rows>
            <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" ID="rowDate" runat="server">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="Div" CssClass="control-label" Span="3">
                        <asp:Label ID="lblRegistrationDate" runat="server" Text="Data registrazione:" />
                    </telerik:LayoutColumn>
                    <telerik:CompositeLayoutColumn HtmlTag="Div" CssClass="form-control dsw-vertical-middle" Span="9">
                        <Content>
                            <telerik:RadDatePicker ID="rdpDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
                            <asp:CompareValidator ControlToValidate="rdpDateFrom" Display="Dynamic" ErrorMessage="Errore formato" ID="cfvDateFrom" Operator="DataTypeCheck" runat="server" Type="Date" />
                            <asp:RequiredFieldValidator ControlToValidate="rdpDateFrom" Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="rfvDateFrom" runat="server" />
                            <telerik:RadDatePicker ID="rdpDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important; margin-left: 5px;" Width="200" DateInput-Label="A" runat="server" />
                            &nbsp;
                            <asp:CompareValidator ControlToValidate="rdpDateTo" Display="Dynamic" ErrorMessage="Errore formato" ID="cfvDateTo" Operator="DataTypeCheck" runat="server" Type="Date" />
                            <asp:RequiredFieldValidator ControlToValidate="rdpDateTo" Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="rfvDateTo" runat="server" />
                        </Content>
                    </telerik:CompositeLayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" WrapperHtmlTag="None">
                <Content>
                    <asp:Button ID="btnUpdate" runat="server" Text="Aggiorna" />
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadWindowManager BackColor="Gray" Behaviors="Close" DestroyOnClose="true" EnableViewState="False" ID="RadWindowManagerProt" runat="server">
        <Windows>
            <telerik:RadWindow Height="300" ID="windowRequestStatement" OnClientClose="CloseRequestStatement" runat="server" Title="Richiesta di Attestazione di conformità" Width="500" />
        </Windows>
    </telerik:RadWindowManager>

    <div class="radGridWrapper">
        <DocSuite.WebComponent:BindGrid AllowMultiRowSelection="true" AutoGenerateColumns="False" GridLines="Both" runat="server" ShowGroupPanel="True" ID="dgDematerialisationStatement">
            <MasterTableView AllowFilteringByColumn="False" GridLines="Both" NoMasterRecordsText="Nessun registrazione" TableLayout="Fixed" FilterItemStyle-Width="100px">
                <ItemStyle CssClass="Scuro" />
                <AlternatingItemStyle CssClass="Chiaro" />
                <Columns>
                    <telerik:GridTemplateColumn UniqueName="UDUniqueId" CurrentFilterFunction="EqualTo" SortExpression="UDUniqueId" DataField="UDId" Visible="false">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblUDUniqueId" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="False" UniqueName="colClientSelect">
                        <HeaderStyle HorizontalAlign="Center" CssClass="headerImage" Width="30px" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:CheckBox AutoPostBack="False" ID="cbSelect" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="UDName" CurrentFilterFunction="EqualTo" SortExpression="UDName" DataField="UDName" HeaderText="Documento" AllowFiltering="False" Groupable="false" DefaultInsertValue="Protocollo">
                        <HeaderStyle HorizontalAlign="Left" Width="140px" />
                        <ItemStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblViewUDName" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="Title" HeaderText="Anno e Numero" DataField="Number" SortExpression="Year,Number" Groupable="false" ShowFilterIcon="false">
                        <HeaderStyle HorizontalAlign="Left" Wrap="false" Width="125px" />
                        <ItemStyle HorizontalAlign="Left" Width="125px" />
                        <FilterTemplate>
                            <telerik:RadMaskedTextBox RenderMode="Lightweight" ID="UDTitleFilter" runat="server" Width="100px" Mask="####/#######" />
                        </FilterTemplate>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="lbtViewUD" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridDateTimeColumn UniqueName="RegistrationDate" HeaderText="Data registrazione" DataField="RegistrationDate" DataFormatString="{0:dd/MM/yyyy}" AllowFiltering="False" SortExpression="RegistrationDate" Groupable="false">
                        <HeaderStyle HorizontalAlign="Center" Wrap="false" Width="125px" />
                        <ItemStyle HorizontalAlign="Center" />
                    </telerik:GridDateTimeColumn>
                    <telerik:GridBoundColumn UniqueName="Subject" DataField="Subject" HeaderText="Oggetto" Groupable="false" AllowFiltering="true" CurrentFilterFunction="EqualTo" ShowFilterIcon="false" FilterControlWidth="600px">
                        <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                        <ItemStyle HorizontalAlign="Left" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="ContainerName" CurrentFilterFunction="Contains" DataField="ContainerName" HeaderText="Contenitore" SortExpression="ContainerName" ShowFilterIcon="false" FilterControlWidth="270px">
                        <HeaderStyle HorizontalAlign="Left" Wrap="false" Width="300px" />
                        <ItemStyle HorizontalAlign="Left" />
                    </telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings>
                <Selecting AllowRowSelect="false" CellSelectionMode="None" UseClientSelectColumnOnly="false" EnableDragToSelectRows="False" />
            </ClientSettings>
            <SortingSettings SortedAscToolTip="Ordine Crescente" SortedDescToolTip="Ordine Descrescente" SortToolTip="Ordina" />
            <GroupingSettings ShowUnGroupButton="true" UnGroupButtonTooltip="Rimuovi" />
            <ClientSettings>
                <Selecting AllowRowSelect="True" UseClientSelectColumnOnly="True" />
            </ClientSettings>
        </DocSuite.WebComponent:BindGrid>
    </div>
</asp:Content>

<asp:Content ID="cn2" runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel runat="server" ID="pnlButtons">
        <asp:Button ID="btnStatement" runat="server" Width="150" Text="Richiedi Attestazione" CausesValidation="false" Visible="true" />
    </asp:Panel>
</asp:Content>


