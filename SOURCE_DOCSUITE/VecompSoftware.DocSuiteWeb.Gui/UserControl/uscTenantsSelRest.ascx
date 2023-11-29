<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscTenantsSelRest.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscTenantsSelRest" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
    <script type="text/javascript">
        var uscTenantsSelRest;
        require(["UserControl/uscTenantsSelRest"], function (UscTenantsSelRest) {
            $(function () {
                uscTenantsSelRest = new UscTenantsSelRest(tenantModelConfiguration.serviceConfiguration);
                uscTenantsSelRest.rddtTenantTreeId = "<%= rddtTenantTree.ClientID%>";
                uscTenantsSelRest.currentTenantId = "<%= CurrentTenantId.ToString()%>";
                uscTenantsSelRest.pageContentId = "<%=PageContent.ClientID%>";
                uscTenantsSelRest.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscTenantsSelRest.initialize();
            });
        });
    </script>
</telerik:RadScriptBlock>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

<asp:Label ID="lblTitle" runat="server" Font-Bold="True">Seleziona AOO/UO</asp:Label>
<telerik:RadDropDownTree RenderMode="Lightweight" ID="rddtTenantTree" runat="server" Width="250px" DataValueField="Value" DropDownSettings-CloseDropDownOnSelection="true"
    FullPathDelimiter=" -> " TextMode="FullPath" ExpandNodeOnSingleClick="true" CheckNodeOnClick="true" 
    DataFieldID="ID" DataTextField="Text" DataFieldParentID="ParentID">
    <DropDownSettings AutoWidth="Disabled" Height="210px" />
</telerik:RadDropDownTree>
