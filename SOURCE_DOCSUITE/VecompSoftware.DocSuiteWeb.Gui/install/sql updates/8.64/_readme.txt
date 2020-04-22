Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- WebAPI 8.64
	- JeepService 8.64 
	- Modulo ResolutionWorkflowManager per il cliente ASL-TO
	- Modulo DocSeriesImporter per il cliente AGSM

#FL
######################################################################################################

La procedura di migrazione del database 8.63 prevede un ordine preciso di esecuzione degli script SQL. 
L'ordine da rispettare è il seguente:

01. ProtocolloDB_migrations.sql
02. PraticheDB_migrations.sql
03. AttiDB_migrations.sql

#FL
######################################################################################################

E' stato introdotto un nuovo parametro 'TemplateDocumentVisibilities' per la gestione della selezione dei depositi documentali (da gestire solo su richiesta).
Il parametro ha la seguente struttura json:

		[
			{
				"Name": "Collaboration",
				"VisibilityChains": {
					"MainChain": false,
					"AttachmentsChain": false,
					"AnnexedChain": false
				}
			}, 
			{
				"Name": "Desk",
				"VisibilityChains": {
					"MainChain": false
				}
			}
		]

Nella proprietà 'Name' viene indicato il modulo in cui attivare la funzionalità (in questo momento le possibilità sono:
	1 Collaboration = 'Collaborazione'
	2 Desk = 'Tavoli').
Nella sezione 'VisibilityChains' vanno indicate le specifiche catene in cui abilitare la funzionalità. I possibili valori inseribili sono:
			MainChain = (Documento principale, attivabile in Collaborazione e Tavoli)
			MainOmissisChain = (Documento principale omissis, attivabile in Collaborazione e utilizzabile se attivo il parametro ResolutionEnv.MainDocumentOmissisEnable)
			AttachmentsChain = (Allegati, attivabile in Collaborazione)
			AttachmentOmissisChain = (Allegati omissis, attivabile in Collaborazione e utilizzabile se attivo il parametro ResolutionEnv.AttachmentOmissisEnable)
			AnnexedChain = (Annessi, attivabile in Collaborazione)	

La funzionalità non si attiva se il modulo non viene impostato nel parametro o se tutte le catene non vengono specificate o vengono specificate a false.
#AC

#############################################################################
Nella DocSuite 8.62 è stata inserita la funzionalità degli Inserti dei Fascicoli che permette di allegare al fascicolo documentazione non protocollata p gestita all'interno di specifiche unità documentali.
Per una mancanza del readme non è stato configurato il parametro FascicleMiscellaneaLocation che necessità di un valore della location che punti ad un archivio standard in Biblos in cui salvare questi files.

L'archivio Biblos non ha requisiti Similmente a Collaborazioni è sato definito in ProtocolEnv il parametro TemplateDocumentRepositoryLocation dove value="<Il valore ID della tabella location deve essere corrispondente all'archivio della Gestione dei Template Documentali> " />

#FL
#############################################################################
Creazione di un nuovo modulo Gestione dei Template Documentali

Necessità di avere una location in DSW che punti ad un archivio standard in Biblos in cui salvare i templates per far funzionare il nuovo modulo in DSW.
Similmente a Collaborazioni è sato definito in ProtocolEnv il parametro TemplateDocumentRepositoryLocation dove value="<Il valore ID della tabella location deve essere corrispondente all'archivio della Gestione dei Template Documentali> " />

#CC
#############################################################################

E' stato modificato il file DocSuiteMenuConfig.json che contiente la configurazione del menù di DocSuite.
A tale file è stata aggiunta una nuova voce da aggiungere come figlia della voce 'Menu11':

	"FirstNode22": { "Name": "Deposito Documentale" } 

#AC
#############################################################################

L'archivio BiblosDs2010 che verrà utilizzato per la gestione del Deposito Documentale deve essere configurato nel seguente modo:

	- Attributo Filename -> Modify always
	- Attributo Signature -> Modify always

#AC
#############################################################################

[ASL-TO]

In questa versione sono state apportate sostanziali modifiche al flusso atti di Torino, in modo tale da rendere la pubblicazione web e l'esecutività automatiche.
Queste modifiche sono legate al parametro 'AutomaticActivityStepEnabled' e alla configurazione di un nuovo modulo del JeepService, chiamato 'ResolutionWorkflowManager'.
Il modulo processa delle attività riguardanti gli atti leggendo dalla tabella ResolutionActivities; per il momento le attività automatiche sono Pubblicazione Web ed Esecutività.
Per quanto riguarda la pubblicazione web, il modulo salva i documenti da pubblicare in una cartella e sposta i documenti andati in errore in un'altra cartella.
Per attivare il nuovo flusso atti automatico è necessario quindi

	1. Attivare il modulo del JS 'ResolutionWorkflowManager': sono da configurare i parametri
					- WebPublishNameFile -> valore del parametro DocSuite 'WebPublishHTMLFile'
					- WebPublishSign -> valore del parametro DocSuite 'WebPublishSign'
					- WebPublishSignTag -> valore del parametro DocSuite 'WebPublishSignTag'
					- DocumentFolderPath -> path della cartella dove vengono salvati i documenti da pubblicazione online
					- ErrorFolderPath -> path della cartella dove vengono spostati i documenti processati andati in errore
	   Questo modulo deve partire una sola volta alle ore 06:00 del mattino.
	   Nel caso di errori del modulo, viene inviata una mail ad assistenza e i documenti andati in errori vengono spostati nell'apposita cartella. Una volta gestito e risolto l'errore, il documento va cancellato manualmente dalla cartella.

       NB: Controllare che nella cartella Config del JS sia presente il file WebApi.Client.Config.Endpoints.json. 
	   In fase di sviluppo non è stato possibile testare la completa pubblicazione web sull'albo on-line, si prevede quindi il presidio di sviluppo dopo l'installazione del modulo da cliente.					

	2. Attivare il modulo del JS 'CleanFolders': configurazione il parametro FoldersPath con il path della cartella da svuotare -> deve essere la stessa configurata nel parametro DocumentFolderPath del modulo precedente.
	   Questo modulo deve partire prima del 'ResolutionWorkflowManager', poichè ha la responsabilità di svuotare la cartella dove vengono salvati i documenti processati correttamente. 
	   Deve quindi potrebbe partire alle ore 05:00. Assicurarsi che il modulo non parta in concomitanza dell'avvio del ResolutionWorkflowManager, in quanto potrebbe causare problemi di logia di funzionamento.

#SDC
#############################################################################

[ASL-TO]
Il parametro 'ErrorAutomaticActivitiesGroup' indica il grupppo di utenti che verrano avvisati, con un messaggio in home page, degli errori avvenuti nelle attività
di avanzamento automatico degli atti (responsabilità del Jeep Service). Per la configurazione di questo parametro dovrà essere il cliente ad indicare il gruppo
di utenti che vedrà il messaggio in home page di DocSuite. Sentire Andriola.

#SDC
#############################################################################

[ASL-TO]
Nel file DocSuiteMenuConfig.json va aggiunto nel Menu9 Delibere e Determine, la proprietà 

			 "FirstNode17": { "Name": "Controllo automatismi avanzamento flusso" }

che permette di visualizzare il nodo di menù 'Controllo automatismi avanzamento flusso', se sono attivi anche i parametri WebPublishEnabled e AutomaticActivityStepEnabled.

#SDC
#############################################################################

[ASL-TO]
Il file RicercaFlussoConfig.json deve essere così configurato:
 
{
  //Determina
  "0": {
    "0": "Adozione",
    "1": "Lettera di avvenuta Adozione",
    "9": "Invio Adozione Collegio Sindacale Firma Digitale",
    "10": "Pubblicazione",
    "6": "Esecutività",
    "11": "Ultima Pagina (Dematerializzazione) Firma Digitale"
  },
  //Delibera
  "1": {
    "0": "Adozione",
    "9": "Invio Adozione Collegio Sindacale Firma Digitale",
    "3": "Collegamento protocollo trasmissione agli Organi di Controllo",
    "10": "Pubblicazione",
    "6": "Esecutività",
    "11": "Ultima Pagina (Dematerializzazione) Firma Digitale"
  }
}			

#SDC
#############################################################################
[ASL-TO]

Rinominazione degli RDLC: provvedere a sostituiere i nomi dei seguenti file:

..\VecompSoftware.DocSuiteWeb.Gui\Report\Protocol\Letters_TrasmAdozioneCollegioSindacalefirmaDigitale.rdlc 
..\VecompSoftware.DocSuiteWeb.Gui\Report\Protocol\Letters_TrasmAdozioneCollegioSindacalefirmaDigitaleDetail.rdlc
..\VecompSoftware.DocSuiteWeb.Report\Templates\Resolution\Letters\Letters_TrasmAdozioneCollegioSindacalefirmaDigitale.rdlc 
con 

..\VecompSoftware.DocSuiteWeb.Gui\Report\Protocol\Letters_Del_TrasmAdozioneCollegioSindacalefirmaDigitale.rdlc
..\VecompSoftware.DocSuiteWeb.Gui\Report\Protocol\Letters_Del_TrasmAdozioneCollegioSindacalefirmaDigitaleDetail.rdlc
..\VecompSoftware.DocSuiteWeb.Report\Templates\Resolution\Letters\Letters_Del_TrasmAdozioneCollegioSindacalefirmaDigitale.rdlc 


Provvedere poi ad aggiungere, nella cartella:

..\VecompSoftware.DocSuiteWeb.Gui\Report\Protocol\

i file:

Letters_Det_TrasmAdozioneCollegioSindacalefirmaDigitale.rdlc
Letters_Det_TrasmAdozioneCollegioSindacalefirmaDigitaleDetail.rdlc

e nella cartella 

..\VecompSoftware.DocSuiteWeb.Report\Templates\Resolution\Letters\ 

il file 

Letters_Det_TrasmAdozioneCollegioSindacalefirmaDigitale.rdlc
Letters_Det_TrasmAdozioneCollegioSindacalefirmaDigitaleDetail.rdlc

#MM
#############################################################################
NOTA PER SVILUPPO, NO ASSISTENZA
Per ogni Archivio (UDS) presente in DB dal cliente è necessario verificare che siano
impostati i seguenti parametri nella definizione dell'XML dell'archivio stesso (colonna ModuleXML):
	- ProtocolEnabled
	- DocumentUnitSynchronizeEnabled
	- PECEnabled
	- WorkflowEnabled
	- CancelMotivationRequired
	- IncrementalIdentityEnabled

Es. 
<UnitaDocumentariaSpecifica xmlns="http://vecompsoftware.it/UnitaDocumentariaSpecifica.xsd" ProtocolEnabled="false" PECEnabled="false" DocumentUnitSynchronizeEnabled="true" CancelMotivationRequired="true" WorkflowEnabled="true" IncrementalIdentityEnabled="True">
	[...]
</UnitaDocumentariaSpecifica>

#AC
#############################################################################