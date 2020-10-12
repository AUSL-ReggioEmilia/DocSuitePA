<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscDynamicMetadataSummaryRest.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDynamicMetadataSummaryRest" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<link href="../Content/site.css" rel="stylesheet" />
<telerik:RadScriptBlock runat="server" ID="RadScriptBlock">
    <script type="text/javascript">
        var uscDynamicMetadataSummaryRest;
        require(["UserControl/uscDynamicMetadataSummaryRest", "jquery", "jqueryui"], function (UscDynamicMetadataSummaryRest) {
            $(function () {
                uscDynamicMetadataSummaryRest = new UscDynamicMetadataSummaryRest(tenantModelConfiguration.serviceConfiguration)
                uscDynamicMetadataSummaryRest.pageContentId = "<%= pageContentDiv.ClientID%>";
                uscDynamicMetadataSummaryRest.uscNotificationId = "<%= uscNotification.ClientID %>";
                uscDynamicMetadataSummaryRest.componentTextId = "<%= componentText.ClientID%>";
                uscDynamicMetadataSummaryRest.componentDateId = "<%= componentDate.ClientID%>";
                uscDynamicMetadataSummaryRest.componentNumberId = "<%= componentNumber.ClientID%>";
                uscDynamicMetadataSummaryRest.componentCheckBoxId = "<%= componentCheckbox.ClientID%>";
                uscDynamicMetadataSummaryRest.componentCommentId = "<%= componentComment.ClientID%>"
                uscDynamicMetadataSummaryRest.componentEnumId = "<%= componentEnum.ClientID %>";
                uscDynamicMetadataSummaryRest.managerId = "<%= manager.ClientID%>";
                uscDynamicMetadataSummaryRest.initialize();
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
