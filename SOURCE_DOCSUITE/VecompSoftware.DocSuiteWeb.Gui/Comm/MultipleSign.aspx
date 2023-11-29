<%@ Page AutoEventWireup="false" CodeBehind="MultipleSign.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.MultipleSign" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Firma Multipla" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
        <style type="text/css">
            .rbDecorated {
                padding-right: 12px !important;
            }
        </style>
        <script type="text/javascript">
            var activex = null;
            function riseSign(inputDir, outputDir, signTypes, comment) {
                activex = new ActiveXObject("VecompSoftware.MultiSigner.MultiSignerCtrl");
                // Controllo activex
                if (!activex) {
                    alert("Impossibile trovare il componente.");
                    return;
                }
                // Attivo pannello di caricamento
                RequestStart();
                // Reimposto variabili di sessione firma
                var errorField = $get("<%= hdnHasError.ClientID%>");
                errorField.value = "0";
                $get("risultati").value = "";

                // Imposto activex
                activex.InputDirectory = inputDir;
                activex.OutputDirectory = outputDir;
                activex.SignTypes = signTypes;
                activex.Options = comment;
                eval("function activex::OnError(message) {return showError(message);}");
                eval("function activex::OnProcessedItem(index) {return showProcessed(index);}");
                // Attivo activex
                activex.SignDocuments();
                // Avvio il check di progressione
                setTimeout(checkIsDone, 1000);
            }

            function showError(message) {
                $get("risultati").value += message + "\n\r ";
                var errorField = $get("<%= hdnHasError.ClientID%>");
                errorField.value = parseInt(errorField.value, 10) + 1;
                return true;
            }

            function showProcessed(index) {
                //$get("risultati").value += "\n\r Item " + index;
                return true;
            }

            function checkIsDone() {
                if (!activex.IsDone()) {
                    setTimeout(checkIsDone, 8000);
                    return;
                }
                ResponseEnd();

                var ajaxModel = {};
                ajaxModel.ActionName = "Signed";
                $find("<%= AjaxManager.ClientID %>").ajaxRequest(JSON.stringify(ajaxModel));
            }

            var currentLoadingPanel = null;
            var currentUpdatedControl = null;
            function RequestStart(sender, args) {
                currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                currentUpdatedControl = "<%= DocumentListGrid.ClientID%>";
                //show the loading panel over the updated control
                currentLoadingPanel.show(currentUpdatedControl);
            }

            function ResponseEnd() {
                //hide the loading panel and clean up the global variables
                if (currentLoadingPanel != null) {
                    currentLoadingPanel.hide(currentUpdatedControl);
                }
                currentUpdatedControl = null;
                currentLoadingPanel = null;
            }

            function RowDeselecting(sender, eventArgs) {
                var masterTable = sender.get_masterTableView();
                var row = masterTable.get_dataItems()[eventArgs.get_itemIndexHierarchical()];
                var cell = masterTable.getCellByColumnUniqueName(row, "SelectColumn");
                var inputCheckBox = cell.getElementsByTagName("input")[0];
                if (inputCheckBox.disabled) {
                    //cancel selection 
                    eventArgs.set_cancel(true);
                    return false;
                }
                return true;
            }

            function OpenGenericWindow(url) {
                var wnd = window.radopen(url, null);
                wnd.set_visibleStatusbar(false);
                wnd.set_modal(true);
                wnd.set_showOnTopWhenMaximized(false);
                wnd.set_destroyOnClose(true);
                return wnd;
            }

            function signTypeDropdown_OnClientLoad(sender) {
                setTimeout(function () {
                    var selectedValue = sender._selectedValue;
                    showHideContainers(selectedValue);
                }, 500)
            }

            function signTypeDropdown_SelectedIndexChanged(sender, eventArgs) {
                document.getElementById("<%= PinTextbox.ClientID %>").value = "";
                showAjaxLoadingPanel("<%= ToolBar.ClientID %>");
                var selectedValue = sender._selectedValue;
                showHideContainers(selectedValue);
            }

            function showHideContainers(selectedValue) {
                var toolBar = $find("<%= ToolBar.ClientID %>");
                if (toolBar != null)
                    switch (selectedValue) {
                        case "0": //CARD 
                        case "5": //GO SIGN
                            {
                                toolBar.findItemByValue("pinContainer2").set_visible(false);
                                toolBar.findItemByValue("pinContainer").set_visible(false);
                                break;
                            }
                        case "3": //ARUBA AUTO
                            {
                                toolBar.findItemByValue("pinContainer2").set_visible(true);
                                toolBar.findItemByValue("pinContainer").set_visible(true);
                                break;
                            }
                        case "4": //INFOCERT AUTO
                            {
                                toolBar.findItemByValue("pinContainer2").set_visible(true);
                                toolBar.findItemByValue("pinContainer").set_visible(true);
                                break;
                            }
                    }
            }

            function showAjaxLoadingPanel(htmlElementId) {
                var ajaxLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>");
                ajaxLoadingPanel.show(htmlElementId);
            }

            function hideAjaxLoadingPanel(htmlElementId) {
                var ajaxLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>");
                ajaxLoadingPanel.hide(htmlElementId);
            }

            /**
             * firma dike go sign
             * @param url
             */
            function signGoSign(transactionId, fileNames) {
                showAjaxLoadingPanel("<%= ToolBar.ClientID %>");
                var windowManager = $find("<%= MasterDocSuite.DefaultWindowManager.ClientID %>");
                window.setTimeout(function () {
                    var wnd = windowManager.open("../UserControl/CommonGoSignFlow.aspx?GoSignSessionId=" + transactionId, null, null, 850, 500, 10, 10);
                    wnd.set_destroyOnClose(true);
                    wnd.add_close(function (sender, args) {
                        var data = args.get_argument();
                        if (data) {
                            var ajaxModel = {};
                            ajaxModel.Value = [transactionId, fileNames];
                            ajaxModel.ActionName = "SignGoSignCompleted";

                            $find("<%= MasterDocSuite.AjaxManager.ClientID %>").ajaxRequest(JSON.stringify(ajaxModel));
                            return;
                        }
                        hideAjaxLoadingPanel("<%= ToolBar.ClientID %>");
                        alert("Attività cancellata dall'utente.");
                    });
                    wnd.set_behaviours(Telerik.Web.UI.WindowBehaviors.Close);
                    wnd.center();
                }, 0);
            }

        </script>
    </telerik:RadScriptBlock>
    
    <telerik:RadToolBar AutoPostBack="true" CssClass="ToolBarContainer" EnableRoundedCorners="False" EnableShadows="False" ID="ToolBar" runat="server" Width="100%">
        <Items>
            <telerik:RadToolBarButton Value="signTypeDropdown">
                <ItemTemplate>
                    <telerik:RadDropDownList runat="server" ID="signTypeDropdown" Width="160px" AutoPostBack="true" DropDownHeight="200px" OnClientLoad="signTypeDropdown_OnClientLoad" OnClientSelectedIndexChanged="signTypeDropdown_SelectedIndexChanged" OnSelectedIndexChanged="signTypeDropdown_SelectedIndexChanged">
                        <Items>
                            <telerik:DropDownListItem Selected="true" Text="Smartcard" Value="0" />
                            <telerik:DropDownListItem Text="Automatica Aruba" Value="3" />
                            <telerik:DropDownListItem Text="Automatica Infocert" Value="4" />
                            <telerik:DropDownListItem Text="Dike GoSign" Value="5" />
                        </Items>
                    </telerik:RadDropDownList>
                </ItemTemplate>
            </telerik:RadToolBarButton>
            <telerik:RadToolBarButton IsSeparator="true" />
            <telerik:RadToolBarButton CommandName="AddComment">
                <ItemTemplate>
                    <telerik:RadButton runat="server" ID="chkAddComment" ToggleType="CheckBox" ButtonType="StandardButton" Checked="False">
                        <ToggleStates>
                            <telerik:RadButtonToggleState Text="Firma con funzioni vicariali" PrimaryIconCssClass="rbToggleCheckboxChecked" />
                            <telerik:RadButtonToggleState Text="Firma con funzioni vicariali" PrimaryIconCssClass="rbToggleCheckbox" />
                        </ToggleStates>
                    </telerik:RadButton>
                </ItemTemplate>
            </telerik:RadToolBarButton>
            <telerik:RadToolBarButton IsSeparator="true" />
            <telerik:RadToolBarButton>
                <ItemTemplate>
                    <i>Tipo File:</i>
                </ItemTemplate>
            </telerik:RadToolBarButton>
            <telerik:RadToolBarButton ButtonType="StandardButton" CheckOnClick="true" Group="typeToggle" runat="server" Text="Originale" ToggleType="Radio" CommandName="originalToggle" />
            <telerik:RadToolBarButton ButtonType="StandardButton" CheckOnClick="true" Group="typeToggle" runat="server" Text="Pdf" ToggleType="Radio" CommandName="pdfToggle" />
            <telerik:RadToolBarButton IsSeparator="true" Value="signSeparator" />
            <telerik:RadToolBarButton Value="signDescription">
                <ItemTemplate>
                    <i>Tipo Firma:</i>
                </ItemTemplate>
            </telerik:RadToolBarButton>
            <telerik:RadToolBarButton ButtonType="StandardButton" CheckOnClick="true" Group="signToggle" runat="server" Text="CAdES" ToggleType="Radio" CommandName="cadesToggle" Value="cadesSign" />
            <telerik:RadToolBarButton ButtonType="StandardButton" CheckOnClick="true" Group="signToggle" runat="server" Text="PAdES" ToggleType="Radio" CommandName="padesToggle" Value="padesSign" />
            <telerik:RadToolBarButton IsSeparator="true" Value="signSeparator" />
            <telerik:RadToolBarButton Value="pinContainer2">
                <ItemTemplate>
                    <span class="templateText">Pin:</span>
                </ItemTemplate>
            </telerik:RadToolBarButton>
            <telerik:RadToolBarButton Value="pinContainer">
                <ItemTemplate>
                    <telerik:RadTextBox ID="pin" runat="server" TextMode="Password" Width="60px" />
                </ItemTemplate>
            </telerik:RadToolBarButton>
            <telerik:RadToolBarButton IsSeparator="true" Value="pinSeparator" />
            <telerik:RadToolBarButton CommandName="sign" ImageUrl="~/App_Themes/DocSuite2008/imgset16/text_signature.png" Text="Firma" ToolTip="Firma il documento visualizzato." Value="sign" />
            <telerik:RadToolBarButton CommandName="undo" ImageUrl="~/App_Themes/DocSuite2008/imgset16/cancel.png" Text="Annulla" ToolTip="Annulla" Value="undo" />
        </Items>
    </telerik:RadToolBar>
    <input type="hidden" id="hdnHasError" runat="server" value="0" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <div class="radGridWrapper">
        <telerik:RadGrid runat="server" ID="DocumentListGrid" Width="100%" Height="100%" EnableViewState="true" AllowMultiRowSelection="True">
            <MasterTableView AutoGenerateColumns="False" DataKeyNames="Serialized">

                <GroupByExpressions>
                    <telerik:GridGroupByExpression>
                        <GroupByFields>
                            <telerik:GridGroupByField FieldName="GroupCode" HeaderText="Codice"></telerik:GridGroupByField>
                        </GroupByFields>
                        <SelectFields>
                            <telerik:GridGroupByField FieldName="Description" FieldAlias="Oggetto" Aggregate="First" />
                            <telerik:GridGroupByField FieldName="IdOwner" FieldAlias="Documenti" Aggregate="Count" />
                        </SelectFields>
                    </telerik:GridGroupByExpression>
                </GroupByExpressions>

                <Columns>
                    <telerik:GridClientSelectColumn HeaderStyle-Width="16px" UniqueName="SelectColumn" />


                    <telerik:GridTemplateColumn HeaderStyle-Width="16px" HeaderImageUrl="~/App_Themes/DocSuite2008/imgset16/documentPreview.png" HeaderText="Tipo documento">
                        <ItemTemplate>
                            <asp:ImageButton ID="documentType" runat="server" CommandName="preview" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>

                    <telerik:GridTemplateColumn UniqueName="Type" HeaderText="Tipo" HeaderStyle-Width="100px">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblDocType" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>

                    <telerik:GridTemplateColumn UniqueName="Name" HeaderText="Documento" HeaderStyle-Width="200px">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblFileName" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>

                    <telerik:GridTemplateColumn HeaderText="Tipo File" HeaderStyle-Width="160px">
                        <ItemTemplate>
                            <telerik:RadButton AutoPostBack="false" ButtonType="StandardButton" GroupName="typeToggle" ID="originalToggle" runat="server" Text="Originale" ToggleType="Radio" Width="75px" />
                            <telerik:RadButton AutoPostBack="false" ButtonType="StandardButton" GroupName="typeToggle" ID="pdfToggle" runat="server" Text="Pdf" ToggleType="Radio" Width="75px" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="SignType" HeaderText="Tipo Firma" HeaderStyle-Width="160px">
                        <ItemTemplate>
                            <telerik:RadButton AutoPostBack="false" ButtonType="StandardButton" GroupName="signToggle" ID="cadesToggle" runat="server" Text="CAdES" ToggleType="Radio" Width="75px" />
                            <telerik:RadButton AutoPostBack="false" ButtonType="StandardButton" GroupName="signToggle" ID="padesToggle" runat="server" Text="PAdES" ToggleType="Radio" Width="75px" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>

            <ClientSettings EnableRowHoverStyle="False">
                <Selecting AllowRowSelect="True" EnableDragToSelectRows="False" UseClientSelectColumnOnly="True" />
                <ClientEvents OnRowDeselecting="RowDeselecting"></ClientEvents>
                <Scrolling AllowScroll="true" ScrollHeight="100%" />
            </ClientSettings>

            <SortingSettings SortedAscToolTip="Ordine crescente" SortedDescToolTip="Ordine descrescente" SortToolTip="Ordina" />
            <GroupingSettings ShowUnGroupButton="true" UnGroupButtonTooltip="Rimuovi"></GroupingSettings>
        </telerik:RadGrid>
    </div>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphFooter">
    <textarea cols="" disabled="disabled" id="risultati" rows="3" style="width: 100%"></textarea>
</asp:Content>