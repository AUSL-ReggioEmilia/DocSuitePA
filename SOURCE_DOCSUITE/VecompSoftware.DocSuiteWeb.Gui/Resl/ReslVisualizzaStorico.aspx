<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ReslVisualizzaStorico.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslVisualizzaStorico" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="../UserControl/uscResolution.ascx" TagName="uscResolution" TagPrefix="usc" %>
<%@ Register Src="../UserControl/uscResolutionBar.ascx" TagName="uscResolutionBar" TagPrefix="usc" %>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript" language="javascript">
            function OpenWindowDocument()
            {					
                var manager = $find("<%=RadWindowManagerResl.ClientID %>");
                var wnd = manager.open("<%= GetWindowDocumentPage() %>", "windowDocument");
                wnd.setSize(600, 400);
                wnd.center();
                return false;
            }
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerResl" runat="server">
        <Windows>
            <telerik:RadWindow Height="300" ID="windowDocument" runat="server" Width="500" />
        </Windows>
    </telerik:RadWindowManager>
    
    <div runat="server" id="PageDiv" style="height: 100%">
        <table id="tblPrincipale" style="height: 100%; width: 100%" runat="server">
            <tr>
                <td style="width: 100%; vertical-align: top;">
                    <usc:uscResolution ID="uscResolution" runat="server" />
                </td>
                <td class="center" width="3%" height="100%">
                    <asp:Table BorderColor="Gray" BorderStyle="Solid" BorderWidth="1px" CellPadding="3" CellSpacing="3" CssClass="datatable" Height="100%" ID="TblButtons" runat="server">
                        <asp:TableRow Height="100%">
                            <asp:TableCell Height="100%" VerticalAlign="Top" />
                        </asp:TableRow>
                    </asp:Table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="cphFooter" ID="Content3" runat="server">
    <table id="tblBtn" runat="server" cellspacing="0" cellpadding="1" border="0">
        <tr>
            <td>
                <asp:Button ID="cmdProposta" runat="server" Text="Proposta" />
                <asp:Button ID="cmdDocumento" runat="server" Text="Documento" />
                <asp:Button ID="cmdFrontespizio" runat="server" Text="Frontespizio" />
                <asp:Button ID="cmdAllegati" runat="server" Text="Allegati" Enabled="False" />
                <asp:Button ID="cmdOrganoControllo" runat="server" Text="Organo Controllo" />
                <asp:Button ID="Pratica" runat="server" Text="Pratica" />
                <asp:Button ID="cmdMail" runat="server" Text="Invia" />
                <asp:TextBox ID="SelPratica" runat="server" Width="56px" />
                <asp:Button ID="btnSelPratica" runat="server" Text="Pratica" />
            </td>
        </tr>
    </table>
</asp:Content>
