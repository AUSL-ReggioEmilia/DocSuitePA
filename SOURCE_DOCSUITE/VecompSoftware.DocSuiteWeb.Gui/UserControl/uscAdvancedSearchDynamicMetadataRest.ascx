<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscAdvancedSearchDynamicMetadataRest.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscAdvancedSearchDynamicMetadataRest" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<link href="../Content/site.css" rel="stylesheet" />

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock">

    <script type="text/javascript">
        var uscAdvancedSearchDynamicMetadataRest;
        require(["UserControl/uscAdvancedSearchDynamicMetadataRest", "jquery", "jqueryui"], function (UscAdvancedSearchDynamicMetadataRest) {
            $(function () {
                uscAdvancedSearchDynamicMetadataRest = new UscAdvancedSearchDynamicMetadataRest(tenantModelConfiguration.serviceConfiguration);
                uscAdvancedSearchDynamicMetadataRest.uscErrorNotificationId = "<%= uscErrorNotification.ClientID %>";
                uscAdvancedSearchDynamicMetadataRest.pageContentId = "<%= pageContentDiv.ClientID %>";
                uscAdvancedSearchDynamicMetadataRest.componentTextId = "<%= componentText.ClientID%>";
                uscAdvancedSearchDynamicMetadataRest.componentDateId = "<%= componentDate.ClientID%>";
                uscAdvancedSearchDynamicMetadataRest.componentNumberId = "<%= componentNumber.ClientID%>";
                uscAdvancedSearchDynamicMetadataRest.componentCheckBoxId = "<%= componentCheckbox.ClientID%>";
                uscAdvancedSearchDynamicMetadataRest.componentCommentId = "<%= componentComment.ClientID%>"
                uscAdvancedSearchDynamicMetadataRest.componentEnumId = "<%= componentEnum.ClientID %>";
                uscAdvancedSearchDynamicMetadataRest.initialize();
            });
        });

    </script>
</telerik:RadScriptBlock>

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

<div id="container" style="max-height: 1px!important">
    <div id="componentText" data-type="Text" runat="server" class="dsw-display-none">
        <div class="t-row" style="margin: 0px;">
            <div class="t-col t-col-3 t-col-left-padding t-col-right-padding">
                <label class="control-label dsw-vertical-middle dsw-text-bold" id="labelNameText"></label>
                <input type="text" class="riTextBox riEnabled col-dsw-10 dsw-display-block" id="textValue" style="margin: 5px 0px;" />
            </div>
        </div>
    </div>
    <div id="componentComment" data-type="Comment" runat="server" class="dsw-display-none">
        <div class="t-row" style="margin: 0px;">
            <div class="t-col t-col-4 t-col-left-padding t-col-right-padding">
                <label class="control-label dsw-vertical-middle dsw-text-bold" id="labelNameComment"></label>
                <textarea class="riTextBox riEnabled col-dsw-8 dsw-display-block" id="commentValue" rows="3" style="margin: 5px 0px;" ></textarea>
            </div>
        </div>
    </div>
    <div id="componentDate" data-type="Date" runat="server" class="dsw-display-none">
        <div class="t-row" style="margin: 0px;">
            <div class="t-col t-col-6 t-col-left-padding t-col-right-padding">
                <label class="control-label dsw-vertical-middle dsw-text-bold" id="labelNameDate"></label>

                <div style="width: 100%;">
                    <label class="strongRiLabel" style="margin-left: 10px;">Da</label>
                    <input type="text" placeholder="__/__/____" class="riTextBox riEnabled dateTextBox" id="dateValueFrom" style="margin: 5px 0px; width: 110px;" />

                    <label class="strongRiLabel" style="margin-left: 5px;">A</label>
                    <input type="text" placeholder="__/__/____" class="riTextBox riEnabled dateTextBox" id="dateValueTo" style="margin: 5px 0px; width: 110px;" />
                    <label class="control-label" for="dateValue" id="dateErrorMsgLabel" style="color: red; visibility: hidden; margin-left: 10px;">Campo data non valido</label>
                </div>
            </div>
        </div>
    </div>
    <div id="componentNumber" data-type="Number" runat="server" class="dsw-display-none">
        <div class="t-row" style="margin: 0px;">
            <div class="t-col t-col-6 t-col-left-padding t-col-right-padding">
                <label class="control-label dsw-vertical-middle dsw-text-bold" id="labelNameNumber"></label>

                <div class="col-dsw-10">
                    <label class="strongRiLabel" style="margin-left: 10px;">Da</label>
                    <input type="number" class="riTextBox riEnabled numberTextBox" id="numberValueFrom" style="margin: 5px 0px; width: 118px;" />

                    <label class="strongRiLabel" style="margin-left: 5px;">A</label>
                    <input type="number" class="riTextBox riEnabled numberTextBox" id="numberValueTo" style="margin: 5px 0px; width: 118px;" />
                    <label class="control-label" for="dateValue" id="numberErrorMsgLabel" style="color: red; visibility: hidden; margin-left: 10px;">Campo numero non valido</label>
                </div>
            </div>
        </div>
    </div>
    <div id="componentCheckbox" data-type="CheckBox" runat="server" class="dsw-display-none">
        <div class="t-row" style="margin: 0px;">
            <div class="t-col t-col-3 t-col-left-padding t-col-right-padding">
                <label class="control-label dsw-vertical-middle dsw-text-bold" id="labelNameCheckBox"></label>

                <input type="checkbox" class="form-control" id="requiredCheckbox" style="margin: 5px 0px;" />
            </div>
        </div>
    </div>
    <div id="componentEnum" data-type="Enum" runat="server" class="dsw-display-none">
        <div class="t-row" style="margin: 0px;">
            <div class="t-col t-col-3 t-col-left-padding t-col-right-padding">
                <label class="control-label dsw-vertical-middle dsw-text-bold" id="labelNameEnum"></label>

                <select id="ddlOptions" class="selectClass col-dsw-4 dsw-display-block" style="margin: 5px 0px;"></select>
            </div>
        </div>
    </div>
</div>

<usc:uscErrorNotification runat="server" ID="uscErrorNotification"></usc:uscErrorNotification>
