Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- WebAPI 8.65
	- ServiceBus Listeners 8.65 
	- BiblosDS 8.65

#FL
############################################################################################################################################################################################################

La procedura di migrazione del database prevede un ordine preciso di esecuzione degli script SQL. 
L'ordine da rispettare è il seguente:

01. ProtocolloDB_migrations.sql
02. PraticheDB_migrations.sql
03. AttiDB_migrations.sql

#FL
######################################################################################################
Per ***REMOVED*** è stata introdotta una nuova funzionalità per gestire l'ordinamento dei documenti di una
Serie Documentale.
Per il corretto funzionamento è necessario che venga aggiunto un nuovo attributo agli archivi di 
BiblosDS con nome "Position" di tipo intero e con modalità "Modify Always". L'attributo deve essere 
associato all'attribute group relativo al documento (non required)

Si consiglia di aggiungere tale attributo per tutti i clienti.

#FL
######################################################################################################
Il file Docsuitenamespace_Entities_8.65.xml contiene l'export delle ultime configurazioni delle code e dei topic del service bus della DocSuite.
Per aggiorarle è necessario aprire il Tool Service Bus Explorer, specificare la connectiostring del s1ervice bus del cliente e dal menu Actions -> Import Entries selezionare
il file Docsuitenamespace_Entities_8.65.xml. Automaticamente eseguirà la configurazione, sovvrasvrivendo l'esistenza ma senza perdere eventuali messaggi presenti nelle code/eventi.

Se da errori in fase di import, si consiglia di cancellare le code/topic che hanno dato l'errore. 
#FL
###############################################################
E' stata aggiunta la colonna DSWEnvironment alla tabella WrokflowRepositories.
Di default è stato messo il valore 0.
Bisogna modificare a mano per ogni record della tabella il campo DSWEnvironment col valore numerico corretto.
I valori possibili sono:

    Any = 0,
    Protocol = 1,
    Resolution = 2,
    Document = 3,
    DocumentSeries = 4,
    Desk = 5,
    Workflow = 6,
    UDS = 7,
    Fascicle = 8,
    Dossier = 9

#SZ
######################################################################################################
E' stato modificato il file WebApi.Client.Config.Endpoints.json che contiente la configurazione degli endpoint utilizzati da DocSuite
per comunicare con le WebAPI.
Assicurarsi di copiarlo in produzione col file presente nella directory Config di DocSuite.

#IS
######################################################################################################
Creazione Nuovo Modulo Dossier

Necessità di avere una location in DSW che punti ad un archivio standard di pratiche in Biblos in cui salvare gli inserti dei Dossier per far funzionare il nuovo modulo in DSW.
E' stato definito in ProtocolEnv il parametro DossierMiscellaneaLocation che definisce l'ID della location da utilizzare per l'archiviazione degli inserti dei dossier. 

#IS
######################################################################################################
Sono stati aggiunti i seguenti Endpoint da aggiungere al TenantModel nella sezione Entities:

         		"Dossier": {
         			"IsActive": true,
         			"Timeout": "00:00:30",
         			"ODATAControllerName": "Dossiers"
         		},
         		"DossierFolder": {
         			"IsActive": true,
         			"Timeout": "00:00:30",
         			"ODATAControllerName": "DossierFolderss"
         		},
         		"DossierDocument": {
         			"IsActive": true,
         			"Timeout": "00:00:30",
         			"ODATAControllerName": "DossierDocuments"
         		},
         		"DossierLog": {
         			"IsActive": true,
         			"Timeout": "00:00:30",
         			"ODATAControllerName": "DossierLogs"
         		}

Vedi gli esempi di configurazione in ExampleMultiTenant_8.65.json e ExampleOneTenant_8.65.json

#IS
######################################################################################################
[AUSL-RE]
E' stato aggiunto il parametro MoveWorkflowDeskToCollaboration per spostare la visibilità del menu di riepilogo
delle attività di workflow nel menù delle collaborazioni invece che nel menù utente. il comportamento
del parametro è così regolato:
        [AUSL-RE = true]
        Se = true la vista di riepilogo delle attività di workflow sarà nello stesso menù di collaborazioni.
        Se = false la vista di riepilogo delle attività di workflow sarà nel menù utente.

#MM
######################################################################################################
[AUSL-RE]
Si è introdotto un nuovo parametro chiamato "FasciclesPanelVisibilities" che governa la visibilità dei pannelli degli inserti e dei fascicoli collegati nel Sommario dei Fascicoli.
La struttura del parametro in configurazione di Default è la seguente:

{
	"InsertsPanelVisibility": true,
	"FasciclesLinkedPanelVisibility": true,
	"GridSearchPanelVisibility": true,
	"FascicleNameVisibility": true,
	"FascicleRacksVisibility": true
}

- Per [AUSL-RE] il parametro va configurato impostando a false i valori relativi a "InsertsPanelVisibility" e "FasciclesLinkedPanelVisibility" nel seguente modo:

{
	"InsertsPanelVisibility": false,
	"FasciclesLinkedPanelVisibility": false,
	"GridSearchPanelVisibility": false,
	"FascicleNameVisibility": false,
	"FascicleRacksVisibility": false
}

Per impostare il parametro per [AUSL-RE] copiare la precedente configurazione e incollarla nella casella di testo del parametro "FasciclesPanelVisibilities".

#IS.
######################################################################################################
Aggiornato il File di configurazione WorkflowOperationConfig.Json, provvedere ad aggiornarlo.

#MM
######################################################################################################
E' stato aggiornato il file di configurazione DocSuiteMenuConfig.json.

#FL
######################################################################################################