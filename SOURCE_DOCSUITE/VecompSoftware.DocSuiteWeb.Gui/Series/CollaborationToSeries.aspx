<%@ Page Title="Collaborazione - inserimento in" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="CollaborationToSeries.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CollaborationToSeries" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            function <%= Me.ID %>_OpenWindow(url, name, parameters) {
                var wnd = $find(name);
                wnd.setUrl(url + "?" + parameters);
                wnd.set_destroyOnClose(true);
                wnd.show();
                wnd.center();
                return false;
            }

            function ShowLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= pnlPageContent.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);
            }

            function HideLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= pnlPageContent.ClientID%>";
                currentLoadingPanel.hide(currentUpdatedControl);
            }

            function responseEnd(sender, eventArgs) {
                HideLoadingPanel();
            }
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager DestroyOnClose="True" ReloadOnShow="True" ID="RadWindowManagerDocument" runat="server">
        <Windows>
            <telerik:RadWindow Behaviors="Maximize,Close,Resize,Reload" DestroyOnClose="True" ID="windowPreviewDocument" ReloadOnShow="false" runat="server" Title="Anteprima documento" />
        </Windows>
    </telerik:RadWindowManager>

    <style type="text/css">
        .customThTable {
            height: 20px;
            padding-left: 3px;
            vertical-align: middle;
            text-align: left;
            line-height: 20px;
            font-weight: bold;
            font-size: 11px;
            background-image: url("../App_Themes/DocSuite2008/images/series/TabellaGrad.gif");
            border-bottom: 1px solid #999999;
            color: #ffffff;
        }

        .customTdTable {
            text-align: left;
            vertical-align: top;
            padding: 2px;
            font-weight: normal;
            font-size: 11px;
            color: #000;
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel runat="server" ID="pnlPageContent">
        <asp:Panel runat="server" ID="pnlSeries">
            <table class="datatable">
                <tr>
                    <th colspan="2">
                        <asp:Label runat="server" ID="lblSeriesSelectionTitle"></asp:Label></th>
                </tr>
                <tr>
                    <td class="DocumentSeriesLabel" style="vertical-align: middle;">Tipo:
                    </td>
                    <td>
                        <telerik:RadDropDownList runat="server" ID="ddlContainers" AutoPostBack="True" DefaultMessage="Seleziona Tipo" Width="300px" CausesValidation="False"></telerik:RadDropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="DocumentSeriesLabel" style="vertical-align: middle;">
                        <asp:Label ID="lblDocumentSeriesTitle" runat="server" />
                    </td>
                    <td>
                        <telerik:RadDropDownList runat="server" ID="ddlSeries" AutoPostBack="True" Width="300px" CausesValidation="False"></telerik:RadDropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="DocumentSeriesLabel" style="vertical-align: middle;">
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ValidationGroup="seriesValidation" runat="server" ID="ddlSeriesValidator" Display="Dynamic" ControlToValidate="ddlSeries" ErrorMessage="Il campo è obbligatorio"></asp:RequiredFieldValidator>
                    </td>
                </tr>                
            </table>
        </asp:Panel>
        
        <asp:Panel runat="server" ID="pnlSubSection">
            <table class="datatable">
                <tr>
                    <th colspan="2">Sotto-sezione</th>
                </tr>                
                <tr>
                    <td class="DocumentSeriesLabel">&nbsp;</td>
                    <td>
                        <telerik:RadDropDownList runat="server" CausesValidation="false" ID="ddlSubsection" AutoPostBack="false" Visible="True" Width="300px" DataTextField="Description" DataValueField="Id" />                        
                    </td>                    
                </tr>
                <tr>
                    <td class="DocumentSeriesLabel"></td>
                    <td>
                        <asp:RequiredFieldValidator ValidationGroup="seriesValidation" ID="subSectionValidation" runat="server" ControlToValidate="ddlSubsection" ErrorMessage="Campo sotto-sezione Obbligatorio" Enabled="False" Display="Dynamic" />
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
                    <td class="DocumentSeriesLabel" style="vertical-align: middle;">Oggetto:
                    </td>
                    <td>
                        <asp:Label runat="server" ID="lblCollaborationSubject"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="DocumentSeriesLabel" style="vertical-align: middle;">Note:
                    </td>
                    <td>
                        <asp:Label runat="server" ID="lblNote"></asp:Label>
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlCollaborationDocuments">
            <table style="width: 100%;">
                <tr>
                    <th class="customThTable">Documenti Collaborazione</th>
                </tr>
                <tr class="Chiaro">
                    <td class="customTdTable">
                        <telerik:RadGrid runat="server" ID="dgvDocuments" Width="100%" AllowMultiRowSelection="true">
                            <GroupingSettings ShowUnGroupButton="true"></GroupingSettings>
                            <MasterTableView AutoGenerateColumns="False" TableLayout="Auto" DataKeyNames="BiblosSerializeKey">
                                <ItemStyle CssClass="Scuro" />
                                <AlternatingItemStyle CssClass="Chiaro" />

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
                                            <telerik:RadDropDownList runat="server" ID="ddlDocumentType" Width="200px" AutoPostBack="False" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                            </MasterTableView>
                            <ClientSettings EnableRowHoverStyle="False">
                                <Selecting AllowRowSelect="True" EnableDragToSelectRows="False" UseClientSelectColumnOnly="True"></Selecting>
                            </ClientSettings>
                        </telerik:RadGrid>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlButtons">
        <asp:Button runat="server" Width="150px" ValidationGroup="seriesValidation" CausesValidation="True" ID="btnConfirm" Text="Conferma"></asp:Button>
    </asp:Panel>
</asp:Content>
