<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscCustomActionsRest.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscCustomActionsRest" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock">
    <script type="text/javascript">
        var uscCustomActionsRest;
        require(["UserControl/uscCustomActionsRest", "jquery", "jqueryui"], function (UscCustomActionsRest) {
            $(function () {
                uscCustomActionsRest = new UscCustomActionsRest(tenantModelConfiguration.serviceConfiguration)
                uscCustomActionsRest.pageContentId = "<%= pageContentDiv.ClientID %>";
                uscCustomActionsRest.isSummary = <%= IsSummary.ToString().ToLower() %>;
                uscCustomActionsRest.menuContentId = "<%= menuContent.ClientID %>";
                uscCustomActionsRest.componentCheckBoxId = "<%= componentCheckbox.ClientID%>";
                uscCustomActionsRest.summaryComponentCheckboxId = "<%= summaryComponentCheckbox.ClientID %>";
                uscCustomActionsRest.summaryComponentIconId = "<%= summaryComponentIcon.ClientID %>";
                uscCustomActionsRest.initialize();
            });
        });
    </script>
</telerik:RadScriptBlock>

<div id="pageContentDiv" runat="server" style="height:100%;">
    <table class="maintable" id="mainTable" style="height:100%;">
        <tr>
            <td>
                <div class="dsw-panel-content">
                    <div class="dsw-panel-content" id="menuContent" runat="server">
                    </div>
                </div>
            </td>
        </tr>
    </table>
</div>

<div id="container" style="max-height: 1px!important; visibility: hidden">
    <div id="componentCheckbox" style="margin-bottom: 5px;" data-type="CheckBox" runat="server">
        <div>
            <label id="labelNameCheckBox" style="font-weight: bold"></label>
            <input type="radio" class="form-control" name="required" id="checkboxValue" aria-readonly="true" />
        </div>
    </div>

    <!-- Put there other custom actions input items -->

</div>

<div id="summaryContainer" style="max-height: 1px!important; visibility: hidden">
    <div id="summaryComponentCheckbox" style="margin-bottom: 5px;" data-type="CheckBox" runat="server">
        <div>
            <label id="summaryLabelNameCheckBox" style="font-weight: bold"></label>
            <input type="radio" class="form-control" name="required" id="summaryCheckboxValue" disabled="disabled" aria-readonly="true" />
        </div>
    </div>
    <div id="summaryComponentIcon" data-type="Icon" runat="server">
        <img style="background-color: yellow;" />
    </div>

    <!-- Put there other custom actions summary items -->

</div>
