<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscDynamicMetadataSummaryClient.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDynamicMetadataSummaryClient" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<link href="../Content/site.css" rel="stylesheet" />
<telerik:RadScriptBlock runat="server" ID="RadScriptBlock">
    <script type="text/javascript">
        var uscDynamicMetadataSummaryClient;
        require(["UserControl/uscDynamicMetadataSummaryClient", "jquery", "jqueryui"], function (UscDynamicMetadataSummaryClient) {
            $(function () {
                uscDynamicMetadataSummaryClient = new UscDynamicMetadataSummaryClient(tenantModelConfiguration.serviceConfiguration)
                uscDynamicMetadataSummaryClient.pageContentId = "<%= pageContentDiv.ClientID%>";
                uscDynamicMetadataSummaryClient.uscNotificationId = "<%= uscNotification.ClientID %>";
                uscDynamicMetadataSummaryClient.componentTextId = "<%= componentText.ClientID%>";
                uscDynamicMetadataSummaryClient.componentDateId = "<%= componentDate.ClientID%>";
                uscDynamicMetadataSummaryClient.componentNumberId = "<%= componentNumber.ClientID%>";
                uscDynamicMetadataSummaryClient.componentCheckBoxId = "<%= componentCheckbox.ClientID%>";
                uscDynamicMetadataSummaryClient.componentCommentId = "<%= componentComment.ClientID%>"
                uscDynamicMetadataSummaryClient.componentEnumId = "<%= componentEnum.ClientID %>";
                uscDynamicMetadataSummaryClient.managerId = "<%= manager.ClientID%>";
                uscDynamicMetadataSummaryClient.initialize();
            });
        });

    </script>
</telerik:RadScriptBlock>

<telerik:RadWindowManager EnableViewState="False" ID="manager" runat="server">
    <Windows>
        <telerik:RadWindow Height="400" ID="managerViewComments" runat="server" Title="Metadati - Commenti" Width="650" />
    </Windows>
</telerik:RadWindowManager>

<div id="pageContentDiv" runat="server">
    <table class="maintable">
        <tr>
            <td>
                <div class="dsw-panel-content">
                    <div class="dsw-panel-content" id="menuContent">
                    </div>
                </div>
            </td>
        </tr>
    </table>
</div>

<div id="container" style="max-height: 1px!important; visibility: hidden">
    <div id="componentText" data-type="Text" runat="server" class="">
        <div class="t-row">
            <div class="t-col t-col-3 dsw-text-right col-dsw-16-important">
                <label class="control-label" id="labelNameText" style="font-weight: bold"></label>
            </div>
            <div class="t-col t-col-4 t-col-left-padding col-dsw-34-important">
                <label class="control-label" id="labelTextValue"></label>
            </div>
        </div>
    </div>
    <div id="componentComment" data-type="Comment" runat="server" class="">
        <div class="t-row">
            <div class="t-col t-col-3 dsw-text-right col-dsw-16-important">
                <label class="control-label" id="labelNameComment" style="font-weight: bold"></label>
            </div>
            <div class="t-col t-col-4 t-col-left-padding col-dsw-34-important">
                <label class="control-label" id="labelCommentValue"></label>
                <input type="image" src="../App_Themes/DocSuite2008/imgset16/search.png" class="imgButton" />
            </div>
        </div>
    </div>
    <div id="componentDate" data-type="Date" runat="server" class="">
        <div class="t-row">
            <div class="t-col t-col-3 dsw-text-right col-dsw-16-important">
                <label class="control-label" id="labelNameDate" style="font-weight: bold"></label>
            </div>
            <div class="t-col t-col-4 t-col-left-padding col-dsw-34-important">
                <label class="control-label" id="labelDateValue"></label>
            </div>
        </div>
    </div>
    <div id="componentNumber" data-type="Number" runat="server" class="">
        <div class="t-row">
            <div class="t-col t-col-3 dsw-text-right col-dsw-16-important">
                <label class="control-label" id="labelNameNumber" style="font-weight: bold"></label>
            </div>
            <div class="t-col t-col-4 t-col-left-padding col-dsw-34-important">
                <label class="control-label" id="labelNumberValue"></label>
            </div>
        </div>
    </div>
    <div id="componentCheckbox" data-type="CheckBox" runat="server" class="">
        <div class="t-row">
            <div class="t-col t-col-3 dsw-text-right col-dsw-16-important">
                <label class="control-label" id="labelNameCheckBox" style="font-weight: bold"></label>
            </div>
            <div class="t-col t-col-4 t-col-left-padding col-dsw-34-important">
                <input type="checkbox" class="form-control" name="required" id="checkboxValue" disabled="disabled" aria-readonly="true" />
            </div>
        </div>
    </div>
    <div id="componentEnum" data-type="Enum" runat="server" class="">
        <div class="t-row">
            <div class="t-col t-col-3 dsw-text-right col-dsw-16-important">
                <label class="control-label" id="labelNameEnum" style="font-weight: bold"></label>
            </div>
            <div class="t-col t-col-4 t-col-left-padding col-dsw-34-important">
                <label class="control-label" id="labelEnumValue"></label>
            </div>
        </div>
    </div>
</div>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
