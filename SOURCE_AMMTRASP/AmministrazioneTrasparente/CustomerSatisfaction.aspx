<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/DocumentSeries.Master" CodeBehind="CustomerSatisfaction.aspx.cs" Inherits="AmministrazioneTrasparente.CustomerSatisfaction" %>
<%@ Register Src="UserControls/uscCustomerSatisfactionForm.ascx" TagPrefix="usc" TagName="uscCustomerSatisfactionForm" %>

<asp:Content ContentPlaceHolderID="MainPlaceHolder" runat="server">
    <usc:uscCustomerSatisfactionForm runat="server" ID="uscCustomerSatisfactionForm"></usc:uscCustomerSatisfactionForm>
</asp:Content>