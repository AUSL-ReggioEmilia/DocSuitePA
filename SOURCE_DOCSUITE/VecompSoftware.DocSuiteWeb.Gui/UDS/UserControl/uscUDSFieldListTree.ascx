<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscUDSFieldListTree.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscUDSFieldListTree" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagPrefix="usc" TagName="uscErrorNotification" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
    <script type="text/javascript">
        var uscUDSFieldListTree;
        require(["UDS/UserControl/uscUDSFieldListTree"], function (UscUDSFieldListTree) {
            $(function () {
                uscUDSFieldListTree = new UscUDSFieldListTree(tenantModelConfiguration.serviceConfiguration, "<%= Me.ClientID %>");
                uscUDSFieldListTree.rtvReadOnlyUDSFieldListId = "<%= rtvReadOnlyUDSFieldList.ClientID %>";
                uscUDSFieldListTree.rddtUDSFieldListId = "<%= rddtUDSFieldList.ClientID %>";
                uscUDSFieldListTree.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                uscUDSFieldListTree.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscUDSFieldListTree.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                uscUDSFieldListTree.idUDSRepository = "<%= If(IdUDSRepository.HasValue, IdUDSRepository.ToString(), String.Empty) %>";
                uscUDSFieldListTree.udsFieldListChildren = '<%= UDSFieldListChildren %>';
                uscUDSFieldListTree.isReadOnly = <%= IsReadOnly.ToString().ToLower() %>;
                uscUDSFieldListTree.hiddenFieldListId = "<%= HiddenFieldId %>";
                uscUDSFieldListTree.initialize();
            });
        });

        function loadUDSFieldListTree(idUDSRepository) {
            if (uscUDSFieldListTree) {
                uscUDSFieldListTree.loadUDSFieldListTree(idUDSRepository);
            }
        }
    </script>
</telerik:RadScriptBlock>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
<telerik:RadTreeView runat="server" ID="rtvReadOnlyUDSFieldList" />
<telerik:RadDropDownTree runat="server" ID="rddtUDSFieldList" Width="100%" FullPathDelimiter=" -> " TextMode="FullPath" AutoPostBack="false"  />
<asp:RequiredFieldValidator runat="server" ID="rfvRddtUDSFieldList" ControlToValidate="rddtUDSFieldList" Display="Dynamic" ErrorMessage="Campo obbligatorio"/>