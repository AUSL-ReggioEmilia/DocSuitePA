<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.UserFascicle" CodeBehind="UserFascicle.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="../UserControl/uscUDFascicleGrid.ascx" TagName="uscUDFascicleGrid" TagPrefix="uc1" %>


<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
        <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">
            function pageLoad(sender, eventArgs) {
                if (!eventArgs.get_isPartialLoad()) {
                    ExecuteAjaxRequest("loadDocumentUnits");
                }
            }

            function ExecuteAjaxRequest(operationName) {
                var manager = $find("<%= AjaxManager.ClientID %>");
                manager.ajaxRequest(operationName);
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
                            <telerik:RadDatePicker ID="rdpDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important; margin-left: 5px;" Width="200" DateInput-Label="A" runat="server" />&nbsp;
                            <asp:CompareValidator ControlToValidate="rdpDateTo" Display="Dynamic" ErrorMessage="Errore formato" ID="cfvDateTo" Operator="DataTypeCheck" runat="server" Type="Date" />
                            <asp:RequiredFieldValidator ControlToValidate="rdpDateTo" Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="rfvDateTo" runat="server" />
                            <asp:CheckBox runat="server" ID="chkRedThreshold" Text="Includi oltre 60 giorni" Checked="true" />
                        </Content>
                    </telerik:CompositeLayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" ID="rowExcludeLinked" runat="server">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="Div" CssClass="control-label" Span="3">
                        <asp:Label ID="lblExcludeLinked" runat="server" Text="Tipo di fascicolazione:" />
                    </telerik:LayoutColumn>
                    <telerik:CompositeLayoutColumn HtmlTag="Div" CssClass="form-control dsw-vertical-middle" Span="9">
                        <Content>
                            <asp:CheckBox runat="server" ID="chkExcludeLinked" Text="escludi anche se referenziati" />
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

<asp:Content ContentPlaceHolderID="cphContent" runat="server" style="overflow:hidden;width:100%;height:100%;">
    <asp:Panel ID="pnlUD" runat="server" Width="100%" Height="100%">
        <uc1:uscUDFascicleGrid ID="uscUDFascicleGrid" runat="server" />
    </asp:Panel>
</asp:Content>
