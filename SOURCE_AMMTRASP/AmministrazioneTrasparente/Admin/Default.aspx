<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Admin.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AmministrazioneTrasparente.Admin.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="content" runat="server">
    <div class="jumbotron">
        <h2>Benvenuto nella sezione amministrativa del sito Amministrazione Trasparente</h2>
        <p>Nella pagina "Parametri" è possibile modificare tutti i parametri di funzionamento del sito in base alle necessità di configurazione.</p>
        <p>In basso è possibile visualizzare le novità introdotte in questa release.</p>
    </div>
    <hr/>
    <div class="panel panel-default">
        <div class="panel-heading">
            <h3 class="panel-title">Novità introdotte nella release</h3>
        </div>
        <div class="panel-body">
            <div class="col-lg-12">
                <h3><span class="glyphicon glyphicon-info-sign"></span>&nbsp;Nuova gestione migrazione.</h3>
                <p>Migliorata la gestione della migrazione DB.</p>    
            </div>
            <div class="col-lg-12">
                <h3><span class="glyphicon glyphicon-info-sign"></span>&nbsp;Layout.</h3>
                <p>Migliorata grafica sezione amministrativa.</p>    
            </div>
            <div class="col-lg-12">
                <h3><span class="glyphicon glyphicon-info-sign"></span>&nbsp;AVCP</h3>
                <p>Implementati stili per visualizzazione HTML dell'xml di AVCPIndex.</p>  
            </div>            
        </div>
    </div>
</asp:Content>
