<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscRoleUserSelRest.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscRoleUserSelRest" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlockRoleUser" EnableViewState="false">
    <script type="text/javascript">
        function confirmRoleUser(sender, args) {
            uscRoleUserSelRest.confirmRoleUser(sender, args);
        }
        var uscRoleUserSelRest;
        require(["UserControl/uscRoleUserSelRest"], function (UscRoleUserSelRest) {
            $(function () {
                uscRoleUserSelRest = new UscRoleUserSelRest(tenantModelConfiguration.serviceConfiguration);
                uscRoleUserSelRest.pnlRoleUserSelRestId = "<%=pnlRoleUserSelRest.ClientID%>";
                uscRoleUserSelRest.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                uscRoleUserSelRest.rddtRoleUserId = "<%=rddtRoleUser.ClientID%>";
                uscRoleUserSelRest.ddlEnvironmentId = "<%=ddlEnvironmentRoleUser.ClientID%>";
                uscRoleUserSelRest.rowEnvironmentId = "<%=rowEnvironment.ClientID%>";
                uscRoleUserSelRest.uscNotificationId = "<%=uscNotification.ClientID%>";
                uscRoleUserSelRest.fascicleEnabled = JSON.parse("<%=FascicleEnabled%>".toLowerCase());
                uscRoleUserSelRest.protocolEnabled = JSON.parse("<%=ProtocolEnabled%>".toLowerCase());
                uscRoleUserSelRest.collaborationEnabled = JSON.parse("<%=CollaborationEnabled%>".toLowerCase());
                uscRoleUserSelRest.collaborationRightsEnabled = JSON.parse("<%=CollaborationRightsEnabled%>".toLowerCase());
                uscRoleUserSelRest.roleUserTypeLabels = <%=RoleUserTypeLabels%>;
                uscRoleUserSelRest.environments = <%=Environments%>;

                uscRoleUserSelRest.initialize();
            });
        });
    </script>

    <style>
        .rddtPopup .rddtScroll{
            height: 180px !important;
        }

        .rddtSlide,
        .rddtPopup_Office2007{
            height: 210px !important;
        }
    </style>
</telerik:RadScriptBlock>

<asp:Panel runat="server" ID="pnlRoleUserSelRest">
    <table class="datatable">
        <tr id="rowEnvironment" runat="server">
            <td class="label" >Tipologia di documento:</td>
            <td class="">
                <telerik:RadDropDownList ID="ddlEnvironmentRoleUser" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label">Utente selezionato:</td>
            <td class="">
                <telerik:RadDropDownTree RenderMode="Lightweight" ID="rddtRoleUser" runat="server" Width="450px" ExpandNodeOnSingleClick="true" CheckNodeOnClick="true" CheckBoxes="SingleCheck" >
                    <DropDownSettings AutoWidth="Disabled" />
                    <HeaderTemplate>
                        <telerik:RadButton ID="confirmRoleUserBtn" Text="Conferma selezione" runat="server" OnClientClicked="confirmRoleUser" AutoPostBack="false" />
                    </HeaderTemplate>
                </telerik:RadDropDownTree>
            </td>
        </tr>
    </table>
</asp:Panel>
<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>