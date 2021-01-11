1) Nuova sezione amministrativa per gestione parametri sito.
L'utenza di accesso di default è Administrator/Passw0rd.

################################################################

2) GoogleAnalyticsCode, il codice deve essere prima generato (da registrazione su sito google analytics) e successivamente inserito sul
sito di AmministrazioneTrasparente (sezione parametri). Da attivare per Piacenza deve essere univoco per ogni cliente.

################################################################

3) La gestione della signature con template sui documenti è una configurazione richiesta da Piacenza.
Il valore di default è : {0:container.name} n° {0:year}/{0:number} del {0:publishingdate|dd/MM/yyyy}.
Per la configurazione completa si veda documentazione relativa (https://vecompsw.sharepoint.com/Prodotti/docsuiteweb/_layouts/15/WopiFrame.aspx?sourcedoc={6E6C4D00-BAD6-4EA8-A447-D4D038AB0B26}&file=Signature%20documenti%20sito%20Amministrazione%20Trasparente.docx&action=default).

################################################################

4) E' stata implementata la possibilità di inserire temi custom in base alle esigenze del cliente andando a sovrascrive il layout base.
Attenzione! La modifica del layout è solo a livello di css, non è modificabile attualmente la struttura dei controlli.
L'installazione di un nuovo tema prevede i seguenti step:
	1) Creazione di una sotto-cartella in css/Themes con il nome del tema (es. AZOSPVR).
	2) Inserire tutti i css/immagini utilizzati per modificare il tema.
	3) Creare nella stessa directory il file di configurazione configuration.json che ha le seguenti caratteristiche:
		"[{
			"Priority" (priorità di gestione del file css, da 1 (prima posizione rispetto agli altri) a N) : "<valore>",
			"CssName" (nome del file css da utilizzare): "<nome del file>.css",
			"ExplorerVersion" (versione di Explorer da utilizzare come condizione di filtro, può non essere inserito): "<versione Explorer>",
			"Condition" (attivo se popolato il parametro ExplorerVersion, serve per indicare il filtro sulla versione di Explorer da applicare) : "<condizione" (Valori possibili: Less,
            Greater,
            LessOrEqual,
            GreaterOrEqual,
            Equals)
		 },
		 {<altro css da inserire>}]"
	4) Inserire nel parametro ThemeConfiguration il tema creato (es. AZOSPVR)

N.B. Utilizzando le condition verranno applicati dei filtri alle versione di Explorer coinvolte es. <!--[if lt IE 8]>. In questo caso il css verrà
applicato solo se si sta visualizzando il sito con versioni di explorer inferiori alla 8.

#################################################################

20141222
1) Parametro CustomFamilyLink per inserire link personalizzate nelle family. Formato da seguire: {id family di riferimento, testo da visualizzare, url}|...

#################################################################

20160201
1) E' stato eseguito un refactor generale del sito che comporta la rigenerazione del DB SQLite. Se si tratta di un aggiornamento e non di una nuova installazione procedere come segue:
	- Fare backup di tutto il sito da aggiornare.
	- Rinominare il file .db presente nella directory App_Data del sito in _AmministrazioneTrasparente.db
	- Aggiornare il sito e fare un start così che venga creato il nuovo .db nella directory App_Data.
	- Lanciare AmministrazioneTrasparenteMigrator.exe dalla directory di installazione e seguire le indicazioni (vecchio path, nuovo path).
	- Eliminare il file _AmministrazioneTrasparente.db

2) Aggiungere, se mancante, il seguente tag nel web.config dopo il tag <system.webServer>:
	<runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <dependentAssembly>
        <assemblyIdentity name="log4net" publicKeyToken="669e0ddf0bb1aa2a" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-1.2.15.0" newVersion="1.2.15.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Telerik.Web.UI" publicKeyToken="121fae78165ba3d4" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-2014.2.724.40" newVersion="2014.2.724.40" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Newtonsoft.Json" publicKeyToken="30ad4fe6b2a6aeed" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-4.5.0.0" newVersion="4.5.0.0" />
      </dependentAssembly>
    </assemblyBinding>
  </runtime>

Per qualsiasi dubbio chiedere a Sviluppo.

AC
###############################################################

Per mostrare un testo personalizzato sopra la ricerca di ogni serie documentale, configurare il file DocumentSeriesHeader.json
presente nella cartella config. Il file json deve avere la seguente struttura:

[
  {
    "IdSeries": 1,
    "Header": "Testo di prova 1"
  },
  {
    "IdSeries": 2,
	"IdSubSection": 2,
    "Header": "Testo di prova 2"
  }
]

Nella proprietà 'Id' va indicato l'id della DocumentSeries.
Nella proprietà 'Header' va indicato il testo da mostrare per quella specifica serie documentale.

Attenzione: se si inserisce un testo virgolettato, per essere deserializzato in modo corretto deve avere il seguente formato \" \". 
			Ad esempio
							"Header": "Testo di \"prova\" 1"

SDC
###############################################################