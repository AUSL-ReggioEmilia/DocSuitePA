<%@ Control AutoEventWireup="false" CodeBehind="uscProtGridBar.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscProtGridBar" Language="vb" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
    <script language="javascript" type="text/javascript">

        function <%= Me.ID %>_OpenWindow(name, _url, parameters) {
            var url = _url;
            url += ("?" + parameters);

            var manager = $find("<%= RadWindowManager.ClientID %>");
            var wnd = manager.open(url, name);
            wnd.center();

            return false;
        }

        function onTaskCompleted(sender, args) {
            var splitted = args.get_argument().split("|");
            if (splitted[0].toString().toLowerCase() == "true") {
                if (splitted[1] * 1 > 0) {
                    var isok = confirm("Si sono verificati " + splitted[1] + " errori durante la fase di esportazione.");
                    if (isok) {
                        var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                        ajaxManager.ajaxRequest("E");
                    }
                } else {
                    alert("Esportazione eseguita correttamente.");
                }
            } else {
                alert("Si sono verificati degli errori durante il processo d\'importazione.");
            }
        }


        // note : all functions prefixed cc come from the same functionality type (MassiveChangeProtocolContainerEnabled )
        // we cannot group them in an object because of the limited functionality of aspx OnClientClicked, here we must pass a global function name

        function ccAnyProtocolsSelected() {
            var gridId = "<%=If(_grid Is Nothing, String.Empty, _grid.ClientID)%>";
            if (gridId === "") {
                return false;
            }

            return !!($.grep($("#" + gridId + " input[type=checkbox]"), function (value, index) {
                return $(value).prop("checked");
            }).length);
        }

        function ccShowContainerChangeDialog() {
            var wnd = $find("<%=modalPopup.ClientID %>");
            $hfSelectedContainer = $("#<%=hfSelectedContainer.ClientID%>");
            $ddlContainers = $("#<%=ddlContainers.ClientID %>")
            wnd.show();
        }

        function ccChangeContainerSelectionChanged(sender, evArgs) {
            $hiddenField = $("#<%=hfSelectedContainer.ClientID%>");
            var selectedItem = evArgs.get_item();
            $hiddenField.val(selectedItem.get_value())
        }

        function ccConfimChangeContainer() {
            //get container id from hidden field
            var containerId = $("#<%=hfSelectedContainer.ClientID%>").val();

            //send ajax command
            var ajaxManager = $find("<%= AjaxManager.ClientID %>");
            ajaxManager.ajaxRequest("updateProtocolContainer," + containerId);

            //close
            $find('<%=modalPopup.ClientID %>').close();
        }

        function ccUpdateChangeClassificationState() {
            $("#<%=btnChangeContainer.ClientID%>").prop("disabled", !ccAnyProtocolsSelected())
        }

        function ccAddCheckboxEvents() {
            //preset this to false. Do not change from backend :
            //- when we set btnChangeContainer.Enabled=False, webforms saves a staste for the btn
            //- Scenario1 : attachking checkbox clientchange events does not work from webforms
            //- Scenario2 : updatepanels or updatecontrols do not work
            //- Scenario3 : sending ajax requests for each checkbox.click to change btnChangeContainer.Enabled state is too heavy
            //- Scenario4 : setting btnChangeContainer.Enabled=False from backend and handling btn.Disabled from JS, will lead to an inconsistend
            //state and when we click changeContainerBtn to open window, state page refreshes at initial state

            var gridId = "<%=If(_grid Is Nothing, String.Empty, _grid.ClientID)%>";
            if (gridId === "") {
                return;
            }
            this.ccUpdateChangeClassificationState();

            $("#" + gridId + " input[type=checkbox]").bind("click", function () {
                if (ccAnyProtocolsSelected()) {
                    $("#<%=btnChangeContainer.ClientID%>").prop("disabled", false)
                } else {
                    $("#<%=btnChangeContainer.ClientID%>").prop("disabled", true)
                }
            })
        }

    </script>

</telerik:RadScriptBlock>

<telerik:RadWindowManager EnableViewState="false" ID="RadWindowManager" runat="server">
    <windows>
        <telerik:RadWindow Height="400" ID="windowExportError" ReloadOnShow="false" runat="server" Width="650" />
        <telerik:RadWindow Behaviors="None" Height="550" ID="wndProgress" runat="server" Width="700" />
        <telerik:RadWindow ID="modalPopup" runat="server" Width="360px" Height="100px" Modal="true"
            Title="Cambia Contenitore" Behaviors="Close">
            <ContentTemplate>
                <div class="cfgContent qsfClear">
                    <asp:HiddenField ID="hfSelectedContainer" runat="server" />
                    <label for="ddlContainers">Seleziona nuovo contenitore</label>
                    <telerik:RadComboBox runat="server" ID="ddlContainers" AutoPostBack="False" OnClientSelectedIndexChanged="ccChangeContainerSelectionChanged" />
                    <input type="button" value="Conferma" onclick="ccConfimChangeContainer()" />
                </div>
            </ContentTemplate>
        </telerik:RadWindow>
    </windows>
</telerik:RadWindowManager>
<asp:Panel runat="server" ID="pnlGridBar">
    <telerik:RadButton ID="btnExport" runat="server" Text="Esporta Documenti" Visible="False" Width="120px" />
    <telerik:RadButton ID="btnDocuments" runat="server" Text="Visualizza documenti" Visible="False" Width="130px" />
    <telerik:RadButton ID="btnStampa" runat="server" Text="Stampa selezione" Visible="False" Width="120px" />
    <telerik:RadButton ID="btnSelectAll" runat="server" Text="Seleziona tutti" Visible="False" Width="120px" />
    <telerik:RadButton ID="btnDeselectAll" runat="server" Text="Annulla selezione" Visible="False" Width="120px" />
    <telerik:RadButton ID="btnSetRead" runat="server" Text="Segna come letti" Visible="False" Width="120px" />
    <telerik:RadButton ID="btnAssign" runat="server" Text="Assegna" Visible="False" Width="120px" />
    <telerik:RadButton ID="btnChangeContainer" runat="server" Text="Cambia Contenitore" Visible="False" Width="120px" disabled="disabled" />
</asp:Panel>
