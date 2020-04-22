<%@ Page Title="Cartella - Modifica" Language="vb" AutoEventWireup="false" CodeBehind="FascicleFolderModifica.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Inherits="VecompSoftware.DocSuiteWeb.Gui.FascicleFolderModifica" %>

<%@ Register Src="../UserControl/uscClassificatore.ascx" TagName="uscCategory" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="settori" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="radScriptBlock">
        <script type="text/javascript">
            var fascicleFolderModifica;
            require(["Fasc/FascicleFolderModifica"], function (FascicleFolderModifica) {
                $(function () {
                    fascicleFolderModifica = new FascicleFolderModifica(tenantModelConfiguration.serviceConfiguration);
                    fascicleFolderModifica.currentFascicleFolderId = "<%= IdFascicleFolder%>";
                    fascicleFolderModifica.currentPageId = "<%= pageContent.ClientID%>";
                    fascicleFolderModifica.btnConfermaId = "<%= btnConferma.ClientID %>";
                    fascicleFolderModifica.txtNameId = "<%= txtName.ClientID%>"; 
                    fascicleFolderModifica.nameRowId = "<%= nameRow.ClientID%>";
                    fascicleFolderModifica.loadingPanelId = "<% = MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>";
                    fascicleFolderModifica.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID%>";
                    fascicleFolderModifica.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    fascicleFolderModifica.sessionUniqueKey = "<%= SessionUniqueKey %>";
                    fascicleFolderModifica.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
    <telerik:RadNotification ID="radNotification" runat="server"
        VisibleOnPageLoad="false" LoadContentOn="PageLoad" Width="400" Height="200" Animation="FlyIn"
        EnableRoundedCorners="true" EnableShadow="true" ContentIcon="delete" Title="Errore pagina" TitleIcon="none" AutoCloseDelay="0" Position="Center" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <usc:uscErrorNotification runat="server" ID="uscNotification" />
    <telerik:RadFormDecorator RenderMode="Lightweight" ID="frmDecorator" EnableRoundedCorners="false" runat="server" DecoratedControls="Fieldset" />
    <telerik:RadPageLayout runat="server" ID="pageContent" HtmlTag="Div" Width="100%" Height="95%">
        <Rows>
            <telerik:LayoutRow CssClass="windowTitle" ID="nameRow">
                <Columns>
                    <telerik:LayoutColumn Span="2" Height="35px">
                        <b>Nome: </b>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="10" CssClass="t-col-left-padding t-col-right-padding" Height="35px">
                        <telerik:RadTextBox ID="txtName" runat="server" Width="100%" />                        
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>            
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadPageLayout runat="server" ID="RadPageLayout1" HtmlTag="Div" Width="100%" Height="100%">
        <Rows>
            <telerik:LayoutRow>
                <Content>
                    <telerik:RadButton Text="Conferma" runat="server" ID="btnConferma" AutoPostBack="false" />
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>
