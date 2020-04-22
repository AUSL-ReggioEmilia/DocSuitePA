<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TbltCreaFascicolo.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltCreaFascicolo" Title="Piano di fascicolazione"
    MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="SelContatti" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var tbltCreaFascicolo;
            require(["Tblt/TbltCreaFascicolo"], function (TbltCreaFascicolo) {
                tbltCreaFascicolo = new TbltCreaFascicolo(tenantModelConfiguration.serviceConfiguration)
                tbltCreaFascicolo.ddlPeriodsId = "<%= ddlPeriods.ClientID %>";
                tbltCreaFascicolo.ajaxManagerId = "<%= MasterDocSuite.AjaxManager.ClientID %>";
                tbltCreaFascicolo.btnSaveId = "<%= btnSave.ClientID%>";
                tbltCreaFascicolo.rowPeriodId = "<%= rowPeriod.ClientID%>";
                tbltCreaFascicolo.ddlUDSId = "<%= ddlUDS.ClientID%>";
                tbltCreaFascicolo.idCategory = "<%= IdCategory%>";
                tbltCreaFascicolo.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                tbltCreaFascicolo.pageLayoutId = "<%= pageLayout.ClientID%>";
                tbltCreaFascicolo.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID%>";
                tbltCreaFascicolo.fascicleContainerEnabled = <%= ProtocolEnv.FascicleContainerEnabled.ToString().ToLower() %>;
                tbltCreaFascicolo.initialize();
            });

            function CloseWindow() {
                tbltCreaFascicolo.closeWindow();
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadFormDecorator RenderMode="Lightweight" ID="frmDecorator" EnableRoundedCorners="false" runat="server" DecoratedControls="Fieldset"></telerik:RadFormDecorator>
    <telerik:RadPageLayout runat="server" ID="pageLayout" HtmlTag="Div" Width="100%">

        <Rows>
            <telerik:LayoutRow HtmlTag="Div" Style="margin-bottom: 10px; margin-top: 10px;  " ID="rowUDSrepository">
                <Columns>
                    <telerik:LayoutColumn Span="12" SpanMd="12">
                        <fieldset runat="server">
                            <legend>Seleziona la tipologia documento
                            </legend>
                            <telerik:RadPageLayout runat="server" HtmlTag="Div">
                                <Rows>
                                    <telerik:LayoutRow HtmlTag="Div">
                                        <Columns>
                                            <telerik:LayoutColumn SpanXl="10" SpanLg="9" Span="8">
                                                <telerik:RadComboBox runat="server" Filter="StartsWith" ID="ddlUDS" CausesValidation="false" Width="200px" ></telerik:RadComboBox>
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>
                                </Rows>
                            </telerik:RadPageLayout>
                        </fieldset>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
        </Rows>


        <Rows>
            <telerik:LayoutRow HtmlTag="Div" Style="margin-bottom: 10px; margin-top: 10px; display: none" ID="rowPeriod">
                <Columns>
                    <telerik:LayoutColumn Span="12" SpanMd="12">
                        <fieldset runat="server">
                            <legend>Seleziona il periodo
                            </legend>
                            <telerik:RadPageLayout runat="server" HtmlTag="Div">
                                <Rows>
                                    <telerik:LayoutRow HtmlTag="Div">
                                        <Columns>
                                            <telerik:LayoutColumn SpanXl="10" SpanLg="9" Span="8">
                                                <telerik:RadComboBox runat="server" ID="ddlPeriods" CausesValidation="false" Width="200px"></telerik:RadComboBox>
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>
                                </Rows>
                            </telerik:RadPageLayout>
                        </fieldset>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnSave" runat="server" Text="Salva" Width="100px" AutoPostBack="false"></telerik:RadButton>
</asp:Content>

