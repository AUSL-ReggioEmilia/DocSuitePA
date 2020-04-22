<%@ Control AutoEventWireup="false" CodeBehind="MailSenderControl.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.MailSenders.MailSenderControl" Language="vb" %>
<%@ Register TagPrefix="usc" TagName="SelContatti" Src="~/UserControl/uscContattiSel.ascx" %>
<%@ Register TagPrefix="usc" TagName="SelSettori" Src="~/UserControl/uscSettori.ascx" %>
<%@ Register TagPrefix="usc" TagName="UploadDocument" Src="~/UserControl/uscDocumentUpload.ascx" %>

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

        function openConfirmWindow() {
            radalert('...Inserito correttamente nella coda di invio.', 300, 100, 'Info', alertCallBackFn, "../App_Themes/DocSuite2008/imgset32/information.png");
        }

        var rtbInstance;

        function saveInstance(sender, args) {
            rtbInstance = sender;
        }

        function countChars() {
            window.setTimeout(function () {
                $get("countChars").innerHTML = rtbInstance.get_textBoxValue().length;
            }, 1);
        }

        function alertCallBackFn() {
            // Metodo per il lancio di una Request
            var manager = $find("<%= RadAjaxManager.GetCurrent(Page).ClientID%>");
            manager.ajaxRequest("MailSender_Confirm");
        }
    </script>
</telerik:RadScriptBlock>

<asp:Panel runat="server" ID="WarningPanel" CssClass="hiddenField">
    <asp:Label ID="WarningLabel" runat="server" />
</asp:Panel>

<telerik:RadToolBar AutoPostBack="False" CssClass="ToolBarContainer" EnableRoundedCorners="False" EnableShadows="False" ID="ToolBar" runat="server">
    <Items>
        <telerik:RadToolBarButton ImageUrl="../App_Themes/DocSuite2008/imgset16/mail_send.png" Text="Invia" ToolTip="Invia" Value="MailSender_Send" />
        <telerik:RadToolBarButton ImageUrl="../App_Themes/DocSuite2008/imgset16/cancel.png" Text="Annulla" ToolTip="Annulla" Value="MailSender_Cancel" />
        <telerik:RadToolBarButton
            Checked="True" CheckOnClick="true" CommandName="needDisposition" ToolTip="Abilita la notifica via mail della di ricezione e lettura" Value="needDisposition">
            <ItemTemplate>
                <telerik:RadButton runat="server" ID="chkNeedDisposition" ToggleType="CheckBox" ButtonType="StandardButton" Checked="False" AutoPostBack="false">
                    <ToggleStates>
                        <telerik:RadButtonToggleState Text="Richiedi conferma di recapito e lettura" PrimaryIconCssClass="rbToggleCheckboxChecked" />
                        <telerik:RadButtonToggleState Text="Richiedi conferma di recapito e lettura" PrimaryIconCssClass="rbToggleCheckbox" />
                    </ToggleStates>
                </telerik:RadButton>
            </ItemTemplate>
        </telerik:RadToolBarButton>
        <telerik:RadToolBarButton
            Checked="True" CheckOnClick="true" CommandName="compressedDocuments" ToolTip="Creare file compressi (ZIP)">
            <ItemTemplate>
                <telerik:RadButton runat="server" ID="chkCompressedDocuments" ToggleType="CheckBox" ButtonType="StandardButton" Checked="False" AutoPostBack="false">
                    <ToggleStates>
                        <telerik:RadButtonToggleState Text="Comprimi i file" PrimaryIconCssClass="rbToggleCheckboxChecked" />
                        <telerik:RadButtonToggleState Text="Comprimi i file" PrimaryIconCssClass="rbToggleCheckbox" />
                    </ToggleStates>
                </telerik:RadButton>
            </ItemTemplate>
        </telerik:RadToolBarButton>
        <telerik:RadToolBarButton
            Checked="True" CheckOnClick="true" CommandName="securedDocuments" ToolTip="Abilita l'invio via mail del docomenti selezioni con ZIP protetto da password">
            <ItemTemplate>
                <telerik:RadButton runat="server" OnClick="chkSecuredDocumentsClick" ID="chkSecuredDocuments" ToggleType="CheckBox" ButtonType="StandardButton" Checked="False" AutoPostBack="true">
                    <ToggleStates>
                        <telerik:RadButtonToggleState Text="Proteggi i file" PrimaryIconCssClass="rbToggleCheckboxChecked" />
                        <telerik:RadButtonToggleState Text="Proteggi i file" PrimaryIconCssClass="rbToggleCheckbox" />
                    </ToggleStates>
                </telerik:RadButton>
            </ItemTemplate>
        </telerik:RadToolBarButton>
    </Items>
</telerik:RadToolBar>


<asp:Panel runat="server" ID="messageTable">
    <%--mittente--%>
    <table class="datatable">
        <tr>
            <th>Mittente</th>
        </tr>
        <tr>
            <td class="DXChiaro">
                <asp:Label runat="server" ID="SenderDescription" />&nbsp;&lt;<asp:Label runat="server" ID="SenderEmail" />&gt;
            </td>
        </tr>
    </table>
    <%-- destinatari --%>
    <input type="hidden" runat="server" id="hf_MessageRecipientPosition" />
    <usc:SelContatti ButtonImportManualVisible="true" Caption="Destinatari" EnableCC="false"
        HeaderVisible="true" ID="MessageRecipients" IsRequired="false" Multiple="true" MultiSelect="true"
        ProtType="True" RequiredErrorMessage="Destinatario obbligatorio" runat="server"
        SimpleMode="true" TreeViewCaption="Destinatari" ButtonManualMultiVisible="True" ButtonAddMyselfVisible="true" />

    <asp:Panel runat="server" ID="panelSettori">
        <%-- Invia per conto del settore --%>
        <usc:SelSettori Caption="Invia per conto del settore :" HeaderVisible="True" ID="uscSettori" MultipleRoles="True" MultiSelect="True"
            Required="false" RoleRestictions="OnlyMine" runat="server" Type="Prot" />
    </asp:Panel>

    <%-- allegati --%>
    <asp:Panel runat="server" ID="DocumentsPanel">
        <table class="datatable">
            <tr>
                <th>Allegati</th>
            </tr>
            <tr>
                <td>
                    <telerik:RadGrid runat="server" ID="DocumentListGrid" Width="100%" AllowMultiRowSelection="true" EnableViewState="true">
                        <MasterTableView AutoGenerateColumns="False" DataKeyNames="Serialized">
                            <Columns>
                                <telerik:GridTemplateColumn HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/documentPreview.png" HeaderText="Tipo Documento">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="documentType" runat="server" CommandName="preview" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/document_empty.png" HeaderText="Documenti Originali" UniqueName="original">
                                    <ItemTemplate>
                                        <asp:CheckBox runat="server" ID="chkOriginal" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/file_extension_pdf.png" HeaderText="Documenti Copia conforme" UniqueName="pdf">
                                    <ItemTemplate>
                                        <asp:CheckBox runat="server" ID="chkPdf" />
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
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlAttachment">
        <table class="datatable">
            <tr>
                <th>Altri allegati</th>
            </tr>
            <tr>
                <td>
                    <usc:UploadDocument TreeViewCaption="Documenti" HeaderVisible="false" ID="uscAttachment" IsDocumentRequired="false" MultipleDocuments="true" runat="server" ShowDocumentsSize="true" ShowTotalSize="True" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <%-- oggetto --%>
    <table class="datatable">
        <tr>
            <th>Oggetto</th>
        </tr>
        <tr>
            <td>
                <span id="countChars"></span>/<asp:Label runat="server" ID="maxChars"></asp:Label>&nbsp;caratteri.
            </td>
        </tr>
        <tr>
            <td>
                <telerik:RadTextBox ID="Subject" TextMode="MultiLine" onkeydown="countChars()" Rows="3" runat="server" Width="100%" Height="50px">
                    <ClientEvents OnValueChanged="countChars()" OnLoad="saveInstance" />
                </telerik:RadTextBox>
            </td>
        </tr>
    </table>
    <%-- messaggio --%>
    <table class="datatable">
        <tr>
            <th>Messaggio</th>
        </tr>
        <tr>
            <td>
                <telerik:RadEditor runat="server" ID="Body" EditModes="Design" EnableResize="true" AutoResizeHeight="true" Width="100%" Height="100%" EmptyMessage="Inserire qui il testo">
                    <Tools>
                        <telerik:EditorToolGroup>
                            <telerik:EditorTool Name="Bold"></telerik:EditorTool>
                            <telerik:EditorTool Name="Italic"></telerik:EditorTool>
                            <telerik:EditorTool Name="Underline"></telerik:EditorTool>
                            <telerik:EditorTool Name="Cut"></telerik:EditorTool>
                            <telerik:EditorTool Name="Copy"></telerik:EditorTool>
                            <telerik:EditorTool Name="Paste"></telerik:EditorTool>
                        </telerik:EditorToolGroup>
                        <telerik:EditorToolGroup>
                            <telerik:EditorTool Name="JustifyLeft"></telerik:EditorTool>
                            <telerik:EditorTool Name="JustifyCenter"></telerik:EditorTool>
                            <telerik:EditorTool Name="JustifyRight"></telerik:EditorTool>
                            <telerik:EditorTool Name="JustifyNone"></telerik:EditorTool>
                        </telerik:EditorToolGroup>
                    </Tools>
                </telerik:RadEditor>
            </td>
        </tr>
    </table>
</asp:Panel>
