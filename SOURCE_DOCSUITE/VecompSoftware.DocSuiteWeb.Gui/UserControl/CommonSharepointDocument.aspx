<%@ Page Title="Carica file da libreria Sharepoint " Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="CommonSharepointDocument.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSharepointDocument" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphHeader">
    <asp:CheckBox ID="chkDisableFileExtensionWhiteList" runat="server" Text="Autorizza caricamento estensioni sconosciute" AutoPostBack="true" TextAlign="Right" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock ID="RadScriptBlock" runat="server" EnableViewState="false">
        <script language="javascript" type="text/javascript">

            function GetRadWindow() {
                if (window.radWindow)
                    return window.radWindow;
                else if (window.frameElement.radWindow)
                    return window.frameElement.radWindow;
                return null;
            }

            function CloseWindow(args) {
                var oWindow = GetRadWindow();
                oWindow.close(args);
            }

            function OnClientFileUploading(sender, args) {
                //Verifico la blacklist
                if (isBlackListed(args.get_fileName())) {
                    validationBlackList(args.get_fileName());
                    args.set_cancel(true);
                }
            }

            function OnClientFileUploaded(sender, args) {
                //Verifico la size
                checkSize(args.get_fileInfo());
            }

            function isBlackListed(fileName) {
                if (fileName.indexOf('.') == -1)
                    return true;

                var disallowed = '<%= ClientSideFileExtensionBlackList%>';
                if (disallowed.length == 0)
                    return false;

                var splitted = disallowed.split('|');
                var extension = fileName.substring(fileName.lastIndexOf('.')).toLowerCase();
                var i = splitted.length;
                while (i--)
                    if (splitted[i] == extension)
                        return true;
                return false;
            }

            function submitPage() {
                var uploadingRows = $(".RadAsyncUpload").find(".ruUploadProgress");
                var i = uploadingRows.length;
                while (i--) {
                    if (!$(uploadingRows[i]).hasClass("ruUploadCancelled") && !$(uploadingRows[i]).hasClass("ruUploadFailure") && !$(uploadingRows[i]).hasClass("ruUploadSuccess")) {
                        alert("Attendere il caricamento dei file prima di procedere.");
                        return false;
                    }
                }
                return true;
            }

            function validationBlackList(args) {
                $("#ErrorHolder").append("<p>Estensione non valida per il file: '" + args + "'.</p>");
                $("#ErrorHolder").show();
            }

            function validationFailed(sender, eventArgs) {
                var maxfilesize = "<%=ProtocolEnv.MaxUploadThreshold%>";
                var mbSize = maxfilesize/1048576;
                var fileExtention = eventArgs.get_fileName().substring(eventArgs.get_fileName().lastIndexOf('.') + 1, eventArgs.get_fileName().length);
                if (eventArgs.get_fileName().lastIndexOf('.') != -1) {
                    if (sender.get_allowedFileExtensions().length > 0 && sender.get_allowedFileExtensions().indexOf(fileExtention) == -1) {
                        //Se ho impostato delle estensioni consentite e non è in queste ==> ritorno errore su estensione
                        $("#ErrorHolder").append("<p>Estensione non valida per il file: '" + eventArgs.get_fileName() + "'.</p>");
                    }
                    else {
                        //Altrimenti si tratta di grandezza eccessiva
                        $("#ErrorHolder").append("<p>Grandezza massima superata per il file: '" + eventArgs.get_fileName() + "'. La dimensione massima consentita è di "+ mbSize +" Mb.</p>");
                    }
                }
                else {
                    $("#ErrorHolder").append("<p>Impossibile gestire il file: '" + eventArgs.get_fileName() + "'.</p>");
                }
                $("#ErrorHolder").show();
            }

             <%--function purgeBlackListed() {
                var upload = $find("<%= AsyncUploadDocument.ClientID%>");
                var files = upload.getUploadedFiles();
                var i = files.length;
                while (i--)
                    if (isBlackListed(files[i])) {
                        var delta = (upload._additionalFieldIndex > 0 ? 1 : 0);
                        if (upload._currentIndex == 1)
                            delta = 1;
                        upload.deleteFileInputAt(i + delta);
                    }
                hideErrors();
            }--%>
            
           <%-- function OnClientRemoved(sender, args) {
                if ($("#ErrorHolder").is(':visible')) {
                    // Controllo se ci sono ancora file in errore
                    var upload = $find("<%= AsyncUploadDocument.ClientID%>");
                    var files = upload.getUploadedFiles();
                    var i = files.length;
                    while (i--) {
                        if (isBlackListed(files[i])) {
                            return;
                        }
                    }
                    // Se sono tutti file validi nascondo il messaggio di errore
                    hideErrors();
                }
            }--%>
            
            function hideErrors() {
                $("#ErrorHolder").empty();
                $("#ErrorHolder").hide();
            }
        </script>
    </telerik:RadScriptBlock>

    <div id="ErrorHolder" class="warningArea" style="display: none;"></div>

    <div style="background-color:aliceblue !important;width:100%;height:100%;margin:0px;padding:0px;">
     <div id="SearchDocumentLibrary" style="background-color:gainsboro !important;width:100%;height: 35px;border-bottom: 1px solid #808080;">
         <div style="padding: 5px;">
             <telerik:RadDropDownList ID="rddDocumentLibrary" runat="server" Width="350px" />
             <telerik:RadButton ID="btnSearch" Style="margin-left: 10px;" runat="server" Text="Apri" />
         </div>
     </div>

    <div id="NavigationDocumentLibrary" style="width:100%;height:80%;margin-top: 15px;">       
         <div id="TreeviewFolder"  style="float:left;width:25%;border-right: 1px solid #808080;height:100%;">
             <telerik:RadTreeView  ID="RadTreeViewFolder" runat="server" ></telerik:RadTreeView>
        </div>
        <div id="GridFiles" style="float:right;width:70%;height:100%;margin-bottom:1%;margin-right:1%">        
                <telerik:RadGrid  style="height:100%;width:100%;" AllowFilteringByColumn="False"  AllowMultiRowSelection="false" AutoGenerateColumns="False" GridLines="Both" ShowGroupPanel="False" ID="dgSharepointFiles"  runat="server">
                <MasterTableView  CommandItemDisplay="None" CurrentResetPageIndexAction="SetPageIndexToFirst" ShowHeadersWhenNoRecords="false"  NoMasterRecordsText="Nessun File" TableLayout="Fixed">
                <Columns>
                <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn" HeaderStyle-Width="25px">
                </telerik:GridClientSelectColumn>
                    <telerik:GridTemplateColumn UniqueName="FileType" HeaderText="" Groupable="false" AllowSorting="True" HeaderStyle-Width="35px">
                    <HeaderStyle HorizontalAlign="Center" Wrap="false"/>
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Image ID="imgFile" runat="server" />        
                    </ItemTemplate>
                    </telerik:GridTemplateColumn>
                     <telerik:GridTemplateColumn UniqueName="PathFileName" Visible="false" HeaderText="Nome File" DataField="FileName" SortExpression="FileName" AllowSorting="True" GroupByExpression="FileName Group By FileName">
                        <HeaderStyle HorizontalAlign="Center" Wrap="false"  />
                        <ItemStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:Label ID="lblPathFileName" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="FileName" HeaderText="Nome File" DataField="FileName" SortExpression="FileName" AllowSorting="True" GroupByExpression="FileName Group By FileName">
                        <HeaderStyle HorizontalAlign="Center" Wrap="false"  />
                        <ItemStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:Label ID="lblFileName" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>

                    <telerik:GridTemplateColumn DataField="FileModify" HeaderText="Modificato Il" UniqueName="FileModify" SortExpression="FileModify" AllowSorting="True" ShowSortIcon="True" GroupByExpression="FileModify Group By FileModify">
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                             <asp:Label ID="lblFileModify" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        <ClientSettings EnablePostBackOnRowClick="true">
            <Selecting AllowRowSelect="True" CellSelectionMode="None" UseClientSelectColumnOnly="false" EnableDragToSelectRows="False" />
        </ClientSettings>
            <SortingSettings SortedAscToolTip="Ordine Crescente" SortedDescToolTip="Ordine Descrescente" SortToolTip="Ordina" />
            <GroupingSettings ShowUnGroupButton="true" UnGroupButtonTooltip="Rimuovi" />
        </telerik:RadGrid>   
      </div>
    </div>
    </div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="cmdConfirm" runat="server" Text="Conferma" Width="120px" OnClientClick="purgeBlackListed(); return submitPage();" />
</asp:Content>
