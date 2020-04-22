<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="CommonExcelImportBusinessContact.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonExcelImportBusinessContact" %>

<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagPrefix="usc" TagName="UploadDocument" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="server">
      <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            function ReturnValuesJson(serializedContact) {
                 GetRadWindow().close(serializedContact);
            }

       
        </script>
    </telerik:RadScriptBlock>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
   
     <asp:Panel runat="server" ID="MainPanel">
     <table class="datatable">
            <tr>
                <th colspan="2">Opzioni Documento</th>
            </tr>
            <tr>
                <td class="label" style="width:20%;">Documento da importare:
                </td>
                <td>
                    <usc:UploadDocument AllowedExtensions=".xls" ButtonPreviewEnabled="False" ButtonScannerEnabled="False" HeaderVisible="False" ID="uscDocument" IsDocumentRequired="True" MultipleDocuments="False" runat="server" SignButtonEnabled="false" />
                </td>
            </tr>
            <tr>
                <td class="label">Campi del documento:
                </td>
                <td>
                    <asp:Label runat="server" ID="lblFields" />
                </td>
            </tr>
            <tr>
                <td></td>
                <td>
                    <ul class="warningArea">
                        <li>Verificare che i Campi corrispondano alle intestazioni delle colonne</li>
                        <li>Verificare che le celle dei Campi di tipo data siano nel formato corretto</li>
                        <li>Se il file è troppo grande la pagina potrebbe andare in errore, ma l'importazione avverrà correttamente</li>
                    </ul>
                </td>
            </tr>
        </table>
       </asp:Panel>
      <telerik:RadAjaxPanel runat="server" ID="ajaxPanel" >
        <asp:Table class="datatable" runat="server" ID="errorValidationTable"></asp:Table>
    </telerik:RadAjaxPanel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Button runat="server" Text="Importa registrazioni" ID="cmdImport" />
</asp:Content>