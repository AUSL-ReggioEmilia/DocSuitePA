<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditPassword.aspx.cs" Inherits="AmministrazioneTrasparente.Admin.EditPassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta name="robots" content="noindex">
    <link href="../css/bootstrap.min.css" rel="stylesheet" />
    <!--[if lt IE 8]>
    <link href="../css/bootstrap-ie7.css" rel="stylesheet" />
        <style>
            .form-control {
                margin-left: 0;
                width: 90%;
            }
        .panel-body {
            height: 90px !important;
        }
        </style>
    <![endif]-->
    <!-- HTML5 shim and Respond.js for IE8 support of HTML5 elements and media queries -->
    <!--[if lt IE 9]>
      <script src="../js/html5shiv.min.js"></script>
      <script src="../js/respond.min.js"></script>
    <![endif]-->
</head>
<body>
    <form id="form1" runat="server" style="padding-top: 30px;">
        <script type="text/javascript">
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;

                return oWindow;
            }
            function UpdateConfirm() {
                alert("Password modificata correttamente.");
                GetRadWindow().close();
            }
            function CloseEdit() {
                GetRadWindow().close();
            }
        </script>
        <div class="panel panel-default">
            <div class="panel-body" style="height: 100px;">
                <div class="form-group">
                    <label for="Name" class="control-label">Nuova Password:</label>
                    <asp:TextBox CssClass="form-control" runat="server" ID="newPassword" TextMode="Password"></asp:TextBox>
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
