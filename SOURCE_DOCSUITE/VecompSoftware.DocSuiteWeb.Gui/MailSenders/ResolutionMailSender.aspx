<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master"
    CodeBehind="ResolutionMailSender.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ResolutionMailSender" %>

<%@ Register Src="~/MailSenders/MailSenderControl.ascx" TagPrefix="uc1" TagName="MailSenderControl" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript" language="javascript">
            function OpenWindowMailSettori(url) {
                setTimeout(function () {
                    var manager = $find("<%=RadWindowManagerResl.ClientID %>");
                    var wnd = manager.open(url, "windowMailSettori");
                    wnd.add_close(CloseMailSettori);
                    wnd.center();
                    return true;
                }, 0);
            }

            function CloseMailSettori(sender, args) {
                sender.remove_close(CloseMailSettori);
                if (args.get_argument() !== null) {
                    var manager = $find("<%= AjaxManager.ClientID %>");
                    manager.ajaxRequest("Send_Mail|" + args.get_argument());
                }
            }
        </script>
    </telerik:RadScriptBlock>
    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerResl" runat="server">
        <Windows>
            <telerik:RadWindow Height="300" ID="windowMailSettori" runat="server" Title="Selezione Destinatari" Width="500" />
        </Windows>
    </telerik:RadWindowManager>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <uc1:MailSenderControl runat="server" ID="MailSenderControl" />
</asp:Content>
