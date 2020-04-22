<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscMulticlassificationRest.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscMulticlassificationRest" %>


<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
    <script type="text/javascript">

        var multiclasification;
        require(["UserControl/uscMulticlassificationRest"], function (UscMulticlassification) {
            $(function () {
                multiclasification = new UscMulticlassification(tenantModelConfiguration.serviceConfiguration);
                multiclasification.radTreeCategoriesId = "<%= RadTreeCategories.ClientID %>";
                multiclasification.idDocumentUnit = "<%= IdDocumentUnit %>";
                multiclasification.isVisible = "<%= Visible %>";
                multiclasification.multiclassificationContainer = document.getElementById("multiclassification-container");
                multiclasification.initialize();
            });
        });
    </script>
</telerik:RadScriptBlock>

<div id="multiclassification-container">
    <telerik:RadPanelBar runat="server" AllowCollapseAllItems="true" ExpandMode="MultipleExpandedItems" Width="100%" ID="rpbCategories" Visible="true">
        <Items>
            <telerik:RadPanelItem CssClass="control-label" Text="Classificazioni ereditate dai procedimenti amministrativi" Expanded="true" runat="server">
                <ContentTemplate>
                    <telerik:RadTreeView CssClass="form-group" ID="RadTreeCategories" runat="server" Width="100%">
                    </telerik:RadTreeView>
                </ContentTemplate>
            </telerik:RadPanelItem>
        </Items>
    </telerik:RadPanelBar>
</div>
