<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmSettoreUtenti" Codebehind="DocmSettoreUtenti.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Selezione Utenti" %>
<%@ Register Src="~/UserControl/uscSelezioneSettoreUtenti.ascx" TagName="uscSelezioneSettoreUtenti" TagPrefix="uc1" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
<%--    <asp:DropDownList ID="ddlToken" runat="server" AutoPostBack="True">
    </asp:DropDownList>--%>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
<%--    <table cellSpacing="0" cellPadding="0" width="100%" border="0" bgcolor="#F0FFFF" style="height: 100%;">
        <tr class="Chiaro">
            <td valign="top">
                <iewc:TreeView ID="Tvw" runat="server" DefaultStyle="font-family:Verdana, Helvetica, sans-serif; font-size:8pt;"
                    Width="100%" Height="420px" BorderColor="Black" BorderWidth="0px">
                </iewc:TreeView>
            </td>
        </tr>
    </table>--%>
 
    <uc1:uscSelezioneSettoreUtenti ID="uscSelezioneSettoreUtenti" runat="server" EnableViewState="true"  />   
</asp:Content>    

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConferma" runat="server" Text="Conferma modifica"></asp:Button>
</asp:Content>