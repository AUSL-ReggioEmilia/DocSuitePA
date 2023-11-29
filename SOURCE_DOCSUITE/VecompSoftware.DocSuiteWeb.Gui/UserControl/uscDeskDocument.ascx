<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscDeskDocument.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDeskDocument" %>
<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagName="DocumentUpload" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        $(function () {
            signBtnVisibility();
        });

        function signBtnVisibility() {
            var currentBrowser = getBrowserType();
            if (currentBrowser.startsWith("ie")) {
                return;
            }

            var grid = $find("<%=dgvDeskDocument.ClientID%>");
            var masterTable = grid.get_masterTableView();
            var items = masterTable.get_dataItems();

            for (var i = 0; i < items.length; i++) {
                var rowValues = items[i];
                var signBtn = rowValues.findElement("btnSignSingleDocument");
                if (signBtn) {
                    signBtn.style.display = 'none';
                }
            }
        }

        function <%= Me.ID %>_CloseDocument(sender, args, type) {
            sender.remove_close(<%= Me.ID %>_CloseDocument);
            if (args.get_argument() !== null) {
                var argument = "<%= Me.ClientID %>|UPLOAD|" + args.get_argument();
                if (type !== undefined && type != "") {
                    argument += "|" + type;
                }
                $find("<%= AjaxManager.ClientID %>").ajaxRequest(argument);
            }
        }

        function <%= Me.ID %>_OpenWindowBase(name) {
            var wnd = $find(name);
            wnd.set_destroyOnClose(true);
            wnd.show();
            wnd.center();
            return false;
        }
        function ClosewindowRenameDocument(args) {
            var oWindow = $find("<%= windowRenameDocument.ClientID%>");
            oWindow.close();
            var argument = "<%= Me.ClientID %>|RELOADDOCUMENT|";
            $find("<%= AjaxManager.ClientID %>").ajaxRequest(argument);
        }

        function <%= Me.ID %>_OpenWindow(url, name, parameters) {
            var wnd = $find(name);
            wnd.setUrl(url + "?" + parameters);
            wnd.set_destroyOnClose(true);
            wnd.show();
            wnd.center();
            return false;
        }

        /* Scaricamento file in checkout */
        function setLink(key, value) {
            var link = document.getElementById(key);
            link.href = value;
            link.click();
        }

        function StartDownload(url) {
            setLink("link_download", url);
        }

        var microsoftExcel = null;
        var microsoftWord = null;
        var handle = null;

        function InitMicrosoftWord() {
            if (!microsoftWord) {
                try {
                    microsoftWord = new ActiveXObject("Word.Application");
                }
                catch (e) {
                    alert("Problema con l'apertura di Microsoft Word. Verificare le impostazioni di sicurezza.");
                    return false;
                }
            }
            return true;
        }

        function InitMicrosoftExcel() {
            if (!microsoftExcel) {
                try {
                    microsoftExcel = new ActiveXObject("Excel.Application");
                }
                catch (e) {
                    alert("Problema con l'apertura di Microsoft Excel. Verificare le impostazioni di sicurezza.");
                    return false;
                }
            }
            return true;
        }

        function OpenDocuments(path) {
            if (InitMicrosoftWord()) {
                try {
                    microsoftWord.Visible = true;
                    handle = microsoftWord.Documents.Open(path);
                    microsoftWord.WindowState = 2;
                    microsoftWord.WindowState = 1;
                    return true;
                }
                catch (e) {
                    alert("Problema con l'apertura di Microsoft Word. Contattare l'assistenza.");
                }
            }
            return false;
        }

        function OpenWorkbooks(path) {
            if (InitMicrosoftExcel()) {
                try {
                    microsoftExcel.visible = true;
                    handle = microsoftExcel.Workbooks.Open(path, 3, false);
                    microsoftExcel.WindowState = 2;
                    microsoftExcel.WindowState = 1;
                    return true;
                }
                catch (e) {
                    alert("Problema con l'apertura di Microsoft Excel. Contattare l'assistenza.");
                }
            }
            return false;
        }

        function OpenAlert(path) {
            alert("Estensione non supportata.", path);
        }

        function OpenWord(path) {
            if (!OpenDocuments(path)) {
                alert("Problema con l'apertura del file. Impossibile trovare il percorso del file. Contattare l'assistenza.");
                return false;
            }
            return false;
        }

        function OpenExcel(path) {
            if (!OpenWorkbooks(path)) {
                alert("Problema con l'apertura del file. Impossibile trovare il percorso del file. Contattare l'assistenza.");
                return false;
            }
            return false;
        }

        var BiblosSerializeKey;

        function <%= Me.ID %>_OpenCheckInWindow(url, name, parameters, biblosKey) {
            var wnd = $find(name);
            wnd.setUrl(url + "?" + parameters);
            wnd.set_destroyOnClose(true);
            wnd.show();
            wnd.center();
            BiblosSerializeKey = biblosKey;
            return false;
        }

        function <%= Me.ID %>_CloseCheckInDocument(sender, args, type) {
            sender.remove_close(<%= Me.ID %>_CloseCheckInDocument);
            if (args.get_argument() !== null) {
                var argument = "<%= Me.ClientID %>|SETCHECKINVERSION|" + args.get_argument();
                if (type !== undefined && type != "") {
                    argument += "|" + type;
                }
                argument += "|" + BiblosSerializeKey;
                $find("<%= AjaxManager.ClientID %>").ajaxRequest(argument);
            }
        }
        /*Firma documento*/
        function <%= Me.ID %>_OpenWindowSign(url, name, parameters) {
            var wnd = $find(name);
            wnd.setUrl(url + "?" + parameters);
            wnd.set_destroyOnClose(true);
            wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Resize + Telerik.Web.UI.WindowBehaviors.Maximize + Telerik.Web.UI.WindowBehaviors.Close);
            wnd.show();
            wnd.center();
            return false;
        }

        // richiamata quando la finestra di firma viene chiusa
        function <%= Me.ID %>_CloseSignWindow(sender, args) {
            if (args.get_argument() !== null) {
                var argument = "<%= Me.ClientID %>|SIGN|" + args.get_argument();
                $find("<%= AjaxManager.ClientID %>").ajaxRequest(argument);
            }
            else
                $find("<%= AjaxManager.ClientID %>").ajaxRequest("<%= Me.ClientID %>|SIGN||");
        }

        function <%= Me.ID %>_CloseSelectTemplateWindow(sender, args) {
            if (args.get_argument() !== null) {
                var argument = "<%= Me.ClientID %>|TEMPLATEDOCUMENT|" + args.get_argument();
                $find("<%= AjaxManager.ClientID %>").ajaxRequest(argument);
            }
        }

        function <%= Me.ID %>_ExecuteCheckInDocument(docAttributes, biblosSerializeKey, version) {
            var argument = "<%= Me.ClientID %>|UPLOADCHECKIN|" + docAttributes;
            argument += "|" + biblosSerializeKey;
            argument += '|' + version;
            $find("<%= AjaxManager.ClientID %>").ajaxRequest(argument);
            var oWindow = $find("<%= windowChangeVersionDocument.ClientID%>");
            oWindow.close();
        }

        function <%= Me.ID %>_UploadDocumentNewVersionOpenWindow(docAttributes, biblosSerializeKey, version) {
            var argument = "<%= Me.ClientID %>|UPLOADVERSIONING|" + docAttributes;
            argument += "|" + biblosSerializeKey;
            argument += '|' + version;
            $find("<%= AjaxManager.ClientID %>").ajaxRequest(argument);
            var oWindow = $find("<%= windowChangeVersionDocument.ClientID%>");
            oWindow.close();
        }
   
        function btnSaveVersionClicked(args, e) {
            var argument = "<%= Me.ClientID %>|SAVENEWVERSION||||";
            $find("<%= AjaxManager.ClientID %>").ajaxRequest(argument);
        }

    </script>

</telerik:RadScriptBlock>

<telerik:RadWindowManager DestroyOnClose="True" ReloadOnShow="True" ID="RadWindowManagerUpload" runat="server">
    <Windows>
        <telerik:RadWindow Behaviors="Close" DestroyOnClose="true" Height="460px" ID="windowScannerDocument" ReloadOnShow="false" runat="server" Title="Scansione Documento" Width="800px" />
        <telerik:RadWindow Behaviors="Close" ID="windowUploadDocument" ReloadOnShow="True" runat="server" Title="Selezione Documento" />
        <telerik:RadWindow Behaviors="Maximize,Close,Resize,Reload" DestroyOnClose="True" ID="windowPreviewDocument" ReloadOnShow="false" runat="server" Title="Anteprima documento" />
        <telerik:RadWindow Behaviors="Close" ID="windowCheckInDocument" ReloadOnShow="True" runat="server" Title="Selezione Documento" />
        <telerik:RadWindow Behaviors="Close" DestroyOnClose="True" Height="500px" ID="signWindow" ReloadOnShow="true" runat="server" Title="Firma Documento" Width="600px" />
        <telerik:RadWindow Behaviors="Close" ID="wndSelectTemplate" ReloadOnShow="true" runat="server" Title="Selezione deposito documentale" />
        <telerik:RadWindow Behaviors="Close" ID="windowRenameDocument" ReloadOnShow="true" runat="server" Title="Rinomina documento">
            <ContentTemplate>
                <asp:Panel runat="server" ID="pnlNewNameWindow">
                    <div class="col-dsw-10 DeskLabel">Inserire nome file:</div>
                    <telerik:RadTextBox runat="server" ID="txtNewName" MaxLength="500" Width="100%" CssClass="inputCommentText" InputType="Text"></telerik:RadTextBox>
                    <asp:RequiredFieldValidator runat="server" ValidationGroup="namedocValidation" ID="namedocValidation" Display="Dynamic" ControlToValidate="txtNewName" ErrorMessage="Il nome file è obbligatorio" />
                </asp:Panel>
                <div class="window-footer-wrapper">
                    <telerik:RadButton runat="server" ID="btnRenameDocument" Text="Conferma" ValidationGroup="namedocValidation" />
                </div>
            </ContentTemplate>
        </telerik:RadWindow>
        <telerik:RadWindow Behaviors="Close" ID="windowChangeVersionDocument" ReloadOnShow="true" runat="server" Title="Cambio versione documento" Height="150" Width="320">
            <ContentTemplate>
                <asp:Panel runat="server" ID="pnlChangeVersion" style="padding:5px 5px 0 5px;">
                    <div class="col-dsw-10">
                        Versione attuale <b>
                            <asp:Label class="DeskLabel" ID="lblActualVersion" runat="server" Text="Label"></asp:Label></b>
                    </div>
                    <br />
                    <div>
                        <div class="col-dsw-10" style="display:inline;">Inserire nuova versione file:</div>
                        <telerik:RadTextBox runat="server" ID="txtVersionDocMax" MaxLength="3" Width="50" CssClass="inputCommentText" InputType="Text" />.
                        <telerik:RadTextBox runat="server" ID="txtVersionDocMin" MaxLength="2" Width="50" CssClass="inputCommentText" InputType="Text" />
                       
                        <div>
                            <asp:RequiredFieldValidator runat="server" ValidationGroup="verdocValidation" ID="RequiredFieldValidator1" Display="Dynamic" ControlToValidate="txtVersionDocMax" ErrorMessage="Il valore è obbligatorio" />
                            <asp:RegularExpressionValidator ControlToValidate="txtVersionDocMax" Display="Dynamic" ErrorMessage="Il Campo deve essere numerico" ID="Regularexpressionvalidator2" runat="server" ValidationExpression="^[0-9]+$" />

                            <asp:RequiredFieldValidator runat="server" ValidationGroup="verdocValidation" ID="RequiredFieldValidator2" Display="Dynamic" ControlToValidate="txtVersionDocMin" ErrorMessage="Il valore è obbligatorio" />
                            <asp:RegularExpressionValidator ControlToValidate="txtVersionDocMin" Display="Dynamic" ErrorMessage="Il Campo deve essere numerico" ID="Regularexpressionvalidator1" runat="server" ValidationExpression="^[0-9]+$" />
                        </div>
                    </div>
                </asp:Panel>
                <div class="window-footer-wrapper">
                    <telerik:RadButton runat="server" ID="btnSaveNewVersion" AutoPostBack="false" OnClientClicked="btnSaveVersionClicked" Text="Conferma" ValidationGroup="verdocValidation" />
                </div>
            </ContentTemplate>
        </telerik:RadWindow>
    </Windows>
</telerik:RadWindowManager>

<table class="datatable">
    <tr>
        <th>Documenti</th>
    </tr>
    <tr>
        <td>
            <telerik:RadToolBar AutoPostBack="true" CssClass="ToolBarContainer" EnableRoundedCorners="False" EnableShadows="False" ID="ToolBar" runat="server" Width="100%" Height="29px">
                <Items>
                    <telerik:RadToolBarButton runat="server" CausesValidation="False" Value="btnUploadDocument" CommandName="btnUploadDocument" ToolTip="Inserisci Documento" CssClass="DeskIcon" ImageUrl="~/App_Themes/DocSuite2008/imgset16/folder.png" />
                    <telerik:RadToolBarButton runat="server" CausesValidation="False" Value="btnScannerDocument" CommandName="btnScannerDocument" ToolTip="Inserisci Documento da Scanner" CssClass="DeskIcon" ImageUrl="~/App_Themes/DocSuite2008/imgset16/scanner.png" />
                    <telerik:RadToolBarButton runat="server" CausesValidation="False" Value="btnSelectTemplate" CommandName="btnSelectTemplate" ToolTip="Seleziona deposito documentale" CssClass="DeskIcon" Visible="false" ImageUrl="~/App_Themes/DocSuite2008/imgset16/template-selection.png" />
                    <telerik:RadToolBarButton runat="server" CausesValidation="False" Value="btnSignDocument" CommandName="btnSignDocument" ToolTip="Firma Documento" CssClass="DeskIcon" Visible="false" ImageUrl="~/App_Themes/DocSuite2008/imgset16/card_chip_gold.png" />
                </Items>
            </telerik:RadToolBar>
        </td>
    </tr>
    <tr>
        <td>
            <telerik:RadGrid runat="server" ID="dgvDeskDocument" AutoGenerateColumns="False" Width="100%" EnableEmbeddedSkins="false" Skin="DeskDocumentCustomSkin" AllowSorting="False" AllowPaging="False">
                <MasterTableView ClientDataKeyNames="BiblosSerializeKey" DataKeyNames="BiblosSerializeKey" AllowFilteringByColumn="False" TableLayout="Fixed">
                    <NoRecordsTemplate>
                        <div>Nessun documento selezionato</div>
                    </NoRecordsTemplate>
                    <Columns>
                        <telerik:GridClientSelectColumn UniqueName="DocumentSelect">
                            <ItemStyle Width="25px" />
                        </telerik:GridClientSelectColumn>
                        <telerik:GridTemplateColumn AllowFiltering="false" HeaderText="Nome">
                            <HeaderStyle Width="45%"></HeaderStyle>
                            <ItemStyle Wrap="true"></ItemStyle>
                            <ItemTemplate>
                                <asp:ImageButton runat="server" ID="imgDocumentExtensionType" CssClass="DeskPreviewDocument" ToolTip="Visualizza anteprima" />
                                <asp:Label runat="server" ID="lblDocumentName" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn AllowFiltering="false" HeaderText="Data">
                            <HeaderStyle Width="15%"></HeaderStyle>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblLastDate"></asp:Label>&nbsp;
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn AllowFiltering="false" HeaderText="Versione">
                            <HeaderStyle Width="100px"></HeaderStyle>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblDocVersion"></asp:Label>&nbsp;
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn UniqueName="LastComment" AllowFiltering="false" HeaderText="Ultima Modifica">
                            <HeaderStyle Width="35%"></HeaderStyle>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblLastComment"></asp:Label>&nbsp;
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn UniqueName="ViewStoryBoard" AllowFiltering="false" HeaderText="Funzioni">
                            <HeaderStyle Width="150px"></HeaderStyle>
                            <ItemTemplate>
                                <telerik:RadButton ToolTip="Rinomina nome documento" Enabled="false" runat="server" ID="btnRenameDocument" CssClass="DeskIcon" CommandName="RenameDocument" ButtonType="LinkButton" Height="16" Width="16">
                                    <Image ImageUrl="~/Comm/Images/interop/Manuale.gif" />
                                </telerik:RadButton>
                                <telerik:RadButton ToolTip="Visualizza Lavagna" Visible="false" runat="server" ID="btnViewStoryBoard" CommandName="GoToStoryBoard" CssClass="DeskIcon" ButtonType="LinkButton" Height="16" Width="17" Text="Visualizza Lavagna">
                                    <Image ImageUrl="~/App_Themes/DocSuite2008/images/desk/DeskDashBoard.png" />
                                </telerik:RadButton>
                                <telerik:RadButton runat="server" ID="btnCheckInDocument" CssClass="DeskIcon" CommandName="CheckIn" ButtonType="LinkButton" Height="16" Width="16">
                                    <Image />
                                </telerik:RadButton>
                                <telerik:RadButton ToolTip="Annulla estrazione documento" runat="server" ID="btnUndoCheckOutDocument" CssClass="DeskIcon" CommandName="CheckUndoOut" ButtonType="LinkButton" Height="16" Width="16">
                                    <Image ImageUrl="~/Comm/Images/CheckOutUndo.png" />
                                </telerik:RadButton>
                                <telerik:RadButton ToolTip="Estrai documento" runat="server" ID="btnCheckOutDocument" CssClass="DeskIcon" CommandName="CheckOut" ButtonType="LinkButton" Height="16" Width="16" Text="Estrai documento">
                                    <Image ImageUrl="~/Comm/Images/CheckOut.png" />
                                </telerik:RadButton>
                                <telerik:RadButton ToolTip="Riapri documento estratto" runat="server" ID="btnReopenCheckOutDocument" CssClass="DeskIcon" CommandName="ReopenCheckOut" ButtonType="LinkButton" Height="16" Width="16" Text="Riapri documento estratto" Visible="false">
                                    <Image ImageUrl="~/Comm/Images/CheckOut.png" />
                                </telerik:RadButton>
                                <telerik:RadButton ToolTip="Firma documento" runat="server" ID="btnSignSingleDocument" CssClass="DeskIcon" CommandName="SignSingleDocument" ButtonType="LinkButton" Height="16" Width="16" Text="Estrai documento">
                                    <Image ImageUrl="~/App_Themes/DocSuite2008/imgset16/card_chip_gold.png" />
                                </telerik:RadButton>

                                <telerik:RadButton ToolTip="Elimina Documento" Visible="false" CommandArgument="Confermi la cancellazione del documento?" OnClientClicking="RadConfirm" runat="server" ID="btnDeleteDocument" CssClass="DeskIcon" CommandName="DeleteDocument" ButtonType="LinkButton" Height="16" Width="16">
                                    <Image ImageUrl="~/App_Themes/DocSuite2008/imgset16/delete.png" />
                                </telerik:RadButton>
                                &nbsp;
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
                <ClientSettings EnableRowHoverStyle="true" EnablePostBackOnRowClick="False">
                    <Selecting AllowRowSelect="true" />
                    <Scrolling AllowScroll="true" SaveScrollPosition="true" UseStaticHeaders="true" />
                </ClientSettings>
            </telerik:RadGrid>
        </td>
    </tr>
</table>
<div class="fakeDiv" runat="server" style="display: none;">
    <a id="link_download" href="">Il documento richiesto è pronto per il download.</a>
</div>
