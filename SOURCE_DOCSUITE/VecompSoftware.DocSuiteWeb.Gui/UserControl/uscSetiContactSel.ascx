<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscSetiContactSel.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscSetiContactSel" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>


<script type="text/javascript">
    var uscSetiContactSel;
    require(["UserControl/uscSetiContactSel"], function (UscSetiContactSel) {
        $(function () {
            uscSetiContactSel = new UscSetiContactSel(tenantModelConfiguration.serviceConfiguration);
            uscSetiContactSel.btnOpenSetiContactId = "<%= btnOpenSetiContact.ClientID%>";
            uscSetiContactSel.wndSetiContactSelId = "<%=wndSetiContactSel.ClientID%>";
            uscSetiContactSel.setiContactEnabledId = <%=ProtocolEnv.SETIIntegrationEnabled.ToString().ToLower()%>;
            uscSetiContactSel.metadataAddId = "<%= MetadataAddId%>";
            uscSetiContactSel.metadataEditId = "<%= MetadataEditId%>";
            uscSetiContactSel.fascicleInsertCommonIdEvent = "<%= FascicleInsertCommonIdEvent %>";
            uscSetiContactSel.fascicleEditCommonIdEvent = "<%= PageContentDiv.ClientID %>";
            uscSetiContactSel.initialize();
        });
    });
</script>

<telerik:RadButton Text="Ricerca" CssClass="add-icon" ID="btnOpenSetiContact"  runat="server" AutoPostBack="false"></telerik:RadButton>

<telerik:RadWindow ID="wndSetiContactSel" runat="server"></telerik:RadWindow>


