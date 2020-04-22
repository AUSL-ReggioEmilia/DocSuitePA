<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="TestWatermark.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TestWatermark" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="server">
        <div>
            <span>Selezione Atto:&nbsp;</span>
            <asp:TextBox runat="server" ID="txtIdResolution"></asp:TextBox>
            <asp:Button runat="server" id="cmdWatermark" Text="Watermark" />
        </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server">
</asp:Content>
