<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UserFascicle.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UserFascicle" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscFascGrid.ascx" TagName="uscFascGrid" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var userFascicle;
            require(["User/UserFascicle"], function (UserFascicle) {
                $(function () {
                    userFascicle = new UserFascicle(tenantModelConfiguration.serviceConfiguration);
                    userFascicle.gridId = "<%= uscFascicleGrid.Grid.ClientID %>";
                    userFascicle.selectableFasciclesThreshold = "<%= SelectableFaciclesThreshold%>";
                    userFascicle.btnDocumentsId = "<%= btnDocuments.ClientID %>";
                    userFascicle.btnSelectAllId = "<%= btnSelectAll.ClientID %>";
                    userFascicle.btnDeselectAllId = "<%= btnDeselectAll.ClientID %>";
                    userFascicle.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    userFascicle.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    userFascicle.backBtnId = "<%= backBtn.ClientID %>";
                    userFascicle.initialize();
                });
            });

            $(document).keypress(function (e) {
                if (e.which == 13) {
                    document.getElementById("<%= btnSearch.ClientID %>").click();
                    e.preventDefault();
                }
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
                                <telerik:RadDatePicker ID="rdpDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server"  />
                                <telerik:RadDatePicker ID="rdpDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important; margin-left: 5px;" Width="200" DateInput-Label="A" runat="server" />
                            </Content>
                        </telerik:CompositeLayoutColumn>
                    </Columns>
                </telerik:LayoutRow>
                <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" WrapperHtmlTag="None">
                    <Content>
                        <telerik:RadButton ID="btnSearch" runat="server" Text="Aggiorna" OnClick="btnSearch_Click"  />
                    </Content>
                </telerik:LayoutRow>
            </Rows>
        </telerik:RadPageLayout>
    </asp:Panel>

</asp:Content>

<asp:Content runat="server"  ContentPlaceHolderID="cphContent" style="overflow: hidden; width: 100%; height: 100%;">
        <telerik:RadPageLayout runat="server" HtmlTag="Div" ID="pageContent" Height="100%">
        <Rows>
            <telerik:LayoutRow runat="server" HtmlTag="Div" Height="100%">
                <Content>
                   <uc1:uscFascGrid runat="server" ID="uscFascicleGrid" />
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel ID="pnlButtons" runat="server" Width="100%">
        <%--Note: VisualizaDocumenti currently hidden because of an issue related to the functionality.--%>
        <telerik:RadButton ID="btnDocuments" runat="server" Width="130px" Text="Visualizza documenti" Visible="false"/>
        <telerik:RadButton ID="btnSelectAll" runat="server" Width="120px" Text="Seleziona tutti"></telerik:RadButton>
        <telerik:RadButton ID="btnDeselectAll" runat="server" Width="120px" Text="Annulla selezione"></telerik:RadButton>
        <telerik:RadButton runat="server" AutoPostBack="false" Visible="false" ID="backBtn" Text="Torna alla ricerca" />
    </asp:Panel>
</asp:Content>

