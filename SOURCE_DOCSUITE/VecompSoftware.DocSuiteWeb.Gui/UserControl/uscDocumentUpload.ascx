<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscDocumentUpload.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDocumentUpload" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
    <script type="text/javascript">
        // richiamata quando la finestra viene chiusa

        function <%= Me.ID %>_CloseUploadDocumentProt(sender, args) {
            <%= Me.ID %>_CloseDocument(sender, args, 'PROT');
        }

        function <%= Me.ID %>_CloseDocument(sender, args, type) {
            sender.remove_close(<%= Me.ID %>_CloseDocument);
            if (args.get_argument() !== null) {
                $get("<%= txtDocumentOK.ClientID %>").value = args.get_argument();
                var argument = "<%= Me.ClientID %>|UPLOAD|" + args.get_argument();
                if (type !== undefined && type != "") {
                    argument += "|" + type;
                }
                $find("<%= AjaxManager.ClientID%>").ajaxRequest(argument);
            }
        }

        function <%= Me.ID %>_CloseScannerRestDocument(sender, args) {
            sender.remove_close(<%= Me.ID %>_CloseScannerRestDocument);
            var documents = sessionStorage.getItem("component.scanner.upload.scan");
            if (documents) {
                var argument = "<%= Me.ClientID %>|UPLOADSCANNERREST|" + documents;
                $find("<%= AjaxManager.ClientID%>").ajaxRequest(argument);
            }
        }

        function <%= Me.ID %>_ClosePrivacyWindow(sender, args) {
            sender.remove_close(<%= Me.ID %>_ClosePrivacyWindow);
            if (args.get_argument() !== null) {
                $get("<%= txtDocumentOK.ClientID %>").value = args.get_argument();
                var argument = "<%= Me.ClientID %>|PRIVACY|" + args.get_argument();
                $find("<%= AjaxManager.ClientID%>").ajaxRequest(argument);
            }
        }

        function <%= Me.ID %>_CloseCopyDocumentResl(sender, args) {
            <%= Me.ID %>_CloseCopyDocument(sender, args, 'RESL');
        }

        // richiamata quando la finestra viene chiusa
        function <%= Me.ID %>_CloseCopyDocument(sender, args, type) {
            sender.remove_close(<%= Me.ID %>_CloseCopyDocument);
            if (args.get_argument() !== null) {
                $get("<%= txtDocumentOK.ClientID %>").value = args.get_argument();
                var argument = "<%= Me.ClientID %>|COPY|" + args.get_argument() + "|" + type;
                $find("<%= AjaxManager.ClientID %>").ajaxRequest(argument);
            }
        }

        function <%= Me.ID %>_CloseCopyDocumentSeries(sender, args, type) {
            <%= Me.ID %>_CloseCopyDocument(sender, args, 'SERIES');
        }

        // richiamata quando la finestra di firma viene chiusa
        function <%= Me.ID %>_CloseSignWindow(sender, args) {
            if (args.get_argument() !== null) {
                var argument = "<%= Me.ClientID %>|SIGN|" + args.get_argument();
                $find("<%= AjaxManager.ClientID %>").ajaxRequest(argument);
            }
        }

        function <%= Me.ID %>_CloseSelectTemplateWindow(sender, args) {
            if (args.get_argument() !== null) {
                var argument = "<%= Me.ClientID %>|TEMPLATEDOCUMENT|" + args.get_argument();
                $find("<%= AjaxManager.ClientID %>").ajaxRequest(argument);
            }
        }

        // richiamata quando non esiste nessun nodo figlio della root per svuotare la textbox di validazione
        function <%= Me.ID %>_ClearTextValidator() {
            $get("<%= txtDocumentOK.ClientID %>").value = "";
        }

        // richiamata quando viene aggiunto un contatto tramite chiamata AJAX perchè lato server non viene riconosciuta la modifica della TextBox
        function <%= Me.ID %>_SetTextValidator() {
            $get("<%= txtDocumentOK.ClientID %>").value = "OK";
            var val = $get("<%= rfvDocument.ClientID %>");
            if (val) {
                val.isValid = true;
            }
        }

        function <%= Me.ID %>_OpenWindow(url, name, parameters) {
            var wnd = $find(name);
            wnd.setUrl(url + "?" + parameters);
            wnd.set_destroyOnClose(true);
            wnd.show();
            wnd.center();
            return false;
        }

        function <%= Me.ID %>_OpenWindowScanner(url, name, parameters) {
            sessionStorage.removeItem("component.scanner.upload.scan");
            var wnd = $find(name);
            wnd.setUrl(url + "?" + parameters);
            wnd.set_destroyOnClose(true);
            wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Resize + Telerik.Web.UI.WindowBehaviors.Maximize + Telerik.Web.UI.WindowBehaviors.Close);
            wnd.show();
            wnd.center();
            return false;
        }

        function <%= Me.ID %>_OpenWindowSign(url, name, parameters) {
            var wnd = $find(name);
            wnd.setUrl(url + "?" + parameters);
            wnd.set_destroyOnClose(true);
            wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Resize + Telerik.Web.UI.WindowBehaviors.Maximize + Telerik.Web.UI.WindowBehaviors.Close);
            wnd.show();
            wnd.center();
            return false;
        }

        function <%= Me.ID %>_ConfirmRemoveDocumentCallback(args) {
            if (args) {
                $find("<%= AjaxManager.ClientID %>").ajaxRequest('<%= Me.ClientID %>|REMOVE');
            }
        }

        function <%= Me.ID %>_ConfirmRemoveDocument() {
            var treeDocuments = $find("<%= RadTreeViewDocument.ClientID %>");
            var documentCount = treeDocuments.get_nodes().getNode(0).get_nodes().get_count();
            if (documentCount > 0) {
                var selectedNode = treeDocuments.get_selectedNode();
                if ((selectedNode != undefined && (selectedNode.get_value() == undefined || selectedNode.get_value().toLowerCase() != 'root'))
                    || (selectedNode == undefined && documentCount == 1)) {
                    if (documentCount == 1) {
                        selectedNode = treeDocuments.get_nodes().getNode(0).get_nodes().getNode(0);
                    }
                    var lockedAttribute = selectedNode.get_attributes().getAttribute('Locked');
                    var removeConfirm = true;
                    if (lockedAttribute != undefined && lockedAttribute == 'True') {
                        removeConfirm = confirm('Attenzione, si stà tentando di eliminare un documento già confermato, continuare?');
                    }

                    <%= Me.ID %>_ConfirmRemoveDocumentCallback(removeConfirm);
                }
            }
        }

        function <%= Me.ID %>_onNodeDropping(sender, args) {
            var dest = args.get_destNode();
            if (dest) {
                <%= Me.ID %>_clientSideEdit(sender, args);
                args.set_cancel(true);
            }
        }

        function <%= Me.ID %>_clientSideEdit(sender, args) {
            var destinationNode = args.get_destNode();
            var destinationTreeViewId = destinationNode.get_treeView().get_id();

            if (destinationNode && destinationTreeViewId == "<%= RadTreeViewDocument.ClientID %>" && destinationNode.get_value() != 'Root') {
                currentTreeView = $find("<%= RadTreeViewDocument.ClientID %>");

                currentTreeView.trackChanges();
                var sourceNodes = args.get_sourceNodes();
                var dropPosition = args.get_dropPosition();

                if (dropPosition == "over")
                    return;

                //Needed to preserve the order of the dragged items
                if (dropPosition == "below") {
                    for (var i = sourceNodes.length - 1; i >= 0; i--) {
                        var sourceNode = sourceNodes[i];
                        sourceNode.get_parent().get_nodes().remove(sourceNode);

                        <%= Me.ID %>_insertAfter(destinationNode, sourceNode);
                    }
                }
                else {
                    for (var j = 0; j < sourceNodes.length; j++) {
                        sourceNode = sourceNodes[j];
                        sourceNode.get_parent().get_nodes().remove(sourceNode);

                        if (dropPosition == "above")
                            <%= Me.ID %>_insertBefore(destinationNode, sourceNode);
                    }
                }
                destinationNode.set_expanded(true);
                currentTreeView.commitChanges();
            }
        }

        function <%= Me.ID %>_insertBefore(destinationNode, sourceNode) {
            var destinationParent = destinationNode.get_parent();
            var index = destinationParent.get_nodes().indexOf(destinationNode);
            destinationParent.get_nodes().insert(index, sourceNode);
        }

        function <%= Me.ID %>_insertAfter(destinationNode, sourceNode) {
            var destinationParent = destinationNode.get_parent();
            var index = destinationParent.get_nodes().indexOf(destinationNode);
            destinationParent.get_nodes().insert(index + 1, sourceNode);
        }

        function <%= Me.ID %>_editDocumentName() {
            var treeDocuments = $find("<%= RadTreeViewDocument.ClientID %>");
            var selectedNode = treeDocuments.get_selectedNode();
            if (selectedNode) {
                selectedNode.startEdit();
            }
            return false;
        }

        var currentLoadingPanel = null;
        var currentUpdatedControl = null;
        function <%= Me.ID %>_requestStart() {
            if (currentLoadingPanel != null) {
                return false;
            }
            currentLoadingPanel = $find("<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
            currentUpdatedControl = "<%= pnlControl.ClientID%>";
            currentLoadingPanel.show(currentUpdatedControl);
        }

        function <%= Me.ID %>_responseEnd() {
            if (currentLoadingPanel == null) {
                currentLoadingPanel = $find("<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                currentUpdatedControl = "<%= pnlControl.ClientID%>";
            }

            currentLoadingPanel.hide(currentUpdatedControl);
            currentUpdatedControl = null;
            currentLoadingPanel = null;
        }

        function <%= Me.ID %>_treeDocumentEdited(sender, args) {
            var node = args.get_node();
            if (node.get_text() != node._originalText) {
                <%= Me.ID %>_requestStart();
            }
        }

        function <%= Me.ID %>_sendSecureDocumentRequest() {
            var treeDocuments = $find("<%= RadTreeViewDocument.ClientID %>");
            var documentCount = treeDocuments.get_nodes().getNode(0).get_nodes().get_count();
            if (documentCount > 0) {
                var selectedNode = treeDocuments.get_selectedNode();
                if ((selectedNode != undefined && (selectedNode.get_value() == undefined || selectedNode.get_value().toLowerCase() != 'root'))
                    || (selectedNode == undefined && documentCount == 1)) {
                    if (documentCount == 1) {
                        selectedNode = treeDocuments.get_nodes().getNode(0).get_nodes().getNode(0);
                    }
                    var secureDocumentAttribute = selectedNode.get_attributes().getAttribute('SecureDocumentId');
                    var sendConfirm = false;
                    if (!secureDocumentAttribute) {
                        sendConfirm = confirm('Confermi la richiesta di securizzazione del documento?');
                    } else {
                        alert('Il documento selezionato è già securizzato');
                    }

                    if (sendConfirm) {
                        <%= Me.ID %>_requestStart();
                        $find("<%= AjaxManager.ClientID %>").ajaxRequest('<%= Me.ClientID %>|SECUREDOCUMENT');
                    }
                }
            }
        }

        function <%= Me.ID %>_ddlPrivacyLevels_SelectedIndexChanged(sender, args) {

            var tree = sender.get_parent();
            var selectedValue = sender.get_selectedItem().get_value();
            var indexNode = sender.get_attributes().getAttribute("nodeIndex");
            var nodeToUpdate = tree._nodeData[0].items[indexNode];
            nodeToUpdate.attributes["PrivacyLevel"] = selectedValue;

            if (args) {
                $find("<%= AjaxManager.ClientID %>").ajaxRequest('<%= Me.ClientID %>|PRIVACYLEVELSET|'.concat(indexNode, "|", selectedValue));
            }
        }
    </script>
</telerik:RadScriptBlock>

<telerik:RadWindowManager DestroyOnClose="True" ReloadOnShow="True" ID="RadWindowManagerUpload" runat="server">
    <Windows>
        <telerik:RadWindow Behaviors="Close" DestroyOnClose="True" Height="500px" ID="signWindow" ReloadOnShow="true" runat="server" Title="Firma Documento" Width="850px" />
        <telerik:RadWindow Behaviors="Close" DestroyOnClose="true" Height="460px" ID="windowScannerDocument" ReloadOnShow="true" runat="server" Title="Scansione Documento" Width="800px" />
        <telerik:RadWindow Behaviors="Close" ID="windowUploadDocument" ReloadOnShow="True" runat="server" Title="Selezione Documento" />
        <telerik:RadWindow Behaviors="Maximize,Close,Resize,Reload" DestroyOnClose="True" ID="windowPreviewDocument" ReloadOnShow="false" runat="server" Title="Anteprima documento" />
        <telerik:RadWindow Behaviors="Close" ID="windowSharedFolder" ReloadOnShow="false" runat="server" Title="Cartella Condivisa" />
        <telerik:RadWindow Behaviors="Close" ID="wndCopyProtocol" ReloadOnShow="true" runat="server" Title="Copia da Protocollo" />
        <telerik:RadWindow Behaviors="Close" ID="wndCopyResl" ReloadOnShow="true" runat="server" Title="Copia da Atto/Delibera" />
        <telerik:RadWindow Behaviors="Close" ID="wndCopySeries" ReloadOnShow="true" runat="server" />
        <telerik:RadWindow Behaviors="Close" ID="wndSelectTemplate" ReloadOnShow="true" runat="server" Title="Selezione deposito documentale" />
    </Windows>
</telerik:RadWindowManager>
<asp:Panel runat="server" ID="pnlControl">
    <table class="datatable">
        <tr>
            <th colspan="2" runat="server" id="tblHeader">
                <asp:Label runat="server" ID="lblCaption" Text="Documento" />
            </th>
        </tr>
        <tr>
            <td class="col-dsw-7">
                <asp:Label Font-Italic="true" ID="lblSendSourceDocument" runat="server" Style="display: none;" Text="Selezionare per quali documenti effettuare ANCHE l'invio del documento originale." />
                <%-- TreeView con l'elenco dei documenti --%>
                <telerik:RadTreeView CausesValidation="false" ID="RadTreeViewDocument" runat="server" ShowLineImages="true">   
                    <NodeTemplate>                                                       
                        <asp:Label runat="server" id="lbl" Text='<%# DataBinder.Eval(Container, "Text") %>'/>                           
                        <asp:label runat="server" ID="lblPrivacy"></asp:label>
                        <telerik:RadDropDownList ID="ddlPrivacyLevels" runat="server" AutoPostBack="false" Width="150px" DropDownHeight="150px"></telerik:RadDropDownList>  
                    </NodeTemplate>
                    <Nodes>
                        <telerik:RadTreeNode Expanded="true" Font-Bold="true" runat="server" Text="Documento" Value="Root">
                            <NodeTemplate>
                                <asp:Label runat="server" id="lbl" Text='<%# DataBinder.Eval(Container, "Text") %>'/>
                            </NodeTemplate>
                        </telerik:RadTreeNode>
                    </Nodes>
                </telerik:RadTreeView>
            </td>
            <td class="col-dsw-3" style="text-align: right;" runat="server" id="tblButtons">
                <%--Pulsanti funzione per l'upload di un documento a parte dell'utente--%>
                <asp:Panel runat="server" ID="pnlButtons">
                    <div style="height: 18px; white-space: nowrap;">
                        <asp:ImageButton CausesValidation="False" ID="btnEditName" ImageUrl="~/App_Themes/DocSuite2008/imgset16/pencil.png" runat="server" Visible="false" ToolTip="Modifica il nome del documento" />
                        <asp:ImageButton CausesValidation="False" ID="btnSecureDocument" ImageUrl="~/App_Themes/DocSuite2008/imgset16/document_signature_green.png" runat="server" Visible="false" ToolTip="Securizzazione del documento" />
                        <asp:ImageButton CausesValidation="False" ID="btnAddDocumentScanner" ImageUrl="~/App_Themes/DocSuite2008/imgset16/scanner.png" runat="server" ToolTip="Inserimento documento da scanner" />
                        <asp:ImageButton CausesValidation="False" ID="btnAddDocument" ImageUrl="~/App_Themes/DocSuite2008/imgset16/folder.png" runat="server" ToolTip="Inserimento documento" />
                        <asp:ImageButton CausesValidation="False" ID="btnUploadSharepoint" ImageUrl="~/App_Themes/DocSuite2008/imgset16/sharepoint.png" runat="server" Visible="false" />
                        <asp:ImageButton CausesValidation="False" ID="btnPrivacyLevel" ImageUrl="~/App_Themes/DocSuite2008/imgset16/lock.png" runat="server" Visible="false" />
                        <asp:ImageButton CausesValidation="False" ID="btnRemoveDocument" ImageUrl="~/App_Themes/DocSuite2008/imgset16/delete.png" runat="server" ToolTip="Elimina documento" />
                        <asp:ImageButton CausesValidation="False" ID="btnPreviewDoc" ImageUrl="~/App_Themes/DocSuite2008/imgset16/documentPreview.png" runat="server" ToolTip="Anteprima documento" />
                        <asp:ImageButton CausesValidation="False" ID="btnSignDocument" ImageUrl="~/App_Themes/DocSuite2008/imgset16/card_chip_gold.png" runat="server" ToolTip="Firma documento" />
                        <asp:ImageButton CausesValidation="False" ID="btnImportSharedFolder" ImageUrl="~/Comm/Images/FolderOpen16.gif" runat="server" ToolTip="Seleziona documento da cartella condivisa" />
                        <asp:ImageButton CausesValidation="False" ID="btnCopyProtocol" ImageUrl="~/Comm/Images/DocSuite/Collegamento16.gif" runat="server" ToolTip="Copia da protocollo" Visible="false" />
                        <asp:ImageButton CausesValidation="False" ID="btnCopyResl" ImageUrl="~/Resl/Images/Delibera.gif" runat="server" ToolTip="Copia da atto/delibera" Visible="false" />
                        <asp:ImageButton CausesValidation="False" ID="btnCopySeries" ImageUrl="~/App_Themes/DocSuite2008/imgset16/document_copies_add.png" runat="server" Visible="false" />
                        <asp:ImageButton CausesValidation="False" ID="btnImportContactManual" ImageUrl="~/App_Themes/DocSuite2008/imgset16/FromExcel.png" runat="server" ToolTip="Importazione contatti manuali destinatari/mittenti" Visible="false" />
                        <asp:ImageButton CausesValidation="False" ID="btnSelectTemplate" ImageUrl="~/Comm/Images/Selezione16.gif" runat="server" ToolTip="Seleziona deposito documentale" Visible="false" />
                    </div>
                </asp:Panel>
                <div id="tblFrontespizio" runat="server" visible="false">
                    <telerik:RadButton ButtonType="LinkButton" CausesValidation="false" SingleClick="True" ID="btnFrontespizio" runat="server" Text="Genera" Width="70px" />
                </div>
                <%--Pulsanti Nascosti per aggiungere documenti alla treeview--%>
                <div style="position: absolute;">
                    <asp:Button CausesValidation="False" ID="btnAddDocumentFile" runat="server" Text="AddDoc" Width="10px" />
                    <asp:Button CausesValidation="False" ID="btnAddDocumentFileFDQ" runat="server" Text="AddDocFDQ" Width="10px" />
                </div>
            </td>
        </tr>
    </table>
</asp:Panel>

<%-- Campi nascosti con le informazioni sul documento aggiunto--%>
<asp:TextBox ID="txtDocumentOK" runat="server" AutoPostBack="True" />
<asp:TextBox ID="txtFileName" runat="server" Width="16px" />
<asp:TextBox ID="txtFileDateTime" runat="server" Width="16px" />
<asp:RequiredFieldValidator ControlToValidate="txtDocumentOK" Display="Dynamic" ErrorMessage="Documento Obbligatorio" ID="rfvDocument" runat="server" />
