<%@ Page Title="Collaborazione - inserimento in Archivio" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="CollaborationToUDS.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CollaborationToUDS" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            function <%= Me.ID %>_OpenWindow(url, name, parameters) {
                var wnd = $find(name);
                wnd.setUrl(url + "?" + parameters);
                wnd.show();
                wnd.center();
                return false;
            }
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager DestroyOnClose="True" ReloadOnShow="True" ID="RadWindowManagerDocument" runat="server">
        <Windows>
            <telerik:RadWindow Behaviors="Maximize,Close,Resize,Reload" DestroyOnClose="false" ID="windowPreviewDocument" ReloadOnShow="true" runat="server" Title="Anteprima documento" />
        </Windows>
    </telerik:RadWindowManager>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel runat="server" ID="pnlPageContent">
        <asp:Panel runat="server" ID="pnlSeries">
            <table class="datatable">
                <tr>
                    <th colspan="2">Archivi</th>
                </tr>
                <tr>
                    <td class="label col-dsw-2" style="vertical-align: middle;">
                        Seleziona un archivio:
                    </td>
                    <td class="col-dsw-8">
                        <telerik:RadDropDownList runat="server" ID="ddlUDSs" AutoPostBack="True" Width="300px" CausesValidation="False"></telerik:RadDropDownList>
                        <asp:RequiredFieldValidator ValidationGroup="udsValidation" runat="server" ID="ddlSeriesValidator" Display="Dynamic" ControlToValidate="ddlUDSs" ErrorMessage="Il campo è obbligatorio"></asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlCollaborationMetaData">
            <table class="datatable">
                <tr>
                    <th colspan="2">Dati Collaborazione</th>
                </tr>
                <tr>
                    <td class="label col-dsw-2" style="vertical-align: middle;">Oggetto:
                    </td>
                    <td class="col-dsw-8">
                        <asp:Label runat="server" ID="lblCollaborationSubject"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="label col-dsw-2" style="vertical-align: middle;">Note:
                    </td>
                    <td class="col-dsw-8">
                        <asp:Label runat="server" ID="lblNote"></asp:Label>
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlCollaborationDocuments">
            <table class="datatable">
                <tr>
                    <th>Documenti Collaborazione</th>
                </tr>
            </table>
            <div class="radGridWrapper">
                <telerik:RadGrid runat="server" ID="dgvDocuments" Width="100%" AllowMultiRowSelection="true">
                    <GroupingSettings ShowUnGroupButton="true"></GroupingSettings>
                    <MasterTableView AutoGenerateColumns="False" TableLayout="Auto" DataKeyNames="BiblosSerializeKey">
                        <GroupByExpressions>
                            <telerik:GridGroupByExpression>
                                <SelectFields>
                                    <telerik:GridGroupByField FieldAlias="Tipologia" FieldName="VersioningDocumentGroup" HeaderText=""></telerik:GridGroupByField>
                                </SelectFields>
                                <GroupByFields>
                                    <telerik:GridGroupByField FieldName="VersioningDocumentGroup" SortOrder="Descending"></telerik:GridGroupByField>
                                </GroupByFields>
                            </telerik:GridGroupByExpression>
                        </GroupByExpressions>
                        <Columns>
                            <telerik:GridClientSelectColumn HeaderStyle-Width="16px" UniqueName="Select" />

                            <telerik:GridTemplateColumn UniqueName="RadioButtonTemplateColumn" HeaderStyle-Width="16px" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/documentPreview.png" HeaderText="Visualizza documento">
                                <ItemTemplate>
                                    <asp:ImageButton ID="imgDocumentExtensionType" runat="server" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>

                            <telerik:GridTemplateColumn UniqueName="Name" HeaderText="Nome Documento">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDocumentName"></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>

                            <telerik:GridTemplateColumn AllowFiltering="False" HeaderText="Tipo Documento" UniqueName="typeDoc">
                                <HeaderStyle Width="60%"></HeaderStyle>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDocumentTypeNotFound"></asp:Label>
                                    <telerik:RadDropDownList runat="server" ID="ddlDocumentType" AutoPostBack="False" OnSelectedIndexChanged="DdlDocumentType_SelectedIndexChanged" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                    <ClientSettings EnableRowHoverStyle="False">
                        <Selecting AllowRowSelect="True" EnableDragToSelectRows="False" UseClientSelectColumnOnly="True"></Selecting>
                    </ClientSettings>
                </telerik:RadGrid>
            </div>
        </asp:Panel>
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlButtons">
        <asp:Button runat="server" Width="150px" ValidationGroup="udsValidation" CausesValidation="True" ID="btnConfirm" Text="Conferma"></asp:Button>
    </asp:Panel>
</asp:Content>
