<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CommMessage.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommMessage" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Invia Mail" ValidateRequest="false" %>

<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="SelContatti" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript" language="javascript">



            //restituisce un riferimento alla radwindow
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

        </script>
    </telerik:RadScriptBlock>
    
    <table id="pnlContent" runat="server" class="datatable">
        <tbody>
            <tr>
                <td class="label">Mittente:</td>
                <td>
                    <asp:Label runat="server" ID="SenderDescription" />&nbsp;&lt;<asp:Label runat="server" ID="SenderEmail" />&gt;
                </td>
            </tr>
            <tr>
                <td class="label"></td>
                <td style="width: 650px;">
                       <usc:SelContatti ButtonImportManualVisible="false" Caption="Destinatari" EnableCC="false"
                                            HeaderVisible="false" ID="MessageDest" IsRequired="false" Multiple="true" MultiSelect="true"
                                            ProtType="True" RequiredErrorMessage="Destinatario obbligatorio" runat="server" SimpleMode="false"
                                            TreeViewCaption="Destinatari:" Type="Prot" />  
                </td>
            </tr>
            <tr>
                <td class="label">Oggetto:</td>
                <td>
                    <asp:TextBox runat="server" ID="MessageSubject" Width="650px" />
                </td>
            </tr>
            <tr>
                <td class="label">Messaggio:</td>
                <td style="padding-bottom: 10px;">
                    <telerik:RadEditor runat="server" ID="MessageBodyEditor" Height="240"  Width="650px"  ContentFilters="ConvertTags" EditorContentFilters="DefaultScripts">
                        <Tools>
                            <telerik:EditorToolGroup>
                                <telerik:EditorTool Name="Bold"></telerik:EditorTool>
                                <telerik:EditorTool Name="Italic"></telerik:EditorTool>
                                <telerik:EditorTool Name="Underline"></telerik:EditorTool>
                                <telerik:EditorTool Name="Cut"></telerik:EditorTool>
                                <telerik:EditorTool Name="Copy"></telerik:EditorTool>
                                <telerik:EditorTool Name="Paste"></telerik:EditorTool>
                            </telerik:EditorToolGroup>
                            <telerik:EditorToolGroup>
                                <telerik:EditorTool Name="JustifyLeft"></telerik:EditorTool>
                                <telerik:EditorTool Name="JustifyCenter"></telerik:EditorTool>
                                <telerik:EditorTool Name="JustifyRight"></telerik:EditorTool>
                                <telerik:EditorTool Name="JustifyNone"></telerik:EditorTool>
                            </telerik:EditorToolGroup>
                        </Tools>
                    </telerik:RadEditor>
                </td>
            </tr>
        </tbody>
    </table>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server">
    <div style="text-align: right;">
        <asp:Button ID="cmdUndo" runat="server" Text="Annulla" />
        <asp:Button ID="cmdSend" runat="server" Text="Invia Email" />
    </div>
</asp:Content>

