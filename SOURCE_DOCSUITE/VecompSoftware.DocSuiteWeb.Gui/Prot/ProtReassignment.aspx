<%@ Page Title="Protocollo - Riassegnazione" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="ProtReassignment.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtReassignment" %>
<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="uscContattiSel" TagPrefix="uc" %>
<%@ Register TagPrefix="usc" TagName="RoleControl" Src="~/Control/RoleControl.ascx" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadCodeBlock runat="server">
        <script language="javascript" type="text/javascript">
            
            function tbRoleControlButtonClicking(sender, args) {
                var btn = args.get_item();
                switch (btn.get_commandName()) {
                    case "ADD":
                        setTimeout(OpenRoleSelector,0);
                        args.set_cancel(true);
                        break;
                }
            }
            
            function OpenRoleSelector() {
                var url = "../UserControl/CommonSelSettori.aspx?<%= GetWindowParameters()%>";
                var wnd = window.radopen(url, "windowSelSettori");
                wnd.setSize(<%= ProtocolEnv.DocumentPreviewWidth%>, <%= ProtocolEnv.DocumentPreviewHeight%>);
                wnd.add_close(RoleSelectorCloseFunction);
                wnd.set_destroyOnClose(true);
                wnd.set_modal(true);
                wnd.center();
            }

            function RoleSelectorCloseFunction(sender, args) {
                if (args.get_argument() !== null) {
                    MyAjaxRequest("ROLES", "ADD", args.get_argument());
                }
            }

            function MyAjaxRequest(sender, action, args) {
                var ajaxManager = $find("<%= AjaxManager.ClientID%>");
                ajaxManager.ajaxRequest(sender + "|"+ action +"|" + args);
            }

        </script>
    </telerik:RadCodeBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <table id="tblProtocollo" class="datatable" runat="server">
        <tr>
            <th colspan="6">
                <asp:Label ID="lbTitle" runat="server" />
            </th>
        </tr>
        <tr>
            <td class="label" style="width: 15%">
                Anno:
            </td>
            <td style="width: 15%">
                <asp:Label ID="lblYear" runat="server" />
            </td>
            <td class="label" style="width: 15%">
                Numero:
            </td>
            <td style="width: 15%">
                <asp:Label ID="lblNumber" runat="server" />
            </td>
            <td class="label" style="width: 15%">
                Data:
            </td>
            <td style="width: 25%; font-weight: bold;">
                <asp:Label ID="lblRegistrationDate" runat="server" />
            </td>
        </tr>
    </table>
    <%-- Rigetto --%>
    <table id="tblReject" class="datatable" runat="server">
        <tr>
            <th>
                Estremi del rigetto
            </th>
        </tr>
        <tr>
            <td>
                <asp:Image ID="imgReject" runat="server" Height="16px" Width="16px" />
                <asp:Label runat="server" ID="lblReject" />
            </td>
        </tr>
    </table>
    <%--Contenitore--%>
	<table id="tblEditContenitore" runat="server" class="datatable">
		<tr>
			<th colspan="2">Contenitore</th>
		</tr>
		<tr>
			<td class="label" style="width:15%;">Contenitore</td>
			<td style="width:85%;">
                <telerik:RadComboBox AutoPostBack="true" CausesValidation="false" EnableLoadOnDemand="true" ID="rcbContainer" ItemRequestTimeout="500" MarkFirstMatch="true" runat="server" Width="300px" />
                &nbsp;
                <asp:RequiredFieldValidator Display="Dynamic" ErrorMessage="Campo contenitore obbligatorio" ID="rfvContainer" ControlToValidate="rcbContainer" runat="server" />
			</td>
		</tr>
    </table>
    <%--Destinatario di protocollo--%>
    <uc:uscContattiSel ID="uscDestinatari" ButtonDeleteVisible="false" ButtonImportVisible="false" ButtonManualVisible="false" ButtonPropertiesVisible="true" ButtonSelectDomainVisible="false" ButtonSelectOChartVisible="false" ButtonSelectVisible="false" Caption="Destinatari" EnableCompression="false" EnableCC="false" IsRequired="false" Multiple="true" MultiSelect="true" ProtType="true" ReadOnlyProperties="true" runat="server" Type="Prot" />
    <%--Autorizzazioni--%>
    <table class="datatable">
        <tr>
            <th>Settori collegati</th>
        </tr>
        <tr>
            <td>
                <telerik:RadToolBar CssClass="ToolBarContainer" EnableRoundedCorners="False" EnableShadows="False" ID="tbRoleControl" OnClientButtonClicking="tbRoleControlButtonClicking" runat="server" Width="100%">
                    <Items>
                        <telerik:RadToolBarButton CommandName="ADD" ImageUrl="~/App_Themes/DocSuite2008/imgset16/brick_add.png" runat="server" ToolTip="Aggiungi Settore Esistente" />
                        <telerik:RadToolBarButton CommandName="REMOVE" ImageUrl="~/App_Themes/DocSuite2008/imgset16/brick_delete.png" runat="server" ToolTip="Elimina Settore Selezionato" />
                    </Items>
                </telerik:RadToolBar>
            </td>
        </tr>
        <tr>
            <td>
                <usc:RoleControl DefaultGroup="" ID="myRolecontrol" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Button runat="server" ID="btnReassign" Text="Riassegna" />
</asp:Content>
