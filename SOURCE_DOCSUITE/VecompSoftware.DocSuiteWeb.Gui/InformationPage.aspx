<%@ Page Language="vb" EnableTheming="false" StylesheetTheme="" Theme="" AutoEventWireup="false" CodeBehind="InformationPage.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.InformationPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head2" runat="server">
    <title>Errore</title>
    <meta http-equiv="Content-Type" content="text/html; charset=windows-1252" />
    <style type="text/css">
        body {
            font-size: 25px;
            font-family: sans-serif;
        }

        h1 {
            font-size: 30px;
        }

        h2 {
            font-size: 28px;
        }

        .section {
            padding: 15px;
        }

        .header {
            font-weight: bold;
            font-size: 1.2em;
            vertical-align: central;
            height: 40px;
        }

            .header img {
                vertical-align: middle;
                margin-right: 10px;
            }

            .header h1 {
                display: inline;
                line-height: 40px;
                text-transform: uppercase;
            }

        .article {
            text-align: left;
            font-size: 20px;
        }

        .console {
            font-size: 16px;
            font-family: Courier;
            font-weight: bold;
            color: #CCCCCC;
            background: #000000;
            border: 3px double #CCCCCC;
            padding: 10px;
            overflow: scroll;
            white-space: nowrap;
        }
    </style>
    <script language="javascript" type="text/javascript">
        function DoRedirect(delay, address) {
            setTimeout(function () { window.top.location.href = address; }, delay);
        }
    </script>
</head>
<body>
    <form id="form" runat="server">
        <div class="section">
            <div class="header">
                <asp:Image ID="image" runat="server" />
                <h1>
                    <asp:Label ID="info" runat="server" /></h1>
            </div>
            <div class="article">
                <h2>
                    <asp:Label ID="titolo" runat="server" /></h2>
                <p>
                    <asp:Label ID="descrizione" runat="server" />
                </p>
            </div>
        </div>
    </form>
</body>
</html>
