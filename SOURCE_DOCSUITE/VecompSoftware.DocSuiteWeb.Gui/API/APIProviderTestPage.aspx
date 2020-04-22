<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.APIProviderTestPage"
    CodeBehind="APIProviderTestPage.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master"
    Title="Gestione APIProvider (BETA)" %>

<asp:Content ID="Content" runat="server" ContentPlaceHolderID="cphContent">
    <div style="padding:10px;">
        <div style="float:left; width:200px;">
            <asp:Button ID="ButtonRenew" runat="server" Text="Rinnova" Width="120px" />
        </div>
        <div>
            Allinea la configurazione dei provider con quelle del provider principale.
            Qualora il provider principale non sia definito ne genera uno a partire dalla chiave "APIDefaultProvider" di ProtocolEnv.
        </div>
    </div>
    <div style="padding:10px;">
        <div style="float:left; width:200px;">
            <asp:Button ID="ButtonCreateNew" runat="server" Text="Crea Nuova" Width="120px" />
        </div>
        <div style="float:left;">
            Crea una nuova voce di configurazione nella tabella "APIProvider".
        </div>
    </div>
</asp:Content>