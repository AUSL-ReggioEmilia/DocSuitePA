<%@ Page Title="Cartella - Inserimento" Language="vb" AutoEventWireup="false" CodeBehind="DossierFascicleFolderInserimento.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Inherits="VecompSoftware.DocSuiteWeb.Gui.DossierFascicleFolderInserimento" %>

<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="settori" TagPrefix="usc" %>
<%@ Register Src="../UserControl/uscFascicleLink.ascx" TagName="uscFascicleLink" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscFascicleInsert.ascx" TagName="uscFascicleInsert" TagPrefix="usc" %>


<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock">
        <script type="text/javascript">
            var dossierFascicleFolderInserimento;
            require(["Dossiers/DossierFascicleFolderInserimento"], function (DossierFascicleFolderInserimento) {
                $(function () {
                    dossierFascicleFolderInserimento = new DossierFascicleFolderInserimento(tenantModelConfiguration.serviceConfiguration);
                    dossierFascicleFolderInserimento.currentDossierId = "<%= IdDossier%>";
                    dossierFascicleFolderInserimento.currentPageId = "<%= pageContent.ClientID%>";                   
                    dossierFascicleFolderInserimento.btnConfermaId = "<%= btnConferma.ClientID%>";
                    dossierFascicleFolderInserimento.ajaxManagerId = "<%= AjaxManager.ClientID%>";
                    dossierFascicleFolderInserimento.managerId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    dossierFascicleFolderInserimento.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    dossierFascicleFolderInserimento.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    dossierFascicleFolderInserimento.persistanceDisabled = <%= PersistanceDisabled.ToString().ToLower() %>;
                    
                    dossierFascicleFolderInserimento.fascicleTypeRow = "<%=fascicleTypeRow.ClientID%>";
                    dossierFascicleFolderInserimento.uscFascInsertId = "<%= uscFascicleInsert.PageContentDiv.ClientID %>";                                
                    dossierFascicleFolderInserimento.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
    <telerik:RadNotification ID="radNotification" runat="server"
        VisibleOnPageLoad="false" LoadContentOn="PageLoad" Width="400" Height="200" Animation="FlyIn"
        EnableRoundedCorners="true" EnableShadow="true" ContentIcon="delete" Title="Errore pagina" TitleIcon="none" AutoCloseDelay="0" Position="Center" />
</asp:Content>


<asp:Content runat="server" ContentPlaceHolderID="cphContent">
       <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
        <telerik:RadFormDecorator RenderMode="Lightweight" ID="frmDecorator"  EnableRoundedCorners="false" runat="server" DecoratedControls="Fieldset"></telerik:RadFormDecorator>
    <telerik:RadPageLayout runat="server" ID="pageContent" HtmlTag="Div" Width="100%" Height="95%">
        <Rows>
            <telerik:LayoutRow runat="server" id="fascicleTypeRow" HtmlTag="Div" CssClass="ts-initialize">
                <Content>
                    <usc:uscFascicleInsert runat="server" ID="uscFascicleInsert" Required="false"/>
                </Content>
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
