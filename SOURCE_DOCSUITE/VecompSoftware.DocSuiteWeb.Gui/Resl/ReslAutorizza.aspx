<%@ Page AutoEventWireup="false" CodeBehind="ReslAutorizza.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslAutorizza" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Delibera - Autorizzazioni" %>

<%@ Register Src="~/UserControl/uscResolution.ascx" TagName="uscResolution" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="uscSettori" TagPrefix="uc1" %>


<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel runat="server" ID="MainPanel">
         <%--Dati--%> 
        <uc1:uscResolution runat="server" ID="uscResolution" />
        
        <table class="datatable">
            <tr>
                <th colspan="2">Autorizzazione</th>
            </tr>
            <tr class="Chiaro">
                <td class="label" style="width: 20%">Nome:</td>
                <td style="width: 80%"><asp:Label runat="server" ID="rrtName"></asp:Label></td>
            </tr>
            
            <tr class="Chiaro">
                <td class="label">Note:</td>
                <td><asp:Label runat="server" ID="lblNote"></asp:Label></td>
            </tr>
        </table>
        

        <table id="tblDati" cellspacing="1" cellpadding="1" width="100%" border="0">
            <tr>
                <td>
                    <uc1:uscSettori Caption="Autorizzazioni" ID="uscAutorizza" MultiSelect="true" Required="false" runat="server" Type="Resl" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="cphFooter">
    <div>
        <asp:Button runat="server" ID="cmdAddFromCollaboration" Text="Aggiungi da Collaborazione" />
        <asp:Button runat="server" ID="cmdAddDefaults" Text="Aggiungi Default" />
        <asp:Button runat="server" ID="cmdAddFromContact" Text="Aggiungi da Contatti" />
    </div>
    <asp:Button ID="btnConferma" runat="server" Text="Conferma modifica" />
</asp:Content>
