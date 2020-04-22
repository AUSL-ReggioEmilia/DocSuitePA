<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscContattiSel.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscContattiSel" %>

<style type="text/css">
        .RadWindow .rwDialogInput {
            width: 800px;
        }
    </style>
<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
    <script type="text/javascript">

        var <%= Me.ClientID %>_uscContattiSel;
        require(["UserControl/uscContattiSel"], function (UscContattiSel) {
            $(function () {
                <%= Me.ClientID %>_uscContattiSel = new UscContattiSel();
                <%= Me.ClientID %>_uscContattiSel.contentId = "<%=TableContent.ClientID%>";
                <%= Me.ClientID %>_uscContattiSel.validationId = "<%=TreeValidator.ClientID%>";
                <%= Me.ClientID %>_uscContattiSel.currentUserContact = <%=CurrentUserContact%>;
                <%= Me.ClientID %>_uscContattiSel.initialize();
            });
        });

        function <%= Me.ID %>_ManualMulti() {
            var prompt = radprompt("Inserisci indirizzi", <%= Me.ID %>_ManualMultiCallBackFn);
            prompt.set_width(800);
            prompt.set_maxWidth(800);
            prompt.set_title("Inserimento contatti manuali");
            prompt.set_autoSize(false);
            prompt.set_initialBehaviors(4);
            prompt.center();
        }

        function <%= Me.ID %>_ManualMultiCallBackFn(arg) {

            if (arg) {
                $find("<%= AjaxManager.ClientID%>").ajaxRequest("<%= Me.ClientID %>" + "|ManualMulti|" + arg);
            }
        }

        function <%= Me.ID %>_OpenWindow(url, name, width, height, closeFunction, restrictionZoneId) {
            var wnd = window.radopen(url, name);
            if (width != '') {
                wnd.setSize(width, height);
            }
           
            if (restrictionZoneId != null && restrictionZoneId != "") {

                wnd.set_restrictionZoneID(restrictionZoneId);
            }
            if (closeFunction != "") {
                wnd.add_close(closeFunction);
            }

            wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Resize + Telerik.Web.UI.WindowBehaviors.Close + Telerik.Web.UI.WindowBehaviors.Maximize + Telerik.Web.UI.WindowBehaviors.Minimize)
            wnd.set_destroyOnClose(true);
            wnd.set_modal(true);
            wnd.center();

            return false;
        }

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

            var restrictionZoneId = "<%= RestrictionZoneId%>";
            if (restrictionZoneId !== null && restrictionZoneId !== "") {
                wnd.set_restrictionZoneID(restrictionZoneId);
            }

            wnd.add_close(closeFunction);

            return false;
        }

        function <%= Me.ID%>_OpenWindowOLD(url, name, closeFunction) {
            var wnd;
            if (name === "windowSelContactManualSimpleMode" ) {
                wnd = window.DSWOpenGenericWindow(url, window.WindowTypeEnum.SMALL);
            } else if (name === "windowImportContact") {
                wnd = window.DSWOpenGenericWindow(url, window.WindowTypeEnum.DOCUMENTS);
            } else {
                wnd = window.DSWOpenGenericWindow(url, window.WindowTypeEnum.NORMAL);
            }

            var restrictionZoneId = "<%= RestrictionZoneId%>";
            if (restrictionZoneId !== null && restrictionZoneId !== "") {
                wnd.set_restrictionZoneID(restrictionZoneId);
            }

            wnd.add_close(closeFunction);
            wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close + Telerik.Web.UI.WindowBehaviors.Maximize )
 
            return false;
        }

        function <%= Me.ID%>_OpenWindowSmart(url, closeFunction) {
            var wnd = window.DSWOpenGenericWindow(url, window.WindowTypeEnum.NORMAL);
            wnd.center();

            var restrictionZoneId = "<%= RestrictionZoneId%>";
            if (restrictionZoneId !== null && restrictionZoneId !== "") {
                wnd.set_restrictionZoneID(restrictionZoneId);
            }

            wnd.add_close(closeFunction);
            wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close + Telerik.Web.UI.WindowBehaviors.Maximize);
 
            return false;
        }

        //richiamata quando viene selezionato conferma e nuovoe la finestra rubrica NON viene chiusa
        function  <%= Me.ID %>_AddUsersToControl(value) {
            if (value !== null) {
                <%= Me.ID %>_UpdateManual(value, 'InsUsers');
            }
        }

        //richiamata quando la finestra rubrica viene chiusa
        function <%= Me.ID %>_CloseFunction(sender, args) {
            sender.remove_close(<%= Me.ID %>_CloseFunction);
            if (args.get_argument() !== null) {
                <%= Me.ID %>_Update(args.get_argument());
            }
        }

        //richiamata quando la finestra Importazione contatti viene chiusa
        function <%= Me.ID %>_CloseImportFunction(sender, args) {
            sender.remove_close(<%= Me.ID %>_CloseImportFunction);
            if (args.get_argument() !== null) {
                <%= Me.ID %>_UpdateImport(returnValue);
            }
        }

        //richiamata quando la finestra Importazione contatti viene chiusa
        function <%= Me.ID %>_CloseImportManualFunction(sender, args) {
            sender.remove_close(<%= Me.ID %>_CloseImportManualFunction);
            if (args.get_argument() !== null) {
                <%= Me.ID %>_UpdateImportManual(args.get_argument());
            }
        }

        //richiamata quando la finestra contatti manuali viene chiusa
        function <%= Me.ID %>_CloseManualFunction(sender, args) {
            sender.remove_close(<%= Me.ID %>_CloseManualFunction);
            if (args.get_argument() !== null) {
                <%= Me.ID %>_UpdateManual(args.get_argument().Contact, args.get_argument().Action);
            }
        }

        //richiamata quando la finestra contatti AD viene Chiusa
        function  <%= Me.ID %>_CloseDomain(sender, args) {
            sender.remove_close(<%= Me.ID %>_CloseDomain);
            if (args.get_argument() !== null) {
                <%= Me.ID %>_UpdateManual(args.get_argument(), 'Ins');
            }
        }

        function  <%= Me.ID %>_CloseManualMulti(sender, args) {
            sender.remove_close(<%= Me.ID %>_CloseDomain);
            if (args.get_argument() !== null) {
                <%= Me.ID %>_UpdateManual(args.get_argument(), 'ContactAD');
            }
        }

        //richiamata quando la finestra contatti manuali viene chiusa
        function <%= Me.ID %>_CloseIPAFunction(sender, args) {
            sender.remove_close(<%= Me.ID %>_CloseIPAFunction);
            if (args.get_argument() !== null) {
                <%= Me.ID %>_UpdateManual(args.get_argument().Contact, args.get_argument().Action);
            }
        }

        //richiamata quando la finestra contatti manager viene chiusa
        function <%= Me.ID %>_CloseRole(sender, args) {
            sender.remove_close(<%= Me.ID %>_CloseRole);
            if (args.get_argument() !== null) {
                <%= Me.ID %>_UpdateManual(args.get_argument(), 'Ins');
            }
        }
        //richiamata quando la finestra contatti OChart viene chiusa

        function <%= Me.ID %>_CloseOChart(sender, args) {
            sender.remove_close(<%= Me.ID %>_CloseOChart);
            if (args.get_argument() !== null) {
                <%= Me.ID %>_UpdateManual(args.get_argument().Contact, args.get_argument().Action);
            }
        }
        
        function <%= Me.ID %>_CloseSmart(sender, args) {
            sender.remove_close(<%= Me.ID %>_CloseSmart);
            if (args.get_argument() !== null) {
                <%= Me.ID %>_UpdateSmart(args.get_argument().contactType, args.get_argument().contact);
            }
        }

        <%--//////////////////////////////////////////////////////////
        //                FUNZIONI DI AGGIORNAMENTO                 //
        //////////////////////////////////////////////////////////--%>
    
        //esegue l'aggiornamento della lista dei contatti
        function <%= Me.ID %>_Update(value) {
            $find("<%= AjaxManager.ClientID %>").ajaxRequest("<%= Me.ClientID %>" + "|Update|" + value);
        }

        //esegue l'aggiornamento della lista contatti da XML
        function <%= Me.ID %>_UpdateImport(file) {
            $find("<%= AjaxManager.ClientID %>").ajaxRequest("<%= Me.ClientID %>" + "|Import|" + file);
        }

        //esegue l'aggiornamento della lista contatti da Excel
        function <%= Me.ID %>_UpdateImportManual(file) {
            $find("<%= AjaxManager.ClientID %>").ajaxRequest("<%= Me.ClientID %>" + "|ImportExcel|" + file);
        }

        //esegue l'aggiornamento della lista contatti manuali
        function <%= Me.ID %>_UpdateManual(contact, action) {
            //restituito un oggetto javascript serializzato con JSon
            $find("<%= AjaxManager.ClientID %>").ajaxRequest("<%= Me.ClientID %>" + "|" + action + "|" + contact);
        }

        //esegue l'aggiornamento della lista contatti manuali
        function <%= Me.ID %>_UpdateSmart(contactType, contact) {
            if (contactType == "Rubrica"){
                <%= Me.ID %>_Update(JSON.parse(contact).Id);
            } else {
                <%= Me.ID %>_UpdateManual(contact, 'Ins');
            }
        }

        <%--//////////////////////////////////////////////////////////
        //                FUNZIONI DI UTILITA'                      //
        //////////////////////////////////////////////////////////--%>
        
        function <%= Me.ID %>_DeleteAllContacts() {
            if (confirm("Eliminare tutti i contatti?")) {
                $find("<%= AjaxManager.ClientID %>").ajaxRequest("<%= Me.ClientID %>" + "|DeleteAll|");
            }
        }

        // Evento che permette di fare il Check su un solo nodo
        function <%= Me.ID %>_ClientNodeChecking(sender, eventArgs) {
            var node = eventArgs.get_node();
            var checked = !node.get_checked();
            if (checked) {
                var nodes = node.get_parent().get_nodes();
                for (var i = 0; i < nodes.get_count(); i++) {
                    nodes.getNode(i).set_checked(false);
                }
            }
        }

    </script>
</telerik:RadScriptBlock>

<telerik:RadWindowManager EnableViewState="false" ID="RadWindowManagerContacts" runat="server">
    <Windows>
        <telerik:RadWindow ID="windowSelContact" ReloadOnShow="true" runat="server" Title="Selezione Contatto" Behaviors="Maximize,Resize,Minimize,Close" />
    </Windows>
</telerik:RadWindowManager>

<table class="datatable" style="min-width: 200px;" id="tableId" runat="server">
    <tr>
        <th colspan="2" id="tblHeader" runat="server">
            <asp:Panel runat="server" ID="pnlIntestazione">
                <asp:Label ID="lblCaption" runat="server" Text="Contatti" />
                <asp:Label CssClass="contactCount" ID="lblCount" runat="server" />
                <asp:ImageButton CausesValidation="False" ID="btnContactMaxItems" runat="server" Visible="false" />
                <asp:CheckBox Checked="true" CssClass="contactCheckCopia" Enabled="false" ID="chkCopia" runat="server" Text="Copia conoscenza" TextAlign="Left" />
            </asp:Panel>
        </th>
    </tr>
    <tr class="Chiaro">
        <td class="DXChiaro">
            <telerik:RadTreeView ID="RadTreeContact" runat="server" Width="100%">
                <Nodes>
                    <telerik:RadTreeNode Expanded="true" Font-Bold="true" runat="server" Text="Contatti" Value="Root" />
                </Nodes>
            </telerik:RadTreeView>
        </td>
        <td style="width:10%">
            <div style="text-align: right; white-space: nowrap;">
                <asp:Panel runat="server" ID="panelOnlyContact" Visible="False">
                    <div style="float: right;">
                        <asp:ImageButton CausesValidation="False" ID="btnSelContact2" ImageUrl="~/App_Themes/DocSuite2008/imgset16/folder.png" runat="server" />
                    </div>
                </asp:Panel>
                <asp:Panel runat="server" ID="panelButtons" HorizontalAlign="Right">
                    <asp:ImageButton CausesValidation="False" ID="btnAddMyself" Visible="False" ImageUrl="~/App_Themes/DocSuite2008/imgset16/user_add.png" runat="server" />
                    <asp:ImageButton CausesValidation="False" ID="btnAddSdiContact" Visible="False" ImageUrl="~/Comm/Images/Loop16.gif" runat="server" />
                    <asp:ImageButton CausesValidation="False" ID="btnSelContactDomain" ImageUrl="~/App_Themes/DocSuite2008/imgset16/user.png" runat="server" />
                    <asp:ImageButton CausesValidation="false" ID="ButtonSelContactOChart" runat="server" ToolTip="Selezione contatto da organigramma" />
                    <asp:ImageButton CausesValidation="False" ID="btnSelContact" ImageUrl="~/App_Themes/DocSuite2008/imgset16/folder.png" runat="server" />
                    <asp:ImageButton CausesValidation="False" ID="btnAddManualMulti" Visible="False" ImageUrl="~/App_Themes/DocSuite2008/imgset16/pencil_add.png" runat="server" />
                    <asp:ImageButton CausesValidation="False" ID="btnAddManual" ImageUrl="~/App_Themes/DocSuite2008/imgset16/pencil.png" runat="server" />
                    <asp:ImageButton CausesValidation="False" ID="btnContactSmart" ImageUrl="~/App_Themes/DocSuite2008/imgset16/account-circle.png" runat="server" ToolTip="Rubrica smart" Visible="false" />
                    <asp:ImageButton CausesValidation="False" ID="btnRoleUser" ImageUrl="~/App_Themes/DocSuite2008/imgset16/brick_add.png" runat="server" Visible="False" />
                    <asp:ImageButton CausesValidation="False" ID="btnIPAContact" ImageUrl="~/Comm/Images/Interop/Building.gif" runat="server" Visible="False" />
                    <asp:ImageButton CausesValidation="False" ID="btnDelContact" ImageUrl="~/App_Themes/DocSuite2008/imgset16/delete.png" runat="server" />
                    <asp:ImageButton CausesValidation="False" ID="cmdDetails" ImageUrl="~/App_Themes/DocSuite2008/imgset16/user_info.png" runat="server" />
                    <asp:ImageButton CausesValidation="False" ID="btnImportContact" runat="server" />
                    <asp:ImageButton CausesValidation="False" ID="btnImportContactManual" ImageUrl="~/App_Themes/DocSuite2008/imgset16/FromExcel.png" runat="server" Visible="false" />
                </asp:Panel>
            </div>
        </td>
    </tr>
</table>

<asp:CustomValidator ClientValidationFunction="anyNodeButRootCheck" ControlToValidate="RadTreeContact" Display="Dynamic" EnableClientScript="true" ErrorMessage="Campo contatti obbligatorio" ID="TreeValidator" OnServerValidate="TreeValidator_ServerValidate" runat="server" ValidateEmptyText="true" />