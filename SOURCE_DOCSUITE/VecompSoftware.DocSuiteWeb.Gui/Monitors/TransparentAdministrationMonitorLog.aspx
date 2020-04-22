<%@ Page Language="vb" Title="Monitoraggio amministrazione trasparente" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="TransparentAdministrationMonitorLog.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TransparentAdministrationMonitorLog" %>

<%@ Import Namespace="VecompSoftware.DocSuiteWeb.Entity.Monitors" %>

<%@ Register Src="~/UserControl/uscAmmTraspMonitorLogGrid.ascx" TagName="uscAmmTraspMonitorLogGrid" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="Settori" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var transparentAdministrationMonitorLog;
            require(["Monitors/TransparentAdministrationMonitorLog"], function (TransparentAdministrationMonitorLog) {
                $(function () {
                    transparentAdministrationMonitorLog = new TransparentAdministrationMonitorLog(tenantModelConfiguration.serviceConfiguration);
                    transparentAdministrationMonitorLog.uscAmmTraspMonitorLogGridId = "<%= uscAmmTraspMonitorLogGrid.PageContentDiv.ClientID %>";
                    transparentAdministrationMonitorLog.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    transparentAdministrationMonitorLog.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    transparentAdministrationMonitorLog.btnSearchId = "<%= btnSearch.ClientID%>";
                    transparentAdministrationMonitorLog.btnCleanId = "<%= btnClean.ClientID %>";
                    transparentAdministrationMonitorLog.dpStartDateFromId = "<%= dtpDateFrom.ClientID%>";
                    transparentAdministrationMonitorLog.dpEndDateFromId = "<%= dtpDateTo.ClientID%>";
                    transparentAdministrationMonitorLog.transparentAdministrationMonitorLogItemsId = "transparentAdministrationMonitorLogItems";
                    transparentAdministrationMonitorLog.txtUsernameId = "<%= txtUsername.ClientID %>";
                    transparentAdministrationMonitorLog.cmbDocumentTypeId = "<%= cmbDocumentType.ClientID %>";
                    transparentAdministrationMonitorLog.cmbContainerId = "<%= cmbContainer.ClientID %>";
                    transparentAdministrationMonitorLog.uscOwnerRoleId = "<%= uscOwnerRole.TableContentControl.ClientID%>";
                     transparentAdministrationMonitorLog.maxNumberElements = "<%= ProtocolEnv.MaxNumberDropdownElements %>";
                    transparentAdministrationMonitorLog.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>

    <table class="datatable">
        <tr>
            <td class="label labelPanel" style="width: 20%;">Periodo:
            </td>
            <td style="width: 80%;">
                <telerik:RadDatePicker DateInput-LabelWidth="30%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="300" DateInput-Label="Dalla data" ID="dtpDateFrom" runat="server" />
                <asp:RequiredFieldValidator runat="server" Display="Dynamic" ControlToValidate="dtpDateFrom" ErrorMessage="Il campo data è obbligatorio"></asp:RequiredFieldValidator>
                <telerik:RadDatePicker DateInput-LabelWidth="30%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="300" DateInput-Label="Alla data" ID="dtpDateTo" runat="server" />
                <asp:RequiredFieldValidator runat="server" Display="Dynamic" ControlToValidate="dtpDateTo" ErrorMessage="Il campo data è obbligatorio"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="label labelPanel" style="width: 20%;">Utente:
            </td>
            <td style="width: 80%;">
                <telerik:RadTextBox Width="30%" ID="txtUsername" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel" style="width: 20%;">Tipologia di documento:
            </td>
            <td style="width: 30%;">
                <telerik:RadComboBox runat="server" ID="cmbDocumentType" DataTextField="Text" DataValueField="Value" Width="30%" AutoPostBack="false" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel" style="width: 20%">Contenitore:
            </td>
            <td style="width: 30%">
                <telerik:RadComboBox runat="server" ID="cmbContainer" DataTextField="Text" DataValueField="Value" Width="50%" AutoPostBack="false" EmptyMessage="" Filter="Contains" MarkFirstMatch="true" EnableLoadOnDemand="true"  ShowMoreResultsBox="true" EnableVirtualScrolling="false" AllowCustomText="false" Height="300" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel" style="width: 20%">Settore:
            </td>
            <td>
                <usc:Settori HeaderVisible="false" ID="uscOwnerRole" MultipleRoles="false" MultiSelect="false" UseSessionStorage="true" Required="False" runat="server" ShowActive="True" />
            </td>
        </tr>
    </table>
    <div style="margin: 1px 1px 10px 1px;">
        <div>
            <telerik:RadButton ID="btnSearch" Text="Aggiorna visualizzazione" Width="150px" runat="server" TabIndex="1" AutoPostBack="False" />
            <telerik:RadButton ID="btnClean" Text="Azzera filtri" Width="150px" runat="server" TabIndex="1" AutoPostBack="False" />
            <telerik:RadButton ID="cmdExportToExcel" runat="server" Text="Esporta" ToolTip="Esporta in Excel" CausesValidation="false" />
            <input type="hidden" id="transparentAdministrationMonitorLogItems" name="TransparentAdministrationMonitorlogItems" value='<%= TransparentAdministrationMonitorLogItems %>' />
        </div>
    </div>
</asp:Content>

<asp:Content ID="content" runat="server" ContentPlaceHolderID="cphContent">

    <telerik:RadPageLayout runat="server" ID="pageContent" Height="100%" HtmlTag="Div">
        <Rows>
            <telerik:LayoutRow Height="100%" HtmlTag="Div">
                <Content>
                    <uc:uscAmmTraspMonitorLogGrid runat="server" ID="uscAmmTraspMonitorLogGrid" />
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>

    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>
