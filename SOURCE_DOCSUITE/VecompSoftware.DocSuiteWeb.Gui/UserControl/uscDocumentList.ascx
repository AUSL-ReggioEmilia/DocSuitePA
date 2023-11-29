<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscDocumentList.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDocumentList" %>
<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">
        function OpenGenericWindow(url) {
            var wnd = window.radopen(url, null);
            wnd.set_visibleStatusbar(false);
            wnd.set_modal(true);
            wnd.set_showOnTopWhenMaximized(false);
            wnd.set_destroyOnClose(true);
            return wnd;
        }

        function ExecuteAjaxRequest(operationName) {
            $find("<%= RadAjaxManager.GetCurrent(Page).ClientID %>").ajaxRequest(operationName);
        }

    </script>
</telerik:RadScriptBlock>

<telerik:RadAjaxLoadingPanel ID="DefaultLoadingPanel" runat="server">
</telerik:RadAjaxLoadingPanel>

<asp:Panel runat="server" ID="DocumentsPanel">
    <table class="datatable">
        <tr>
            <th>
                <div style="display: flex; align-items: center">
                    <asp:CheckBox Style="margin-right: 25px" Checked="false" ID="chkSelectAllOriginals" runat="server" Text="Seleziona tutti gli originali" OnCheckedChanged="CheckedSelectAllOriginalsChanged" AutoPostBack="True"/>
                    <asp:CheckBox Checked="false" ID="chkSelectAllCertifiedCopies" runat="server" Text="Seleziona tutte le copie conformi" OnCheckedChanged="CheckedSelectAllCertifiedCopiesChanged" AutoPostBack="True"/>
                </div>
                <asp:Label runat="server" ID="DocumentsCaption">
                </asp:Label>
            </th>
        </tr>
        <tr>
            <td>
                <telerik:RadAjaxPanel ID="RadAjaxDefaultLoadingPanel" runat="server" LoadingPanelID="DefaultLoadingPanel">
                    <telerik:RadGrid runat="server" ID="DocumentListGrid" Width="100%" AllowMultiRowSelection="true" EnableViewState="true" EnableColumnsViewState="true">
                        <MasterTableView AutoGenerateColumns="False" DataKeyNames="Item1.Serialized">
                            <Columns>
                                <telerik:GridTemplateColumn HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/documentPreview.png" HeaderText="Tipo Documento">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="documentType" runat="server" CommandName="preview" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/document_empty.png" HeaderText="Documenti Originali" UniqueName="original">
                                    <ItemTemplate>
                                        <asp:CheckBox runat="server" ID="chkOriginal" OnCheckedChanged="CheckChanged" AutoPostBack="True" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/file_extension_pdf.png" HeaderText="Documenti Copia conforme" UniqueName="pdf">
                                    <ItemTemplate>
                                        <asp:CheckBox runat="server" ID="chkPdf" OnCheckedChanged="CheckChanged" AutoPostBack="true" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderStyle-Width="100%" HeaderText="Nome Documento" UniqueName="Description">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblFileName" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                        <ClientSettings EnableRowHoverStyle="False">
                            <Selecting AllowRowSelect="true" UseClientSelectColumnOnly="True" />
                        </ClientSettings>
                    </telerik:RadGrid>
                </telerik:RadAjaxPanel>
            </td>
        </tr>
    </table>
</asp:Panel>
