<%@ Page AutoEventWireup="false" CodeBehind="CollaborationMailSender.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CollaborationMailSender" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/MailSenders/MailSenderControl.ascx" TagPrefix="uc1" TagName="MailSenderControl" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <uc1:MailSenderControl runat="server" id="MailSenderControl" />
</asp:Content>
