<%@ Page Language="vb" EnableTheming="false" StylesheetTheme="" Theme="" AutoEventWireup="false" CodeBehind="ErrorPage.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ErrorPage" %>

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

        .warningArea {
            padding: 8px 14px 8px 14px;
            border-color: #fbeed5;
            border-width: 1px;
            border-style: solid;
            font-weight: bold;
            text-shadow: 0 1px 0 #ffffff; /* fallback */
            text-shadow: 0 1px 0 rgba(255, 255, 255, 0.5);
            background-color: #fcf8e3;
            border: 1px solid #ff0000;
            -webkit-border-radius: 4px;
            -moz-border-radius: 4px;
            border-radius: 4px;
            margin: 15px;
            font-size: small;
            line-height: 13px;
        }

        code {
            font-size: 1.02em;
            color: green;
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
                <div id="console" runat="server" visible="False">
                    <div class="warningArea">
                        <h5>Come segnalare un errore</h5>
                        <ul>
                            <li>Scrivere i passi necessari a riprodurre il problema.</li>
                            <li>Allegare screenshot di tutta la finestra del browser. Premere <code>Alt + Stamp</code> e incollare nella mail con <code>CTRL+V</code>.</li>
                            <li>Includere il contenuto del messaggio sottostante. Selezionare il testo, copiare con <code>CTRL+C</code> e incollare nella mail con <code>CTRL+V</code>.</li>
                        </ul>
                    </div>
                    <div class="console">
                        <asp:Label ID="consoleMessage" runat="server" />
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
