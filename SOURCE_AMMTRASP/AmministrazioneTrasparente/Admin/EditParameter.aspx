<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditParameter.aspx.cs" Inherits="AmministrazioneTrasparente.Admin.EditParameter" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta name="robots" content="noindex">
    <link href="../css/bootstrap.css" rel="stylesheet" />
    <!--[if lt IE 8]>
    <style>
        #KeyValue {
            width: 90%;
        }
    </style>
    <![endif]-->
</head>
<body>
    <form id="form1" runat="server" style="padding-top: 30px;">
        <script type="text/javascript">
            function CloseAndRebind(args) {
                GetRadWindow().BrowserWindow.refreshGrid(args);
                GetRadWindow().close();
            }
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;

                return oWindow;
            }
            function CancelEdit() {
                GetRadWindow().close();
            }
        </script>
        <div class="panel panel-default">
            <div class="panel-body" style="height: 380px;">
                <div class="form-group">
                    <label class="control-label">Parametro:</label>
                    <br/>
                    <asp:Label runat="server" ID="KeyName"></asp:Label>
                </div>
                <div class="form-group">
                    <label class="control-label">Note:</label>
                    <br/>
                    <asp:Label runat="server" ID="Note"></asp:Label>
                </div>
                <div class="form-group">
                    <label for="KeyValue" class="control-label">Valore:</label>
                    <textarea class="form-control" rows="6" runat="server" id="KeyValue"></textarea>
                </div>
            </div>
            <div class="pull-right" style="margin-top: 10px;">
                <asp:Button runat="server" ID="Close" Text="Torna" CssClass="btn btn-info" OnClick="Close_OnClick" />
                <asp:Button runat="server" ID="Save" Text="Salva" CssClass="btn btn-success" OnClick="Save_OnClick" />
            </div>
        </div>
    </form>
</body>
</html>
