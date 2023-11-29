<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscUDSDynamics.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscUDSDynamics" %>
<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagPrefix="usc" TagName="UploadDocument" %>
<%@ Register Src="~/UserControl/uscSettori.ascx" TagPrefix="usc" TagName="Settori" %>
<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagPrefix="usc" TagName="Contatti" %>

<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        function customValidateControl(sender, args) {
            var controlId = sender.id.replace("validator_", "");
            var ctrl = $("#" + chkControlId);
            if (ctrl.is("checkbox")) {
                args.IsValid = ctrl.checked;
            }
        }

        function loadControlsValues(metadataModel, controlList) {
            for (var i = 0; i < controlList.length; i++) {
                if (controlList[i].ControlName === "RadDropDownList") {
                    for (var j = 0; j < metadataModel[0].Items.length; j++) {
                        var metadataItem = metadataModel[0].Items[j];
                        if (controlList[i].ClientID.endsWith(metadataItem.ColumnName.toLowerCase())) {
                            var ctrl = $find(controlList[i].ClientID);
                            for (var k = 0; k < ctrl.get_items().get_count(); k++) {
                                var ctrlItem = ctrl.get_items().getItem(k);
                                var value = getMetadataValue(metadataItem.Value);
                                if (ctrlItem.get_text() === value) {
                                    ctrlItem.select();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        function getMetadataValue(str) {
            try {
                return JSON.parse(str)[0];
            } catch (e) {
                return str;
            }
        }
    </script>
</telerik:RadScriptBlock>

<telerik:RadFormDecorator RenderMode="Lightweight" ID="frmDecorator" runat="server" DecoratedControls="Fieldset"></telerik:RadFormDecorator>
<asp:PlaceHolder runat="server" ID="dynamicControls" />
<input type="hidden" id="hiddenLookup" runat="server" />
<input type="hidden" id="hiddenFieldList" runat="server" />