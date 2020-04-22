<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscDynamicMetadataClient.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDynamicMetadataClient" %>
<link href="../Content/site.css" rel="stylesheet" />
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock">

    <script type="text/javascript">
        var uscDynamicMetadataClient;
        require(["UserControl/uscDynamicMetadataClient", "jquery", "jqueryui"], function (UscDynamicMetadataClient) {
            $(function () {
                uscDynamicMetadataClient = new UscDynamicMetadataClient(tenantModelConfiguration.serviceConfiguration)
                uscDynamicMetadataClient.pageContentId = "<%= pageContentDiv.ClientID %>";
                uscDynamicMetadataClient.uscNotificationId = "<%= uscNotification.ClientID %>";
                uscDynamicMetadataClient.componentTextId = "<%= componentText.ClientID%>";
                uscDynamicMetadataClient.componentDateId = "<%= componentDate.ClientID%>";
                uscDynamicMetadataClient.componentNumberId = "<%= componentNumber.ClientID%>";
                uscDynamicMetadataClient.componentCheckBoxId = "<%= componentCheckbox.ClientID%>";
                uscDynamicMetadataClient.componentCommentId = "<%= componentComment.ClientID%>"
                uscDynamicMetadataClient.componentEnumId = "<%= componentEnum.ClientID %>";
                uscDynamicMetadataClient.currentUser = <%= CurrentUser %>;
                uscDynamicMetadataClient.initialize();
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
    <div id="componentText" data-type="Text" style="padding: 6px;display:none" runat="server" class="">
        <div class="t-row">
            <div class="t-col t-col-2 dsw-text-right">
                <label class="control-label" id="labelNameText" style="font-weight: bold"></label>
            </div>
            <div class="t-col t-col-10 t-col-left-padding t-col-right-padding">
                <input type="text" class="riTextBox riEnabled" name="required" id="textValue" style="width: 99%" />
            </div>
        </div>
    </div>
    <div id="componentComment" data-type="Comment" style="padding: 6px;display:none" runat="server" class="">
        <div class="t-row">
            <div class="t-col t-col-2 dsw-text-right">
                <label class="control-label" id="labelNameComment" style="font-weight: bold"></label>
            </div>
            <div class="t-col t-col-10 t-col-left-padding t-col-right-padding">
                <textarea class="riTextBox riEnabled" id="commentValue" rows="3" style="width: 99%"></textarea>
            </div>
        </div>
    </div>
    <div id="componentDate" data-type="Date" style="padding: 5px;display:none" runat="server" class="">
        <div class="t-row">
            <div class="t-col t-col-2 dsw-text-right">
                <label class="control-label" id="labelNameDate" style="font-weight: bold"></label>
            </div>
            <div class="t-col t-col-10 t-col-left-padding t-col-right-padding">
                <input type="text" placeholder="__/__/____" class="riTextBox riEnabled" name="required" id="dateValue" />
                <label class="control-label" for="dateValue" id="labelValidator" style="color: red; visibility: hidden">Campo data non valido</label>
            </div>
        </div>
    </div>
    <div id="componentNumber" data-type="Number" style="padding: 5px;display:none" runat="server" class="">
        <div class="t-row">
            <div class="t-col t-col-2 dsw-text-right">
                <label class="control-label" id="labelNameNumber" style="font-weight: bold"></label>
            </div>
            <div class="t-col t-col-10 t-col-left-padding t-col-right-padding">
                <input type="number" class="riTextBox riEnabled" name="required" id="numberValue" />
            </div>
        </div>
    </div>
    <div id="componentCheckbox" data-type="CheckBox" style="padding: 5px;display:none" runat="server" class="">
        <div class="t-row">
            <div class="t-col t-col-2 dsw-text-right">
                <label class="control-label" id="labelNameCheckBox" style="font-weight: bold"></label>
            </div>
            <div class="t-col t-col-10 t-col-left-padding t-col-right-padding">
                <input type="checkbox" class="form-control" name="required" id="requiredCheckbox" />
            </div>
        </div>
    </div>
    <div id="componentEnum" data-type="Enum" style="padding: 5px;display:none" runat="server" class="">
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
