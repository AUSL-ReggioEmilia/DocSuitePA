<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UtltImpersonator.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltImpersonator" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Admin Impersonator"%>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <div>
        <asp:Label runat="server" ID="lblUser" Text="User"></asp:Label><br />
        <asp:TextBox runat="server" ID="txtUser"></asp:TextBox><br />
        <asp:Label runat="server" ID="lblPwd" Text="Password"></asp:Label><br />
        <asp:TextBox runat="server" ID="txtPwd" TextMode="Password"></asp:TextBox><br /><br />
        <asp:Button runat="server" ID="cmdImpersonate" Text="OK" Width="100" />
    </div>
</asp:Content>