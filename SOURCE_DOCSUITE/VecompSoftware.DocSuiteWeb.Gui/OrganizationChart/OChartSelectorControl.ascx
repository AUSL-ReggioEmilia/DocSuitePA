<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="OChartSelectorControl.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.OChartSelectorControl" %>

<telerik:RadCodeBlock ID="CodeBlock" runat="server">
    <script type="text/javascript">

        function onToolBarButtonClicked(sender, args) {
            alert(String.format("Button with text: '{0}' clicked", args.get_item().get_text()));
        }

        function openWindowSelector() {
            var oWnd = $find("<%=WindowSelector.ClientID%>");
            oWnd.show();
            return false;
        }
    </script>
</telerik:RadCodeBlock>

<telerik:RadToolBar ID="ToolBar" runat="server" AutoPostBack="true" CssClass="ToolBarContainer" EnableRoundedCorners="False" EnableShadows="False" Width="100%" OnClientButtonClicked="onToolBarButtonClicked">
    <Items>
        <telerik:RadToolBarButton CausesValidation="False" CommandName="add" ImageUrl="~/App_Themes/DocSuite2008/imgset16/brick_add.png" ToolTip="Aggiungi Organigramma" />
        <telerik:RadToolBarButton CausesValidation="False" CommandName="delete" ImageUrl="~/App_Themes/DocSuite2008/imgset16/brick_delete.png" ToolTip="Elimina Organigramma" />
    </Items>
</telerik:RadToolBar>
<telerik:RadTreeView ID="TreeViewSelectedItems" runat="server" />

<telerik:RadWindow runat="server" ID="WindowSelector" Title="Cerca nell'Organigramma" Width="480" Height="140" Behaviors="Close">
    <ContentTemplate>
        <asp:UpdatePanel ID="pnlOChartSelector" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div>
                    <asp:DropDownList ID="DropDownProviders" runat="server" />
                    <asp:TextBox ID="TextBoxFilter" runat="server" />
                    <asp:Button ID="ButtonSearch" runat="server" Text="Cerca" />
                </div>
                <div>
                    <telerik:RadTreeView ID="TreeViewItems" runat="server" />
                </div>
                <div style="text-align: right">
                    <asp:Button ID="ButtonConfirm" runat="server" Text="Conferma" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>
