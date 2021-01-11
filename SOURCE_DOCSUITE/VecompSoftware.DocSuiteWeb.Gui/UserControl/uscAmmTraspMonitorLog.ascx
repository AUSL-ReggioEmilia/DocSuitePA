<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscAmmTraspMonitorLog.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscAmmTraspMonitorLog" %>

<%@ Register Src="~/UserControl/uscMonitoraggio.ascx" TagName="uscMonitoraggio" TagPrefix="ucsm" %>
<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="settori" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">
        var uscAmmTraspMonitorLog;
        require(["UserControl/uscAmmTraspMonitorLog"], function (UscAmmTraspMonitorLog) {
            $(function () {
                uscAmmTraspMonitorLog = new UscAmmTraspMonitorLog(tenantModelConfiguration.serviceConfiguration);
                uscAmmTraspMonitorLog.rwAmmTraspMonitorLogId = "<%= rwAmmTraspMonitorLog.ClientID %>";
                uscAmmTraspMonitorLog.pageContentId = "<%= pnlPageContent.ClientID %>";
                uscAmmTraspMonitorLog.uscAmmTraspMonitorLogUpdatePanelId = "<%= ammTraspMonitorLogUpdatePanel.ClientID %>";
                uscAmmTraspMonitorLog.txtAmmTraspMonitorLogNameId = "<%= txtAmmTraspMonitorLogName.ClientID %>";
                uscAmmTraspMonitorLog.dpAmmTraspMonitorLogDateId = "<%= dpAmmTraspMonitorLogDate.ClientID %>";
                uscAmmTraspMonitorLog.txtAmmTraspMonitorLogNoteId = "<%= txtAmmTraspMonitorLogNote.ClientID %>";
                uscAmmTraspMonitorLog.cmbAmmTraspMonitorLogRatingId = "<%= cmbAmmTraspMonitorLogRating.ClientID %>";
                uscAmmTraspMonitorLog.cmdAmmTraspMonitorLogSaveId = "<%= cmdAmmTraspMonitorLogSave.ClientID %>";
                uscAmmTraspMonitorLog.uscMonitoraggioContentId = "<%= uscMonitoraggio.FindControl("tblMonitoraggio").ClientID %>";
                uscAmmTraspMonitorLog.lblArchiveId = "<%= uscMonitoraggio.FindControl("lblArchive").ClientID %>";
                uscAmmTraspMonitorLog.lblCreatedById = "<%= uscMonitoraggio.FindControl("lblCreatedBy").ClientID %>";
                uscAmmTraspMonitorLog.lblMonitoringId = "<%= uscMonitoraggio.FindControl("lblMonitoring").ClientID %>";
                uscAmmTraspMonitorLog.currentDisplayName = "<%= CurrentDisplayName %>";
                uscAmmTraspMonitorLog.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                uscAmmTraspMonitorLog.txtAmmTraspMonitorLogDocumentUnitIdId = "<%= txtAmmTraspMonitorLogDocumentUnitId.ClientID %>";
                uscAmmTraspMonitorLog.txtAmmTraspMonitorLogDocumentUnitIdValue
                    = document.getElementById(uscAmmTraspMonitorLog.txtAmmTraspMonitorLogDocumentUnitIdId).value;
                uscAmmTraspMonitorLog.txtAmmTraspMonitorLogDocumentUnitNameId = "<%= txtAmmTraspMonitorLogDocumentUnitName.ClientID %>";
                uscAmmTraspMonitorLog.txtAmmTraspMonitorLogDocumentUnitNameValue
                    = document.getElementById(uscAmmTraspMonitorLog.txtAmmTraspMonitorLogDocumentUnitNameId).value;
                uscAmmTraspMonitorLog.ratingValues = JSON.parse('<%= MonitoringTransparentRatings %>');
                uscAmmTraspMonitorLog.uscOwnerRoleId = "<%= uscOwnerRole.TableContentControl.ClientID %>";
                uscAmmTraspMonitorLog.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID%>";
                uscAmmTraspMonitorLog.uscMonitoringEditButtonId = "<%= uscMonitoringEditButton.ClientID %>"
                uscAmmTraspMonitorLog.initialize();
            });
        });

        function onRequestStart(sender, args) {
            args.set_enableAjax(false);
        }
    </script>
</telerik:RadScriptBlock>

<telerik:RadWindow runat="server" ID="rwAmmTraspMonitorLog" Title="Aggiungi nuovo" Width="700" Height="400px" Behaviors="Close"
    IconUrl="~/App_Themes/DocSuite2008/imgset16/add.png">
    <ContentTemplate>        

        <usc:uscErrorNotification runat="server" ID="uscNotification" />

        <asp:UpdatePanel ID="ammTraspMonitorLogUpdatePanel" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <table id="ammTraspMonitorLogWindowTable" class="datatable monitorLog" style="height: 100%; margin-bottom: 0">
                    <tr>
                        <td>
                            <asp:Label ID="txtAmmTraspMonitorLogDocumentUnitId" runat="server" Width="100%" ReadOnly="true"
                                Visible="true" ClientIDMode="Static" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="txtAmmTraspMonitorLogDocumentUnitName" runat="server" Width="100%" ReadOnly="true"
                                Visible="true" ClientIDMode="Static" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">Utente corrente</td>
                        <td>
                            <telerik:RadTextBox ID="txtAmmTraspMonitorLogName" runat="server" Width="90%" ReadOnly="true" BackColor="#cccccc" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">Data e ora del monitoraggio</td>
                        <td>
                            <telerik:RadTextBox ID="dpAmmTraspMonitorLogDate" runat="server" Width="90%" ReadOnly="true" BackColor="#cccccc" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">Note</td>
                        <td>
                            <telerik:RadTextBox ID="txtAmmTraspMonitorLogNote" runat="server" Rows="5" Width="90%" TextMode="MultiLine" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">Valutazione</td>
                        <td>
                            <telerik:RadComboBox runat="server" ID="cmbAmmTraspMonitorLogRating" DataTextField="Text" DataValueField="Value" Width="90%" AutoPostBack="false" CheckBoxes="true" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label"></td>
                        <td>
                            <asp:RequiredFieldValidator runat="server" ID="cmbAmmTraspMonitorLogRatingValidator" ControlToValidate="cmbAmmTraspMonitorLogRating"
                                Display="None" ValidationGroup="vRating" ErrorMessage="Il campo Valutazione è obbligatorio" CssClass="validationClass"/>
                            <asp:ValidationSummary runat="server" ID="vSummary" ValidationGroup="vRating" DisplayMode="BulletList" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label"></td>
                        <td>
                            <asp:Panel runat="server" CssClass="content-wrapper" Style="width: 90%;">
                                <usc:settori Caption="Settore" ID="uscOwnerRole" RoleRestictions="None" MultipleRoles="False" MultiSelect="False" Required="False" runat="server" UseSessionStorage="true" />
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" class="buttons">
                            <telerik:RadButton ID="cmdAmmTraspMonitorLogSave" ValidationGroup="vRating" CausesValidation="true" runat="server" Text="Salva" Width="100px" AutoPostBack="false">
                            </telerik:RadButton>                            
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>

<asp:Panel runat="server" ID="pnlPageContent">
    <ucsm:uscMonitoraggio runat="server" ID="uscMonitoraggio" />
</asp:Panel>
