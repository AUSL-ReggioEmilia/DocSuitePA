<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="ReslActivityResults.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslActivityResults" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">

            function OpenWindowEdit(idActivity) {
                var manager = $find("<%= RadWindowManagerResolution.ClientID %>");
                var wnd = manager.open('../Resl/ReslActivityEdit.aspx?Type=Resl&IdActivity=' + idActivity, 'windowEdit');
                wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
                wnd.set_visibleStatusbar(false);
                wnd.add_close(CloseEdit);
                wnd.set_modal(true);
                wnd.set_width(400);
                wnd.set_height(300);
                wnd.center();
                return false;
            }

            function CloseEdit(sender, args) {
                if (args.get_argument()) {
                    var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                    ajaxManager.ajaxRequest(args.get_argument());
                }
            }

        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager BackColor="Gray" Behaviors="Close" DestroyOnClose="true" EnableViewState="False" ID="RadWindowManagerResolution" runat="server">
        <Windows>
            <telerik:RadWindow Height="200" ID="windowEdit" OnClientClose="CloseEdit" runat="server" Width="400" Title="Modifica data attività" />
        </Windows>
    </telerik:RadWindowManager>

    <telerik:RadPageLayout runat="server" HtmlTag="None" CssClass="col-dsw-10">
        <Rows>
            <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" ID="rowDate" runat="server">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="Div" CssClass="control-label" Span="3">
                        <asp:Label ID="lblRegistrationDate" runat="server" Text="Data Attività:" />
                    </telerik:LayoutColumn>
                    <telerik:CompositeLayoutColumn HtmlTag="Div" CssClass="form-control dsw-vertical-middle" Span="9">
                        <Content>
                            <telerik:RadDatePicker ID="rdpDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
                            <asp:CompareValidator ControlToValidate="rdpDateFrom" Display="Dynamic" ErrorMessage="Errore formato" ID="cfvDateFrom" Operator="DataTypeCheck" runat="server" Type="Date" />
                            <asp:RequiredFieldValidator ControlToValidate="rdpDateFrom" Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="rfvDateFrom" runat="server" />

                            <telerik:RadDatePicker ID="rdpDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important; margin-left: 5px;" Width="200" DateInput-Label="A" runat="server" />
                            <asp:CompareValidator ControlToValidate="rdpDateTo" Display="Dynamic" ErrorMessage="Errore formato" ID="cfvDateTo" Operator="DataTypeCheck" runat="server" Type="Date" />
                            <asp:RequiredFieldValidator ControlToValidate="rdpDateTo" Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="rfvDateTo" runat="server" />
                        </Content>
                    </telerik:CompositeLayoutColumn>
                </Columns>
            </telerik:LayoutRow>

            <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" WrapperHtmlTag="None">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="Div" CssClass="control-label" Span="3">
                        <asp:Label ID="lblActivityDate" runat="server" Text="Tipo Attività:" />
                    </telerik:LayoutColumn>
                    <telerik:CompositeLayoutColumn HtmlTag="Div" CssClass="form-control dsw-vertical-middle" Span="9">
                        <Content>
                            <asp:RadioButtonList BorderWidth="0px" ID="rblActivity" CssClass="autoWidth" RepeatDirection="Horizontal" runat="server" Width="150px" />
                        </Content>
                    </telerik:CompositeLayoutColumn>
                </Columns>
            </telerik:LayoutRow>

             <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" WrapperHtmlTag="None">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="Div" CssClass="control-label" Span="3" Height="60px">
                        <asp:Label ID="lblOC" runat="server" Text="Organo di Controllo:" />
                    </telerik:LayoutColumn>
                    <telerik:CompositeLayoutColumn HtmlTag="Div" CssClass="dsw-vertical-middle" Span="9">
                        <Content>
                            <asp:CheckBox ID="cbCS" runat="server" Text="Collegio Sindacale" /><br />
                            <asp:CheckBox ID="cbR" runat="server" Text="Regione" /><br />
                            <asp:CheckBox ID="cbCC" runat="server" Text="Corte dei Conti" /><br />
                            <asp:CheckBox ID="cbA" runat="server" Text="Altro" />
                        </Content>
                    </telerik:CompositeLayoutColumn>
                </Columns>
            </telerik:LayoutRow>


            <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" WrapperHtmlTag="None">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="Div" CssClass="control-label" Span="3">
                        <asp:Label ID="lblStatus" runat="server" Text="Stato:" />
                    </telerik:LayoutColumn>
                    <telerik:CompositeLayoutColumn HtmlTag="Div" CssClass="form-control dsw-vertical-middle" Span="9">
                        <Content>
                            <asp:RadioButtonList BorderWidth="0px" ID="rblStatus" CssClass="autoWidth" RepeatDirection="Horizontal" runat="server" Width="150px" />
                        </Content>
                    </telerik:CompositeLayoutColumn>
                </Columns>
            </telerik:LayoutRow>

           
            <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" WrapperHtmlTag="None">
                <Content>
                    <asp:Button ID="btnSearch" runat="server" Text="Cerca" />
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <div style="overflow: hidden; width: 100%; height: 100%;">
        <DocSuite:BindGrid AllowFilteringByColumn="True" AllowMultiRowSelection="true" AutoGenerateColumns="False" GridLines="Both"
            ID="dgTaskHeaders" PageSize="20" runat="server" AllowSorting="true">
            <MasterTableView CommandItemDisplay="None" CurrentResetPageIndexAction="SetPageIndexToFirst" NoMasterRecordsText="Nessun registrazione"
                TableLayout="Auto" AllowSorting="true">
                <ItemStyle CssClass="Scuro" />
                <AlternatingItemStyle CssClass="Chiaro" />
                <Columns>
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../Resl/Images/Atto.png" HeaderText="Tipologia" UniqueName="Tipologia">
                        <HeaderStyle HorizontalAlign="Center" CssClass="headerImage" />
                        <ItemStyle HorizontalAlign="Center" CssClass="cellImage" />
                        <ItemTemplate>
                            <asp:Image ID="imgTipoAtto" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>

                    <telerik:GridTemplateColumn UniqueName="ViewSummary" AllowFiltering="false" Groupable="false"
                        HeaderText="Stato" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/information.png">
                        <HeaderStyle HorizontalAlign="Center" Width="2%" />
                        <ItemStyle HorizontalAlign="Center" Width="2%" />
                        <ItemTemplate>
                            <asp:Image runat="server" ID="cmdViewSummary"
                                ImageUrl="../App_Themes/DocSuite2008/imgset16/information.png" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>

                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Data Attività Prevista" SortExpression="ActivityDate" UniqueName="ActivityDate">
                        <HeaderStyle HorizontalAlign="Center" Width="130" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkActivityDate" runat="server" NavigateUrl="#" CssClass="button-disabled" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>

                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Numero" SortExpression="R.Id" UniqueName="ResolutionId">
                        <HeaderStyle HorizontalAlign="Left" Width="150" />
                        <ItemStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:HyperLink ID="lnkResolution" runat="server" ToolTip="Visualizza sommario" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>

                    <telerik:GridBoundColumn DataField="Description"
                        HeaderText="Descrizione" UniqueName="Description" Groupable="false" CurrentFilterFunction="Contains">
                        <HeaderStyle HorizontalAlign="Left" Width="180" />
                        <ItemStyle HorizontalAlign="Left" />
                    </telerik:GridBoundColumn>

                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="R.ResolutionObject"  HeaderText="Oggetto" SortExpression="R.ResolutionObject" UniqueName="R.ResolutionObject">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" Wrap="true" />
                    </telerik:GridBoundColumn>

                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Tipo OC" UniqueName="TipOC">
                        <HeaderStyle HorizontalAlign="Left" Width="80" />
                        <ItemStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:Label ID="lblTipOC" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>

                    <telerik:GridBoundColumn AllowFiltering="false" DataField="Type" HeaderText="Tipo Attività" SortExpression="ActivityType" UniqueName="ActivityType">
                        <HeaderStyle HorizontalAlign="Left" Width="100" />
                        <ItemStyle HorizontalAlign="Left" Wrap="true" />
                    </telerik:GridBoundColumn>

                    <telerik:GridBoundColumn AllowFiltering="false" CurrentFilterFunction="Contains" DataField="Status" HeaderText="Stato" SortExpression="Status" UniqueName="Status">
                        <HeaderStyle HorizontalAlign="Left" Width="100" />
                        <ItemStyle HorizontalAlign="Left" Wrap="true" />
                    </telerik:GridBoundColumn>

                </Columns>
            </MasterTableView>
            <SortingSettings SortedAscToolTip="Ordine crescente" SortedDescToolTip="Ordine descrescente"
                SortToolTip="Ordina" />
        </DocSuite:BindGrid>
    </div>
</asp:Content>
