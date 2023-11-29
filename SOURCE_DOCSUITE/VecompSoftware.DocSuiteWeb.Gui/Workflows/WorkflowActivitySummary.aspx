<%@ Page Title="Attività" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="WorkflowActivitySummary.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.WorkflowActivitySummary" %>

<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="ContattiSel" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscUploadDocumentRest.ascx" TagName="uscUploadDocumentRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock" EnableViewState="false">
        <script type="text/javascript">
            var workflowActivitySummary;

            require(["Workflows/WorkflowActivitySummary"], function (WorkflowActivitySummary) {
                $(function () {
                    workflowActivitySummary = new WorkflowActivitySummary(tenantModelConfiguration.serviceConfiguration);

                    workflowActivitySummary.ddlNameWorkflowId = "<%= ddlNameWorkflow.ClientID%>";
                    workflowActivitySummary.tblFilterStateId = "<%= tblFilterState.ClientID%>";

                    workflowActivitySummary.uscProponenteId = "<%= uscProponente.ClientID%>";
                    workflowActivitySummary.uscDestinatariId = "<%= uscDestinatari.ClientID%>";

                    workflowActivitySummary.tdpDateId = "<%= rtdDate.ClientID%>";
                    workflowActivitySummary.rtbNoteId = "<%= txtNote.ClientID%>";
                    workflowActivitySummary.rtbParereId = "<%= txtParere.ClientID%>";
                    workflowActivitySummary.lblActivityDateId = "<%= lblActivityDate.ClientID%>";

                    workflowActivitySummary.treeProponenteId = "<%= uscProponente.TreeViewContact.ClientID %>"
                    workflowActivitySummary.treeDestinatariId = "<%= uscDestinatari.TreeViewContact.ClientID %>"
                    workflowActivitySummary.logTreeId = "<%= logTree.ClientID%>";

                    workflowActivitySummary.cmdDocumentsId = "<%= cmdDocuments.ClientID%>";
                    workflowActivitySummary.cmdManageActivityId = "<%= cmdManageActivity.ClientID%>";
                    workflowActivitySummary.cmdRefuseId = "<%=cmdRefuse.ClientID%>";
                    workflowActivitySummary.cmdApproveId = "<%=cmdApprove.ClientID%>";
                    workflowActivitySummary.cmdSignId = "<%=cmdSign.ClientID%>";
                    workflowActivitySummary.cmdCompleteActivityId = "<%=cmdCompleteActivity.ClientID%>";
                    workflowActivitySummary.mainContainerId = "<%= mainContainer.ClientID%>";

                    workflowActivitySummary.ajaxManagerId = "<%= MasterDocSuite.AjaxManager.ClientID %>";
                    workflowActivitySummary.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    workflowActivitySummary.currentUser = <%= CurrentUser %>;
                    workflowActivitySummary.activityDateId = "<%=activityDate.ClientID%>";
                    workflowActivitySummary.tlrDateId = "<%=tlrDate.ClientID%>";
                    workflowActivitySummary.currentUser = <%= CurrentUser %>;
                    workflowActivitySummary.managerWindowsId = "<%= manager.ClientID %>";
                    workflowActivitySummary.uscUploadDocumentiId = "<%=uscUploadDocumenti.ClientID%>";
                    workflowActivitySummary.uscAddDocumentsRestId = "<%=addDocumentsRest.uploadDocumentComponent.ClientID%>";
                    workflowActivitySummary.addDocumentsSection = document.getElementById("addDocuments");
                    workflowActivitySummary.bindDocument = "<%=uscUploadDocumenti.uploadDocumentComponent.ClientID%>";
                    workflowActivitySummary.documentSection = document.getElementById("docUpload");
                    workflowActivitySummary.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";

                    workflowActivitySummary.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>

    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

    <telerik:RadPageLayout runat="server" ID="attivitaContainer" Width="100%" HtmlTag="Div">
        <Rows>
            <telerik:LayoutRow HtmlTag="Div">
                <Content>
                    <div class="dsw-panel">
                        <div class="dsw-panel-content">
                            <telerik:RadPageLayout runat="server" HtmlTag="Div" Width="100%">
                                <Rows>
                                    <telerik:LayoutRow HtmlTag="Div">
                                        <Columns>
                                            <telerik:LayoutColumn Span="3" CssClass="dsw-text-right" Style="margin-top: 5px;">
                                                <b>Nome attività:</b>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="3" CssClass="t-col-left-padding" Style="margin-top: 5px;">
                                                <telerik:RadComboBox ID="ddlNameWorkflow" Enabled="false" runat="server" Width="350px" />
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>

                                    <telerik:LayoutRow HtmlTag="Div">
                                        <Columns>
                                            <telerik:LayoutColumn Span="3" CssClass="dsw-text-right" Style="margin-top: 5px;">
                                                <b>Priorità:</b>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="8" CssClass="t-col-left-padding" Style="margin-top: 5px;">
                                                <asp:RadioButtonList Enabled="false" ID="tblFilterState" runat="server" RepeatDirection="Horizontal">
                                                    <asp:ListItem Text="Normale" Value="1" />
                                                    <asp:ListItem Text="Bassa" Value="2" />
                                                    <asp:ListItem Text="Alta" Value="3" />
                                                </asp:RadioButtonList>
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>
                                </Rows>
                            </telerik:RadPageLayout>
                        </div>
                    </div>
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>

    <telerik:RadWindowManager EnableViewState="False" ID="manager" runat="server">
        <Windows>
            <telerik:RadWindow Height="300" ID="singleSign" runat="server" Title="Firma documento" Width="750" />
        </Windows>
    </telerik:RadWindowManager>
</asp:Content>


<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel runat="server" ID="mainContainer">
        <telerik:RadPageLayout runat="server" ID="applicantContainer" Width="100%" HtmlTag="Div">
            <Rows>
                <telerik:LayoutRow ID="applicant">
                    <Content>
                        <div class="dsw-panel">
                            <div class="dsw-panel-title">
                                Mittente
                            </div>
                            <div class="dsw-panel-content">
                                <telerik:RadPageLayout runat="server" HtmlTag="Div">
                                    <Rows>
                                        <telerik:LayoutRow HtmlTag="Div">
                                            <Columns>
                                                <telerik:LayoutColumn Span="3" CssClass="dsw-text-right" Style="margin-top: 5px;">
                                                    <b>Mittenti:</b>
                                                </telerik:LayoutColumn>
                                                <telerik:LayoutColumn Span="6" CssClass="t-col-left-padding" Style="margin-top: 5px;">
                                                    <usc:ContattiSel IsEnable="false" ButtonSelectDomainVisible="false" ButtonDeleteVisible="false" ButtonImportVisible="false" ButtonIPAVisible="false" ButtonManualVisible="false" ButtonPropertiesVisible="false" ButtonRoleVisible="false" ButtonSelectOChartVisible="false" ButtonSelectVisible="false" EnableCheck="False" EnableViewState="true" HeaderVisible="false" ID="uscProponente" IsRequired="true" Multiple="true" runat="server" TreeViewCaption="Mittenti" UseAD="true" />
                                                </telerik:LayoutColumn>
                                            </Columns>
                                        </telerik:LayoutRow>
                                    </Rows>
                                </telerik:RadPageLayout>
                            </div>
                        </div>
                    </Content>
                </telerik:LayoutRow>
            </Rows>
        </telerik:RadPageLayout>
        <telerik:RadPageLayout runat="server" ID="receiverContainer" Width="100%" HtmlTag="Div">
            <Rows>
                <telerik:LayoutRow ID="receiver">
                    <Content>
                        <div class="dsw-panel">
                            <div class="dsw-panel-title">
                                Destinatario
                            </div>
                            <div class="dsw-panel-content">
                                <telerik:RadPageLayout runat="server" HtmlTag="Div">
                                    <Rows>
                                        <telerik:LayoutRow HtmlTag="Div">
                                            <Columns>
                                                <telerik:LayoutColumn Span="3" CssClass="dsw-text-right" Style="margin-top: 5px;">
                                                    <b>Destinatari:</b>
                                                </telerik:LayoutColumn>
                                                <telerik:LayoutColumn Span="6" CssClass="t-col-left-padding" Style="margin-top: 5px;">
                                                    <usc:ContattiSel IsEnable="false" ButtonSelectDomainVisible="false" ButtonDeleteVisible="false" ButtonImportVisible="false" ButtonIPAVisible="false" ButtonManualVisible="false" ButtonPropertiesVisible="false" ButtonRoleVisible="false" ButtonSelectOChartVisible="false" ButtonSelectVisible="false" EnableCheck="False" EnableViewState="true" HeaderVisible="false" ID="uscDestinatari" IsRequired="true" Multiple="true" runat="server" TreeViewCaption="Destinatari" UseAD="true" />
                                                </telerik:LayoutColumn>
                                            </Columns>
                                        </telerik:LayoutRow>
                                    </Rows>
                                </telerik:RadPageLayout>
                            </div>
                        </div>
                    </Content>
                </telerik:LayoutRow>
            </Rows>
        </telerik:RadPageLayout>

        <div id="docUpload" style="display:none">
            <telerik:RadPageLayout runat="server" ID="documentUpload" Width="100%" HtmlTag="Div">
                <Rows>
                    <telerik:LayoutRow ID="doc">
                        <Content>
                            <div class="dsw-panel">
                                <div class="dsw-panel-title">
                                    Documento
                                </div>
                                <div class="dsw-panel-content">
                                    <telerik:RadPageLayout runat="server" HtmlTag="Div">
                                        <Rows>
                                            <telerik:LayoutRow HtmlTag="Div">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right" Style="margin-top: 5px;">
                                                        <b>Documento:</b>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="6" CssClass="t-col-left-padding" Style="margin-top: 5px;">
                                                        <usc:uscUploadDocumentRest ID="uscUploadDocumenti" runat="server" MultipleUploadEnabled="false" />
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                        </Rows>
                                    </telerik:RadPageLayout>
                                </div>
                            </div>
                        </Content>
                    </telerik:LayoutRow>
                </Rows>
            </telerik:RadPageLayout>
        </div>

        <div id="addDocuments">
            <telerik:RadPageLayout runat="server" Width="100%" HtmlTag="Div">
                <Rows>
                    <telerik:LayoutRow>
                        <Content>
                            <div class="dsw-panel">
                                <div class="dsw-panel-title">
                                    Documenti
                                </div>
                                <div class="dsw-panel-content">
                                    <telerik:RadPageLayout runat="server" HtmlTag="Div">
                                        <Rows>
                                            <telerik:LayoutRow HtmlTag="Div">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right" Style="margin-top: 5px;">
                                                        <b>Documenti:</b>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="6" CssClass="t-col-left-padding" Style="margin-top: 5px;">
                                                        <usc:uscUploadDocumentRest ID="addDocumentsRest" runat="server" MultipleUploadEnabled="true" />
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                        </Rows>
                                    </telerik:RadPageLayout>
                                </div>
                            </div>
                        </Content>
                    </telerik:LayoutRow>
                </Rows>
            </telerik:RadPageLayout>
        </div>

        <telerik:RadPageLayout runat="server" ID="dateContainer" Width="100%" HtmlTag="Div">
            <Rows>
                <telerik:LayoutRow HtmlTag="Div">
                    <Content>
                        <div class="dsw-panel">
                            <div class="dsw-panel-title">
                                Dati
                            </div>
                            <div class="dsw-panel-content">
                                <telerik:RadPageLayout runat="server" HtmlTag="Div" Width="100%">
                                    <Rows>
                                        <telerik:LayoutRow HtmlTag="Div" ID="tlrDate">
                                            <Columns>
                                                <telerik:LayoutColumn Span="3" CssClass="dsw-text-right" Style="margin-top: 5px;">
                                                    <b>Data di scadenza:</b>
                                                </telerik:LayoutColumn>
                                                <telerik:LayoutColumn Span="2" CssClass="t-col-left-padding" Style="margin-top: 5px;">
                                                    <telerik:RadDatePicker Enabled="false" ID="rtdDate" runat="server" />
                                                </telerik:LayoutColumn>
                                            </Columns>
                                        </telerik:LayoutRow>

                                        <telerik:LayoutRow HtmlTag="Div">
                                            <Columns>
                                                <telerik:LayoutColumn Span="3" CssClass="dsw-text-right" Style="margin-top: 5px;">
                                                    <b>Note:</b>
                                                </telerik:LayoutColumn>
                                                <telerik:LayoutColumn Span="8" CssClass="t-col-left-padding" Style="margin-top: 5px;">
                                                    <telerik:RadTextBox ID="txtNote" ReadOnly="true" runat="server" TextMode="MultiLine" Rows="3" Width="100%" />
                                                </telerik:LayoutColumn>
                                            </Columns>
                                        </telerik:LayoutRow>

                                        <telerik:LayoutRow HtmlTag="Div" ID="activityDate">
                                            <Columns>
                                                <telerik:LayoutColumn Span="3" CssClass="dsw-text-right" Style="margin-top: 5px;">
                                                    <b>Risposta:</b><br />
                                                    <b>Ricevuta in data:
                                                        <asp:Label ID="lblActivityDate" runat="server" />
                                                    </b>
                                                </telerik:LayoutColumn>
                                                <telerik:LayoutColumn Span="8" CssClass="t-col-left-padding" Style="margin-top: 5px;">
                                                    <telerik:RadTextBox runat="server" ReadOnly="false" Placeholder="" TextMode="MultiLine" Rows="3" Width="100%" ID="txtParere" />
                                                </telerik:LayoutColumn>
                                            </Columns>
                                        </telerik:LayoutRow>

                                    </Rows>
                                </telerik:RadPageLayout>
                            </div>
                        </div>
                    </Content>
                </telerik:LayoutRow>
            </Rows>
        </telerik:RadPageLayout>

        <telerik:RadPageLayout runat="server" ID="logContainer" Width="100%" HtmlTag="Div">
            <Rows>
                <telerik:LayoutRow ID="logger">
                    <Content>
                        <div class="dsw-panel">
                            <div class="dsw-panel-title">
                                Collegamenti
                            </div>
                                <telerik:RadTreeView ID="logTree" runat="server" Width="100%" />         
                        </div>
                    </Content>
                </telerik:LayoutRow>
            </Rows>
        </telerik:RadPageLayout>
    </asp:Panel>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphFooter" ID="footerContent">
    <telerik:RadButton Text="Documenti" runat="server" ID="cmdDocuments" AutoPostBack="false" />
    <telerik:RadButton Text="Gestisci" runat="server" ID="cmdManageActivity" AutoPostBack="false" />
    <telerik:RadButton Text="Approva" runat="server" ID="cmdApprove" AutoPostBack="false" Style="display:none;"/>
    <telerik:RadButton Text="Rifiuta" runat="server" ID="cmdRefuse" AutoPostBack="false" Style="display:none;"/>
    <telerik:RadButton Text="Firma digitale" runat="server" ID="cmdSign" AutoPostBack="false" Style="display:none;"/>
    <telerik:RadButton Text="Completa attività" runat="server" ID="cmdCompleteActivity" AutoPostBack="false" Style="display:none;"/>
</asp:Content>
