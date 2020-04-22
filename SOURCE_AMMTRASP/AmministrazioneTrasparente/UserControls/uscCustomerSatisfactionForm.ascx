<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uscCustomerSatisfactionForm.ascx.cs" Inherits="AmministrazioneTrasparente.UserControls.CustomerSatisfactionForm" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock">
    <script src="https://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.3.1.min.js"></script>
    <script type="text/javascript">

        function unchecked(idChecked, firstIdUnchecked, secondIdUnchecked) {
            var checkBox = document.getElementById(idChecked);
            if (checkBox.checked == true) {
                document.getElementById(firstIdUnchecked).checked = false;
                if (secondIdUnchecked !== undefined)
                    document.getElementById(secondIdUnchecked).checked = false;
            }
        }

        function sendJson() {
            var formData = $('form').serializeArray();
            var i;
            for (i = 0; i < formData.length; i++) {
                formData[i].key = formData[i]['name'];
                delete formData[i].name;
            }
            formData = JSON.stringify(formData);

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "../CustomerSatisfaction.aspx/SendXml",
                data: "{'json': '" + formData + "'}",
                dataType: "json"
            });
        }

    </script>
</telerik:RadScriptBlock>

<form>

    <div class="FirstSection">
        <div>
            <h1 style="font-size: x-large; color: forestgreen;">Questionario di gradimento sulla sezione Amministrazione Trasparente</h1>
        </div>
        <hr>
        <div>
            <p>
                Gentile visitatore,<br>
                L’Azienda Usl di Reggio Emilia, in attuazione alla normativa vigente, ha pubblicato nella sezione Amministrazione trasparente del proprio sito internet diversi dati e informazioni, alcuni obbligatori per legge, altri ritenuti d’interesse dei cittadini.<br>
                Il questionario che viene proposto è anonimo ed ha la finalità sia di rilevare il gradimento dei visitatori su questa sezione del sito, sia di acquisire suggerimenti utili a migliorare l’accessibilità e la fruibilità delle informazioni.<br>
                Il questionario richiede solo alcuni minuti di attenzione, l’esito dell’elaborazione in forma aggregata dei dati sarà pubblicata nella sezione Amministrazione trasparente periodicamente alla voce “<em>Altri contenuti</em>”.
            </p>
            <p>Grazie per la collaborazione</p>
            <p>&nbsp;</p>
            <p><strong>I campi contrasseganti con <span style="color: #FF0000;">*</span> sono obbligatori</strong></p>
        </div>
        <p>&nbsp;</p>
        <div>
            <div class="col-md-6">
                <p>
                    Lavora o collabora con una struttura sanitaria? <span style="color: #FF0000;">*</span>
                </p>
            </div>
            <div class="col-md-6">
                <div class="col-md-3">
                    <input id="sanitariaSi" onclick="unchecked('sanitariaSi', 'sanitariaNo')" type="radio" value="Si" name="lavoraConSanitaria" required />
                    <label style="font-weight: normal">Sì</label>
                </div>
                <div class="col-md-3">
                    <input id="sanitariaNo" onclick="unchecked('sanitariaNo', 'sanitariaSi')" type="radio" value="No" name="lavoraConSanitaria" required />
                    <label style="font-weight: normal">No</label>
                </div>
            </div>
        </div>
        <p>&nbsp;</p>
        <div>
            <div class="col-md-6">
                <p>
                    Consulta prevalentemente Amministrazione trasparente dell’AUSL di Reggio Emilia per: <span style="color: #FF0000;">*</span>
                </p>
            </div>
            <div class="col-md-6">
                <div class="col-md-5">
                    <input id="first" onclick="unchecked('first', 'second', 'third')" type="radio" value="personale" name="consultaAmministrazionePer" required />
                    <label style="font-weight: normal">acquisire informazioni ad uso personale</label>
                </div>
                <div class="col-md-5">
                    <input id="second" onclick="unchecked('second', 'first', 'third')" type="radio" value="professionale" name="consultaAmministrazionePer" required />
                    <label style="font-weight: normal">acquisire informazioni ad uso professionale</label>
                </div>
                <div class="col-md-2">
                    <input id="third" onclick="unchecked('third', 'first', 'second')" type="radio" value="altro" name="consultaAmministrazionePer" required />
                    <label style="font-weight: normal">altro</label>
                </div>
            </div>
        </div>
        <p>&nbsp;</p>
        <div>
            <div class="col-md-6">
                <p>
                    Come è venuto a conoscenza di questa sezione del sito internet dell’AUSL di Reggio Emilia? <span style="color: #FF0000;">*</span>
                </p>
            </div>
            <div class="col-md-6">
                <select name="conoscenzaDiQuestaDelSito" class="col-md-12" required>
                    <option value="" disabled selected>- Scegliere -</option>
                    <option value="1">ho trovato l’indirizzo sui mezzi di comunicazione (es. stampa)</option>
                    <option value="2">attraverso motori di ricerca</option>
                    <option value="3">attraverso un link da un altro sito</option>
                    <option value="4">attraverso indicazione diretta da parte di personale dell’AUSL di Reggio Emilia</option>
                    <option value="5">attraverso suggerimento di un conoscente</option>
                    <option value="6">altro</option>
                </select>
            </div>
        </div>
        <p>&nbsp;</p>
        <div>
            <div class="col-md-6">
                <p>
                    Con quale frequenza consulta questa sezione? <span style="color: #FF0000;">*</span>
                </p>
            </div>
            <div class="col-md-6">
                <select name="frequenzaConsulta" class="col-md-12" required>
                    <option value="" disabled selected>- Scegliere -</option>
                    <option value="1">tutti i giorni</option>
                    <option value="2">una o più volte a settimana</option>
                    <option value="3">una o più volte al mese</option>
                    <option value="4">sporadicamente</option>
                    <option value="5">è la prima volta</option>
                </select>
            </div>
        </div>
        <p>&nbsp;</p>
        <div>
            <div class="col-md-6">
                <p>
                    Per quale motivo ha consultato questa sezione? <span style="color: #FF0000;">*</span>
                </p>
            </div>
            <div class="col-md-6">
                <select name="motivoHaConsultato" class="col-md-12" required>
                    <option value="" disabled selected>- Scegliere -</option>
                    <option value="1">cercavo informazioni</option>
                    <option value="2">cercavo un documento</option>
                    <option value="3">per stampare modulistica</option>
                    <option value="4">per caso</option>
                    <option value="5">altro</option>
                </select>
            </div>
        </div>
        <p>&nbsp;</p>
        <div>
            <div class="col-md-6">
                <p>
                    La consultazione di questa sezione ha favorito l’accesso alle informazioni che cercava? <span style="color: #FF0000;">*</span>
                </p>
            </div>
            <div class="col-md-6">
                <select name="favoritoAccessoAlleInformazioni" class="col-md-12" required>
                    <option value="" disabled selected>- Scegliere -</option>
                    <option value="1">per niente</option>
                    <option value="2">poco</option>
                    <option value="3">abbastanza</option>
                    <option value="4">molto</option>
                </select>
            </div>
        </div>

    </div>

    <p>&nbsp;</p>
    <hr>

    <div class="secondSection">

        <div>
            <div class="col-md-12">
                <p>
                    La sezione Amministrazione trasparente del sito dell’Ausl di Reggio Emilia secondo lei è:
                </p>
            </div>
            <p>&nbsp;</p>
            <div>
                <div class="col-md-6">
                    <p>
                        Chiara nel linguaggio
                    </p>
                </div>
                <div class="col-md-6">
                    <div class="col-md-3">
                        <input id="linguaggioSi" type="radio" onclick="unchecked('linguaggioSi', 'linguaggioNo')" value="Si" name="chiaraNelLinguaggio" />
                        <label style="font-weight: normal">Sì</label>
                    </div>
                    <div class="col-md-3">
                        <input id="linguaggioNo" type="radio" onclick="unchecked('linguaggioNo', 'linguaggioSi')" value="No" name="chiaraNelLinguaggio" />
                        <label style="font-weight: normal">No</label>
                    </div>
                </div>
            </div>
            <p>&nbsp;</p>
            <div>
                <div class="col-md-6">
                    <p>
                        Chiara nei contenuti
                    </p>
                </div>
                <div class="col-md-6">
                    <div class="col-md-3">
                        <input id="contenutiSi" onclick="unchecked('contenutiSi', 'contenutiNo')" type="radio" value="Si" name="chiaraNeiContenuti" />
                        <label style="font-weight: normal">Sì</label>
                    </div>
                    <div class="col-md-3">
                        <input id="contenutiNo" onclick="unchecked('contenutiNo', 'contenutiSi')" type="radio" value="No" name="chiaraNeiContenuti" />
                        <label style="font-weight: normal">No</label>
                    </div>
                </div>
            </div>
            <p>&nbsp;</p>
            <div>
                <div class="col-md-6">
                    <p>
                        Semplice nella ricerca delle informazioni
                    </p>
                </div>
                <div class="col-md-6">
                    <div class="col-md-3">
                        <input id="ricercaSi" onclick="unchecked('ricercaSi', 'ricercaNo')" type="radio" value="Si" name="sempliceNellaRicerca" />
                        <label style="font-weight: normal">Sì</label>
                    </div>
                    <div class="col-md-3">
                        <input id="ricercaNo" onclick="unchecked('ricercaNo', 'ricercaSi')" type="radio" value="No" name="sempliceNellaRicerca" />
                        <label style="font-weight: normal">No</label>
                    </div>
                </div>
            </div>
            <p>&nbsp;</p>
            <div>
                <div class="col-md-6">
                    <p>
                        Facile per reperire moduli e documenti
                    </p>
                </div>
                <div class="col-md-6">
                    <div class="col-md-3">
                        <input id="documentiSi" onclick="unchecked('documentiSi', 'documentiNo')" type="radio" value="Si" name="facilePerReperire" />
                        <label style="font-weight: normal">Sì</label>
                    </div>
                    <div class="col-md-3">
                        <input id="documentiNo" onclick="unchecked('documentiNo', 'documentiSi')" type="radio" value="No" name="facilePerReperire" />
                        <label style="font-weight: normal">No</label>
                    </div>
                </div>
            </div>
            <p>&nbsp;</p>
            <div>
                <div class="col-md-6">
                    <p>
                        Completa nelle informazioni
                    </p>
                </div>
                <div class="col-md-6">
                    <div class="col-md-3">
                        <input id="informazioniSi" onclick="unchecked('informazioniSi', 'informazioniNo')" type="radio" value="Si" name="completaNelleInformazioni" />
                        <label style="font-weight: normal">Sì</label>
                    </div>
                    <div class="col-md-3">
                        <input id="informazioniNo" onclick="unchecked('informazioniNo', 'informazioniSi')" type="radio" value="No" name="completaNelleInformazioni" />
                        <label style="font-weight: normal">No</label>
                    </div>
                </div>
            </div>
            <p>&nbsp;</p>
            <div>
                <div class="col-md-6">
                    <p>
                        Veloce nello scaricare allegati
                    </p>
                </div>
                <div class="col-md-6">
                    <div class="col-md-3">
                        <input id="allegatiSi" onclick="unchecked('allegatiSi', 'allegatiNo')" type="radio" value="Si" name="veloceNelloScaricare" />
                        <label style="font-weight: normal">Sì</label>
                    </div>
                    <div class="col-md-3">
                        <input id="allegatiNo" onclick="unchecked('allegatiNo', 'allegatiSi')" type="radio" value="No" name="veloceNelloScaricare" />
                        <label style="font-weight: normal">No</label>
                    </div>
                </div>
            </div>
            <p>&nbsp;</p>
            <div>
                <div class="col-md-6">
                    <p>
                        Chiara nella grafica
                    </p>
                </div>
                <div class="col-md-6">
                    <div class="col-md-3">
                        <input id="graficaSi" onclick="unchecked('graficaSi', 'graficaNo')" type="radio" value="Si" name="chiaraNellaGrafica" />
                        <label style="font-weight: normal">Sì</label>
                    </div>
                    <div class="col-md-3">
                        <input id="graficaNo" onclick="unchecked('graficaNo', 'graficaSi')" type="radio" value="No" name="chiaraNellaGrafica" />
                        <label style="font-weight: normal">No</label>
                    </div>
                </div>
            </div>
            <p>&nbsp;</p>
            <div>
                <div class="col-md-6">
                    <p>
                        Come giudica nel complesso la qualità di questa sezione? <span style="color: #FF0000;">*</span>
                    </p>
                </div>
                <div class="col-md-6">
                    <select name="qualitàDiQuestaSezione" class="col-md-12" required>
                        <option value="" disabled selected>- Scegliere -</option>
                        <option value="1">scarsa</option>
                        <option value="2">discreta</option>
                        <option value="3">buona</option>
                        <option value="4">ottima</option>
                    </select>
                </div>
            </div>
            <p>&nbsp;</p>
            <div>
                <div class="col-md-12">
                    <p>
                        Suggerimenti per migliorare la sezione Amministrazione trasparente del sito dell’AUSL di Reggio Emilia
                    </p>
                </div>
                <div class="col-md-12">
                    <textarea name="suggerimentiPerMigliorare" class="col-md-12" rows="6"></textarea>
                </div>
            </div>
        </div>
    </div>

    <hr>

    <div class="thirdSection">

        <div class="col-md-12">
            <p>
                Dati anagrafici a fini statistici (facoltativi)
            </p>
        </div>
    </div>
    <p>&nbsp;</p>
    <div>
        <div class="col-md-6">
            <p>
                Genere
            </p>
        </div>
        <div class="col-md-6">
            <div class="col-md-3">
                <input id="uomoId" onclick="unchecked('uomoId', 'donnaId')" type="radio" value="Uomo" name="genere" />
                <label style="font-weight: normal">Uomo</label>
            </div>
            <div class="col-md-3">
                <input id="donnaId" onclick="unchecked('donnaId', 'uomoId')" type="radio" value="Donna" name="genere" />
                <label style="font-weight: normal">Donna</label>
            </div>
        </div>
    </div>
    <p>&nbsp;</p>
    <div>
        <div class="col-md-6">
            <p>
                Fascia di età
            </p>
        </div>
        <div class="col-md-6">
            <select name="fasciaDiEtà" class="col-md-12">
                <option value="" disabled selected>- Nessuno -</option>
                <option value="1">meno di 18 anni</option>
                <option value="2">18 – 30 anni</option>
                <option value="3">31 – 40 anni</option>
                <option value="4">41 – 65 anni</option>
                <option value="5">oltre 65 anni</option>
            </select>
        </div>
    </div>
    <p>&nbsp;</p>
    <div>
        <div class="col-md-6">
            <p>
                Attuale occupazione
            </p>
        </div>
        <div class="col-md-6">
            <select name="attualeOccupazione" class="col-md-12">
                <option value="" disabled selected>- Nessuno -</option>
                <option value="1">dipendente pubblico</option>
                <option value="2">dipendente privato</option>
                <option value="3">imprenditore/libero professionista</option>
                <option value="4">commerciante/artigiano</option>
                <option value="5">casalinga</option>
                <option value="6">pensionato</option>
                <option value="7">studente</option>
                <option value="8">in attesa di occupazione</option>
                <option value="9">altro</option>
            </select>
        </div>
    </div>
    <p>&nbsp;</p>
    <div>
        <div class="col-md-6">
            <p>
                Il suo titolo di studio
            </p>
        </div>
        <div class="col-md-6">
            <select name="titoloDiStudio" class="col-md-12">
                <option value="" disabled selected>- Nessuno -</option>
                <option value="1">licenza elementare o nessun titolo</option>
                <option value="2">licenza media</option>
                <option value="3">diploma scuola media superiore</option>
                <option value="4">diploma universitario</option>
                <option value="5">laurea o laurea con specializzazione</option>
            </select>
        </div>
    </div>
    <p>&nbsp;</p>
    <div>
        <div class="col-md-6">
            <p>
                Dove risiede
            </p>
        </div>
        <div class="col-md-6">
            <select name="doveRisiede" class="col-md-12">
                <option value="" disabled selected>- Nessuno -</option>
                <option value="1">nella provincia di Reggio Emilia</option>
                <option value="2">in altra provincia dell’Emilia Romagna</option>
                <option value="3">in altra regione italiana</option>
                <option value="4">in altro Stato</option>
            </select>
        </div>
    </div>
    <p>&nbsp;</p>
    <input type="Submit" id="submitBtn" onclick="sendJson()" class="btn btn-success" value="Invia" />
    <p>&nbsp;</p>
</form>
