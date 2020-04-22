<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="AmministrazioneTrasparente.Error" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">
    <title>Errore</title>
    
    <link type="text/css" rel="stylesheet" href="./css/bootstrap.css" />
    <link href="./css/non-responsive.css" rel="stylesheet" />
    <link href="./css/custom.css" rel="stylesheet" />

    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="description" content="">
    <meta name="author" content="">
</head>
<body>
      <div class="navbar navbar-default navbar-fixed-top">
            <div class="container">
                <div class="navbar-header">
                    <a class="navbar-brand" href="./index.aspx"><img alt="Amministrazione Trasparente" src="./img/<%= LogoImg %>" style="height: 63px;" />&nbsp;Amministrazione Trasparente</a>
                </div>

            </div>
        </div>

    <div class="jumbotron">
       <p class="text-warning">Si è verificato un problema.</p>
        
       <p class="text-primary"><img class="img-thumbnail" src="img/screen_error.png" />L'elaborazione dei dati da Lei richiesta ha generato un errore sul Server. <br />Il problema è già stato segnalato agli amministratori del sito.</p>
        <p class="small text-muted">Per informazioni contattare <a href="mailto:<%= ContactMail %>"><%= ContactMail %></a></p>
    </div>
</body>
</html>
