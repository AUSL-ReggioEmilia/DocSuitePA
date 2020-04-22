<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ProtZebraLabel.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtZebraLabel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head id="HEAD1" runat="server">
    <title>Protocollo - Stampa Etichetta</title>
    <style type="text/css">
        html, body, form, table {
            width: 100%; 
            height: 100%;
        }
    </style>
</head>
<body class="prot">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <span style="display: none; visibility: hidden;">
            <object id="Zebra" name="Zebra" classid="clsid:45A43232-AA25-4BF7-B62F-5622A6D1FAA8">
            </object>
        </span>
    </telerik:RadScriptBlock>
    <form id="CommBiblosDSThinClientx" method="post" runat="server">
        <telerik:RadScriptManager runat="server" ID="ScriptManager1">
        </telerik:RadScriptManager>

        <script language="javascript" type="text/javascript">
            //restituisce un riferimento alla radwindow
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            window.setTimeout("hideLoadingPage();", 100);

            function hideLoadingPage() {
                Zebra.PrinterName = "<%= GetPrinterName()%>";
                Zebra.Year = "<%= GetYear()%>";
                Zebra.Number = "<%= GetNumber()%>";
                Zebra.ProtocolDate = "<%= GetDate()%>";
                Zebra.NumberCopies = "1";
                Zebra.LabelToPrint = "<%= GetLabel()%>";
                Zebra.Genera();
                var oWindow = GetRadWindow();
                if (oWindow != null) {
                    oWindow.close();
                }
            }
        </script>

            <table id="loading" style="text-align: center;">
                <tr>
                    <td class="SXScuro" style="vertical-align: middle; text-align: center;">Stampa etichetta in corso...
                    </td>
                </tr>
            </table>
    </form>
</body>
</html>
