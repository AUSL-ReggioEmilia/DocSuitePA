<%@ Page Title="Gestisci attività" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="WorkflowActivityManage.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.WorkflowActivityManage" %>

<%@ Import Namespace="VecompSoftware.DocSuiteWeb.Gui" %>
<%@ Register Src="../UserControl/uscFascicleSearch.ascx" TagName="uscFascicleSearch" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/uscCategoryRest.ascx" TagName="uscCategoryRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock" EnableViewState="false">
        <script type="text/javascript">
            var workflowActivityManage;
            require(["Workflows/WorkflowActivityManage"], function (WorkflowActivityManage) {
                $(function () {
                    workflowActivityManage = new WorkflowActivityManage(tenantModelConfiguration.serviceConfiguration);
                    workflowActivityManage.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    workflowActivityManage.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID  %>";
                    workflowActivityManage.currentUser = <%= CurrentUser %>;
                    workflowActivityManage.currentPageId = "<%= mainPanel.ClientID %>";
                    workflowActivityManage.uniqueId = "<%=CurrentWorkflowActivityId %>";
                    workflowActivityManage.lblProponenteId = "<%=lblProponente.ClientID %>";
                    workflowActivityManage.lblDestinatarioId = "<%=lblDestinatario.ClientID %>";
                    workflowActivityManage.grdUDId = "<%= grdUD.ClientID %>";
                    workflowActivityManage.lblRegistrationDateId = "<%= lblRegistrationDate.ClientID %>";
                    workflowActivityManage.lblSubjectId = "<%= lblSubject.ClientID %>";
                    workflowActivityManage.ddlUDSArchivesId = "<%= ddlUDSArchives.ClientID %>";
                    workflowActivityManage.rfvUDSArchivesId = "<%= rfvUDSArchives.ClientID %>";
                    workflowActivityManage.searchFasciclesId = "<%= searchFascicles.ClientID %>";
                    workflowActivityManage.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    workflowActivityManage.maxNumberElements = "<%= ProtocolEnv.MaxNumberDropdownElements %>";
                    workflowActivityManage.rblDocumentUnitId = "<%= rblDocumentUnit.ClientID %>";
                    workflowActivityManage.pnlUDSID = "<%= pnlUDS.ClientID %> ";
                    workflowActivityManage.pnlFascicleSelectId = "<%= pnlFascicleSelect.ClientID %>";
                    workflowActivityManage.uscFascicleSearchId = "<%= uscFascicleSearch.PageControl.ClientID %>";
                    workflowActivityManage.btnConfirmId = "<%= btnConfirm.ClientID %>";
                    workflowActivityManage.rblDocumentUnitUniqueId = "<%= rblDocumentUnit.UniqueID %>";
                    workflowActivityManage.ddlUDSArchivesUniqueId = "<%= ddlUDSArchives.UniqueId %>";

                    workflowActivityManage.panelDocumentUnitSelectId = "<%= panelDocumentUnitSelect.ClientID %>";
                    workflowActivityManage.panelManageId = "<%= panelManage.ClientID %>";

                    workflowActivityManage.initialize();

                    new ResizeSensor($(".details-template")[0], function () {
                        var height = $(".details-template").height() - 4;
                        $("#<%= pnlDocumentUnitSelect.ClientID %>").height(height);
                    });
                });
            });

            function showLoadingPanel(sender, args) {
                currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                currentUpdatedControl = "<%=mainPanel.ClientID %>";
                currentLoadingPanel.show(currentUpdatedControl);
            }

            function onFascicleMiscellaneaClick(sender, args) {
                if (workflowActivityManage.hasSelectedFascicle()) {
                    showLoadingPanel();
                    workflowActivityManage.cmdFascMiscellaneaInsert_Click();
                } else {
                    args.set_cancel(true);
                    alert("Nessun fascicolo selezionato");
                }
                return false;
            }

            function onArchiveClick(sender, args) {
                if (Page_ClientValidate('Attach')) {
                    showLoadingPanel(sender, args);
                    return true;
                }
                return false;
            }
        </script>
        <script type="text/javascript" language="javascript">
            function toggleSelection(source) {
                var isChecked = source.checked;
                $("[name*=grMaindocument]").each(function (index) {
                    $(this).attr('checked', false);
                });
                source.checked = isChecked;
            }

        </script>
    </telerik:RadScriptBlock>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">

    <telerik:RadWindowManager EnableViewState="False" ID="manager" runat="server">
        <Windows>
            <telerik:RadWindow Height="600" ID="searchFascicles" runat="server" Title="Ricerca fascicolo" Width="750" />
        </Windows>
    </telerik:RadWindowManager>
    <asp:Panel runat="server" ID="mainPanel">
        <asp:Panel runat="server" ID="mainContainer">
            <telerik:RadPageLayout runat="server" ID="applicantContainer" Width="100%" HtmlTag="Div">
                <Rows>
                    <telerik:LayoutRow ID="applicant">
                        <Content>
                            <div class="dsw-panel">
                                <div class="dsw-panel-title">
                                    Richiedente
                                </div>
                                <div class="dsw-panel-content">
                                    <telerik:RadPageLayout runat="server" HtmlTag="Div">
                                        <Rows>
                                            <telerik:LayoutRow HtmlTag="Div">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                        <b>Da:</b>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="6" CssClass="t-col-left-padding">
                                                        <asp:Label ID="lblProponente" runat="server"></asp:Label>
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                        </Rows>
                                        <Rows>
                                            <telerik:LayoutRow HtmlTag="Div">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                        <b>A:</b>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="6" CssClass="t-col-left-padding">
                                                        <asp:Label ID="lblDestinatario" runat="server"></asp:Label>
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                        </Rows>
                                        <Rows>
                                            <telerik:LayoutRow HtmlTag="Div">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                        <b>Data:</b>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="6" CssClass="t-col-left-padding">
                                                        <asp:Label ID="lblRegistrationDate" runat="server"></asp:Label>
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                        </Rows>
                                        <Rows>
                                            <telerik:LayoutRow HtmlTag="Div">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                        <b>Oggetto:</b>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="6" CssClass="t-col-left-padding">
                                                        <asp:Label ID="lblSubject" runat="server"></asp:Label>
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
                <Rows>
                    <telerik:LayoutRow ID="managein">
                        <Content>
                            <telerik:RadPageLayout runat="server">
                                <Rows>
                                    <telerik:LayoutRow HtmlTag="Div" Height="100%">
                                        <Columns>
                                            <telerik:LayoutColumn Span="4" CssClass="t-col-left-padding t-col-right-padding" ID="panelDocumentUnitSelect" runat="server">
                                                <asp:Panel runat="server" ID="pnlDocumentUnitSelect">
                                                    <div class="dsw-panel" style="height: 100px;">
                                                        <div class="dsw-panel-title">
                                                            Gestisci in
                                                        </div>
                                                        <div class="dsw-panel-content" style="margin-left:40px">
                                                            <asp:RadioButtonList ID="rblDocumentUnit" runat="server" AutoPostBack="false">
                                                                <asp:ListItem Text="Collaborazione" Value="Collaborazione" Selected="True"> </asp:ListItem>
                                                                <asp:ListItem Text="Protocollo" Value="Protocollo"> </asp:ListItem>
                                                                <asp:ListItem Text="Fascicolo" Value="Fascicolo"> </asp:ListItem>
                                                                <asp:ListItem Text="Archivi" Value="Archivi"> </asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </div>
                                                    </div>
                                                </asp:Panel>
                                            </telerik:LayoutColumn>
                                            <telerik:LayoutColumn Span="8" CssClass="  t-col-left-padding t-col-right-padding" ID="panelManage" runat="server">
                                                <asp:Panel runat="server" ID="pnlUDS" Style="display: none;">
                                                    <div class="dsw-panel" style="height: 100px;">
                                                        <div class="dsw-panel-title">
                                                            Archivio
                                                        </div>
                                                        <div class="dsw-panel-content" style="padding-left: 10%; padding-top: 10px;">
                                                            <telerik:RadComboBox runat="server" ID="ddlUDSArchives" DataTextField="Name" DataValueField="Id" AutoPostBack="false" Filter="Contains" CausesValidation="false" Height="200px" Width="300" />
                                                            <asp:RequiredFieldValidator runat="server" ID="rfvUDSArchives" ErrorMessage="Selezionare un archivio" ValidationGroup="Attach" ControlToValidate="ddlUDSArchives"></asp:RequiredFieldValidator>
                                                        </div>
                                                    </div>
                                                </asp:Panel>

                                                <asp:Panel runat="server" ID="pnlFascicleSelect" Style="display: none;">
                                                    <uc1:uscFascicleSearch runat="server" ID="uscFascicleSearch" MinHeight="100px"></uc1:uscFascicleSearch>
                                                </asp:Panel>
                                            </telerik:LayoutColumn>
                                        </Columns>
                                    </telerik:LayoutRow>
                                </Rows>
                            </telerik:RadPageLayout>
                        </Content>
                    </telerik:LayoutRow>
                </Rows>
            </telerik:RadPageLayout>
        </asp:Panel>

        <asp:Panel runat="server" ID="panelPEC">

            <telerik:RadGrid AllowSorting="False" Skin="Office2010Blue" AllowPaging="False" AllowMultiRowSelection="true"
                AutoGenerateColumns="False" GridLines="none" ID="grdUD" runat="server">
                <GroupingSettings ShowUnGroupButton="true"></GroupingSettings>
                <MasterTableView EnableGroupsExpandAll="True" NoDetailRecordsText="Nessuna documenti presente"
                    NoMasterRecordsText="Nessun documento presente" GroupLoadMode="Client" DataKeyNames="MainDocumentName">
                    <Columns>
                        <telerik:GridBoundColumn DataField="UniqueId" ReadOnly="True" UniqueName="UniqueId" Display="False">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="DocumentUnitName" ReadOnly="True" UniqueName="DocumentUnitName" AllowFiltering="false" Display="False"></telerik:GridBoundColumn>
                        <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn" HeaderStyle-Width="16px">
                        </telerik:GridClientSelectColumn>
                        <telerik:GridTemplateColumn UniqueName="RadioButtonTemplateColumn" HeaderStyle-Width="16px" HeaderStyle-HorizontalAlign="Center" HeaderImageUrl="../Comm/Images/PriorityHigh16.gif" HeaderTooltip="Documento Principale">
                            <ItemTemplate>
                                <asp:RadioButton runat="server" ID="rbtMainDocument" AutoPostBack="false" GroupName="grMaindocument" onclick="toggleSelection(this);" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn UniqueName="DocumentExtensionImage" HeaderStyle-Width="16px" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/documentPreview.png" HeaderText="Tipo documento">
                            <ItemTemplate>
                                <asp:Image ID="ImageFile" runat="server" ImageUrl="#" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn AllowFiltering="false" ItemStyle-HorizontalAlign="left" AutoPostBackOnFilter="false"
                            UniqueName="Title" HeaderText="Documenti" HeaderStyle-Width="300px">
                            <ItemTemplate>
                                <asp:Label ID="lblFileName" runat="server"></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn AllowFiltering="false" ItemStyle-HorizontalAlign="center" AutoPostBackOnFilter="false"
                            UniqueName="UDRegistrationDate" HeaderText="Data Registrazione" HeaderStyle-Width="200px">
                            <ItemTemplate>
                                <asp:Label ID="lblUDRegistrationDate" runat="server"></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>

                        <telerik:GridTemplateColumn AllowFiltering="false" AutoPostBackOnFilter="false">
                            <ItemTemplate>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>

                        <telerik:GridTemplateColumn AllowFiltering="false" AutoPostBackOnFilter="false" Visible="false" UniqueName="UDObject" HeaderText="Oggetto/Note">
                            <ItemTemplate>
                                <asp:Label ID="lblUDObject" runat="server"></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
                <ClientSettings AllowGroupExpandCollapse="true" Selecting-AllowRowSelect="true">
                </ClientSettings>
                <GroupingSettings ShowUnGroupButton="true"></GroupingSettings>
            </telerik:RadGrid>
            <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <telerik:RadButton Text="Conferma" runat="server" ID="btnConfirm" AutoPostBack="true" Width="150px" CausesValidation="false" OnClientClicking="showLoadingPanel" ValidationGroup="Attach"
        SingleClick="true" SingleClickText="Attendere...">
    </telerik:RadButton>
</asp:Content>
