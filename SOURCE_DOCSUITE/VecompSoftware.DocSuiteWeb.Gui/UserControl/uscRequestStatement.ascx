<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscRequestStatement.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscRequestStatement" %>

<%@ Import Namespace="VecompSoftware.DocSuiteWeb.Gui" %>

<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="ContattiSel" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="settori" TagPrefix="usc" %>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
<asp:Panel runat="server" ID="pnlRequestStatementCompliance">
    <telerik:RadPageLayout runat="server" widht="100%" ID="pnlSigners" CssClass="dsw-panel">
        <Rows>
            <telerik:LayoutRow CssClass="dsw-panel-title" Style="margin-bottom: 2px;" ID="documentPanel" runat="server">
                <Columns>
                    <telerik:LayoutColumn Span="12" Style="margin-bottom: 10px;">
                        <asp:Label ID="lblDocument" runat="server" Font-Bold="True">Seleziona i Documenti</asp:Label>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow Style="margin-bottom: 2px">
                <Columns>
                    <telerik:LayoutColumn Style="margin-bottom: 2px; padding-left:1px; padding-right:1px">
                        <telerik:RadGrid runat="server" ID="DocumentListGrid" Width="100%" AllowAutomaticUpdates="True" GridLines="None" AllowPaging="False" AllowMultiRowSelection="true">
                            <MasterTableView CommandItemDisplay="None" AutoGenerateColumns="False">
                                <Columns>

                                    <telerik:GridClientSelectColumn HeaderStyle-Width="16px" UniqueName="Select" />

                                    <telerik:GridTemplateColumn UniqueName="DocumentExtensionImage" HeaderStyle-Width="16px" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/documentPreview.png" HeaderText="Tipo documento">
                                        <ItemTemplate>
                                            <asp:Image ID="DocsImage" runat="server" ImageUrl='<%# ImagePath.FromFile(Eval("Document.Name").ToString())%>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                    <telerik:GridCheckBoxColumn UniqueName="IsMainDocument" DataField="Document.IsMainDocument" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" HeaderImageUrl="~/Comm/Images/PriorityHigh16.gif" HeaderTooltip="Documento principale"/>

                                    <telerik:GridBoundColumn UniqueName="Name" DataField="Document.Name" HeaderText="Nome Documento" ItemStyle-HorizontalAlign="Left" />
                                    <telerik:GridBoundColumn UniqueName="IdDocument" DataField="Document.IdDocument" Display="false" />
                                    <telerik:GridBoundColumn UniqueName="IdChain" DataField="Document.IdChain" Display="false" />
                                </Columns>
                            </MasterTableView>
                            <ClientSettings EnableRowHoverStyle="False">
                                <Selecting AllowRowSelect="True" EnableDragToSelectRows="False" UseClientSelectColumnOnly="True"></Selecting>
                            </ClientSettings>
                        </telerik:RadGrid>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>

            <telerik:LayoutRow CssClass="dsw-panel-title" Style="margin-bottom: 2px;" ID="rowSignerTitle" runat="server">
                <Columns>
                    <telerik:LayoutColumn Span="12" Style="margin-bottom: 10px;">
                        <asp:Label ID="lblSigners" runat="server" Font-Bold="True">Seleziona Firmatari </asp:Label>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>


            <telerik:LayoutRow Style="margin-bottom: 2px;" ID="rowSigners" runat="server">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="None" Span="12" CssClass="content-wrapper" Style="margin-bottom: 10px;">
                        <asp:Panel runat="server" CssClass="content-wrapper">
                            <usc:ContattiSel ButtonDeleteVisible="true" ButtonImportVisible="false" ButtonIPAVisible="false" ButtonManualVisible="false" ButtonPropertiesVisible="false" ButtonRoleVisible="true" ButtonSelectOChartVisible="false" ButtonSelectVisible="true" EnableCheck="True" EnableViewState="true" HeaderVisible="false" ID="uscSigners" IsRequired="true" runat="server" TreeViewCaption="Firmatari" UseAD="true" MultiSelect="false" Multiple="true" />
                        </asp:Panel>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
                   <telerik:LayoutRow CssClass="dsw-panel-title" Style="margin-bottom: 2px;" ID="rowSecretaryTitle" runat="server">
                <Columns>
                    <telerik:LayoutColumn Span="12" Style="margin-bottom: 10px;">
                        <asp:Label ID="lblSecretaries" runat="server" Font-Bold="True">Segreterie </asp:Label>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
               <telerik:LayoutRow Style="margin-bottom: 2px;" ID="rowSecretaries" runat="server">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="None" Span="12" CssClass="content-wrapper" Style="margin-bottom: 10px;">
                        <asp:Panel runat="server" CssClass="content-wrapper">
                            <usc:Settori Caption="Segreterie" Checkable="True" HeaderVisible="false" ID="uscSettoriSegreterie" MultipleRoles="True" MultiSelect="true" ReadOnly="True" Required="False" runat="server" />
                        </asp:Panel>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Panel>
