<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CommonViewSettori.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonViewSettori" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Autorizzazioni non accettate" %>

<%@ Register Src="uscSettori.ascx" TagName="uscSettori" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <%-- Treeview Settori --%>
 <uc:uscSettori ID="uscSettori" ReadOnly="true" Required="false" runat="server" Visible="false" />
 </asp:Content>
 
 