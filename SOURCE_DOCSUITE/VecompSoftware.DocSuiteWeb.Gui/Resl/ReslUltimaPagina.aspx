<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslUltimaPagina" Codebehind="ReslUltimaPagina.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="../UserControl/uscDocumentUpload.ascx" TagName="uscDocumentUpload" TagPrefix="uc1" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow() {
                GetRadWindow().close();
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
   <telerik:RadPageLayout runat="server" ID="pageContent" HtmlTag="Div" Width="100%">
        <Rows>
            <telerik:LayoutRow runat="server">
                <Rows>
                    <telerik:LayoutRow runat="server" HtmlTag="Div">
                        <Content>
                            <uc1:uscDocumentUpload ID="uscDocumentUpload" Caption="Ultima pagina" runat="server" ButtonScannerEnabled="true" ButtonLibrarySharepointEnabled="false" MultipleDocuments="false" UseSessionStorage="true" Type="Resl" />
                        </Content>
                    </telerik:LayoutRow>
                </Rows>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
   <asp:Panel runat="server">
        <telerik:RadButton ID="btnSave" runat="server" Width="150px" Text="Conferma"></telerik:RadButton>
    </asp:Panel>
</asp:Content>