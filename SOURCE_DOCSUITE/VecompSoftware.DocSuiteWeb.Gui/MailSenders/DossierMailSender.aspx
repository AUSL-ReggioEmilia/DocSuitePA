<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="DossierMailSender.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DossierMailSender" %>

<%@ Register Src="~/MailSenders/MailSenderControl.ascx" TagPrefix="uc1" TagName="MailSenderControl" %>


<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <uc1:MailSenderControl runat="server" ID="MailSenderControl" AuthorizationsVisibility="False" />
</asp:Content>
