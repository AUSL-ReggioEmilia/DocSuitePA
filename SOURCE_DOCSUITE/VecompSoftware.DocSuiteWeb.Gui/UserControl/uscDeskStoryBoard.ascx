<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscDeskStoryBoard.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDeskStoryBoard" %>
<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        function CloseWindowComment(args) {
            var oWindow = $find("<%= windowAddCommentDocument.ClientID%>");
            oWindow.close();
            $find("<%= AjaxManager.ClientID %>").ajaxRequest(args);
        }

        function <%= Me.ID %>_OpenWindow(name) {
            var wnd = $find(name);
            wnd.set_destroyOnClose(true);
            wnd.show();
            wnd.center();
            return false;
        }

        function OnToolBarClientButtonClicked(sender, args) {
            var button = args.get_item();
            switch (button.get_commandName()) {
                case 'AddComment':
                case 'ViewDocComments':
                case 'ViewAllComments':
                    return $find("<%= AjaxManager.ClientID %>").ajaxRequest(button.get_commandName());
            }
            return false;
        }
    </script>
</telerik:RadScriptBlock>

<telerik:RadWindowManager DestroyOnClose="True" ReloadOnShow="True" ID="RadWindowManagerComment" runat="server">
    <Windows>
        <telerik:RadWindow Behaviors="Close" DestroyOnClose="true" Height="160px" ID="windowAddCommentDocument" runat="server" Title="Aggiungi un nuovo commento" Width="500px">
            <ContentTemplate>
                <asp:Panel runat="server" ID="pnlEditorWindow" Width="100%">
                    <telerik:RadTextBox runat="server" ID="txtComment" MaxLength="500" Width="100%" CssClass="inputCommentText" InputType="Text" TextMode="MultiLine" Rows="4" EmptyMessage="Commento"></telerik:RadTextBox>
                </asp:Panel>
                <div class="window-footer-wrapper">
                    <telerik:RadButton runat="server" ID="btnSaveComment" Text="Conferma" ValidationGroup="editorCommentValidator" />
                </div>
            </ContentTemplate>
        </telerik:RadWindow>
    </Windows>
</telerik:RadWindowManager>

<table class="datatable" style="table-layout: fixed;">
    <tr>
        <th>Lavagna Contributi</th>
    </tr>
    <tr runat="server" id="trStoryBoardToolBar" visible="False">
        <td>
            <telerik:RadToolBar runat="server" ID="storyBoardToolBar" OnClientButtonClicked="OnToolBarClientButtonClicked" EnableRoundedCorners="False" EnableShadows="False">
                <Items>
                    <telerik:RadToolBarButton runat="server" CausesValidation="False" PostBack="False" ToolTip="Aggiungi commento" Text="Nuovo" CommandName="AddComment"></telerik:RadToolBarButton>
                    <telerik:RadToolBarButton IsSeparator="true"></telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Group="ViewDocComments" CausesValidation="False" PostBack="False" ToolTip="Visualizza commenti documenti" Text="Commenti Documenti" CheckOnClick="True" AllowSelfUnCheck="True" Checked="False" CommandName="ViewDocComments"></telerik:RadToolBarButton>
                    <telerik:RadToolBarButton IsSeparator="true"></telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Group="ViewAllComments" CausesValidation="False" PostBack="False" ToolTip="Visualizza tutti i commenti" Text="Visualizza Tutti" CheckOnClick="True" AllowSelfUnCheck="True" Checked="False" CommandName="ViewAllComments"></telerik:RadToolBarButton>
                </Items>
            </telerik:RadToolBar>
        </td>
    </tr>
</table>
<div class="radGridWrapper">
    <DocSuite.WebComponent:BindGrid EnableExportButtons="False" BackColor="White" EnableClearFilterButtons="False" EnableHeaderSection="False" runat="server" ID="dgvStoryBoard" AutoGenerateColumns="False" AllowPaging="True" CellSpacing="0" GridLines="None">
        <MasterTableView DataKeyNames="DeskId" NoMasterRecordsText="Nessun commento presente" ShowFooter="False" ShowHeader="False" TableLayout="Fixed">
            <Columns>
                <telerik:GridTemplateColumn>
                    <ItemTemplate>
                        <asp:Panel runat="server" ID="pnlComment" CssClass="deskStoryBoard">
                            <fieldset>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Image runat="server" ID="imgUser" ImageAlign="Middle" ImageUrl="~/App_Themes/DocSuite2008/imgset32/comment-edit.png" />
                                        </td>
                                        <td>
                                            <div class="deskCommentDocument">
                                                <asp:Label runat="server" ID="lblDeskDocumentRef"></asp:Label>
                                            </div>
                                            <div class="deskCommentAuthor">
                                                <asp:Label runat="server" ID="lblDeskCommentAuthor"></asp:Label>
                                                <asp:Label runat="server" ID="lblCommentDate" CssClass="commentDate"></asp:Label>
                                            </div>
                                            <div class="deskComment">
                                                <asp:Label runat="server" ID="lblDeskComment"></asp:Label>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                        </asp:Panel>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </DocSuite.WebComponent:BindGrid>
</div>
