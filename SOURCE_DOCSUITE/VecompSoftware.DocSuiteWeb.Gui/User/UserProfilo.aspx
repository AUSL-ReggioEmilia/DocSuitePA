<%@ Page AutoEventWireup="false" Codebehind="UserProfilo.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UserProfilo" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Utente - Settori abilitati" %>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="cphContent">

<telerik:RadTabStrip RenderMode="Lightweight" runat="server" ID="RadTabStrip1" MultiPageID="RadMultiPage1" Skin="Silk">
    <Tabs>
        <telerik:RadTab Text="Protocollo" Width="200px" Visible="false"></telerik:RadTab>
        <telerik:RadTab Text="Pratiche" Width="200px" Visible="false"></telerik:RadTab>
        <telerik:RadTab Text="Atti" Width="200px" Visible="false"></telerik:RadTab>
    </Tabs>
</telerik:RadTabStrip>


<telerik:RadMultiPage runat="server" ID="RadMultiPage1" SelectedIndex="0" CssClass="outerMultiPage">
    <telerik:RadPageView runat="server" ID="RadPageView1" Visible="false">
            <table id="TABLE1" class="datatable">    
                <tr>
                    <th>
                        Protocollo
                    </th>
                </tr>
                <tr>
                    <td>
                        <telerik:RadTreeView CheckBoxes="true" ID="tvwSettoriProtMng" runat="server" Width="100%">
                            <Nodes>
                                <telerik:RadTreeNode Checkable="false" Expanded="True" Text="Settori abilitati (Manager)" />
                            </Nodes>
                        </telerik:RadTreeView>
                        <telerik:RadTreeView CheckBoxes="true" ID="tvwSettoriProt" runat="server" Width="100%">
                            <Nodes>
                                <telerik:RadTreeNode Checkable="false" Expanded="True" Text="Settori abilitati" />
                            </Nodes>
                        </telerik:RadTreeView>
                    </td>
                </tr>
            </table>
    </telerik:RadPageView>
    <telerik:RadPageView runat="server" ID="RadPageView2" Visible="false">
            <table id="TBLROLES" class="datatable">
                <tr>
                    <th>
                        Pratiche
                    </th>
                </tr>
                <tr>
                    <td>
                        <telerik:RadTreeView CheckBoxes="true" ID="tvwSettoriDocm" runat="server" Width="100%">
                            <Nodes>
                                <telerik:RadTreeNode Checkable="false" Expanded="True" Text="Settori abilitati" />
                            </Nodes>
                        </telerik:RadTreeView>
                    </td>
                </tr>
            </table>
    </telerik:RadPageView>
    <telerik:RadPageView runat="server" ID="RadPageView3" Visible="false">
            <table class="datatable">
                <tr>
                    <th>
                        <asp:Label ID="lblAtti" runat="server"></asp:Label>
                    </th>
                </tr>
                <tr>
                    <td>
                        <telerik:RadTreeView CheckBoxes="true" ID="tvwSettoriResl" runat="server" Width="100%">
                            <Nodes>
                                <telerik:RadTreeNode Checkable="false" Expanded="True" Text="Settori abilitati" />
                            </Nodes>
                        </telerik:RadTreeView>
                    </td>
                </tr>
            </table>
    </telerik:RadPageView>
</telerik:RadMultiPage>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConferma" runat="server" Text="Conferma" />
    <asp:Button ID="btnExtend" runat="server" Text="Visualizza sotto-settori" Visible="false" />
	<asp:button ID="btnSelectAll" runat="server" text="Seleziona tutti"/>
	<asp:button ID="btnDeselectAll" runat="server" text="Deseleziona tutti"/>
</asp:Content>