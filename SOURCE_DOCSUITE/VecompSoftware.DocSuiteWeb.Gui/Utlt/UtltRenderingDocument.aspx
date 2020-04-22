<%@ Page AutoEventWireup="false" Codebehind="UtltRenderingDocument.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltRenderingDocument" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Documento in PDF/TIF" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(initRequest);
            // Rimozione pulsante dall'ajaxificazione per ottenere la response corretta
            function initRequest(sender, args) {
                if (args.get_postBackElement().id.indexOf("<%= btnExport.ClientID%>") != -1) {
                    args.set_cancel(true);  //stop async request
                    sender._form["__EVENTTARGET"].value = args.get_postBackElement().id.replace(/\_/g, "$");
                    sender._form["__EVENTARGUMENT"].value = "";
                    sender._form.submit();
                    return;
                }
            }
            function OnClientFilesUploaded(sender, args) {
                $find('<%= AjaxManager.ClientID%>').ajaxRequest();
            }
            function validationFailed(sender, eventArgs) {
                radalert("Estensione non valida per il file: '" + eventArgs.get_fileName() + "'.");
            }
        </script>
    </telerik:RadScriptBlock>
    <table class="datatable">
        <tbody>
            <tr>
                <td class="label" style="width: 30%;">
                    File da convertire:
                </td>
                <td>
                    <telerik:RadAsyncUpload ID="AsyncUploadDocument" OnClientFilesUploaded="OnClientFilesUploaded" InitialFileInputsCount="1" InputSize="80" MultipleFileSelection="Disabled" OnClientValidationFailed="validationFailed" runat="server" UploadedFilesRendering="BelowFileInput" Width="100%">
                        <Localization Select="Sfoglia" />
                    </telerik:RadAsyncUpload>
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 30%;">
                    Nome file confermato:
                </td>
                <td>
                    <asp:Label runat="server" ID="lblFileName" />
                </td>
            </tr>
        </tbody>
    </table>
    <asp:Button ID="btnExport" runat="server" Text="Converti" />
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Repeater ID="rptFormati" runat="server">
        <HeaderTemplate>
            <table class="datatable">
                <thead>
                    <tr>
                        <th colspan="2">Formati Supportati</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="label" style="width: 30%;">
                    <%# DataBinder.Eval(Container.DataItem, "Tipo") %>
                </td>
                <td>
                    <asp:Image ID="Image1" ImageUrl='<%# Eval("Url")%>' runat="server" />
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</asp:Content>