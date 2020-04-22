<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscCompleteWorkflow.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscCompleteWorkflow" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">
        var uscCompleteWorkflow;
        require(["UserControl/uscCompleteWorkflow"], function (UscCompleteWorkflow) {
            $(function () {
                uscCompleteWorkflow = new UscCompleteWorkflow(tenantModelConfiguration.serviceConfiguration);
                uscCompleteWorkflow.workflowActivityId = "<%=WorkflowActivityId.ToString()%>";
                uscCompleteWorkflow.rblActivityStatusId = "<%=rblActivityStatus.ClientID%>";
                uscCompleteWorkflow.btnConfirmId = "<%=btnConfirm.ClientID%>";
                uscCompleteWorkflow.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscCompleteWorkflow.contentId = "<%= pnlCompleteWorkflow.ClientID %>";
                uscCompleteWorkflow.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                uscCompleteWorkflow.currentUser = <%= CurrentUser %>;
                uscCompleteWorkflow.txtWfId = "<%=txtWf.ClientID%>";
                uscCompleteWorkflow.pnlWorkflowNoteId = "<%=pnlWorkflowNote.ClientID%>";
                uscCompleteWorkflow.ctrlTxtWfId = "<%= ctrlTxtWf.ClientId%>"
                uscCompleteWorkflow.lblMotivationId = "<%= lblMotivation.ClientId%>"
                uscCompleteWorkflow.initialize();
            });
        });

    </script>
</telerik:RadScriptBlock>
<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

<asp:Panel runat="server" ID="pnlCompleteWorkflow" Height="100%">
    <table runat="server" class="datatable">
        <tr>
            <td class="label col-dsw-2">
                <telerik:RadLabel runat="server" CssClass="label">Stato attività</telerik:RadLabel>
            </td>
            <td id="rowActivityStatus" class="col-dsw-8">
                <asp:RadioButtonList runat="server" ID="rblActivityStatus">
                    <asp:ListItem Text="Da lavorare" Value="1"></asp:ListItem>
                    <asp:ListItem Text="In carico" Value="2"></asp:ListItem>
                    <asp:ListItem Text="Rifiutata" Value="4"></asp:ListItem>
                    <asp:ListItem Text="Completata" Value="8"></asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>        
    </table>

    <telerik:RadPageLayout runat="server" CssClass="datatable">
        <Rows>    
            <telerik:LayoutRow Style="margin-bottom: 2px; display:none" ID="pnlWorkflowNote" runat="server">
                <Columns>
                    <telerik:LayoutColumn Span="2" CssClass="label col-dsw-2">
                        <asp:label runat="server" CssClass="label" ID="lblMotivation"></asp:label>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="10" CssClass="col-dsw-10" Style="margin-bottom: 2px;">
                        <telerik:RadTextBox ID="txtWf" TextMode="MultiLine" onBlur="javascript:ChangeStrWithValidCharacter(this);" Rows="3" runat="server" Width="100%" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtWf" ErrorMessage="E' obbligatorio inserire la descrizione per completare l'attivià" Display="Dynamic" ID="ctrlTxtWf"></asp:RequiredFieldValidator>                    
                    </telerik:LayoutColumn>                    
                </Columns>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>

    <telerik:RadAjaxPanel runat="server" CssClass="window-footer-wrapper">
        <telerik:RadButton ID="btnConfirm" Text="Conferma" runat="server" TabIndex="1" />
    </telerik:RadAjaxPanel>
</asp:Panel>
