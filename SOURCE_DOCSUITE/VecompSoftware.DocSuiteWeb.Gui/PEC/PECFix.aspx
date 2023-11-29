<%@ Page AutoEventWireup="false" CodeBehind="PECFix.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.PECFix" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="PEC - Correzione" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">

            function responseEnd(sender, eventArgs) {
                $(location).attr('href', '<%= String.Format("PECSummary.aspx?Type=Pec&PECId={0}", CurrentPecMail.Id)%>');
            }

            function FixMail(sender, args) {
                var txtMailTo = $("#<%= txtMailTo.ClientID%>").val();
                var txtMailCC = $("#<%= txtMailCC.ClientID%>").val();
                var txtMailSubject = $("#<%= txtMailSubject.ClientID%>").val();

                $("#<%= txtMailTo.ClientID%>").val("");
                $("#<%= txtMailCC.ClientID%>").val("");
                $("#<%= txtMailSubject.ClientID%>").val("");

                $find("<%= AjaxManager.ClientID %>").ajaxRequest("<%= Me.ClientID%>" + "|FixPEC|" + txtMailTo + "|" + txtMailCC + "|" + txtMailSubject);
        }

    </script>
</telerik:RadScriptBlock>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel runat="server" ID="pnlContent">
        <%-- destinatari --%>
        <table class="datatable">
            <tr>
                <th>Destinatari</th>
            </tr>
            <tr>
                <td>
                   <asp:TextBox ID="txtMailTo" TextMode="MultiLine" Rows="2" runat="server" Width="100%"  />
                </td>
            </tr>
        </table>
        <%-- destinatari cc --%>
        <table class="datatable" id="tblCc" runat="server">
            <tr>
                <th>Destinatari Copia Conoscenza</th>
            </tr>
            <tr>
                <td>
                     <asp:TextBox runat="server" ID="txtMailCC" Rows="2" TextMode="MultiLine" Width="100%" />
                </td>
            </tr>
        </table>
        <%-- oggetto --%>
        <table class="datatable">
            <tr>
                <th>Oggetto</th>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtMailSubject" TextMode="MultiLine" Rows="3" runat="server" Width="100%"  />
                </td>
            </tr>
        </table>
        <%-- messaggio --%>
        <table class="datatable">
            <tr>
                <th>Messaggio</th>
            </tr>
            <tr>
                <td style="height: 100%">
                    <telerik:RadEditor runat="server" ID="txtMailBody" EditModes="Design" EnableResize="true" AutoResizeHeight="true" Width="100%" Height="100%" EmptyMessage="Inserire qui il testo">
                        <Tools>
                            <telerik:EditorToolGroup>
                                <telerik:EditorTool Name="Bold" />
                                <telerik:EditorTool Name="Italic" />
                                <telerik:EditorTool Name="Underline" />
                                <telerik:EditorTool Name="Cut" />
                                <telerik:EditorTool Name="Copy" />
                                <telerik:EditorTool Name="Paste" />
                            </telerik:EditorToolGroup>
                            <telerik:EditorToolGroup>
                                <telerik:EditorTool Name="JustifyLeft" />
                                <telerik:EditorTool Name="JustifyCenter" />
                                <telerik:EditorTool Name="JustifyRight" />
                                <telerik:EditorTool Name="JustifyNone" />
                            </telerik:EditorToolGroup>
                        </Tools>
                    </telerik:RadEditor>
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="cmdFix" runat="server" Text="Correggi" Width="120px" SingleClick="true" OnClientClicked="FixMail" AutoPostBack="false" />
</asp:Content>
