<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscMonitoraggio.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscMonitoraggio" %>


<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">
        var uscMonitoraggio;
        require(["UserControl/uscMonitoraggio"], function (UscMonitoraggio) {
            $(function () {
                uscMonitoraggio = new UscMonitoraggio(tenantModelConfiguration.serviceConfiguration);
                uscMonitoraggio.lblArchiveId = "<%= lblArchive.ClientID %>";
                uscMonitoraggio.lblCreatedById = "<%= lblCreatedBy.ClientID %>";
                uscMonitoraggio.lblMonitoringId = "<%= lblMonitoring.ClientID %>";
                uscMonitoraggio.btnEditId = "<%= btnEdit.ClientID %>";
                uscMonitoraggio.initialize();
            });
        });

        function onRequestStart(sender, args) {
            args.set_enableAjax(false);
        }
    </script>
</telerik:RadScriptBlock>

<table id="tblMonitoraggio" class="datatable" runat="server" style="display: none;">
    <tr>
        <th colspan="2">Monitoraggio trasparenza
        </th>
    </tr>
    <tr style="display: none">
        <td class="label" style="width: 15%">Archivio:
        </td>
        <td style="width: 85%">
            <asp:Label runat="server" ID="lblArchive"></asp:Label>
        </td>
    </tr>
    <tr style="display: none">
        <td class="label">Creato da:
        </td>
        <td>
            <asp:Label runat="server" ID="lblCreatedBy"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="label">Monitoraggio:
        </td>
        <td>
            <div>
                <asp:Label runat="server" ID="lblMonitoring"></asp:Label>
                <telerik:RadImageButton CausesValidation="False" ID="btnEdit" runat="server" AutoPostBack="false" Style="height: 8px;">
                    <Image Url="~/App_Themes/DocSuite2008/imgset16/pencil.png" />
                </telerik:RadImageButton>
            </div>
        </td>
    </tr>
</table>

