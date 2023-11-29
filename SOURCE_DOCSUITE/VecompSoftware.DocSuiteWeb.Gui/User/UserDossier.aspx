<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UserDossier.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UserDossier" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscDossierGrid.ascx" TagName="uscDossierGrid" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            $(document).keypress(function (e) {
                if (e.which == 13) {
                    document.getElementById("<%= btnSearch.ClientID %>").click();
                    e.preventDefault();
                 }
             });
            var userDossier;
            require(["User/UserDossier"], function (UserDossier) {
                $(function () {
                    userDossier = new UserDossier(tenantModelConfiguration.serviceConfiguration);
                    userDossier.rdpDateFromId = "<%= rdpDateFrom.ClientID%>";
                    userDossier.rdpDateToId = "<%= rdpDateTo.ClientID%>";
                    userDossier.ddlDossierTypeId = "<%= ddlDossierType.ClientID %>"
                    userDossier.btnSearchId = "<%= btnSearch.ClientID%>";
                    userDossier.uscDossierGridId = "<%= uscDossierGrid.PageContentDiv.ClientID %>";
                    userDossier.desktopDayDiff = "<%= VecompSoftware.DocSuiteWeb.Data.DocSuiteContext.Current.ProtocolEnv.DesktopDayDiff %>";
                    userDossier.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    userDossier.actionType = "<%= ActionType %>";
                    userDossier.dossierTypologyEnabled = <%= ProtocolEnv.DossierTypologyEnabled.ToString().ToLower() %>;
                    userDossier.columnDossierTypeKeyId = "<%= columnDossierTypeKey.ClientID %>";
                    userDossier.columnDossierTypeValueId = "<%= columnDossierTypeValue.ClientID %>";
                    userDossier.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>

    <asp:Panel ID="pnlSearch" runat="server">
        <telerik:RadPageLayout runat="server" HtmlTag="None" CssClass="col-dsw-10" Height="100%">
            <Rows>
                <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" ID="rowDate" runat="server">
                    <Columns>
                        <telerik:LayoutColumn HtmlTag="Div" CssClass="control-label" Span="3">
                            <asp:Label ID="lblRegistrationDate" runat="server" Text="Data registrazione:" />
                        </telerik:LayoutColumn>
                        <telerik:CompositeLayoutColumn HtmlTag="Div" CssClass="form-control dsw-vertical-middle" Span="9">
                            <Content>
                                <telerik:RadDatePicker ID="rdpDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
                                <telerik:RadDatePicker ID="rdpDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important; margin-left: 5px;" Width="200" DateInput-Label="A" runat="server" />
                            </Content>
                        </telerik:CompositeLayoutColumn>
                    </Columns>
                </telerik:LayoutRow>
                <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" ID="rowExcludeLinked" runat="server">
                    <Columns>
                        <telerik:LayoutColumn HtmlTag="Div" CssClass="control-label" Span="3" ID="columnDossierTypeKey">
                            <asp:Label ID="lblExcludeLinked" runat="server" Text="Tipo di dossier:" />
                        </telerik:LayoutColumn>
                        <telerik:CompositeLayoutColumn HtmlTag="Div" CssClass="form-control dsw-vertical-middle" Span="9" ID="columnDossierTypeValue">
                            <Content>
                                <telerik:RadDropDownList runat="server" ID="ddlDossierType" AutoPostBack="false"
                                    Width="300px" CausesValidation="false">
                                </telerik:RadDropDownList>
                            </Content>
                        </telerik:CompositeLayoutColumn>
                    </Columns>
                </telerik:LayoutRow>
                <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" WrapperHtmlTag="None">
                    <Content>
                        <telerik:RadButton ID="btnSearch" runat="server" Text="Aggiorna" AutoPostBack="false" />
                    </Content>
                </telerik:LayoutRow>
            </Rows>
        </telerik:RadPageLayout>
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server" style="overflow: hidden; width: 100%; height: 100%;">
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
    <asp:Panel ID="pnlUD" runat="server" Width="100%" Height="100%">
        <uc:uscDossierGrid runat="server" ID="uscDossierGrid" />
    </asp:Panel>
</asp:Content>
