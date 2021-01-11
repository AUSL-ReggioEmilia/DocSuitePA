<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscUDSLookup.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscUDSLookup" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript">
            var uscUDSLookup;
            require(["UDS/UserControl/uscUDSLookup"], function (UscUDSLookup) {
                $(function () {
                    uscUDSLookup = new UscUDSLookup(tenantModelConfiguration.serviceConfiguration);
                    uscUDSLookup.UDSName = "<%= UDSName %>";
                    uscUDSLookup.propertyName = "<%= PropertyName %>";
                    uscUDSLookup.lookupValue = '<%= LookupValue %>';
                    uscUDSLookup.rcbLookupId = "<%= rcbLookup.ClientID %>";
                    uscUDSLookup.maxNumberElements = "<%= MaxNumberDropdownElements %>";
                    uscUDSLookup.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    uscUDSLookup.checkBoxesEnabled = <%= CheckBoxesEnabled.ToString().ToLower() %>;
                    uscUDSLookup.hiddenLookupId = "<%= HiddenFieldId %>";
                    uscUDSLookup.lookupLabel = "<%= LookupLabel %>";
                    uscUDSLookup.initialize();
                });
            });

        </script>
    </telerik:RadScriptBlock>


<asp:Panel runat="server" ID="pnlLookup">
     <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
    <telerik:RadComboBox ID="rcbLookup" AllowCustomText="false" Width="250px" CausesValidation="false" EnableLoadOnDemand="true"
        EnableVirtualScrolling="false" ShowMoreResultsBox="true" AutoPostBack="false" MaxHeight="160px" runat="server" />    
</asp:Panel>
<asp:RequiredFieldValidator runat="server" ID="rfvLookup" ControlToValidate="rcbLookup" Display="Dynamic" ErrorMessage="Campo obbligatorio"/>