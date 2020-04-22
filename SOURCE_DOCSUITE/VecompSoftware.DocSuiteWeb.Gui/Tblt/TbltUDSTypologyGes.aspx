<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltUDSTypologyGes" CodeBehind="TbltUDSTypologyGes.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">
            var tbltUDSTypologyGes;
            require(["Tblt/TbltUDSTypologyGes"], function (TbltUDSTypologyGes) {
                $(function () {
                    tbltUDSTypologyGes = new TbltUDSTypologyGes(tenantModelConfiguration.serviceConfiguration);
                    tbltUDSTypologyGes.btnConfirmId = "<%= btnConfirm.ClientID %>";
                    tbltUDSTypologyGes.txtNameId = "<%= txtName.ClientID %>";
                    tbltUDSTypologyGes.txtOldNameId = "<%= txtOldName.ClientID %>";
                    tbltUDSTypologyGes.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    tbltUDSTypologyGes.action = "<%= Action%>";
                    tbltUDSTypologyGes.currentUDSTypologyId = "<%= IdUDSTypology%>";
                    tbltUDSTypologyGes.rowOldNameId = "<%= rowOldName.ClientID%>";
                    tbltUDSTypologyGes.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>


    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
    <telerik:RadPageLayout runat="server" ID="pageContent" HtmlTag="Div" Width="100%" Height="90%">
        <Rows>
            <telerik:LayoutRow Style="margin-top: 5px;" runat="server" ID="rowOldName">
                <Columns>
                    <telerik:LayoutColumn Span="2" Height="30px">
                        <b>Nome attuale: </b>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="10" CssClass="t-col-left-padding t-col-right-padding" Height="30px">
                        <telerik:RadTextBox ID="txtOldName" runat="server" Width="98%" Enabled="false"/>                        
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow Style="margin-top: 5px;">
                <Columns>
                    <telerik:LayoutColumn Span="2" Height="30px">
                        <b>Nome: </b>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="10" CssClass="t-col-left-padding t-col-right-padding" Height="30px">
                        <telerik:RadTextBox ID="txtName" runat="server" Width="98%" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtName" EnableViewState="false" ErrorMessage="Il campo Nome è obbligatorio" Display="Dynamic" ID="rfvName" />
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnConfirm" runat="server" AutoPostBack="false" Text="Conferma"></telerik:RadButton>
</asp:Content>

