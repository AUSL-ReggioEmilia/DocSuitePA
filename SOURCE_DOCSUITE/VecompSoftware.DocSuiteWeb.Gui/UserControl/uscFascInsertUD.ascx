<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscFascInsertUD.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscFascInsertUD" %>

<%@ Register Src="~/UserControl/uscClassificatore.ascx" TagName="uscClassificatore" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>


<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        var uscFascInsertUD;
        require(["UserControl/uscFascInsertUD"], function (UscFascInsertUD) {
            $(function () {
                uscFascInsertUD = new UscFascInsertUD(tenantModelConfiguration.serviceConfiguration);
                uscFascInsertUD.currentFascicleId = "<%= IdFascicle %>";
                uscFascInsertUD.panelFilterId = "<%= pnlAggiungi.ClientID %>";
                uscFascInsertUD.labelTitoloId = "<%= lblTitolo.ClientID %>";
                uscFascInsertUD.txtYearId = "<%= txtYear.ClientID %>";
                uscFascInsertUD.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                uscFascInsertUD.txtNumberId = "<%= txtNumber.ClientID %>";
                uscFascInsertUD.btnReferenceId = "<%= btnReference.ClientID %>";
                uscFascInsertUD.btnSearchId = "<%= btnSearch.ClientID %>";
                uscFascInsertUD.pageContentId = "<%= contentPane.ClientID %>";
                uscFascInsertUD.ajaxManagerId = "<%= BasePage.AjaxManager.ClientID %>";
                uscFascInsertUD.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscFascInsertUD.grdUdDocSelectedId = "<%= grdUdDocSelected.ClientID %>";
                uscFascInsertUD.rcbUDDocId = "<%= rcbUDDoc.ClientID %>";
                uscFascInsertUD.txtSubjectId = "<%= txtSubject.ClientID %>";
                uscFascInsertUD.treeCategoryId = "<%= uscCategoryFasc.TreeCategory.ClientID %>";
                uscFascInsertUD.rcbContainersId = "<%= rcbContainers.ClientID %>";
                uscFascInsertUD.maxNumberElements = "<%= ProtocolEnv.MaxNumberDropdownElements %>";
                uscFascInsertUD.chbCategoryChildId = "<%= chbCategoryChild.ClientID%>";
                uscFascInsertUD.radWindowManagerId = "<%= RadWindowManager.ClientID %>";
                uscFascInsertUD.idFascicleFolder = "<%= IdFascicleFolder %>";
                uscFascInsertUD.initialize();
            });
        });

        function onRequestStart(sender, args) {
            args.set_enableAjax(false);
        }

        function OnGridCommand(sender, args) {
            args.set_cancel(true);
            if (args.get_commandName() == "Page") {
                uscFascInsertUD.onPageChanged();
            }
        }

    </script>
</telerik:RadScriptBlock>

<telerik:RadWindowManager EnableViewState="False" ID="RadWindowManager" runat="server" />
<div class="splitterWrapper">
    <telerik:RadSplitter runat="server" ResizeWithParentPane="false" ID="splContent" Width="100%" Height="100%" Orientation="Horizontal">
        <telerik:RadPane runat="server" Height="100%" Scrolling="Y" ID="contentPane">
            <telerik:RadPageLayout runat="server" Width="100%" ID="pnlAggiungi" HtmlTag="Div" CssClass="dsw-panel">
                <Rows>
                    <telerik:LayoutRow HtmlTag="Div" CssClass="dsw-panel-title" Style="margin-bottom: 2px;">
                        <Columns>
                            <telerik:LayoutColumn Span="12" Style="margin-bottom: 10px;">
                                <asp:Label ID="lblTitolo" runat="server" Font-Bold="True"></asp:Label>
                            </telerik:LayoutColumn>
                        </Columns>
                    </telerik:LayoutRow>
                    <telerik:LayoutRow HtmlTag="Div" CssClass="dsw-panel-content">
                        <Rows>
                            <telerik:LayoutRow HtmlTag="Div" Style="margin-bottom: 2px;">
                                <Columns>
                                    <telerik:LayoutColumn Span="2" SpanMd="2" SpanLg="2" CssClass="dsw-vertical-middle">
                                        <b>Anno:</b>
                                    </telerik:LayoutColumn>
                                    <telerik:LayoutColumn Span="2" SpanMd="2" SpanLg="2" CssClass="t-col-left-padding">
                                        <telerik:RadNumericTextBox ID="txtYear" NumberFormat-DecimalDigits="0" NumberFormat-GroupSeparator="" IncrementSettings-InterceptArrowKeys="True" IncrementSettings-InterceptMouseWheel="True" Width="100%" MaxLength="4" runat="server" />
                                    </telerik:LayoutColumn>
                                    <telerik:LayoutColumn Span="2" SpanMd="1" SpanLg="1" CssClass="dsw-text-right dsw-vertical-middle">
                                        <b>Numero:</b>
                                    </telerik:LayoutColumn>
                                    <telerik:LayoutColumn Span="2" SpanMd="2" SpanLg="2" CssClass="t-col-left-padding">
                                        <telerik:RadTextBox ID="txtNumber" Width="100%" runat="server" />
                                    </telerik:LayoutColumn>
                                </Columns>
                            </telerik:LayoutRow>
                            <telerik:LayoutRow HtmlTag="Div">
                                <Columns>
                                    <telerik:LayoutColumn Span="2" SpanMd="2" SpanLg="2" Style="padding-top: 10px;">
                                        <b>Tipologia di documento:</b>
                                    </telerik:LayoutColumn>
                                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding" Style="margin-top: 3px;">
                                        <telerik:RadComboBox ID="rcbUDDoc" EmptyMessage="Seleziona una tipologia di documenti" CausesValidation="true"
                                            EnableVirtualScrolling="false" AutoPostBack="False" MaxHeight="250px"
                                            Style="margin: 5px 0px 5px 2px;" runat="server">
                                        </telerik:RadComboBox>
                                    </telerik:LayoutColumn>
                                </Columns>
                            </telerik:LayoutRow>
                            <telerik:LayoutRow HtmlTag="Div">
                                <Columns>
                                    <telerik:LayoutColumn Span="2" SpanMd="2" SpanLg="2" Style="padding-top: 10px;">
                                        <b>Contenitore:</b>
                                    </telerik:LayoutColumn>
                                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding" Style="margin-top: 3px;">
                                        <telerik:RadComboBox ID="rcbContainers" EmptyMessage="Seleziona un contenitore" AllowCustomText="false" Width="550px"
                                            EnableLoadOnDemand="true" EnableVirtualScrolling="false" ShowMoreResultsBox="true" AutoPostBack="false" MaxHeight="250px"
                                            Style="margin: 5px 0px 5px 2px;" runat="server">
                                        </telerik:RadComboBox>
                                    </telerik:LayoutColumn>
                                </Columns>
                            </telerik:LayoutRow>
                            <telerik:LayoutRow HtmlTag="Div" Style="margin-bottom: 2px;">
                                <Columns>
                                    <telerik:LayoutColumn Span="2" SpanMd="2" SpanLg="2" CssClass="dsw-vertical-middle">
                                        <b>Oggetto:</b>
                                    </telerik:LayoutColumn>
                                    <telerik:LayoutColumn Span="10" SpanMd="10" SpanLg="10" CssClass="t-col-right-padding t-col-left-padding dsw-vertical-middle">
                                        <telerik:RadTextBox runat="server" ID="txtSubject" Width="100%"></telerik:RadTextBox>
                                    </telerik:LayoutColumn>
                                </Columns>
                            </telerik:LayoutRow>
                            <telerik:LayoutRow HtmlTag="Div">
                                <Columns>
                                    <telerik:LayoutColumn Span="2" SpanMd="2" SpanLg="2" Style="padding-top: 10px;">
                                        <b>Classificatore:</b>
                                    </telerik:LayoutColumn>
                                    <telerik:LayoutColumn Span="10" SpanMd="10" SpanLg="10" CssClass="t-col-right-padding t-col-left-padding dsw-vertical-middle">
                                        <usc:uscClassificatore HeaderVisible="false" ID="uscCategoryFasc" Required="false" runat="server" />
                                        <asp:CheckBox ID="chbCategoryChild" runat="server" Text="Estendi ricerca alle sottocategorie" />
                                    </telerik:LayoutColumn>
                                </Columns>
                            </telerik:LayoutRow>
                            <telerik:LayoutRow HtmlTag="Div" Style="margin-bottom: 2px; margin-top: 5px;">
                                <Columns>
                                    <telerik:LayoutColumn Span="12">
                                        <telerik:RadButton ID="btnSearch" runat="server" Text="Cerca" />
                                    </telerik:LayoutColumn>
                                </Columns>
                            </telerik:LayoutRow>
                        </Rows>
                    </telerik:LayoutRow>
                </Rows>
            </telerik:RadPageLayout>            
            <telerik:RadPageLayout runat="server" HtmlTag="Div" ID="pageContent" Width="100%">
                <Rows>
                    <telerik:LayoutRow>
                        <Content>
                            <div class="dsw-panel" id="panelContent">
                                <div class="dsw-panel-title">
                                    Documenti trovati
                                </div>
                                <div class="dsw-panel-content">
                                    <telerik:RadPageLayout runat="server" HtmlTag="Div" Width="100%">
                                        <Rows>
                                            <telerik:LayoutRow HtmlTag="Div" Style="margin-bottom: 5px;">
                                                <Content>
                                                    <telerik:RadAjaxPanel ClientEvents-OnRequestStart="onRequestStart" runat="server" ID="RadAjaxPanel1">
                                                        <telerik:RadGrid runat="server" ID="grdUdDocSelected" GridLines="None" Skin="Office2010Blue" PageSize="10" AllowPaging="True" AllowMultiRowSelection="true" AllowSorting="false">
                                                            <ClientSettings>
                                                                <Resizing AllowColumnResize="false" />
                                                                <ClientEvents OnCommand="OnGridCommand" />
                                                            </ClientSettings>
                                                            <MasterTableView AutoGenerateColumns="false" ClientDataKeyNames="IdFascicle,IsFascicolable" NoMasterRecordsText="Nessun documento trovato." PagerStyle-PagerTextFormat="{4} Visualizzati {3} su {5}">
                                                                <Columns>
                                                                    <telerik:GridClientSelectColumn HeaderStyle-Width="30px" UniqueName="ClientSelectColumn" />
                                                                    <telerik:GridTemplateColumn HeaderStyle-Width="100px" UniqueName="colType" HeaderText="Documenti">
                                                                        <ClientItemTemplate>
                                                                <label>#= UDType #</label>
                                                                        </ClientItemTemplate>
                                                                    </telerik:GridTemplateColumn>
                                                                    <telerik:GridTemplateColumn HeaderStyle-Width="100px" UniqueName="colViewDocuments" HeaderText="Numero" AllowFiltering="false" Groupable="false">
                                                                        <ClientItemTemplate>                                                                
                                                                 <a href="#= UDLink #" target="_parent">#= UDTitle #</a>
                                                                        </ClientItemTemplate>
                                                                    </telerik:GridTemplateColumn>
                                                                    <telerik:GridBoundColumn DataField="RegistrationDate" UniqueName="RegistrationDate" HeaderText="Data registrazione" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="80px" />
                                                                    <telerik:GridTemplateColumn UniqueName="colSubject" HeaderStyle-Width="200px" HeaderText="Oggetto">
                                                                        <ClientItemTemplate>
                                                                <label>#= Object #</label>
                                                                        </ClientItemTemplate>
                                                                    </telerik:GridTemplateColumn>
                                                                    <telerik:GridTemplateColumn UniqueName="colCategory" HeaderText="Classificazione">
                                                                        <ClientItemTemplate>
                                                                <label>#= Category #</label>
                                                                        </ClientItemTemplate>
                                                                    </telerik:GridTemplateColumn>
                                                                    <telerik:GridTemplateColumn UniqueName="colContainer" HeaderText="Contenitore">
                                                                        <ClientItemTemplate>
                                                                <label>#= Container #</label>
                                                                        </ClientItemTemplate>
                                                                    </telerik:GridTemplateColumn>
                                                                </Columns>
                                                            </MasterTableView>
                                                            <ClientSettings Selecting-AllowRowSelect="true" />
                                                            <PagerStyle Mode="NextPrevAndNumeric" Position="Bottom" AlwaysVisible="true"></PagerStyle>
                                                        </telerik:RadGrid>
                                                    </telerik:RadAjaxPanel>
                                                </Content>
                                            </telerik:LayoutRow>
                                        </Rows>
                                    </telerik:RadPageLayout>
                                </div>
                            </div>
                        </Content>
                    </telerik:LayoutRow>
                </Rows>
            </telerik:RadPageLayout>
        </telerik:RadPane>
        <telerik:RadPane runat="server" Height="40px" Scrolling="None">
            <div style="border-top:1px solid #d9d9d9;">
                <div style="margin: 10px 3px;">
                    <telerik:RadButton Enabled="False" ID="btnReference" runat="server" CausesValidation="false" Text="Inserisci" />
                </div>
            </div>
        </telerik:RadPane>
    </telerik:RadSplitter>
</div>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>








