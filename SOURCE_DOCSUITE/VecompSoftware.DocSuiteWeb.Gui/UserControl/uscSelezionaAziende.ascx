<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscSelezionaAziende.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscSelezionaAziende" %>

<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        window.$ = $telerik.$;
        $(document).ready(function () {//Wait for the DOM
            document.getElementById('<%= lblWarningFilter.ClientID%>').style.display = 'none';
            $("#<%= txtFilterTags.ClientID%>").keyup(function () {
                var search = $.trim($(this).val());
                document.getElementById('<%= lblWarningFilter.ClientID%>').style.display = search.length < 3 ? '' : 'none';
            });
        });

        //richiamata quando la finestra rubrica viene chiusa
        function <%= Me.ID %>_CloseFunction(sender, args) {
            <%--sender.remove_close(<%= Me.ID %>_CloseFunction);
            if (args.get_argument() !== null) {
                <%= Me.ID %>_Update(args.get_argument());
            }--%>
            console.log(sender);
            var contact = args._argument.IdContact + '|' + args._argument.Action + '|' + args._argument.Contact;
            $find("<%= AjaxManager.ClientID %>").ajaxRequest(contact);
        }
        //richiamata quando viene aperta la finestra di inserimento di un nuovo contatto
        function <%= Me.ID%>_OpenWindowOLDFullScreen(url, name, closeFunction) {
            var wnd;
            if (name === "windowSelContactManualSimpleMode") {
                wnd = window.DSWOpenGenericWindow(url, window.WindowTypeEnum.SMALL);
            } else if (name === "windowImportContact") {
                wnd = window.DSWOpenGenericWindow(url, window.WindowTypeEnum.DOCUMENTS);
            } else {
                wnd = window.DSWOpenGenericWindow(url, window.WindowTypeEnum.NORMAL);
            }

            wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Resize + Telerik.Web.UI.WindowBehaviors.Close + Telerik.Web.UI.WindowBehaviors.Maximize + Telerik.Web.UI.WindowBehaviors.Minimize)
            wnd.set_destroyOnClose(true);
            wnd.set_modal(true);
            wnd.center();

            wnd.add_close(closeFunction);

            return false;
        }

    </script>
</telerik:RadScriptBlock>

<telerik:RadWindowManager EnableViewState="false" ID="RadWindowManagerContacts" runat="server">
    <Windows>
        <telerik:RadWindow ID="windowSelContact" ReloadOnShow="true" runat="server" Title="Selezione Contatto" Behaviors="Maximize,Resize,Minimize,Close" />
    </Windows>
</telerik:RadWindowManager>

<div id="blockSearch" runat="server">
    <span style="margin-left: 5px; margin-top: 5px;"></span>
    <telerik:RadButton ID="btnAddContact" runat="server" ToolTip="Inserimento Nuovo Contatto" Width="22px">
        <Icon PrimaryIconUrl="~/App_Themes/DocSuite2008/imgset16/folder.png" PrimaryIconWidth="16" PrimaryIconHeight="16" />
    </telerik:RadButton>
    <telerik:RadButton ID="btnAddCompanyContact" runat="server" ToolTip="Inserimento Nuovo Contatto" Visible="false" Width="22px">
        <Icon PrimaryIconUrl="~/App_Themes/DocSuite2008/imgset16/pencil.png" PrimaryIconWidth="16" PrimaryIconHeight="16" />
    </telerik:RadButton>
    <asp:Label ID="lblFilterName" Style="margin-left: 25px; vertical-align: middle;" runat="server">Filtra: </asp:Label>
    <asp:TextBox ID="txtFilterTags" Width="150px" Style="vertical-align: middle;" runat="server" />
    <telerik:RadButton ID="btnSearch" runat="server" Text="Ricerca" />
    <asp:Label ID="lblWarningFilter" runat="server">Inserire almeno 2 caratteri</asp:Label>
</div>

<div id="pnlAziende" runat="server" style="text-align: center; margin-top: 5px; margin-left: 2px;">
    <div style="display: inline; display: inline-block; zoom: 1;">
        <telerik:RadListBox ID="sourceAziende" runat="server" Width="320px" Height="250"
            SelectionMode="Multiple" AllowTransfer="true" TransferToID="targetAziende" AutoPostBackOnTransfer="true" TransferMode="Move"
            AllowReorder="false" AutoPostBackOnReorder="true" EnableDragAndDrop="true" DataTextField="FullDescription">
        </telerik:RadListBox>
        <telerik:RadListBox ID="targetAziende" runat="server" Width="300px" Height="250" DataKeyField="Id"
            SelectionMode="Multiple" AllowReorder="false" AutoPostBackOnReorder="true" EnableDragAndDrop="true">
        </telerik:RadListBox>
    </div>
</div>

<div style="margin-top: 5px;">
    <div style="vertical-align: middle; display: inline; margin-left: 5px;">
        <telerik:RadButton ID="btnSave" runat="server" Text="Conferma" ToolTip="Salva"></telerik:RadButton>
    </div>
</div>

