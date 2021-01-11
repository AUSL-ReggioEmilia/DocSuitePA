<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Test.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Services.AuslPcPubblicazione.Test" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Pagina di test caricamento Atti</title>

    <style type="text/css">
        #form1
        {
            height: 644px;
        }
    </style>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h3>&nbsp;VISUALIZZA</h3>
            <asp:Panel ID="Panel2" runat="server" Height="100px" Width="562px">
                <asp:Label ID="Label11" runat="server" Text="NPubblicazione"></asp:Label>
                <br />
                <asp:TextBox ID="prova" runat="server"></asp:TextBox>
                <br />
                <br />
                <asp:Button ID="send" runat="server" text="Visualizza" />
                <h3>
                    INSERIMENTO</h3>
            </asp:Panel>
        </div>

        <div style="height: 107px; margin-top: 36px">
            <asp:Panel ID="Panel1" runat="server" Height="90px" Width="561px">
                <asp:Label ID="LabelDocumento" runat="server" Text="Tipo Documento"></asp:Label>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Label ID="Label2" runat="server" Text="Titolo"></asp:Label>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Label ID="Label3" runat="server" Text="Oggetto"></asp:Label>
                <br />
                <asp:TextBox ID="TipoDocumento" runat="server"></asp:TextBox>
                <asp:TextBox ID="Titolo" runat="server"></asp:TextBox>
                <asp:TextBox ID="Oggetto" runat="server"></asp:TextBox>
                <br />
                <br />
                <asp:Button ID="Inserisci" runat="server" Height="26px" Text="Inserisci" 
                    Width="71px" />
            </asp:Panel>
            <br />
        </div>

        <div style="height: 170px; width: 409px;">
            <h3>PUBBLICAZIONE</h3>
            <asp:Panel ID="Panel3" runat="server" Height="106px" Width="557px">
                <asp:Label ID="Label1" runat="server" Text="attoPath"></asp:Label>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Label ID="Label4" runat="server" Text="NPubblicazione"></asp:Label>
                 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Label ID="Label12" runat="server" Text="Oggetto"></asp:Label>
                <br />
                <asp:TextBox ID="AttoPath" runat="server"></asp:TextBox>
                <asp:TextBox ID="NPubblicazione" runat="server" Text=""></asp:TextBox>
                <asp:TextBox ID="OggettoPubblicazione" runat="server"></asp:TextBox>
                <br />
                <br />
                <asp:Button ID="Pubblica" runat="server" Text="Pubblica" />
            </asp:Panel>
        </div>
        <div>
            <h3>REVOCA</h3>
            <asp:Panel ID="Panel4" runat="server" Height="84px" style="margin-top: 0px" Width="552px">
                <asp:Label ID="Label8" runat="server" Text="NPubblicazione"></asp:Label>
                <br />
                <asp:TextBox ID="nPublicazione2" runat="server" Text=""></asp:TextBox>
                <br />
                <br />
                <asp:Button ID="Revoca" runat="server" Text="Revoca" />
            </asp:Panel>       
            <br /><br />
        </div>

        <br /><br />
    </form>


</body>
</html>
