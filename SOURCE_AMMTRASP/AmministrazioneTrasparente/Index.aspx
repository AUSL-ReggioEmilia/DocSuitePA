<%@ Page Language="C#" MasterPageFile="~/MasterPages/DocumentSeries.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="AmministrazioneTrasparente.Index" %>

<asp:Content ContentPlaceHolderID="MainPlaceHolder" runat="server">
    <div class="well">
        <p class="text-center">
            <img src="./img/Amministrazione_Trasparente.png" class="img-thumbnail" />
        </p>

        <div class="media">
            <div class="media-left">
                <a href="http://www.normattiva.it" target="blank" class="pull-left" style="margin-right: 20px">
                    <img src="img/normattiva.png" alt="Normattiva" class="img-thumbnail" />
                </a>
            </div>
            <div class="media-body">
                <p>
                    Le pubbliche amministrazioni sono tenute, ai sensi del D.Lgs 33 del 14/03/2013, a pubblicare dati riguardanti il loro assetto organizzativo, procedure adottate e prestazioni rese. 
                </p>
                <p class="">
                    In attesa del completamento e dell'inserimento dei dati richiesti dal decreto legislativo, le informazioni attualmente non disponibili su sito possono essere richieste all'indirizzo di posta elettronica <a href="mailto:<%= ContactMail %>"><%= ContactMail %></a>
                </p>
                <p class="text-muted">Questa sezione del sito è in fase di implementazione e integrazione</p>
            </div>
        </div>
    </div>
    <div class="well">
        <h2>Istruzioni per l'utente
        </h2>
        <p>
            Per iniziare a consultare i dati selezionare una serie documentale nel menu di sinistra.
        </p>
        <p>
            I dati sono organizzati in sezioni di primo e secondo livello.
        </p>
        <p>
            All'interno delle sezioni di secondo livello è possibile fare ricerche e consultazione delle pubblicazioni.
        </p>
        <p>
            La normativa vigente richiamata nelle diverse sezioni di questo sito è consultabile nel portale <a href="http://www.normattiva.it">www.normattiva.it</a>
        </p>
    </div>

    <asp:Label ID="lblStatistics" runat="server">
    <div class="well">
        <h4><a href="Statics.aspx">Statistiche di accesso</a></h4>
    </div>
    </asp:Label>
</asp:Content>
