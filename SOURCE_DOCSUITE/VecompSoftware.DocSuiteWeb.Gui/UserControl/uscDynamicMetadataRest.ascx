<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscDynamicMetadataRest.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDynamicMetadataRest" %>
<link href="../Content/site.css" rel="stylesheet" />
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock">

    <script type="text/javascript">
        var uscDynamicMetadataRest;
        require(["UserControl/uscDynamicMetadataRest", "jquery", "jqueryui"], function (UscDynamicMetadataRest) {
            $(function () {
                uscDynamicMetadataRest = new UscDynamicMetadataRest(tenantModelConfiguration.serviceConfiguration)
                uscDynamicMetadataRest.pageContentId = "<%= pageContentDiv.ClientID %>";
                uscDynamicMetadataRest.uscNotificationId = "<%= uscNotification.ClientID %>";
                uscDynamicMetadataRest.componentTextId = "<%= componentText.ClientID%>";
                uscDynamicMetadataRest.componentDateId = "<%= componentDate.ClientID%>";
                uscDynamicMetadataRest.componentNumberId = "<%= componentNumber.ClientID%>";
                uscDynamicMetadataRest.componentCheckBoxId = "<%= componentCheckbox.ClientID%>";
                uscDynamicMetadataRest.componentCommentId = "<%= componentComment.ClientID%>"
                uscDynamicMetadataRest.componentEnumId = "<%= componentEnum.ClientID %>";
                uscDynamicMetadataRest.currentUser = <%= CurrentUser %>;
                uscDynamicMetadataRest.validationEnabled = <%= ValidationEnabled.ToString().ToLower() %>;
                uscDynamicMetadataRest.initialize();
            });
        });

    </script>
</telerik:RadScriptBlock>

<div style="margin-right: 5px;" id="pageContentDiv" runat="server">
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

<div id="container" style="max-height: 1px!important">
    <div id="componentText" data-type="Text" style="padding: 6px; display: none" runat="server" class="">
        <div class="t-row">
            <div class="t-col t-col-2 dsw-text-right">
                <label class="control-label" id="labelNameText" style="font-weight: bold"></label>
            </div>
            <div class="t-col t-col-10 t-col-left-padding t-col-right-padding">
                <input type="text" class="riTextBox riEnabled" name="required" id="textValue" style="width: 99%" />
            </div>
        </div>
    </div>
    <div id="componentComment" data-type="Comment" style="padding: 6px; display: none" runat="server" class="">
        <div class="t-row">
            <div class="t-col t-col-2 dsw-text-right">
                <label class="control-label" id="labelNameComment" style="font-weight: bold"></label>
            </div>
            <div class="t-col t-col-10 t-col-left-padding t-col-right-padding">
                <textarea class="riTextBox riEnabled" id="commentValue" rows="3" style="width: 99%"></textarea>
            </div>
        </div>
    </div>
    <div id="componentDate" data-type="Date" style="padding: 5px; display: none" runat="server" class="">
        <div class="t-row">
            <div class="t-col t-col-2 dsw-text-right">
                <label class="control-label" id="labelNameDate" style="font-weight: bold"></label>
            </div>
            <div class="t-col t-col-10 t-col-left-padding t-col-right-padding">
                <input type="date" class="riTextBox riEnabled" name="required" id="dateValue" />
                <label class="control-label" for="dateValue" id="labelValidator" style="color: red; visibility: hidden">Campo data non valido</label>
            </div>
        </div>
    </div>
    <div id="componentNumber" data-type="Number" style="padding: 5px; display: none" runat="server" class="">
        <div class="t-row">
            <div class="t-col t-col-2 dsw-text-right">
                <label class="control-label" id="labelNameNumber" style="font-weight: bold"></label>
            </div>
            <div class="t-col t-col-10 t-col-left-padding t-col-right-padding">
                <input type="number" class="riTextBox riEnabled" name="required" id="numberValue" />
            </div>
        </div>
    </div>
    <div id="componentCheckbox" data-type="CheckBox" style="padding: 5px; display: none" runat="server" class="">
        <div class="t-row">
            <div class="t-col t-col-2 dsw-text-right">
                <label class="control-label" id="labelNameCheckBox" style="font-weight: bold"></label>
            </div>
            <div class="t-col t-col-10 t-col-left-padding t-col-right-padding">
                <input type="checkbox" class="form-control" name="required" id="requiredCheckbox" />
            </div>
        </div>
    </div>
    <div id="componentEnum" data-type="Enum" style="padding: 5px; display: none" runat="server" class="">
        <div class="t-row">
            <div class="t-col t-col-2 dsw-text-right">
                <label class="control-label" id="labelNameEnum" style="font-weight: bold"></label>
            </div>
            <div class="t-col t-col-10 t-col-left-padding t-col-right-padding">
                <select id="ddlOptions" class="selectClass"></select>
            </div>
        </div>
    </div>
</div>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
