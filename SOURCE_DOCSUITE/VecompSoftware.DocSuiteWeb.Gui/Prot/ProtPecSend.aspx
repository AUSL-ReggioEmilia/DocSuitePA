<%@ Page AutoEventWireup="false" CodeBehind="ProtPecSend.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtPecSend" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Inoltra PEC Selezionate" %>

<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagPrefix="usc" TagName="SelContatti" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphContent">
        <table class="datatable">
            <tr class="Chiaro">
                <td>
                    <div>
                        <usc:SelContatti runat="server" ID="uscDestinatari" Type="Prot" Caption="Destinatari" ProtType="True" Multiple="true" MultiSelect="true" IsRequired="true" EnableCC="true" RequiredErrorMessage="Destinatario obbligatorio" TreeViewCaption="Destinatari" ButtonImportManualVisible="true" />
                    </div>
                    <hr />
                    <div style="float:left; width:150px; font-weight:bold;">Oggetto:</div>
                    <div>
                        <asp:TextBox ID="txtMailSubject" runat="server" Width="500px" />
                    </div>
                    <div style="float:left; width:150px; font-weight:bold;">Messaggio:</div>
                    <div>
                        <asp:TextBox ID="txtMailBody" runat="server" Width="500px" Height="175px" TextMode="MultiLine" />
                    </div>
                    <div style="float:left; width:150px; font-weight:bold;">PEC Allegate:</div>
                    <div style="width:500px; height:100px; border:solid 1px #dcdcdc; overflow:scroll; padding-left:25px;">
                        <asp:BulletedList ID="blsMailAttachments" runat="server" Font-Size="10px" BulletStyle="CustomImage" BulletImageUrl="~/Comm/Images/File/Mail16.gif" />
                    </div>
                </td>
            </tr>
        </table>
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="cphFooter">
    <div style="text-align: right">
        <asp:Button ID="cmdSend" runat="server" Text="Invia Mail" />
    </div> 
</asp:Content>