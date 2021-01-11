<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscWorkflowFolderSelRest.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscWorkflowFolderSelRest" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
    <script type="text/javascript">

        var uscWorkflowFolderSelRest;
        require(["UserControl/uscWorkflowFolderSelRest"], function (UscWorkflowFolderSelRest) {
            $(function () {
                uscWorkflowFolderSelRest = new UscWorkflowFolderSelRest(tenantModelConfiguration.serviceConfiguration);
                uscWorkflowFolderSelRest.rtvWorkflowFolderSelRestId = "<%=rtvWorkflowFolderSelRest.ClientID%>"
                uscWorkflowFolderSelRest.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscWorkflowFolderSelRest.pageContentId = "<%=TableContentControl.ClientID%>";
                uscWorkflowFolderSelRest.validatorAnyNodeId = "<%= AnyNodeCheck.ClientID%>";
                uscWorkflowFolderSelRest.validatorTemplateNodeCheckId = "<%= templateNodeCheck.ClientID%>";
                uscWorkflowFolderSelRest.initialize();
            });
        });
    </script>
</telerik:RadScriptBlock>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

<div class="dsw-panel-content" id="dossiersWithTemplatesContainer" runat="server">
    <telerik:RadTreeView ID="rtvWorkflowFolderSelRest" runat="server" Width="100%">
    </telerik:RadTreeView>
</div>

<div>
    <asp:CustomValidator runat="server" ID="AnyNodeCheck"
        ControlToValidate="rtvWorkflowFolderSelRest"
        ValidateEmptyText="true"
        EnableClientScript="true"
        Enabled="false"
        ClientValidationFunction="anySelectedNode"
        Display="Dynamic" ErrorMessage="La selezione della cartella del dossier è obbligatoria" />

    <asp:CustomValidator runat="server" ID="templateNodeCheck"
        ControlToValidate="rtvWorkflowFolderSelRest"
        ValidateEmptyText="true"
        EnableClientScript="true"
        Enabled="false"
        ClientValidationFunction="uscWorkflowFolderSelRest.validateTemplateSelectedNode"
        Display="Dynamic" ErrorMessage="Seleziona una cartella del dossier che ha un template definito" />
</div>
