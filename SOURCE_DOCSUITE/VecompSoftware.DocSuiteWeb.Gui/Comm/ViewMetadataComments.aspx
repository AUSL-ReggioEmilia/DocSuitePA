<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ViewMetadataComments.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ViewMetadataComments" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ID="cn" runat="server" ContentPlaceHolderID="cphContent">
<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
    <script type="text/javascript">
        var viewMetadataComments;
        require(["Comm/ViewMetadataComments"], function (ViewMetadataComments) {
            $(function () {
                viewMetadataComments = new ViewMetadataComments(tenantModelConfiguration.serviceConfiguration);
                viewMetadataComments.discussionLabel = "<%= DiscussionLabel %>";
                viewMetadataComments.pageContentId = "<%= pageContent.ClientID %>";
                viewMetadataComments.uscNotificationId = "<%= uscNotification.ClientID %>";
                viewMetadataComments.initialize();
            });
        });


    </script>
</telerik:RadScriptBlock>

    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
    <div id="pageContent" runat="server">
    </div>
</asp:Content>

