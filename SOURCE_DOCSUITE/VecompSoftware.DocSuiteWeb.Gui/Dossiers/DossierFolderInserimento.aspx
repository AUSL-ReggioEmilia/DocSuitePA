<%@ Page Title="Cartella - Inserimento" Language="vb" AutoEventWireup="false" CodeBehind="DossierFolderInserimento.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Inherits="VecompSoftware.DocSuiteWeb.Gui.DossierFolderInserimento" %>

<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="settori" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscFascicleInsert.ascx" TagName="uscFascicleInsert" TagPrefix="usc" %>


<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock">
        <script type="text/javascript">
            var dossierFolderInserimento;
            require(["Dossiers/DossierFolderInserimento"], function (DossierFolderInserimento) {
                $(function () {
                    dossierFolderInserimento = new DossierFolderInserimento(tenantModelConfiguration.serviceConfiguration);
                    dossierFolderInserimento.currentDossierId = "<%= IdDossier%>";
                    dossierFolderInserimento.currentPageId = "<%= pageContent.ClientID%>";
                    dossierFolderInserimento.txtNameId = "<%= txtName.ClientID%>";
                    dossierFolderInserimento.btnConfermaId = "<%= btnConferma.ClientID%>";
                    dossierFolderInserimento.btnConfermaUniqueId = "<%= btnConferma.UniqueID%>";
                    dossierFolderInserimento.ajaxManagerId = "<%= AjaxManager.ClientID%>";
                    dossierFolderInserimento.managerId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    dossierFolderInserimento.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    dossierFolderInserimento.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    dossierFolderInserimento.uscFolderAccountedId = "<%= uscFolderAccounted.TableContentControl.ClientID%>";
                    dossierFolderInserimento.persistanceDisabled = <%= PersistanceDisabled.ToString().ToLower() %>;
                    dossierFolderInserimento.dossierRolesRowId = "<%= dossierRolesRow.ClientID%>";
                    dossierFolderInserimento.dossierNameRowId = "<%= dossierNameRow.ClientID%>";                   
                    dossierFolderInserimento.initialize();
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
            <telerik:LayoutRow CssClass="windowTitle ts-initialize" ID="dossierNameRow" >
                <Columns>
                    <telerik:LayoutColumn Span="2" Height="35px">
                        <b>Nome: </b>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="10" CssClass="t-col-left-padding t-col-right-padding" Height="35px">
                        <telerik:RadTextBox ID="txtName" runat="server" Width="100%"/>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>

            <telerik:LayoutRow HtmlTag="Div" ID="dossierRolesRow" CssClass="ts-initialize">
                <Content>
                       <usc:settori Caption="Settori autorizzati" ID="uscFolderAccounted" Required="false" CausesValidation="false" RoleRestictions="OnlyMine" MultipleRoles="True" MultiSelect="True" runat="server" Environment="Document" UseSessionStorage="true"/>
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
