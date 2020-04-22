<%@ Page Title="Conferma Bozze" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="ReslSeriesToConfirm.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslSeriesToConfirm" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            function ExecuteAjaxRequest(operationName) {
                $find("<%= RadAjaxManager.GetCurrent(Page).ClientID %>").ajaxRequest(operationName);
            }
        </script>
    </telerik:RadScriptBlock>
    <style type="text/css">
        .customThTable {
            height: 20px;
            padding-left: 3px;
            vertical-align: middle;
            text-align: left;
            line-height: 20px;
            font-weight: bold;
            font-size: 11px;
            background-image: none;
            border-bottom: 1px solid #999999;
            color: #000000;
            background-color: #ffc78c;
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
    <asp:Panel runat="server" ID="pnlPageContainer" Width="100%">
        <asp:Panel runat="server" ID="pnlDraftToComplete">
            <table class="datatable">
                <tr>
                    <th colspan="3">Bozze da completare</th>
                </tr>
                <tr>
                    <td style="vertical-align: middle; width: 200px"></td>
                    <td style="vertical-align: middle; width: 400px">
                        <telerik:RadDropDownList runat="server" ID="ddlDraftSeries" Width="400px" AutoPostBack="True" CausesValidation="False"></telerik:RadDropDownList>
                    </td>
                    <td style="vertical-align: middle;">
                        <asp:Button runat="server" ID="btnRemoveDraftLink" Text="Rimuovi collegamento Bozza" />
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <!-- Pannello gestione documenti resolution -->
        <asp:Panel runat="server" ID="pnlResolutionDocuments">
            <table style="width: 100%;">
                <tr>
                    <th class="customThTable">
                        <asp:Label runat="server" ID="dgvTitle"></asp:Label></th>
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
                                            <telerik:GridGroupByField FieldAlias="Tipologia" FieldName="DocumentGroup" HeaderText=""></telerik:GridGroupByField>
                                        </SelectFields>
                                        <GroupByFields>
                                            <telerik:GridGroupByField FieldName="DocumentGroup" SortOrder="Descending"></telerik:GridGroupByField>
                                        </GroupByFields>
                                    </telerik:GridGroupByExpression>
                                </GroupByExpressions>
                                <Columns>

                                    <telerik:GridClientSelectColumn HeaderStyle-Width="16px" UniqueName="Select" />

                                    <telerik:GridTemplateColumn UniqueName="Name" HeaderText="Nome Documento">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblDocumentName"></asp:Label>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                    <telerik:GridTemplateColumn UniqueName="CcDocument" HeaderStyle-Width="16px" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/file_extension_pdf.png" HeaderText="Copia conforme">
                                        <ItemTemplate>
                                            <asp:CheckBox runat="server" ID="chkCcDocument" Checked="False" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                    <telerik:GridTemplateColumn AllowFiltering="False" HeaderText="Tipo Documento" UniqueName="typeDoc">
                                        <HeaderStyle Width="60%"></HeaderStyle>
                                        <ItemTemplate>
                                            <telerik:RadDropDownList runat="server" ID="ddlDocumentType" AutoPostBack="False" />
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
        <!-- Dati dinamici -->
        <asp:Panel runat="server" ID="pnlDynamicData">
            <asp:PlaceHolder runat="server" ID="DynamicControls"></asp:PlaceHolder>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlButtons">
        <asp:Button runat="server" ID="btnConfirm" Width="150px" Text="Conferma" />
    </asp:Panel>
</asp:Content>
