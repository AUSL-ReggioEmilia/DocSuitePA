<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UserCollRisultati.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UserCollRisultati" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscCollGrid.ascx" TagName="CollGrid" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">
            function pageLoad(sender, eventArgs) {
                if (!eventArgs.get_isPartialLoad()) {                    
                    ExecuteAjaxRequest("InitialPageLoad");
                }
                setSignButtonsVisibility();
            }

            function setSignButtonsVisibility() {
                if ("<%= HasDgrooveSigner %>" === "False") {
                    return;
                }

                if (document.getElementById("<%= btnDgrooveSigns.ClientID %>") === null ||
                    document.getElementById("<%= btnMultiSign.ClientID %>" === null) ) {
                    return;
                }

                var currentBrowser = getBrowserType();

                if (currentBrowser.startsWith("ie")) {
                    document.getElementById("<%= btnDgrooveSigns.ClientID %>").style.display = 'none';
                    document.getElementById("<%= btnMultiSign.ClientID %>").style.display = '';
                }
                else {
                    document.getElementById("<%= btnDgrooveSigns.ClientID %>").style.display = '';
                    document.getElementById("<%= btnMultiSign.ClientID %>").style.display = 'none';
                }
            }

            function resizeGrid() {
                $find("<%= uscCollaborationGrid.Grid.ClientID %>").repaint();
            }

            function SaveToSessionStorageAndRedirect(documents) {
                sessionStorage.setItem("DocsToSign", documents);
                window.location.href = "../Comm/DgrooveSigns.aspx";
            }

            function OpenWindowsChangeSigner(url) {
                return OpenWindow(url, '700', '380', OnChangeSigner);
            }

            function OpenAuthorizeWindow(url) {
                var documentType = $find("<%= ddlDocType.ClientID %>");
                if (documentType != null && documentType.get_selectedItem() != null) {
                    var documentTypeName = documentType.get_selectedItem().get_value();
                    if (documentTypeName != "W") {
                        url += "&Environment=" + documentTypeName;
                        return OpenWindow(url, '430', '430', Refresh);
                    } else {
                        alert("Non è possibile gestire collaborazioni di tipologia attività");
                        return false;
                    }
                }
                alert("Selezionare una tipologia di documento per l'autorizzazione");
                return false;
            }

            function OpenWindow(url, width, height, funcName) {
                var wnd = DSWOpenGenericWindow(url, WindowTypeEnum.NORMAL);
                wnd.set_visibleStatusbar(false);
                wnd.set_modal(true);
                wnd.set_showOnTopWhenMaximized(false);
                wnd.set_destroyOnClose(true);
                wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
                wnd.add_close(funcName);
                wnd.setSize(width, height);
                wnd.center();
                return false;
            }

            function OnChangeSigner(sender, args) {
                sender.remove_close(OnChangeSigner);
                if (args.get_argument() !== null) {
                    var manager = $find("<%= AjaxManager.ClientID%>");
                    manager.ajaxRequest('CHANGESIGNER' + '|' + args.get_argument());
                }
            }

            function ExecuteAjaxRequest(operationName) {
                var manager = $find("<%= AjaxManager.ClientID %>");
                manager.ajaxRequest(operationName);
            }

            function Refresh() {
                var manager = $find("<%= AjaxManager.ClientID %>");
                manager.ajaxRequest('UPDATE');
                return false;
            }

            function UpdateData() {
                ExecuteAjaxRequest('UpdateData');
            }

            function SignAndNext() {
                ExecuteAjaxRequest('SIGNNEXT');  
                ShowLoadingPanels();
            }

            function ShowLoadingPanels()
            {
                var ajaxFlatLoadingPanel = $find("<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>");
                var pnlButtons = "<%= buttons.ClientID%>";
                ajaxFlatLoadingPanel.show(pnlButtons);

                var pnlbody = "<%= pnlHeaderDiv.ClientID%>";
                ajaxFlatLoadingPanel.show(pnlbody);
            }

            function Sign() {
                ExecuteAjaxRequest('SIGN');
                ShowLoadingPanels();
            }

            function AutomaticNext() {
                ExecuteAjaxRequest('AUTOMATICNEXT');
                return false;
            }

            function ConfirmDeleteCollaboration(id) {
                function deleteConfirmCallbackFunction(arg) {
                    if (arg) {
                        var manager = $find("<%= AjaxManager.ClientID%>");
                        manager.ajaxRequest('DELETECOLLABORATION' + '|' + id);
                    }
                }
                radconfirm("<strong>Attenzione!</strong><p>La collaborazione selezionata non presenta documenti associati. Eliminare tale collaborazione?</p>", deleteConfirmCallbackFunction, 200, 100, null, "Eliminazione", "../Comm/Images/File/Remove32.gif");
            }

            function CloseWindow(args) {
                var oWindow = $find('<%= nextCollaborationWindow.ClientID%>');
                oWindow.close();
                if (args == "rebind") {
                    UpdateData();
                }
                return false;
            }

            function AutomaticNextCollaborationsWindow() {
                var oWindowCust = $find('<%= nextCollaborationWindow.ClientID%>');
                oWindowCust.show();
            }

            function CheckUoiaCollaborationTypes() {
                var items = getSelectedItems();
                var res = true;

                $.each(items, function (index, item) {
                    if (item.get_cell('Entity.DocumentType').innerHTML != "U") {
                        alert("Le collaborazioni selezionate devono essere tutte di tipo UOIA.");
                        res = false;
                        return false;
                    }
                });

                return res;
            }

            function getSelectedItems() {
                var tableView = $find("<%= uscCollaborationGrid.Grid.ClientID %>").get_masterTableView();
                var items = tableView.get_dataItems();
                var selectedItems = [];

                $.each(items, function (index, item) {
                    var cbSelect = item.findElement('cbSelect');
                    if (cbSelect != null && cbSelect.checked) {
                        selectedItems.push(item);
                    }
                });
                return selectedItems;
            }

            function OnAbsenseClose(sender, eventArgs) {
                sender.remove_close(OnAbsenseClose);
                if (eventArgs.get_argument() !== null) {
                    var manager = $find("<%= AjaxManager.ClientID%>");
                    manager.ajaxRequest('ABSENTMANAGERS' + '|' + eventArgs.get_argument());
                }
            }
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindow runat="server" ShowContentDuringLoad="False" ID="nextCollaborationWindow" Title="Prosegui automatico delle seguenti collaborazioni" Width="600" Height="450">
        <ContentTemplate>
            <telerik:RadGrid runat="server" ID="grdCollaborations" Width="100%" Height="325px" AllowMultiRowSelection="true">
                <MasterTableView AutoGenerateColumns="False">
                    <Columns>
                        <telerik:GridTemplateColumn HeaderStyle-Width="20%" UniqueName="Id" HeaderText="Id collaborazione">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="idCollaboration" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderStyle-Width="80%" UniqueName="Subject" HeaderText="Oggetto">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="collaborationObject"></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
                <ClientSettings EnableRowHoverStyle="False">
                    <Selecting AllowRowSelect="False" EnableDragToSelectRows="False" />
                </ClientSettings>
            </telerik:RadGrid>
            <div id="actionButtons">
                <asp:Button runat="server" ID="confirmNextAction" Width="150px" Text="Conferma e prosegui" OnClientClick="return AutomaticNext();" />
                <asp:Button runat="server" ID="exitNextAction" Text="Annulla" Width="120px" OnClientClick="return CloseWindow();" />
            </div>
        </ContentTemplate>
    </telerik:RadWindow>

     <div id="pnlHeaderDiv" runat="server">
    <telerik:RadPageLayout runat="server" HtmlTag="None" CssClass="col-dsw-10">
        <Rows>
            <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" ID="rowFilter" Visible="false" runat="server">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="Div" Span="3" CssClass="control-label">
                        <asp:Label ID="lblFilter" CssClass="label" runat="server" Text="Collaborazioni:" />
                    </telerik:LayoutColumn>
                    <telerik:CompositeLayoutColumn HtmlTag="Div" Span="9" CssClass="form-control" runat="server">
                        <Content>
                            <asp:Panel ID="filters" runat="server">
                                <telerik:RadButton ID="allCollaborations" runat="server" Checked="True" ToggleType="Radio" GroupName="FilterButton" ButtonType="LinkButton" AutoPostBack="True" Text="Tutte">
                                    <ToggleStates>
                                        <telerik:RadButtonToggleState PrimaryIconCssClass="rbToggleRadioChecked" />
                                        <telerik:RadButtonToggleState PrimaryIconCssClass="rbToggleRadio" />
                                    </ToggleStates>
                                </telerik:RadButton>
                                <telerik:RadButton ID="activeCollaborations" runat="server" ToggleType="Radio" Style="margin-left: 5px;" GroupName="FilterButton" ButtonType="LinkButton" AutoPostBack="True" Text="Attive">
                                    <ToggleStates>
                                        <telerik:RadButtonToggleState PrimaryIconCssClass="rbToggleRadioChecked" />
                                        <telerik:RadButtonToggleState PrimaryIconCssClass="rbToggleRadio" />
                                    </ToggleStates>
                                </telerik:RadButton>
                                <telerik:RadButton ID="pastCollaborations" runat="server" ToggleType="Radio" Style="margin-left: 5px;" GroupName="FilterButton" ButtonType="LinkButton" AutoPostBack="True" Text="Passate">
                                    <ToggleStates>
                                        <telerik:RadButtonToggleState PrimaryIconCssClass="rbToggleRadioChecked" />
                                        <telerik:RadButtonToggleState PrimaryIconCssClass="rbToggleRadio" />
                                    </ToggleStates>
                                </telerik:RadButton>
                                <telerik:RadButton ID="signRequired" runat="server" ToggleType="Radio" Style="margin-left: 5px;" GroupName="FilterButton" ButtonType="LinkButton" AutoPostBack="True" Text="Firma obbligatoria">
                                    <ToggleStates>
                                        <telerik:RadButtonToggleState PrimaryIconCssClass="rbToggleRadioChecked" />
                                        <telerik:RadButtonToggleState PrimaryIconCssClass="rbToggleRadio" />
                                    </ToggleStates>
                                </telerik:RadButton>
                                <telerik:RadButton ID="onlyVision" runat="server" ToggleType="Radio" Style="margin-left: 5px;" GroupName="FilterButton" ButtonType="LinkButton" AutoPostBack="True" Text="Presa visione">
                                    <ToggleStates>
                                        <telerik:RadButtonToggleState PrimaryIconCssClass="rbToggleRadioChecked" />
                                        <telerik:RadButtonToggleState PrimaryIconCssClass="rbToggleRadio" />
                                    </ToggleStates>
                                </telerik:RadButton>
                            </asp:Panel>
                        </Content>
                    </telerik:CompositeLayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="Div" Span="3" CssClass="control-label">
                        <asp:Label ID="lblDocType" runat="server" Text="Tipologia documento:" />
                    </telerik:LayoutColumn>
                    <telerik:CompositeLayoutColumn HtmlTag="Div" CssClass="form-control" Span="9">
                        <Content>
                            <telerik:RadDropDownList ID="ddlDocType" runat="server" AutoPostBack="True" Width="350px" DefaultMessage="Seleziona una tipologia..." DropDownHeight="200px" />
                        </Content>
                    </telerik:CompositeLayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" ID="rowDate" runat="server">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="Div" CssClass="control-label" Span="3">
                        <asp:Label ID="lblRegistrationDate" runat="server" Text="Data inserimento:" />
                    </telerik:LayoutColumn>
                    <telerik:CompositeLayoutColumn HtmlTag="Div" CssClass="form-control dsw-vertical-middle" Span="9">
                        <Content>
                            <telerik:RadDatePicker ID="rdpDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
                            <asp:CompareValidator ControlToValidate="rdpDateFrom" Display="Dynamic" ErrorMessage="Errore formato" ID="cfvDateFrom" Operator="DataTypeCheck" runat="server" Type="Date" />
                            <asp:RequiredFieldValidator ControlToValidate="rdpDateFrom" Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="rfvDateFrom" runat="server" />

                            <telerik:RadDatePicker ID="rdpDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important; margin-left: 5px;" Width="200" DateInput-Label="A" runat="server" />
                            <asp:CompareValidator ControlToValidate="rdpDateTo" Display="Dynamic" ErrorMessage="Errore formato" ID="cfvDateTo" Operator="DataTypeCheck" runat="server" Type="Date" />
                            <asp:RequiredFieldValidator ControlToValidate="rdpDateTo" Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="rfvDateTo" runat="server" />
                        </Content>
                    </telerik:CompositeLayoutColumn>
                </Columns>
            </telerik:LayoutRow>            
            <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" WrapperHtmlTag="None">
                <Content>
                    <asp:Button ID="btnUpdate" runat="server" Text="Aggiorna" />
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
                     </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <div style="height:100%">
        <usc:CollGrid ID="uscCollaborationGrid" runat="server" />
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <div class="dsw-col-10" id="buttons" runat="server">
        <asp:Button ID="btnInsert" runat="server" Text="Inserimento" Visible="false" Width="130px"/>
        <asp:Button ID="cmdPreviewDocuments" runat="server" Text="Documenti" Width="120px" Visible="false" />
        <asp:Button ID="cmdCollaborationVersioningManagement" runat="server" Text="Check In Multiplo" Width="120px" Visible="false" />
        <asp:Button ID="btnChangeSigner" runat="server" Text="Cambia Responsabile" Visible="false" Width="130px" />
        <asp:Button CausesValidation="False" ID="btnRoles" runat="server" Text="Autorizza" ToolTip="Modifica Autorizzazioni" Width="120px" />
        <asp:Button ID="btnMultiSign" runat="server" Text="Firma" OnClientClick="Sign();" Width="120px" />
        <asp:Button ID="btnDgrooveSigns" runat="server" Text="Firma" Width="120px" />
        <asp:Button runat="server" ID="btnSignAndNext" Text="Firma e prosegui" Width="150px" OnClientClick="SignAndNext();" />
        <asp:Button ID="btnAbsence" runat="server" Text="Direttori Assenti" Visible="false" Width="150px" />
        <asp:Button runat="server" ID="btnUoia" Text="Collaborazione Unica" Width="150px" OnClientClick="if(!CheckUoiaCollaborationTypes()) return false;" ToolTip="Genera Collaborazione Unica" Visible="false" Enabled="false" />
        <DocSuite:PromptClickOnceButton ID="btnNext" runat="server" Text="Prosegui" ConfirmationMessage="Confermi l'invio dei Documenti Selezionati?" DisableAfterClick="true" ConfirmBeforeSubmit="true" Width="120px" />
        <asp:Button ID="btnSelectAll" runat="server" Text="Seleziona tutti" Width="120px" Visible="false" />
        <asp:Button ID="btnDeselectAll" runat="server" Text="Annulla selezione" Width="120px" Visible="false" />
    </div>
</asp:Content>
