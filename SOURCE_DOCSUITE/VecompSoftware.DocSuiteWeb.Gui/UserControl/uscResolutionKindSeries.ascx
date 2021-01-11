<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscResolutionKindSeries.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscResolutionKindSeries" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript">
        var uscResolutionKindSeries;
        require(["UserControl/UscResolutionKindSeries"], function (UscResolutionKindSeries) {
            $(function () {
                uscResolutionKindSeries = new UscResolutionKindSeries(tenantModelConfiguration.serviceConfiguration);
                uscResolutionKindSeries.pnlPageContentId = "<%= pnlPageContent.ClientID %>";
                uscResolutionKindSeries.wndResolutionKindDocumentSeriesId = "<%= wndResolutionKindDocumentSeries.ClientID %>";
                uscResolutionKindSeries.grdDocumentSeriesId = "<%= grdDocumentSeries.ClientID %>";
                uscResolutionKindSeries.btnAddSeriesId = "<%= btnAddSeries.ClientID %>";
                uscResolutionKindSeries.btnEditSeriesId = "<%= btnEditSeries.ClientID %>";
                uscResolutionKindSeries.btnCancelSeriesId = "<%= btnCancelSeries.ClientID %>";
                uscResolutionKindSeries.archivesDataSourceId = "<%= archivesDataSource.ClientID %>";
                uscResolutionKindSeries.rcbArchivesId = "<%= rcbArchives.ClientID %>";
                uscResolutionKindSeries.rcbConstraintsId = "<%= rcbConstraints.ClientID %>";
                uscResolutionKindSeries.pnlConstraintsSelectionId = "<%= pnlConstraintsSelection.ClientID %>";
                uscResolutionKindSeries.constraintsDataSourceId = "<%= constraintsDataSource.ClientID %>";
                uscResolutionKindSeries.btnConfirmSeriesId = "<%= btnConfirmSeries.ClientID %>";
                uscResolutionKindSeries.chbDocumentRequiredId = "<%= chbDocumentRequired.ClientID %>";
                uscResolutionKindSeries.managerWindowsId = "<%= wndManager.ClientID %>";
                uscResolutionKindSeries.defaultManagerWindowsId = "<%= BasePage.MasterDocSuite.DefaultWindowManager.ClientID %>";
                uscResolutionKindSeries.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscResolutionKindSeries.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                uscResolutionKindSeries.pnlWindowContentId = "<%= pnlWindowContent.ClientID %>";
                uscResolutionKindSeries.initialize();
            });
        });
    </script>
</telerik:RadCodeBlock>

<telerik:RadWindowManager ID="wndManager" runat="server">
    <Windows>
        <telerik:RadWindow ID="wndResolutionKindDocumentSeries" Behaviors="Close" Height="300px" Width="500px" runat="server">
            <ContentTemplate>
                <asp:Panel runat="server" ID="pnlWindowContent" CssClass="dsw-panel">
                    <div class="dsw-panel-content">
                        <p>
                            <b>Archivio:</b>
                        </p>
                        <asp:Panel runat="server">
                            <telerik:RadClientDataSource runat="server" ID="archivesDataSource"></telerik:RadClientDataSource>
                            <telerik:RadComboBox runat="server" ID="rcbArchives" Filter="Contains" AutoPostBack="false" Width="300px" ClientDataSourceID="archivesDataSource"
                                EnableLoadOnDemand="true" DataTextField="Name" DataValueField="EntityId" Height="300" Style="margin-bottom: 5px;">
                            </telerik:RadComboBox>                          
                        </asp:Panel>
                        <asp:Panel runat="server" ID="pnlConstraintsSelection" Style="margin-bottom: 5px;">
                            <p>
                                <b>Obbligo trasparenza di default:</b>
                            </p>
                            <telerik:RadClientDataSource runat="server" ID="constraintsDataSource"></telerik:RadClientDataSource>
                            <telerik:RadComboBox runat="server" AutoPostBack="false" Filter="Contains" Width="300px" ClientDataSourceID="constraintsDataSource"
                                ID="rcbConstraints" EnableLoadOnDemand="true" DataTextField="Name" DataValueField="UniqueId" Height="300">
                            </telerik:RadComboBox>
                        </asp:Panel>
                        <asp:CheckBox runat="server" ID="chbDocumentRequired" Text="Documento richiesto"></asp:CheckBox>
                        <div class="window-footer-wrapper">
                            <telerik:RadButton runat="server" ID="btnConfirmSeries" SingleClick="true" SingleClickText="Attendere..." AutoPostBack="false" Text="Conferma" />
                        </div>
                    </div>
                </asp:Panel>
            </ContentTemplate>
        </telerik:RadWindow>
    </Windows>
</telerik:RadWindowManager>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
<asp:Panel runat="server" ID="pnlPageContent">
    <telerik:RadGrid runat="server" ID="grdDocumentSeries" AutoGenerateColumns="False" Style="margin-top: 2px;" AllowMultiRowSelection="false" GridLines="Both" ItemStyle-BackColor="LightGray">
        <MasterTableView TableLayout="Auto" DataKeyNames="UniqueId" ClientDataKeyNames="UniqueId" AllowFilteringByColumn="false" GridLines="Both" Width="100%" NoMasterRecordsText="Nessun archivio definito">
            <Columns>
                <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn" HeaderStyle-Width="26px" ItemStyle-Width="26px">
                </telerik:GridClientSelectColumn>
                <telerik:GridTemplateColumn UniqueName="Name" HeaderText="Archivio" AllowFiltering="false" AllowSorting="true" Groupable="false">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                    <ClientItemTemplate>
                        <div class="dsw-text-left">      
                            <span>#=DocumentSeries.Name#</span>
                        </div>                                            
                    </ClientItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="DocumentRequired" HeaderText="Documento richiesto" AllowFiltering="false" AllowSorting="true" Groupable="false">
                    <HeaderStyle HorizontalAlign="Left" Width="16px" />
                    <ItemStyle HorizontalAlign="Left" />
                    <ClientItemTemplate>
                        <div class="dsw-text-center">      
                            # if(DocumentRequired){#
                                <img src="../App_Themes/DocSuite2008/imgset16/accept.png" title="Documento richiesto"></img>
                            #} else {#
                                <img src="../App_Themes/DocSuite2008/imgset16/cancel.png" title="Documento non richiesto"></img>
                            #}#
                        </div>                                            
                    </ClientItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="ConstraintValue" HeaderText="Obbligo trasparenza di default" AllowFiltering="false" AllowSorting="true" Groupable="false">
                    <HeaderStyle HorizontalAlign="Left" Width="40%" />
                    <ItemStyle HorizontalAlign="Left" />
                    <ClientItemTemplate>
                        # if(DocumentSeriesConstraint != null){#
                            <div class="dsw-text-left">      
                                <span>#=DocumentSeriesConstraint.Name#</span>
                            </div> 
                        #}#                                                                                                           
                    </ClientItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings>
            <Selecting AllowRowSelect="true" />
        </ClientSettings>
    </telerik:RadGrid>
    <div style="margin: 2px;">
        <telerik:RadButton runat="server" ID="btnAddSeries" AutoPostBack="false" SingleClick="true" SingleClickText="Attendere..." Text="Aggiungi"></telerik:RadButton>
        <telerik:RadButton runat="server" ID="btnEditSeries" AutoPostBack="false" SingleClick="true" SingleClickText="Attendere..." Text="Modifica"></telerik:RadButton>
        <telerik:RadButton runat="server" ID="btnCancelSeries" AutoPostBack="false" SingleClick="true" SingleClickText="Attendere..." Text="Elimina"></telerik:RadButton>
    </div>
</asp:Panel>
