<%@ Page Language="vb" Title="Protocollo" AutoEventWireup="false" CodeBehind="ProtMDRisultati.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtMDRisultati" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="../UserControl/uscODATAProtGrid.ascx" TagName="uscODATAProtGrid" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/uscProtGridBar.ascx" TagName="uscProtGridBar" TagPrefix="uc1" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">
            function pageLoad(sender, eventArgs) {
                if (!eventArgs.get_isPartialLoad()) {
                    ExecuteAjaxRequest();
                }
            }

            function ExecuteAjaxRequest() {
                var manager = $find("<%= AjaxManager.ClientID %>");
                manager.ajaxRequestWithTarget('<%= btnUpdate.UniqueID %>', '');
            }
        </script>
    </telerik:RadScriptBlock>
        <div id="pnlHeader" runat="server">
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
            </div>
</asp:Content>

<asp:Content ID="ProtGridContent" ContentPlaceHolderID="cphContent" runat="server">
    <div style="height:100%">
        <uc1:uscODATAProtGrid runat="server" id="uscProtocolGrid" ColumnTenantNameVisible="true"/>
    </div>
</asp:Content>

