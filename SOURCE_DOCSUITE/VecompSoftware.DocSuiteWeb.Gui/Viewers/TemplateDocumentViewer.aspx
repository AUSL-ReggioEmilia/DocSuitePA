<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/Base.Master" CodeBehind="TemplateDocumentViewer.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TemplateDocumentViewer" %>
<%@ Register Src="ViewerLight.ascx" TagName="uscViewerLight" TagPrefix="uc1" %>

<asp:Content ContentPlaceHolderID="cphMain" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            function showLoadingPanel () {
                $('#' + '<%= loadingPanel.ClientID %>').show();
                HidePDFActivex();
            }

            function hideLoadingPanel() {
                $('#' + '<%= loadingPanel.ClientID %>').hide();
                ShowPDFActivex();
            }
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadAjaxLoadingPanel ID="loadingPanel" runat="server" Transparency="30">
        <div class="loading">
            <asp:ImageButton ID="btnEnablePreview" CssClass="loading-img" ToolTip="Visualizza anteprima del documento" runat="server" ImageUrl="../App_Themes/DocSuite2008/images/download_preview.png" AlternateText="loading"></asp:ImageButton>
        </div>
    </telerik:RadAjaxLoadingPanel>

    <div runat="server" class="viewerWrapper">
        <uc1:uscViewerLight CheckBoxes="true" ID="ViewerLight" AlwaysDocumentTreeOpen="false" runat="server" />
    </div>
</asp:Content>
