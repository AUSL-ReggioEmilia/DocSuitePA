<%@ Control AutoEventWireup="false" CodeBehind="ViewerLight.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.Viewers.ViewerLight" Language="vb" %>

<telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <link rel="Stylesheet" media="screen" type="text/css" href="<%=ResolveUrl("~/Viewers/css/ViewerLight.css")%>" />
    <script type="text/javascript">
        function getTreeView() {
            var baseTreeView = $find("<%= rtvListDocument.ClientID%>");
            if (baseTreeView != null) { return baseTreeView; }
            var multiPage = $find("<%= rtvListDocumentContainer.ClientID%>");
            var selectedPage = multiPage.get_selectedPageView();
            if (selectedPage != null) {
                return $find(selectedPage.get_element().children[0].id);
            }
            return null;
        }

        function getSplitter() {
            return $find(getSplitterID());
        }

        function getSplitterID() {
            return "<%= RadPageSplitter.ClientID%>";
        }

        function getToolBar() {
            return $find("<%= ToolBar.ClientID%>");
        }

        function getLeftPaneID() {
            return "<%= LeftPane.ClientID%>";
        }

        function getAjaxManager() {
            return $find("<%= RadAjaxManager.GetCurrent(Page).ClientID%>");
        }

        function getPDFPane() {
            return $find("<%= PDFPane.ClientID%>");
        }

        function toggleTreeView() {
            var tree = getTreeView();

            if (tree.get_enabled())
                tree.set_enabled(false);
            else
                tree.set_enabled(true);
        }

        function HidePDFActivex() {
            $get("pdfViewer").style.display = 'none';
        }

        function ShowPDFActivex() {
            $get("pdfViewer").style.display = 'block';
        }

        // Metodo per il lancio di una Request
        function AjaxRequest(manager, request) {
            manager.ajaxRequest(request);
        }

        // Nasconde / Visualizza la toolbar passata per parametro
        function ToggleToolBar(toolBar, enable) {
            if (toolBar) {
                var items = toolBar.get_items();
                for (var i = 0; i < items.get_count(); i++) {
                    var itm = items.getItem(i);
                    itm.set_enabled(enable);
                }
            }
        }

        // Nasconde / Visualiza il pannello nello splitter
        function TogglePane(splitter, paneId) {
            var pane = splitter.getPaneById(paneId);

            if (pane) {
                if (pane.get_collapsed()) {
                    pane.expand();
                }
                else {
                    pane.collapse();
                }
            } else {
                alert("Pane with ID '" + paneId + "' not found.");
            }
        }

        // Esegue la stampa del documento passato per parametro.
        function PrintReaderActiveXDocument(pPdfPath) {
            var viewer = $get("pdfViewer");
            if (pPdfPath != null && pPdfPath != "") {
                viewer.src = pPdfPath;
            }
            else {
                showSelectedDocument();
            }
            viewer.PrintWithDialog();
        }

        var _printIframe = null;
        function PrintGenericEmbedDocument(pPdfPath) {
            var viewer = $("#pdfViewer").clone();
            if (pPdfPath != null && pPdfPath != "") {
                viewer.attr("data", pPdfPath);
                $("#pdfViewer").replaceWith(viewer);
                $get("pdfViewer").src = pPdfPath;
            }
            else {
                showSelectedDocument();
            }

            var iframe = this._printIframe;
            if (!this._printIframe) {
                iframe = this._printIframe = document.createElement('iframe');
                document.body.appendChild(iframe);

                iframe.style.display = 'none';
                iframe.onload = function () {
                    setTimeout(function () {
                        iframe.focus();
                        iframe.contentWindow.print();
                    }, 1);
                };
            }

            var url = pPdfPath;
            if (url == null || url == "") {
                url = GetSelectedDocument().get_attributes().getAttribute("ViewLink");
            }
            iframe.src = url;
        }

        function PrintDocument(pPdfPath) {
            if (hasReaderActiveX()) {
                PrintReaderActiveXDocument(pPdfPath);
            } else {
                PrintGenericEmbedDocument(pPdfPath);
            }
        }

        function StartDownload(url) {
            setLink("link_download", url);
        }

        function setLink(key, value) {
            var link = document.getElementById(key);
            link.href = value;
            link.click();
        }

        var lastClickedNode = null;

        function SetCheckIsSignedStatus() {
            var toolBar = $find("<%= ToolBar.ClientID %>");
            if (!toolBar) {
                return;
            }
            var item = toolBar.findItemByValue("ViewerLight_CheckIsSigned");

            if (item != null) {
                item.disable();
                var node = GetSelectedDocument();
                if (node.get_attributes().getAttribute("Extension").toLowerCase() == ".pdf") {
                    item.enable();
                }
            }
        }

        function ClientNodeClicked(sender, eventArgs) {
            var node = eventArgs.get_node();
            var value = node.get_value();

            SetCheckIsSignedStatus();

            if (value === "DOCUMENT" || value === "GROUP") {
                var toolbar = getToolBar();
                var actionButtons = toolbar.get_allItems();
                var toDeleteButton;
                var isInvoiceButtonAttribute;
                for (var i = 0; i < actionButtons.length; i++) {
                    toDeleteButton = actionButtons[i];
                    isInvoiceButtonAttribute = toDeleteButton.get_attributes().getAttribute("IsInvoiceStyle");
                    if (isInvoiceButtonAttribute && isInvoiceButtonAttribute == "True") {
                        toolbar.get_items().remove(toDeleteButton);
                    }
                }

                if (!lastClickedNode) {
                    var tree = getTreeView();
                    lastClickedNode = tree.findNodeByAttribute("HasDownload", "true");
                }

                var isInvoice = node.get_attributes().getAttribute("IsInvoice");
                if (isInvoice && isInvoice == "<%= True.ToString() %>") {
                    node.highlight();
                    lastClickedNode = node;
                    AjaxRequest(getAjaxManager(), "ViewerLight_LoadInvoiceStylesheets");
                    return;
                }

                var viewLinkAttribute = node.get_attributes().getAttribute("ViewLink");
                if (viewLinkAttribute) {
                    $get("pdfViewer").src = viewLinkAttribute;
                    if (!hasReaderActiveX()) {
                        var object = $("#pdfViewer").clone();
                        object.attr("data", viewLinkAttribute);
                        $("#pdfViewer").replaceWith(object);
                    }
                    if (lastClickedNode !== null) {
                        lastClickedNode.unhighlight();
                    }
                    node.highlight();
                    lastClickedNode = node;
                } else {
                    node.unhighlight();
                    if (lastClickedNode !== null) {
                        lastClickedNode.highlight();
                        lastClickedNode.select();
                    }
                }
            }
        }

        function LoadDocumentInViewer(viewLinkAttribute) {
            $get("pdfViewer").src = viewLinkAttribute;
            if (!hasReaderActiveX()) {
                var object = $("#pdfViewer").clone();
                object.attr("data", viewLinkAttribute);
                $("#pdfViewer").replaceWith(object);
            }
        }

        function ClientNodeChecked(sender, eventArgs) {
            var node = eventArgs.get_node();
            var value = node.get_value();

            if (value == "FOLDER") {
                // Check a cascata
                var v = node.get_checked();
                var allNodes = node.get_allNodes();
                for (var i = 0; i < allNodes.length; i++) {
                    var node = allNodes[i];
                    node.set_checked(v);
                }
            }

            var treeView = $find("<%= rtvListDocument.ClientID %>");

            for (var i = 0; i < treeView.get_allNodes().length; i++) {
                var node = treeView.get_allNodes()[i];

                if (node.get_checked() == true && node.get_value() === "DOCUMENT") {
                    if ("<%= Button_StartWorklow %>"!="") {
                        $find("<%= Button_StartWorklow %>").set_enabled(true);
                        break;
                    }
                }

                if (node.get_checked() == false && node.get_value() === "DOCUMENT") {
                    if ("<%= Button_StartWorklow %>" != "") {
                        $find("<%= Button_StartWorklow %>").set_enabled(false);
                    }
                }
            }
        }

        function ClientAllChecked(cbSelect) {
            var tree = getTreeView();
            var v = cbSelect.checked == true;
            var allNodes = tree.get_allNodes();
            for (var i = 0; i < allNodes.length; i++) {
                var node = allNodes[i];
                node.set_checked(v);
            }
        }

        function OnClientButtonClickingHandler(sender, args) {
            var button = args.get_item();
            var val = button.get_value();
            switch (val) {
                case "ToggleDocuments":
                    TogglePane(getSplitter(), getLeftPaneID());
                    break;
                case "ViewerLight_Print":
                case "ViewerLight_Download":
                case "ViewerLight_Original":
                case "ViewerLight_Version":
                case "ViewerLight_CheckIsSigned":
                case "ViewerLight_ModifyPrivacy":
                    AjaxRequest(getAjaxManager(), val);
                    break;
                case "ViewerLight_LoadInvoiceStylesheets":
                    $get("pdfViewer").src = button.get_attributes().getAttribute("ViewerLink");
                    if (!hasReaderActiveX()) {
                        var object = $("#pdfViewer").clone();
                        object.attr("data", button.get_attributes().getAttribute("ViewerLink"));
                        $("#pdfViewer").replaceWith(object);
                    }
                    break;
            }
        }

        function GetSelectedDocument() {
            var node = getTreeView().get_selectedNode();
            if (node == null) {
                alert("Nessun nodo selezionato");
                return null;
            }
            if (node.get_value() != "DOCUMENT") {
                alert("Selezionare un documento");
                return null;
            }
            return node;
        }

        function requestEnd(sender, eventArgs) {
            SetCheckIsSignedStatus();
        }

        var createAxObject = function (type) {
            var ax;
            try {
                ax = new ActiveXObject(type);
            } catch (e) {
                ax = null;
            }
            return ax;
        };

        var _supportActiveX = null;
        var hasReaderActiveX = function () {
            var axObj = null;
            this._supportActiveX = (Object.getOwnPropertyDescriptor && Object.getOwnPropertyDescriptor(window, "ActiveXObject")) || ("ActiveXObject" in window);
            if (this._supportActiveX) {
                axObj = createAxObject("AcroPDF.PDF");
                //If "AcroPDF.PDF" didn't work, try "PDF.PdfCtrl"
                if (!axObj) { axObj = createAxObject("PDF.PdfCtrl"); }
                if (axObj !== null) { return true; }
            }
            return false;
        };

        function <%= Me.ID %>_OpenWindow(name, currentDocumentUnitId, currentLocationId, models) {

            HidePDFActivex();
            var url = "../Viewers/ModifyDocumentPrivacyLevel.aspx?UDId=" + currentDocumentUnitId + "&IdLocation=" + currentLocationId + "&DocumentsModel=" + models;

            var wnd = window.radopen(url, null);
            wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Resize + Telerik.Web.UI.WindowBehaviors.Close + Telerik.Web.UI.WindowBehaviors.Move + Telerik.Web.UI.WindowBehaviors.Maximize + Telerik.Web.UI.WindowBehaviors.Minimize)
            wnd.set_destroyOnClose(true);
            wnd.setSize(1000, 400);
            wnd.add_close(CloseFunction)
            wnd.set_modal(true);
            wnd.center();

            return false;
        }

        function CloseFunction(sender, args) {
            sender.remove_close(CloseFunction);
            var ajaxManager = $find("<%= AjaxManager.ClientID %>");
            if (args.get_argument() != null) {
                ajaxManager.ajaxRequest("PrivacyWindowClose" + "|" + args.get_argument());
            } else {
                ajaxManager.ajaxRequest("PrivacyWindowClose" + "|" + "false");
            }
        }

        function showSelectedDocument() {
            var treeView = getTreeView();
            if (treeView) {
                if (treeView.get_selectedNode()) {
                    $get("pdfViewer").src = treeView.get_selectedNode().get_attributes().getAttribute("ViewLink");
                }
            }
        }

        function SetWorkflowSessionStorage() {
            var archiveChainId;
            var archiveDocumentId;
            var documentName;
            var environment;
            var dtos = [];
            var environment_document_value = "3";
            var treeView = $find("<%= rtvListDocument.ClientID %>");

            for (var i = 0; i < treeView.get_allNodes().length; i++) {
                var node = treeView.get_allNodes()[i];
                if (node.get_checked() == false) {
                    continue;
                }
                environment = node.get_attributes().getAttribute("Environment");

                if (environment != environment_document_value) {
                    continue;
                }

                archiveChainId = node.get_attributes().getAttribute("BiblosChainId");
                archiveDocumentId = node.get_attributes().getAttribute("BiblosDocumentId");
                documentName = node.get_attributes().getAttribute("BiblosDocumentName");

                var dto = {
                    ArchiveChainId: archiveChainId,
                    ChainType: -1,
                    ArchiveDocumentId: archiveDocumentId,
                    ArchiveName: "",
                    DocumentName: documentName,
                    ReferenceDocument: null
                };

                dtos.push(dto);
            }

            $find("<%= Button_StartWorklow %>").set_enabled(true);
            sessionStorage.setItem("DocumentsReferenceModel", JSON.stringify(dtos));
        }

        function DisplayViewer() {
            document.getElementById("ctl00_cphContent_ViewerLight_PDFPane").style.display = "block";
        }
    </script>

    <telerik:RadWindowManager EnableViewState="false" ID="RadWindowManager" runat="server">
        <Windows>
            <telerik:RadWindow Height="450" ID="windowStartWorkflow" runat="server" Title="Avvia attività" Width="600" OnClientActivate="SetWorkflowSessionStorage" OnClientClose="DisplayViewer"/>
            <telerik:RadWindow ID="windowModifyPrivacyLevel" ReloadOnShow="false" runat="server" OnClientClose="CloseFunction" />
        </Windows>
    </telerik:RadWindowManager>
</telerik:RadCodeBlock>

<telerik:RadSplitter runat="server" Width="100%" Height="100%" BorderStyle="None" BorderSize="0" PanesBorderSize="0" Orientation="Horizontal" ResizeWithParentPane="True" ResizeWithBrowserWindow="True">
    <telerik:RadPane ID="paneToolbar" runat="server" Width="100%" Scrolling="None" Height="35px">
        <div runat="server" class="dsw-display-inline">
            <telerik:RadToolBar AutoPostBack="False" EnableRoundedCorners="False" EnableShadows="False" ID="ToolBar" OnClientButtonClicking="OnClientButtonClickingHandler" Width="100%" runat="server">
                <Items>
                    <telerik:RadToolBarButton ToolTip="Seleziona/Deseleziona tutti i documenti/cartelle">
                        <ItemTemplate>
                            <input type="checkbox" id="cbSelect" runat="server" onclick="return ClientAllChecked(this);" title="Seleziona/Deseleziona tutti i documenti/cartelle" />
                        </ItemTemplate>
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton Text="Lista documenti" ToolTip="Visualizza/Nascondi lista documenti" Value="ToggleDocuments" />
                    <telerik:RadToolBarButton ImageUrl="~/App_Themes/DocSuite2008/imgset16/printer.png" Text="Stampa" ToolTip="Stampa documento" Value="ViewerLight_Print" />
                    <telerik:RadToolBarButton ImageUrl="~/App_Themes/DocSuite2008/imgset16/downloadPDF.png" Text="Copie conformi" ToolTip="Copie conformi" Value="ViewerLight_Download" />
                    <telerik:RadToolBarButton ImageUrl="~/App_Themes/DocSuite2008/imgset16/downloadDocument.png" Text="Originali" ToolTip="Originali" Value="ViewerLight_Original" />
                    <telerik:RadToolBarButton ImageUrl="~/App_Themes/DocSuite2008/imgset16/view_history.png" Text="Visualizza versioni" ToolTip="Visualizza versioni" Value="ViewerLight_Version" />
                    <telerik:RadToolBarButton ImageUrl="~/App_Themes/DocSuite2008/imgset16/file_extension_pdf_signed.png" Text="Controlla firma" ToolTip="Controlla firma" Value="ViewerLight_CheckIsSigned" Enabled="false" />
                    <telerik:RadToolBarButton ImageUrl="~/App_Themes/DocSuite2008/imgset16/lock.png" Value="ViewerLight_ModifyPrivacy" />
                </Items>
            </telerik:RadToolBar>
        </div>
    </telerik:RadPane>
    <telerik:RadPane runat="server" ID="pnlMessageBox" Width="100%" Collapsed="True" Scrolling="None" Height="30px">
        <asp:Timer ID="timerMessage" runat="server" Interval="1000" OnTick="timerMessage_Elapsed" Enabled="false"></asp:Timer>
        <div runat="server" class="success dsw-display-inline dsw-text-right" id="messageBox" visible="false">
            <asp:Label ID="messageText" runat="server"></asp:Label>
        </div>
    </telerik:RadPane>
    <telerik:RadPane runat="server" Width="100%" Height="100%" Scrolling="None">
        <telerik:RadSplitter ID="RadPageSplitter" runat="server" Width="100%" Height="100%" BorderSize="0" ResizeWithParentPane="True" ResizeWithBrowserWindow="true">
            <telerik:RadPane Collapsed="True" ID="LeftPane" runat="server" CssClass="PanePosition">
                <telerik:RadTreeView CssClass="TreeViewFullHeight DocumentTreeContainer rtvListDocumentContainerAligned" ID="rtvListDocument" OnClientNodeChecked="ClientNodeChecked" OnClientNodeClicked="ClientNodeClicked" runat="server" SingleExpandPath="False" />
                <telerik:RadSplitter runat="server" ID="multiPages" Visible="false" Height="100%" Orientation="Horizontal">
                    <telerik:RadPane runat="server" Height="100%" Scrolling="Y">
                        <telerik:RadMultiPage ID="rtvListDocumentContainer" runat="server" SelectedIndex="0" ScrollBars="Auto" />
                    </telerik:RadPane>
                    <telerik:RadPane runat="server" Height="26px">
                        <telerik:RadTabStrip runat="server" ID="tabStripViews" Height="26px" Orientation="HorizontalBottom" Align="Justify" SelectedIndex="0" MultiPageID="rtvListDocumentContainer">
                            <Tabs></Tabs>
                        </telerik:RadTabStrip>
                    </telerik:RadPane>
                </telerik:RadSplitter>
            </telerik:RadPane>

            <telerik:RadSplitBar runat="server" ID="SplitBar" />

            <telerik:RadPane runat="server" CssClass="PDFContainer" ID="PDFPane" Scrolling="None" Height="100%">
                <object id="pdfViewer" name="pdfViewer" type="application/pdf" width="100%" height="100%" style="display: block;">
                    <noembed>Your browser does not support embedded PDF files.</noembed>
                    <div>
                        <p>&nbsp;</p>
                        <p style="text-align: center; color: white;">
                            Impossibile visualizzare il documento. Adobe Reader non è installato oppure non è correttamente configurato.
                            Contattare il supporto IT.
                        </p>
                    </div>
                    <param name="src" runat="server" id="PdfSRC" value="" />
                </object>
            </telerik:RadPane>
        </telerik:RadSplitter>
    </telerik:RadPane>
</telerik:RadSplitter>
<div class="fakeDiv" id="fake" runat="server">
    <a id="link_download" href="#" class="dsw-display-none">Il documento richiesto è pronto per il download.</a>
</div>
