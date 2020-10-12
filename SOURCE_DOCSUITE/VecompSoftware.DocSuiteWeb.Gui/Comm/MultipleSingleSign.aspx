<%@ Page AutoEventWireup="false" CodeBehind="MultipleSingleSign.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.MultipleSingleSign" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Firma Multipla" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            function OpenSignWindow(sender, args) {
                sender.set_enabled(false);
                var hiddenPezza = $get("<%= hiddenPezza.ClientID%>");
                var parsedQueryString = sender.get_commandArgument();
                if (!parsedQueryString) {
                    hiddenPezza.value = "";

                    args.set_cancel(true);
                    return false;
                }

                var wnd = DSWOpenGenericWindow("../Comm/SingleSign.aspx?" + parsedQueryString, WindowTypeEnum.NORMAL);
                wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
                wnd.set_destroyOnClose(true);
                wnd.add_close(CloseSignWindow);
                wnd.center();

                hiddenPezza.value = parsedQueryString;

                args.set_cancel(true);
                return false;
            }

            function CloseSignWindow(sender, args) {
                if (args.get_argument() !== null) {
                    $find("<%= AjaxManager.ClientID %>").ajaxRequest(args.get_argument());
                }
            }
        </script>
    </telerik:RadScriptBlock>
    <asp:HiddenField runat="server" ID="hiddenPezza"/>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadGrid runat="server" ID="DocumentListGrid" Width="100%" EnableViewState="true" AllowMultiRowSelection="True">
        <MasterTableView AutoGenerateColumns="False" DataKeyNames="Serialized">
            
            <GroupByExpressions>
                <telerik:GridGroupByExpression>
                    <GroupByFields>
                        <telerik:GridGroupByField FieldName="GroupCode" HeaderText="Codice"></telerik:GridGroupByField>
                    </GroupByFields>
                    <SelectFields>
                        <telerik:GridGroupByField FieldName="Description" FieldAlias="Oggetto" Aggregate="First" />
                        <telerik:GridGroupByField FieldName="IdOwner" FieldAlias="Documenti" Aggregate="Count" />
                    </SelectFields>
                </telerik:GridGroupByExpression>
            </GroupByExpressions>

            <Columns>
                <telerik:GridTemplateColumn HeaderStyle-Width="16px" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/documentPreview.png" HeaderText="Tipo Documento">
                    <ItemTemplate>
                        <asp:Image ID="documentType" runat="server"/>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn UniqueName="Type" HeaderText="Tipo" HeaderStyle-Width="100px" >
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblDocType" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn UniqueName="Name" HeaderText="Documento" HeaderStyle-Width="200px">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblFileName" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                
                <telerik:GridTemplateColumn HeaderText="Firma" HeaderStyle-Width="160px">
                    <ItemTemplate>
                        <telerik:RadButton AutoPostBack="false" OnClientClicking="OpenSignWindow" ButtonType="LinkButton" ID="btnSign" runat="server" Text="Firma">
                            <Icon PrimaryIconHeight="16px" PrimaryIconWidth="16px" />
                        </telerik:RadButton>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        
        <SortingSettings SortedAscToolTip="Ordine crescente" SortedDescToolTip="Ordine descrescente" SortToolTip="Ordina" />
        <GroupingSettings ShowUnGroupButton="true" UnGroupButtonTooltip="Rimuovi" />
    </telerik:RadGrid>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:button ID="btnConfirm" runat="server" Text="Conferma" />
    <asp:Button runat="server" ID="btnUndo" Text="Annulla"/>
</asp:Content>
